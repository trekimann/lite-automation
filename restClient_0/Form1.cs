using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Json;
using System.Text.RegularExpressions;
using System.Dynamic;

namespace restClient_0
{
    public partial class Form1 : Form
    {
        public static Panel previousGroupBox { get; set; } = new Panel();
        public static Panel previousjourneyQuantity { get; set; } = new Panel();
        private HashSet<string> recordJourneyCalled = new HashSet<string>();
        private DataObject dataObject = new DataObject();
        private List<string> orderOfPhases = new List<string>();
        private string portNumber = "26000";
        private Panel previousTest = new Panel();
        private List<TestRunningInformation> testsTorun = new List<TestRunningInformation>();
        private TestExicutor testExicutor = new TestExicutor();

        private Dictionary<string, List<List<string>>> dictionaryOfGroupedPhases = new Dictionary<string, List<List<string>>>();

        public Form1()
        {
            InitializeComponent();
            generateTestMenuItem();
            this.testExicutor = new TestExicutor(previousGroupBox, previousjourneyQuantity, this.dataObject, this.orderOfPhases, this.previousTest, this.testsTorun, this.runTestsToolStripMenuItem, this.testLocations, this.requestTestsPanel, this.menuStrip2, this.RequestTests, this.TestExicutorPanel, this.portNumber);
            this.testExicutor.chooseTestToRun();


        }

        //new panl for running
        // clear everything else
        // play/pause get json
        // display data on seperate panels
        // get info on test http response
        // listner on seperate thread
        // refactor and improve 


        private void generateTestMenuItem()
        {
            // Populate Journey dropdown tab
            JsonObject dataTableJSON = RESTClient.makeRequest("http://localhost:" + portNumber+ "/Xml/JourneyOrder");
            var journeys = dataTableJSON["journeys"];

            for (int i = 0; i < journeys.Count; i++)
            {
                ToolStripDropDownButton journeyMenuItem = new ToolStripDropDownButton();
                journeyMenuItem.Text = (string)dataTableJSON["journeys"][i]["name"];
                // Add phases to journey 
                journeyMenuItem.Click += new EventHandler(this.makeNewPhaseItem);
                //menuItem.Click += (sender2, e2) => makeNewPhaseItem(sender2, e2, journeysToolStripMenuItem, currentJourney);
                journeysToolStripMenuItem.DropDownItems.Add(journeyMenuItem);

            }

        }

        private void makeNewPhaseItem(object sender, System.EventArgs e)
        {
            ToolStripDropDownButton toolStripDropDownButtonSepcificJourney = sender as ToolStripDropDownButton;

            this.phasesMenuStrip.Items.Clear();
            ToolStripDropDownButton phaseOrderTitle = new ToolStripDropDownButton();
            phaseOrderTitle.Text = "Phase Order";
            phasesMenuStrip.Items.Add(phaseOrderTitle);

            JsonObject dataTableJSON = RESTClient.makeRequest("http://localhost:"+portNumber+ "/Xml/JourneyOrder/" + toolStripDropDownButtonSepcificJourney.Text);
           
           
            var phaseOrder = dataTableJSON["phaseOrder"];

            AddNewPhasesGroupsInnerJourney(toolStripDropDownButtonSepcificJourney, phaseOrder);
        }

