namespace Big3.Hitbase.CDUtilities
{
    partial class FormSearchAmazon
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxArtist = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonTransfer = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSort = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxEAN = new System.Windows.Forms.TextBox();
            this.labelEAN = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.checkBoxEditBeforeSave = new System.Windows.Forms.CheckBox();
            this.pagerControl = new Big3.Hitbase.Controls.PagerControl();
            this.amazonProgressControl = new Big3.Hitbase.CDUtilities.AmazonProgressControl();
            this.listBoxResult = new Big3.Hitbase.CDUtilities.CDListBoxWithCover(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Interpret:";
            // 
            // textBoxArtist
            // 
            this.textBoxArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxArtist.Location = new System.Drawing.Point(80, 17);
            this.textBoxArtist.Name = "textBoxArtist";
            this.textBoxArtist.Size = new System.Drawing.Size(258, 20);
            this.textBoxArtist.TabIndex = 1;
            this.textBoxArtist.TextChanged += new System.EventHandler(this.textBoxArtist_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Titel:";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTitle.Location = new System.Drawing.Point(39, 17);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(298, 20);
            this.textBoxTitle.TabIndex = 1;
            this.textBoxTitle.TextChanged += new System.EventHandler(this.textBoxTitle_TextChanged);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Location = new System.Drawing.Point(230, 77);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(107, 23);
            this.buttonSearch.TabIndex = 4;
            this.buttonSearch.Text = "Suche starten";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTransfer.Location = new System.Drawing.Point(432, 487);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(184, 23);
            this.buttonTransfer.TabIndex = 8;
            this.buttonTransfer.Text = "Ausgewählte CD(s) übernehmen";
            this.buttonTransfer.UseVisualStyleBackColor = true;
            this.buttonTransfer.Click += new System.EventHandler(this.buttonTransfer_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(622, 487);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Schließen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(12, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Suchergebnis:";
            // 
            // comboBoxSort
            // 
            this.comboBoxSort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSort.FormattingEnabled = true;
            this.comboBoxSort.Location = new System.Drawing.Point(80, 48);
            this.comboBoxSort.Name = "comboBoxSort";
            this.comboBoxSort.Size = new System.Drawing.Size(258, 21);
            this.comboBoxSort.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Sortiert nach:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(15, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxSort);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxArtist);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxEAN);
            this.splitContainer1.Panel2.Controls.Add(this.labelEAN);
            this.splitContainer1.Panel2.Controls.Add(this.buttonSearch);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxTitle);
            this.splitContainer1.Size = new System.Drawing.Size(682, 102);
            this.splitContainer1.SplitterDistance = 341;
            this.splitContainer1.TabIndex = 0;
            // 
            // textBoxEAN
            // 
            this.textBoxEAN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEAN.Location = new System.Drawing.Point(39, 48);
            this.textBoxEAN.Name = "textBoxEAN";
            this.textBoxEAN.Size = new System.Drawing.Size(298, 20);
            this.textBoxEAN.TabIndex = 3;
            this.textBoxEAN.TextChanged += new System.EventHandler(this.textBoxEAN_TextChanged);
            // 
            // labelEAN
            // 
            this.labelEAN.AutoSize = true;
            this.labelEAN.Location = new System.Drawing.Point(3, 51);
            this.labelEAN.Name = "labelEAN";
            this.labelEAN.Size = new System.Drawing.Size(32, 13);
            this.labelEAN.TabIndex = 2;
            this.labelEAN.Text = "EAN:";
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResult.BackColor = System.Drawing.Color.Transparent;
            this.labelResult.Location = new System.Drawing.Point(472, 108);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(225, 13);
            this.labelResult.TabIndex = 15;
            this.labelResult.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // checkBoxEditBeforeSave
            // 
            this.checkBoxEditBeforeSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxEditBeforeSave.AutoSize = true;
            this.checkBoxEditBeforeSave.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxEditBeforeSave.Location = new System.Drawing.Point(15, 445);
            this.checkBoxEditBeforeSave.Name = "checkBoxEditBeforeSave";
            this.checkBoxEditBeforeSave.Size = new System.Drawing.Size(194, 17);
            this.checkBoxEditBeforeSave.TabIndex = 17;
            this.checkBoxEditBeforeSave.Text = "Vor dem Speichern Daten anzeigen";
            this.checkBoxEditBeforeSave.UseVisualStyleBackColor = false;
            // 
            // pagerControl
            // 
            this.pagerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pagerControl.AutoSize = true;
            this.pagerControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pagerControl.BackColor = System.Drawing.Color.Transparent;
            this.pagerControl.CurrentPage = 0;
            this.pagerControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pagerControl.Location = new System.Drawing.Point(417, 444);
            this.pagerControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pagerControl.Name = "pagerControl";
            this.pagerControl.NumberOfPages = 0;
            this.pagerControl.ShowPageNumberCount = 6;
            this.pagerControl.Size = new System.Drawing.Size(280, 20);
            this.pagerControl.TabIndex = 16;
            this.pagerControl.Visible = false;
            this.pagerControl.PageChanged += new Big3.Hitbase.Controls.PagerControl.PageChangedDelegate(this.pagerControl_PageChanged);
            // 
            // amazonProgressControl
            // 
            this.amazonProgressControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.amazonProgressControl.AutoSize = true;
            this.amazonProgressControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.amazonProgressControl.BackColor = System.Drawing.Color.White;
            this.amazonProgressControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.amazonProgressControl.Location = new System.Drawing.Point(273, 242);
            this.amazonProgressControl.Name = "amazonProgressControl";
            this.amazonProgressControl.Padding = new System.Windows.Forms.Padding(3);
            this.amazonProgressControl.Size = new System.Drawing.Size(163, 78);
            this.amazonProgressControl.TabIndex = 11;
            this.amazonProgressControl.Visible = false;
            // 
            // listBoxResult
            // 
            this.listBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxResult.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxResult.FormattingEnabled = true;
            this.listBoxResult.IntegralHeight = false;
            this.listBoxResult.ItemHeight = 80;
            this.listBoxResult.Location = new System.Drawing.Point(15, 124);
            this.listBoxResult.Name = "listBoxResult";
            this.listBoxResult.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxResult.Size = new System.Drawing.Size(682, 315);
            this.listBoxResult.TabIndex = 7;
            this.listBoxResult.SelectedIndexChanged += new System.EventHandler(this.listBoxResult_SelectedIndexChanged);
            this.listBoxResult.SizeChanged += new System.EventHandler(this.listBoxResult_SizeChanged);
            // 
            // FormSearchAmazon
            // 
            this.AcceptButton = this.buttonSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(709, 522);
            this.Controls.Add(this.checkBoxEditBeforeSave);
            this.Controls.Add(this.pagerControl);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.amazonProgressControl);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonTransfer);
            this.Controls.Add(this.listBoxResult);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "FormSearchAmazon";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Medieninformationen bei Amazon suchen";
            this.Load += new System.EventHandler(this.FormSearchAmazon_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxArtist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Button buttonSearch;
        private CDListBoxWithCover listBoxResult;
        private System.Windows.Forms.Button buttonTransfer;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private AmazonProgressControl amazonProgressControl;
        private System.Windows.Forms.ComboBox comboBoxSort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label labelResult;
        private Big3.Hitbase.Controls.PagerControl pagerControl;
        public System.Windows.Forms.CheckBox checkBoxEditBeforeSave;
        private System.Windows.Forms.TextBox textBoxEAN;
        private System.Windows.Forms.Label labelEAN;
    }
}