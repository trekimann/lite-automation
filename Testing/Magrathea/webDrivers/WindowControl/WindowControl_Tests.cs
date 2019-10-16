using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magrathea.webDrivers;
using System.Dynamic;
using Moq;
using Magrathea.Executors;
using Selenium.testLogger;

namespace Testing.Magrathea.webDrivers.WindowControl_Tests
{
    [TestFixture]
    class WindowControl_Test
    {
        IWebDriver Driver;
        IMagratheaExecutor MockExecutor;
        ITestLogger MockLogger;
        ITestInformation MockTestInfo;

        [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver(@"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\WebDrivers");
            MockLogger = new Mock<ITestLogger>().Object;
            MockTestInfo = new Mock<ITestInformation>().Object;
            var mockContainer = new Mock<IMagratheaExecutor>();
            mockContainer.Setup(m => m.Logger).Returns(MockLogger);
            mockContainer.Setup(m => m.TestInfo).Returns(MockTestInfo);
            MockExecutor = mockContainer.Object;
        }

        [TearDown]
        public void Tear()
        {
            Driver.Close();
            Driver.Quit();
            Driver = null;
            MockExecutor = null;
        }

        [Test]
        public void OpensUrlIfNoUrlIsLoaded()
        {
            String url = "https://www.google.com/?gws_rd=ssl";
            String tabname = "Google";

            WindowControl WindowControl = new WindowControl(Driver,MockExecutor);

            WindowControl.OpenNewTab(url, tabname);

            String LandedUrl = Driver.Url;

            Assert.AreEqual(url, LandedUrl);
        }

        [Test]
        public void IfThereRareTabsAlreadyOpen_AddThemToTheLists()
        {
            String url1 = "https://www.google.com/?gws_rd=ssl";
            
            Driver.Url = url1;

            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);

            Assert.IsTrue(WindowControl.OpenTabs.Count > 0);
        }
        
        [Test]
        public void OpensUrlInNewTab_WhenATabIsAlreadyOpen()
        {
            String url1 = "https://www.google.com/?gws_rd=ssl";
            String url2 = "https://www.hastingsdirect.com/";

            String tabname = "Hastings";

            Driver.Url = url1;

            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);

            WindowControl.OpenNewTab(url2, tabname);

            String LandedUrl = Driver.Url;

            Assert.AreEqual(url2, LandedUrl);
        }

        [Test]
        public void Switching_betweenTabs()
        {
            String url1 = "https://www.google.com/?gws_rd=ssl";
            String url2 = "https://www.hastingsdirect.com/";

            String tabname1 = "Google";
            String tabname2 = "Hastings";

            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);

            WindowControl.OpenNewTab(url1, tabname1);
            WindowControl.OpenNewTab(url2, tabname2);

            WindowControl.SwitchToTab(tabname1);
            String LandedUrl = Driver.Url;
            Assert.AreEqual(url1, LandedUrl);

            WindowControl.SwitchToTab(tabname2);
            LandedUrl = Driver.Url;
            Assert.AreEqual(url2, LandedUrl);
        }

        [Test]
        public void resizeWindowWorks_1Window()
        {
            String url1 = "https://www.google.com/?gws_rd=ssl";
            String tabname1 = "Google";
            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);
            WindowControl.OpenNewTab(url1, tabname1);
            int newHeight = 500;
            int newWidth = 500;

            string size = WindowControl.ResizeWindow(newHeight, newWidth);

            Assert.AreEqual("{Width="+newWidth+", Height="+newHeight+"}", size);
        }

        [Test]
        public void resizeWindowWorks_manyWindows()
        {
            String url1 = "https://www.google.com/?gws_rd=ssl";
            String url2 = "https://www.hastingsdirect.com/";
            String tabname1 = "Google";
            String tabname2 = "Hastings";
            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);
            WindowControl.OpenNewTab(url1, tabname1);
            WindowControl.OpenNewTab(url2, tabname2);
            int newHeight = 500;
            int newWidth = 500;

            String size = WindowControl.ResizeWindow(newHeight, newWidth);
            Assert.AreEqual("{Width=" + newWidth + ", Height=" + newHeight + "}", size);
        }

        [Test]
        public void resizeWindowWhenOneValueIs0_OnlyResizesInSpecifiedDirection()
        {
            String url1 = "https://www.google.com/?gws_rd=ssl";
            String tabname1 = "Google";
            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);
            WindowControl.OpenNewTab(url1, tabname1);
            int newHeight = 500;
            int newWidth = 0;


            String size = WindowControl.ResizeWindow(newHeight, newWidth);

            string[] heightWidth = size.Split(',');

            Assert.AreEqual(" Height="+newHeight+"}", heightWidth[1]);
            Assert.AreNotEqual("{Width="+newWidth, heightWidth[0]);
        }

        [Test]
        public void checkForNewTab_FindsNewTab()
        {
            string url = @"https://devwar.hastingsdirect.com/Portal/servletcontroller?action=devland&producerCode=RGVmYXVsdA==&CampaignCode=RGVmYXVsdERlZmF1bHRDYW1wYWlnbg==";
            String tabname = "Hastings Direct";
            Driver.Url = url;
            WindowControl WindowControl = new WindowControl(Driver, MockExecutor);

            Driver.FindElement(By.XPath("/html[1]/body[1]/div[1]/span[1]/a[1]")).Click();

            var result = WindowControl.CheckForNewTab();

            Assert.GreaterOrEqual(WindowControl.UsedHandles.Count(), 1);
        }

    }
}
