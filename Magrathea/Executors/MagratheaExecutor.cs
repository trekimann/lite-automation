using Magrathea.DevelopmentTools;
using Magrathea.ElementOperations;
using Magrathea.Executors;
using Magrathea.forStatements;
using Magrathea.ifStatements;
using Magrathea.objects;
using Magrathea.webDrivers;
using Magrathea.whileStatements;
using OpenQA.Selenium;
using Selenium.htmlElementManager;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magrathea
{
    public class MagratheaExecutor : IMagratheaExecutor
    {
        //public TestBuilder Builder { get; private set; } = new TestBuilder();
        public ITestInformation TestInfo { get; private set; }
        private DictionaryTools DictionayTools { get; set; }
        private LocateElementByClick Locate { get; set; }


        public IHtmlElementManager Manager { get; private set; }
        private ITestLoggerExecutor TestExecutor { get; set; }
        private IElementManagerExecutor ElementExecutor { get; set; }
        public ITestLogger Logger { get; private set; }
        public IWebDriver Driver { get; private set; }
        private IXmlParser parser;
        public IWindowControl WindowSwitching { get; private set; }

        private Dictionary<string, typeObjectPair> dataValuesMap = new Dictionary<string, typeObjectPair>();
        private List<String> listNames = new List<string>();
        public Boolean Pause { get; set; } = false;
        public XElement ExecutedTest { get; private set; }
        public XElement CallingTest { get; private set; }
        public XElement InUsePhase { get; private set; }
        public Boolean DebugMag { get; set; } = false;//boolean for if a test being executed is being debuged
        public Boolean debugPhase { get; private set; } = false;//boolean for if the logic of a test is being debuged
        public Boolean TestingFunction { get; set; } = false;
        private Boolean insideFor = false;
        private string xmlDirectory;
        public string CurrentScenario { get; private set; }

        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;

        public MagratheaExecutor(XmlParser parser, TestInformation testInfo, String xmlDirectory)
        {
            this.parser = parser;
            this.xmlDirectory = xmlDirectory;
            this.TestInfo = testInfo;
        }

        public object FocusWindow()
        {
            String window = Driver.CurrentWindowHandle;
            Driver.SwitchTo().Window(window);

            var js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("document.title = \"" + TestInfo.TestName + "\"");

            return true;
        }

        public Object CheckDefinition(String SearchTerm, String journey)
        {
            return this.DictionayTools.CheckForDefinition(SearchTerm, journey);
        }

        public Object StartStopMouseTracking()
        {
            if (!Locate.tracking)
            {
                Locate.StartMouseListen();
            }
            else
            {
                Locate.StopMouseThread();
            }

            return Locate.tracking;
        }

        public int StopTest()
        {
            Manager.stopTest = true;
            return TestInfo.TestNumber;
        }

        public void SetDebug(Boolean value, XElement phase)
        {
            this.debugPhase = value;
            this.InUsePhase = phase;
        }

        public Boolean switchPause()
        {
            Pause = !Manager.Pause;
            Manager.Pause = Pause;
            return Pause;
        }

        public void SetListNames(List<String> listNames)
        {
            this.listNames = listNames;
        }

        public void SetInsideFor(Boolean value)
        {
            this.insideFor = value;
        }

        public void SetLogger(ITestLogger logger)
        {
            this.Logger = logger;
        }

        public void SetDriver(IWebDriver driver)
        {
            this.Driver = driver;
        }

        public void SetElementManager(HtmlElementManager manager)
        {
            this.Manager = manager;
        }

        public Dictionary<string, typeObjectPair> getDataValuesMap()
        {
            return dataValuesMap;
        }

        private void StartSinglePageJourney(XElement runningTest)
        {
            // get test name
            String testName = runningTest.Attribute("name").Value.ToString();
            //get function
            var function = runningTest.Element("Function").Elements().ToList();

            XElement fullDictionary = parser.ParseXml(TestInfo.DictionaryDirectory + "\\ElementDictionary.xml");
            ElementIdentifierDictionary journeyDictionary = new ElementIdentifierDictionary(
                    parser.ParseDictionary(TestInfo.DictionaryDirectory + "\\ElementDictionary.xml", "All"), fullDictionary);
            Manager.setElementDictionary(journeyDictionary);
            this.DictionayTools = new DictionaryTools(TestInfo.DictionaryDirectory + "\\ElementDictionary.xml");
            this.Locate = new LocateElementByClick();
            Locate.dictionaryTools = this.DictionayTools;
            Locate.logger = this.Logger;
            Locate.driver = Driver;

            InUsePhase = runningTest;
            CallingTest = runningTest;
            //get data
            var data = runningTest.Elements("Data").ToList()[0];
            //check if there are several scenarios
            var scenarios = data.Elements("scenario").ToList();

            if (scenarios.Count != 0)
            {
                foreach (XElement scenario in scenarios)
                {
                    //get phase values
                    Manager.stopTest = false;

                    //get scenario name
                    var name = scenario.Attribute("name").Value;
                    CurrentScenario = name;
                    //log which scenario
                    Logger.recordOutcome("---------------Starting scenario : \"" + name + "\" ---------------");
                    SinglePageExecution(scenario, function, name);
                    journeyDictionary.XmlRepair(TestInfo.DictionaryDirectory);
                }
            }
            else
            {
                SinglePageExecution(data, function);
                journeyDictionary.XmlRepair(TestInfo.DictionaryDirectory);
            }
        }

        private void SinglePageExecution(XElement data, List<XElement> function, String scenario = null)
        {
            //get phaseValues
            List<XElement> dataItems = data.Elements("phaseValue").ToList();
            PopulateTestData(dataItems, scenario);

            //execute function
            if (!Manager.stopTest)
            {
                ExecuteFunction(function, "");
            }
        }

        public void StartJourney(XElement runningTest)
        {
            // get test name
            String testName = runningTest.Attribute("name").Value.ToString();
            String testurl = runningTest.Attribute("url").Value.ToString();

            //logger.recordOutcome("Starting " + testName);

            //make window switching controle
            this.WindowSwitching = new WindowControl(Driver, this);

            if (testName.Contains("debug"))
            {
                DebugMag = true;
            }
            else
            {
                DebugMag = false;
            }

            TestExecutor = new TestLoggerExecutor(Logger, this);

            //check if single page
            String singlePage = "";
            try
            {
                singlePage = runningTest.Attribute("singlePage").Value.ToString();
            }
            catch (Exception e) { }
            if (singlePage.Equals("true", ignoringCase))
            {
                StartSinglePageJourney(runningTest);
            }
            else
            {
                // make list of all phases inside
                List<XElement> phases = runningTest.Elements().ToList();
                // loop though each phase
                foreach (XElement phase in phases)
                {
                    if (Manager.stopTest) { break; }
                    // get phase name
                    String phaseName = phase.Attribute("name").Value.ToString();
                    // get phaseValues and add them to the dataMap
                    List<XElement> phaseValues = phase.Elements().ToList();
                    PopulateTestData(phaseValues);
                    //get package name
                    String package = phase.Attribute("package").Value.ToString();
                    // Load phase template to run
                    String fileLocationForPhase = xmlDirectory + "\\Journeys\\" + package + "\\" + phaseName + ".xml";
                    InUsePhase = parser.ParseXml(fileLocationForPhase);
                    CallingTest = InUsePhase;
                    // get phase function as a list
                    List<XElement> function = InUsePhase.Element("Function").Elements().ToList();

                    // build dictionary
                    XElement fullDictionary = parser.ParseXml(TestInfo.DictionaryDirectory + "\\ElementDictionary.xml");
                    ElementIdentifierDictionary journeyDictionary = new ElementIdentifierDictionary(
                            parser.ParseDictionary(TestInfo.DictionaryDirectory + "\\ElementDictionary.xml", package), fullDictionary);
                    Manager.setElementDictionary(journeyDictionary);
                    this.DictionayTools = new DictionaryTools(TestInfo.DictionaryDirectory + "\\ElementDictionary.xml");
                    this.Locate = new LocateElementByClick();
                    Locate.dictionaryTools = this.DictionayTools;
                    Locate.logger = this.Logger;
                    Locate.driver = Driver;

                    // rename browser window
                    var js = (IJavaScriptExecutor)Driver;
                    js.ExecuteScript("document.title = \"" + runningTest.Attribute("name").Value + ": " + phaseName.Replace("phase", "") + "\"");
                    // execute function
                    ExecuteFunction(function, "");
                    journeyDictionary.XmlRepair(TestInfo.DictionaryDirectory);
                }
            }
        }

        public void PopulateTestData(List<XElement> listForDataItems, String scenario = null, Boolean clearData = true)
        {
            if (clearData)
            {
                //clear out old values
                dataValuesMap.Clear();
            }
            foreach (XElement element in listForDataItems)
            {                
                typeObjectPair variablePair = new typeObjectPair();
                // check type of variable
                String type = element.Attribute("type").Value.ToString();
                String defaultValue = "";
                try { defaultValue = element.FirstNode.ToString().Trim(); } catch { }

                if (defaultValue.Equals("", ignoringCase))
                {
                    defaultValue = null;
                }
                String name = element.Attribute("name").Value.ToString();
                if (name.Equals("AdditionallistOfAccidents", ignoringCase))
                {
                    Console.WriteLine("Mag Break Point");
                }
                variablePair.Cast = type;
                Console.WriteLine("Adding " + name + " to variable map with value " + defaultValue + " and type " + type);

                if (type.Equals("Boolean", ignoringCase))
                {
                    variablePair.Value = Boolean.Parse((String)defaultValue);
                }
                else if (type.Equals("string", ignoringCase))
                {
                    //remove any special characters
                    string value = (String)defaultValue;
                    if (value != null)
                    {
                        value = value.Replace("&amp;", "&");
                    }
                    variablePair.Value = value;
                }
                else if (type.Equals("int", ignoringCase))
                {
                    variablePair.Value = int.Parse((String)defaultValue);
                }
                else if (type.Equals("list", ignoringCase))
                {
                    List<XElement> listElements = element.Elements().ToList();
                    PopulateTestData(listElements,scenario,false);
                }
                else if (type.Equals("dateTime", ignoringCase))
                {
                    MagDateTime time = new MagDateTime(this);
                    variablePair.Value = (time.SetValue(element));
                }

                //check if data value aready in dictionary
                if (dataValuesMap.ContainsKey(name))
                {
                    Logger.recordOutcome("A phaseValue name is repeated", "Warning");
                    Logger.recordOutcome(name);
                    //check if its the same value
                    if ((DataValuesMap(name).ToString().Equals(variablePair.ToString())))
                    {
                        Logger.recordOutcome("A phaseValue value is repeated", "Warning");
                        Logger.recordOutcome(name + " : " + variablePair.ToString(), "Warning");
                        Logger.recordOutcome("Please update the phase for future runs to avoid confusion", "Warning");
                    }
                    else
                    {
                        Logger.recordOutcome("Please rename one of the phaseValues currently called: " + name, "Fatal");
                        Logger.recordOutcome("Test is being skipped", "Fatal");
                        Manager.stopTest = true;
                        break;
                    }
                }
                else
                {
                    dataValuesMap.Add(name, variablePair);
                }

                //ExecutedTest = Builder.AddPhaseValueToDataElement(element, ExecutedTest,scenario);
            }
            Console.WriteLine(dataValuesMap.Count + " Values in dictionary");
        }

        public object ExecuteFunction(List<XElement> function, String forModifier)
        {
            Object toReturn = null;
            for (int step = 0; step < function.Count(); step++)
            {
                if (Manager.stopTest)
                {
                    break;
                }
                XElement element = function[step];
                //ExecutedTest = Builder.AddTestElement(ExecutedTest, element);
                toReturn = ExecuteElement(element, forModifier);
            }
            Console.WriteLine("Function complete: " + function.Count() + " steps");
            return toReturn;
        }

        public void CheckAgainstForList(XElement element, String forModifier)
        {
            String elementText = "";
            try { elementText = element.FirstNode.ToString().Trim(); } catch { }
            foreach (String listName in listNames)
            {
                if (elementText.Equals(listName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // copy any containing children
                    var clone = CloneElement(element);
                    List<XElement> children = clone.Elements().ToList(); // element.clone().detach().getChildren();
                    String currentText = element.FirstNode.ToString().Trim();
                    element.Value = currentText + forModifier;
                    // add children back to element
                    if (children.Count > 0)
                    {
                        foreach (XElement content in children)
                        {
                            if (content.HasAttributes)
                            {
                                element.Add(content);
                            }
                        }
                    }
                    break;
                }
            }
        }

        public XElement CloneElement(XElement elementToClone)
        {
            return new XElement(elementToClone);
        }

        public typeObjectPair DataValuesMap(String variable)
        {
            typeObjectPair toReturn = null;
            try
            {
                toReturn = dataValuesMap[variable];
            }
            catch (Exception e)
            {
                Logger.recordOutcome("Error getting value: " + variable, "Warning");
                Logger.recordOutcome(e);
                Logger.screenshot();
                StopTest();//need to restart test from the start
                Logger.recordOutcome("---------Test was stopped due to a phaseValue being absent from the Data---------", "Fail");
                
                //Driver.Navigate().GoToUrl(TestInfo.TestUrl);
            }
            return toReturn;
        }

        public Object ElementManagerExecute(XElement managerElement)
        {
            if(ElementExecutor==null)
            {
                ElementExecutor = new ElementManagerExecutor(this.Logger, this, this.Manager);
            }

            return ElementExecutor.ElementExecution(managerElement);            
        }

        public void ExecuteFor(XElement forElement)
        {
            Console.WriteLine("Executing for " + forElement.Attribute("type").Value);
            ForStatements fors = new ForStatements(this, InUsePhase);

            String typeOfFor = forElement.Attribute("type").Value;

            // check type of for
            if (typeOfFor.Equals("list", ignoringCase))
            {
                fors.listFor(forElement);
            }
        }

        private void ExecuteInnerFunction(XElement element, String forModifier)
        {

            // find inner function element
            String innerFunctionName = element.Attribute("target").Value;

            if (innerFunctionName.Equals("enterLicenceNumber"))
            {
                Console.WriteLine("Mag debug Point");
            }

            Console.WriteLine("Inner Function Call execution: " + innerFunctionName);
            XElement mainFunction = InUsePhase.Element("Function");
            List<XElement> innerFunction = mainFunction.Elements("innerFunction").ToList();
            // find the phase with the right name
            XElement requiredInnerFunction = null;
            foreach (XElement innerfunction in innerFunction)
            {
                if (innerfunction.Attribute("name").Value.Trim().Equals(innerFunctionName, ignoringCase))
                {
                    requiredInnerFunction = CloneElement(innerfunction);
                    break;
                }
            }
            if (requiredInnerFunction != null)
            {
                List<XElement> innerFunctionElements = requiredInnerFunction.Elements().ToList();
                // run inner function
                ExecuteFunction(innerFunctionElements, forModifier);
            }
            else
            {
                Console.WriteLine("nammed inner function not found");
            }
        }

        public Object GetPhaseValue(XElement element)
        {
            localVariable local = new localVariable(this);
            return local.getValue(element);
        }

        private void SetPhaseValue(XElement element)
        {// takes a phaseValue and sets it according to .getText
            localVariable local = new localVariable(this);
            dataValuesMap = local.setVariable(element);
        }

        private void DisposePhaseValue(XElement element)
        {
            localVariable local = new localVariable(this);
            dataValuesMap = local.disposeVariable(element, dataValuesMap);
        }
        
        public Object ExecuteElement(XElement element, String forModifier)
        {
            
            if (Pause && !TestingFunction)
            {
                while (Pause)
                {
                    Manager.PauseTest();
                }
            }
            String elementName = element.Name.ToString();
            //if (elementName.Equals("selenium", ignoringCase))
            //{
            //    Console.WriteLine("Mag debug point");
            //}

            //write out currently executing line to test executor
            if (DebugMag)
            {
                var elementString = element.ToString();
                Logger.SendTextToClient("Currently Executing: \n" + elementString.Replace("\t\t", "\t").TrimStart());
            }

            XElement clone = CloneElement(element);
            // check if its from a for, if so add modifier to .gettext
            if (insideFor)
            {
                CheckAgainstForList(clone, forModifier);
            }
            if (elementName.Equals("if", ignoringCase))
            {
                ExecuteIf(clone, forModifier);
            }
            else if (elementName.Equals("testLogger", ignoringCase))
            {
                TestExecutor.Log(clone);
            }
            else if (elementName.Equals("elementManager", ignoringCase))
            {
                return ElementManagerExecute(clone);
            }
            else if (elementName.Equals("for", ignoringCase))
            {
                ExecuteFor(clone);
            }
            else if (elementName.Equals("innerFunctionCall", ignoringCase))
            {
                ExecuteInnerFunction(clone, forModifier);
            }
            else if (elementName.Equals("setPhaseValue", ignoringCase))
            {
                SetPhaseValue(clone);
            }
            else if (elementName.Equals("disposePhaseValue", ignoringCase))
            {
                DisposePhaseValue(clone);
            }
            else if (elementName.Equals("getPhaseValue", ignoringCase))
            {
                return GetPhaseValue(clone);
            }
            else if (elementName.Equals("dateTime", ignoringCase))
            {
                MagDateTime time = new MagDateTime(this);
                return time.dateTime(element);
            }
            else if (elementName.Equals("string", ignoringCase))
            {
                MagString magString = new MagString(this);
                return magString.stringMethod(element);
            }
            else if (elementName.Equals("while", ignoringCase))
            {
                WhileStatements whiles = new WhileStatements(this);
                whiles.execute(clone);
            }
            else if (elementName.Equals("Boolean", ignoringCase))
            {
                MagBool magBool = new MagBool(this);
                return magBool.boolMethod(clone);
            }
            else if (elementName.Equals("quoteGeneration", ignoringCase))
            {
                var isl = new IslMagMethods(this);
                return isl.Quote(clone);
            }
            else if (elementName.Equals("selenium", ignoringCase))
            {
                SeleniumElement sel = new SeleniumElement(this);
                return sel.seleniumMethods(clone);
            }
            else if (elementName.Equals("Phase", ignoringCase))
            {
                ExecutePhase(clone);
            }else if (elementName.Equals("windowControl",ignoringCase))
            {
                return WindowSwitching.WindowMethods(clone);
            }
            return null;
        }

        public object ExecutePhase(XElement Phase)
        {
            //get phase name
            String phaseName = Phase.Attribute("name").Value;
            //get package name
            String package = Phase.Attribute("package").Value;
            // Load phase template to run
            String fileLocationForPhase = xmlDirectory + "\\Journeys\\" + package + "\\" + phaseName + ".xml";
            InUsePhase = parser.ParseXml(fileLocationForPhase);
            // get phase function as a list
            List<XElement> function = InUsePhase.Element("Function").Elements().ToList();
            // rename browser window
            if (TestInfo.WebBrowser != "Android")
            {
                var js = (IJavaScriptExecutor)Driver;
                js.ExecuteScript("document.title = \"" + phaseName.Replace("phase", "") + "\"");
            }

            Logger.recordOutcome("---***---Starting Phase: \"" + phaseName.Replace("phase", "") + "\"---***---");

            //check if the phaseValues needed are in the dictionary, if not use defaults
            XElement neededValues = InUsePhase.Element("Data");
            checkphaseValuesAreThere(neededValues);

            // execute function
            var toReturn = ExecuteFunction(function, "");
            InUsePhase = CallingTest;

            return toReturn;
        }

        private void checkphaseValuesAreThere(XElement neededPhaseValues)
        {
            List<XElement> phaseValuesNeeded = neededPhaseValues.Elements("phaseValue").ToList();
            //compare each needed value to find it in the dataValuesMap
            foreach (var value in phaseValuesNeeded)
            {
                string valueName = value.Attribute("name").Value;

                if(valueName == "MTA_EditDriverResidence")
                {
                    Console.WriteLine("Mag Break Point");
                }

                if(!dataValuesMap.ContainsKey(valueName))
                {
                    typeObjectPair variablePair = new typeObjectPair();
                    //if not in the dataValuesMap, get default then add it
                    String defaultValue = null;
                    try
                    {
                        defaultValue = value.FirstNode.ToString().Trim();
                    }catch(Exception e)
                    {
                        defaultValue = value.Value;
                        if(defaultValue=="")
                        {
                            defaultValue = null;
                        }
                    }
                    String type = value.Attribute("type").Value;
                    if(type.Equals("dropDown",ignoringCase))
                    {
                        type = "string";
                    }

                    if (type.Equals("Boolean", ignoringCase))
                    {
                        variablePair.Value = Boolean.Parse((String)defaultValue);
                    }
                    else if (type.Equals("string", ignoringCase))
                    {
                        if(defaultValue=="")
                        {
                            defaultValue = null;
                        }
                        variablePair.Value = (String)defaultValue;
                    }
                    else if (type.Equals("int", ignoringCase))
                    {     
                        if(defaultValue==null)
                        {
                            defaultValue = "0";
                        }
                        variablePair.Value = int.Parse((String)defaultValue);
                    }
                    //else if (type.Equals("list", ignoringCase))
                    //{
                    //    List<XElement> listElements = element.Elements().ToList();
                    //    PopulateTestData(listElements);
                    //}
                    else if (type.Equals("dateTime", ignoringCase))
                    {
                        if (defaultValue != null)
                        {
                            MagDateTime time = new MagDateTime(this);
                            variablePair.Value = (time.SetValue(value));
                        }else
                        {
                            variablePair.Value = defaultValue;
                        }
                    }
                    variablePair.Cast = type;
                    dataValuesMap.Add(valueName, variablePair);
                    Logger.recordOutcome("Added Default phaseValue for: "+valueName,"info");
                }
            }
        }

        public void ExecuteIf(XElement ifFunction, String forModifier)
        {
            Console.WriteLine("Executing if. " + ifFunction.Attribute("type").Value.ToString() + ". On " + ifFunction.FirstNode.ToString());
            IfStatements ifs = new IfStatements(this);

            ifs.If(ifFunction);

            Console.WriteLine("if compare returned " + ifs.resultOfCompare);
            if (ifs.resultOfCompare)
            {
                List<XElement> insideIf = ifFunction.Elements().ToList();
                ExecuteFunction(insideIf, forModifier);
            }
        }
    }
}
