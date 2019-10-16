using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TestExecution
{
    internal class MagratheatestExecutionPanel
    {
        private int panelWidth;
        //public string webBrowser { get; private set; } = "chrome";
        private List<String> webBrowsers { get; set; }

        public Boolean Checked { get; private set; }
        public string testName { get; private set; }
        public string outputDirectory { get; set; }
        public string testDirectory { get; set; }
        public int panelNumber { get; private set; }

        //mobile information
        public Dictionary<String, String> DesiredCapibilities { get; set; }

        private int currentY;
        private Panel Panel;

        private CheckBox magratheaTestSeletion = new CheckBox();
        private CheckBox chromeBrowserSeletion = new CheckBox();
        private CheckBox firefoxBrowserSeletion = new CheckBox();
        private CheckBox AndroidBrowserSelection = new CheckBox();

        //private Panel RightClickMenu = new Panel();

        public List<String> GetBrowsers()
        {
            return webBrowsers;
        }

        public MagratheatestExecutionPanel(int panelWidth, string testName, int currentY,int panelNumber)
        {
            this.panelWidth = panelWidth;
            this.testName = testName.Substring(0,testName.Length-4);
            this.currentY = currentY;
            this.panelNumber = panelNumber;
        }

        //public void rightClickMenuCreation()
        //{
        //    RightClickMenu = new Panel();
        //    RightClickMenu.Visible = false;
        //    RightClickMenu.Size = new Size(50, 50);
        //    RightClickMenu.BackColor = Color.Black;
        //}

        //public void Panel_MouseDown(object sender, MouseEventArgs e)
        //{//showing panel at the location of the mouse click
        //    switch (e.Button)
        //    {
        //        case MouseButtons.Right:
        //            {
        //                this.RightClickMenu.Visible = true;
        //                this.RightClickMenu.BringToFront();
        //                this.RightClickMenu.Location= new Point(e.X, e.Y);
        //            }
        //            break;
        //    }
        //}

        public void createTestPanel()
        {
            this.Panel = new Panel();
            this.Panel.Size = new Size(this.panelWidth, 30);
            //this.Panel.MouseDown += new MouseEventHandler(Panel_MouseDown);
            webBrowsers = new List<string>();

            magratheaTestSeletion.CheckedChanged += new EventHandler(this.AddOrRemoveMagTest);
            magratheaTestSeletion.Name = testName;
            magratheaTestSeletion.Text = testName;
            magratheaTestSeletion.ForeColor = SystemColors.ButtonFace;
            magratheaTestSeletion.Size = new Size(testName.Length*10 + 30, 20);

            chromeBrowserSeletion.CheckedChanged += new EventHandler(this.AddOrRemoveChromeSelection);
            chromeBrowserSeletion.Name = "ChromeCheckBox_"+testName;
            chromeBrowserSeletion.Text = "Chrome";
            chromeBrowserSeletion.Location = new Point(this.panelWidth- chromeBrowserSeletion.Text.Length*10, 0);
            chromeBrowserSeletion.ForeColor = SystemColors.ButtonFace;

            firefoxBrowserSeletion.CheckedChanged += new EventHandler(this.AddOrRemoveFirefoxSelection);
            firefoxBrowserSeletion.Name = "FirefoxCheckBox_" + testName;
            firefoxBrowserSeletion.Text = "Firefox";
            firefoxBrowserSeletion.Location = new Point(chromeBrowserSeletion.Location.X - firefoxBrowserSeletion.Text.Length * 10, 0);
            firefoxBrowserSeletion.ForeColor = SystemColors.ButtonFace;

            AndroidBrowserSelection.CheckedChanged += new EventHandler(this.AddOrRemoveAndroidSelection);
            AndroidBrowserSelection.Name = "AndroidCheckBox_" + testName;
            AndroidBrowserSelection.Text = "Android";
            AndroidBrowserSelection.Location = new Point(firefoxBrowserSeletion.Location.X - AndroidBrowserSelection.Text.Length * 10, 0);
            AndroidBrowserSelection.ForeColor = SystemColors.ButtonFace;

            //rightClickMenuCreation();

            this.Panel.Controls.Add(magratheaTestSeletion);
            this.Panel.Controls.Add(chromeBrowserSeletion);
            this.Panel.Controls.Add(firefoxBrowserSeletion);
            this.Panel.Controls.Add(AndroidBrowserSelection);
            //this.Panel.Controls.Add(RightClickMenu);

            this.Panel.Location = new Point(0, currentY);
            this.Panel.BorderStyle = BorderStyle.FixedSingle;
            pickBackgroundColor();
        }

        public void pickBackgroundColor()
        {
            if(panelNumber %2==0)
            {
                Panel.BackColor = Color.FromArgb(25, Color.White);
            }
            else
            {

            }
        }

        private Boolean AddOrRemoveBrowser(String browser)
        {
            if(webBrowsers.Contains(browser))
            {
                webBrowsers.Remove(browser);
                return false;
            }
            else
            {
                webBrowsers.Add(browser);
                return true;
            }
        }

        private void AddOrRemoveChromeSelection(object sender, EventArgs e)
        {
            CheckBox Selected = sender as CheckBox;
            //webBrowser = "chrome";
            AddOrRemoveBrowser("chrome");
        }
        private void AddOrRemoveFirefoxSelection(object sender, EventArgs e)
        {
            CheckBox Selected = sender as CheckBox;
            //webBrowser = "firefox";
            AddOrRemoveBrowser("firefox");
        }

        private void AddOrRemoveAndroidSelection(object sender, EventArgs e)
        {
            CheckBox Selected = sender as CheckBox;
            //webBrowser = "firefox";
            var result = AddOrRemoveBrowser("Android");

            if(result)
            {
                DesiredCapibilities = new Dictionary<string, string>();
                DesiredCapibilities.Add("RemoteAddress", "http://localhost:4723/wd/hub");
                DesiredCapibilities.Add("Capability_browserName" , "Chrome");
                DesiredCapibilities.Add("Capability_noReset" , "true");
                DesiredCapibilities.Add("AdbLocation" , @"C:\Users\"+ Environment.UserName + @"\AppData\Local\Android\Sdk\platform-tools\adb.exe");
            }
            else
            {
                DesiredCapibilities = null;
            }
        }

        private void AddOrRemoveMagTest(object sender, EventArgs e)
        {
            CheckBox magratheaTestSeletion = sender as CheckBox;
            this.Checked = magratheaTestSeletion.Checked;

            // rename with the number of the selection

        }

        public Panel GetPanel()
        {
            return this.Panel;
        }

    }
}