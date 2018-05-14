namespace Big3.Hitbase.CDUtilities
{
    partial class FormLoanProperties
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
            this.textBoxArtist = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLoanedTo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerLoaned = new System.Windows.Forms.DateTimePicker();
            this.checkBoxActivateBringBack = new System.Windows.Forms.CheckBox();
            this.labelBringBack = new System.Windows.Forms.Label();
            this.dateTimePickerBringBack = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonBringBack = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxGiveBack = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBoxArtist
            // 
            this.textBoxArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxArtist.Location = new System.Drawing.Point(106, 22);
            this.textBoxArtist.Name = "textBoxArtist";
            this.textBoxArtist.ReadOnly = true;
            this.textBoxArtist.Size = new System.Drawing.Size(308, 20);
            this.textBoxArtist.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Interpret:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Titel:";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTitle.Location = new System.Drawing.Point(106, 48);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.ReadOnly = true;
            this.textBoxTitle.Size = new System.Drawing.Size(308, 20);
            this.textBoxTitle.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(12, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Verliehen an:";
            // 
            // comboBoxLoanedTo
            // 
            this.comboBoxLoanedTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLoanedTo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxLoanedTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxLoanedTo.FormattingEnabled = true;
            this.comboBoxLoanedTo.Location = new System.Drawing.Point(106, 74);
            this.comboBoxLoanedTo.Name = "comboBoxLoanedTo";
            this.comboBoxLoanedTo.Size = new System.Drawing.Size(200, 21);
            this.comboBoxLoanedTo.Sorted = true;
            this.comboBoxLoanedTo.TabIndex = 5;
            this.comboBoxLoanedTo.TextChanged += new System.EventHandler(this.comboBoxLoanedTo_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(12, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "V&erliehen am:";
            // 
            // dateTimePickerLoaned
            // 
            this.dateTimePickerLoaned.Location = new System.Drawing.Point(106, 101);
            this.dateTimePickerLoaned.Name = "dateTimePickerLoaned";
            this.dateTimePickerLoaned.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerLoaned.TabIndex = 7;
            // 
            // checkBoxActivateBringBack
            // 
            this.checkBoxActivateBringBack.AutoSize = true;
            this.checkBoxActivateBringBack.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxActivateBringBack.Location = new System.Drawing.Point(15, 130);
            this.checkBoxActivateBringBack.Name = "checkBoxActivateBringBack";
            this.checkBoxActivateBringBack.Size = new System.Drawing.Size(153, 17);
            this.checkBoxActivateBringBack.TabIndex = 8;
            this.checkBoxActivateBringBack.Text = "&Rückgabetermin aktivieren";
            this.checkBoxActivateBringBack.UseVisualStyleBackColor = false;
            this.checkBoxActivateBringBack.CheckedChanged += new System.EventHandler(this.checkBoxActivateBringBack_CheckedChanged);
            // 
            // labelBringBack
            // 
            this.labelBringBack.AutoSize = true;
            this.labelBringBack.BackColor = System.Drawing.Color.Transparent;
            this.labelBringBack.Location = new System.Drawing.Point(12, 182);
            this.labelBringBack.Name = "labelBringBack";
            this.labelBringBack.Size = new System.Drawing.Size(88, 13);
            this.labelBringBack.TabIndex = 11;
            this.labelBringBack.Text = "Rückgabe&termin:";
            this.labelBringBack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimePickerBringBack
            // 
            this.dateTimePickerBringBack.Location = new System.Drawing.Point(106, 178);
            this.dateTimePickerBringBack.Name = "dateTimePickerBringBack";
            this.dateTimePickerBringBack.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerBringBack.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(12, 210);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "&Kommentar:";
            // 
            // textBoxComment
            // 
            this.textBoxComment.AcceptsReturn = true;
            this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComment.Location = new System.Drawing.Point(15, 226);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxComment.Size = new System.Drawing.Size(399, 83);
            this.textBoxComment.TabIndex = 14;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(177, 324);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 15;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(339, 324);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 17;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonBringBack
            // 
            this.buttonBringBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBringBack.Location = new System.Drawing.Point(258, 324);
            this.buttonBringBack.Name = "buttonBringBack";
            this.buttonBringBack.Size = new System.Drawing.Size(75, 23);
            this.buttonBringBack.TabIndex = 16;
            this.buttonBringBack.Text = "Rü&ckgabe";
            this.buttonBringBack.UseVisualStyleBackColor = true;
            this.buttonBringBack.Click += new System.EventHandler(this.buttonBringBack_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(82, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "in:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxGiveBack
            // 
            this.comboBoxGiveBack.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxGiveBack.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxGiveBack.FormattingEnabled = true;
            this.comboBoxGiveBack.Items.AddRange(new object[] {
            "1 Tag",
            "2 Tagen",
            "3 Tagen",
            "4 Tagen",
            "5 Tagen",
            "6 Tagen",
            "1 Woche",
            "2 Wochen",
            "3 Wochen",
            "4 Wochen",
            "1 Monat",
            "2 Monaten",
            "3 Monaten",
            "6 Monaten",
            "12 Monaten"});
            this.comboBoxGiveBack.Location = new System.Drawing.Point(106, 151);
            this.comboBoxGiveBack.Name = "comboBoxGiveBack";
            this.comboBoxGiveBack.Size = new System.Drawing.Size(125, 21);
            this.comboBoxGiveBack.TabIndex = 10;
            this.comboBoxGiveBack.SelectedIndexChanged += new System.EventHandler(this.comboBoxGiveBack_SelectedIndexChanged);
            this.comboBoxGiveBack.Leave += new System.EventHandler(this.comboBoxGiveBack_Leave);
            // 
            // FormLoanProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(426, 359);
            this.Controls.Add(this.comboBoxGiveBack);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonBringBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxComment);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePickerBringBack);
            this.Controls.Add(this.labelBringBack);
            this.Controls.Add(this.checkBoxActivateBringBack);
            this.Controls.Add(this.dateTimePickerLoaned);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxLoanedTo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxArtist);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLoanProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Informationen zur ausgeliehenen CD";
            this.Load += new System.EventHandler(this.FormLoanProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelBringBack;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBringBack;
        public System.Windows.Forms.TextBox textBoxArtist;
        public System.Windows.Forms.TextBox textBoxTitle;
        public System.Windows.Forms.ComboBox comboBoxLoanedTo;
        public System.Windows.Forms.DateTimePicker dateTimePickerLoaned;
        public System.Windows.Forms.CheckBox checkBoxActivateBringBack;
        public System.Windows.Forms.DateTimePicker dateTimePickerBringBack;
        public System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxGiveBack;
    }
}