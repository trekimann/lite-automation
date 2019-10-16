using NUnit.Framework;
using Selenium.testLogger;
using Magrathea.webDrivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Testing.Magrathea.webDrivers.AndroidWebdriver_Tests
{
	[TestFixture]
    class createWebDriver_Tests
    {    


		[Test]
		public void BuildDesiredCapabilities_AddsFieldsFromTestInformation_Happy()
        {
            Dictionary < String, String > DesiredCapibilities = new Dictionary<String, String>();
            DesiredCapibilities.Add("Capability_deviceName", "ce071717b972983604");
            DesiredCapibilities.Add("Capability_appPackage", "com.hastingsdirect.mobile.sit");
            DesiredCapibilities.Add("Capability_appActivity", "com.digitaslbi.hastingsdirect.MainActivity");
            DesiredCapibilities.Add("Capability_noReset", "true");
            DesiredCapibilities.Add("RemoteAddress","http://localhost:4723/wd/hub");
            

            AndroidWebdriver driver = new AndroidWebdriver(DesiredCapibilities);

            var cap = driver.cap.ToCapabilities();
            Assert.AreEqual("8.0.0", cap.GetCapability("platformVersion"));
            Assert.AreEqual("Android",cap.GetCapability("platformName"));
            Assert.AreEqual("ce071717b972983604", cap.GetCapability("deviceName"));
            Assert.AreEqual("com.hastingsdirect.mobile.sit",cap.GetCapability("appPackage"));
            Assert.AreEqual("com.digitaslbi.hastingsdirect.MainActivity", cap.GetCapability("appActivity"));
            Assert.AreEqual("true", cap.GetCapability("noReset"));
        }

        [Test]
        public void BuildDesiredCapabilities_AddsFieldsFromTestInformation_MissingInformationInTestInformation_DoesntAddToDesidedCapabilities()
        {
            Dictionary<String, String> DesiredCapibilities = new Dictionary<String, String>();
            DesiredCapibilities.Add("Capability_appPackage", "com.hastingsdirect.mobile.sit");
            //DesiredCapibilities.Add("Capability_appActivity", "com.digitaslbi.hastingsdirect.MainActivity");
            //DesiredCapibilities.Add("Capability_noReset", "true");
            DesiredCapibilities.Add("RemoteAddress", "http://localhost:4723/wd/hub");


            AndroidWebdriver driver = new AndroidWebdriver(DesiredCapibilities);

            var cap = driver.cap.ToCapabilities(); ;

            //var dict = driver.cap.ToDictionary();

            //Assert.AreEqual(dict.Count, 1);

            Assert.AreEqual("8.0.0", cap.GetCapability("platformVersion"));
            Assert.AreEqual(null, cap.GetCapability("platformName"));
            Assert.AreEqual(null, cap.GetCapability("deviceName"));
            Assert.AreEqual(null, cap.GetCapability("appPackage"));
            Assert.AreEqual(null, cap.GetCapability("appActivity"));
            Assert.AreEqual(null, cap.GetCapability("noReset"));
        }

        [Test]
        public void createWebDriver_returnsAndroidWebdriver_Happy()
        {
            Dictionary < String, String > DesiredCapibilities = new Dictionary<String, String>();
            DesiredCapibilities.Add("Capability_deviceName", "ce071717b972983604");
            DesiredCapibilities.Add("Capability_appPackage", "com.hastingsdirect.mobile.sit");
            DesiredCapibilities.Add("Capability_appActivity", "com.digitaslbi.hastingsdirect.MainActivity");
            DesiredCapibilities.Add("Capability_noReset", "true");
            DesiredCapibilities.Add("RemoteAddress","http://localhost:4723/wd/hub");

            String Url = "http://www.google.com";

            AndroidWebdriver Driver = new AndroidWebdriver(DesiredCapibilities);

            IWebDriver driver = Driver.CreateWebdriver();

            driver.Navigate().GoToUrl(Url);
            driver.Quit();
        }

    }
}
