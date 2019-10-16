
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magrathea.webDrivers
{
    public class FirefoxWebdriver : IOurWebDriver
    {
        private String geckoDirectory;

        public FirefoxWebdriver(String webdriverType)
        {
            this.geckoDirectory = webdriverType;
        }

        public IWebDriver CreateWebdriver()
        {
            var ffOptions = new FirefoxOptions();
            ffOptions.Profile = new FirefoxProfile();
            ffOptions.Profile.AcceptUntrustedCertificates = true;
            ffOptions.Profile.AssumeUntrustedCertificateIssuer = true;
            ffOptions.AcceptInsecureCertificates = true;


            //var service = FirefoxDriverService.CreateDefaultService(geckoDirectory, "geckodriver.exe");
            String geckDir = geckoDirectory+ @"\geckodriver.exe";
            IWebDriver driver = null;
            try
            {
                driver = new FirefoxDriver(geckoDirectory, ffOptions);
            }
            catch(Exception e)
            {
                Thread.Sleep(2500);
                driver = new FirefoxDriver(geckoDirectory,ffOptions);
            }

            return driver;
        }
    }
}
