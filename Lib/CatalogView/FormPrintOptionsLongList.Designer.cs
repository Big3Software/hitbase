namespace Big3.Hitbase.CatalogView
{
    partial class FormPrintOptionsLongList
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxPrintCDCover = new System.Windows.Forms.CheckBox();
            this.labelCDCoverSize = new System.Windows.Forms.Label();
            this.numericUpDownCDCoverSize = new System.Windows.Forms.NumericUpDown();
            this.checkBoxTextBelowCDCover = new System.Windows.Forms.CheckBox();
            this.groupBoxPosition = new System.Windows.Forms.GroupBox();
            this.radioButtonCDCoverPositionLeft = new System.Windows.Forms.RadioButton();
            this.radioButtonCDCoverPositionCenter = new System.Windows.Forms.RadioButton();
            this.radioButtonCDCoverPositionRight = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCDCoverSize)).BeginInit();
            this.groupBoxPosition.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBoxPosition);
            this.groupBox1.Controls.Add(this.checkBoxTextBelowCDCover);
            this.groupBox1.Controls.Add(this.numericUpDownCDCoverSize);
            this.groupBox1.Controls.Add(this.labelCDCoverSize);
            this.groupBox1.Controls.Add(this.checkBoxPrintCDCover);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 245);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CD-Cover";
            // 
            // checkBoxPrintCDCover
            // 
            this.checkBoxPrintCDCover.AutoSize = true;
            this.checkBoxPrintCDCover.Location = new System.Drawing.Point(19, 38);
            this.checkBoxPrintCDCover.Name = "checkBoxPrintCDCover";
            this.checkBoxPrintCDCover.Size = new System.Drawing.Size(131, 17);
            this.checkBoxPrintCDCover.TabIndex = 0;
            this.checkBoxPrintCDCover.Text = "CD-Cover ausdrucken";
            this.checkBoxPrintCDCover.UseVisualStyleBackColor = true;
            this.checkBoxPrintCDCover.CheckedChanged += new System.EventHandler(this.checkBoxPrintCDCover_CheckedChanged);
            // 
            // labelCDCoverSize
            // 
            this.labelCDCoverSize.AutoSize = true;
            this.labelCDCoverSize.Location = new System.Drawing.Point(16, 81);
            this.labelCDCoverSize.Name = "labelCDCoverSize";
            this.labelCDCoverSize.Size = new System.Drawing.Size(172, 13);
            this.labelCDCoverSize.TabIndex = 1;
            this.labelCDCoverSize.Text = "Größe (in Prozent der Seitenbreite):";
            // 
            // numericUpDownCDCoverSize
            // 
            this.numericUpDownCDCoverSize.Location = new System.Drawing.Point(194, 79);
            this.numericUpDownCDCoverSize.Name = "numericUpDownCDCoverSize";
            this.numericUpDownCDCoverSize.Size = new System.Drawing.Size(67, 20);
            this.numericUpDownCDCoverSize.TabIndex = 2;
            // 
            // checkBoxTextBelowCDCover
            // 
            this.checkBoxTextBelowCDCover.AutoSize = true;
            this.checkBoxTextBelowCDCover.Location = new System.Drawing.Point(19, 117);
            this.checkBoxTextBelowCDCover.Name = "checkBoxTextBelowCDCover";
            this.checkBoxTextBelowCDCover.Size = new System.Drawing.Size(206, 17);
            this.checkBoxTextBelowCDCover.TabIndex = 3;
            this.checkBoxTextBelowCDCover.Text = "Text beginnt unterhalb des CD-Covers";
            this.checkBoxTextBelowCDCover.UseVisualStyleBackColor = true;
            this.checkBoxTextBelowCDCover.CheckedChanged += new System.EventHandler(this.checkBoxTextBelowCDCover_CheckedChanged);
            // 
            // groupBoxPosition
            // 
            this.groupBoxPosition.Controls.Add(this.radioButtonCDCoverPositionRight);
            this.groupBoxPosition.Controls.Add(this.radioButtonCDCoverPositionCenter);
            this.groupBoxPosition.Controls.Add(this.radioButtonCDCoverPositionLeft);
            this.groupBoxPosition.Location = new System.Drawing.Point(19, 158);
            this.groupBoxPosition.Name = "groupBoxPosition";
            this.groupBoxPosition.Size = new System.Drawing.Size(258, 65);
            this.groupBoxPosition.TabIndex = 4;
            this.groupBoxPosition.TabStop = false;
            this.groupBoxPosition.Text = "Position auf der Seite";
            // 
            // radioButtonCDCoverPositionLeft
            // 
            this.radioButtonCDCoverPositionLeft.AutoSize = true;
            this.radioButtonCDCoverPositionLeft.Location = new System.Drawing.Point(26, 31);
            this.radioButtonCDCoverPositionLeft.Name = "radioButtonCDCoverPositionLeft";
            this.radioButtonCDCoverPositionLeft.Size = new System.Drawing.Size(50, 17);
            this.radioButtonCDCoverPositionLeft.TabIndex = 0;
            this.radioButtonCDCoverPositionLeft.TabStop = true;
            this.radioButtonCDCoverPositionLeft.Text = "Links";
            this.radioButtonCDCoverPositionLeft.UseVisualStyleBackColor = true;
            // 
            // radioButtonCDCoverPositionCenter
            // 
            this.radioButtonCDCoverPositionCenter.AutoSize = true;
            this.radioButtonCDCoverPositionCenter.Location = new System.Drawing.Point(94, 31);
            this.radioButtonCDCoverPositionCenter.Name = "radioButtonCDCoverPositionCenter";
            this.radioButtonCDCoverPositionCenter.Size = new System.Drawing.Size(48, 17);
            this.radioButtonCDCoverPositionCenter.TabIndex = 1;
            this.radioButtonCDCoverPositionCenter.TabStop = true;
            this.radioButtonCDCoverPositionCenter.Text = "Mitte";
            this.radioButtonCDCoverPositionCenter.UseVisualStyleBackColor = true;
            // 
            // radioButtonCDCoverPositionRight
            // 
            this.radioButtonCDCoverPositionRight.AutoSize = true;
            this.radioButtonCDCoverPositionRight.Location = new System.Drawing.Point(161, 31);
            this.radioButtonCDCoverPositionRight.Name = "radioButtonCDCoverPositionRight";
            this.radioButtonCDCoverPositionRight.Size = new System.Drawing.Size(59, 17);
            this.radioButtonCDCoverPositionRight.TabIndex = 2;
            this.radioButtonCDCoverPositionRight.TabStop = true;
            this.radioButtonCDCoverPositionRight.Text = "Rechts";
            this.radioButtonCDCoverPositionRight.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(338, 12);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(338, 41);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormPrintOptionsLongList
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(425, 269);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrintOptionsLongList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Druckoptionen";
            this.Load += new System.EventHandler(this.FormPrintOptionsLongList_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCDCoverSize)).EndInit();
            this.groupBoxPosition.ResumeLayout(false);
            this.groupBoxPosition.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxPrintCDCover;
        private System.Windows.Forms.Label labelCDCoverSize;
        private System.Windows.Forms.NumericUpDown numericUpDownCDCoverSize;
        private System.Windows.Forms.CheckBox checkBoxTextBelowCDCover;
        private System.Windows.Forms.GroupBox groupBoxPosition;
        private System.Windows.Forms.RadioButton radioButtonCDCoverPositionRight;
        private System.Windows.Forms.RadioButton radioButtonCDCoverPositionCenter;
        private System.Windows.Forms.RadioButton radioButtonCDCoverPositionLeft;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}