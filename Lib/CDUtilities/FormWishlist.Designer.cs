namespace Big3.Hitbase.CDUtilities
{
    partial class FormWishlist
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonPlayNext = new System.Windows.Forms.Button();
            this.buttonPlayImmediatly = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.listViewWishlistItems = new System.Windows.Forms.ListView();
            this.columnHeaderArtist = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTitle = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderFrom = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderComment = new System.Windows.Forms.ColumnHeader();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(64, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(487, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "Folgende Titel sind in der Wunschliste eingetragen. Um weitere Titel hinzuzufügen" +
                ", wechseln Sie in das Katalog-Fenster, markieren Sie die gewünschten Titel und w" +
                "ählen Sie \"Zur Wunschliste hinzufügen\".";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Big3.Hitbase.CDUtilities.Images.AddToWishlist;
            this.pictureBox1.Location = new System.Drawing.Point(12, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // buttonProperties
            // 
            this.buttonProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProperties.Location = new System.Drawing.Point(426, 93);
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.Size = new System.Drawing.Size(125, 23);
            this.buttonProperties.TabIndex = 2;
            this.buttonProperties.Text = "Eigenschaften...";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Location = new System.Drawing.Point(426, 122);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(125, 23);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "Löschen";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonPlayNext
            // 
            this.buttonPlayNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlayNext.Location = new System.Drawing.Point(426, 151);
            this.buttonPlayNext.Name = "buttonPlayNext";
            this.buttonPlayNext.Size = new System.Drawing.Size(125, 23);
            this.buttonPlayNext.TabIndex = 4;
            this.buttonPlayNext.Text = "Als nächstes spielen";
            this.buttonPlayNext.UseVisualStyleBackColor = true;
            this.buttonPlayNext.Click += new System.EventHandler(this.buttonPlayNext_Click);
            // 
            // buttonPlayImmediatly
            // 
            this.buttonPlayImmediatly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlayImmediatly.Location = new System.Drawing.Point(426, 180);
            this.buttonPlayImmediatly.Name = "buttonPlayImmediatly";
            this.buttonPlayImmediatly.Size = new System.Drawing.Size(125, 23);
            this.buttonPlayImmediatly.TabIndex = 5;
            this.buttonPlayImmediatly.Text = "Sofort spielen";
            this.buttonPlayImmediatly.UseVisualStyleBackColor = true;
            this.buttonPlayImmediatly.Click += new System.EventHandler(this.buttonPlayImmediatly_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(426, 400);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(125, 23);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Schließen";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listViewWishlistItems
            // 
            this.listViewWishlistItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewWishlistItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderArtist,
            this.columnHeaderTitle,
            this.columnHeaderFrom,
            this.columnHeaderComment});
            this.listViewWishlistItems.FullRowSelect = true;
            this.listViewWishlistItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewWishlistItems.HideSelection = false;
            this.listViewWishlistItems.Location = new System.Drawing.Point(12, 93);
            this.listViewWishlistItems.Name = "listViewWishlistItems";
            this.listViewWishlistItems.Size = new System.Drawing.Size(408, 330);
            this.listViewWishlistItems.TabIndex = 7;
            this.listViewWishlistItems.UseCompatibleStateImageBehavior = false;
            this.listViewWishlistItems.View = System.Windows.Forms.View.Details;
            this.listViewWishlistItems.SelectedIndexChanged += new System.EventHandler(this.listViewWishlistItems_SelectedIndexChanged);
            this.listViewWishlistItems.DoubleClick += new System.EventHandler(this.listViewWishlistItems_DoubleClick);
            // 
            // columnHeaderArtist
            // 
            this.columnHeaderArtist.Text = "Interpret";
            this.columnHeaderArtist.Width = 100;
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Titel";
            this.columnHeaderTitle.Width = 100;
            // 
            // columnHeaderFrom
            // 
            this.columnHeaderFrom.Text = "Von";
            this.columnHeaderFrom.Width = 100;
            // 
            // columnHeaderComment
            // 
            this.columnHeaderComment.Text = "Kommentar";
            this.columnHeaderComment.Width = 100;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(426, 209);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(125, 23);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Speichern...";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrint.Location = new System.Drawing.Point(426, 238);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(125, 23);
            this.buttonPrint.TabIndex = 9;
            this.buttonPrint.Text = "Drucken...";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // FormWishlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(563, 435);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.listViewWishlistItems);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonPlayImmediatly);
            this.Controls.Add(this.buttonPlayNext);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonProperties);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(579, 400);
            this.Name = "FormWishlist";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wunschliste";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonPlayNext;
        private System.Windows.Forms.Button buttonPlayImmediatly;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListView listViewWishlistItems;
        private System.Windows.Forms.ColumnHeader columnHeaderArtist;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderFrom;
        private System.Windows.Forms.ColumnHeader columnHeaderComment;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonPrint;
    }
}