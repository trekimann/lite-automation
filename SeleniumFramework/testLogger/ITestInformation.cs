using System.Collections.Generic;

namespace Selenium.testLogger
{
    public interface ITestInformation
    {
        string Capability_appActivity { get; set; }
        string Capability_appPackage { get; set; }
        string Capability_browserName { get; set; }
        string Capability_deviceName { get; set; }
        string Capability_noReset { get; set; }
        string Capability_platformName { get; set; }
        string Capability_platformVersion { get; set; }
        string ChromeDirectory { get; set; }
        Dictionary<string, string> DesiredCapibilities { get; set; }
        string DictionaryDirectory { get; set; }
        string FirefoxDirectory { get; set; }
        string MobileEmulation { get; set; }
        string OutputDirectory { get; set; }
        string RemoteAddress { get; set; }
        string RootXmlDirectory { get; set; }
        string TestDirectory { get; set; }
        string TestName { get; set; }
        int TestNumber { get; set; }
        string TestUrl { get; set; }
        string WebBrowser { get; set; }
        int WindowHeight { get; set; }
        int WindowWidth { get; set; }
    }
}