        private void AddNewPhasesGroupsInnerJourney(ToolStripDropDownButton toolStripDropDownButton, JsonValue phaseOrder)
        {
            for (int i = 0; i < phaseOrder.Count; i++)
            {
                ToolStripDropDownButton toolStripMenuItem = new ToolStripDropDownButton();
                // Handel phase order groups
                if (phaseOrder[i].ContainsKey("phaseOrderGroup"))
                {
                    string innerOrMainJourney = "JourneyOrder";
                    string groupName = toolStripMenuItem.Text;
                    groupPhases(toolStripDropDownButton, phaseOrder, i, toolStripMenuItem, innerOrMainJourney);
                }
                else
                {

                    if (phaseOrder[i].ContainsKey("target"))
                    {

                        string target = (string)phaseOrder[i]["target"];
                        
                        Panel quantityOfJourneysPanel;
                        NumericUpDown numberOfInnerJourneys;
                        
                        createPanelForQuantityOfInnerJourneys(target, out quantityOfJourneysPanel, out numberOfInnerJourneys);

                        ToolStripDropDownButton toolStripMenuItemInnerJourney = new ToolStripDropDownButton();
                        toolStripMenuItemInnerJourney.Name = target;
                        toolStripMenuItemInnerJourney.Width = 400;
                        toolStripMenuItemInnerJourney.Text = (string)phaseOrder[i]["target"];
                        toolStripMenuItemInnerJourney.Click += (sender2, e2) => toggleJourneyQuantity(sender2, e2, quantityOfJourneysPanel);
                        this.phasesMenuStrip.Items.Add(toolStripMenuItemInnerJourney);

                        string requestToREST = "http://localhost:"+portNumber+"/Xml/InnerJourneyOrder/" + target;
                        JsonObject innerjourneyPhasesJSON = RESTClient.makeRequest(requestToREST);


                        int numInnerJourneys = (int)numberOfInnerJourneys.Value;
                        string groupName = toolStripMenuItem.Text;
                        numberOfInnerJourneys.ValueChanged += (sender2, e2) => addInnerJourney(sender2, target, innerjourneyPhasesJSON, toolStripMenuItemInnerJourney, numInnerJourneys, groupName); 
                    }
                    else
                    {
                        // get phase data
                        createPhase(toolStripDropDownButton, phaseOrder, i, toolStripMenuItem);
                        phasesMenuStrip.Items.Add(toolStripMenuItem);

                    }
                    
                }
            }
        }

        private void createPhase(ToolStripDropDownButton journeyDropDownButton, JsonValue phaseOrder, int i, ToolStripDropDownButton phaseMenuButton)
        {
            String phaseText = (string)phaseOrder[i]["name"];
            String happyDefault = (string)phaseOrder[i]["happyDefault"];
            dataObject.phaseOrder.Add(journeyDropDownButton.Text + "|" + phaseText);


            dataObject.addPhaseAttributetoDataContainer(phaseText, new Dictionary<string, Dictionary<string, string>>());
            dataObject.addDataElementToDataObject(phaseText,"happyDefault", new Dictionary<string, string>());
            dataObject.addAtributeToElement(phaseText, "happyDefault", "value", happyDefault);


            string requestToREST = "http://localhost:"+portNumber+ "/Xml/" + journeyDropDownButton.Text + "/" + phaseText;

            phaseMenuButton.Text = phaseText;
            phaseMenuButton.Name = phaseText;

            JsonObject elementsJSON = RESTClient.makeRequest(requestToREST);

            Panel newPhasePanel = MagDisplayPanel.createMagPanel(phaseText);

            phaseMenuButton.Click += (sender2, e2) => toggleElementstoggleElementsMenuItem(sender2, e2, newPhasePanel);
           
                var htmlElements = elementsJSON["magElements"];
                addElementsToUi(newPhasePanel, htmlElements, (string)phaseOrder[i]["name"]);
            
            this.Controls.Add(newPhasePanel);
        }

