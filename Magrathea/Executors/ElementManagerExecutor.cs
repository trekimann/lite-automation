using Selenium.htmlElementManager;
using Selenium.testLogger;
using System;
using System.Xml.Linq;

namespace Magrathea.Executors
{
    public class ElementManagerExecutor : IElementManagerExecutor
    {
        private StringComparison IgnoringCase = StringComparison.InvariantCultureIgnoreCase;

        private IHtmlElementManager Manager;
        private IMagratheaExecutor MagExe;
        private ITestLogger Logger;

        public ElementManagerExecutor(ITestLogger Logger, IMagratheaExecutor MagExe, IHtmlElementManager Manager)
        {
            this.Logger = Logger;
            this.MagExe = MagExe;
            this.Manager = Manager;
        }

        public Object ElementExecution(XElement managerElement)
        {
            //toggle Element Testing flag
            Manager.TestingFunction = MagExe.TestingFunction;

            Object toReturn = null;
            String target = "";
            String variable = "";
            try { target = managerElement.Attribute("target").Value; } catch { }
            String type = managerElement.Attribute("type").Value;

            try { variable = managerElement.FirstNode.ToString(); } catch { }
            String value = "";
            // check type of variable
            if (type.IndexOf("sendkeys", IgnoringCase) >= 0)//indexOf Allows any type which starts with sendKeys down this path
            {
                toReturn = value = SendKeys(target, variable, type);
            }
            else if (type.Equals("SelectFirstOption", IgnoringCase))
            {
                toReturn = Manager.FirstSelection(managerElement);
            }
            else if (type.Equals("click", IgnoringCase) && !MagExe.debugPhase)
            {// click
                Manager.Click(target);
            }
            else if (type.Equals("pause", IgnoringCase) && !MagExe.debugPhase)
            {
                try {
                    //get value for pause
                    String pause = managerElement.Attribute("value").Value;
                    //check if timed or indefinite
                    if (pause.Equals("indefinite", IgnoringCase))
                    {
                        Logger.recordOutcome("Test was indefinitely paused");
                        Logger.sendSystemMessage("Test Paused");
                        MagExe.switchPause();
                    }
                    else
                    {
                        try
                        {
                            int durationOfPause = Int32.Parse(pause);
                            Manager.DirtyPause(durationOfPause);
                        }
                        catch (Exception e)
                        {
                            Logger.recordOutcome("Problem Pausing", "Warning");
                            Logger.recordOutcome(e);
                            Logger.recordOutcome("Test was indefinitely paused");
                            Logger.sendSystemMessage("Test Paused");
                            MagExe.switchPause();
                        }
                    }
                }
                catch (Exception e)
                {
                    //if no value, it might be a specific element to pause on
                    string pauseTarget = managerElement.Attribute("target").Value;
                    Manager.PauseTargets.Add(pauseTarget);
                }
                
            }
            else if (type.Equals("pageCheck", IgnoringCase) && !MagExe.debugPhase)
            {
                toReturn = Manager.allChecks();
            }
            else if (type.Equals("getText", IgnoringCase))
            {
                toReturn = Manager.GetText(managerElement);
            }
            else if (type.Equals("scrollTo",IgnoringCase))
            {
                toReturn = Manager.ScrollTo(managerElement);
            }
            else if (type.Equals("getAttribute",IgnoringCase))
            {
                toReturn = Manager.getAttribute(managerElement);
            }

            Console.WriteLine("Executing elementManager. Type: " + type + ". Target: " + target + ". Value:"
                    + variable + " : " + value);

            // elementAvailable mock
            if (type.Equals("elementAvailable", IgnoringCase))
            {
                if (!MagExe.debugPhase)
                {
                    toReturn = Manager.ElementAvailable(target);
                }
                else
                {
                    toReturn = true;
                }
            }
            return toReturn;
        }

        public string SendKeys(string target, string variable, string type)
        {
            String value = "";
            if (type.Equals("sendkeysclear", IgnoringCase))
            {
                Manager.SendKeysClear(target);
            }
            else
            {
                String variableType = MagExe.DataValuesMap(variable).Cast;
                if (variableType.Equals("boolean", IgnoringCase))
                {
                    value = ((Boolean)(MagExe.DataValuesMap(variable).Value)).ToString();
                }
                else if (variableType.Equals("int", IgnoringCase))
                {
                    value = ((Int32)(MagExe.DataValuesMap(variable).Value)).ToString();
                }
                else if (variableType.Equals("string", IgnoringCase))
                {
                    value = (String)(MagExe.DataValuesMap(variable).Value);
                }
                if (type.Equals("sendkeys", IgnoringCase))
                {// send keys
                    if (variableType != null)
                    {
                        if (!MagExe.debugPhase)
                        {
                            Manager.SendKeys(target, value);
                        }
                    }
                }
                else if (type.Equals("sendkeysTAB", IgnoringCase))
                {// send keys tab  
                    if (variableType != null)
                    {
                        if (!MagExe.debugPhase)
                        {
                            Manager.SendKeysTab(target, value);
                        }
                    }
                }
                else if (type.Equals("sendkeysnocheck", IgnoringCase))
                {// send keys without checking on box contents
                    if (variableType != null)
                    {
                        if (!MagExe.debugPhase)
                        {
                            Manager.SendKeysNoCheck(target, value);
                        }
                    }
                }
                else if (type.Equals("sendkeysSlow", IgnoringCase))
                {// send keys without checking on box contents
                    if (variableType != null)
                    {
                        if (!MagExe.debugPhase)
                        {
                            Manager.SendKeysSlow(target, value);
                        }
                    }
                }
            }
            return value;
        }
    }
}
