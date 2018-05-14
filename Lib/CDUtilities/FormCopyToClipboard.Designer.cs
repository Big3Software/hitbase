namespace Big3.Hitbase.CDUtilities
{
    partial class FormCopyToClipboard
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxFieldSeperator = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxRecordSeperator = new System.Windows.Forms.ComboBox();
            this.checkBoxFieldsInFirstLine = new System.Windows.Forms.CheckBox();
            this.checkBoxQuoteTextFields = new System.Windows.Forms.CheckBox();
            this.chooseFieldControl = new Big3.Hitbase.CDUtilities.ChooseFieldControl();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(358, 391);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(439, 391);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(502, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Wählen Sie aus der folgenden Liste aus, welche Felder für die selektierten CDs od" +
    "er Tracks in die Zwischenablage kopiert werden sollen:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(13, 269);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Trennzeichen für Felder:";
            // 
            // comboBoxFieldSeperator
            // 
            this.comboBoxFieldSeperator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxFieldSeperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFieldSeperator.FormattingEnabled = true;
            this.comboBoxFieldSeperator.Location = new System.Drawing.Point(166, 266);
            this.comboBoxFieldSeperator.Name = "comboBoxFieldSeperator";
            this.comboBoxFieldSeperator.Size = new System.Drawing.Size(83, 21);
            this.comboBoxFieldSeperator.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(13, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Trennzeichen für Datensätze:";
            // 
            // comboBoxRecordSeperator
            // 
            this.comboBoxRecordSeperator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxRecordSeperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecordSeperator.FormattingEnabled = true;
            this.comboBoxRecordSeperator.Location = new System.Drawing.Point(166, 294);
            this.comboBoxRecordSeperator.Name = "comboBoxRecordSeperator";
            this.comboBoxRecordSeperator.Size = new System.Drawing.Size(83, 21);
            this.comboBoxRecordSeperator.TabIndex = 8;
            // 
            // checkBoxFieldsInFirstLine
            // 
            this.checkBoxFieldsInFirstLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxFieldsInFirstLine.AutoSize = true;
            this.checkBoxFieldsInFirstLine.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxFieldsInFirstLine.Location = new System.Drawing.Point(15, 325);
            this.checkBoxFieldsInFirstLine.Name = "checkBoxFieldsInFirstLine";
            this.checkBoxFieldsInFirstLine.Size = new System.Drawing.Size(190, 17);
            this.checkBoxFieldsInFirstLine.TabIndex = 9;
            this.checkBoxFieldsInFirstLine.Text = "Feldnamen in erste Zeile schreiben";
            this.checkBoxFieldsInFirstLine.UseVisualStyleBackColor = false;
            // 
            // checkBoxQuoteTextFields
            // 
            this.checkBoxQuoteTextFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxQuoteTextFields.AutoSize = true;
            this.checkBoxQuoteTextFields.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxQuoteTextFields.Location = new System.Drawing.Point(15, 348);
            this.checkBoxQuoteTextFields.Name = "checkBoxQuoteTextFields";
            this.checkBoxQuoteTextFields.Size = new System.Drawing.Size(212, 17);
            this.checkBoxQuoteTextFields.TabIndex = 10;
            this.checkBoxQuoteTextFields.Text = "Textfelder in Anführungszeichen setzen";
            this.checkBoxQuoteTextFields.UseVisualStyleBackColor = false;
            // 
            // chooseFieldControl
            // 
            this.chooseFieldControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseFieldControl.BackColor = System.Drawing.Color.Transparent;
            this.chooseFieldControl.FieldType = Big3.Hitbase.DataBaseEngine.FieldType.Unknown;
            this.chooseFieldControl.Location = new System.Drawing.Point(12, 44);
            this.chooseFieldControl.Name = "chooseFieldControl";
            this.chooseFieldControl.Size = new System.Drawing.Size(502, 216);
            this.chooseFieldControl.TabIndex = 11;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(15, 373);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(499, 12);
            this.progressBar.TabIndex = 12;
            // 
            // FormCopyToClipboard
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(526, 426);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.chooseFieldControl);
            this.Controls.Add(this.checkBoxQuoteTextFields);
            this.Controls.Add(this.checkBoxFieldsInFirstLine);
            this.Controls.Add(this.comboBoxRecordSeperator);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxFieldSeperator);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FormCopyToClipboard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "In die Zwischenablage kopieren";
            this.Load += new System.EventHandler(this.FormCopyToClipboard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxFieldSeperator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxRecordSeperator;
        private System.Windows.Forms.CheckBox checkBoxFieldsInFirstLine;
        private System.Windows.Forms.CheckBox checkBoxQuoteTextFields;
        private ChooseFieldControl chooseFieldControl;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}