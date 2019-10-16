using Magrathea.Executors;
using OpenQA.Selenium;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Magrathea.webDrivers
{
    public class WindowControl : IWindowControl
    {
        public IWebDriver Driver { get; private set; }
        public IJavaScriptExecutor JsExecutor { get; private set; }
        public IMagratheaExecutor Executor { get; private set; }
        public ITestLogger Logger { get; private set; }
        public Dictionary<String, String> OpenTabs { get; private set; } = new Dictionary<string, string>();
        public List<String> UsedHandles { get; private set; } = new List<string>();
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public WindowControl(IWebDriver driver, IMagratheaExecutor executor)
        {
            this.Executor = executor;
            this.Logger = Executor.Logger;
            this.Driver = driver;
            this.JsExecutor = Driver as IJavaScriptExecutor;

            //populate the handle list if there was already open windows
            List<String> windowHandleSet = Driver.WindowHandles.ToList();
            foreach (String handle in windowHandleSet)
            {
                Driver.SwitchTo().Window(handle);
                String tabName = GetWindowTitle();
                OpenTabs.Add(tabName, handle);
                UsedHandles.Add(handle);
                Console.WriteLine("Added page as: " + tabName);
            }
        }

        public object WindowMethods(XElement clone)
        {
            // find type of element that called the WindowControl object
            String elementType = clone.Attribute("type").Value;
            Object toReturn = null;
            if (elementType.Equals("newTab", ignoringCase))
            {
                //get tabName
                string tabName = clone.Attribute("tabName").Value;
                //get url to open
                string url = "";
                try
                {
                    //check if its a variable
                    var target = clone.Attribute("target").Value;
                    //get the value from phaseValues
                    url = (string)Executor.DataValuesMap(target).Value;

                }
                catch (Exception e)
                {
                    //see if the url is hardCoded
                    var hasChildren = clone.Elements().ToList();
                    if (hasChildren.Count() > 0)
                    {
                        //get the child value
                        url = (string)Executor.GetPhaseValue(hasChildren[0]);
                    }
                    else
                    {
                        url = clone.FirstNode.ToString();
                    }
                }
                toReturn = OpenNewTab(url, tabName);

            }
            else if (elementType.Equals("closeTab", ignoringCase))
            {
                String toClose = "";
                try
                {
                    //see if there is a tab specifically to close
                    toClose = clone.Attribute("tabName").Value;
                    CloseTab(toClose);
                }
                catch (Exception e)
                {
                    //check for an internal child value
                    var hasChildren = clone.Elements().ToList();
                    if (hasChildren.Count() > 0)
                    {
                        toClose = (string)Executor.GetPhaseValue(hasChildren[0]);
                        CloseTab(toClose);
                    }
                    else
                    {
                        //if not then close the currently open tab
                        CloseTab(Driver.CurrentWindowHandle);
                    }
                }
            }
            else if (elementType.Equals("getWindowTitle", ignoringCase))
            {
                toReturn = GetWindowTitle();
            }
            else if (elementType.Equals("switchToTab", ignoringCase))
            {
                //check if the desired tab is hard coded
                String tabName = "";
                try
                {
                    tabName = clone.Attribute("tabName").Value;
                }
                catch (Exception e)
                {
                    //check the child values
                    var hasChildren = clone.Elements().ToList();
                    if (hasChildren.Count() > 0)
                    {
                        tabName = (string)Executor.GetPhaseValue(hasChildren[0]);
                    }
                    else
                    {
                        //this will be if they put the variable straight into the element
                        tabName = clone.FirstNode.ToString();
                        tabName = (string)Executor.DataValuesMap(tabName).Value;
                    }
                }
                return SwitchToTab(tabName);
            }
            else if (elementType.Equals("setWindowTitle", ignoringCase))
            {
                //check if the desired tab is hard coded
                String tabName = "";
                try
                {
                    tabName = clone.Attribute("tabName").Value;
                }
                catch (Exception e)
                {
                    //check the child values
                    var hasChildren = clone.Elements().ToList();
                    if (hasChildren.Count() > 0)
                    {
                        tabName = (string)Executor.GetPhaseValue(hasChildren[0]);
                    }
                    else
                    {
                        //this will be if they put the variable straight into the element
                        tabName = clone.FirstNode.ToString();
                        tabName = (string)Executor.DataValuesMap(tabName).Value;
                    }
                }
                return SetWindowTitle(tabName);
            }
            else if (elementType.Equals("resize", ignoringCase))
            {
                //get height and width values
                String height = "";
                String width = "";
                try { height = clone.Attribute("height").Value; }
                catch (Exception e) { height = "0"; }

                try { width = clone.Attribute("width").Value; }
                catch (Exception ex) { width = "0"; }

                //check if one or both says full
                if (height.Equals("full", ignoringCase) && width.Equals("full", ignoringCase))
                {
                    Driver.Manage().Window.Maximize();
                    return Driver.Manage().Window.Size.ToString();
                }
                if (height.Equals("full", ignoringCase))
                {
                    height = Screen.PrimaryScreen.WorkingArea.Height.ToString();
                }
                if (width.Equals("full", ignoringCase))
                {
                    width = Screen.PrimaryScreen.WorkingArea.Width.ToString();
                }

                toReturn = ResizeWindow(Int32.Parse(height), Int32.Parse(width));
            }
            else if (elementType.Equals("checkForNewTab", ignoringCase))
            {
                toReturn = CheckForNewTab();
            }

            return toReturn;
        }

        public Object OpenNewTab(String pageUrl, String tabName)
        {
            Object toReturn = null;
            if (!Driver.Url.Contains("http"))//only happens if no page has been previously loaded
            {
                Driver.Url = pageUrl;
                String windowHandle = Driver.CurrentWindowHandle;
                //OpenTabs.Add(tabName, windowHandle);
                UsedHandles.Add(windowHandle);
            }
            else
            {
                String JsOpenNewWindow = "window.open();";
                JsExecutor.ExecuteScript(JsOpenNewWindow);
                String handle = GetNewHandle();
                OpenTabs.Add(tabName, handle);
                SwitchToTab(tabName);
                Driver.Url = pageUrl;
                UsedHandles.Add(handle);
            }

            toReturn = SetWindowTitle(tabName);

            return toReturn;
        }

        public void CloseTab(String tabName)
        {
            tabName = SwitchToTab(tabName);
            Driver.Close();
            UsedHandles.Remove(OpenTabs[tabName]);
            OpenTabs.Remove(tabName);

            //change focus to a window that is still open
            SwitchToTab(UsedHandles[0]);
        }

        public String GetNewHandle()
        {
            // get all open tabs
            List<String> windowHandleSet = Driver.WindowHandles.ToList();
            // compare the list of tabs to what is in the map. If its different, that is the
            // new handle to return
            foreach (String openHandle in windowHandleSet)
            {
                if (!UsedHandles.Contains(openHandle))
                {
                    return openHandle;
                }
            }
            return null;
        }

        public String SwitchToTab(String tabName)
        {
            String handle = "";
            try
            {//see is the tab is in the OpenTabsList
                handle = OpenTabs[tabName];
                Driver.SwitchTo().Window(handle);
            }
            catch (Exception e)
            {
                //if not,see if its a window handle what was sent
                try
                {
                    Driver.SwitchTo().Window(tabName);
                    tabName = GetTabNameFromHandle(tabName);
                }
                catch (Exception ex)
                {
                    Logger.recordOutcome("Unable to switch to tab, please check test", "Warning");
                    Logger.recordOutcome(e);
                    Logger.recordOutcome(ex);
                }
            }
            Logger.recordOutcome("Switched to: " + tabName);
            return tabName;
        }

        public string GetTabNameFromHandle(String handle)
        {
            foreach (string tabName in OpenTabs.Keys)
            {
                var associatedHandle = OpenTabs[tabName];
                if (associatedHandle == handle)
                {
                    return tabName;
                }
            }
            return Driver.Title;
        }

        public String GetWindowTitle()
        {
            //find the currently focused tabs handle
            var currentHandle = Driver.CurrentWindowHandle;
            //look up the handle in the dictionary
            return GetTabNameFromHandle(currentHandle);
        }

        public String SetWindowTitle(String title)
        {
            //find the currently focused tabs handle
            var currentHandle = Driver.CurrentWindowHandle;
            //look up the handle in the dictionary
            Boolean replaced = false;
            foreach (string tabName in OpenTabs.Keys)
            {
                var associatedHandle = OpenTabs[tabName];
                if (associatedHandle == currentHandle)
                {
                    //change tabKey for the current page
                    OpenTabs.Remove(tabName);
                    OpenTabs.Add(title, currentHandle);
                    replaced = true;
                    break;
                }
            }
            JsExecutor.ExecuteScript("document.title =\"" + title + "\"");
            if (!replaced)
            {
                return CheckForNewTab();
            }
            return title;
        }

        public String CheckForNewTab()
        {
            String newTab = null;
            // count number of currently open tabs
            List<String> windowHandleSet = Driver.WindowHandles.ToList();
            if (windowHandleSet.Count > UsedHandles.Count)// if there are more open windows than in the list
            {
                String newHandle = GetNewHandle();
                UsedHandles.Add(newHandle);// adds to list of used handles so that there is not confusion when a deliberate tab is opened
                //change to new tab
                Driver.SwitchTo().Window(newHandle);
                String tabTitle = GetWindowTitle();
                if (OpenTabs.ContainsKey(tabTitle))
                {
                    Logger.recordOutcome("There was a tab with name: " + tabTitle + " already. Added as " + tabTitle + "_");
                    tabTitle = tabTitle + "_";
                }
                OpenTabs.Add(tabTitle, newHandle);
                newTab = tabTitle;
            }
            return newTab;
        }

        public String ResizeWindow(int Height, int Width)
        {
            String toReturn = "";
            if (Executor.TestInfo.WebBrowser != "Android")
            {
                int HeightToSetTo = Driver.Manage().Window.Size.Height;
                int WidthToSetTo = Driver.Manage().Window.Size.Width;

                if (Height != 0)
                {
                    HeightToSetTo = Height;
                }
                if (Width != 0)
                {
                    WidthToSetTo = Width;
                }
                Driver.Manage().Window.Size = new Size(WidthToSetTo, HeightToSetTo);
                toReturn = Driver.Manage().Window.Size.ToString();
                Logger.recordOutcome("Window size was set to: " + toReturn);
            }else
            {
                Logger.recordOutcome("On Android Device so window was not resized");
            }
            return toReturn;
        }


    }
}
