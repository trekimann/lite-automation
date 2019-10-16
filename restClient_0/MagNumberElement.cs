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
    class MagNumberElement
    {

        private DataObject dataObject;

        public MagNumberElement(DataObject dataObject)
        {
            this.dataObject = dataObject;
        }

        public void createNumericUpDownboxWithLabel(Panel magratheaElementsPanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {

            var label = MagElementHelper.createLabel((string)htmlElement["text"], xLocationOnPanel, yLocationOnPanel);
            var numericUpDown = new NumericUpDown();
            numericUpDown.Location = new Point(yLocationOnPanel, xLocationOnPanel + 25);
            try
            {
                numericUpDown.Maximum = (int)htmlElement["max"];
                numericUpDown.Minimum = (int)htmlElement["min"];
                numericUpDown.Value = (int)htmlElement["value"];
            }
            catch { }
            numericUpDown.Name = (string)htmlElement["elementId"];

            dataObject.addDataElementToDataObject(phase, numericUpDown.Name, new Dictionary<string, string>());
            dataObject.addAtributeToElement(phase, numericUpDown.Name, "value", numericUpDown.Value.ToString());
            dataObject.addAtributeToElement(phase, numericUpDown.Name, "type", "int");

            numericUpDown.ValueChanged += (sender, e) => addIntToDataObject(sender, e, phase);
            magratheaElementsPanel.Controls.Add(label);
            magratheaElementsPanel.Controls.Add(numericUpDown);
        }

        private void addIntToDataObject(object sender, EventArgs e, string phase)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            dataObject.addAtributeToElement(phase, numericUpDown.Name, "value", numericUpDown.Value.ToString());


        }
    }
}
