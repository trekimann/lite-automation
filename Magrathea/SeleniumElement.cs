using Magrathea.ElementOperations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea
{
    class SeleniumElement
    {
        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        private Dictionary<String, typeObjectPair> dataValuesMap;
        private MagratheaExecutor magratheaExecutor;

        public SeleniumElement(MagratheaExecutor magratheaExecutor)
        {
            this.magratheaExecutor = magratheaExecutor;
        }

        internal object seleniumMethods(XElement seleniumElement)
        {
            // find type of element that called the selenium object
            String elementType = seleniumElement.Attribute("type").Value;
            Object toReturn = null;

            if (elementType.Equals("switchFrame"))
            {
                toReturn = SwitchFrame(seleniumElement);
            }
            else if (elementType.Equals("javascript"))
            {
                toReturn = JavaScript(seleniumElement);
            }else if (elementType.Equals("endTest"))
            {
                toReturn = EndTest(seleniumElement);
            }else if(elementType.Equals("navigateToUrl"))
            {
                toReturn = NavigateToUrl(seleniumElement);
            }else if (elementType.Equals("BREAK"))
            {
                Console.WriteLine("Lite Break Point");
            }else if (elementType.Equals("getUrl"))
            {
                toReturn = GetCurrentUrl();
            }

            return toReturn;
        }
        public String GetCurrentUrl()
        {
            return magratheaExecutor.Driver.Url;
        }
        private object NavigateToUrl(XElement seleniumElement)
        {
            String toReturn = null;
            //get the URL
            String url = "";
            //check for child variable
            List < XElement > children = seleniumElement.Elements().ToList();
            if (children.Count>0)
            {
                //get children element value
                url = (string) magratheaExecutor.ExecuteFunction(children,"");
            }
            else
            {
                url = seleniumElement.FirstNode.ToString();
            }


            if(url.Equals("TestStart"))
            {
                //go to the landing url
                url = magratheaExecutor.TestInfo.TestUrl;
            }
            else
            {
                url = url.Replace("&amp;", "&");
            }

            magratheaExecutor.Driver.Navigate().GoToUrl(url);
            magratheaExecutor.Driver.Navigate().Refresh();
           
            toReturn = magratheaExecutor.Driver.Url;

            return toReturn;
        }

        private object EndTest(XElement seleniumElement)
        {
            string toReturn = null;
            //get message
            magratheaExecutor.StopTest();
            return toReturn;
        }

        private object JavaScript(XElement seleniumElement)
        {
            String toReturn = null;

            //get the js to execute
            String JsScript = seleniumElement.FirstNode.ToString();

            JsScript.Replace("&amp;", "&");

            //execute the JS
            try
            {
                (magratheaExecutor.Driver as IJavaScriptExecutor).ExecuteScript(JsScript);
                //get any console output from the execution of the JS

                //var logEntries = magratheaExecutor.driver.Manage().Logs.GetLog(LogType.Browser);
                //foreach (LogEntry entry in logEntries)
                //{
                //    Console.WriteLine(entry.Timestamp + " " + entry.Level + " " + entry.Message);
                //    //do something useful with the logs
                //}

                magratheaExecutor.Logger.recordOutcome("Injected Javascript");
                magratheaExecutor.Logger.recordOutcome(JsScript);
            }
            catch (Exception e)
            {
                magratheaExecutor.Logger.recordOutcome("JavaScript could not be executed","Warning");
                magratheaExecutor.Logger.recordOutcome(e);
                toReturn = e.ToString();
            }
            return toReturn;
        }          

        private object SwitchFrame(XElement seleniumElement)
        {
            String toReturn = null;
            try
            {
                //get target id
                String target = seleniumElement.Attribute("target").Value;

                //if its defaultContent swtich to default frame
                if (target.Equals("DefaultContent", ignoringCase))
                {
                    magratheaExecutor.Driver.SwitchTo().DefaultContent();
                }
                else
                {
                    //find element
                    var found = magratheaExecutor.Manager.Find(target);
                    if (found != null)// if its found switch to the frame
                    {
                        magratheaExecutor.Driver.SwitchTo().Frame(found);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in frame switching");
            }
            return toReturn;
        }
    }
}
