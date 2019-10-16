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
    class MagFormElements
    {
        public static NumericUpDown magNumericUpDown(JsonValue htmlElement)
        {
            JsonValue specificJsonValue = htmlElement;
            
            string min = MagElementHelper.tryAndGetAttribute(specificJsonValue, "min");
            string max = MagElementHelper.tryAndGetAttribute(specificJsonValue, "max");
            string value = MagElementHelper.tryAndGetAttribute(specificJsonValue, "value");
            NumericUpDown numericUpDown = new NumericUpDown();
            if (!max.Equals(""))
            {
                numericUpDown.Maximum = Int32.Parse(max);
            }

            if (!min.Equals(""))
            {
                numericUpDown.Minimum = Int32.Parse(min);
            }

            if (value.Equals(""))
            {
                numericUpDown.Value = 0;
            }
            else { numericUpDown.Value = Int32.Parse(value); }
            numericUpDown.ReadOnly = true;
            numericUpDown.MouseWheel += new MouseEventHandler(MagElementHelper.stopMouseWheelFromWorking);
            numericUpDown.Location = new Point(0, 20);
            
            return numericUpDown;

        }

    }
}
