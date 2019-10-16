using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TestExecution
{
    internal class TestExecutionGenerator
    {
        private TabPage testExecution;
        private TextBox directoryToTests;
        private Button getTestsButton;
        private Button executeTestsButton;
        private FolderBrowserDialog testDirectoryFinder;
        private Panel testListPanel;
        private List<string> magratheaTests = new List<string>();
        public MagTestListPopulator MagTestListPopulator { get; private set; }

        public TestExecutionGenerator(TabPage testExecution, TextBox directoryToTests, Button getTestsButton, Button executeTestsButton, FolderBrowserDialog testDirectoryFinder, Panel testListPanel)
        {
            this.testExecution = testExecution;
            this.directoryToTests = directoryToTests;
            this.getTestsButton = getTestsButton;
            this.executeTestsButton = executeTestsButton;
            this.testDirectoryFinder = testDirectoryFinder;
            this.testListPanel = testListPanel;
        }

        public void generateTests()
        {
            this.getTestsButton.Click += new EventHandler(this.GetTestsButton_Click);          

        }

        private void GetTestsButton_Click(object sender, EventArgs e)
        {

            if (testDirectoryFinder.ShowDialog() == DialogResult.OK)
            {
                var stringDirectoryPath = testDirectoryFinder.SelectedPath;
                directoryToTests.Text = stringDirectoryPath;
                var fileList = Directory.GetFileSystemEntries(stringDirectoryPath, "*.xml");

                magratheaTests.Clear();

                foreach (var file in fileList)
                {
                    var trimmedFile = file.Replace(stringDirectoryPath+"\\", "");
                    if (trimmedFile.StartsWith("US"))
                    {
                        this.magratheaTests.Add(trimmedFile);
                    }
                    
                }

                this.testListPanel.Controls.Clear();
                this.MagTestListPopulator = new MagTestListPopulator(this.testListPanel, this.magratheaTests, stringDirectoryPath);
                this.MagTestListPopulator.populateMagratheaTests();
                

            }
        }
    }
}