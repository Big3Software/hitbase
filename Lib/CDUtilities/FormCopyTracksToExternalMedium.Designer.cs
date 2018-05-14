namespace Big3.Hitbase.CDUtilities
{
    partial class FormCopyTracksToExternalMedium
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
            Big3.Hitbase.Controls.ToggleColumnHeader toggleColumnHeader1 = new Big3.Hitbase.Controls.ToggleColumnHeader();
            Big3.Hitbase.Controls.ToggleColumnHeader toggleColumnHeader2 = new Big3.Hitbase.Controls.ToggleColumnHeader();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCopyTracksToExternalMedium));
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDrive = new System.Windows.Forms.ComboBox();
            this.buttonCopyAll = new System.Windows.Forms.Button();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.columnHeaderNr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.treeListViewDrive = new Big3.Hitbase.Controls.TreeListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.labelStatus = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.buttonCopySelected = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(363, 452);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "Schließen";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(86, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Gerät:";
            // 
            // comboBoxDrive
            // 
            this.comboBoxDrive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDrive.FormattingEnabled = true;
            this.comboBoxDrive.Location = new System.Drawing.Point(128, 222);
            this.comboBoxDrive.Name = "comboBoxDrive";
            this.comboBoxDrive.Size = new System.Drawing.Size(310, 21);
            this.comboBoxDrive.TabIndex = 3;
            this.comboBoxDrive.SelectedIndexChanged += new System.EventHandler(this.comboBoxDrive_SelectedIndexChanged);
            // 
            // buttonCopyAll
            // 
            this.buttonCopyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopyAll.Location = new System.Drawing.Point(287, 176);
            this.buttonCopyAll.Name = "buttonCopyAll";
            this.buttonCopyAll.Size = new System.Drawing.Size(151, 23);
            this.buttonCopyAll.TabIndex = 4;
            this.buttonCopyAll.Text = "Alle Musikstücke kopieren";
            this.buttonCopyAll.UseVisualStyleBackColor = true;
            this.buttonCopyAll.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // listViewFiles
            // 
            this.listViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderNr,
            this.columnHeaderFilename,
            this.columnHeaderSize});
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.Location = new System.Drawing.Point(21, 44);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(417, 126);
            this.listViewFiles.TabIndex = 6;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewFiles_ItemSelectionChanged);
            // 
            // columnHeaderNr
            // 
            this.columnHeaderNr.Text = "Nr.";
            this.columnHeaderNr.Width = 40;
            // 
            // columnHeaderFilename
            // 
            this.columnHeaderFilename.Text = "Datei";
            this.columnHeaderFilename.Width = 290;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Größe";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(18, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Aktuell vorhandene Dateien auf dem Gerät:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(18, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(257, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Folgende Musikstücke werden auf das Gerät kopiert:";
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox.Location = new System.Drawing.Point(21, 217);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(50, 50);
            this.pictureBox.TabIndex = 9;
            this.pictureBox.TabStop = false;
            // 
            // treeListViewDrive
            // 
            this.treeListViewDrive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeListViewDrive.BackColor = System.Drawing.SystemColors.Window;
            toggleColumnHeader1.Hovered = false;
            toggleColumnHeader1.Image = null;
            toggleColumnHeader1.Index = 0;
            toggleColumnHeader1.Pressed = false;
            toggleColumnHeader1.ScaleStyle = Big3.Hitbase.Controls.ColumnScaleStyle.Slide;
            toggleColumnHeader1.Selected = false;
            toggleColumnHeader1.Text = "Datei";
            toggleColumnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            toggleColumnHeader1.Visible = true;
            toggleColumnHeader1.Width = 300;
            toggleColumnHeader2.Hovered = false;
            toggleColumnHeader2.Image = null;
            toggleColumnHeader2.Index = 0;
            toggleColumnHeader2.Pressed = false;
            toggleColumnHeader2.ScaleStyle = Big3.Hitbase.Controls.ColumnScaleStyle.Slide;
            toggleColumnHeader2.Selected = false;
            toggleColumnHeader2.Text = "Größe";
            toggleColumnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            toggleColumnHeader2.Visible = true;
            this.treeListViewDrive.Columns.AddRange(new Big3.Hitbase.Controls.ToggleColumnHeader[] {
            toggleColumnHeader1,
            toggleColumnHeader2});
            this.treeListViewDrive.ColumnSortColor = System.Drawing.Color.Gainsboro;
            this.treeListViewDrive.ColumnTrackColor = System.Drawing.Color.WhiteSmoke;
            this.treeListViewDrive.GridLineColor = System.Drawing.Color.WhiteSmoke;
            this.treeListViewDrive.HeaderMenu = null;
            this.treeListViewDrive.ItemHeight = 20;
            this.treeListViewDrive.ItemMenu = null;
            this.treeListViewDrive.LabelEdit = false;
            this.treeListViewDrive.Location = new System.Drawing.Point(21, 286);
            this.treeListViewDrive.MultiSelect = true;
            this.treeListViewDrive.Name = "treeListViewDrive";
            this.treeListViewDrive.RowSelectColor = System.Drawing.SystemColors.Highlight;
            this.treeListViewDrive.RowTrackColor = System.Drawing.Color.WhiteSmoke;
            this.treeListViewDrive.ShowLines = true;
            this.treeListViewDrive.ShowRootLines = true;
            this.treeListViewDrive.Size = new System.Drawing.Size(417, 160);
            this.treeListViewDrive.SmallImageList = this.imageList;
            this.treeListViewDrive.StateImageList = null;
            this.treeListViewDrive.TabIndex = 10;
            this.treeListViewDrive.Text = "treeListView1";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "Folder.bmp");
            this.imageList.Images.SetKeyName(1, "MusicFile.bmp");
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(125, 246);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 11;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            // 
            // buttonCopySelected
            // 
            this.buttonCopySelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopySelected.Location = new System.Drawing.Point(89, 176);
            this.buttonCopySelected.Name = "buttonCopySelected";
            this.buttonCopySelected.Size = new System.Drawing.Size(192, 23);
            this.buttonCopySelected.TabIndex = 12;
            this.buttonCopySelected.Text = "Selektierte Musikstücke kopieren";
            this.buttonCopySelected.UseVisualStyleBackColor = true;
            this.buttonCopySelected.Click += new System.EventHandler(this.buttonCopySelected_Click);
            // 
            // FormCopyTracksToExternalMedium
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 487);
            this.Controls.Add(this.buttonCopySelected);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.treeListViewDrive);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listViewFiles);
            this.Controls.Add(this.buttonCopyAll);
            this.Controls.Add(this.comboBoxDrive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 400);
            this.Name = "FormCopyTracksToExternalMedium";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Auf Gerät kopieren";
            this.Load += new System.EventHandler(this.FormCopyTracksToExternalMedium_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDrive;
        private System.Windows.Forms.Button buttonCopyAll;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox;
        private Big3.Hitbase.Controls.TreeListView treeListViewDrive;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ColumnHeader columnHeaderNr;
        private System.Windows.Forms.ColumnHeader columnHeaderFilename;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button buttonCopySelected;
    }
}