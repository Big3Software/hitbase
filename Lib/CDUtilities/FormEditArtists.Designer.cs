namespace Big3.Hitbase.CDUtilities
{
    partial class FormEditArtists
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
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder1 = new XPTable.Models.DataSourceColumnBinder();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.personGroupTable = new XPTable.Models.Table();
            this.personGroupListBox = new Big3.Hitbase.CDUtilities.ArtistListBoxWithImage(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonDetails = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTable = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPrintPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPageSetup = new System.Windows.Forms.ToolStripButton();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCheckAll = new System.Windows.Forms.Button();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.personGroupTable)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.buttonClose);
            this.groupBox1.Controls.Add(this.buttonCheckAll);
            this.groupBox1.Controls.Add(this.buttonCheck);
            this.groupBox1.Controls.Add(this.buttonDelete);
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.buttonNew);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 450);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Vorhandene Personen/Gruppen:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.personGroupTable);
            this.panel1.Controls.Add(this.personGroupListBox);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Location = new System.Drawing.Point(15, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(577, 397);
            this.panel1.TabIndex = 7;
            // 
            // personGroupTable
            // 
            this.personGroupTable.BorderColor = System.Drawing.Color.Black;
            this.personGroupTable.DataMember = null;
            this.personGroupTable.DataSourceColumnBinder = dataSourceColumnBinder1;
            this.personGroupTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.personGroupTable.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.personGroupTable.Location = new System.Drawing.Point(0, 25);
            this.personGroupTable.Name = "personGroupTable";
            this.personGroupTable.Size = new System.Drawing.Size(577, 372);
            this.personGroupTable.TabIndex = 6;
            this.personGroupTable.Text = "table1";
            this.personGroupTable.UnfocusedBorderColor = System.Drawing.Color.Black;
            this.personGroupTable.Visible = false;
            this.personGroupTable.CellPropertyChanged += new XPTable.Events.CellEventHandler(this.artistTable_CellPropertyChanged);
            this.personGroupTable.BeginEditing += new XPTable.Events.CellEditEventHandler(this.artistTable_BeginEditing);
            this.personGroupTable.EditingStopped += new XPTable.Events.CellEditEventHandler(this.artistTable_EditingStopped);
            this.personGroupTable.EditingCancelled += new XPTable.Events.CellEditEventHandler(this.artistTable_EditingCancelled);
            this.personGroupTable.SelectionChanged += new XPTable.Events.SelectionEventHandler(this.artistTable_SelectionChanged);
            this.personGroupTable.DoubleClick += new System.EventHandler(this.artistTable_DoubleClick);
            // 
            // personGroupListBox
            // 
            this.personGroupListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.personGroupListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.personGroupListBox.FormattingEnabled = true;
            this.personGroupListBox.IntegralHeight = false;
            this.personGroupListBox.ItemHeight = 80;
            this.personGroupListBox.Location = new System.Drawing.Point(0, 25);
            this.personGroupListBox.Name = "personGroupListBox";
            this.personGroupListBox.Size = new System.Drawing.Size(577, 372);
            this.personGroupListBox.TabIndex = 5;
            this.personGroupListBox.SelectedIndexChanged += new System.EventHandler(this.artistListBox_SelectedIndexChanged);
            this.personGroupListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.personGroupListBox_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonDetails,
            this.toolStripButtonTable,
            this.toolStripLabelCount,
            this.toolStripSeparator1,
            this.toolStripButtonRefresh,
            this.toolStripSeparator2,
            this.toolStripButtonPrintPreview,
            this.toolStripButtonPrint,
            this.toolStripButtonPageSetup});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(577, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonDetails
            // 
            this.toolStripButtonDetails.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDetails.Image = global::Big3.Hitbase.CDUtilities.Images.ArtistThumbView;
            this.toolStripButtonDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDetails.Name = "toolStripButtonDetails";
            this.toolStripButtonDetails.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDetails.Text = "Detailansicht mit Bild";
            this.toolStripButtonDetails.Click += new System.EventHandler(this.toolStripButtonDetails_Click);
            // 
            // toolStripButtonTable
            // 
            this.toolStripButtonTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTable.Image = global::Big3.Hitbase.CDUtilities.Images.ArtistTableView;
            this.toolStripButtonTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTable.Name = "toolStripButtonTable";
            this.toolStripButtonTable.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTable.Text = "Tabellenansicht";
            this.toolStripButtonTable.Click += new System.EventHandler(this.toolStripButtonTable_Click);
            // 
            // toolStripLabelCount
            // 
            this.toolStripLabelCount.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelCount.Name = "toolStripLabelCount";
            this.toolStripLabelCount.Size = new System.Drawing.Size(123, 22);
            this.toolStripLabelCount.Text = "Person(en)/Gruppe(n)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = global::Big3.Hitbase.CDUtilities.Images.Refresh;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "Aktualisieren";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPrintPreview
            // 
            this.toolStripButtonPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrintPreview.Image = global::Big3.Hitbase.CDUtilities.Images.PrintPreview;
            this.toolStripButtonPrintPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrintPreview.Name = "toolStripButtonPrintPreview";
            this.toolStripButtonPrintPreview.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrintPreview.Text = "Seitenansicht";
            this.toolStripButtonPrintPreview.Click += new System.EventHandler(this.toolStripButtonPrintPreview_Click);
            // 
            // toolStripButtonPrint
            // 
            this.toolStripButtonPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrint.Image = global::Big3.Hitbase.CDUtilities.Images.Print;
            this.toolStripButtonPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrint.Name = "toolStripButtonPrint";
            this.toolStripButtonPrint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrint.Text = "Drucken";
            this.toolStripButtonPrint.Click += new System.EventHandler(this.toolStripButtonPrint_Click);
            // 
            // toolStripButtonPageSetup
            // 
            this.toolStripButtonPageSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPageSetup.Image = global::Big3.Hitbase.CDUtilities.Images.PageSetup;
            this.toolStripButtonPageSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPageSetup.Name = "toolStripButtonPageSetup";
            this.toolStripButtonPageSetup.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPageSetup.Text = "Seite einrichten";
            this.toolStripButtonPageSetup.Click += new System.EventHandler(this.toolStripButtonPageSetup_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(598, 403);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(89, 23);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Schließen";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonCheckAll
            // 
            this.buttonCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheckAll.Location = new System.Drawing.Point(598, 145);
            this.buttonCheckAll.Name = "buttonCheckAll";
            this.buttonCheckAll.Size = new System.Drawing.Size(89, 23);
            this.buttonCheckAll.TabIndex = 4;
            this.buttonCheckAll.Text = "Alle prüfen";
            this.buttonCheckAll.UseVisualStyleBackColor = true;
            this.buttonCheckAll.Click += new System.EventHandler(this.buttonCheckAll_Click);
            // 
            // buttonCheck
            // 
            this.buttonCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheck.Location = new System.Drawing.Point(598, 116);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(89, 23);
            this.buttonCheck.TabIndex = 3;
            this.buttonCheck.Text = "Prüfen";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Location = new System.Drawing.Point(598, 87);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(89, 23);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Löschen";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEdit.Location = new System.Drawing.Point(598, 58);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(89, 23);
            this.buttonEdit.TabIndex = 1;
            this.buttonEdit.Text = "Ändern...";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNew.Location = new System.Drawing.Point(598, 29);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(89, 23);
            this.buttonNew.TabIndex = 0;
            this.buttonNew.Text = "Neu...";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // FormEditArtists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 474);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FormEditArtists";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Personen/Gruppen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditArtists_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormEditArtists_FormClosed);
            this.Load += new System.EventHandler(this.FormEditArtists_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.personGroupTable)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCheckAll;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonNew;
        private ArtistListBoxWithImage personGroupListBox;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonDetails;
        private System.Windows.Forms.ToolStripButton toolStripButtonTable;
        private XPTable.Models.Table personGroupTable;
        private System.Windows.Forms.ToolStripLabel toolStripLabelCount;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrint;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintPreview;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonPageSetup;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}