        private void createPhase(ToolStripDropDownButton journeyDropDownButton, JsonValue phaseOrder, int i, ToolStripDropDownButton phaseMenuButton, int innerJourneyNumber)
        {
            string phaseName = (string)phaseOrder[i]["name"];
            //string phaseName = (string)phaseOrder[i]["name"] + innerJourneyNumber;
            String happyDefault = (string)phaseOrder[i]["happyDefault"];

            for (int j = 0; j < dataObject.phaseOrder.Count; j++)
            {
                string phaseTobeadded = dataObject.phaseOrder[j].Split('|')[0];
                string innerJourney = dataObject.phaseOrder[j];
                if (journeyDropDownButton.Text.Equals(dataObject.phaseOrder[j].Split('|')[0])&& dataObject.phaseOrder[j].Split('|')[1].Equals("innerjourney"))
                {
                    dataObject.phaseOrder.Insert(j, journeyDropDownButton.Text + "|" + phaseName);
                    j++;
                }

            }

            dataObject.addPhaseAttributetoDataContainer(phaseName, new Dictionary<string, Dictionary<string, string>>());
            dataObject.addDataElementToDataObject(phaseName, "happyDefault", new Dictionary<string, string>());
            dataObject.addAtributeToElement(phaseName, "happyDefault", "value", happyDefault);


            string requestToREST = "http://localhost:"+portNumber+ "/Xml/" + journeyDropDownButton.Text + "/" + (string)phaseOrder[i]["name"];

            phaseMenuButton.Text = phaseName;
            phaseMenuButton.Name = phaseName;

            JsonObject elementsJSON = RESTClient.makeRequest(requestToREST);

            Panel newPhasePanel = MagDisplayPanel.createMagPanel(phaseName);

            phaseMenuButton.Click += (sender2, e2) => toggleElementstoggleElementsMenuItem(sender2, e2, newPhasePanel);

            var htmlElements = elementsJSON["magElements"];


            addElementsToUi(newPhasePanel, htmlElements, phaseName);

            this.Controls.Add(newPhasePanel);
        }

        private void createPanelForQuantityOfInnerJourneys(string target, out Panel quantityOfJourneysPanel, out NumericUpDown numberOfInnerJourneys)
        {
            quantityOfJourneysPanel = new Panel();
            quantityOfJourneysPanel.BackColor = SystemColors.ActiveCaption;
            quantityOfJourneysPanel.Location = new Point(725, 27);
            quantityOfJourneysPanel.Name = "panel1";
            quantityOfJourneysPanel.Size = new Size(211, 79);
            quantityOfJourneysPanel.TabIndex = 9;
            TextBox journeyQuantityTextBox = new TextBox();
            journeyQuantityTextBox.Enabled = false;
            journeyQuantityTextBox.Location = new Point(1, 3);
            journeyQuantityTextBox.Name = target;
            journeyQuantityTextBox.AutoSize = true;
            journeyQuantityTextBox.TabIndex = 0;
            journeyQuantityTextBox.Text = target;
            journeyQuantityTextBox.TextAlign = HorizontalAlignment.Center;
            numberOfInnerJourneys = new NumericUpDown();
            numberOfInnerJourneys.Name = "testnum";
            numberOfInnerJourneys.Location = new Point(16, 41);
            numberOfInnerJourneys.Size = new Size(120, 120);
            numberOfInnerJourneys.TabIndex = 9;
            numberOfInnerJourneys.Maximum = 1;

            quantityOfJourneysPanel.Visible = false;
            quantityOfJourneysPanel.Controls.Add(journeyQuantityTextBox);
            this.Controls.Add(quantityOfJourneysPanel);
            quantityOfJourneysPanel.Controls.Add(numberOfInnerJourneys);
        }

        private void toggleJourneyQuantity(object sender2, EventArgs e2, Panel quantityOfJourneysPanel)
        {
            ToolStripDropDown toolStripDropDown = sender2 as ToolStripDropDown;


            if (quantityOfJourneysPanel.Visible == true)
            {
                previousGroupBox.Visible = false;
                quantityOfJourneysPanel.Visible = false;
                this.TestExicutorPanel.Visible = false;
            }
            else
            {
                previousGroupBox.Visible = false;
                quantityOfJourneysPanel.Visible = true;
                this.TestExicutorPanel.Visible = false;
            }
            previousjourneyQuantity = quantityOfJourneysPanel;
            this.testExicutor.previousjourneyQuantity = quantityOfJourneysPanel;
        }

