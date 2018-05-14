namespace Big3.Hitbase.CatalogView
{
    partial class FormPrintCatalog
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
            this.printPreviewControl = new System.Windows.Forms.PrintPreviewControl();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonCurrentCD = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButtonTrackList = new System.Windows.Forms.RadioButton();
            this.radioButtonDetailListWithTracks = new System.Windows.Forms.RadioButton();
            this.radioButtonDetailList = new System.Windows.Forms.RadioButton();
            this.radioButtonCDList = new System.Windows.Forms.RadioButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelPage = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxZoom = new System.Windows.Forms.ToolStripComboBox();
            this.buttonSelectFields = new System.Windows.Forms.Button();
            this.buttonSort = new System.Windows.Forms.Button();
            this.buttonFont = new System.Windows.Forms.Button();
            this.checkBoxDetailPrintEmptyFields = new System.Windows.Forms.CheckBox();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonCDCover = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxHeaderPrintDate = new System.Windows.Forms.CheckBox();
            this.checkBoxHeaderPrintFilter = new System.Windows.Forms.CheckBox();
            this.checkBoxHeaderPrintSort = new System.Windows.Forms.CheckBox();
            this.textBoxHeaderText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPageUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPageDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOnePage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTwoPages = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFourPages = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrintSetup = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // printPreviewControl
            // 
            this.printPreviewControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.printPreviewControl.Location = new System.Drawing.Point(12, 40);
            this.printPreviewControl.Name = "printPreviewControl";
            this.printPreviewControl.Size = new System.Drawing.Size(522, 512);
            this.printPreviewControl.TabIndex = 0;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(719, 529);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(93, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Schließen";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButtonCurrentCD);
            this.groupBox1.Controls.Add(this.radioButton5);
            this.groupBox1.Controls.Add(this.radioButtonTrackList);
            this.groupBox1.Controls.Add(this.radioButtonDetailListWithTracks);
            this.groupBox1.Controls.Add(this.radioButtonDetailList);
            this.groupBox1.Controls.Add(this.radioButtonCDList);
            this.groupBox1.Location = new System.Drawing.Point(540, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 173);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Druckart";
            // 
            // radioButtonCurrentCD
            // 
            this.radioButtonCurrentCD.AutoSize = true;
            this.radioButtonCurrentCD.Location = new System.Drawing.Point(15, 139);
            this.radioButtonCurrentCD.Name = "radioButtonCurrentCD";
            this.radioButtonCurrentCD.Size = new System.Drawing.Size(127, 17);
            this.radioButtonCurrentCD.TabIndex = 5;
            this.radioButtonCurrentCD.TabStop = true;
            this.radioButtonCurrentCD.Text = "Aktuell eingelegte CD";
            this.radioButtonCurrentCD.UseVisualStyleBackColor = true;
            this.radioButtonCurrentCD.CheckedChanged += new System.EventHandler(this.radioButtonCurrentCD_CheckedChanged);
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(15, 116);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(160, 17);
            this.radioButton5.TabIndex = 4;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "Liste der ausgeliehenen CDs";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButtonTrackList
            // 
            this.radioButtonTrackList.AutoSize = true;
            this.radioButtonTrackList.Location = new System.Drawing.Point(15, 93);
            this.radioButtonTrackList.Name = "radioButtonTrackList";
            this.radioButtonTrackList.Size = new System.Drawing.Size(78, 17);
            this.radioButtonTrackList.TabIndex = 3;
            this.radioButtonTrackList.TabStop = true;
            this.radioButtonTrackList.Text = "Track-Liste";
            this.radioButtonTrackList.UseVisualStyleBackColor = true;
            this.radioButtonTrackList.CheckedChanged += new System.EventHandler(this.radioButtonTrackList_CheckedChanged);
            // 
            // radioButtonDetailListWithTracks
            // 
            this.radioButtonDetailListWithTracks.AutoSize = true;
            this.radioButtonDetailListWithTracks.Location = new System.Drawing.Point(15, 70);
            this.radioButtonDetailListWithTracks.Name = "radioButtonDetailListWithTracks";
            this.radioButtonDetailListWithTracks.Size = new System.Drawing.Size(222, 17);
            this.radioButtonDetailListWithTracks.TabIndex = 2;
            this.radioButtonDetailListWithTracks.TabStop = true;
            this.radioButtonDetailListWithTracks.Text = "Lange CD-Liste (detailliert) mit Trackdaten";
            this.radioButtonDetailListWithTracks.UseVisualStyleBackColor = true;
            this.radioButtonDetailListWithTracks.CheckedChanged += new System.EventHandler(this.radioButtonDetailListWithTracks_CheckedChanged);
            // 
            // radioButtonDetailList
            // 
            this.radioButtonDetailList.AutoSize = true;
            this.radioButtonDetailList.Location = new System.Drawing.Point(15, 47);
            this.radioButtonDetailList.Name = "radioButtonDetailList";
            this.radioButtonDetailList.Size = new System.Drawing.Size(233, 17);
            this.radioButtonDetailList.TabIndex = 1;
            this.radioButtonDetailList.TabStop = true;
            this.radioButtonDetailList.Text = "Lange CD-Liste (detailliert) ohne Trackdaten";
            this.radioButtonDetailList.UseVisualStyleBackColor = true;
            this.radioButtonDetailList.CheckedChanged += new System.EventHandler(this.radioButtonDetailList_CheckedChanged);
            // 
            // radioButtonCDList
            // 
            this.radioButtonCDList.AutoSize = true;
            this.radioButtonCDList.Location = new System.Drawing.Point(15, 24);
            this.radioButtonCDList.Name = "radioButtonCDList";
            this.radioButtonCDList.Size = new System.Drawing.Size(193, 17);
            this.radioButtonCDList.TabIndex = 0;
            this.radioButtonCDList.TabStop = true;
            this.radioButtonCDList.Text = "Kurze CD-Liste (Zusammenfassung)";
            this.radioButtonCDList.UseVisualStyleBackColor = true;
            this.radioButtonCDList.CheckedChanged += new System.EventHandler(this.radioButtonCDList_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrint,
            this.toolStripButtonPrintSetup,
            this.toolStripSeparator3,
            this.toolStripButtonPageUp,
            this.toolStripButtonPageDown,
            this.toolStripLabelPage,
            this.toolStripSeparator1,
            this.toolStripButtonOnePage,
            this.toolStripButtonTwoPages,
            this.toolStripButtonFourPages,
            this.toolStripSeparator2,
            this.toolStripComboBoxZoom});
            this.toolStrip1.Location = new System.Drawing.Point(12, 12);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(522, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelPage
            // 
            this.toolStripLabelPage.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelPage.Name = "toolStripLabelPage";
            this.toolStripLabelPage.Size = new System.Drawing.Size(123, 22);
            this.toolStripLabelPage.Text = "Seite wird berechnet...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripComboBoxZoom
            // 
            this.toolStripComboBoxZoom.Name = "toolStripComboBoxZoom";
            this.toolStripComboBoxZoom.Size = new System.Drawing.Size(121, 25);
            this.toolStripComboBoxZoom.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxZoom_SelectedIndexChanged);
            // 
            // buttonSelectFields
            // 
            this.buttonSelectFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectFields.Location = new System.Drawing.Point(15, 28);
            this.buttonSelectFields.Name = "buttonSelectFields";
            this.buttonSelectFields.Size = new System.Drawing.Size(119, 23);
            this.buttonSelectFields.TabIndex = 4;
            this.buttonSelectFields.Text = "Felder auswählen...";
            this.buttonSelectFields.UseVisualStyleBackColor = true;
            this.buttonSelectFields.Click += new System.EventHandler(this.buttonSelectFields_Click);
            // 
            // buttonSort
            // 
            this.buttonSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSort.Location = new System.Drawing.Point(15, 57);
            this.buttonSort.Name = "buttonSort";
            this.buttonSort.Size = new System.Drawing.Size(119, 23);
            this.buttonSort.TabIndex = 5;
            this.buttonSort.Text = "Sortieren...";
            this.buttonSort.UseVisualStyleBackColor = true;
            this.buttonSort.Click += new System.EventHandler(this.buttonSort_Click);
            // 
            // buttonFont
            // 
            this.buttonFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFont.Location = new System.Drawing.Point(140, 57);
            this.buttonFont.Name = "buttonFont";
            this.buttonFont.Size = new System.Drawing.Size(117, 23);
            this.buttonFont.TabIndex = 6;
            this.buttonFont.Text = "Schriftart...";
            this.buttonFont.UseVisualStyleBackColor = true;
            this.buttonFont.Click += new System.EventHandler(this.buttonFont_Click);
            // 
            // checkBoxDetailPrintEmptyFields
            // 
            this.checkBoxDetailPrintEmptyFields.AutoSize = true;
            this.checkBoxDetailPrintEmptyFields.Location = new System.Drawing.Point(15, 145);
            this.checkBoxDetailPrintEmptyFields.Name = "checkBoxDetailPrintEmptyFields";
            this.checkBoxDetailPrintEmptyFields.Size = new System.Drawing.Size(145, 17);
            this.checkBoxDetailPrintEmptyFields.TabIndex = 7;
            this.checkBoxDetailPrintEmptyFields.Text = "Leere Textfelder drucken";
            this.checkBoxDetailPrintEmptyFields.UseVisualStyleBackColor = true;
            // 
            // buttonFilter
            // 
            this.buttonFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFilter.Location = new System.Drawing.Point(140, 28);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(117, 23);
            this.buttonFilter.TabIndex = 8;
            this.buttonFilter.Text = "Filter...";
            this.buttonFilter.UseVisualStyleBackColor = true;
            this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonCDCover);
            this.groupBox2.Controls.Add(this.checkBoxDetailPrintEmptyFields);
            this.groupBox2.Controls.Add(this.buttonSort);
            this.groupBox2.Controls.Add(this.buttonFilter);
            this.groupBox2.Controls.Add(this.buttonSelectFields);
            this.groupBox2.Controls.Add(this.buttonFont);
            this.groupBox2.Location = new System.Drawing.Point(540, 191);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 176);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Druckoptionen";
            // 
            // buttonCDCover
            // 
            this.buttonCDCover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCDCover.Location = new System.Drawing.Point(15, 86);
            this.buttonCDCover.Name = "buttonCDCover";
            this.buttonCDCover.Size = new System.Drawing.Size(119, 23);
            this.buttonCDCover.TabIndex = 9;
            this.buttonCDCover.Text = "CD-Cover...";
            this.buttonCDCover.UseVisualStyleBackColor = true;
            this.buttonCDCover.Click += new System.EventHandler(this.buttonCDCover_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.checkBoxHeaderPrintDate);
            this.groupBox3.Controls.Add(this.checkBoxHeaderPrintFilter);
            this.groupBox3.Controls.Add(this.checkBoxHeaderPrintSort);
            this.groupBox3.Controls.Add(this.textBoxHeaderText);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(540, 373);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(272, 89);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Kopfzeilen";
            // 
            // checkBoxHeaderPrintDate
            // 
            this.checkBoxHeaderPrintDate.AutoSize = true;
            this.checkBoxHeaderPrintDate.Location = new System.Drawing.Point(118, 54);
            this.checkBoxHeaderPrintDate.Name = "checkBoxHeaderPrintDate";
            this.checkBoxHeaderPrintDate.Size = new System.Drawing.Size(57, 17);
            this.checkBoxHeaderPrintDate.TabIndex = 4;
            this.checkBoxHeaderPrintDate.Text = "Datum";
            this.checkBoxHeaderPrintDate.UseVisualStyleBackColor = true;
            this.checkBoxHeaderPrintDate.CheckedChanged += new System.EventHandler(this.checkBoxHeaderPrintDate_CheckedChanged);
            // 
            // checkBoxHeaderPrintFilter
            // 
            this.checkBoxHeaderPrintFilter.AutoSize = true;
            this.checkBoxHeaderPrintFilter.Location = new System.Drawing.Point(209, 54);
            this.checkBoxHeaderPrintFilter.Name = "checkBoxHeaderPrintFilter";
            this.checkBoxHeaderPrintFilter.Size = new System.Drawing.Size(48, 17);
            this.checkBoxHeaderPrintFilter.TabIndex = 3;
            this.checkBoxHeaderPrintFilter.Text = "Filter";
            this.checkBoxHeaderPrintFilter.UseVisualStyleBackColor = true;
            this.checkBoxHeaderPrintFilter.CheckedChanged += new System.EventHandler(this.checkBoxHeaderPrintFilter_CheckedChanged);
            // 
            // checkBoxHeaderPrintSort
            // 
            this.checkBoxHeaderPrintSort.AutoSize = true;
            this.checkBoxHeaderPrintSort.Location = new System.Drawing.Point(15, 54);
            this.checkBoxHeaderPrintSort.Name = "checkBoxHeaderPrintSort";
            this.checkBoxHeaderPrintSort.Size = new System.Drawing.Size(74, 17);
            this.checkBoxHeaderPrintSort.TabIndex = 2;
            this.checkBoxHeaderPrintSort.Text = "Sortierung";
            this.checkBoxHeaderPrintSort.UseVisualStyleBackColor = true;
            this.checkBoxHeaderPrintSort.CheckedChanged += new System.EventHandler(this.checkBoxHeaderPrintSort_CheckedChanged);
            // 
            // textBoxHeaderText
            // 
            this.textBoxHeaderText.Location = new System.Drawing.Point(79, 26);
            this.textBoxHeaderText.Name = "textBoxHeaderText";
            this.textBoxHeaderText.Size = new System.Drawing.Size(178, 20);
            this.textBoxHeaderText.TabIndex = 1;
            this.textBoxHeaderText.Leave += new System.EventHandler(this.textBoxHeaderText_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Überschrift:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(540, 487);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(272, 39);
            this.label2.TabIndex = 11;
            this.label2.Text = "Hinweis: In der Vorschau werden maximal die ersten 10 Seiten angezeigt.";
            // 
            // toolStripButtonPrint
            // 
            this.toolStripButtonPrint.Image = global::Big3.Hitbase.CatalogView.Images.Print;
            this.toolStripButtonPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrint.Name = "toolStripButtonPrint";
            this.toolStripButtonPrint.Size = new System.Drawing.Size(71, 22);
            this.toolStripButtonPrint.Text = "Drucken";
            this.toolStripButtonPrint.Click += new System.EventHandler(this.toolStripButtonPrint_Click);
            // 
            // toolStripButtonPageUp
            // 
            this.toolStripButtonPageUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPageUp.Image = global::Big3.Hitbase.CatalogView.Images.PageUp;
            this.toolStripButtonPageUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPageUp.Name = "toolStripButtonPageUp";
            this.toolStripButtonPageUp.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPageUp.Text = "Vorherige Seite";
            this.toolStripButtonPageUp.Click += new System.EventHandler(this.toolStripButtonPageUp_Click);
            // 
            // toolStripButtonPageDown
            // 
            this.toolStripButtonPageDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPageDown.Image = global::Big3.Hitbase.CatalogView.Images.PageDown;
            this.toolStripButtonPageDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPageDown.Name = "toolStripButtonPageDown";
            this.toolStripButtonPageDown.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPageDown.Text = "Nächste Seite";
            this.toolStripButtonPageDown.Click += new System.EventHandler(this.toolStripButtonPageDown_Click);
            // 
            // toolStripButtonOnePage
            // 
            this.toolStripButtonOnePage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOnePage.Image = global::Big3.Hitbase.CatalogView.Images.OnePage;
            this.toolStripButtonOnePage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOnePage.Name = "toolStripButtonOnePage";
            this.toolStripButtonOnePage.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOnePage.Text = "Eine Seite";
            this.toolStripButtonOnePage.Click += new System.EventHandler(this.toolStripButtonOnePage_Click);
            // 
            // toolStripButtonTwoPages
            // 
            this.toolStripButtonTwoPages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTwoPages.Image = global::Big3.Hitbase.CatalogView.Images.TwoPages;
            this.toolStripButtonTwoPages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTwoPages.Name = "toolStripButtonTwoPages";
            this.toolStripButtonTwoPages.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTwoPages.Text = "Zwei Seiten";
            this.toolStripButtonTwoPages.Click += new System.EventHandler(this.toolStripButtonTwoPages_Click);
            // 
            // toolStripButtonFourPages
            // 
            this.toolStripButtonFourPages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFourPages.Image = global::Big3.Hitbase.CatalogView.Images.FourPages;
            this.toolStripButtonFourPages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFourPages.Name = "toolStripButtonFourPages";
            this.toolStripButtonFourPages.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFourPages.Text = "Vier Seiten";
            this.toolStripButtonFourPages.Click += new System.EventHandler(this.toolStripButtonFourPages_Click);
            // 
            // toolStripButtonPrintSetup
            // 
            this.toolStripButtonPrintSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrintSetup.Image = global::Big3.Hitbase.CatalogView.Images.PrintSetup;
            this.toolStripButtonPrintSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrintSetup.Name = "toolStripButtonPrintSetup";
            this.toolStripButtonPrintSetup.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrintSetup.Text = "Druckereinrichtung";
            this.toolStripButtonPrintSetup.Click += new System.EventHandler(this.toolStripButtonPrintSetup_Click);
            // 
            // FormPrintCatalog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(824, 564);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.printPreviewControl);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FormPrintCatalog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Drucken";
            this.Load += new System.EventHandler(this.FormPrintCatalog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrintCatalog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PrintPreviewControl printPreviewControl;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButtonTrackList;
        private System.Windows.Forms.RadioButton radioButtonDetailListWithTracks;
        private System.Windows.Forms.RadioButton radioButtonDetailList;
        private System.Windows.Forms.RadioButton radioButtonCDList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrint;
        private System.Windows.Forms.ToolStripButton toolStripButtonPageUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonPageDown;
        private System.Windows.Forms.ToolStripLabel toolStripLabelPage;
        private System.Windows.Forms.ToolStripButton toolStripButtonOnePage;
        private System.Windows.Forms.ToolStripButton toolStripButtonFourPages;
        private System.Windows.Forms.ToolStripButton toolStripButtonTwoPages;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxZoom;
        private System.Windows.Forms.Button buttonSelectFields;
        private System.Windows.Forms.Button buttonSort;
        private System.Windows.Forms.Button buttonFont;
        private System.Windows.Forms.CheckBox checkBoxDetailPrintEmptyFields;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxHeaderPrintFilter;
        private System.Windows.Forms.CheckBox checkBoxHeaderPrintSort;
        private System.Windows.Forms.TextBox textBoxHeaderText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxHeaderPrintDate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonCurrentCD;
        private System.Windows.Forms.Button buttonCDCover;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintSetup;
    }
}