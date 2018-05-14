namespace Big3.Hitbase.CDUtilities
{
    partial class FormDeleteMedium
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
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonDeleteCategory = new System.Windows.Forms.RadioButton();
            this.radioButtonChangeCategory = new System.Windows.Forms.RadioButton();
            this.comboBoxMediums = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(75, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 63);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wenn Sie existierende Datensätze mit dem zu löschenden Medium haben, sollten Sie " +
    "in diesen Datensätzen das Medium ändern. Hitbase kann dies für Sie automatisch e" +
    "rledigen.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // radioButtonDeleteCategory
            // 
            this.radioButtonDeleteCategory.AutoSize = true;
            this.radioButtonDeleteCategory.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonDeleteCategory.Location = new System.Drawing.Point(35, 96);
            this.radioButtonDeleteCategory.Name = "radioButtonDeleteCategory";
            this.radioButtonDeleteCategory.Size = new System.Drawing.Size(366, 17);
            this.radioButtonDeleteCategory.TabIndex = 0;
            this.radioButtonDeleteCategory.TabStop = true;
            this.radioButtonDeleteCategory.Text = "In den betreffenden Datensätzen die Zuweisung zum Medium entfernen.";
            this.radioButtonDeleteCategory.UseVisualStyleBackColor = false;
            this.radioButtonDeleteCategory.CheckedChanged += new System.EventHandler(this.radioButtonDeleteCategory_CheckedChanged);
            // 
            // radioButtonChangeCategory
            // 
            this.radioButtonChangeCategory.AutoSize = true;
            this.radioButtonChangeCategory.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonChangeCategory.Location = new System.Drawing.Point(35, 122);
            this.radioButtonChangeCategory.Name = "radioButtonChangeCategory";
            this.radioButtonChangeCategory.Size = new System.Drawing.Size(291, 17);
            this.radioButtonChangeCategory.TabIndex = 1;
            this.radioButtonChangeCategory.TabStop = true;
            this.radioButtonChangeCategory.Text = "In den betreffenden Datensätzen das Medium ändern in:";
            this.radioButtonChangeCategory.UseVisualStyleBackColor = false;
            this.radioButtonChangeCategory.CheckedChanged += new System.EventHandler(this.radioButtonChangeCategory_CheckedChanged);
            // 
            // comboBoxMediums
            // 
            this.comboBoxMediums.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMediums.FormattingEnabled = true;
            this.comboBoxMediums.Location = new System.Drawing.Point(53, 145);
            this.comboBoxMediums.Name = "comboBoxMediums";
            this.comboBoxMediums.Size = new System.Drawing.Size(192, 21);
            this.comboBoxMediums.TabIndex = 2;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(228, 196);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(128, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "Medium löschen";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(362, 196);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::Big3.Hitbase.CDUtilities.Images.Information;
            this.pictureBox1.Location = new System.Drawing.Point(12, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // FormDeleteMedium
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(449, 231);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxMediums);
            this.Controls.Add(this.radioButtonChangeCategory);
            this.Controls.Add(this.radioButtonDeleteCategory);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDeleteMedium";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Medium löschen";
            this.Load += new System.EventHandler(this.FormDeleteMedium_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonDeleteCategory;
        private System.Windows.Forms.RadioButton radioButtonChangeCategory;
        private System.Windows.Forms.ComboBox comboBoxMediums;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}