        private void addInnerJourney(Object sender, string target, JsonObject innerjourneyPhasesJSON, ToolStripDropDownButton toolStripMenuItemInnerJourney, int numberOfInnerJourneys, string groupName)
        {

            NumericUpDown numericUpDown = sender as NumericUpDown;
            
            int lastInnerjourneyKey;
            if (toolStripMenuItemInnerJourney.DropDownItems.Count == 0)
            {
                lastInnerjourneyKey = 0;
            }
            else
            {
                ToolStripItem lastInnerJourney = toolStripMenuItemInnerJourney.DropDownItems[toolStripMenuItemInnerJourney.DropDownItems.Count - 1];
                string innerJourney = lastInnerJourney.Name;
                lastInnerjourneyKey = Int32.Parse(lastInnerJourney.Name);
            }
            int currentInnerjourneyKey = Int32.Parse(numericUpDown.Value.ToString());

            if (lastInnerjourneyKey < currentInnerjourneyKey)
            {        
                List<string> listOfPhasesInJourney = new List<string>();
                for (int l = 0; l < innerjourneyPhasesJSON["phaseOrder"].Count; l++)
                {
                    ToolStripDropDownButton innerJourneyPhase = new ToolStripDropDownButton();
                    listOfPhasesInJourney.Add(innerjourneyPhasesJSON["phaseOrder"][l]["name"]);
                    createPhase(toolStripMenuItemInnerJourney, innerjourneyPhasesJSON["phaseOrder"], l, innerJourneyPhase, currentInnerjourneyKey);

                    toolStripMenuItemInnerJourney.DropDownItems.Add(innerJourneyPhase);
                    //innerJourneyPhase.Name = numericUpDown.Value.ToString();
                    innerJourneyPhase.Name = innerJourneyPhase.ToString();
                    //innerJourney numbering
                    //innerJourneyPhase.Text = target + numericUpDown.Value;

                    innerJourneyPhase.Text = innerjourneyPhasesJSON["phaseOrder"][l]["name"];

                }
                this.dictionaryOfGroupedPhases[groupName].Add(listOfPhasesInJourney);
            }
            else
            {
                
                for (int l = 0; l < innerjourneyPhasesJSON["phaseOrder"].Count; l++)
                {
                    string phaseToBeremoved = (string)innerjourneyPhasesJSON["phaseOrder"][l]["name"] + lastInnerjourneyKey.ToString();
                    toolStripMenuItemInnerJourney.DropDownItems.RemoveByKey(lastInnerjourneyKey.ToString());
                    for (int i = 0; i < dataObject.phaseOrder.Count; i++)
                    {
                       
                        dataObject.phaseOrder.Remove(target+"|"+phaseToBeremoved);
                    }

                    
                }
            }
        }

        private void groupPhasesInnerJourney(ToolStripDropDownButton toolStripDropDownButton, JsonValue phaseOrder, int i, ToolStripDropDownButton groupedJourney, string innerOrMainJourney, ToolStripDropDownButton innerJourneyButton)
        {
            groupedJourney.Text = (string)phaseOrder[i]["name"];
            iterateThroughPhasesForGroupBy(toolStripDropDownButton, phaseOrder, i, groupedJourney, innerOrMainJourney, (string)phaseOrder[i]["name"]);
            innerJourneyButton.DropDownItems.Add(groupedJourney);


        }

        private void groupPhases(ToolStripDropDownButton toolStripDropDownButton, JsonValue phaseOrder, int i, ToolStripDropDownButton groupedJourney, string innerOrMainJourney)
        {
            groupedJourney.Text = (string)phaseOrder[i]["name"];
            iterateThroughPhasesForGroupBy(toolStripDropDownButton, phaseOrder, i, groupedJourney, innerOrMainJourney, (string)phaseOrder[i]["name"]);
            phasesMenuStrip.Items.Add(groupedJourney);

        }

