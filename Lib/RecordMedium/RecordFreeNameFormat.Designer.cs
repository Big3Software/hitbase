namespace Big3.Hitbase.RecordMedium
{
    partial class RecordFreeNameFormat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordFreeNameFormat));
            this.textFormatFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textExample = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listTemplates = new System.Windows.Forms.ListBox();
            this.listAvailableFields = new System.Windows.Forms.ListBox();
            this.comboBoxFelder = new System.Windows.Forms.ComboBox();
            this.listAvailableTrackFields = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonUpTemplate = new System.Windows.Forms.Button();
            this.buttonDownTemplate = new System.Windows.Forms.Button();
            this.buttonSaveTemplate = new System.Windows.Forms.Button();
            this.buttonAddTemplate = new System.Windows.Forms.Button();
            this.buttonDeleteTemplate = new System.Windows.Forms.Button();
            this.buttonResetDefault = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textFormatFile
            // 
            this.textFormatFile.Location = new System.Drawing.Point(12, 344);
            this.textFormatFile.Multiline = true;
            this.textFormatFile.Name = "textFormatFile";
            this.textFormatFile.Size = new System.Drawing.Size(597, 32);
            this.textFormatFile.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 327);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dateiname für Aufnahme (Keine Sonderzeichen wie: \\/:*?\"<>|):";
            // 
            // textExample
            // 
            this.textExample.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textExample.Location = new System.Drawing.Point(12, 405);
            this.textExample.Multiline = true;
            this.textExample.Name = "textExample";
            this.textExample.ReadOnly = true;
            this.textExample.Size = new System.Drawing.Size(631, 36);
            this.textExample.TabIndex = 2;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(293, 447);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "&Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(391, 39);
            this.label3.TabIndex = 5;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(374, 447);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "A&bbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // listTemplates
            // 
            this.listTemplates.FormattingEnabled = true;
            this.listTemplates.Location = new System.Drawing.Point(12, 85);
            this.listTemplates.Name = "listTemplates";
            this.listTemplates.Size = new System.Drawing.Size(328, 199);
            this.listTemplates.TabIndex = 6;
            this.listTemplates.SelectedIndexChanged += new System.EventHandler(this.listTemplates_SelectedIndexChanged);
            // 
            // listAvailableFields
            // 
            this.listAvailableFields.FormattingEnabled = true;
            this.listAvailableFields.Location = new System.Drawing.Point(374, 85);
            this.listAvailableFields.Name = "listAvailableFields";
            this.listAvailableFields.Size = new System.Drawing.Size(162, 238);
            this.listAvailableFields.TabIndex = 8;
            this.listAvailableFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listAvailableFields_MouseDoubleClick);
            // 
            // comboBoxFelder
            // 
            this.comboBoxFelder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFelder.FormattingEnabled = true;
            this.comboBoxFelder.Location = new System.Drawing.Point(459, 27);
            this.comboBoxFelder.Name = "comboBoxFelder";
            this.comboBoxFelder.Size = new System.Drawing.Size(184, 21);
            this.comboBoxFelder.TabIndex = 9;
            this.comboBoxFelder.SelectedIndexChanged += new System.EventHandler(this.comboBoxFelder_SelectedIndexChanged);
            // 
            // listAvailableTrackFields
            // 
            this.listAvailableTrackFields.FormattingEnabled = true;
            this.listAvailableTrackFields.Location = new System.Drawing.Point(552, 85);
            this.listAvailableTrackFields.Name = "listAvailableTrackFields";
            this.listAvailableTrackFields.Size = new System.Drawing.Size(160, 238);
            this.listAvailableTrackFields.TabIndex = 10;
            this.listAvailableTrackFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listAvailableTrackFields_MouseDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 388);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(267, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Beispiel Dateiname für das aktuell ausgewählte Format:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(371, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "CD bezogene Felder:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(549, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Track bezogene Felder:";
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.domainUpDown1.Location = new System.Drawing.Point(649, 415);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.Size = new System.Drawing.Size(45, 26);
            this.domainUpDown1.TabIndex = 14;
            this.domainUpDown1.Text = "domainUpDown1";
            this.domainUpDown1.SelectedItemChanged += new System.EventHandler(this.domainUpDown1_SelectedItemChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(646, 398);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Track:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(12, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(204, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Formate für Datei- und Verzeichnisnamen:";
            // 
            // buttonUpTemplate
            // 
            this.buttonUpTemplate.Font = new System.Drawing.Font("Wingdings", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonUpTemplate.Location = new System.Drawing.Point(346, 161);
            this.buttonUpTemplate.Name = "buttonUpTemplate";
            this.buttonUpTemplate.Size = new System.Drawing.Size(22, 23);
            this.buttonUpTemplate.TabIndex = 17;
            this.buttonUpTemplate.Text = "ñ";
            this.buttonUpTemplate.UseVisualStyleBackColor = true;
            this.buttonUpTemplate.Click += new System.EventHandler(this.buttonUpTemplate_Click);
            // 
            // buttonDownTemplate
            // 
            this.buttonDownTemplate.Font = new System.Drawing.Font("Wingdings", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonDownTemplate.Location = new System.Drawing.Point(346, 190);
            this.buttonDownTemplate.Name = "buttonDownTemplate";
            this.buttonDownTemplate.Size = new System.Drawing.Size(22, 23);
            this.buttonDownTemplate.TabIndex = 18;
            this.buttonDownTemplate.Text = "ò";
            this.buttonDownTemplate.UseVisualStyleBackColor = true;
            this.buttonDownTemplate.Click += new System.EventHandler(this.buttonDownTemplate_Click);
            // 
            // buttonSaveTemplate
            // 
            this.buttonSaveTemplate.Enabled = false;
            this.buttonSaveTemplate.Location = new System.Drawing.Point(616, 353);
            this.buttonSaveTemplate.Name = "buttonSaveTemplate";
            this.buttonSaveTemplate.Size = new System.Drawing.Size(96, 23);
            this.buttonSaveTemplate.TabIndex = 19;
            this.buttonSaveTemplate.Text = "&Speichern";
            this.buttonSaveTemplate.UseVisualStyleBackColor = true;
            this.buttonSaveTemplate.Click += new System.EventHandler(this.buttonSaveTemplate_Click);
            // 
            // buttonAddTemplate
            // 
            this.buttonAddTemplate.Location = new System.Drawing.Point(12, 291);
            this.buttonAddTemplate.Name = "buttonAddTemplate";
            this.buttonAddTemplate.Size = new System.Drawing.Size(75, 23);
            this.buttonAddTemplate.TabIndex = 20;
            this.buttonAddTemplate.Text = "&Hinzufügen";
            this.buttonAddTemplate.UseVisualStyleBackColor = true;
            this.buttonAddTemplate.Click += new System.EventHandler(this.buttonAddTemplate_Click);
            // 
            // buttonDeleteTemplate
            // 
            this.buttonDeleteTemplate.Enabled = false;
            this.buttonDeleteTemplate.Location = new System.Drawing.Point(93, 290);
            this.buttonDeleteTemplate.Name = "buttonDeleteTemplate";
            this.buttonDeleteTemplate.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteTemplate.TabIndex = 21;
            this.buttonDeleteTemplate.Text = "&Löschen";
            this.buttonDeleteTemplate.UseVisualStyleBackColor = true;
            this.buttonDeleteTemplate.Click += new System.EventHandler(this.buttonDeleteTemplate_Click);
            // 
            // buttonResetDefault
            // 
            this.buttonResetDefault.Location = new System.Drawing.Point(189, 290);
            this.buttonResetDefault.Name = "buttonResetDefault";
            this.buttonResetDefault.Size = new System.Drawing.Size(151, 23);
            this.buttonResetDefault.TabIndex = 22;
            this.buttonResetDefault.Text = "Standard &Wiederherstellen";
            this.buttonResetDefault.UseVisualStyleBackColor = true;
            this.buttonResetDefault.Click += new System.EventHandler(this.buttonResetDefault_Click);
            // 
            // RecordFreeNameFormat
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Big3.Hitbase.RecordMedium.Images.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(726, 481);
            this.Controls.Add(this.buttonResetDefault);
            this.Controls.Add(this.buttonDeleteTemplate);
            this.Controls.Add(this.buttonAddTemplate);
            this.Controls.Add(this.buttonSaveTemplate);
            this.Controls.Add(this.buttonDownTemplate);
            this.Controls.Add(this.buttonUpTemplate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.domainUpDown1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listAvailableTrackFields);
            this.Controls.Add(this.comboBoxFelder);
            this.Controls.Add(this.listAvailableFields);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listTemplates);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textExample);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textFormatFile);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordFreeNameFormat";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Freies Format für Dateinamen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textFormatFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textExample;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listTemplates;
        private System.Windows.Forms.ListBox listAvailableFields;
        private System.Windows.Forms.ComboBox comboBoxFelder;
        private System.Windows.Forms.ListBox listAvailableTrackFields;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DomainUpDown domainUpDown1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonUpTemplate;
        private System.Windows.Forms.Button buttonDownTemplate;
        private System.Windows.Forms.Button buttonSaveTemplate;
        private System.Windows.Forms.Button buttonAddTemplate;
        private System.Windows.Forms.Button buttonDeleteTemplate;
        private System.Windows.Forms.Button buttonResetDefault;
    }
}