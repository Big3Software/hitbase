namespace Big3.Hitbase.CDUtilities
{
    partial class FormTransferFields
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTransferFields));
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSourceField = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetField = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.checkBoxMoveField = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(77, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(368, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mit dieser Funktion können Sie den Inhalt eines Feldes in ein beliebiges anderes " +
                "Feld übertragen.";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(80, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(365, 61);
            this.label3.TabIndex = 2;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(80, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(303, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Wählen Sie zunächst das Feld aus, das Sie kopieren möchten:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(80, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(338, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Wählen Sie nun das Feld aus, in das Sie die Daten kopieren möchten:";
            // 
            // comboBoxSourceField
            // 
            this.comboBoxSourceField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSourceField.FormattingEnabled = true;
            this.comboBoxSourceField.Location = new System.Drawing.Point(83, 168);
            this.comboBoxSourceField.Name = "comboBoxSourceField";
            this.comboBoxSourceField.Size = new System.Drawing.Size(205, 21);
            this.comboBoxSourceField.TabIndex = 5;
            this.comboBoxSourceField.SelectedIndexChanged += new System.EventHandler(this.comboBoxSourceField_SelectedIndexChanged);
            // 
            // comboBoxTargetField
            // 
            this.comboBoxTargetField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTargetField.FormattingEnabled = true;
            this.comboBoxTargetField.Location = new System.Drawing.Point(83, 224);
            this.comboBoxTargetField.Name = "comboBoxTargetField";
            this.comboBoxTargetField.Size = new System.Drawing.Size(205, 21);
            this.comboBoxTargetField.TabIndex = 6;
            this.comboBoxTargetField.SelectedIndexChanged += new System.EventHandler(this.comboBoxTargetField_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(80, 293);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(258, 36);
            this.label5.TabIndex = 7;
            this.label5.Text = "Definieren Sie einen Filter, um die Feldübertragung auf bestimmte CDs oder Tracks" +
                " einzuschränken.";
            // 
            // buttonFilter
            // 
            this.buttonFilter.Location = new System.Drawing.Point(370, 293);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(75, 23);
            this.buttonFilter.TabIndex = 8;
            this.buttonFilter.Text = "Filter...";
            this.buttonFilter.UseVisualStyleBackColor = true;
            this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
            // 
            // checkBoxMoveField
            // 
            this.checkBoxMoveField.AutoSize = true;
            this.checkBoxMoveField.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxMoveField.Location = new System.Drawing.Point(83, 341);
            this.checkBoxMoveField.Name = "checkBoxMoveField";
            this.checkBoxMoveField.Size = new System.Drawing.Size(275, 17);
            this.checkBoxMoveField.TabIndex = 9;
            this.checkBoxMoveField.Text = "Daten im Ursprungsfeld löschen (Daten verschieben)";
            this.checkBoxMoveField.UseVisualStyleBackColor = false;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(283, 399);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "Ausführen";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(370, 399);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Feldübertragung";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(15, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 2);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(12, 266);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(145, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Optionen für Feldübertragung";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(15, 273);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(430, 2);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            // 
            // FormTransferFields
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(457, 434);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxMoveField);
            this.Controls.Add(this.buttonFilter);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxTargetField);
            this.Controls.Add(this.comboBoxSourceField);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTransferFields";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Feldübertragung";
            this.Load += new System.EventHandler(this.FormTransferFields_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxSourceField;
        private System.Windows.Forms.ComboBox comboBoxTargetField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.CheckBox checkBoxMoveField;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}