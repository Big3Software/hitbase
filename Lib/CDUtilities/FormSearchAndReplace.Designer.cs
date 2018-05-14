namespace Big3.Hitbase.CDUtilities
{
    partial class FormSearchAndReplace
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
            this.buttonSearchAndReplace = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonCD = new System.Windows.Forms.RadioButton();
            this.radioButtonTrack = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSearchFor = new System.Windows.Forms.TextBox();
            this.textBoxReplace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxField = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelStatus = new System.Windows.Forms.Label();
            this.checkBoxCaseSensitive = new System.Windows.Forms.CheckBox();
            this.checkBoxWholeWords = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonSearchAndReplace
            // 
            this.buttonSearchAndReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchAndReplace.Location = new System.Drawing.Point(233, 350);
            this.buttonSearchAndReplace.Name = "buttonSearchAndReplace";
            this.buttonSearchAndReplace.Size = new System.Drawing.Size(130, 23);
            this.buttonSearchAndReplace.TabIndex = 15;
            this.buttonSearchAndReplace.Text = "Suchen und ersetzen";
            this.buttonSearchAndReplace.UseVisualStyleBackColor = true;
            this.buttonSearchAndReplace.Click += new System.EventHandler(this.buttonSearchAndReplace_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(369, 350);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 16;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(413, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wählen Sie bitte zunächst aus, ob Sie in einem CD- oder Track-Feld suchen möchten" +
                ":";
            // 
            // radioButtonCD
            // 
            this.radioButtonCD.AutoSize = true;
            this.radioButtonCD.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonCD.Location = new System.Drawing.Point(59, 44);
            this.radioButtonCD.Name = "radioButtonCD";
            this.radioButtonCD.Size = new System.Drawing.Size(40, 17);
            this.radioButtonCD.TabIndex = 1;
            this.radioButtonCD.TabStop = true;
            this.radioButtonCD.Text = "&CD";
            this.radioButtonCD.UseVisualStyleBackColor = false;
            this.radioButtonCD.CheckedChanged += new System.EventHandler(this.radioButtonCD_CheckedChanged);
            // 
            // radioButtonTrack
            // 
            this.radioButtonTrack.AutoSize = true;
            this.radioButtonTrack.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonTrack.Location = new System.Drawing.Point(59, 67);
            this.radioButtonTrack.Name = "radioButtonTrack";
            this.radioButtonTrack.Size = new System.Drawing.Size(53, 17);
            this.radioButtonTrack.TabIndex = 2;
            this.radioButtonTrack.TabStop = true;
            this.radioButtonTrack.Text = "&Track";
            this.radioButtonTrack.UseVisualStyleBackColor = false;
            this.radioButtonTrack.CheckedChanged += new System.EventHandler(this.radioButtonTrack_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 197);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "&Suchen nach:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(12, 223);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "&Ersetzen:";
            // 
            // textBoxSearchFor
            // 
            this.textBoxSearchFor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearchFor.Location = new System.Drawing.Point(96, 194);
            this.textBoxSearchFor.Name = "textBoxSearchFor";
            this.textBoxSearchFor.Size = new System.Drawing.Size(348, 20);
            this.textBoxSearchFor.TabIndex = 8;
            this.textBoxSearchFor.TextChanged += new System.EventHandler(this.textBoxSearchFor_TextChanged);
            // 
            // textBoxReplace
            // 
            this.textBoxReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReplace.Location = new System.Drawing.Point(96, 220);
            this.textBoxReplace.Name = "textBoxReplace";
            this.textBoxReplace.Size = new System.Drawing.Size(348, 20);
            this.textBoxReplace.TabIndex = 10;
            this.textBoxReplace.TextChanged += new System.EventHandler(this.textBoxReplace_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(12, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "&Feld:";
            // 
            // comboBoxField
            // 
            this.comboBoxField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxField.FormattingEnabled = true;
            this.comboBoxField.Location = new System.Drawing.Point(96, 130);
            this.comboBoxField.Name = "comboBoxField";
            this.comboBoxField.Size = new System.Drawing.Size(201, 21);
            this.comboBoxField.Sorted = true;
            this.comboBoxField.TabIndex = 5;
            this.comboBoxField.SelectedIndexChanged += new System.EventHandler(this.comboBoxField_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(12, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(326, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Wählen Sie das gewünschte Feld aus, in dem Sie suchen möchten:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(12, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(285, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Geben Sie nun an, was Sie suchen und ersetzen möchten:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 304);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(429, 13);
            this.progressBar.TabIndex = 13;
            this.progressBar.Visible = false;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus.Location = new System.Drawing.Point(12, 322);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(59, 13);
            this.labelStatus.TabIndex = 14;
            this.labelStatus.Text = "labelStatus";
            this.labelStatus.Visible = false;
            // 
            // checkBoxCaseSensitive
            // 
            this.checkBoxCaseSensitive.AutoSize = true;
            this.checkBoxCaseSensitive.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxCaseSensitive.Location = new System.Drawing.Point(96, 246);
            this.checkBoxCaseSensitive.Name = "checkBoxCaseSensitive";
            this.checkBoxCaseSensitive.Size = new System.Drawing.Size(181, 17);
            this.checkBoxCaseSensitive.TabIndex = 11;
            this.checkBoxCaseSensitive.Text = "&Groß-, Kleinschreibung beachten";
            this.checkBoxCaseSensitive.UseVisualStyleBackColor = false;
            // 
            // checkBoxWholeWords
            // 
            this.checkBoxWholeWords.AutoSize = true;
            this.checkBoxWholeWords.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxWholeWords.Location = new System.Drawing.Point(96, 269);
            this.checkBoxWholeWords.Name = "checkBoxWholeWords";
            this.checkBoxWholeWords.Size = new System.Drawing.Size(144, 17);
            this.checkBoxWholeWords.TabIndex = 12;
            this.checkBoxWholeWords.Text = "&Nur ganzes Wort suchen";
            this.checkBoxWholeWords.UseVisualStyleBackColor = false;
            // 
            // FormSearchAndReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(456, 385);
            this.Controls.Add(this.checkBoxWholeWords);
            this.Controls.Add(this.checkBoxCaseSensitive);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxField);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxReplace);
            this.Controls.Add(this.textBoxSearchFor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButtonTrack);
            this.Controls.Add(this.radioButtonCD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSearchAndReplace);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSearchAndReplace";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Suchen und Ersetzen";
            this.Load += new System.EventHandler(this.FormSearchAndReplace_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSearchAndReplace;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonCD;
        private System.Windows.Forms.RadioButton radioButtonTrack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSearchFor;
        private System.Windows.Forms.TextBox textBoxReplace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.CheckBox checkBoxCaseSensitive;
        private System.Windows.Forms.CheckBox checkBoxWholeWords;
    }
}