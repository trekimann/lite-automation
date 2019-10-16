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
    class MagTimeDateElement
    {
        private DataObject dataObject;

        public MagTimeDateElement(DataObject dataObject)
        {
            this.dataObject = dataObject;
        }

        public void createDateTimeInput(Panel newFlowLayoutPanel, JsonValue htmlElement, int xLocationOnPanel, int yLocationOnPanel, string phase)
        {
            string labelText = (string)htmlElement["text"];
            string datePreSet = (string)htmlElement["value"];
            var datePreSetSplit = datePreSet.Split('-');
            var year = Int32.Parse(datePreSetSplit[0]);
            var month = Int32.Parse(datePreSetSplit[1]);
            var day = Int32.Parse(datePreSetSplit[2]);

            Label label = MagElementHelper.createLabel(labelText, xLocationOnPanel, yLocationOnPanel);
            DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.Name = (string)htmlElement["elementId"];
            dateTimePicker.Value = new DateTime(year, month, day);
            dateTimePicker.Location = new Point(yLocationOnPanel, xLocationOnPanel + 25);

            dataObject.addDataElementToDataObject(phase, (string)htmlElement["elementId"], new Dictionary<string, string>());
            dataObject.addAtributeToElement(phase, (string)htmlElement["elementId"], "value", dateTimePicker.Value.ToString());
            dataObject.addAtributeToElement(phase, (string)htmlElement["elementId"], "type", "dateTime");


            dateTimePicker.ValueChanged += (sender, e) => addDateTimeToDataObject(sender, e, phase);
            newFlowLayoutPanel.Controls.Add(label);
            newFlowLayoutPanel.Controls.Add(dateTimePicker);

        }

        private void addDateTimeToDataObject(object sender, EventArgs e, string phase)
        {
            DateTimePicker dateTimePicker = sender as DateTimePicker;
            dataObject.addAtributeToElement(phase, dateTimePicker.Name, "value", dateTimePicker.Value.ToString());

        }
    }
}
