using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restClient_0
{
    class TestRunningInformation
    {
        public String testName { get; set; }
        public String testUrl { get; set; }
        public String webBrowser { get; set; }
        public String outputDirectory { get; set; }
        public String firefoxDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\WebDrivers";
        public String chromeDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\WebDrivers";
        public String dictionaryDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\Element Dictionary";
        public String testDirectory { get; set; }
        public int testNumber { get; set; }
        public String rootXmlDirectory { get; set; } = @"O:\SeleniumLite\EXPERIMENTAL";

        public int windowHeight { get; set; } = 0;
        public int windowWidth { get; set; } = 0;
        public string mobileEmulation { get; set; }

        public TestRunningInformation() { }

        public TestRunningInformation(String testName, string testDirectory, String webBrowsers, String outputDirectory)
        {
            this.testDirectory = testDirectory;
            this.testName = testName;
            this.webBrowser = webBrowsers;
            this.outputDirectory = outputDirectory;
        }

        public void setTestNumber(int testNumber)
        {
            this.testNumber = testNumber;
        }

        public int getTestNumber()
        {
            return testNumber;
        }

        public String getDictionaryDirectory()
        {
            return dictionaryDirectory;
        }

        public String getFirefoxDirectory()
        {
            return firefoxDirectory;
        }

        public void setFirefoxDirectory(String firefoxDirectoy)
        {
            this.firefoxDirectory = firefoxDirectoy;
        }

        public String getChromeDirectory()
        {
            return chromeDirectory;
        }

        public void setChromeDirectory(String chromeDirectoy)
        {
            this.chromeDirectory = chromeDirectoy;
        }

        public void setWebBrowser(String webBrowsers)
        {
            this.webBrowser = webBrowsers;
        }

        public String getTestName()
        {
            return testName;
        }

        public String getTestUrl()
        {
            return testUrl;
        }

        public String getWebBrowser()
        {
            return webBrowser;
        }

        public String getOutputDirectory()
        {
            return outputDirectory;
        }

        public void setTestName(String testName)
        {
            this.testName = testName;
        }

        public void setTestUrl(String testUrl)
        {
            this.testUrl = testUrl;
        }

        public void setOutputDirectory(String testOutputDirectory)
        {
            this.outputDirectory = testOutputDirectory;
        }

    }
}
