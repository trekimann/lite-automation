using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace restClient_0
{
    class JourneyOrder
    {
        public void populateUiJourneyList(string restRequest, MenuStrip topMenuStrip, MenuStrip leftMenuStrip)
        {
            JsonObject dataTableJSON = RESTClient.makeRequest(restRequest);
            var journeys = dataTableJSON["journeys"];

            for (int i = 0; i < journeys.Count; i++)
            {
                ToolStripDropDownButton journeyMenuItem = new ToolStripDropDownButton();
                journeyMenuItem.Text = (string)dataTableJSON["journeys"][i]["name"];
                // Add phases to journey 
                journeyMenuItem.Click += (sender2, e2) => makeNewPhaseItem(sender2, e2, leftMenuStrip);
                topMenuStrip.Items.Add(journeyMenuItem);

            }
        }

        private void makeNewPhaseItem(object sender, System.EventArgs e, MenuStrip leftMenuStrip)
        {
            ToolStripDropDownButton toolStripDropDownButton = sender as ToolStripDropDownButton;

            leftMenuStrip.Items.Clear();
            ToolStripDropDownButton phaseOrderTitle = new ToolStripDropDownButton();
            phaseOrderTitle.Text = "Phase Order";
            leftMenuStrip.Items.Add(phaseOrderTitle); 
        
        }

    }
}
