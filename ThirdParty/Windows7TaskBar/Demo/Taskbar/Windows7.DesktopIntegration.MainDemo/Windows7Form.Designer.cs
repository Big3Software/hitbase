// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.DesktopIntegration.MainDemo
{
    partial class Windows7Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Windows7Form));
            this.btnChangeAppId = new System.Windows.Forms.Button();
            this.btnProgressDemo = new System.Windows.Forms.Button();
            this.btnAddToRecent = new System.Windows.Forms.Button();
            this.btnToggleOverlay = new System.Windows.Forms.Button();
            this.btnBuildJumpList = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnBGWorker = new System.Windows.Forms.Button();
            this.btnTogglePreview = new System.Windows.Forms.Button();
            this.btnCreateChild = new System.Windows.Forms.Button();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseOverlayIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.administrativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerFileExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unregisterFileExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnThumbClip = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnChangeAppId
            // 
            this.btnChangeAppId.Location = new System.Drawing.Point(253, 185);
            this.btnChangeAppId.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnChangeAppId.Name = "btnChangeAppId";
            this.btnChangeAppId.Size = new System.Drawing.Size(226, 53);
            this.btnChangeAppId.TabIndex = 5;
            this.btnChangeAppId.Text = "Change App Id";
            this.btnChangeAppId.UseVisualStyleBackColor = true;
            this.btnChangeAppId.Click += new System.EventHandler(this.btnChangeAppId_Click);
            // 
            // btnProgressDemo
            // 
            this.btnProgressDemo.Location = new System.Drawing.Point(15, 51);
            this.btnProgressDemo.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnProgressDemo.Name = "btnProgressDemo";
            this.btnProgressDemo.Size = new System.Drawing.Size(226, 53);
            this.btnProgressDemo.TabIndex = 0;
            this.btnProgressDemo.Text = "Progress Bar";
            this.btnProgressDemo.UseVisualStyleBackColor = true;
            this.btnProgressDemo.Click += new System.EventHandler(this.btnProgressDemo_Click);
            // 
            // btnAddToRecent
            // 
            this.btnAddToRecent.Location = new System.Drawing.Point(15, 118);
            this.btnAddToRecent.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnAddToRecent.Name = "btnAddToRecent";
            this.btnAddToRecent.Size = new System.Drawing.Size(226, 53);
            this.btnAddToRecent.TabIndex = 1;
            this.btnAddToRecent.Text = "Add to Recent";
            this.btnAddToRecent.UseVisualStyleBackColor = true;
            this.btnAddToRecent.Click += new System.EventHandler(this.btnAddToRecent_Click);
            // 
            // btnToggleOverlay
            // 
            this.btnToggleOverlay.Location = new System.Drawing.Point(15, 185);
            this.btnToggleOverlay.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnToggleOverlay.Name = "btnToggleOverlay";
            this.btnToggleOverlay.Size = new System.Drawing.Size(226, 53);
            this.btnToggleOverlay.TabIndex = 2;
            this.btnToggleOverlay.Text = "Toggle Overlay Icon";
            this.btnToggleOverlay.UseVisualStyleBackColor = true;
            this.btnToggleOverlay.Click += new System.EventHandler(this.btnToggleOverlay_Click);
            // 
            // btnBuildJumpList
            // 
            this.btnBuildJumpList.Location = new System.Drawing.Point(253, 51);
            this.btnBuildJumpList.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnBuildJumpList.Name = "btnBuildJumpList";
            this.btnBuildJumpList.Size = new System.Drawing.Size(226, 53);
            this.btnBuildJumpList.TabIndex = 3;
            this.btnBuildJumpList.Text = "Build Jump List";
            this.btnBuildJumpList.UseVisualStyleBackColor = true;
            this.btnBuildJumpList.Click += new System.EventHandler(this.btnBuildJumpList_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 253);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(744, 35);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 29);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(70, 30);
            this.statusLabel.Text = "Ready";
            // 
            // btnBGWorker
            // 
            this.btnBGWorker.Location = new System.Drawing.Point(253, 118);
            this.btnBGWorker.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnBGWorker.Name = "btnBGWorker";
            this.btnBGWorker.Size = new System.Drawing.Size(226, 53);
            this.btnBGWorker.TabIndex = 4;
            this.btnBGWorker.Text = "Background Worker";
            this.btnBGWorker.UseVisualStyleBackColor = true;
            this.btnBGWorker.Click += new System.EventHandler(this.btnBGWorker_Click);
            // 
            // btnTogglePreview
            // 
            this.btnTogglePreview.Location = new System.Drawing.Point(491, 51);
            this.btnTogglePreview.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnTogglePreview.Name = "btnTogglePreview";
            this.btnTogglePreview.Size = new System.Drawing.Size(226, 53);
            this.btnTogglePreview.TabIndex = 6;
            this.btnTogglePreview.Text = "Toggle Preview";
            this.btnTogglePreview.UseVisualStyleBackColor = true;
            this.btnTogglePreview.Click += new System.EventHandler(this.btnTogglePreview_Click);
            // 
            // btnCreateChild
            // 
            this.btnCreateChild.Location = new System.Drawing.Point(491, 118);
            this.btnCreateChild.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnCreateChild.Name = "btnCreateChild";
            this.btnCreateChild.Size = new System.Drawing.Size(226, 53);
            this.btnCreateChild.TabIndex = 7;
            this.btnCreateChild.Text = "Create Child";
            this.btnCreateChild.UseVisualStyleBackColor = true;
            this.btnCreateChild.Click += new System.EventHandler(this.btnCreateChild_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.administrativeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(744, 38);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.chooseOverlayIconToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 34);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(292, 34);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // chooseOverlayIconToolStripMenuItem
            // 
            this.chooseOverlayIconToolStripMenuItem.Name = "chooseOverlayIconToolStripMenuItem";
            this.chooseOverlayIconToolStripMenuItem.Size = new System.Drawing.Size(292, 34);
            this.chooseOverlayIconToolStripMenuItem.Text = "Choose Overlay Icon...";
            this.chooseOverlayIconToolStripMenuItem.Click += new System.EventHandler(this.chooseOverlayIconToolStripMenuItem_Click);
            // 
            // administrativeToolStripMenuItem
            // 
            this.administrativeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerFileExtensionsToolStripMenuItem,
            this.unregisterFileExtensionsToolStripMenuItem});
            this.administrativeToolStripMenuItem.Name = "administrativeToolStripMenuItem";
            this.administrativeToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.administrativeToolStripMenuItem.Text = "Administrative";
            // 
            // registerFileExtensionsToolStripMenuItem
            // 
            this.registerFileExtensionsToolStripMenuItem.Name = "registerFileExtensionsToolStripMenuItem";
            this.registerFileExtensionsToolStripMenuItem.Size = new System.Drawing.Size(322, 34);
            this.registerFileExtensionsToolStripMenuItem.Text = "Register File Extensions";
            this.registerFileExtensionsToolStripMenuItem.Click += new System.EventHandler(this.registerFileExtensionsToolStripMenuItem_Click);
            // 
            // unregisterFileExtensionsToolStripMenuItem
            // 
            this.unregisterFileExtensionsToolStripMenuItem.Name = "unregisterFileExtensionsToolStripMenuItem";
            this.unregisterFileExtensionsToolStripMenuItem.Size = new System.Drawing.Size(322, 34);
            this.unregisterFileExtensionsToolStripMenuItem.Text = "Unregister File Extensions";
            this.unregisterFileExtensionsToolStripMenuItem.Click += new System.EventHandler(this.unregisterFileExtensionsToolStripMenuItem_Click);
            // 
            // btnThumbClip
            // 
            this.btnThumbClip.Location = new System.Drawing.Point(491, 185);
            this.btnThumbClip.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnThumbClip.Name = "btnThumbClip";
            this.btnThumbClip.Size = new System.Drawing.Size(226, 53);
            this.btnThumbClip.TabIndex = 9;
            this.btnThumbClip.Text = "Thumbnail Clip";
            this.btnThumbClip.UseVisualStyleBackColor = true;
            this.btnThumbClip.Click += new System.EventHandler(this.btnThumbClip_Click);
            // 
            // Windows7Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(744, 288);
            this.Controls.Add(this.btnThumbClip);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnBuildJumpList);
            this.Controls.Add(this.btnToggleOverlay);
            this.Controls.Add(this.btnAddToRecent);
            this.Controls.Add(this.btnBGWorker);
            this.Controls.Add(this.btnProgressDemo);
            this.Controls.Add(this.btnCreateChild);
            this.Controls.Add(this.btnTogglePreview);
            this.Controls.Add(this.btnChangeAppId);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.MaximizeBox = false;
            this.Name = "Windows7Form";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Windows 7 Taskbar Showcase";
            this.Load += new System.EventHandler(this.Windows7Form_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChangeAppId;
        private System.Windows.Forms.Button btnProgressDemo;
        private System.Windows.Forms.Button btnAddToRecent;
        private System.Windows.Forms.Button btnToggleOverlay;
        private System.Windows.Forms.Button btnBuildJumpList;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnBGWorker;
        private System.Windows.Forms.Button btnTogglePreview;
        private System.Windows.Forms.Button btnCreateChild;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseOverlayIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem administrativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registerFileExtensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unregisterFileExtensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button btnThumbClip;
    }
}