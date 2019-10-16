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
    class MagDropDownElement
    {

        private DataObject dataObject;

        public MagDropDownElement(DataObject dataObject)
        {
            this.dataObject = dataObject;
        }

        public void createNumericUpDownboxWithLabel(Panel newPhasePanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {

            var label = MagElementHelper.createLabel((string)htmlElement["text"], xLocationOnPanel, yLocationOnPanel);
            var dropDown = new ComboBox();

            dropDown.Name = (string)htmlElement["elementId"];
            dropDown.Location = new Point(yLocationOnPanel, xLocationOnPanel + 25);
            dropDown.Text = htmlElement["value"];

            foreach (JsonValue dropDownItem in htmlElement["dropDownItems"])
            {
                dropDown.Items.Add((string)dropDownItem);
            }
            newPhasePanel.Controls.Add(label);
            newPhasePanel.Controls.Add(dropDown);

            dataObject.addDataElementToDataObject(phase, dropDown.Name, new Dictionary<string, string>());
                dataObject.addAtributeToElement(phase, dropDown.Name, "value", dropDown.Text);
                dataObject.addAtributeToElement(phase, dropDown.Name, "type", "string");

            dropDown.TextChanged += (sender, e) => adddropDownValue(sender, e, phase);

        }

        private void adddropDownValue(object sender, EventArgs e, string phase)
        {
                ComboBox numericUpDown = sender as ComboBox;
                dataObject.addAtributeToElement(phase, numericUpDown.Name, "value", numericUpDown.Text);
        }
    }
}
