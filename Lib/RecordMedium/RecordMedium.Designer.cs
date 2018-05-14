namespace Big3.Hitbase.RecordMedium
{
    partial class RecordMedium
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordMedium));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonAbbrechen = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textRecordFormatFile = new System.Windows.Forms.TextBox();
            this.listTemplates = new System.Windows.Forms.ListBox();
            this.buttonCustomFileFormat = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.buttonSelectFileDir = new System.Windows.Forms.Button();
            this.textFileDir = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "flac-org.png");
            this.imageList1.Images.SetKeyName(1, "Lame-org.png");
            this.imageList1.Images.SetKeyName(2, "OGG-Org.png");
            this.imageList1.Images.SetKeyName(3, "WAV.png");
            this.imageList1.Images.SetKeyName(4, "msdos-Org.png");
            this.imageList1.Images.SetKeyName(5, "flac-rot.png");
            this.imageList1.Images.SetKeyName(6, "Lame-rot.png");
            this.imageList1.Images.SetKeyName(7, "OGG-rot.png");
            this.imageList1.Images.SetKeyName(8, "msdos-rot.png");
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.buttonAbbrechen);
            this.panel3.Controls.Add(this.buttonOK);
            this.panel3.Location = new System.Drawing.Point(13, 402);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(557, 30);
            this.panel3.TabIndex = 1;
            // 
            // buttonAbbrechen
            // 
            this.buttonAbbrechen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAbbrechen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAbbrechen.Location = new System.Drawing.Point(480, 3);
            this.buttonAbbrechen.Name = "buttonAbbrechen";
            this.buttonAbbrechen.Size = new System.Drawing.Size(74, 23);
            this.buttonAbbrechen.TabIndex = 1;
            this.buttonAbbrechen.Text = "A&bbrechen";
            this.buttonAbbrechen.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(400, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(74, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.BackColor = System.Drawing.Color.Transparent;
            this.groupBox8.Controls.Add(this.button1);
            this.groupBox8.Controls.Add(this.label8);
            this.groupBox8.Controls.Add(this.textRecordFormatFile);
            this.groupBox8.Controls.Add(this.listTemplates);
            this.groupBox8.Controls.Add(this.buttonCustomFileFormat);
            this.groupBox8.Location = new System.Drawing.Point(12, 112);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(558, 284);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Dateinamen";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(238, 255);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "&Zeichen ersetzen...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(252, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Aktuelles Format für die Erzeugung der Dateinamen:\r\n";
            // 
            // textRecordFormatFile
            // 
            this.textRecordFormatFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textRecordFormatFile.Location = new System.Drawing.Point(6, 184);
            this.textRecordFormatFile.Multiline = true;
            this.textRecordFormatFile.Name = "textRecordFormatFile";
            this.textRecordFormatFile.Size = new System.Drawing.Size(546, 65);
            this.textRecordFormatFile.TabIndex = 8;
            // 
            // listTemplates
            // 
            this.listTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listTemplates.FormattingEnabled = true;
            this.listTemplates.Location = new System.Drawing.Point(6, 19);
            this.listTemplates.Name = "listTemplates";
            this.listTemplates.Size = new System.Drawing.Size(546, 134);
            this.listTemplates.TabIndex = 7;
            this.listTemplates.SelectedIndexChanged += new System.EventHandler(this.listTemplates_SelectedIndexChanged);
            // 
            // buttonCustomFileFormat
            // 
            this.buttonCustomFileFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCustomFileFormat.Location = new System.Drawing.Point(6, 255);
            this.buttonCustomFileFormat.Name = "buttonCustomFileFormat";
            this.buttonCustomFileFormat.Size = new System.Drawing.Size(226, 23);
            this.buttonCustomFileFormat.TabIndex = 4;
            this.buttonCustomFileFormat.Text = "&Erweiterte Einstellungen für Dateinamen...";
            this.buttonCustomFileFormat.UseVisualStyleBackColor = true;
            this.buttonCustomFileFormat.Click += new System.EventHandler(this.buttonCustomFileFormat_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.BackColor = System.Drawing.Color.Transparent;
            this.groupBox7.Controls.Add(this.buttonSelectFileDir);
            this.groupBox7.Controls.Add(this.textFileDir);
            this.groupBox7.Controls.Add(this.label7);
            this.groupBox7.Controls.Add(this.label6);
            this.groupBox7.Location = new System.Drawing.Point(12, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(558, 94);
            this.groupBox7.TabIndex = 4;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Ausgabeverzeichnis";
            // 
            // buttonSelectFileDir
            // 
            this.buttonSelectFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectFileDir.Location = new System.Drawing.Point(527, 56);
            this.buttonSelectFileDir.Name = "buttonSelectFileDir";
            this.buttonSelectFileDir.Size = new System.Drawing.Size(25, 23);
            this.buttonSelectFileDir.TabIndex = 3;
            this.buttonSelectFileDir.Text = "...";
            this.buttonSelectFileDir.UseVisualStyleBackColor = true;
            this.buttonSelectFileDir.Click += new System.EventHandler(this.buttonSelectFileDir_Click);
            // 
            // textFileDir
            // 
            this.textFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textFileDir.Location = new System.Drawing.Point(79, 58);
            this.textFileDir.Name = "textFileDir";
            this.textFileDir.Size = new System.Drawing.Size(442, 20);
            this.textFileDir.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Verzeichnis:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(326, 26);
            this.label6.TabIndex = 0;
            this.label6.Text = "Geben Sie an, in welches Verzeichnis die Musikdateien gespeichert\r\nwerden sollen:" +
    "";
            // 
            // RecordMedium
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Big3.Hitbase.RecordMedium.Images.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonAbbrechen;
            this.ClientSize = new System.Drawing.Size(584, 444);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.panel3);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 482);
            this.Name = "RecordMedium";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Aufnahme";
            this.Load += new System.EventHandler(this.RecordMedium_Load);
            this.panel3.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonAbbrechen;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textRecordFormatFile;
        private System.Windows.Forms.ListBox listTemplates;
        private System.Windows.Forms.Button buttonCustomFileFormat;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button buttonSelectFileDir;
        public System.Windows.Forms.TextBox textFileDir;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;

    }
}

