namespace Big3.Hitbase.Controls
{
    partial class ChoosePictureButton
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pastePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonChoosePicture = new System.Windows.Forms.Button();
            this.SearchInWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStripImage
            // 
            this.contextMenuStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePictureToolStripMenuItem,
            this.scanToolStripMenuItem,
            this.loadFromWebToolStripMenuItem,
            this.SearchInWebToolStripMenuItem,
            this.showPictureToolStripMenuItem,
            this.deletePictureToolStripMenuItem,
            this.toolStripMenuItem1,
            this.copyPictureToolStripMenuItem,
            this.pastePictureToolStripMenuItem});
            this.contextMenuStripImage.Name = "contextMenuStripImage";
            this.contextMenuStripImage.Size = new System.Drawing.Size(219, 186);
            this.contextMenuStripImage.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripImage_Opening);
            // 
            // changePictureToolStripMenuItem
            // 
            this.changePictureToolStripMenuItem.Name = "changePictureToolStripMenuItem";
            this.changePictureToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.changePictureToolStripMenuItem.Text = "&Bild auswählen...";
            this.changePictureToolStripMenuItem.Click += new System.EventHandler(this.changePictureToolStripMenuItem_Click);
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.scanToolStripMenuItem.Text = "Bild &scannen...";
            this.scanToolStripMenuItem.Click += new System.EventHandler(this.scanToolStripMenuItem_Click);
            // 
            // loadFromWebToolStripMenuItem
            // 
            this.loadFromWebToolStripMenuItem.Name = "loadFromWebToolStripMenuItem";
            this.loadFromWebToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.loadFromWebToolStripMenuItem.Text = "Bild aus dem &Internet laden";
            this.loadFromWebToolStripMenuItem.Click += new System.EventHandler(this.loadFromWebToolStripMenuItem_Click);
            // 
            // showPictureToolStripMenuItem
            // 
            this.showPictureToolStripMenuItem.Name = "showPictureToolStripMenuItem";
            this.showPictureToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.showPictureToolStripMenuItem.Text = "Bild &anzeigen";
            this.showPictureToolStripMenuItem.Click += new System.EventHandler(this.showPictureToolStripMenuItem_Click);
            // 
            // deletePictureToolStripMenuItem
            // 
            this.deletePictureToolStripMenuItem.Name = "deletePictureToolStripMenuItem";
            this.deletePictureToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.deletePictureToolStripMenuItem.Text = "Bild &entfernen";
            this.deletePictureToolStripMenuItem.Click += new System.EventHandler(this.deletePictureToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(215, 6);
            // 
            // copyPictureToolStripMenuItem
            // 
            this.copyPictureToolStripMenuItem.Image = global::Big3.Hitbase.Controls.Images.Copy;
            this.copyPictureToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyPictureToolStripMenuItem.Name = "copyPictureToolStripMenuItem";
            this.copyPictureToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.copyPictureToolStripMenuItem.Text = "&Kopieren";
            this.copyPictureToolStripMenuItem.Click += new System.EventHandler(this.copyPictureToolStripMenuItem_Click);
            // 
            // pastePictureToolStripMenuItem
            // 
            this.pastePictureToolStripMenuItem.Image = global::Big3.Hitbase.Controls.Images.Paste;
            this.pastePictureToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pastePictureToolStripMenuItem.Name = "pastePictureToolStripMenuItem";
            this.pastePictureToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.pastePictureToolStripMenuItem.Text = "Ein&fügen";
            this.pastePictureToolStripMenuItem.Click += new System.EventHandler(this.pastePictureToolStripMenuItem_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.ContextMenuStrip = this.contextMenuStripImage;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(227, 205);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.Visible = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // buttonChoosePicture
            // 
            this.buttonChoosePicture.BackgroundImage = global::Big3.Hitbase.Controls.Images.CDCover;
            this.buttonChoosePicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonChoosePicture.ContextMenuStrip = this.contextMenuStripImage;
            this.buttonChoosePicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonChoosePicture.Location = new System.Drawing.Point(0, 0);
            this.buttonChoosePicture.Name = "buttonChoosePicture";
            this.buttonChoosePicture.Size = new System.Drawing.Size(227, 205);
            this.buttonChoosePicture.TabIndex = 0;
            this.buttonChoosePicture.Text = "Klicken Sie hier, um ein Bild hinzuzufügen.";
            this.buttonChoosePicture.UseVisualStyleBackColor = true;
            this.buttonChoosePicture.Click += new System.EventHandler(this.buttonChoosePicture_Click);
            // 
            // SearchInWebToolStripMenuItem
            // 
            this.SearchInWebToolStripMenuItem.Name = "SearchInWebToolStripMenuItem";
            this.SearchInWebToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.SearchInWebToolStripMenuItem.Text = "Bild im &Internet suchen";
            this.SearchInWebToolStripMenuItem.Click += new System.EventHandler(this.SearchInWebToolStripMenuItem_Click);
            // 
            // ChoosePictureButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.buttonChoosePicture);
            this.Name = "ChoosePictureButton";
            this.Size = new System.Drawing.Size(227, 205);
            this.Load += new System.EventHandler(this.ChoosePictureButton_Load);
            this.contextMenuStripImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonChoosePicture;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripImage;
        private System.Windows.Forms.ToolStripMenuItem changePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromWebToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pastePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SearchInWebToolStripMenuItem;
    }
}
