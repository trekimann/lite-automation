using Magrathea;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using System.Json;
using Newtonsoft.Json;
using Selenium.testLogger;
using Newtonsoft.Json.Linq;
using Magrathea.Executors;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.IO;
using HttpWebServices;
using Magrathea.DevelopmentTools;
using Selenium;
using System.Web.Http.Controllers;

namespace SelfHostApi
{
    public class PhaseElementsObject
    {
        public string phaseName { get; set; }
        public string package { get; set; }
        public List<Object> magElements { get; set; } = new List<Object>();
    }

    public class XmlController : ApiController
    {
        private static HttpWebOutgoing networkMessaging { get; set; }
        private static MultiThreadedExecutor multiExe { get; set; }
        private static LocateElementByClick locate { get; set; }

    protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            if(networkMessaging == null)
            {
            networkMessaging = new HttpWebOutgoing();
            locate = new LocateElementByClick();
            var port = ((System.Web.Http.SelfHost.HttpSelfHostConfiguration)controllerContext.Configuration).BaseAddress.Port;

            multiExe = new MultiThreadedExecutor(networkMessaging, port+1);
            }
        }

        //static HttpWebOutgoing networkMessaging = new HttpWebOutgoing();
        //static MultiThreadedExecutor multiExe = new MultiThreadedExecutor(networkMessaging,26000);
        //static LocateElementByClick locate = new LocateElementByClick();

        private StringComparison ignoringCase = StringComparison.InvariantCultureIgnoreCase;
        //----------------------------------------------------Gets---------------------------------------
        //-----Xml controller----
        [HttpGet]
        [Route("Xml/{package}/{phaseName}")]//getting the phase details for a specific phase
        public PhaseElementsObject Get(string package, string phaseName)
        {
            //parse the phase xml
            XmlParser parser = new XmlParser();
            string directory = @"O:\SeleniumLite\EXPERIMENTAL\Journeys\";
            var parsed = parser.ParseXml(directory + package + @"\" + phaseName + ".xml");

            //get Data element list
            var dataElement = parsed.Element("Data").Elements().ToList();
            PhaseElementsObject phaseObject = new PhaseElementsObject();
            phaseObject.phaseName = phaseName;
            phaseObject.package = package;
            //loop though the data elements and build the object to seralise
            foreach (XElement element in dataElement)
            {
                Object magPhase = ElementToObject(element);
                phaseObject.magElements.Add(magPhase);
            }
            return phaseObject;
        }

        [HttpGet]
        [Route("Xml/InnerJourneyOrder/{innerJourney}")]
        public Object GetInnerJourney(string innerJourney)
        {
            XmlParser parser = new XmlParser();
            string directory = @"O:\SeleniumLite\EXPERIMENTAL\Journeys\journeyOrder.xml";
            var journeys = parser.ParseXml(directory).Elements().ToList();

            //find element with the journey requested
            XElement requestedJourneyElement = new XElement("test");
            var found = false;
            foreach (XElement journeyElement in journeys)
            {
                if ((journeyElement.Attribute("name").Value.Equals(innerJourney, ignoringCase))
                    && journeyElement.Name.ToString().Equals("innerJourneyOrder", ignoringCase))
                {
                    requestedJourneyElement = journeyElement;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, innerJourney + " not found"));
            }
            //create journey expando
            dynamic journeyObject = new ExpandoObject();
            journeyObject.name = requestedJourneyElement.Attribute("name").Value;
            journeyObject.phaseOrder = new List<Object>();
            //get list of all phases
            List<XElement> phases = requestedJourneyElement.Elements().ToList();
            //loop though the phases and add them to journey object
            foreach (XElement phase in phases)
            {
                //check for child elements
                if (phase.HasElements)
                {
                    dynamic childPhases = new ExpandoObject();
                    childPhases.name = phase.Attribute("name").Value;
                    childPhases.phaseOrderGroup = new List<Object>();
                    List<XElement> phaseGroup = phase.Elements().ToList();
                    foreach (XElement group in phaseGroup)
                    {
                        //turn element into object
                        var phaseObject = PhaseToObject(group);
                        childPhases.phaseOrderGroup.Add(phaseObject);
                    }
                    journeyObject.phaseOrder.Add(childPhases);
                }
                else
                {
                    var phaseObject = PhaseToObject(phase);
                    journeyObject.phaseOrder.Add(phaseObject);
                }
            }
            return journeyObject;
        }

