using System;
using System.Collections.Generic;
using System.Drawing;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace restClient_0
{
    class TestExicutor
    {

        private Panel previousGroupBox;
        public Panel previousjourneyQuantity { get; set; }
        private DataObject dataObject;
        private List<string> orderOfPhases;
        private string portNumber;
        private Panel previousTest { get; set; }
        private List<TestRunningInformation> testsTorun;
        private ToolStripMenuItem runTestsToolStripMenuItem;
        private TextBox testLocations;
        private Panel requestTestsPanel;
        private MenuStrip TestExicutionMenu;
        public Button RequestTests { get; private set; }
        private Panel TestExicutorPanel;

        public Control Controls { get; private set; }

        public TestExicutor() { }

        public TestExicutor(Panel previousGroupBox, Panel previousjourneyQuantity, DataObject dataObject, List<string> orderOfPhases, Panel previousTest, List<TestRunningInformation> testsTorun, ToolStripMenuItem runTestsToolStripMenuItem, TextBox testLocations, Panel requestTestsPanel, MenuStrip menuStrip2, Button RequestTests, Panel TestExicutorPanel, String portNumber)
        {
            this.previousGroupBox = previousGroupBox;
            this.previousjourneyQuantity = previousjourneyQuantity;
            this.dataObject = dataObject;
            this.orderOfPhases = orderOfPhases;
            this.portNumber = portNumber;
            this.previousTest = previousTest;
            this.testsTorun = testsTorun;
            this.runTestsToolStripMenuItem = runTestsToolStripMenuItem;
            this.testLocations = testLocations;
            this.requestTestsPanel = requestTestsPanel;
            this.TestExicutionMenu = menuStrip2;
            this.RequestTests = RequestTests;
            this.TestExicutorPanel = TestExicutorPanel;
           
        }
        

        public void chooseTestToRun()
        {
            this.runTestsToolStripMenuItem.Click += (sender, e) => showRunTests(sender, e);
            this.testLocations.Text = @"\\BX-WKS12282\Users\MANNN\Desktop\SeleniumLite_BETA\DevFiles\XML Experiment\XML based Tests"; //hardcoded for testing
            this.RequestTests.Click += (sender, e) => postRequestForTests(sender, e);

        }

        private void showRunTests(object sender, EventArgs e)
        {
            if (this.requestTestsPanel.Visible)
            {
                //this.requestTestsPanel.Visible = false;
                this.requestTestsPanel.Hide();
                this.previousGroupBox.Visible = false;
                //this.RequestTests.Hide();
                //this.testLocations.Visible = false;
                this.previousTest.Visible = false;
                this.previousjourneyQuantity.Visible = false;
                Form1.previousjourneyQuantity.Visible = false;
                Form1.previousGroupBox.Visible = false;
                
            }
            else
            {
                this.TestExicutorPanel.Visible = true;
                //this.requestTestsPanel.Visible = true;
                this.requestTestsPanel.Show();
                //this.RequestTests.Show();
                //this.testLocations.Visible = true;

                Console.WriteLine(this.testLocations.Visible + " " + this.RequestTests.Visible);

                this.previousTest.Visible = false;
                this.previousjourneyQuantity.Visible = false;
                this.previousGroupBox.Visible = false;
                this.TestExicutionMenu.Items.Clear();
                //this.TestExicutorPanel.Controls.Clear();
                Form1.previousjourneyQuantity.Visible = false;
                Form1.previousGroupBox.Visible = false;

            }
        }

        private void postRequestForTests(object sender, EventArgs e)
        {

            RequestTests request = new RequestTests(this.testLocations.Text);
            JsonValue listOfTests = RestClientPost.makeRequest(@"http://localhost:" + portNumber + "/Test/TestList", request);
            string locationOfTests = listOfTests["DirectorySearched"];
            Button runAllTests = new Button();
            runAllTests.Text = "Run All Tests!";
            runAllTests.Location = new Point(100, 120);
            runAllTests.Click += (sender2, e2) => runAllOfTheTests(sender, e);
            runAllTests.Visible = false;
            //runAllTests.Click += (sender3, e3) => runTestsOnBrowserRequest(sender3, e3, runTestInformation, runAllTests);
            this.TestExicutorPanel.Controls.Add(runAllTests);

            foreach (var test in listOfTests["TestsInDirectory"])
            {
                Panel testPanel = new Panel();
                testPanel.BackColor = SystemColors.ControlDark;
                testPanel.Controls.Add(this.testLocations);
                testPanel.Controls.Add(this.RequestTests);
                testPanel.Location = new Point(309, 85);
                testPanel.Name = "requestTestsPanel";
                testPanel.Size = new Size(300, 300);
                testPanel.TabIndex = 10;
                testPanel.Visible = false;
                Label testName = new Label();

                TextBox outPutDirectoryTextBox = new TextBox();
                outPutDirectoryTextBox.Text = @"C:\Users\maesoa\Desktop\TestS";
                outPutDirectoryTextBox.Location = new Point(10, 110);
                testPanel.Controls.Add(outPutDirectoryTextBox);

                testName.Text = (JsonValue)test;
                testPanel.Controls.Add(testName);
                TestRunningInformation runTestInformation = new TestRunningInformation(testName.Text, locationOfTests, "Chrome", outPutDirectoryTextBox.Text);

                CheckBox firefox = new CheckBox();
                CheckBox chrome = new CheckBox();

                firefox.Name = "FireFox";
                firefox.Text = "FireFox";
                firefox.Location = new Point(10, 25);
                firefox.CheckStateChanged += (sender4, e4) => useFireFoxBrowser(sender4, e4, runTestInformation, chrome);
                testPanel.Controls.Add(firefox);

                chrome.Text = "Chrome";
                chrome.Name = "Chrome";
                chrome.CheckStateChanged += (sender4, e4) => useChromeBrowser(sender4, e4, runTestInformation, firefox);
                chrome.Checked = true;
                chrome.Location = new Point(10, 50);
                testPanel.Controls.Add(chrome);

                Label outPutDirectory = new Label();
                outPutDirectory.Text = "Output Directory";
                outPutDirectory.Location = new Point(10, 80);
                testPanel.Controls.Add(outPutDirectory);

                CheckBox runTestCheckBox = new CheckBox();
                runTestCheckBox.Text = "Run Test";
                runTestCheckBox.Name = "Run Test";
                runTestCheckBox.Location = new Point(10, 150);
                testPanel.Controls.Add(runTestCheckBox);

                this.TestExicutorPanel.Controls.Add(testPanel);
                ToolStripDropDownButton testToRun = new ToolStripDropDownButton();
                testToRun.Text = test.ToString();
                this.TestExicutionMenu.Items.Add(testToRun);
                testToRun.Click += (sender2, e2) => hideAndShowTestsToRun(sender2, e2, testPanel);
                Console.WriteLine(test.ToString());
                runTestCheckBox.CheckStateChanged += (sender3, e3) => runTestsOnBrowserRequest(sender3, e3, runTestInformation, runAllTests);


            }



            this.requestTestsPanel.Hide();
            //this.requestTestsPanel.Visible = false;
            //this.testLocations.Visible = false;
            //this.RequestTests.Hide();


            //this.TestsToRun.Visible = true;
        }

        private void useFireFoxBrowser(object sender4, EventArgs e4, TestRunningInformation runTestInformation, CheckBox chrome)
        {
            CheckBox fireFoxCheckBox = sender4 as CheckBox;

            if (fireFoxCheckBox.Checked)
            {
                runTestInformation.setWebBrowser("FireFox");
                chrome.Checked = false;
            }
        }

        private void useChromeBrowser(object sender4, EventArgs e4, TestRunningInformation runTestInformation, CheckBox fireFox)
        {
            CheckBox chromeCheckBox = sender4 as CheckBox;

            if (chromeCheckBox.Checked)
            {
                runTestInformation.setWebBrowser("Chrome");
                fireFox.Checked = false;
            }

        }

        private void runAllOfTheTests(object sender, EventArgs e)
        {
            RestClientPost.makeRequest(@"http://localhost:" + portNumber + "/Test/RunTest", testsTorun);
        }

        private void runTestsOnBrowserRequest(object sender3, EventArgs e3, TestRunningInformation runTestInformation, Button runAllTests)
        {

            if (testsTorun.Contains(runTestInformation))
            {
                testsTorun.Remove(runTestInformation);
                if (testsTorun.Count == 0)
                {
                    runAllTests.Visible = false;
                }

            }
            else
            {
                testsTorun.Add(runTestInformation);
                runAllTests.Visible = true;
            }


        }

        private void hideAndShowTestsToRun(object sender, EventArgs e, Panel testPanel)
        {
            ToolStripDropDownButton testToRun = sender as ToolStripDropDownButton;
            if (testPanel.Visible)
            {
                testPanel.Visible = false;
                this.previousTest.Visible = false;
                this.requestTestsPanel.Hide();
                //this.requestTestsPanel.Visible = false;
                this.testLocations.Visible = false;
            }
            else
            {
                testPanel.Visible = true;
                if (!(this.previousTest.Equals(testPanel)))
                {
                    this.previousTest.Visible = false;
                }
                this.RequestTests.Hide();
                this.testLocations.Visible = false;
                this.requestTestsPanel.Hide();
                //this.requestTestsPanel.Visible = false;
                this.previousGroupBox.Visible = false;
            }
            this.previousTest = testPanel;
            

        }

    }
}
