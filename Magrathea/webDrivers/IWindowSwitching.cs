using System.Collections.Generic;
using System.Xml.Linq;
using OpenQA.Selenium;

namespace Magrathea.webDrivers
{
    public interface IWindowControl
    {
        IWebDriver Driver { get; }
        IJavaScriptExecutor JsExecutor { get; }
        Dictionary<string, string> OpenTabs { get; }
        List<string> UsedHandles { get; }

        string CheckForNewTab();
        string GetNewHandle();
        string GetWindowTitle();
        object OpenNewTab(string pageUrl, string tabName);
        string SetWindowTitle(string title);
        string SwitchToTab(string tabName);
        object WindowMethods(XElement clone);
    }
}