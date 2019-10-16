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
    class MagTextElements
    {
        private DataObject dataObject;

        public MagTextElements(DataObject dataObject)
        {
            this.dataObject = dataObject;
        }

        public void createStringInput(Panel magratheaElementsPanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {
            JsonValue specificElement = htmlElement;
            string labelText = MagElementHelper.tryAndGetAttribute(specificElement, "text");
            string textBoxName = MagElementHelper.tryAndGetAttribute(specificElement, "elementId");
            string textBoxValue = MagElementHelper.tryAndGetAttribute(specificElement, "value");
            Label label = MagElementHelper.createLabel(labelText, xLocationOnPanel, yLocationOnPanel);
            TextBox textbox = new TextBox();
            textbox.Location = new Point(yLocationOnPanel, xLocationOnPanel + 25);
            textbox.Text = textBoxValue;
            textbox.Name = textBoxName;
            magratheaElementsPanel.Controls.Add(label);
            magratheaElementsPanel.Controls.Add(textbox);
            dataObject.addDataElementToDataObject(phase, textBoxName, new Dictionary<string, string>());
            dataObject.addAtributeToElement(phase, textBoxName, "value", textBoxValue);
            dataObject.addAtributeToElement(phase, textBoxName, "type", "string");
            textbox.TextChanged += (sender2, e2) => addTextToDataObject(sender2, e2, phase);
        }

        private void addTextToDataObject(object sender2, EventArgs e2, string phase)
        {
            TextBox textBox = sender2 as TextBox;
            dataObject.addAtributeToElement(phase, textBox.Name, "value", textBox.Text);
        }

      
    }
}
