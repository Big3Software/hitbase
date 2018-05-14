// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.DesktopIntegration.IMClient
{
    partial class IMClient
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
            this.components = new System.ComponentModel.Container();
            this.lstBuddies = new System.Windows.Forms.ListBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.buddyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendFileTimer = new System.Windows.Forms.Timer(this.components);
            this.buddyContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstBuddies
            // 
            this.lstBuddies.ContextMenuStrip = this.buddyContextMenu;
            this.lstBuddies.FormattingEnabled = true;
            this.lstBuddies.Location = new System.Drawing.Point(12, 64);
            this.lstBuddies.Name = "lstBuddies";
            this.lstBuddies.Size = new System.Drawing.Size(195, 186);
            this.lstBuddies.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 23);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(59, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Set Status:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "Available",
            "Away",
            "Appear Offline"});
            this.cmbStatus.Location = new System.Drawing.Point(78, 23);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(129, 21);
            this.cmbStatus.TabIndex = 2;
            this.cmbStatus.SelectedIndexChanged += new System.EventHandler(this.cmbStatus_SelectedIndexChanged);
            // 
            // buddyContextMenu
            // 
            this.buddyContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendFileToolStripMenuItem});
            this.buddyContextMenu.Name = "buddyContextMenu";
            this.buddyContextMenu.Size = new System.Drawing.Size(131, 26);
            // 
            // sendFileToolStripMenuItem
            // 
            this.sendFileToolStripMenuItem.Name = "sendFileToolStripMenuItem";
            this.sendFileToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.sendFileToolStripMenuItem.Text = "Send File...";
            this.sendFileToolStripMenuItem.Click += new System.EventHandler(this.sendFileToolStripMenuItem_Click);
            // 
            // IMClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 262);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lstBuddies);
            this.Name = "IMClient";
            this.Text = "Simple IM Client";
            this.Load += new System.EventHandler(this.IMClient_Load);
            this.buddyContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstBuddies;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.ContextMenuStrip buddyContextMenu;
        private System.Windows.Forms.ToolStripMenuItem sendFileToolStripMenuItem;
        private System.Windows.Forms.Timer sendFileTimer;
    }
}