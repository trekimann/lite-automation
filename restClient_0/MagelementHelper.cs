using System;
using System.Drawing;
using System.Json;
using System.Windows.Forms;

namespace restClient_0
{
    class MagElementHelper
    {
        public static string tryAndGetAttribute(JsonValue htmlElements, string attribute)
        {
            string toReturn = "";
            try
            {
                toReturn = (string)htmlElements[attribute];
            }
            catch
            {
                Console.WriteLine("Attribute " + attribute + "was not found");
            }
            return toReturn;

        }

        public static Label createLabel(string labelText, int x, int y)
        {
            
            var label = new Label();
            label.Text = labelText;
            label.Location = new Point(y, x);
            label.AutoSize = true;
            return label;
        }

        public static void stopMouseWheelFromWorking(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }
}
