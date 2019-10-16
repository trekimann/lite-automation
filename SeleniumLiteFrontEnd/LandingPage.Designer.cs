namespace SeleniumLiteFrontEnd
{
    partial class LandingPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabCtrlLanding = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TabCtrlLanding.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabCtrlLanding
            // 
            this.TabCtrlLanding.Controls.Add(this.tabPage1);
            this.TabCtrlLanding.Controls.Add(this.tabPage2);
            this.TabCtrlLanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabCtrlLanding.Location = new System.Drawing.Point(0, 0);
            this.TabCtrlLanding.Name = "TabCtrlLanding";
            this.TabCtrlLanding.SelectedIndex = 0;
            this.TabCtrlLanding.Size = new System.Drawing.Size(1064, 682);
            this.TabCtrlLanding.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1056, 656);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Generate Tests";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1032, 632);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Run Tests";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // LandingPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 682);
            this.Controls.Add(this.TabCtrlLanding);
            this.Name = "LandingPage";
            this.Text = "SeleniumLite: Magrathea";
            this.TabCtrlLanding.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabCtrlLanding;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}

