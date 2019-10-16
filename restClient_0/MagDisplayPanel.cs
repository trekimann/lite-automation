using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace restClient_0
{
    class MagDisplayPanel
    {
        public static Panel createMagPanel(string phaseName)
        {
            Panel magPanel = new Panel();
            magPanel.BackColor = SystemColors.ActiveCaption;
            magPanel.Location = new Point(270, 50);
            magPanel.Name = phaseName;
            magPanel.Size = new Size(900, 600);
            magPanel.TabIndex = 9;
            magPanel.AutoScroll = true;
            magPanel.TabStop = true;
            magPanel.Text = phaseName;
            magPanel.Visible = false;
            return magPanel;
        }

        public static Panel createPanelWithNumericUpDown(NumericUpDown numericUpDown, int xLocationOnPanel, int yLocationOnPanel)
        {
            Panel panelOfItems = new Panel();
            panelOfItems.BackColor = SystemColors.ControlLight;
            panelOfItems.AutoSize = true;
            panelOfItems.Location = new Point(yLocationOnPanel, xLocationOnPanel);
            panelOfItems.Controls.Add(numericUpDown);
            return panelOfItems;
        }
    }
}
