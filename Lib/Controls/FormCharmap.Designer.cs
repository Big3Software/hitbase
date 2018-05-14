namespace Big3.Hitbase.Controls
{
    partial class FormCharmap
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
            this.ctlCharmap = new Big3.Hitbase.Controls.ctlCharmap();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ctlCharmap
            // 
            this.ctlCharmap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ctlCharmap.BackColor = System.Drawing.SystemColors.Control;
            this.ctlCharmap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctlCharmap.CharSelected = '\0';
            this.ctlCharmap.Location = new System.Drawing.Point(12, 10);
            this.ctlCharmap.Name = "ctlCharmap";
            this.ctlCharmap.Size = new System.Drawing.Size(315, 251);
            this.ctlCharmap.TabIndex = 0;
            // 
            // buttonPaste
            // 
            this.buttonPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPaste.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonPaste.Location = new System.Drawing.Point(252, 267);
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(75, 23);
            this.buttonPaste.TabIndex = 1;
            this.buttonPaste.Text = "Einfügen";
            this.buttonPaste.UseVisualStyleBackColor = true;
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // FormCharmap
            // 
            this.AcceptButton = this.buttonPaste;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 302);
            this.Controls.Add(this.buttonPaste);
            this.Controls.Add(this.ctlCharmap);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCharmap";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sonderzeichen einfügen";
            this.ResumeLayout(false);

        }

        #endregion

        private ctlCharmap ctlCharmap;
        private System.Windows.Forms.Button buttonPaste;
    }
}