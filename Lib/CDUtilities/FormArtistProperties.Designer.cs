namespace Big3.Hitbase.CDUtilities
{
    partial class FormArtistProperties
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
            this.contextMenuStripImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.choosePictureButton = new Big3.Hitbase.Controls.ChoosePictureButton();
            this.textBoxDayOfDeath = new System.Windows.Forms.TextBox();
            this.textBoxBirthday = new System.Windows.Forms.TextBox();
            this.labelBirthday = new System.Windows.Forms.Label();
            this.labelDeath = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonGoToHomepage = new System.Windows.Forms.Button();
            this.textBoxHomepage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxCountry = new System.Windows.Forms.ComboBox();
            this.comboBoxSex = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxArtistType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSaveAs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.contextMenuStripImage.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripImage
            // 
            this.contextMenuStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePictureToolStripMenuItem,
            this.deletePictureToolStripMenuItem,
            this.toolStripMenuItem1,
            this.copyPictureToolStripMenuItem});
            this.contextMenuStripImage.Name = "contextMenuStripImage";
            this.contextMenuStripImage.Size = new System.Drawing.Size(149, 76);
            // 
            // changePictureToolStripMenuItem
            // 
            this.changePictureToolStripMenuItem.Name = "changePictureToolStripMenuItem";
            this.changePictureToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.changePictureToolStripMenuItem.Text = "&Bild ändern...";
            // 
            // deletePictureToolStripMenuItem
            // 
            this.deletePictureToolStripMenuItem.Name = "deletePictureToolStripMenuItem";
            this.deletePictureToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.deletePictureToolStripMenuItem.Text = "Bild &entfernen";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
            // 
            // copyPictureToolStripMenuItem
            // 
            this.copyPictureToolStripMenuItem.Image = global::Big3.Hitbase.CDUtilities.Images.Copy;
            this.copyPictureToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyPictureToolStripMenuItem.Name = "copyPictureToolStripMenuItem";
            this.copyPictureToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.copyPictureToolStripMenuItem.Text = "&Kopieren";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.choosePictureButton);
            this.groupBox.Controls.Add(this.textBoxDayOfDeath);
            this.groupBox.Controls.Add(this.textBoxBirthday);
            this.groupBox.Controls.Add(this.labelBirthday);
            this.groupBox.Controls.Add(this.labelDeath);
            this.groupBox.Controls.Add(this.textBoxComment);
            this.groupBox.Controls.Add(this.label7);
            this.groupBox.Controls.Add(this.buttonGoToHomepage);
            this.groupBox.Controls.Add(this.textBoxHomepage);
            this.groupBox.Controls.Add(this.label6);
            this.groupBox.Controls.Add(this.comboBoxCountry);
            this.groupBox.Controls.Add(this.comboBoxSex);
            this.groupBox.Controls.Add(this.label5);
            this.groupBox.Controls.Add(this.label4);
            this.groupBox.Controls.Add(this.comboBoxArtistType);
            this.groupBox.Controls.Add(this.label3);
            this.groupBox.Controls.Add(this.textBoxSaveAs);
            this.groupBox.Controls.Add(this.label2);
            this.groupBox.Controls.Add(this.textBoxName);
            this.groupBox.Controls.Add(this.label1);
            this.groupBox.Location = new System.Drawing.Point(12, 12);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(524, 366);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Interpret";
            // 
            // choosePictureButton
            // 
            this.choosePictureButton.ButtonText = "Klicken Sie hier, um ein Bild hinzuzufügen.";
            this.choosePictureButton.EnableLoadFromWeb = false;
            this.choosePictureButton.ImageFilename = null;
            this.choosePictureButton.ImageWatermark = global::Big3.Hitbase.CDUtilities.Images.MissingPersonImageWatermark;
            this.choosePictureButton.Location = new System.Drawing.Point(11, 24);
            this.choosePictureButton.Name = "choosePictureButton";
            this.choosePictureButton.Size = new System.Drawing.Size(120, 127);
            this.choosePictureButton.TabIndex = 20;
            // 
            // textBoxDayOfDeath
            // 
            this.textBoxDayOfDeath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDayOfDeath.Location = new System.Drawing.Point(408, 162);
            this.textBoxDayOfDeath.Name = "textBoxDayOfDeath";
            this.textBoxDayOfDeath.Size = new System.Drawing.Size(100, 20);
            this.textBoxDayOfDeath.TabIndex = 14;
            this.textBoxDayOfDeath.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxDayOfDeath_Validating);
            // 
            // textBoxBirthday
            // 
            this.textBoxBirthday.Location = new System.Drawing.Point(90, 162);
            this.textBoxBirthday.Name = "textBoxBirthday";
            this.textBoxBirthday.Size = new System.Drawing.Size(100, 20);
            this.textBoxBirthday.TabIndex = 12;
            this.textBoxBirthday.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxBirthday_Validating);
            // 
            // labelBirthday
            // 
            this.labelBirthday.AutoSize = true;
            this.labelBirthday.Location = new System.Drawing.Point(8, 165);
            this.labelBirthday.Name = "labelBirthday";
            this.labelBirthday.Size = new System.Drawing.Size(68, 13);
            this.labelBirthday.TabIndex = 11;
            this.labelBirthday.Text = "Geboren am:";
            // 
            // labelDeath
            // 
            this.labelDeath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDeath.AutoSize = true;
            this.labelDeath.Location = new System.Drawing.Point(326, 165);
            this.labelDeath.Name = "labelDeath";
            this.labelDeath.Size = new System.Drawing.Size(76, 13);
            this.labelDeath.TabIndex = 13;
            this.labelDeath.Text = "Gestorben am:";
            // 
            // textBoxComment
            // 
            this.textBoxComment.AcceptsReturn = true;
            this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComment.Location = new System.Drawing.Point(11, 236);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxComment.Size = new System.Drawing.Size(497, 110);
            this.textBoxComment.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 220);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Kommentar:";
            // 
            // buttonGoToHomepage
            // 
            this.buttonGoToHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGoToHomepage.Image = global::Big3.Hitbase.CDUtilities.Images.Internet;
            this.buttonGoToHomepage.Location = new System.Drawing.Point(481, 186);
            this.buttonGoToHomepage.Name = "buttonGoToHomepage";
            this.buttonGoToHomepage.Size = new System.Drawing.Size(27, 23);
            this.buttonGoToHomepage.TabIndex = 17;
            this.buttonGoToHomepage.UseVisualStyleBackColor = true;
            this.buttonGoToHomepage.Click += new System.EventHandler(this.buttonGoToHomepage_Click);
            // 
            // textBoxHomepage
            // 
            this.textBoxHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHomepage.Location = new System.Drawing.Point(90, 188);
            this.textBoxHomepage.Name = "textBoxHomepage";
            this.textBoxHomepage.Size = new System.Drawing.Size(388, 20);
            this.textBoxHomepage.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 191);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Homepage:";
            // 
            // comboBoxCountry
            // 
            this.comboBoxCountry.FormattingEnabled = true;
            this.comboBoxCountry.Location = new System.Drawing.Point(247, 130);
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.Size = new System.Drawing.Size(155, 21);
            this.comboBoxCountry.TabIndex = 9;
            this.comboBoxCountry.SelectedIndexChanged += new System.EventHandler(this.comboBoxCountry_SelectedIndexChanged);
            // 
            // comboBoxSex
            // 
            this.comboBoxSex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSex.FormattingEnabled = true;
            this.comboBoxSex.Location = new System.Drawing.Point(247, 103);
            this.comboBoxSex.Name = "comboBoxSex";
            this.comboBoxSex.Size = new System.Drawing.Size(155, 21);
            this.comboBoxSex.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Herkunftsland:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(149, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Geschlecht:";
            // 
            // comboBoxArtistType
            // 
            this.comboBoxArtistType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxArtistType.FormattingEnabled = true;
            this.comboBoxArtistType.Location = new System.Drawing.Point(247, 76);
            this.comboBoxArtistType.Name = "comboBoxArtistType";
            this.comboBoxArtistType.Size = new System.Drawing.Size(155, 21);
            this.comboBoxArtistType.TabIndex = 5;
            this.comboBoxArtistType.SelectedIndexChanged += new System.EventHandler(this.comboBoxArtistType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(149, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Art:";
            // 
            // textBoxSaveAs
            // 
            this.textBoxSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSaveAs.Location = new System.Drawing.Point(247, 50);
            this.textBoxSaveAs.Name = "textBoxSaveAs";
            this.textBoxSaveAs.Size = new System.Drawing.Size(261, 20);
            this.textBoxSaveAs.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Speichern unter:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(247, 24);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(261, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            this.textBoxName.Leave += new System.EventHandler(this.textBoxName_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(149, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(380, 384);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(461, 384);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormArtistProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(549, 419);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FormArtistProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Interpret";
            this.Load += new System.EventHandler(this.FormArtistProperties_Load);
            this.contextMenuStripImage.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonGoToHomepage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelBirthday;
        private System.Windows.Forms.Label labelDeath;
        public System.Windows.Forms.TextBox textBoxName;
        public System.Windows.Forms.TextBox textBoxSaveAs;
        public System.Windows.Forms.ComboBox comboBoxArtistType;
        public System.Windows.Forms.ComboBox comboBoxSex;
        public System.Windows.Forms.TextBox textBoxComment;
        public System.Windows.Forms.TextBox textBoxHomepage;
        public System.Windows.Forms.ComboBox comboBoxCountry;
        public System.Windows.Forms.TextBox textBoxDayOfDeath;
        public System.Windows.Forms.TextBox textBoxBirthday;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripImage;
        private System.Windows.Forms.ToolStripMenuItem changePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyPictureToolStripMenuItem;
        private Big3.Hitbase.Controls.ChoosePictureButton choosePictureButton;
    }
}