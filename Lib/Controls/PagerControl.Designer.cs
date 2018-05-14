namespace Big3.Hitbase.Controls
{
    partial class PagerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabelFirst = new System.Windows.Forms.LinkLabel();
            this.linkLabelPrevious = new System.Windows.Forms.LinkLabel();
            this.linkLabelNext = new System.Windows.Forms.LinkLabel();
            this.linkLabelLast = new System.Windows.Forms.LinkLabel();
            this.panelPages = new System.Windows.Forms.Panel();
            this.labelPages = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.linkLabelFirst, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelPrevious, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelNext, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelLast, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelPages, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelPages, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 20);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // linkLabelFirst
            // 
            this.linkLabelFirst.AutoSize = true;
            this.linkLabelFirst.Location = new System.Drawing.Point(175, 0);
            this.linkLabelFirst.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.linkLabelFirst.Name = "linkLabelFirst";
            this.linkLabelFirst.Size = new System.Drawing.Size(27, 20);
            this.linkLabelFirst.TabIndex = 4;
            this.linkLabelFirst.TabStop = true;
            this.linkLabelFirst.Text = "<<";
            this.linkLabelFirst.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFirst_LinkClicked);
            // 
            // linkLabelPrevious
            // 
            this.linkLabelPrevious.AutoSize = true;
            this.linkLabelPrevious.Location = new System.Drawing.Point(212, 0);
            this.linkLabelPrevious.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.linkLabelPrevious.Name = "linkLabelPrevious";
            this.linkLabelPrevious.Size = new System.Drawing.Size(18, 20);
            this.linkLabelPrevious.TabIndex = 5;
            this.linkLabelPrevious.TabStop = true;
            this.linkLabelPrevious.Text = "<";
            this.linkLabelPrevious.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPrevious_LinkClicked);
            // 
            // linkLabelNext
            // 
            this.linkLabelNext.AutoSize = true;
            this.linkLabelNext.Location = new System.Drawing.Point(240, 0);
            this.linkLabelNext.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.linkLabelNext.Name = "linkLabelNext";
            this.linkLabelNext.Size = new System.Drawing.Size(18, 20);
            this.linkLabelNext.TabIndex = 6;
            this.linkLabelNext.TabStop = true;
            this.linkLabelNext.Text = ">";
            this.linkLabelNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelNext_LinkClicked);
            // 
            // linkLabelLast
            // 
            this.linkLabelLast.AutoSize = true;
            this.linkLabelLast.Location = new System.Drawing.Point(268, 0);
            this.linkLabelLast.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.linkLabelLast.Name = "linkLabelLast";
            this.linkLabelLast.Size = new System.Drawing.Size(27, 20);
            this.linkLabelLast.TabIndex = 7;
            this.linkLabelLast.TabStop = true;
            this.linkLabelLast.Text = ">>";
            this.linkLabelLast.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLast_LinkClicked);
            // 
            // panelPages
            // 
            this.panelPages.AutoSize = true;
            this.panelPages.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelPages.Location = new System.Drawing.Point(235, 0);
            this.panelPages.Margin = new System.Windows.Forms.Padding(0);
            this.panelPages.Name = "panelPages";
            this.panelPages.Size = new System.Drawing.Size(0, 0);
            this.panelPages.TabIndex = 8;
            // 
            // labelPages
            // 
            this.labelPages.AutoSize = true;
            this.labelPages.Location = new System.Drawing.Point(4, 0);
            this.labelPages.Margin = new System.Windows.Forms.Padding(4, 0, 45, 0);
            this.labelPages.Name = "labelPages";
            this.labelPages.Size = new System.Drawing.Size(121, 20);
            this.labelPages.TabIndex = 9;
            this.labelPages.Text = "Seite {0} von {1}";
            // 
            // PagerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PagerControl";
            this.Size = new System.Drawing.Size(300, 20);
            this.Load += new System.EventHandler(this.PagerControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel linkLabelFirst;
        private System.Windows.Forms.LinkLabel linkLabelPrevious;
        private System.Windows.Forms.LinkLabel linkLabelNext;
        private System.Windows.Forms.LinkLabel linkLabelLast;
        private System.Windows.Forms.Panel panelPages;
        private System.Windows.Forms.Label labelPages;

    }
}
