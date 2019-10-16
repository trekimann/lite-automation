using OpenQA.Selenium;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magrathea.webDrivers
{
    public interface IOurWebDriver
    {
        //TestInformation TestInformation { get; set; }
        //IWebDriver Driver { get; set; }
        //IJavaScriptExecutor JsDriver { get; set; }

        IWebDriver CreateWebdriver();
        //String Navigate(String uri);
        //String WindowSize(int x, int y);
        //String WindowMaximise();
        //String Quit();


    }
}
