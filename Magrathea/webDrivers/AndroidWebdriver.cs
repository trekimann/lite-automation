using System;
using System.Drawing;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using Selenium.testLogger;
using SharpAdbClient;
using System.Collections.Generic;

namespace Magrathea.webDrivers
{
    public class AndroidWebdriver : IOurWebDriver
    {
        //public TestInformation TestInformation;
        //public DesiredCapabilities cap { get; private set; }
        public AppiumOptions cap { get; private set; }
        public Dictionary<String,String> DesiredCapibilities { get; set; }
        public IWebDriver Driver { get; set ; }
        public IJavaScriptExecutor JsDriver { get; set; }

        public String DeviceName { get; private set; }

        public AndroidWebdriver(Dictionary<String, String> DesiredCapibilities)
        {
            this.DesiredCapibilities = DesiredCapibilities;
            BuildDesiredCapabilities();
        }

        private void BuildDesiredCapabilities()
        {
            //this.cap = new DesiredCapabilities();
            this.cap = new AppiumOptions();// { BrowserName = "Chrome" };

            //loop thourgh the testInformation object and pull out anything which starts with Capability_
            
            var tag = "Capability_";

            foreach (string Capibility in DesiredCapibilities.Keys)
            {
                if (Capibility.StartsWith(tag))
                {
                    String capibility = Capibility.Replace(tag, "");
                    String value = DesiredCapibilities[Capibility];
                    cap.AddAdditionalCapability(capibility, value);
                }
            }           
        }

        public IWebDriver CreateWebdriver()
        {
            //http://appium.io/docs/en/writing-running-appium/caps/
            cap.AddAdditionalCapability("fullReset", "False");
            cap.AddAdditionalCapability("platformName", "Android");
           
            //can use IWebElement instead of AndroidElement if using the chrome app on device instead of using the app.
            String uri = DesiredCapibilities["RemoteAddress"];
            String adbLocation = DesiredCapibilities["AdbLocation"];
            //launch ADB and get connected device info
            AdbServer server = new AdbServer();
            var result = server.StartServer(adbLocation, true);
            var Devices = AdbClient.Instance.GetDevices();

            var device = Devices[0];
            var receiver = new ConsoleOutputReceiver();

            //get android version
            AdbClient.Instance.ExecuteRemoteCommand("getprop ro.build.version.release", device, receiver);
            string version = receiver.ToString().Trim();
            cap.AddAdditionalCapability("platformVersion", version);

            DeviceName = device.Name;
            cap.AddAdditionalCapability("deviceName", DeviceName);

            var androidDriver = new AndroidDriver<IWebElement>(new Uri(uri),cap);
            
            return androidDriver;
        }
               
        public string Navigate(string uri)
        {
            Driver.Navigate().GoToUrl(uri);
            String landedUri = Driver.Url;
            return landedUri;
        }

        public string WindowSize(int x, int y)
        {
            return "Not Availible On Android";
            Driver.Manage().Window.Size = new Size(x, y);
            String resized = Driver.Manage().Window.Size.Width.ToString() + " x: " + Driver.Manage().Window.Size.Height.ToString();
            return resized;
        }

        public string WindowMaximise()
        {
            return "Not Availible On Android";
        }

        public string Quit()
        {
            String toReturn = "Failed To Quit";
            try
            {
                Driver.Quit();
                toReturn = "Quit Android Driver";
            }catch(Exception e)
            {

            }
            return toReturn;
        }

        public Boolean Click(IWebElement element)
        {           
            element.Click();
            return true;
        }
    }
}
