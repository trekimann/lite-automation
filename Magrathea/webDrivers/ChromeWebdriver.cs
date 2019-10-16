using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Magrathea.webDrivers
{
    public class ChromeWebdriver : IOurWebDriver
    {
        private String chromeDriverDirectory;
        private string mobile;

        public ChromeWebdriver(String chromeDriverDirectory, string mobile=null)
        {
            this.chromeDriverDirectory = chromeDriverDirectory;  
            if(mobile!=null)
            {
                this.mobile = mobile;
            }
        }

        public IWebDriver CreateWebdriver()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            //mobile experiment
            if (mobile != null)
            {
                chromeOptions.EnableMobileEmulation(mobile);
            }
            //mobile experiment
            //setting up logging for JS reporting
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);

            IWebDriver driver;
            
            driver = new ChromeDriver(chromeDriverDirectory,chromeOptions);
            //		System.setProperty("webdriver.ie.driver", "C:\\Users\\MANNN\\eclipse-workspace\\SeleniumGUIAutomationLite\\SeleniumRequiredFiles\\IEDriverServer32.exe");
            //		driver = new InternetExplorerDriver();
            return driver;
        }
    }
}
