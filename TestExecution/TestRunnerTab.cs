using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Json;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TestExecution
{
    internal class TestRunnerTab
    {
        private string testName;
        private JsonValue testId;
        public TabPage TestRunTab { get; private set; }

        public TabPage DevelopmentTab { get; private set; }
        public TextBox ElementEditorTxtBx { get; private set; }
        public RichTextBox functionRTB { get; private set; }
        public ListBox ElementsBox { get; private set; }
        public Button BtnAddFunction { get; private set; }
        public Button BtnExecuteFunction { get; private set; }
        private int apiPort { get; set; }


        public TabPage MotherPage { get; private set; }
        public TabControl InternalTabs { get; private set; }
        private Button StopBut;
        private Button StartPause;
        private Button Screenshot;
        private Button TrackMouseBtn;
        private Button FocusWindowBtn;
        private Label testLabel;
        private RichTextBox LogOutPut;
        public Boolean TestEnded { get; set; } = false;

        public Dictionary<String, String> Functions { get; set; }
        

        public TestRunnerTab(string key, JsonValue value, int apiPort)
        {
            this.testName = key;
            this.testId = value;
            this.apiPort = apiPort;
        }

        public void DevelopmentTabCreation()
        {
            DevelopmentTab = new TabPage();

            this.DevelopmentTab.Location = new Point(0, 0);
            this.DevelopmentTab.Padding = new Padding(3);
            this.DevelopmentTab.Size = new Size(760, 400);
            this.DevelopmentTab.Text = "Test Development";
            this.DevelopmentTab.UseVisualStyleBackColor = true;

            DevelopmentTab.BackColor = Color.DimGray;

            //add rich text box
            functionRTB = new RichTextBox();
            functionRTB.Location = new Point(0, 50);
            functionRTB.Name = "FunctionExperimentation";
            functionRTB.ScrollBars = RichTextBoxScrollBars.Vertical;
            functionRTB.Size = new Size(DevelopmentTab.Size.Width-150, DevelopmentTab.Size.Height - 80);
            functionRTB.TabIndex = 1;
            functionRTB.Text = "";
            functionRTB.WordWrap = true;
            functionRTB.AcceptsTab = true;
            DevelopmentTab.Controls.Add(functionRTB);

            //add list box
            ElementsBox = new ListBox();
            ElementsBox.FormattingEnabled = true;
            ElementsBox.Location = new Point(functionRTB.Size.Width-3 + 10,50);
            ElementsBox.Name = "FunctionElementTemplateListBox";
            ElementsBox.Size = new Size(DevelopmentTab.Size.Width-10- functionRTB.Size.Width, functionRTB.Size.Height-18);
            ElementsBox.TabIndex = 2;
            foreach (String function in Functions.Keys)
            {
                ElementsBox.Items.Add(function);
            }
            ElementsBox.SelectedIndexChanged += new EventHandler(FunctionElementTemplateList_SelectedIndexChanged);

            DevelopmentTab.Controls.Add(ElementsBox);

            //add Element Demo textbox
            ElementEditorTxtBx = new TextBox();
            ElementEditorTxtBx.Location = new Point(0, 25);
            ElementEditorTxtBx.Name = "TxtBxFunctionElement";
            ElementEditorTxtBx.Size = new Size(functionRTB.Size.Width+50, 20);
            ElementEditorTxtBx.TabIndex = 3;
            ElementEditorTxtBx.Text = "<elementManager type=\"\"></elementManager>";
            DevelopmentTab.Controls.Add(ElementEditorTxtBx);

            //add function button
            BtnAddFunction = new Button();
            BtnAddFunction.Location = new Point(ElementEditorTxtBx.Size.Width+8, 25);
            BtnAddFunction.Name = "BtnAddFunction";
            BtnAddFunction.Size = new Size(90, 25);
            BtnAddFunction.TabIndex = 4;
            BtnAddFunction.Text = "Add Function";
            BtnAddFunction.UseVisualStyleBackColor = true;
            BtnAddFunction.Click += new EventHandler(this.BtnAddFunction_Click);
            DevelopmentTab.Controls.Add(BtnAddFunction);

            //add execution button
            BtnExecuteFunction = new Button();
            BtnExecuteFunction.Text = "Execute Function";
            BtnExecuteFunction.Location = new Point(ElementsBox.Location.X, ElementsBox.Location.Y+ ElementsBox.Size.Height-5);
            BtnExecuteFunction.Name = "BtnExecuteFunction";
            BtnExecuteFunction.Size = new Size(90, 25);
            BtnExecuteFunction.TabIndex = 5;
            BtnExecuteFunction.UseVisualStyleBackColor = true;

            BtnExecuteFunction.Click += new EventHandler(BtnExecuteFunction_Click);
            DevelopmentTab.Controls.Add(BtnExecuteFunction);

            ToggleDevelopmentTabEnabled();//dissable the buttons
        }

        public void ToggleDevelopmentTabEnabled()
        {
            BtnExecuteFunction.Enabled = !BtnExecuteFunction.Enabled;
            BtnAddFunction.Enabled = !BtnAddFunction.Enabled;
        }

        private void BtnExecuteFunction_Click(Object sender, EventArgs e)
        {
            List<String> Functions = new List<String>();
            //add root elements to parse
            String XmlToparse = "<x>" + functionRTB.Text + "</x>";
            try
            {
                XDocument parsedDoc = XDocument.Parse(XmlToparse);
                List<XElement> functionList = parsedDoc.Root.Elements().ToList();
                foreach(var function in functionList)
                {
                    Functions.Add(function.ToString());
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            ////testing run function in an active test
            //XElement element = new XElement("elementManager");
            //XAttribute type = new XAttribute("type", "pause");
            //XAttribute value = new XAttribute("value", "indefinite");
            //element.Add(type);
            //element.Add(value);
            //Functions.Add(element.ToString());
            ////testing run function in an active test

            dynamic toSend = new ExpandoObject();
            toSend.TestId = Int32.Parse(testId.ToString().Replace("\"", ""));
            toSend.Function = Functions;
            RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Test/TestFunction", toSend);            
            
        }

        private void BtnAddFunction_Click(object sender, EventArgs e)
        {
            String rawFunction = ElementEditorTxtBx.Text;
            XElement functionElement = null;
            //convert to xelement
            try
            {
                functionElement = XElement.Parse(rawFunction);
            }
            catch (Exception exe)
            {                
                Console.WriteLine(exe.ToString());
                return;
            }
            functionRTB.SelectedText = rawFunction+Environment.NewLine;
        }

        private void FunctionElementTemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //populate function textbox with template from dictionary
            ElementEditorTxtBx.Text = Functions[ElementsBox.Text];
        }

        public void MakeTabGroup()
        {
            InternalTabs = new TabControl();
            InternalTabs.Size = new Size(768, 400);
            InternalTabs.Location = new Point(0, 0);
            InternalTabs.TabPages.Add(TestRunTab);
            InternalTabs.TabPages.Add(DevelopmentTab);

            MotherPage = new TabPage();
            MotherPage.Controls.Add(InternalTabs);
        }

        public void StyleMotherPage()
        {
            MotherPage.Text = testName;
            MotherPage.Size = new Size(768, 400);
            MotherPage.Location = new Point(0, 0);
            MotherPage.BackColor = Color.DimGray;
        }

        public void createTestRunnerTab()
        {
            this.TestRunTab = new TabPage();
            this.StopBut = new Button();
            this.StartPause = new Button();
            this.Screenshot = new Button();
            this.FocusWindowBtn = new Button();
            this.TrackMouseBtn = new Button();
            this.testLabel = new Label();
            this.LogOutPut = new RichTextBox();
            styleTab();
            styleLogOut();
            styleStartPause();
            styleStopButton();
            styleScreenshot();
            styleFocusButton();
            styleTrackMouse();
            styleTestLabel();
            TestRunTab.ImageIndex = 4;
            DevelopmentTabCreation();
            MakeTabGroup();
            StyleMotherPage();
        }
        
        private void styleTestLabel()
        {
            this.testLabel.AutoSize = true;
            this.testLabel.Location = new Point(10, 10);
            this.testLabel.Name = testName;
            this.testLabel.Size = new Size(35, 13);
            this.testLabel.TabIndex = 0;
            this.testLabel.Text = testName;
            testLabel.ForeColor = SystemColors.ButtonFace;
        }

        private void styleLogOut()
        {
            this.LogOutPut.Location = new Point(0, 80);
            this.LogOutPut.Name = "LogOutPut";
            this.LogOutPut.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.LogOutPut.Size = new Size(this.TestRunTab.Size.Width, this.TestRunTab.Size.Height - 100);
            this.LogOutPut.TabIndex = 4;
            this.LogOutPut.Text = "";
            this.LogOutPut.BackColor = SystemColors.InactiveCaptionText;
            LogOutPut.ForeColor = SystemColors.ButtonFace;
        }

        private void styleTab()
        {
            this.TestRunTab.Controls.Add(this.LogOutPut);
            this.TestRunTab.Controls.Add(this.StopBut);
            this.TestRunTab.Controls.Add(this.StartPause);
            this.TestRunTab.Controls.Add(this.TrackMouseBtn);
            this.TestRunTab.Controls.Add(this.FocusWindowBtn);
            this.TestRunTab.Controls.Add(this.testLabel);
            this.TestRunTab.Controls.Add(this.Screenshot);
            this.TestRunTab.Location = new Point(0, 0);
            this.TestRunTab.Name = this.testLabel.Text;
            this.TestRunTab.Padding = new Padding(3);
            this.TestRunTab.Size = new Size(760, 400);
            this.TestRunTab.TabIndex = 1;
            //this.TestRunTab.Text = testName;
            this.TestRunTab.Text = "Test Output";
            this.TestRunTab.UseVisualStyleBackColor = true;

            TestRunTab.BackColor = Color.DimGray;
        }

        //-----------------Buttons-----------------

        private Size buttonSize = new Size(100, 40);
        private int ButtonXloc = 10;
        private void styleScreenshot()
        {
            this.Screenshot.Location = new Point(ButtonXloc, 30);
            this.Screenshot.Name = "ScreenshotBut";
            this.Screenshot.Size = buttonSize;
            this.Screenshot.TabIndex = 4;
            this.Screenshot.Text = "Screenshot";
            this.Screenshot.UseVisualStyleBackColor = true;
            this.Screenshot.Enabled = false;
            this.Screenshot.Click += new EventHandler(this.ScreenshotClick);
            ButtonXloc = ButtonXloc + 110;
        }

        private void styleTrackMouse()
        {
            this.TrackMouseBtn.Location = new Point(ButtonXloc, 30);
            this.TrackMouseBtn.Name = "TrackMoueBtn";
            this.TrackMouseBtn.Size = buttonSize;
            this.TrackMouseBtn.TabIndex = 5;
            this.TrackMouseBtn.Text = "Track Mouse";
            this.TrackMouseBtn.UseVisualStyleBackColor = true;
            this.TrackMouseBtn.Enabled = false;
            this.TrackMouseBtn.Click += new EventHandler(this.MouseTrackStart);
            ButtonXloc = ButtonXloc + 110;
        }

        private void styleStopButton()
        {
            this.StopBut.Location = new Point(ButtonXloc, 30);
            this.StopBut.Name = "StopBut";
            this.StopBut.Size = buttonSize;
            this.StopBut.TabIndex = 3;
            this.StopBut.Text = "Stop";
            this.StopBut.UseVisualStyleBackColor = true;
            this.StopBut.Enabled = false;
            this.StopBut.Click += new EventHandler(this.Stop_Click);
            ButtonXloc = ButtonXloc + 110;
        }

        private void styleStartPause()
        {
            this.StartPause.Location = new Point(ButtonXloc, 30);
            this.StartPause.Name = "StartPause";
            this.StartPause.Size = buttonSize;
            this.StartPause.TabIndex = 2;
            this.StartPause.Text = "Pause";
            this.StartPause.UseVisualStyleBackColor = true;
            this.StartPause.Enabled = false;
            this.StartPause.Click += new EventHandler(this.StartPause_Click);
            ButtonXloc = ButtonXloc + 110;
        }

        private void styleFocusButton()
        {
            this.FocusWindowBtn.Location = new Point(ButtonXloc, 30);
            this.FocusWindowBtn.Name = "FocusWindow";
            this.FocusWindowBtn.Size = buttonSize;
            this.FocusWindowBtn.TabIndex = 6;
            this.FocusWindowBtn.Text = "Focus";
            this.FocusWindowBtn.UseVisualStyleBackColor = true;
            this.FocusWindowBtn.Enabled = false;
            this.FocusWindowBtn.Click += new EventHandler(this.FocusButton_Click);
            ButtonXloc = ButtonXloc + 110;
        }

        private void FocusButton_Click(object sender, EventArgs e)
        {
           var testResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/FocusWindow/" + this.testId + "/");

            Console.WriteLine(testResponse.ToString());
        }

        private void MouseTrackStart(object sender, EventArgs e)
        {
            //start mouse tracking thread if its not already started by talking to the backend
            dynamic toSend = new ExpandoObject();
            toSend.TestId = Int32.Parse(testId.ToString().Replace("\"",""));

            JsonValue tracking= RestClient.makeRequest(httpVerb.POST, @"http://localhost:"+apiPort+"/Test/ElementAtPosition",toSend);

            Console.WriteLine("breakPoint");

            TrackMouseBtn.Text = "Track Mouse: " + tracking.ToString();
        }

        private void ScreenshotClick(object sender, EventArgs e)
        {
            var testResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/Screenshot/" + this.testId + "/");

            Console.WriteLine(testResponse.ToString());
        }        

        private void Stop_Click(object sender, EventArgs e)
        {
            var testResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/Stop/" + this.testId + "/");
            TestRunTab.ImageIndex = 1;
            MotherPage.ImageIndex = 1;
            DisableButtons();
        }

        private void StartPause_Click(object sender, EventArgs e)
        {

            var testResponse = RestClient.makeRequest(httpVerb.GET, @"http://localhost:"+apiPort+"/Test/Pause/" + this.testId + "/");
            if (testResponse["Paused"])
            {
                this.StartPause.Text = "Play";
                TestRunTab.ImageIndex = 3;//2
                MotherPage.ImageIndex = 3;//2
            }
            else
            {
                this.StartPause.Text = "Pause";
                TestRunTab.ImageIndex = 2;//1
                MotherPage.ImageIndex = 2;//1
            }

        }

        public void DisableButtons()
        {
            StopBut.Enabled = false;
            StartPause.Enabled = false;
            Screenshot.Enabled = false;
            TrackMouseBtn.Enabled = false;
            FocusWindowBtn.Enabled = false;
            TestEnded = true;
        }

        public void EnableButtons()
        {
            StopBut.Enabled = true;
            StartPause.Enabled = true;
            Screenshot.Enabled = true;
            TrackMouseBtn.Enabled = true;
            FocusWindowBtn.Enabled = true;
        }

        //-----------------Buttons-----------------

        public void updateLogContent(String message)
        {
            if (!TestEnded)
            {
                if (!StopBut.Enabled)
                {
                    EnableButtons();
                    ToggleDevelopmentTabEnabled();
                    TestRunTab.ImageIndex = 2;
                    MotherPage.ImageIndex = 2;
                }
                this.LogOutPut.Text += message + Environment.NewLine;
                LogOutPut.SelectionStart = LogOutPut.Text.Length;
                LogOutPut.ScrollToCaret();
            }
        }

        public void SystemMessage(String BodySent)
        {
            if (BodySent.Equals("Test Paused"))
            {
                SwapPauseLabel();
            }
            else if (BodySent.Equals("Test Success"))
            {
                TestRunTab.ImageIndex = 0;
                MotherPage.ImageIndex = 0;
                DisableButtons();
                ToggleDevelopmentTabEnabled();
            }
            else if (BodySent.Equals("Test Aborted"))
            {
                TestRunTab.ImageIndex = 1;
                MotherPage.ImageIndex = 1;
                DisableButtons();
                ToggleDevelopmentTabEnabled();
            }
        }

        internal void SwapPauseLabel()
        {
            if (this.StartPause.Text.Equals("Play"))
            {
                this.StartPause.Text = "Pause";
                TestRunTab.ImageIndex = 2;
                MotherPage.ImageIndex = 2;
            }
            else
            {
                this.StartPause.Text = "Play";
                TestRunTab.ImageIndex = 3;
                MotherPage.ImageIndex = 3;
            }
        } 
    }
}