using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TestExecution
{
    public partial class Form1 : Form
    {
        private int apiPort = 26000;

        public class ElementIdentifierWrapper
        {//think of this as a DTO from the API, instead of using a reference to the front
            public String elementId { get; set; }
            public String elementName { get; set; }
            public String elementXpath { get; set; }
            public String journey { get; set; }

            public override String ToString()
            {
                String toReturn = "";

                toReturn = toReturn + "elementId: " + elementId + "\n";
                toReturn = toReturn + "elementName: " + elementName + "\n";
                toReturn = toReturn + "elementXpath: " + elementXpath + "\n";
                //toReturn = toReturn + "journey: " + journey + "\n";
                return toReturn;
            }
        }


        private TestExecutionGenerator testExecutionGenerator;
        private JsonValue testsCurrentlyRunning;
        private Dictionary<String, TestRunnerTab> TestTabs = new Dictionary<string, TestRunnerTab>();
        private TestExecutorListener listener;

        public Form1()
        {
            try
            {
                Console.WriteLine("Starting program");
                InitializeComponent();
                splashScreen();
                TestExecutor();
                UserName.Text = Environment.UserName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void splashScreen(Boolean stayUp = false)
        {
            SplashLanding.BringToFront();
            SplashLanding.Visible = true;
            SplashLanding.Enabled = true;
            RTB_SplashScreen_ChangeLog.Clear();
            //try to load changelog file
            try
            {
                string logDir = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\ChangeLog.txt";
                string changeLog = File.ReadAllText(logDir, Encoding.UTF8);
                //check the modification date of the log
                var lastModified_ChangeLog = File.GetLastWriteTime(logDir).DayOfYear;
                Console.WriteLine("ChangeLog day: " + lastModified_ChangeLog);

                //check creation date of files being executed
                var executionDirectory = Directory.GetCurrentDirectory();
                Console.WriteLine("Execution Directory: "+ executionDirectory);

                var lastModified_Api = File.GetLastWriteTime(executionDirectory+@"\SelfHostApi.exe").DayOfYear;

                var lastModified_Execution = File.GetLastWriteTime(executionDirectory + @"\TestExecution.exe").DayOfYear;
                
                if(lastModified_ChangeLog>lastModified_Api)
                {
                    RTB_SplashScreen_ChangeLog.Text += "Please Update to the newest version of the SelfHostApi\n\r\n\r";
                    Console.WriteLine("SelfHostApi day: "+ lastModified_Api);
                    stayUp = true;
                }
                if (lastModified_ChangeLog > lastModified_Execution)
                {
                    RTB_SplashScreen_ChangeLog.Text += "Please Update to the newest version of Deep Thought\n\r\n\r";
                    Console.WriteLine("Deep Thought day: " + lastModified_Execution);
                    stayUp = true;
                }
                if (stayUp)
                {
                    //set rtb to changelog
                    RTB_SplashScreen_ChangeLog.Text += changeLog;
                    Console.WriteLine(RTB_SplashScreen_ChangeLog.Text);
                }
                else
                {
                    SplashLanding.Visible = false;
                    SplashLanding.Enabled = false;
                    SplashLanding.SendToBack();
                }

            }
            catch(Exception e)
            {
                SplashLanding.Visible = false;
                SplashLanding.Enabled = false;
                SplashLanding.SendToBack();
                Console.WriteLine(e.ToString());
            }
            LblOutput.Text = "";
        }
        private void BTN_SplashScreen_Ok_Click(object sender, EventArgs e)
        {
            SplashLanding.Visible = false;
            SplashLanding.Enabled = false;
        }

        private void BtnInfo_Click(object sender, EventArgs e)
        {
            splashScreen(true);
        }

        public void TestExecutor()
        {
            this.testExecutionGenerator = new TestExecutionGenerator(this.TestExecution, this.DirectoryToTests, this.GetTestsButton, this.ExecuteTestsButton, this.testDirectoryFinder, this.TestListPanel);
            testExecutionGenerator.generateTests();
        }

        private void ExecuteTestsButton_Click(object sender, EventArgs e)
        {
            //clear current tabs
            //var tabsOpen = TestRunnerPanel.TabPages;
            //int openTabs = tabsOpen.Count;
            //for(int i=2; i<openTabs; i++)
            //{
            //    TestRunnerPanel.TabPages[2].Dispose();
            //}

            try
            {
                var testsList = testExecutionGenerator.MagTestListPopulator.TestToExecute();

                if (testsList != null && testsList.Count > 0)
                {             
                    //set number of threads
                    String threadsToRun = ThreadNumberSelction.Value.ToString();
                    var ThreadResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/Threads/" + threadsToRun);

                    //if using local, change dir for dictionaryDirectory
                    if (UseLocalChkBx.Checked)
                    {
                        //get local Dir
                        String current = Directory.GetCurrentDirectory();

                        Console.WriteLine("Directory Before checking for Data: "+current);
                        if (current.EndsWith("Data2"))
                        {
                            Console.WriteLine("Ends With Data2");
                            current = current.Replace("Data2", "");
                        }
                        else if (current.EndsWith("Data"))
                        {
                            Console.WriteLine("contains Data");
                            current = current.Replace("Data", "");
                        }

                        //current = Directory.GetParent(current).ToString();
                        var dictionary = current + @"\Element Dictionary";
                        var DriverLoc = current + @"\WebDrivers";
                        Console.WriteLine("Current Directory: " + current);
                        Console.WriteLine("Dictionary Directory: " + dictionary);
                        Console.WriteLine("Driver Directory: " + DriverLoc);
                        //add dir to each object
                        foreach (dynamic test in testsList)
                        {
                            test.dictionaryDirectory = dictionary;
                            test.chromeDirectory = DriverLoc;
                            test.firefoxDirectory = DriverLoc;
                        }
                    }

                    this.testsCurrentlyRunning = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Test/RunTest", testsList);


                    var tests = testsCurrentlyRunning["TestIDs"];
                    listener = new TestExecutorListener(apiPort+1, LogOutPut, this);
                    listener.InitialiseListener();

                    foreach (KeyValuePair<string, JsonValue> nameID in tests)
                    {
                        int tabLocation = Int32.Parse(nameID.Value);
                        var testTab = new TestRunnerTab(nameID.Key, nameID.Value, apiPort);
                        testTab.Functions = Functions;
                        testTab.createTestRunnerTab();
                        //this.TestRunnerPanel.Controls.Add(testTab.TestRunTab);
                        //TestRunnerPanel.TabPages.Insert(tabLocation, testTab.TestRunTab);
                        //TestRunnerPanel.TabPages.Add(testTab.TestRunTab);
                        TestRunnerPanel.TabPages.Add(testTab.MotherPage);
                        TestTabs.Add(nameID.Value, testTab);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        internal void RecieveMessage(JsonValue message)
        {
            String content = message["Content"];
            String BodySent = message["BodySent"];
            String id = "";
            TestRunnerTab recievingTest = null; ;

            if (content.Equals("System Message") && BodySent.Equals("All Completed"))
            {
                listener.startAbort();
            }
            else
            {
                try
                {
                    id = message["Identifier"];
                    recievingTest = TestTabs[id];
                    if (content.Equals("Log Message"))
                    {
                        recievingTest.updateLogContent(BodySent);
                    }
                    else if (content.Equals("System Message"))
                    {
                        recievingTest.SystemMessage(BodySent);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void StartPause_Click(object sender, EventArgs e)
        {

            var testResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/Pause/" + this.StartPause.Name);
            if (testResponse["Paused"])
            {
                this.StartPause.Text = "Play";
            }
            else
            {
                this.StartPause.Text = "Pause";
            }

        }

        private void Stop_Click(object sender, EventArgs e)
        {
            var testResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/Stop/" + this.StopBut.Name);
        }

        private void SearchForTermBtn_Click(object sender, EventArgs e)
        {
            String term = SearchTermBox.Text;

            checkForDefinition(term);
        }

        private Boolean checkForDefinition(String term)
        {
            return checkForDefinition(term, "all");
        }

        private Boolean checkForDefinition(String term, String Journey)
        {
            dynamic searchRequest = new ExpandoObject();
            searchRequest.SearchTerm = term + "---***---" + Journey;
            searchRequest.Directory = testDirectoryFind.FileName;

            var searchResult = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/CheckDefinition", searchRequest);
            searchRequest = JsonConvert.DeserializeObject<ExpandoObject>(searchResult.ToString());

            Boolean found = (Boolean)searchRequest.Found;
            String Result = "";
            if (found)
            {
                LocateInPhaseBtn.Enabled = true;

                Result = searchRequest.Result;
            }
            else
            {
                Result = "No Element found with term: " + term;
            }
            SearchResultsRTB.Text = Result;
            return found;
        }

        private List<string> comboSource = new List<string>();

        private void LoadDictionary_Click(object sender, EventArgs e)
        {
            if(FunctionElementTemplateList.Items.Count<=1)
            {
                foreach(String function in Functions.Keys)
                {
                    FunctionElementTemplateList.Items.Add(function);
                }
            }

            if (testDirectoryFind.ShowDialog() == DialogResult.OK)
            {
                String file = testDirectoryFind.FileName;

                CreationToolTabControl.Enabled = true;
                DictionaryDirectory.Text = file;
                //get dictionary from API
                dynamic request = new ExpandoObject();
                request.Directory = file;

                var responseJson = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/DictionaryTools/Dictionary", request);

                var mainDictionary = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, ElementIdentifierWrapper>>>(responseJson.ToString());

                if (mainDictionary.Count == 0)
                {
                    SearchResultsRTB.Text = "There was a problem loading the dictionary. Please check SelfHostApi console output for exceptions";
                }
                else
                {
                    //set up journey based dictionarys
                    var subDictionarys = mainDictionary.Keys;

                    foreach (var journey in subDictionarys)
                    {
                        comboSource.Add(journey);
                    }
                    JourneyCombo.DataSource = comboSource;
                    SearchForTermBtn.Enabled = true;
                    AddDefBtn.Enabled = true;
                    //show panel when its hidden later}
                }
            }
        }

        private void LoadTestBtn_Click(object sender, EventArgs e)
        {
            string oldFilter = testDirectoryFind.Filter;
            testDirectoryFind.Filter = "xml|US_*.xml";
            if (testDirectoryFind.ShowDialog() == DialogResult.OK)
            {
                String file = testDirectoryFind.FileName;

                SaveFileLocationTxtBx.Text = file;

                //load up test info
                currentTemplate = XDocument.Load(file).Root;

                //set boxes to reflect the loaded test
                TestNameTxtBx.Text = currentTemplate.Attribute("name").Value;
                TargetUrlRtb.Text = currentTemplate.Attribute("url").Value;
                RtbInProgressBox.Text = currentTemplate.ToString();
            }
            testDirectoryFind.Filter = oldFilter;
        }
        //here
        private void BtnClearFields_Click(object sender, EventArgs e)
        {
            ElementIdTxtBox.Text="";
            elementIdentifierTxtBox.Text="";
            ElementNameTxtbox.Text = "";
            ElementxPathTxtBox.Text = "";
        }

        private void AddDefBtn_Click(object sender, EventArgs e)
        {
            String file = testDirectoryFind.FileName;

            String Journey = JourneyCombo.SelectedValue.ToString();
            String Id = ElementIdTxtBox.Text.Trim();
            String Identifier = elementIdentifierTxtBox.Text.Trim();
            String name = ElementNameTxtbox.Text.Trim();
            String xPath = ElementxPathTxtBox.Text.Trim();
            String junk = "------*****------";

            if ((Id == "") && (name == ""))
            {
                ErrorLabel.Text = "No values to save";
                return;
            }
            if (Identifier == "")
            {
                ErrorLabel.Text = "No Identifier added";
                return;
            }
            if (name == "")
            {
                name = junk;
            }
            if (Id == "")
            {
                Id = junk;
            }

            dynamic defintionObject = new ExpandoObject();
            defintionObject.elementId = Id;
            defintionObject.elementName = name;
            defintionObject.elementXpath = xPath;
            defintionObject.journey = Journey;
            defintionObject.identifier = Identifier;
            defintionObject.Directory = file;

            var searchResultJson = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/AddDefintion", defintionObject);

            dynamic searchResult = JsonConvert.DeserializeObject<ExpandoObject>(searchResultJson.ToString());

            ErrorLabel.Text = searchResult.Result;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (listener != null)
            {
                listener.startAbort();
            }
            if (mouseThread != null)
            {
                StopMouseThread();
            }
        }

        private void LocateInPhaseBtn_Click(object sender, EventArgs e)
        {
            LocatedPhaseRtb.Text = "";
            //build list of elementId's from previous textbox
            List<String> elementIds = new List<string>();
            for (int i = 0; i < SearchResultsRTB.Lines.Count(); i++)
            {
                String line = SearchResultsRTB.Lines[i];
                if (line.Contains("Element Identifier: "))
                {
                    var elementId = line.Replace("Element Identifier: ", "");
                    if (!elementIds.Contains(elementId))
                    {
                        elementIds.Add(elementId);
                    }
                }
            }

            dynamic searchTerms = new ExpandoObject();
            searchTerms.ListOfSearchTerms = elementIds;
            searchTerms.Directory = testDirectoryFind.FileName;
            searchTerms.PhasesDirectory = @"O:\SeleniumLite\EXPERIMENTAL\Journeys";//hard coded for now until a dedicated textbox is added

            var searchResult = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/CheckDefinition/LocateInPhases", searchTerms);

            //rebuild expando and pull details from it
            dynamic response = JsonConvert.DeserializeObject<ExpandoObject>(searchResult.ToString());

            LocatedPhaseRtb.Text = response.Result;

            CreationToolTabControl.SelectedIndex = 2;
        }

        //--------------------------------------Mouse Tracking/Get Element By Click-------------------
        Thread mouseThread;
        Boolean stopThread;
        private void StartMouseListen_Click(object sender, EventArgs e)
        {
            //start thread for detecting mouse clicks
            mouseThread = new Thread(() => MousePosOnClick());
            mouseThread.Start();
            StopTrackBtn.Enabled = true;
            StartMouseListen.Enabled = false;
        }
        public void UpdateMousePosBox(String textToAdd)
        {
            MouseLocRtb.Text += textToAdd;
        }

        private void MousePosOnClick()
        {
            Point position = new Point();
            stopThread = false;
            while (!stopThread)
            {
                Boolean ctrl = (Control.ModifierKeys == Keys.Control);
                Boolean RightClick = (MouseButtons == MouseButtons.Right);
                if (ctrl)
                {
                    if (RightClick)
                    {
                        position = MousePosition;
                        dynamic objectToSend = new ExpandoObject();
                        objectToSend.MouseX = position.X;
                        objectToSend.MouseY = position.Y;
                        JsonObject returnedDetails = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/ElementAtPosition/", objectToSend);

                        dynamic elementDetails = JsonConvert.DeserializeObject<ExpandoObject>(returnedDetails.ToString());

                        //pull apart response
                        String elementId = elementDetails.id;
                        String elementName = elementDetails.name;
                        String elementxPath = elementDetails.xPath;

                        this.BeginInvoke(new Action(delegate
                        {
                            UpdateMousePosBox("X:" + position.X + Environment.NewLine);
                            UpdateMousePosBox("Y:" + position.Y + Environment.NewLine);
                            UpdateMousePosBox("element's Id:" + elementId + Environment.NewLine);
                            UpdateMousePosBox("element's name:" + elementName + Environment.NewLine);
                            UpdateMousePosBox("element's xpath: " + elementxPath + Environment.NewLine + Environment.NewLine);

                            //search for element in dictionary
                            Boolean inDictionary = false;
                            if (elementId != "No Element Found")
                            {
                                if (elementId != "")
                                {
                                    inDictionary = checkForDefinition(elementId);
                                }
                                if (!inDictionary && elementName != "")
                                {
                                    inDictionary = checkForDefinition(elementName);
                                }
                                if (!inDictionary)
                                {
                                    inDictionary = checkForDefinition(elementxPath);
                                }

                                if (!inDictionary)
                                {
                                    //prepopulate add definition fields
                                    elementIdentifierTxtBox.Text = "Element Was Not Found In Dictionary";
                                    ElementIdTxtBox.Text = elementId;
                                    ElementNameTxtbox.Text = elementName;
                                    ElementxPathTxtBox.Text = elementxPath;
                                    CreationToolTabControl.SelectedIndex = 1;
                                }
                                if (inDictionary)
                                {
                                    CreationToolTabControl.SelectedIndex = 0;
                                }
                            }
                        }
                        ));
                    }
                }
            }
            Console.WriteLine("Mouse Tracking Thread Ended");
        }

        private void StopTrackBtn_Click(object sender, EventArgs e)
        {
            StopMouseThread();
        }

        private void StopMouseThread()
        {
            stopThread = true;
            if (mouseThread != null)
            { mouseThread.Abort(); }
            StopTrackBtn.Enabled = false;
            mouseThread = null;
        }

        private void LaunchDevDriverBtn_Click(object sender, EventArgs e)
        {
            CloseDriverBtn.Enabled = true;

            dynamic webInfo = new ExpandoObject();
            webInfo.ChromeDirectory = @"O:\SeleniumLite\EXPERIMENTAL\SeleniumBackEnd\WebDrivers";
            webInfo.Url = DevUrlTxtBox.Text;

            var testResponse = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/ElementAtPosition/StartDriver", webInfo);

            StartMouseListen.Enabled = true;
        }

        private void CloseDriverBtn_Click(object sender, EventArgs e)
        {
            StopMouseThread();
            RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Dev/ElementAtPosition/CloseDriver");
            CloseDriverBtn.Enabled = false;

            StartMouseListen.Enabled = false;
        }

        //--------------------------------------Mouse Tracking/Get Element By Click-------------------


        //--------------------------------------Test Template Creation--------------------------------

        private XElement generateTemplate()
        {
            String name = TestNameTxtBx.Text;
            //check if test name starts with US_
            if (!name.StartsWith("US_"))
            {
                name = "US_" + name;
            }

            String URL = TargetUrlRtb.Text.Trim();

            //make root element
            XElement TestRoot = new XElement("TestData");
            XAttribute testName = new XAttribute("name", name);
            XAttribute testUrl = new XAttribute("url", URL);
            TestRoot.Add(testName);
            TestRoot.Add(testUrl);

            if (Chk_SinglePage.Checked)
            {
                XAttribute singelPage = new XAttribute("singlePage", "true");
                TestRoot.Add(singelPage);
            }


            XElement Data = new XElement("Data");
            XElement Function = new XElement("Function");

            TestRoot.Add(Data);
            TestRoot.Add(Function);

            return TestRoot;
        }

        private void Btn_TemplateSaveLocation_Click(object sender, EventArgs e)
        {
            if (testDirectoryFinder.ShowDialog() == DialogResult.OK)
            {
                SaveFileLocationTxtBx.Text = testDirectoryFinder.SelectedPath;
            }
        }

        private void Btn_SaveTemplate_Click(object sender, EventArgs e)
        {
            RtbToXml();
            XElement template = currentTemplate;
            if (template == null)
            {
                template = generateTemplate();
            }

            String name = template.Attribute("name").Value;
            String saveLocation = SaveFileLocationTxtBx.Text;
            if (!saveLocation.EndsWith(".xml"))
            {
                saveLocation = saveLocation + "\\" + name + ".xml";
            }
            
            //save template
            template.Save(saveLocation);
            LblOutput.Text = "Saved to: " + saveLocation;
        }

        private void RtbToXml()
        {
            String RtbValue = RtbInProgressBox.Text.Trim();
            
            if(RtbValue!="")
            {
                try
                {
                    currentTemplate = XElement.Parse(RtbValue);
                }catch (Exception e)
                {
                }
            }
        }

        XElement currentTemplate = null;
        private StringComparison IgnoringCase = StringComparison.InvariantCultureIgnoreCase;
        private void BtnAddPhaseValue_Click(object sender, EventArgs e)
        {
            if (currentTemplate == null)
            {
                currentTemplate = generateTemplate();
            }
            String rawPhaseValue = TxtBx_PhaseValue.Text;
            //convert to xelement
            var phaseValue = XElement.Parse(rawPhaseValue);

            //check the value conforms to requirements
            List<String> types = new List<string>()
            {
                "string",
                "boolean",
                "datetime",
                "int"
            };
            Boolean fine = false;
            String type = phaseValue.Attribute("type").Value;
            foreach (String typeList in types)
            {
                if (type.Equals(typeList, IgnoringCase))
                {
                    fine = true;
                    break;
                }
            }
            if (!fine)
            {
                LblOutput.Text = "phaseValue type was not a valid option";
                return;
            }

            String phaseValueName = phaseValue.Attribute("name").Value;
            if (phaseValueName == "")
            {
                LblOutput.Text = "phaseValue name is blank";
                return;
            }
            //check if value of same name is already added
            //generate list of currently used phaseValue names
            var phaseValues = currentTemplate.Element("Data").Elements("phaseValue").ToList();

            if(phaseValues.Count>0)
            {
                foreach(var value in phaseValues)
                {
                    var usedName = value.Attribute("name").Value;
                    if(phaseValueName.Equals(usedName,IgnoringCase))
                    {
                        LblOutput.Text = "phaseValue name is already in use";
                        return;
                    }
                }
            }

            currentTemplate.Element("Data").Add(phaseValue);

            RtbInProgressBox.Text = currentTemplate.ToString();
            LblOutput.Text = "phaseValue added";
        }

        private void BtnGenerateTemplate_Click(object sender, EventArgs e)
        {
            var template = generateTemplate();
            RtbInProgressBox.Text = template.ToString();
            currentTemplate = template;
        }

        private void BtnAddFunction_Click(object sender, EventArgs e)
        {
            if (currentTemplate == null)
            {
                currentTemplate = generateTemplate();
            }

            String rawFunction = TxtBxFunctionElement.Text;
            XElement functionElement = null;
            //convert to xelement
            try {
            functionElement = XElement.Parse(rawFunction);
                }
            catch(Exception exe)
            {
                LblOutput.Text = "Function Could not be added";
                Console.WriteLine(exe.ToString());
                return;
            }
            currentTemplate.Element("Function").Add(functionElement);
            RtbInProgressBox.Text = currentTemplate.ToString();
            LblOutput.Text = "FunctionElement added";
        }

        public static Dictionary<String, String> Functions { get; private set; } = new Dictionary<string, string>()
        {
            {"click","<elementManager type = \"click\" target= \"ELEMENT IDENTIFIER\"/>"},
            {"sendkeys","<elementManager type= \"sendkeys\" target= \"ELEMENT IDENTIFIER\">PHASEVALUE TO SEND</elementManager>"},
            {"pause","<elementManager type= \"pause\" value = \"indefinite\"/>"},
            {"elementAvailable","<elementManager type= \"elementAvailable\" target= \"ELEMENT IDENTIFIER\"/>"},
            {"getText","<elementManager type= \"getText\" target= \"ELEMENT IDENTIFIER\"/>"},
            {"selectFirstOption","<elementManager type= \"selectFirstOption\" target= \"ELEMENT IDENTIFIER\"/>"},
            {"scrollTo","<elementManager type= \"scrollTo\" target= \"ELEMENT IDENTIFIER\"/>"},
            {"setPhaseValue","<setPhaseValue name = \"\" type = \"\">VALUE</setPhaseValue>"},
            {"getPhaseValue","<getPhaseValue target=\"\"/>"},
            {"disposePhaseValue ","<disposePhaseValue  target=\"\"/>"},
            {"screenshot","<testLogger type= \"screenshot\"/>"},
            {"recordToFile","<testLogger type= \"recordToFile\" status= \"info\"></testLogger>"},
            {"getUrl","<selenium type= \"geturl\"/>"},
            {"endTest","<selenium type= \"endTest\"/>"},
            {"navigateToUrl","<selenium type = \"navigateToUrl\">URL GOES HERE</selenium>"},
            {"ifBoolean","<if type= \"ifBoolean\" condition = \"\">PHASEVALUE"+Environment.NewLine+"</if>"},
            {"ifString","<if type= \"ifString\" condition = \"\" stringsToMatch=\"\">PHASEVALUE"+Environment.NewLine+"</if>"},
            {"ifInt","<if type= \"ifInt\" condition = \"\" int=\"\">PHASEVALUE"+Environment.NewLine+"</if>"},
            {"ifNull","<if type= \"ifNull\" condition = \"\" >PHASEVALUE"+Environment.NewLine+"</if>"},
            {"newTab_PhaseValue","<windowControl type=\"newTab\" tabName = \"\" target = \"PHASEVALUE\" />"},
            {"newTab_Hardcoded","<windowControl type=\"newTab\" tabName = \"\" >URL GOES HERE</windowControl>"},
            {"closeTab_tabName","<windowControl type=\"closeTab\" tabName = \"\" />"},
            {"closeTab_current","<windowControl type=\"closeTab\" />"},
            {"checkForNewTab","<windowControl type = \"checkForNewTab\"/>"},
            {"switchToTab_PhaseValue","<windowControl type=\"switchToTab\">PHASEVALUE</windowControl>"},
            {"switchToTab_Hardcoded","<windowControl type=\"switchToTab\" tabName = \"HARD_CODED\"/>"},
            {"getWindowTitle","<windowControl type = \"getWindowTitle\"/>"},
            {"setWindowTitle_PhaseValue","<windowControl type=\"setWindowTitle\">PHASEVALUE</windowControl>"},
            {"setWindowTitle_Hardcoded","<windowControl type=\"setWindowTitle\" tabName = \"HARD_CODED\"/>"},
            {"resize","<windowControl type=\"resize\" height = \"full\" width = \"500\"/>"},
            {"innerFunction","<innerFunction name=\"secondJob\">"+Environment.NewLine+"</innerFunction>"},
            {"innerFunctionCall","<innerFunctionCall target =\"\"/>"},
            {"Phase","<Phase name= \"\" package = \"\" />"}
        };

        private void FunctionElementTemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //populate function textbox with template from dictionary
            TxtBxFunctionElement.Text = Functions[FunctionElementTemplateList.Text];
        }

        private void BtnFindFirstDictionary_Click(object sender, EventArgs e)
        {
            if (testDirectoryFind.ShowDialog() == DialogResult.OK)
            {               
                ChkBxUseAlreadyLoaded.Checked = false;
                TxtBxFirstDictionaryLocation.Text = testDirectoryFind.FileName;
                EnableMerge();
            }
        }

        private void BtnFindSecondDictionary_Click(object sender, EventArgs e)
        {
            if (testDirectoryFind.ShowDialog() == DialogResult.OK)
            {
                TxtBxSecondDictionaryLocation.Text = testDirectoryFind.FileName;
                EnableMerge();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkBxUseAlreadyLoaded.Checked)
            {
                TxtBxFirstDictionaryLocation.Text = DictionaryDirectory.Text;
            }
            else
            {
                TxtBxFirstDictionaryLocation.Text = "";
            }
        }

        private void EnableMerge()
        {
            if(TxtBxFirstDictionaryLocation.Text.EndsWith("ElementDictionary.xml") && TxtBxSecondDictionaryLocation.Text.EndsWith("ElementDictionary.xml"))
            {
                BtnMergeDictionarys.Enabled = true;
            }
            else
            {
                BtnMergeDictionarys.Enabled = false;
            }
        }

        private void BtnMergeDictionarys_Click(object sender, EventArgs e)
        {            
            dynamic request = new ExpandoObject();
            request.FirstDirectory = TxtBxFirstDictionaryLocation.Text;
            request.SecondDirectory = TxtBxSecondDictionaryLocation.Text;

            var responseJson = RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Dev/DictionaryTools/DictionaryMerge", request);            
        }

        private void ListenerPortNum_ValueChanged(object sender, EventArgs e)
        {
            apiPort = Int32.Parse(ListenerPortNum.Value.ToString());
        }
    }
}
