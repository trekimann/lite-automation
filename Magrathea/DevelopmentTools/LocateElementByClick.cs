using Magrathea.webDrivers;
using OpenQA.Selenium;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Magrathea.DevelopmentTools
{
    public class LocateElementByClick
    {
        public IWebDriver driver { get; set; }
        public DictionaryTools dictionaryTools { get; set; }
        public ITestLogger logger { get; set; }
        // private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public void launchDriver(String chromeDirectory, String url)
        {
            if (driver == null)
            {
                var chromeDriverCreator = new ChromeWebdriver(chromeDirectory);
                driver = chromeDriverCreator.CreateWebdriver();
            }
            if (url != "")
            {
                driver.Navigate().GoToUrl(url);
            }
        }
        public void closeDriver()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
        }

        public IWebElement FindByxPath(String xpath)
        {
            IWebElement found = null;
            try
            {
                found = driver.FindElement(By.XPath(xpath));
                var js = (IJavaScriptExecutor)driver;
                try { js.ExecuteScript(@"$(arguments[0]).css({ ""border-width"" : ""4px"", ""border-style"" : ""solid"", ""border-color"" : ""red"" });", found); } catch (Exception hi) { Console.WriteLine(hi); }
            }
            catch (Exception e)
            { }
            return found;
        }

        // Get the element at the viewport coordinates X, Y
        public IWebElement GetElementFromPointRaw(int X, int Y)
        {
            while (true)
            {
                String s_Script = "return document.elementFromPoint(arguments[0], arguments[1]);";

                var js = (IJavaScriptExecutor)driver;
                IWebElement found = (IWebElement)js.ExecuteScript(s_Script, X, Y);
                if (found == null)
                    return null;

                if (found.TagName != "frame" && found.TagName != "iframe")
                {
                    //highlight found element
                    try { js.ExecuteScript(@"$(arguments[0]).css({ ""border-width"" : ""4px"", ""border-style"" : ""solid"", ""border-color"" : ""red"" });", found); } catch (Exception hi) { Console.WriteLine(hi); }
                    return found;
                }

                Point p_Pos = GetElementPosition(found);
                X -= p_Pos.X;
                Y -= p_Pos.Y;

                driver.SwitchTo().Frame(found);
            }
        }

        public IWebElement GetElementFromPoint(int X, int Y)
        {
            IWebElement toReturn = null;

            toReturn = GetElementFromPointRaw(X, Y);


            if (toReturn != null)
            {
                String id = toReturn.GetAttribute("id");
                String name = toReturn.GetAttribute("name");
                String type = toReturn.GetAttribute("type");

                if ((id == null || id == "") && name == null && type == null)
                {
                    //element is probably nested, find its parent by going up a level
                    Boolean gotDetails = false;
                    while (!gotDetails)
                    {
                        String xPath = GenerateXPATH(toReturn, "");
                        //go back 1 level of xpath
                        var split = xPath.Split('/');
                        String newPath = "";
                        for (int i = 0; i < split.Count() - 1; i++)
                        {
                            newPath += split[i] + "/";
                        }
                        newPath = newPath.Remove(newPath.Length - 1);//gets rid of last /
                        toReturn = FindByxPath(newPath);

                        id = toReturn.GetAttribute("id");
                        name = toReturn.GetAttribute("name");
                        if (id != null || name != null)
                        {
                            gotDetails = true;
                        }
                    }
                }
            }
            return toReturn;
        }

        public String GenerateXPATH(IWebElement childElement, String current)
        {

            String childTag = childElement.TagName;
            if (childTag.Equals("html"))
            {
                return "/html[1]" + current;
            }
            IWebElement parentElement = childElement.FindElement(By.XPath(".."));
            List<IWebElement> childrenElements = parentElement.FindElements(By.XPath("*")).ToList();
            int count = 0;
            for (int i = 0; i < childrenElements.Count; i++)
            {
                IWebElement childrenElement = childrenElements[i];
                string childrenElementString = childrenElement.ToString();
                string childElementString = childElement.ToString();
                String childrenElementTag = childrenElement.TagName;
                if (childTag.Equals(childrenElementTag))
                {
                    count++;
                }
                if (childElementString.Equals(childrenElementString))
                {
                    return GenerateXPATH(parentElement, "/" + childTag + "[" + count + "]" + current);
                }
            }
            return null;
        }

        // Get the position of the top/left corner of the Element in the document.
        // NOTE: IWebElement.Location is always measured from the top of the document and ignores the scroll position.
        public Point GetElementPosition(IWebElement i_Elem)
        {
            String s_Script = "var X, Y; "
                            + "if (window.pageYOffset) " // supported by most browsers 
                            + "{ "
                            + "  X = window.pageXOffset; "
                            + "  Y = window.pageYOffset; "
                            + "} "
                            + "else " // Internet Explorer 6, 7, 8
                            + "{ "
                            + "  var  Elem = document.documentElement; "         // <html> node (IE with DOCTYPE)
                            + "  if (!Elem.clientHeight) Elem = document.body; " // <body> node (IE in quirks mode)
                            + "  X = Elem.scrollLeft; "
                            + "  Y = Elem.scrollTop; "
                            + "} "
                            + "return new Array(X, Y);";

            var js = (IJavaScriptExecutor)driver;
            IList<Object> i_Coord = (IList<Object>)js.ExecuteScript(s_Script);

            int s32_ScrollX = Convert.ToInt32(i_Coord[0]);
            int s32_ScrollY = Convert.ToInt32(i_Coord[1]);

            return new Point(i_Elem.Location.X - s32_ScrollX,
                             i_Elem.Location.Y - s32_ScrollY);
        }


        //--------------------------------------Mouse Tracking/Get Element By Click-------------------
        Thread mouseThread;
        Boolean stopThread;
        public Boolean tracking { get; private set; } = false;
        public void StartMouseListen()
        {
            //start thread for detecting mouse clicks
            mouseThread = new Thread(() => MousePosOnClick());
            mouseThread.Start();
            tracking = true;
        }

        private void MousePosOnClick()
        {
            Point position = new Point();
            stopThread = false;
            while (!stopThread)
            {
                Boolean ctrl = (Control.ModifierKeys == System.Windows.Forms.Keys.Control);
                Boolean RightClick = (Control.MouseButtons == MouseButtons.Right);
                if (ctrl)
                {
                    if (RightClick)
                    {
                        position = Control.MousePosition;
                        var MouseX = position.X;
                        var MouseY = position.Y;

                        var element = GetElementFromPoint(MouseX, MouseY);

                        if (element != null)
                        {
                            String elementId = element.GetAttribute("id");
                            String elementName = element.GetAttribute("name");
                            String elementxPath = GenerateXPATH(element, "");

                            String SendToFront = "";

                            SendToFront += Environment.NewLine;
                            SendToFront += "element's Id:" + elementId + Environment.NewLine;
                            SendToFront += "element's name:" + elementName + Environment.NewLine;
                            SendToFront += "element's xpath: " + elementxPath + Environment.NewLine;

                            //search for element in dictionary
                            Boolean inDictionary = false;
                            dynamic results = new ExpandoObject();
                            if (elementId != "No Element Found")
                            {
                                if (elementId != "" && elementId != null)
                                {
                                    results = dictionaryTools.CheckForDefinition(elementId);
                                    inDictionary = (Boolean)results.Found;
                                }
                                if (!inDictionary && elementName != "" && elementName != null)
                                {
                                    results = dictionaryTools.CheckForDefinition(elementName);
                                    inDictionary = (Boolean)results.Found;
                                }
                                if (!inDictionary)
                                {
                                    results = dictionaryTools.CheckForDefinition(elementxPath);
                                    inDictionary = (Boolean)results.Found;
                                }
                                if (!inDictionary)
                                {
                                    SendToFront += "Element Was Not Found In Dictionary";
                                }
                                if (inDictionary)
                                {
                                    SendToFront = Environment.NewLine + results.Result;
                                }
                            }
                            logger.recordOutcome(SendToFront,"Info");
                        }
                    }
                }
            }
            Console.WriteLine("Mouse Tracking Thread Ended");
        }        

        public void StopMouseThread()
        {
            stopThread = true;
            tracking = false;

            if (mouseThread != null)
            {
                mouseThread.Abort();
            }
            mouseThread = null;
        }
        //--------------------------------------Mouse Tracking/Get Element By Click-------------------
    }
}
