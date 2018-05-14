namespace Big3.Hitbase.RecordMedium
{
    partial class RecordFormProgress
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
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.LabelProgress = new System.Windows.Forms.Label();
            this.ProgressBarAll = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.LabelProgressAll = new System.Windows.Forms.Label();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(12, 60);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(520, 18);
            this.ProgressBar.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(457, 194);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // LabelProgress
            // 
            this.LabelProgress.AutoSize = true;
            this.LabelProgress.Location = new System.Drawing.Point(12, 9);
            this.LabelProgress.Name = "LabelProgress";
            this.LabelProgress.Size = new System.Drawing.Size(107, 13);
            this.LabelProgress.TabIndex = 3;
            this.LabelProgress.Text = "Einen Moment bitte...";
            // 
            // ProgressBarAll
            // 
            this.ProgressBarAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBarAll.Location = new System.Drawing.Point(12, 149);
            this.ProgressBarAll.Name = "ProgressBarAll";
            this.ProgressBarAll.Size = new System.Drawing.Size(520, 18);
            this.ProgressBarAll.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Gesamtfortschritt:";
            // 
            // LabelProgressAll
            // 
            this.LabelProgressAll.AutoSize = true;
            this.LabelProgressAll.Location = new System.Drawing.Point(12, 119);
            this.LabelProgressAll.Name = "LabelProgressAll";
            this.LabelProgressAll.Size = new System.Drawing.Size(78, 13);
            this.LabelProgressAll.TabIndex = 6;
            this.LabelProgressAll.Text = "Track N von N";
            // 
            // labelSpeed
            // 
            this.labelSpeed.Location = new System.Drawing.Point(392, 119);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(140, 13);
            this.labelSpeed.TabIndex = 7;
            this.labelSpeed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // RecordFormProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(544, 229);
            this.Controls.Add(this.labelSpeed);
            this.Controls.Add(this.LabelProgressAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProgressBarAll);
            this.Controls.Add(this.LabelProgress);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.ProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RecordFormProgress";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Einen Moment bitte...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.ProgressBar ProgressBar;
        public System.Windows.Forms.Label LabelProgress;
        public System.Windows.Forms.ProgressBar ProgressBarAll;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label LabelProgressAll;
        public System.Windows.Forms.Label labelSpeed;
    }
}