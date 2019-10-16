using System;
using System.Collections.Generic;
using System.Text;

namespace Selenium.testLogger
{
    public class TestInformation : ITestInformation
    {
        public String TestName { get; set; }
        public String TestUrl { get; set; }
        public String WebBrowser { get; set; }
        public String OutputDirectory { get; set; }
        public String TestDirectory { get; set; }
        public int TestNumber { get; set; }
        public string MobileEmulation { get; set; }
        public String FirefoxDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\WebDrivers";
        public String ChromeDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\WebDrivers";
        public String DictionaryDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\Element Dictionary";
        public String RootXmlDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL";
        public int WindowHeight { get; set; } = 0;
        public int WindowWidth { get; set; } = 0;

        //mobile device specifics
        public Dictionary<String, String> DesiredCapibilities { get; set; }

        public String Capability_platformVersion { get; set; }
        public String Capability_platformName { get; set; }
        public String Capability_deviceName { get; set; }
        public String Capability_browserName { get; set; }
        public String Capability_appPackage { get; set; }
        public String Capability_appActivity { get; set; }
        public String Capability_noReset { get; set; }
        public String RemoteAddress { get; set; }


        //public TestInformation() { }

        //public TestInformation(String testName, String webBrowsers, String outputDirectory,
        //        String firefoxDirectoy, String chromeDirectoy)
        //{
        //    this.TestName = testName;
        //    this.WebBrowser = webBrowsers;
        //    this.OutputDirectory = outputDirectory;
        //    this.FirefoxDirectory = firefoxDirectoy;
        //    this.ChromeDirectory = chromeDirectoy;
        //}       
    }
}
