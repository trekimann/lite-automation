using System.Drawing;
using System.Windows.Forms;

namespace restClient_0
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.journeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phasesMenuStrip = new System.Windows.Forms.MenuStrip();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.requestTestsPanel = new System.Windows.Forms.Panel();
            this.testLocations = new System.Windows.Forms.TextBox();
            this.RequestTests = new System.Windows.Forms.Button();
            this.TestExicutorPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.requestTestsPanel.SuspendLayout();
            this.TestExicutorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.journeysToolStripMenuItem,
            this.runTestsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(30, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1234, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // journeysToolStripMenuItem
            // 
            this.journeysToolStripMenuItem.Name = "journeysToolStripMenuItem";
            this.journeysToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.journeysToolStripMenuItem.Text = "Generate Tests";
            this.journeysToolStripMenuItem.Click += new System.EventHandler(this.journeysToolStripMenuItem_Click);
            // 
            // runTestsToolStripMenuItem
            // 
            this.runTestsToolStripMenuItem.Name = "runTestsToolStripMenuItem";
            this.runTestsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.runTestsToolStripMenuItem.Text = "Run Tests";
            // 
            // phasesMenuStrip
            // 
            this.phasesMenuStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.phasesMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.phasesMenuStrip.Name = "phasesMenuStrip";
            this.phasesMenuStrip.Size = new System.Drawing.Size(30, 682);
            this.phasesMenuStrip.TabIndex = 8;
            this.phasesMenuStrip.Text = "menuStrip2";
            // 
            // menuStrip2
            // 
            this.menuStrip2.Location = new System.Drawing.Point(30, 24);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1234, 24);
            this.menuStrip2.TabIndex = 9;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // requestTestsPanel
            // 
            this.requestTestsPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.requestTestsPanel.Controls.Add(this.testLocations);
            this.requestTestsPanel.Controls.Add(this.RequestTests);
            this.requestTestsPanel.Location = new System.Drawing.Point(256, 57);
            this.requestTestsPanel.Name = "requestTestsPanel";
            this.requestTestsPanel.Size = new System.Drawing.Size(315, 84);
            this.requestTestsPanel.TabIndex = 10;
            this.requestTestsPanel.Visible = false;
            // 
            // testLocations
            // 
            this.testLocations.Location = new System.Drawing.Point(14, 31);
            this.testLocations.Name = "testLocations";
            this.testLocations.Size = new System.Drawing.Size(163, 20);
            this.testLocations.TabIndex = 1;
            // 
            // RequestTests
            // 
            this.RequestTests.Location = new System.Drawing.Point(183, 29);
            this.RequestTests.Name = "RequestTests";
            this.RequestTests.Size = new System.Drawing.Size(115, 23);
            this.RequestTests.TabIndex = 0;
            this.RequestTests.Text = "Request Tests";
            this.RequestTests.UseVisualStyleBackColor = true;
            // 
            // TestExicutorPanel
            // 
            this.TestExicutorPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.TestExicutorPanel.Controls.Add(this.requestTestsPanel);
            this.TestExicutorPanel.Location = new System.Drawing.Point(231, 54);
            this.TestExicutorPanel.Name = "TestExicutorPanel";
            this.TestExicutorPanel.Size = new System.Drawing.Size(1033, 439);
            this.TestExicutorPanel.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.TestExicutorPanel);
            this.Controls.Add(this.menuStrip2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.phasesMenuStrip);
            this.Location = new System.Drawing.Point(0, 30);
            this.MainMenuStrip = this.menuStrip2;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 720);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "c# REST Client";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.requestTestsPanel.ResumeLayout(false);
            this.requestTestsPanel.PerformLayout();
            this.TestExicutorPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem journeysToolStripMenuItem;
        private System.Windows.Forms.MenuStrip phasesMenuStrip;
        private System.Windows.Forms.NumericUpDown testnum;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ToolStripMenuItem runTestsToolStripMenuItem;
        private MenuStrip menuStrip2;
        private Panel requestTestsPanel;
        private Button RequestTests;
        private TextBox testLocations;
        private Panel TestExicutorPanel;
    }
}

