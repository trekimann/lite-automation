using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestExecution
{
    class MagTestListPopulator
    {
        private Panel testListPanel;
        private IEnumerable<string> magratheaTests;
        private List<MagratheatestExecutionPanel> OfSelectedTests = new List<MagratheatestExecutionPanel>();
        private string stringDirectoryPath;

        public MagTestListPopulator(Panel testListPanel, IEnumerable<string> magratheaTests, String stringDirectoryPath)
        {
            this.testListPanel = null;
            this.testListPanel = testListPanel;
            this.magratheaTests = magratheaTests;
            this.stringDirectoryPath = stringDirectoryPath;
        }

        public void populateMagratheaTests()
        {
            testListPanel.Controls.Clear();
            int currentY = 0;
            int panelNumber = 1;
            foreach (var testName in this.magratheaTests)
            {
                var magratheaTest = new MagratheatestExecutionPanel(this.testListPanel.Size.Width - 20, testName, currentY,panelNumber);
                magratheaTest.createTestPanel();
                this.OfSelectedTests.Add(magratheaTest);
                currentY += 30;
                this.testListPanel.Controls.Add(magratheaTest.GetPanel());
                panelNumber++;
            }
        }

        public List<Object> TestToExecute()
        {
            List<Object> tests = new List<object>();

            foreach (var magTestPanel in OfSelectedTests)
            {
                if (magTestPanel.Checked)
                {

                    magTestPanel.outputDirectory = stringDirectoryPath;
                    magTestPanel.testDirectory = stringDirectoryPath;

                    foreach (String browser in magTestPanel.GetBrowsers())
                    {
                        dynamic testInformationObject = new ExpandoObject();
                        testInformationObject.webBrowser = browser;
                        var testInformationDictionary = (IDictionary<string, object>)testInformationObject;
                        PropertyInfo[] magTestPanelProperties = typeof(MagratheatestExecutionPanel).GetProperties();
                        foreach (var property in magTestPanelProperties)
                        {
                            var name = property.Name;
                            var value = magTestPanel.GetType().GetProperty(name).GetValue(magTestPanel, null);
                            testInformationDictionary.Add(name, value);
                        }
                        tests.Add(testInformationObject);
                    }
                }
            }
            return tests;
        }
    }
}
