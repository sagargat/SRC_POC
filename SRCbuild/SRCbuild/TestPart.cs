using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace SRCbuild
{
    [TestClass]
    public class TestPart
    {

        #region variable decalreation
        private IWebDriver driver;
        private TestContext testContextInstance;
        #endregion

        #region privateMethods
        private void InitializeBrowser()
        {

            ChromeOptions options = new ChromeOptions();
            // options.AddArgument("--start-maximized");
            driver = new ChromeDriver();

        }

        private void OpenURL()
        {
            // Initialize Browser
            InitializeBrowser();
            driver.Navigate().GoToUrl("http://srcbuild:9000/");
            driver.Manage().Window.Maximize();
            //Action.
        }
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }


        [TestCleanup()]
        public void MyTestCleanup()
        {
            try
            {
                // Stop the web driver
                driver.Close();
                driver.Quit();
            }
            catch (Exception)
            {
            }
        }
        #endregion
        
        #region TestCases
        [TestMethod]
        public void IsOpeningURL()
        {
            OpenURL();

            // Initialize wait instance
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Verify we are navigating to correct page by verifying User Name on Login page
            IWebElement UserName = wait.Until(d => d.FindElement(By.XPath("//*[@id='one23-Header']/ul/li[2]/a/span[2]")));
            Assert.AreEqual<String>("Bob Miller", UserName.Text);

        }

        [TestMethod]
        public void NavigateToUOMList()
        {

            OpenURL();
            //Navigate to Setting 
            driver.FindElement(By.XPath("//*[@id='accordion']/div[4]/a/div/span[2]")).Click();

            // Wait to open Menu and load elements
            Task.Delay(2000).Wait();

            // Navigate to Setting > UOM
            driver.FindElement(By.XPath("//*[@id='collapseFour']/div/div/a[4]")).Click();

            // Ensure accessing correct Page bu compairing Heading 
            IWebElement PageName = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div[1]/ul/li"));
            Assert.AreEqual<String>("UNIT OF MEASURES", PageName.Text);

        }


        //[TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
                    "|DataDirectory|\\UOMData.csv",
                    "UOMData#csv",
                    DataAccessMethod.Sequential),
            // DeploymentItem("UOMData.csv"),
        TestMethod]
        public void CreateUOM()
        {

            string UOMName = testContextInstance.DataRow[0].ToString();
            string UOMPrecision = testContextInstance.DataRow[1].ToString();

            NavigateToUOMList();

       
            // Click On Create UOM Button to open Slider by using Xpath
                    Task.Delay(2000).Wait();
                    driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[4]/div/span[2]/a")).Click();

                    // Provide input to UOM Name
                    Task.Delay(2000).Wait();
                    driver.FindElement(By.Name("name")).SendKeys(UOMName);

                    // Provide input to Presision by using Name
                    driver.FindElement(By.Name("precision")).Clear();
                    driver.FindElement(By.Name("precision")).SendKeys(UOMPrecision);

                    // Click On Create Button by using Class Name
                    Task.Delay(2000).Wait();
                    driver.FindElement(By.ClassName("one23-ActionButton")).Click();

                    // Ensure UOM Created 
                    Task.Delay(2000).Wait();
                    IWebElement UOMBreadcrum = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div/ul/li[2]/span"));
                    Assert.AreEqual<String>(UOMName.ToUpper(), UOMBreadcrum.Text);

        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
                   "|DataDirectory|\\UOMValidation.csv",
                   "UOMValidation#csv",
                   DataAccessMethod.Sequential),
            // DeploymentItem("UOMData.csv"),
       TestMethod]
        public void CheckValidationsOnUOM()
        {

            string UOMName = testContextInstance.DataRow[0].ToString();
            string UOMPrecision = testContextInstance.DataRow[1].ToString();
            string expectedErrorForName = testContextInstance.DataRow[2].ToString();
            string expectedErrorForPrecision = testContextInstance.DataRow[3].ToString();

            NavigateToUOMList();

            // Click On Create UOM Button to open Slider by using Xpath
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[4]/div/span[2]/a")).Click();

            // Provide input to UOM Name
            Task.Delay(2000).Wait();
            driver.FindElement(By.Name("name")).SendKeys(UOMName);

            // Ensure Name should through error as  (Special characters not allowed)
            Task.Delay(2000).Wait();
            IWebElement ActualldErrorName = driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[1]/span[2]"));
            Assert.AreEqual<String>(expectedErrorForName, ActualldErrorName.Text);

            // Provide input to Presision by using Name
            driver.FindElement(By.Name("precision")).Clear();
            driver.FindElement(By.Name("precision")).SendKeys(UOMPrecision);


            // Ensure Presision should through error as  (enter value between 0 to 6)
            Task.Delay(2000).Wait();
            IWebElement ActualldErrorPrecision = driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[2]/span[2]"));
            Assert.AreEqual<String>(expectedErrorForPrecision, ActualldErrorPrecision.Text);

        }

        [TestMethod]
        public void CheckDuplicateUOM()
        {

            NavigateToUOMList();

            // Collecting already created UOM list
            Task.Delay(2000).Wait();
            ReadOnlyCollection<IWebElement> links = driver.FindElements(By.XPath("//a[starts-with(@href,'#/UnitOfMeasureDetail/')]"));
            var UOMCount = links.Count;
            if (UOMCount == 0)
            {
                string NewUOMName = "Each";
                string NewUOMPrecision = "0";

                // Click On Create UOM Button to open Slider by using Xpath
                Task.Delay(2000).Wait();
                driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[4]/div/span[2]/a")).Click();

                // Provide input to UOM Name
                Task.Delay(2000).Wait();
                driver.FindElement(By.Name("name")).SendKeys(NewUOMName);

                // Provide input to Presision by using Name
                driver.FindElement(By.Name("precision")).Clear();
                driver.FindElement(By.Name("precision")).SendKeys(NewUOMPrecision);

                // Click On Create Button by using Class Name
                Task.Delay(1000).Wait();
                driver.FindElement(By.ClassName("one23-ActionButton")).Click();

                Task.Delay(5000).Wait();
                driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div/ul/li[1]/a")).Click();
            }

            string UOMName = links[0].Text;
            string UOMPrecision = "4";

            // Click On Create UOM Button to open Slider by using Xpath
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[4]/div/span[2]/a")).Click();

            // Provide input to UOM Name
            Task.Delay(2000).Wait();
            driver.FindElement(By.Name("name")).SendKeys(UOMName);

            // Provide input to Presision by using Name
            driver.FindElement(By.Name("precision")).Clear();
            driver.FindElement(By.Name("precision")).SendKeys(UOMPrecision);

            // Click On Create Button by using Class Name
            Task.Delay(2000).Wait();
            driver.FindElement(By.ClassName("one23-ActionButton")).Click();

            // Check Validation Message
            Task.Delay(2000).Wait();
            IWebElement UOMMessage = driver.FindElement(By.XPath("/html/body/div[3]/div/h4"));
            Assert.AreEqual<String>("Error", UOMMessage.Text);
        }



        [TestMethod]
        // Create location by direct navigates to Location List
        public void CreateLocationByDirectLink()
        {
            var LocationName = "Main";
            var LocationDesc = "Main Storage";
            InitializeBrowser();
            // Navigat to Location List
            driver.Navigate().GoToUrl("http://localhost:1000/module/setting/#/LocationList");

            // Wait to open Page and load elements
            Task.Delay(2000).Wait();

            // Click On Create Location Button to open Slider 
            driver.FindElement(By.ClassName("one23-ActionButton")).Click();

            // Wait to open slider and load elements
            Task.Delay(2000).Wait();

            // Provide input to Location Code
            driver.FindElement(By.Name("code")).SendKeys(LocationName);

            // Provide input to Description 
            driver.FindElement(By.Name("description")).SendKeys(LocationDesc);

            // // Click On Toggle Button for sending QUARANTINED as Yes
            //driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div/div/span[3]")).Click();

            ////// Click On Toggle Button for sending MRP EXCLUDED as Yes
            ////driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/div/div/span[2]")).Click();

            // Click On Create Button 
            Task.Delay(2000).Wait();
            driver.FindElement(By.ClassName("one23-ActionButton")).Click();

            // Ensure Location Created 
            Task.Delay(2000).Wait();
            IWebElement LocationBreadcrum = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div/ul/li[2]/span"));
            Assert.AreEqual<String>(LocationName.ToUpper(), LocationBreadcrum.Text);
        }

        [TestMethod]
        public void NavigateToLocationList()
        {

            OpenURL();
            //Navigate to Setting 
            driver.FindElement(By.XPath("//*[@id='accordion']/div[4]/a/div/span[2]")).Click();

            // Wait to open Menu and load elements
            Task.Delay(2000).Wait();

            // Navigate to Setting > Location  
            driver.FindElement(By.XPath("//*[@id='collapseFour']/div/div/a[2]")).Click();

            // Ensure accessing correct Page bu compairing Heading 
            Task.Delay(1000).Wait();
            IWebElement PageName = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div[1]/ul/li"));
            Assert.AreEqual<String>("LOCATIONS", PageName.Text);

        }


        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
                   "|DataDirectory|\\LocationData.csv",
                   "LocationData#csv",
                   DataAccessMethod.Sequential),
            // DeploymentItem("UOMData.csv"),
       TestMethod]
        public void CreateLocation()
        {
            string LocationCode = testContextInstance.DataRow[0].ToString();
            string LocationDescription = testContextInstance.DataRow[1].ToString();
            string Quarentined = testContextInstance.DataRow[2].ToString();
            string MRPExcluded = testContextInstance.DataRow[3].ToString();

            NavigateToLocationList(); 
            
                    // Click On Create Location Button to open Slider 
                    Task.Delay(2000).Wait();
                    driver.FindElement(By.ClassName("one23-ActionButton")).Click();

                    // Wait to open slider and load elements
                    Task.Delay(2000).Wait();

                    // Provide input to Location Code
                    driver.FindElement(By.Name("code")).SendKeys(LocationCode);

                    // Provide input to Description 
                    driver.FindElement(By.Name("description")).SendKeys(LocationDescription);


                    if (Quarentined.Equals("1"))
                    {
                        // Click On Toggle Button for sending QUARANTINED as Yes
                        driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div/div/span[3]")).Click();
                    }

                    if (MRPExcluded.Equals("1"))
                    {
                        // Click On Toggle Button for sending MRP EXCLUDED as Yes
                        driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/div/div/span[2]")).Click();
                    }
                    // Click On Create Button 
                    Task.Delay(2000).Wait();
                    driver.FindElement(By.ClassName("one23-ActionButton")).Click();

                    // Ensure Location Created 
                    Task.Delay(2000).Wait();
                    IWebElement LocationBreadcrum = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div/ul/li[2]/span"));
                    Assert.AreEqual<String>(LocationCode.ToUpper(), LocationBreadcrum.Text);

        }

        [TestMethod]
        public void NavigateToPartList()
        {

            OpenURL();
            //Navigate to Engineering 
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='accordion']/div[1]/a/div/span[2]")).Click();


            // Navigate to Engineering > Parts
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='collapseOne']/div/div/a/span")).Click();

            // Ensure accessing correct Page
            Task.Delay(1000).Wait();
            IWebElement PageName = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div[1]/ul/li"));
            Assert.AreEqual<String>("PARTS", PageName.Text);

        }

        [TestMethod]
        public void TestRequiredFieldOnCreatePart()
        {

            NavigateToPartList();

            // Click On Add New Part
            Task.Delay(2000).Wait();
            driver.FindElement(By.ClassName("one23-ActionButton")).Click();

            // Cheking Required for Part Number
            Task.Delay(2000).Wait();
            driver.FindElement(By.Name("partNumber")).Clear();
            Assert.AreEqual<String>("(required)", driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[1]/span")).Text);

            // Cheking Required for Part Description
            driver.FindElement(By.Name("description")).Clear();
            Assert.AreEqual<String>("(required)", driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[2]/span")).Text);

            // Cheking Required for UOM
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div")).Click();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div/div[1]/input")).SendKeys(Keys.Backspace);
            Task.Delay(1000).Wait();
            string set = driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/span")).Text;
            Assert.AreEqual<String>("(please select)", driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/span")).Text);

            // Cheking Required for Status 
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div/div[1]/input")).SendKeys(Keys.Tab);
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/div/div[1]/input")).SendKeys(Keys.Backspace);
            string t = driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/span")).Text;
            Assert.AreEqual<String>("(please select)", driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/span")).Text);

            // Cheking Primery is Required for ACQUISITION TYPE
            Assert.IsNotNull(driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[6]/span")));

            // Cheking Required for Status
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/div/div[1]/input")).SendKeys(Keys.Tab);
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[8]/div/div[1]/input")).SendKeys(Keys.Backspace);
            Task.Delay(1000).Wait();
            Assert.AreEqual<String>("(please select)", driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[8]/span")).Text);

        }


        // Create Part without Product Group, 
        // ACQUISITION TYPE as Purchased 
        // Default Location,UOM and Status value Selected
         [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
                   "|DataDirectory|\\CreatePart.csv",
                   "CreatePart#csv",
                   DataAccessMethod.Sequential),
             // DeploymentItem("UOMData.csv"),
       TestMethod]
        public void CreatePartWithoutProductGroup()
        {
            var PartPartNumber = testContextInstance.DataRow[0].ToString();
            var PartDescription = testContextInstance.DataRow[1].ToString();
            var PartBin = testContextInstance.DataRow[2].ToString();
            var ParMinStock = testContextInstance.DataRow[3].ToString();
            NavigateToPartList();

            // Click On Add New Part
            Task.Delay(2000).Wait();
            driver.FindElement(By.ClassName("one23-ActionButton")).Click();

            // Input to Part Number
            Task.Delay(2000).Wait();
            driver.FindElement(By.Name("partNumber")).SendKeys(PartPartNumber);

            // Input to Part Description
            driver.FindElement(By.Name("description")).SendKeys(PartDescription);

            // select Product Group from drop down list
            Task.Delay(1000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[5]/div")).Click();
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[5]/div/div[2]/div/div[1]")).Click();

            // select Purchased as ACQUISITION TYPE by drag and drop
            Task.Delay(1000).Wait();
            //Created Java script handler
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            // Reads File
            string javasc = File.ReadAllText("drag_and_drop_helper.js");

            Task.Delay(2000).Wait();
            // Provided Dragable objects with Drag in Posiotion
            object ja = js.ExecuteScript(javasc + "$('#item0').simulateDragDrop({ dropTarget: '#divPrimary'});");

            // Input to Bin
            driver.FindElement(By.Name("bin")).SendKeys(PartBin);

            // Input to Min. Stock Level
            driver.FindElement(By.Name("minimumstocklevel")).SendKeys(ParMinStock);

            // Click On Create Button
            driver.FindElement(By.XPath("//*[@id='divSlide']/ng-include/div/div[3]/div[1]/div/a")).Click();

            // Ensure Part Created 
            Task.Delay(2000).Wait();
            IWebElement PartBreadcrum = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div/ul/li[2]/span"));
            Assert.AreEqual<String>(PartPartNumber.ToUpper(), PartBreadcrum.Text);

        }


        // Create Part with Product Group, 
        // Primery Acquisition Type as Purchased and Secondery Acquisition Type as Manufactured 
        // Location,UOM and Status value Select manually
        [TestMethod]
        public void CreatePartWithProductGroup()
        {
            var PartPartNumber = "PC-1000";
            var PartDescription = "Personal Computer";
            var PartBin = "SJ/8648-RT";
            var ParMinStock = "5000";
            NavigateToPartList();

            // Click On Add New Partsrc
            Task.Delay(2000).Wait();
            driver.FindElement(By.ClassName("one23-ActionButton")).Click();

            // Input to Part Number
            Task.Delay(2000).Wait();
            driver.FindElement(By.Name("partNumber")).SendKeys(PartPartNumber);

            // Input to Part Description
            driver.FindElement(By.Name("description")).SendKeys(PartDescription);

            // select UNIT OF MEASURE from drop down list
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div")).Click();
            Task.Delay(1000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[3]/div/div[2]/div/div[2]")).Click();

            // select STATUS from drop down list
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/div")).Click();
            Task.Delay(1000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[4]/div/div[2]/div/div[3]")).Click();

            // select Product Group from drop down list
            Task.Delay(1000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[5]/div")).Click();
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[5]/div/div[2]/div/div[1]")).Click();


            // select Purchased as ACQUISITION TYPE by drag and drop

            //Created Java script handler
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            // Reads File
            string javasc = File.ReadAllText("drag_and_drop_helper.js");
            Task.Delay(2000).Wait();

            // Selecting Purchesed as Primary  Acquisition Type 
            object ja = js.ExecuteScript(javasc + "$('#item1').simulateDragDrop({ dropTarget: '#divPrimary'});");

            // Selecting  Manufactured as Secondery Acquisition Type 
            Task.Delay(2000).Wait();
            object ja1 = js.ExecuteScript(javasc + "$('#item0').simulateDragDrop({ dropTarget: '#divSecondary'});");

            // select Location from drop down list
            Task.Delay(1000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[8]/div")).Click();
            Task.Delay(2000).Wait();
            driver.FindElement(By.XPath("//*[@id='one23-Slider-Form']/form/div[8]/div/div[2]/div/div[2]")).Click();

            // Input to Bin
            driver.FindElement(By.Name("bin")).SendKeys(PartBin);

            // Input to Min. Stock Level
            driver.FindElement(By.Name("minimumstocklevel")).SendKeys(ParMinStock);

            // Click On Create Button
            driver.FindElement(By.XPath("//*[@id='divSlide']/ng-include/div/div[3]/div[1]/div/a")).Click();

            // Ensure Location Created 
            Task.Delay(2000).Wait();
            IWebElement PartBreadcrum = driver.FindElement(By.XPath("//*[@id='one23-ContentDisplay']/div/div[1]/div/ul/li[2]/span"));
            Assert.AreEqual<String>(PartPartNumber.ToUpper(), PartBreadcrum.Text);

        }

       
        #endregion

    }
}
