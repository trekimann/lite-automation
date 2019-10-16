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
    class SubmitTestData
    {
        private List<TextBox> allUrLTextBoxes = new List<TextBox>();
        private List<TextBox> allTestNameBoxes = new List<TextBox>();
        private List<TextBox> allTestlocationBoxes = new List<TextBox>();
        private string testName = "US";
        private string testUrl = @"http://bx2-dev-ec02.test.hastings.local:9080/Portal/servletcontroller?action=devland&producerCode=RGVmYXVsdA==&CampaignCode=RGVmYXVsdERlZmF1bHRDYW1wYWlnbg==";
        private string testLocation = "C:\\Users\\maesoa\\Desktop\\TestS";
        private string portNumber = "26000";

        private DataObject dataObject;

        public SubmitTestData(DataObject dataObject, string portNumber, string testName, string testUrl, string testLocation)
        {
            this.dataObject = dataObject;
            this.portNumber = portNumber;
            this.testName = testName;
            this.testUrl = testUrl;
            this.testLocation = testLocation;
        }

        public SubmitTestData(DataObject dataObject)
        {
            this.dataObject = dataObject;
            
        }

        public void submitData(Panel magratheaElementsPanel, JsonValue htmlElement, string phase)
        {
            Button submitTestData = new Button();
            string labelTestNameString = "TestName";
            string labelTestUrlString = "Test Url";
            string textBoxName = "TestName";
            string outPutLocationName = "Output Location";

            var labelTestName = new Label();
            labelTestName.Text = labelTestNameString;
            labelTestName.Location = new Point(700, 50);
            labelTestName.AutoSize = true;

            TextBox testName = new TextBox();
            this.allTestNameBoxes.Add(testName);
            testName.Location = new Point(700, 70);
            testName.Text = this.testName;
            testName.Name = textBoxName;
            testName.TextChanged += (sender, e) => updateTestName(sender, e, htmlElement, "testInformation");
            magratheaElementsPanel.Controls.Add(labelTestName);
            magratheaElementsPanel.Controls.Add(testName);

            var labelUrlName = new Label();
            labelUrlName.Text = labelTestUrlString;
            labelUrlName.Location = new Point(700, 92);
            labelUrlName.AutoSize = true;

            TextBox urlName = new TextBox();
            allUrLTextBoxes.Add(urlName);
            urlName.Location = new Point(700, 110);
            urlName.Text = this.testUrl;
            urlName.Name = labelTestUrlString;
            urlName.TextChanged += (sender, e) => updateUrl(sender, e, htmlElement, "testInformation");

            var outPutLocationLabel = new Label();
            outPutLocationLabel.Text = outPutLocationName;
            outPutLocationLabel.Location = new Point(700, 132);
            outPutLocationLabel.AutoSize = true;

            TextBox outPutLocationTextBox = new TextBox();
            allTestlocationBoxes.Add(outPutLocationTextBox);
            outPutLocationTextBox.Location = new Point(700, 150);
            outPutLocationTextBox.Text = this.testLocation;
            outPutLocationTextBox.Name = outPutLocationName;
            outPutLocationTextBox.TextChanged += (sender, e) => updateOutPutLocationTextBox(sender, e, htmlElement, "testInformation");

            magratheaElementsPanel.Controls.Add(labelTestName);
            magratheaElementsPanel.Controls.Add(testName);

            magratheaElementsPanel.Controls.Add(labelUrlName);
            magratheaElementsPanel.Controls.Add(urlName);

            magratheaElementsPanel.Controls.Add(outPutLocationLabel);
            magratheaElementsPanel.Controls.Add(outPutLocationTextBox);


            submitTestData.Text = "Submit Test Data";
            submitTestData.Location = new Point(700, 20);
            submitTestData.Click += (sender2, e2) => submitTestDataToService(sender2, e2);

            dataObject.addPhaseAttributetoDataContainer("testInformation", new Dictionary<string, Dictionary<string, string>>());
            dataObject.addDataElementToDataObject("testInformation", "TestUrl", new Dictionary<string, string>());
            dataObject.addAtributeToElement("testInformation", "TestUrl", "value", urlName.Text);


            dataObject.addPhaseAttributetoDataContainer("testInformation", new Dictionary<string, Dictionary<string, string>>());
            dataObject.addDataElementToDataObject("testInformation", "outputLocation", new Dictionary<string, string>());
            dataObject.addAtributeToElement("testInformation", "outputLocation", "value", outPutLocationTextBox.Text);

            dataObject.addPhaseAttributetoDataContainer("testInformation", new Dictionary<string, Dictionary<string, string>>());
            dataObject.addDataElementToDataObject("testInformation", "TestName", new Dictionary<string, string>());
            dataObject.addAtributeToElement("testInformation", "TestName", "value", testName.Text);

            dataObject.addPhaseAttributetoDataContainer("testInformation", new Dictionary<string, Dictionary<string, string>>());
            dataObject.addDataElementToDataObject("testInformation", "happyDefault", new Dictionary<string, string>());
            dataObject.addAtributeToElement("testInformation", "happyDefault", "value", "true");

            testName.TextChanged += (sender2, e2) => addTextToDataObject(sender2, e2, phase);
            magratheaElementsPanel.Controls.Add(submitTestData);
        }

        private void updateOutPutLocationTextBox(object sender, EventArgs e, JsonValue jsonValue, string phase)
        {
            TextBox locationOfTest = sender as TextBox;

            foreach (var testLocation in this.allTestlocationBoxes)
            {
                testLocation.Text = locationOfTest.Text;
            }


            dataObject.addAtributeToElement("testInformation", "outputLocation", "value", locationOfTest.Text);
        }

        private void updateUrl(object sender, EventArgs e, JsonValue jsonValue, string phase)
        {
            TextBox TestUrlBox = sender as TextBox;

            foreach (var testUrl in this.allUrLTextBoxes)
            {
                testUrl.Text = TestUrlBox.Text;
            }

            dataObject.addAtributeToElement("testInformation", "TestUrl", "value", TestUrlBox.Text);

        }

        private void updateTestName(object sender, EventArgs e, JsonValue jsonValue, string phase)
        {

            TextBox testNameTextBox = sender as TextBox;

            foreach (var testNameInBox in this.allTestNameBoxes)
            {
                testNameInBox.Text = testNameTextBox.Text;
            }


            dataObject.addAtributeToElement("testInformation", "TestName", "value", testNameTextBox.Text);

        }

        private void addTextToDataObject(object sender2, EventArgs e2, string phase)
        {
            TextBox textBox = sender2 as TextBox;

            dataObject.addAtributeToElement("testInformation", textBox.Name, "value", textBox.Text);

        }

        private void submitTestDataToService(object sender2, EventArgs e2)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> cleanData = dataObject.cleanData();
            Console.WriteLine(cleanData["testInformation"]["TestName"]["value"]);
            Console.WriteLine(cleanData["testInformation"]["TestUrl"]["value"]);
            Console.WriteLine(cleanData["testInformation"]["outputLocation"]["value"]);
            TestData testData = new TestData(cleanData["testInformation"]["TestName"]["value"], cleanData["testInformation"]["TestUrl"]["value"], cleanData["testInformation"]["outputLocation"]["value"]);

            string previousJourney = "";
            JourneyData journeyData = new JourneyData();

            foreach (var journeyandPhase in dataObject.phaseOrder)
            {

                string journey = journeyandPhase.Split('|')[0];
                string phase = journeyandPhase.Split('|')[1];

                Console.WriteLine(phase);

                if ((!journey.Equals(previousJourney)) && cleanData.ContainsKey(phase))
                {
                    journeyData = new JourneyData(journey);
                    testData.addJourney(journeyData);
                }


                if (cleanData.ContainsKey(phase))
                {
                    PhaseData phaseData = new PhaseData();
                    phaseData.AddPhase(phase, cleanData[phase]);
                    journeyData.addPhase(phaseData);
                    previousJourney = journey;

                }
                else
                {
                    Console.WriteLine(phase);
                }
            }

            RestClientPost.makeRequest("http://localhost:" + portNumber + "/Xml/GenerateTestScript", testData);
        }

    }
}