        private void iterateThroughPhasesForGroupBy(ToolStripDropDownButton toolStripDropDownButton, JsonValue phaseOrder, int i, ToolStripDropDownButton groupedJourney, string innerOrMainJourney, string groupName)
        {
            string phaseOrderGroupName = (string)phaseOrder[i]["name"];
            List<string> phasesInGroup = new List<string>();
            List<List<string>> phasesJourney = new List<List<string>>();
            phasesJourney.Add(phasesInGroup);

            for (int j = 0; j < phaseOrder[i]["phaseOrderGroup"].Count; j++)
            {
               
                Console.WriteLine(phaseOrder[i]["phaseOrderGroup"][j].ToString());
                String phaseText;
                if (phaseOrder[i]["phaseOrderGroup"][j].ContainsKey("target"))
                {
                    phaseText = (string)phaseOrder[i]["phaseOrderGroup"][j]["target"];
                    string target = (string)phaseOrder[i]["phaseOrderGroup"][j]["target"];
               
                    dataObject.phaseOrder.Add(target + "|innerjourney");
                    Panel quantityOfJourneysPanel;
                    NumericUpDown numberOfInnerJourneys;
                    createPanelForQuantityOfInnerJourneys(target, out quantityOfJourneysPanel, out numberOfInnerJourneys);

                    ToolStripDropDownButton toolStripMenuItemInnerJourney = new ToolStripDropDownButton();
                    toolStripMenuItemInnerJourney.Name = target;
                    toolStripMenuItemInnerJourney.Text = target;
                    
                    toolStripMenuItemInnerJourney.Width = 150;
                    toolStripMenuItemInnerJourney.Click += (sender2, e2) => toggleJourneyQuantity(sender2, e2, quantityOfJourneysPanel);
                    groupedJourney.DropDownItems.Add(toolStripMenuItemInnerJourney);

                    string requestToREST = "http://localhost:"+portNumber+ "/Xml/InnerJourneyOrder" + "/" + target;
                    JsonObject innerjourneyPhasesJSON = RESTClient.makeRequest(requestToREST);


                    int numInnerJourneys = (int)numberOfInnerJourneys.Value;
                    numberOfInnerJourneys.ValueChanged += (sender2, e2) => addInnerJourney(sender2, target, innerjourneyPhasesJSON, toolStripMenuItemInnerJourney, numInnerJourneys, groupName);
                }
                else
                {

                    var phaseGroup = phaseOrder[i]["phaseOrderGroup"];
                    var phaseName = phaseGroup[j];
                    phasesInGroup.Add((string)phaseOrder[i]["phaseOrderGroup"][j]["name"]);
                    phaseText = (string)phaseName["name"];
                    ToolStripDropDownButton groupedMenuItem = new ToolStripDropDownButton();
                    groupedMenuItem.Text = phaseText;
                    groupedMenuItem.Name = phaseOrderGroupName;
                    JsonObject EllementsJSON = RESTClient.makeRequest("http://localhost:"+portNumber+"/" + innerOrMainJourney + "/" + toolStripDropDownButton.Text + "/" + phaseText);
                    Panel flowLayoutPanel = MagDisplayPanel.createMagPanel(phaseText);
                    groupedMenuItem.Click += (sender2, e2) => toggleElementstoggleElementsMenuItemGroup(sender2, e2, flowLayoutPanel);
                    dataObject.addPhaseAttributetoDataContainer(phaseText, new Dictionary<string, Dictionary<string, string>>());
                    JsonValue phaseOrderGroup = phaseOrder[i]["phaseOrderGroup"];
                    createPhase(toolStripDropDownButton, phaseOrderGroup, j, groupedMenuItem);
                    this.Controls.Add(flowLayoutPanel);
                    groupedJourney.DropDownItems.Add(groupedMenuItem);
                }

            }
            dictionaryOfGroupedPhases.Add(phaseOrderGroupName, phasesJourney);
        }

        private void addElementsToUi(Panel magratheaElementsPanel, JsonValue htmlElements, string phase)
        {
            int xLocationOnPanel = 0;
            int yLocationOnPanel = 0;

            for (int k = 0; k < htmlElements.Count; k++)
            {
                JsonValue htmlElement = htmlElements[k];
                changeJsonToElement(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);
                SubmitTestData submitTestData = new SubmitTestData(dataObject);
                submitTestData.submitData(magratheaElementsPanel, htmlElement, phase);
                xLocationOnPanel = xLocationOnPanel + 50;
            }
        }

