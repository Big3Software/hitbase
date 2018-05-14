namespace Big3.Hitbase.CDUtilities
{
    partial class FormChooseCodes
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxCode1 = new System.Windows.Forms.ComboBox();
            this.comboBoxCode2 = new System.Windows.Forms.ComboBox();
            this.comboBoxCode3 = new System.Windows.Forms.ComboBox();
            this.comboBoxCode4 = new System.Windows.Forms.ComboBox();
            this.comboBoxCode5 = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "1. Kennzeichen:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(15, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "2. Kennzeichen:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(15, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "3. Kennzeichen:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(15, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "4. Kennzeichen:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(15, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "5. Kennzeichen:";
            // 
            // comboBoxCode1
            // 
            this.comboBoxCode1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCode1.FormattingEnabled = true;
            this.comboBoxCode1.Location = new System.Drawing.Point(105, 25);
            this.comboBoxCode1.Name = "comboBoxCode1";
            this.comboBoxCode1.Size = new System.Drawing.Size(236, 21);
            this.comboBoxCode1.TabIndex = 5;
            // 
            // comboBoxCode2
            // 
            this.comboBoxCode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCode2.FormattingEnabled = true;
            this.comboBoxCode2.Location = new System.Drawing.Point(105, 52);
            this.comboBoxCode2.Name = "comboBoxCode2";
            this.comboBoxCode2.Size = new System.Drawing.Size(236, 21);
            this.comboBoxCode2.TabIndex = 6;
            // 
            // comboBoxCode3
            // 
            this.comboBoxCode3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCode3.FormattingEnabled = true;
            this.comboBoxCode3.Location = new System.Drawing.Point(105, 79);
            this.comboBoxCode3.Name = "comboBoxCode3";
            this.comboBoxCode3.Size = new System.Drawing.Size(236, 21);
            this.comboBoxCode3.TabIndex = 7;
            // 
            // comboBoxCode4
            // 
            this.comboBoxCode4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCode4.FormattingEnabled = true;
            this.comboBoxCode4.Location = new System.Drawing.Point(105, 106);
            this.comboBoxCode4.Name = "comboBoxCode4";
            this.comboBoxCode4.Size = new System.Drawing.Size(236, 21);
            this.comboBoxCode4.TabIndex = 8;
            // 
            // comboBoxCode5
            // 
            this.comboBoxCode5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCode5.FormattingEnabled = true;
            this.comboBoxCode5.Location = new System.Drawing.Point(105, 133);
            this.comboBoxCode5.Name = "comboBoxCode5";
            this.comboBoxCode5.Size = new System.Drawing.Size(236, 21);
            this.comboBoxCode5.TabIndex = 9;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(185, 173);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(266, 173);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormChooseCodes
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(353, 208);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxCode5);
            this.Controls.Add(this.comboBoxCode4);
            this.Controls.Add(this.comboBoxCode3);
            this.Controls.Add(this.comboBoxCode2);
            this.Controls.Add(this.comboBoxCode1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChooseCodes";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kennzeichen auswählen";
            this.Load += new System.EventHandler(this.FormChooseCodes_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxCode1;
        private System.Windows.Forms.ComboBox comboBoxCode2;
        private System.Windows.Forms.ComboBox comboBoxCode3;
        private System.Windows.Forms.ComboBox comboBoxCode4;
        private System.Windows.Forms.ComboBox comboBoxCode5;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}