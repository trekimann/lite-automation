using Magrathea.Executors;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.testLogger;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Testing.Magrathea.Executors.JourneyExecutor_tests
{
    [TestFixture]
    class CreateDriver_Tests
    {
        [Test]
        public void Empty_driverType_ReturnsNull()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",               
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsNull(driver);
        }

        //--------------------------Android--------------------------
        [Test]
        public void MakesCorrectDriver_AndroidDriver_Happy_Uppercase()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "Android",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",
                Capability_platformVersion = "7",
                //Capability_platformVersion = "8.0.0",
                Capability_platformName = "Android",
                //Capability_deviceName = "ad05170216ef91340c",
                Capability_deviceName = "emulator-5554",
                //Capability_appPackage = "com.hastingsdirect.mobile.sit",
                //Capability_appActivity = "com.digitaslbi.hastingsdirect.MainActivity",
                Capability_noReset = "true",
                RemoteAddress = "http://localhost:4723/wd/hub",
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsInstanceOf(typeof(AndroidDriver<IWebElement>), driver);
            driver.Quit();
        }
        [Test]
        public void MakesCorrectDriver_AndroidDriver_Happy_Lowercase()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "android",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",
                Capability_platformVersion = "9",
                //Capability_platformVersion = "8.0.0",
                Capability_platformName = "Android",
                //Capability_deviceName = "ad05170216ef91340c",
                Capability_deviceName = "emulator-5554",
                //Capability_appPackage = "com.hastingsdirect.mobile.sit",
                //Capability_appActivity = "com.digitaslbi.hastingsdirect.MainActivity",
                Capability_noReset = "true",
                RemoteAddress = "http://localhost:4723/wd/hub",
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsInstanceOf(typeof(AndroidDriver<IWebElement>), driver);
            driver.Quit();
        }

        //--------------------------Android--------------------------

        //--------------------------Chrome--------------------------
        [Test]
        public void MakesCorrectDriver_ChromeDriver_Happy_Lowercase()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "chrome",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",                
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsInstanceOf(typeof(ChromeDriver), driver);
            driver.Quit();
        }
        [Test]
        public void MakesCorrectDriver_ChromeDriver_Happy_Uppercase()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "Chrome",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",                
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsInstanceOf(typeof(ChromeDriver), driver);
            driver.Quit();
        }
        //--------------------------Chrome--------------------------

        //--------------------------Firefox--------------------------
        [Test]
        public void MakesCorrectDriver_FirefoxDriver_Happy_Lowercase()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "firefox",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsInstanceOf(typeof(FirefoxDriver), driver);
            driver.Quit();
        }
        [Test]
        public void MakesCorrectDriver_FirefoxDriver_Happy_Uppercase()
        {
            TestInformation info = new TestInformation()
            {
                TestName = "NUnit Test",
                TestUrl = "http://www.google.com",
                WebBrowser = "Firefox",
                OutputDirectory = @"C:\Users\MANN",
                TestDirectory = @"C:\Users\MANN",
            };

            JourneyExecutor journey = new JourneyExecutor(null, null);

            var driver = journey.CreateDriver(info);

            Assert.IsInstanceOf(typeof(FirefoxDriver), driver);
            driver.Quit();
        }
        //--------------------------Firefox--------------------------
    }
}