        private void changeJsonToElement(Panel magratheaElementsPanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {


            

            string inputType = (string)htmlElement["inputType"].ToString();
            

            if (((string)htmlElement["inputType"]).Equals("number"))
            {

                MagNumberElement magNumberElement = new MagNumberElement(dataObject);
                magNumberElement.createNumericUpDownboxWithLabel(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);

            }
            else if (((string)htmlElement["inputType"]).Equals("checkbox"))
            {
                MagCheckBoxElement magCheckBoxElement = new MagCheckBoxElement(dataObject);
                magCheckBoxElement.createCheckBox(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);

            }
            else if (((string)htmlElement["inputType"]).Equals("dropdown"))
            {

               
                MagDropDownElement magDropDownElement = new MagDropDownElement(dataObject);
                magDropDownElement.createNumericUpDownboxWithLabel(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);

            }
            else if (((string)htmlElement["inputType"]).Equals("text"))
            {

             
                MagTextElements magTextElements = new MagTextElements(dataObject);
                magTextElements.createStringInput(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);

            }
            else if (((string)htmlElement["inputType"]).Equals("date"))
            {

                MagTimeDateElement magTimeDateElement = new MagTimeDateElement(dataObject);
                magTimeDateElement.createDateTimeInput(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);

            }
            else if (((string)htmlElement["inputType"]).Equals("list"))
            {

                createList(magratheaElementsPanel, htmlElement, xLocationOnPanel, yLocationOnPanel, phase);

            }
        }

        private void createList(Panel magPanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {
            string pattern = @"\d+$";
            Regex rgx = new Regex(pattern);
            string labelText = MagElementHelper.tryAndGetAttribute(htmlElement, "text");
            NumericUpDown magNumericUpDown = MagFormElements.magNumericUpDown(htmlElement);
            Panel panelOfItems = MagDisplayPanel.createPanelWithNumericUpDown(magNumericUpDown, xLocationOnPanel,yLocationOnPanel);
            magNumericUpDown.ValueChanged += (sender, e) => addItemToList(sender, e, htmlElement, panelOfItems, phase);
            Label label = MagElementHelper.createLabel(labelText, xLocationOnPanel, yLocationOnPanel);
            magPanel.Controls.Add(label);
            panelOfItems.Controls.Add(magNumericUpDown);
            magPanel.Controls.Add(panelOfItems);
        }

        private void addItemToList(object sender, EventArgs e, JsonValue listItems, Panel panelOfItems, string phase)
        {
            string pattern = @"\d+$";
            Regex rgx = new Regex(pattern);
            string listElementName = rgx.Replace((string)listItems["listItems"][0]["elementId"], "");

            NumericUpDown spinnerListItems = sender as NumericUpDown;
            int currentValueOnSpinner = Int32.Parse(spinnerListItems.Value.ToString());
            int currentNumberOfListItems;
            if (panelOfItems.Controls.Count == 1)
            {
                currentNumberOfListItems = 0;
            }
            else
            {
                var lastItemOnTheList = panelOfItems.Controls[panelOfItems.Controls.Count - 1];
                string item = lastItemOnTheList.Name;
                currentNumberOfListItems = Int32.Parse(lastItemOnTheList.Name);
            }

            if (currentNumberOfListItems < currentValueOnSpinner)
            {
                Panel itemPanel = new Panel();
                itemPanel.BackColor = SystemColors.HotTrack;
                itemPanel.Name = (currentValueOnSpinner-1).ToString();
                itemPanel.AutoSize = true;
                var panelItems = panelOfItems.Controls[0].ToString();
                int xLocationOnPanel = 0;
                int yLocationOnPanel = 0;
                int countOfAccidents = 0;
                
                foreach (JsonValue listItem in listItems["listItems"])
                {
                    
                    string numbersRemovedFromEndOfString = rgx.Replace((string)listItems["listItems"][countOfAccidents]["elementId"], "");
                    listItems["listItems"][countOfAccidents]["elementId"] = numbersRemovedFromEndOfString + (currentValueOnSpinner-1).ToString();
                    countOfAccidents++;
                    changeJsonToElement(itemPanel, listItem, xLocationOnPanel, yLocationOnPanel, phase);
                    xLocationOnPanel = 100 + xLocationOnPanel;
                    
                }
                if (currentValueOnSpinner==1)
                {
                    itemPanel.Location = new Point(0, 50);
                }
                 else
                {
                    Console.WriteLine(xLocationOnPanel);
                    itemPanel.Location = new Point(0, xLocationOnPanel * (currentValueOnSpinner - 1) + 100);
                }
                
                xLocationOnPanel = 0;
                panelOfItems.Controls.Add(itemPanel);
            }
            else
            {
                if (currentNumberOfListItems == -1)
                {
                    
                }
                else
                {
                    var data = dataObject;
                    dataObject.removeEllementFromDictionary(phase, listElementName + panelOfItems.Controls[panelOfItems.Controls.Count - 1].Name);
                    panelOfItems.Controls.Remove(panelOfItems.Controls[panelOfItems.Controls.Count - 1]);
                }
            }

        }

        private void toggleElementstoggleElementsMenuItemGroup(object sender, EventArgs e, Panel newGroupBox)
        {
        
            //private Dictionary<string, List<List<string>>> dictionaryOfGroupedPhases = new Dictionary<string, List<List<string>>>();
        ToolStripDropDownButton toolStripMenuItem = sender as ToolStripDropDownButton;
            string phaseName = toolStripMenuItem.Name;
            string groupName = null;
            
            foreach (var journeygroup in this.dictionaryOfGroupedPhases)
            {
                foreach (List<string> group in journeygroup.Value)
                {
                    if (group.Contains(toolStripMenuItem.Name))
                    {
                        foreach (var phase in group)
                        {
                            dataObject.addAtributeToElement(phase, "happyDefault", "value", "true");
                            groupName = journeygroup.Key;

                        }

                    }
                    

                }
             
            }
            
                foreach (var group in this.dictionaryOfGroupedPhases[groupName])
                {
                    if (!(group.Contains(toolStripMenuItem.Name)))
                    {
                        foreach (var phase in group)
                        {
                            dataObject.addAtributeToElement(phase, "happyDefault", "value", "false");


                        }

                    }
                }
            

            
            if (newGroupBox.Visible == true)
            {
                previousGroupBox.Visible = false;
                newGroupBox.Visible = false;
                previousjourneyQuantity.Visible = false;
                this.requestTestsPanel.Visible = false;
                this.TestExicutorPanel.Visible = false;

            }
            else
            {
                previousGroupBox.Visible = false;
                newGroupBox.Visible = true;
                previousjourneyQuantity.Visible = false;
                this.requestTestsPanel.Visible = false;
                this.TestExicutorPanel.Visible = false;

            }
            previousGroupBox = newGroupBox;

        }


        private void toggleElementstoggleElementsMenuItem(object sender, EventArgs e, Panel newGroupBox)
        {

            //private Dictionary<string, List<List<string>>> dictionaryOfGroupedPhases = new Dictionary<string, List<List<string>>>();
            ToolStripDropDownButton toolStripMenuItem = sender as ToolStripDropDownButton;
            string phaseName = toolStripMenuItem.Name;


            foreach (var journeygroup in this.dictionaryOfGroupedPhases)
            {
                foreach (List<string> group in journeygroup.Value)
                {
                    if (group.Contains(toolStripMenuItem.Name))
                    {
                        foreach (var phase in group)
                        {
                            dataObject.addAtributeToElement(phase, "happyDefault", "value", "false");


                        }

                    }


                }

            }
            dataObject.addAtributeToElement(phaseName, "happyDefault", "value", "true");


            
            if (newGroupBox.Visible == true)
            {
                previousGroupBox.Visible = false;
                newGroupBox.Visible = false;
                previousjourneyQuantity.Visible = false;
                this.requestTestsPanel.Visible = false;
                this.TestExicutorPanel.Visible = false;

            }
            else
            {
                previousGroupBox.Visible = false;
                newGroupBox.Visible = true;
                previousjourneyQuantity.Visible = false;
                this.requestTestsPanel.Visible = false;
                this.TestExicutorPanel.Visible = false;

            }
            previousGroupBox = newGroupBox;
        }

        private void journeysToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void RequestTests_Click(object sender, EventArgs e)
        {

        }
    }
}