        [HttpGet]
        [Route("Xml/JourneyOrder/{journey}")]
        public Object GetSpecificJourneyOrder(string journey)
        {
            XmlParser parser = new XmlParser();
            string directory = @"O:\SeleniumLite\EXPERIMENTAL\Journeys\journeyOrder.xml";
            var journeys = parser.ParseXml(directory).Elements().ToList();

            //find element with the journey requested
            XElement requestedJourneyElement = new XElement("test");
            var found = false;
            foreach (XElement journeyElement in journeys)
            {
                if (journeyElement.Attribute("name").Value.Equals(journey, ignoringCase))
                {
                    requestedJourneyElement = journeyElement;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, journey + " not found"));
            }
            //create journey expando
            dynamic journeyObject = new ExpandoObject();
            journeyObject.name = requestedJourneyElement.Attribute("name").Value;
            journeyObject.phaseOrder = new List<Object>();
            //get list of all phases
            List<XElement> phases = requestedJourneyElement.Elements().ToList();
            //loop though the phases and add them to journey object
            foreach (XElement phase in phases)
            {
                //check for child elements
                if (phase.HasElements)
                {
                    dynamic childPhases = new ExpandoObject();
                    childPhases.name = phase.Attribute("name").Value;
                    childPhases.phaseOrderGroup = new List<Object>();
                    List<XElement> phaseGroup = phase.Elements().ToList();
                    foreach (XElement group in phaseGroup)
                    {
                        //turn element into object
                        var phaseObject = PhaseToObject(group);
                        childPhases.phaseOrderGroup.Add(phaseObject);
                    }
                    journeyObject.phaseOrder.Add(childPhases);
                }
                else
                {
                    var phaseObject = PhaseToObject(phase);
                    journeyObject.phaseOrder.Add(phaseObject);
                }
            }
            return journeyObject;
        }

        [HttpGet]
        [Route("Xml/JourneyOrder")]//getting the journey order
        public Object Get()
        {
            XmlParser parser = new XmlParser();
            string directory = @"O:\SeleniumLite\EXPERIMENTAL\Journeys\journeyOrder.xml";
            var journeys = parser.ParseXml(directory).Elements().ToList();

            //build Expando for JSON
            dynamic journeyOrder = new ExpandoObject();
            journeyOrder.journeys = new List<ExpandoObject>();

            //loop though the journeys and add attribues to new expando Object
            foreach (XElement journey in journeys)
            {
                //create journey expando
                dynamic journeyObject = new ExpandoObject();
                journeyObject.name = journey.Attribute("name").Value;
                journeyObject.phaseOrder = new List<Object>();
                //get list of all phases
                List<XElement> phases = journey.Elements().ToList();
                //loop though the phases and add them to journey object
                foreach (XElement phase in phases)
                {
                    //check for child elements
                    if (phase.HasElements)
                    {
                        dynamic childPhases = new ExpandoObject();
                        childPhases.name = phase.Attribute("name").Value;
                        childPhases.phaseOrderGroup = new List<Object>();
                        List<XElement> phaseGroup = phase.Elements().ToList();
                        foreach (XElement group in phaseGroup)
                        {
                            //turn element into object
                            var phaseObject = PhaseToObject(group);
                            childPhases.phaseOrderGroup.Add(phaseObject);
                        }
                        journeyObject.phaseOrder.Add(childPhases);
                    }
                    else
                    {
                        var phaseObject = PhaseToObject(phase);
                        journeyObject.phaseOrder.Add(phaseObject);
                    }
                }
                journeyOrder.journeys.Add(journeyObject);
            }
            return journeyOrder;
        }

        //-----Test controller----
        [HttpGet]
        [Route("Test/FocusWindow/{testNumber}")]
        public Object FocusWindow(int testNumber)
        {
            return multiExe.FocusWindow(testNumber);
        }

        [HttpGet]
        [Route("Test/Screenshot/{testNumber}")]
        public Object ManualScreenshot(int testNumber)
        {
            return multiExe.Screenshot(testNumber);
        }        

        [HttpGet]
        [Route("Test/Stop/{testNumber}")]
        public Object StopTest(int testNumber)
        {
            return multiExe.StopTest(testNumber);
        }

        [HttpGet]
        [Route("Test/StopClose/{testNumber}")]
        public Object StopAndQuitTest(int testNumber)
        {
            return multiExe.StopQuit(testNumber);
        }

        [HttpGet]
        [Route("Test/Pause/{testNumber}")]
        public Object PauseTest(int testNumber)
        {
            return multiExe.PauseTest(testNumber);
        }

        [HttpGet]
        [Route("Test/Threads/{numberOfThreads}")]
        public Object SetThreadNumber(int numberOfThreads)
        {
            dynamic toReturn = new ExpandoObject();
            toReturn.ThreadsEnabled = multiExe.SetNumberOfThreads(numberOfThreads);
            return toReturn;
        }

        [HttpGet]
        [Route("Dev/ElementAtPosition/CloseDriver")]
        public Object CloseDevDriver()
        {
            dynamic toReturn = new ExpandoObject();
                locate.closeDriver();
            return toReturn;
        }

        //----------------------------------------------------Gets---------------------------------------

        //----------------------------------------------------Posts--------------------------------------
        [HttpPost]
        [Route("Dev/DictionaryTools/Dictionary")]
        public Object GetFullDictionary([FromBody]Object directory)
        {
            //Dictionary<String, Dictionary<String, ElementIdentifierWrapper>> dictionary;
            dynamic toReturn = new ExpandoObject();

            dynamic DirectoryObject = JsonConvert.DeserializeObject<ExpandoObject>(directory.ToString());
            //pull out directory
            String directoryString = DirectoryObject.Directory;
            DictionaryTools tools = new DictionaryTools(directoryString);
            toReturn.Dictionary = tools.mainDictionary;

            return tools.mainDictionary;
        }

        [HttpPost]
        [Route("Dev/DictionaryTools/DictionaryMerge")]
        public Object MergeDictionarys([FromBody]Object directorys)
        {
            //Dictionary<String, Dictionary<String, ElementIdentifierWrapper>> dictionary;
            dynamic toReturn = new ExpandoObject();

            dynamic DirectoryObject = JsonConvert.DeserializeObject<ExpandoObject>(directorys.ToString());
            //pull out directory
            String FirstdirectoryString = DirectoryObject.FirstDirectory;
            DictionaryTools tools = new DictionaryTools(FirstdirectoryString);

            tools.Merge(DirectoryObject.SecondDirectory);

            return tools.mainDictionary;
        }

        [HttpPost]
        [Route("Dev/CheckDefinition/LocateInPhases")]
        public Object CheckDefinitionInPhase([FromBody]Object detailsCheck)
        {
            dynamic toReturn = new ExpandoObject();

            //get list of search terms back from json
            dynamic searchTerms = JsonConvert.DeserializeObject<ExpandoObject>(detailsCheck.ToString());
            List<object> objectList = searchTerms.ListOfSearchTerms;
            List<String> idsToFind = new List<string>();
            foreach(var id in objectList)
            {
                idsToFind.Add((String)id);
            }

            //if dictionary location is included use that to make dictionary tools
            String directory = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\Element Dictionary\ElementDictionary.xml";
            try
            {
                directory = searchTerms.Directory;
            }catch(Exception e)
            {}
            //if phase location included use that, otherwise use O drive
            String PhasesDirectory = @"O:\SeleniumLite\EXPERIMENTAL\Journeys";
            try
            {
                PhasesDirectory = searchTerms.PhasesDirectory;
            }catch(Exception e)
            {}

            DictionaryTools tools = new DictionaryTools(directory);
            toReturn = tools.CheckForDefintionInPhases(idsToFind, PhasesDirectory);

            return toReturn;
        }

        [HttpPost]
        [Route("Dev/AddDefintion")]
        public Object AddDeffintion([FromBody] Object definition)
        {
            dynamic toReturn = new ExpandoObject();

            dynamic request = JsonConvert.DeserializeObject<ExpandoObject>(definition.ToString());

            //pull apart object
            String Directory = request.Directory;

            String identifier = request.identifier;
            ElementIdentifierWrapper definitionWrapper = new ElementIdentifierWrapper();
            definitionWrapper.SetJourney(request.journey);
            definitionWrapper.SetElementId(request.elementId);
            definitionWrapper.SetElementName(request.elementName);
            definitionWrapper.SetElementXpath(request.elementXpath);

            DictionaryTools tools = new DictionaryTools(Directory);
            toReturn = tools.AddTermToDictionary(identifier, definitionWrapper);

            return toReturn;
        }

        [HttpPost]
        [Route("Dev/CheckDefinition")]
        public Object CheckDefinition([FromBody]Object detailsCheck)
        {
            dynamic toReturn = new ExpandoObject();

            dynamic checkInfo = JsonConvert.DeserializeObject<ExpandoObject>(detailsCheck.ToString());
            
            string SearchTerm = checkInfo.SearchTerm;
            try
            {
                int TestId = (int) checkInfo.TestId;
                toReturn = multiExe.CheckDefinition(TestId, SearchTerm);
            }catch(Exception e)
            {
                string directory = checkInfo.Directory;
                //if there is no id, prob calling from front end with no test
                DictionaryTools tools = new DictionaryTools(directory);
                string searchTermMain = SearchTerm.Split(new string[] { "---***---" }, StringSplitOptions.None)[0];
                string journey = SearchTerm.Split(new string[] { "---***---" }, StringSplitOptions.None)[1];

                toReturn = tools.CheckForDefinition(searchTermMain, journey);
            }
            return toReturn;
        }

        [HttpPost]
        [Route("Dev/ElementAtPosition/StartDriver")]
        public Object StartDevDriver([FromBody] Object target)
        {
            dynamic toReturn = new ExpandoObject();
            dynamic webInfo = JsonConvert.DeserializeObject<ExpandoObject>(target.ToString());
            String chromeDirectory = webInfo.ChromeDirectory;
            String url = webInfo.Url;

            locate.launchDriver(chromeDirectory,url);

            return toReturn;
        }

        [HttpPost]
        [Route("Dev/ElementAtPosition")]
        public Object GetElementsUnderClick([FromBody] Object mousePosition)
        {
            dynamic toReturn=new ExpandoObject();
            toReturn.id = "No Element Found";
            toReturn.name = "No Element Found";
            toReturn.xPath = "No Element Found";
            dynamic mousePos = JsonConvert.DeserializeObject<ExpandoObject>(mousePosition.ToString());
            int MouseX = (int)mousePos.MouseX;
            int MouseY = (int)mousePos.MouseY;

            var element = locate.GetElementFromPoint(MouseX, MouseY);

            if (element != null)
            {

                String xPath = locate.GenerateXPATH(element, "");

                toReturn.xPath = "";
                toReturn.type = "";

                if (element != null)
                {
                    String id = element.GetAttribute("id");
                    String name = element.GetAttribute("name");
                    String type = element.GetAttribute("type");

                    if (id == null)
                    { id = ""; }
                    toReturn.id = id;

                    if (name == null)
                    { name = ""; }
                    toReturn.name = name;

                    if (type == null)
                    { type = ""; }
                    toReturn.type = type;

                    toReturn.xPath = xPath;
                }
            }
            return toReturn;
        }

        [HttpPost]
        [Route("Test/ElementAtPosition")]
        public Object GetElementUnderClickInTest([FromBody]Object testId)
        {
            dynamic toReturn = new ExpandoObject();

            //get test id
            dynamic testInfo = JsonConvert.DeserializeObject<ExpandoObject>(testId.ToString());
            int test = (int) testInfo.TestId;
            toReturn = multiExe.StartMouseTracking(test);

            return toReturn;
        }

        [HttpPost]
        [Route("Test/RunTest")]
        public Object RunTest([FromBody] Object testInfo)
        {
            try { 
            //JsonValue castToSystem = JsonValue.Parse((testInfo.ToString()));
            //find type of object
            string type = testInfo.GetType().ToString();
            dynamic toReturn = new ExpandoObject();

            if (type.Equals("Newtonsoft.Json.Linq.JObject"))
            {
                var testToRun = JsonConvert.DeserializeObject<TestInformation>(testInfo.ToString());
                List<TestInformation> singleTest = new List<TestInformation>();
                singleTest.Add(testToRun);
                multiExe.TestList = singleTest;
                multiExe.ExecuteThreadedTests();
                toReturn.TestName = testToRun.TestName;
                toReturn.WebBrowser = testToRun.WebBrowser;

                if (testToRun.MobileEmulation != null)
                {
                    toReturn.MobileEmulation = testToRun.MobileEmulation;
                }

                toReturn.Status = "Executing";
                toReturn.ID = "0";
            }
            else if (type.Equals("Newtonsoft.Json.Linq.JArray"))
            {
                var testList = JsonConvert.DeserializeObject<List<TestInformation>>(testInfo.ToString());
                multiExe.TestList = testList;
                toReturn.Message = "Running " + testList.Count + " Tests";
                toReturn.RunningThreads = multiExe.NumberOfThreads;
                int testId = 0;
                toReturn.TestIDs = new OrderedDictionary();
                foreach (TestInformation info in testList)
                {
                    info.TestNumber = testId;
                    toReturn.TestIDs.Add(info.TestName + "-" + info.WebBrowser, testId.ToString());
                    testId++;
                }
                multiExe.ExecuteThreadedTests();
            }
            return toReturn;
            }
            catch(Exception e)
            {
                Console.WriteLine("Run Test Crashed");
                Console.WriteLine( e.ToString());
                return e.ToString();
            }
        }

        [HttpPost]
        [Route("Test/TestList")]
        public Object ReturnListOfTests([FromBody] JObject directoryJson)
        {
            dynamic toReturn = new ExpandoObject();
            var directory = directoryJson.Property("directory").Value.ToString();
            try
            {
                string[] foundFiles = Directory.GetFiles(directory, "*.xml");

                toReturn.DirectorySearched = directory;
                toReturn.TestsInDirectory = new List<String>();
                foreach (string longFile in foundFiles)
                {
                    var split = longFile.Split('\\');
                    string file = split[split.Count() - 1].Replace(".xml", "");
                    if (file.StartsWith("US"))
                    {
                        toReturn.TestsInDirectory.Add(file);
                    }
                }
            }
            catch (Exception e) { toReturn.Error = e.Message.ToString(); }

            return toReturn;
        }

        [HttpPost]
        [Route("Test/TestFunction")]
        public Object TestFunction([FromBody] JObject functionToTest)
        {
            dynamic toReturn = new ExpandoObject();
            dynamic FunctionToTest = JsonConvert.DeserializeObject<ExpandoObject>(functionToTest.ToString());
            int test = (int)FunctionToTest.TestId;
            List<Object> listOfElements= FunctionToTest.Function;
            List<XElement> FunctionsToTest = new List<XElement>();
            foreach (Object element in listOfElements)
            {
                FunctionsToTest.Add(XElement.Parse((String)element));
            }
             
            toReturn = multiExe.TestFunction(test, FunctionsToTest);

            return toReturn;
        }

        [HttpPost]
        [Route("Xml/GenerateTestScript")]
        public Object GenerateTestScript([FromBody] JObject testScript)
        {
            dynamic toReturn = new ExpandoObject();

            JsonValue TestScript = JsonValue.Parse((testScript.ToString()));
            //get test name
            string name = TestScript["testName"];
            //get url
            string url = TestScript["testUrl"];
            //get output location
            string outputLocation = TestScript["outputLocation"];
            //make Root Element
            XElement root = new XElement("TestData", new XAttribute("name", name), new XAttribute("url", url));

            //find list of journey
            var Journey = TestScript["testData"];

            if (Journey.JsonType.ToString().Equals("Object", ignoringCase))
            {
                JsonToJourney(root, Journey);
            }

            else
            {
                foreach (JsonValue journeyGroup in Journey)
                {
                    JsonToJourney(root, journeyGroup);
                }
            }
            XDocument testScriptXml = new XDocument(root);
            //append testname to file.]
            outputLocation += @"\" + name + ".xml";

            testScriptXml.Save(outputLocation);
            toReturn.Message = "Test Script Created";
            toReturn.Location = outputLocation;
            return toReturn;
        }
        //----------------------------------------------------Posts--------------------------------------

        //----------------------------------------------------Methods------------------------------------

        private void JsonToJourney(XElement root, JsonValue journeyGroup)
        {
            //journey name
            string journeyName = journeyGroup["journey"];
            //build journey element
            XElement journeyElement = new XElement("Journey", new XAttribute("name", journeyName));

            //find phase names
            var phases = journeyGroup["phaseData"];
            //check if only one thing
            if (phases.JsonType.ToString().Equals("Object", ignoringCase))
            {
                JsonToPhase(journeyElement, phases);
            }

            else//loop though the phases
            {
                foreach (JsonValue phase in phases)
                {
                    JsonToPhase(journeyElement, phase);
                }
            }
            root.Add(journeyElement);
        }

        private void JsonToPhase(XElement journeyElement, JsonValue phase)
        {
            string phaseName = phase["phase"];

            //make element for phase
            XElement phaseElement = new XElement("Phase", new XAttribute("name", phaseName));
            //loop though phase values
            try
            {
                var values = phase["phaseValue"];
                JsonToPhaseValue(phaseElement, values);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            journeyElement.Add(phaseElement);
        }

        private void JsonToPhaseValue(XElement phaseElement, JsonValue value)
        {
            Regex toReplace = new Regex("[\"{}]");
            List<String> stringValues = new List<string>();
            //check for many values
            var count = value.Count;
            if (count > 1)
            {
                var splitValues = value.ToString().Split(new string[] { "\"}," }, StringSplitOptions.None).ToList();
                foreach (string jsonValue in splitValues)
                {
                    stringValues.Add(toReplace.Replace(jsonValue, ""));
                }
            }
            else
            {
                var replaced = toReplace.Replace(value.ToString(), "");
                stringValues.Add(replaced);
            }

            //break down json into strings of values
            foreach (String stringValue in stringValues)
            {
                string replaced = stringValue;
                string name = replaced.Split(':')[0].Trim();
                replaced = replaced.Replace(name + ":", "").Trim();
                var values = replaced.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                if (replaced != "")
                {
                    //make element
                    XElement PhaseValue = new XElement("phaseValue");
                    XAttribute atName = new XAttribute("name", name);
                    PhaseValue.Add(atName);
                    //get values
                    foreach (String item in values)
                    {

                        var trimItem = item.Trim();

                        if (trimItem.StartsWith("value", ignoringCase))
                        {
                            string phaseValue = trimItem.Split(':')[1].Trim();
                            PhaseValue.Value = phaseValue;
                        }
                        else
                        {
                            var splitItem = trimItem.Split(':');
                            XAttribute attribute = new XAttribute(splitItem[0], splitItem[1].Trim());
                            PhaseValue.Add(attribute);
                        }

                    }
                    //add phasevalue element to phase element
                    phaseElement.Add(PhaseValue);
                }
            }
        }

        private OrderedDictionary PhaseToObject(XElement phase)
        {
            //turn element into object
            var phaseObject = new OrderedDictionary();
            var attributes = phase.Attributes().ToList();
            foreach (XAttribute att in attributes)
            {
                phaseObject.Add(att.Name.ToString(), att.Value);
            }
            var happy = phase.Nodes().OfType<XText>().First().ToString().Trim();
            if (!happy.Equals(""))
            {
                phaseObject.Add("happyDefault", happy);
            }

            return phaseObject;
        }

        private string GetValue(XElement element, string attribute)
        {
            string toReturn = "";
            try { toReturn = element.Attribute(attribute).Value; } catch { Console.WriteLine(attribute + " : not found"); }
            return toReturn;
        }

        private Object ElementToObject(XElement element)
        {
            dynamic magPhase = new ExpandoObject();
            magPhase.elementId = GetValue(element, "name");

            //get any value
            try { magPhase.value = element.Nodes().OfType<XText>().First().ToString().Trim(); } catch { }

            //Look for the type of input
            string _InputType = (string)GetValue(element, "type");
            if (!_InputType.Equals(""))
            {
                if (_InputType.Equals("int", ignoringCase))
                {
                    _InputType = "number";
                }
                else if (_InputType.Equals("string", ignoringCase))
                {
                    _InputType = "text";
                }
                else if (_InputType.Equals("boolean", ignoringCase))
                {
                    _InputType = "checkbox";
                }
                else if (_InputType.Equals("dateTime", ignoringCase))
                {
                    _InputType = "date";
                }
                else if (_InputType.Equals("dropDown", ignoringCase))
                {
                    _InputType = "dropdown";
                    //if its a dropdown, loop though children elements and get the options
                    List<XElement> dropdownOptions = element.Elements().ToList();
                    magPhase.dropDownItems = new List<string>();
                    foreach (XElement drop in dropdownOptions)
                    {
                        magPhase.dropDownItems.Add(drop.FirstNode.ToString().Trim());
                    }
                }
                else if (_InputType.Equals("list", ignoringCase))
                {
                    _InputType = "list";
                    List<XElement> listItems = element.Elements().ToList();
                    magPhase.listItems = new List<Object>();

                    foreach (XElement listElement in listItems)
                    {
                        magPhase.listItems.Add(ElementToObject(listElement));
                    }
                }
                magPhase.inputType = _InputType;
            }

            string initialCount = GetValue(element, "initialCount");
            if (!initialCount.Equals(""))
            {
                magPhase.initialCount = initialCount;
            }

            string text = GetValue(element, "labelText");
            if (!text.Equals(""))
            { magPhase.text = text; }

            string max = GetValue(element, "max");
            if (!max.Equals(""))
            {
                magPhase.max = max;
            }

            string min = GetValue(element, "min");
            if (!min.Equals(""))
            {
                magPhase.min = min;
            }

            return magPhase;
        }
    }
}
