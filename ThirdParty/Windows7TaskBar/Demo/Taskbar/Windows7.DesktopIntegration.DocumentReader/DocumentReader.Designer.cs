// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Windows7.DesktopIntegration.DocumentReader
{
    partial class DocumentReader
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
            this.rtbText = new System.Windows.Forms.RichTextBox();
            this.btnTogglePreview = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnToggleClip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbText
            // 
            this.rtbText.Location = new System.Drawing.Point(13, 34);
            this.rtbText.Name = "rtbText";
            this.rtbText.Size = new System.Drawing.Size(658, 378);
            this.rtbText.TabIndex = 0;
            this.rtbText.Text = "";
            // 
            // btnTogglePreview
            // 
            this.btnTogglePreview.Location = new System.Drawing.Point(13, 5);
            this.btnTogglePreview.Name = "btnTogglePreview";
            this.btnTogglePreview.Size = new System.Drawing.Size(110, 23);
            this.btnTogglePreview.TabIndex = 1;
            this.btnTogglePreview.Text = "Toggle Preview";
            this.btnTogglePreview.UseVisualStyleBackColor = true;
            this.btnTogglePreview.Click += new System.EventHandler(this.btnTogglePreview_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(596, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load...";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnToggleClip
            // 
            this.btnToggleClip.Location = new System.Drawing.Point(129, 5);
            this.btnToggleClip.Name = "btnToggleClip";
            this.btnToggleClip.Size = new System.Drawing.Size(110, 23);
            this.btnToggleClip.TabIndex = 3;
            this.btnToggleClip.Text = "Toggle Clip";
            this.btnToggleClip.UseVisualStyleBackColor = true;
            this.btnToggleClip.Click += new System.EventHandler(this.btnToggleClip_Click);
            // 
            // DocumentReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 424);
            this.Controls.Add(this.btnToggleClip);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnTogglePreview);
            this.Controls.Add(this.rtbText);
            this.Name = "DocumentReader";
            this.Text = "Simple Document Reader";
            this.Load += new System.EventHandler(this.DocumentReader_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbText;
        private System.Windows.Forms.Button btnTogglePreview;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnToggleClip;
    }
}