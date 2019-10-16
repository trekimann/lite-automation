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
    class MagCheckBoxElement
    {
        private DataObject dataObject;

        public MagCheckBoxElement(DataObject dataObject)
        {
            this.dataObject = dataObject;
        }

        public void createCheckBox(Panel magratheaElementsPanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {
            CheckBox checkBox = new CheckBox();
            string labelText = (string)htmlElement["text"];
            var label = MagElementHelper.createLabel(labelText, xLocationOnPanel, yLocationOnPanel);
            checkBox.Location = new Point(yLocationOnPanel, xLocationOnPanel + 25);
            checkBox.Name = (string)htmlElement["elementId"];

            dataObject.addDataElementToDataObject(phase, checkBox.Name, new Dictionary<string, string>());
            dataObject.addAtributeToElement(phase, checkBox.Name, "type", "Boolean");


            if (((string)htmlElement["value"]).Equals("true"))
            {
                checkBox.Checked = true;
            }
            else
            {
                checkBox.Checked = false;
            }
            var check = checkBox.CheckState.ToString();
            if (check.Equals("Unchecked"))
            {
                dataObject.addAtributeToElement(phase, checkBox.Name, "value", "false");
            }
            else
            {
                dataObject.addAtributeToElement(phase, checkBox.Name, "value", "true");
            }

            checkBox.CheckedChanged += (sender, e) => addBooleanToDataObject(sender, e, phase);
            magratheaElementsPanel.Controls.Add(label);
            magratheaElementsPanel.Controls.Add(checkBox);
        }

        private void addBooleanToDataObject(object sender, EventArgs e, string phase)
        {
            CheckBox checkBox = sender as CheckBox;
            var check = checkBox.CheckState.ToString();
            if (check.Equals("unchecked"))
            {
                dataObject.addAtributeToElement(phase, checkBox.Name, "value", "false");
            }
            else
            {
                dataObject.addAtributeToElement(phase, checkBox.Name, "value", "true");
            }


        }
    }
}
