namespace Big3.Hitbase.CDUtilities
{
    partial class FormWishlistReminder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWishlistReminder));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelWishTitle = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.listViewWishlistItems = new System.Windows.Forms.ListView();
            this.columnHeaderArtist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFrom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonPlayNext = new System.Windows.Forms.Button();
            this.buttonPlayImmediatly = new System.Windows.Forms.Button();
            this.buttonOpenItem = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonRemindAgain = new System.Windows.Forms.Button();
            this.comboBoxRemindAgain = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelWishedBy = new System.Windows.Forms.Label();
            this.labelWishedAt = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::Big3.Hitbase.CDUtilities.Images.AddToWishlist;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelWishTitle
            // 
            this.labelWishTitle.AutoSize = true;
            this.labelWishTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelWishTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWishTitle.Location = new System.Drawing.Point(79, 12);
            this.labelWishTitle.Name = "labelWishTitle";
            this.labelWishTitle.Size = new System.Drawing.Size(19, 13);
            this.labelWishTitle.TabIndex = 1;
            this.labelWishTitle.Text = "...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(79, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Gewünscht von:";
            // 
            // listViewWishlistItems
            // 
            this.listViewWishlistItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewWishlistItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderArtist,
            this.columnHeaderTitle,
            this.columnHeaderFrom});
            this.listViewWishlistItems.FullRowSelect = true;
            this.listViewWishlistItems.HideSelection = false;
            this.listViewWishlistItems.Location = new System.Drawing.Point(12, 80);
            this.listViewWishlistItems.Name = "listViewWishlistItems";
            this.listViewWishlistItems.Size = new System.Drawing.Size(504, 118);
            this.listViewWishlistItems.TabIndex = 3;
            this.listViewWishlistItems.UseCompatibleStateImageBehavior = false;
            this.listViewWishlistItems.View = System.Windows.Forms.View.Details;
            this.listViewWishlistItems.SelectedIndexChanged += new System.EventHandler(this.listViewWishlistItems_SelectedIndexChanged);
            // 
            // columnHeaderArtist
            // 
            this.columnHeaderArtist.Text = "Interpret";
            this.columnHeaderArtist.Width = 200;
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Titel";
            this.columnHeaderTitle.Width = 200;
            // 
            // columnHeaderFrom
            // 
            this.columnHeaderFrom.Text = "Von";
            this.columnHeaderFrom.Width = 100;
            // 
            // buttonPlayNext
            // 
            this.buttonPlayNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlayNext.Location = new System.Drawing.Point(12, 204);
            this.buttonPlayNext.Name = "buttonPlayNext";
            this.buttonPlayNext.Size = new System.Drawing.Size(118, 23);
            this.buttonPlayNext.TabIndex = 4;
            this.buttonPlayNext.Text = "Als nächstes spielen";
            this.buttonPlayNext.UseVisualStyleBackColor = true;
            this.buttonPlayNext.Click += new System.EventHandler(this.buttonPlayNext_Click);
            // 
            // buttonPlayImmediatly
            // 
            this.buttonPlayImmediatly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlayImmediatly.Location = new System.Drawing.Point(136, 204);
            this.buttonPlayImmediatly.Name = "buttonPlayImmediatly";
            this.buttonPlayImmediatly.Size = new System.Drawing.Size(94, 23);
            this.buttonPlayImmediatly.TabIndex = 5;
            this.buttonPlayImmediatly.Text = "Sofort spielen";
            this.buttonPlayImmediatly.UseVisualStyleBackColor = true;
            this.buttonPlayImmediatly.Click += new System.EventHandler(this.buttonPlayImmediatly_Click);
            // 
            // buttonOpenItem
            // 
            this.buttonOpenItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenItem.Location = new System.Drawing.Point(338, 204);
            this.buttonOpenItem.Name = "buttonOpenItem";
            this.buttonOpenItem.Size = new System.Drawing.Size(97, 23);
            this.buttonOpenItem.TabIndex = 6;
            this.buttonOpenItem.Text = "Element öffnen";
            this.buttonOpenItem.UseVisualStyleBackColor = true;
            this.buttonOpenItem.Click += new System.EventHandler(this.buttonOpenItem_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(441, 204);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "Schließen";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonRemindAgain
            // 
            this.buttonRemindAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemindAgain.Location = new System.Drawing.Point(405, 276);
            this.buttonRemindAgain.Name = "buttonRemindAgain";
            this.buttonRemindAgain.Size = new System.Drawing.Size(111, 23);
            this.buttonRemindAgain.TabIndex = 8;
            this.buttonRemindAgain.Text = "Erneut erinnern";
            this.buttonRemindAgain.UseVisualStyleBackColor = true;
            this.buttonRemindAgain.Click += new System.EventHandler(this.buttonRemindAgain_Click);
            // 
            // comboBoxRemindAgain
            // 
            this.comboBoxRemindAgain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRemindAgain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRemindAgain.FormattingEnabled = true;
            this.comboBoxRemindAgain.Location = new System.Drawing.Point(12, 277);
            this.comboBoxRemindAgain.Name = "comboBoxRemindAgain";
            this.comboBoxRemindAgain.Size = new System.Drawing.Size(387, 21);
            this.comboBoxRemindAgain.TabIndex = 9;
            this.comboBoxRemindAgain.SelectedIndexChanged += new System.EventHandler(this.comboBoxRemindAgain_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(12, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(387, 31);
            this.label3.TabIndex = 10;
            this.label3.Text = "&Klicken Sie auf \"Erneut erinnern\", um nach Ablauf des unten gewählten Zeitraums " +
    "erneut erinnert zu werden.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(79, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "um:";
            // 
            // labelWishedBy
            // 
            this.labelWishedBy.AutoSize = true;
            this.labelWishedBy.BackColor = System.Drawing.Color.Transparent;
            this.labelWishedBy.Location = new System.Drawing.Point(170, 36);
            this.labelWishedBy.Name = "labelWishedBy";
            this.labelWishedBy.Size = new System.Drawing.Size(16, 13);
            this.labelWishedBy.TabIndex = 12;
            this.labelWishedBy.Text = "...";
            // 
            // labelWishedAt
            // 
            this.labelWishedAt.AutoSize = true;
            this.labelWishedAt.BackColor = System.Drawing.Color.Transparent;
            this.labelWishedAt.Location = new System.Drawing.Point(170, 54);
            this.labelWishedAt.Name = "labelWishedAt";
            this.labelWishedAt.Size = new System.Drawing.Size(16, 13);
            this.labelWishedAt.TabIndex = 13;
            this.labelWishedAt.Text = "...";
            // 
            // FormWishlistReminder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(528, 310);
            this.Controls.Add(this.labelWishedAt);
            this.Controls.Add(this.labelWishedBy);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxRemindAgain);
            this.Controls.Add(this.buttonRemindAgain);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonOpenItem);
            this.Controls.Add(this.buttonPlayImmediatly);
            this.Controls.Add(this.buttonPlayNext);
            this.Controls.Add(this.listViewWishlistItems);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelWishTitle);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(544, 346);
            this.Name = "FormWishlistReminder";
            this.Text = "Erinnerung";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelWishTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView listViewWishlistItems;
        private System.Windows.Forms.Button buttonPlayNext;
        private System.Windows.Forms.Button buttonPlayImmediatly;
        private System.Windows.Forms.Button buttonOpenItem;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonRemindAgain;
        private System.Windows.Forms.ColumnHeader columnHeaderArtist;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderFrom;
        private System.Windows.Forms.ComboBox comboBoxRemindAgain;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelWishedBy;
        private System.Windows.Forms.Label labelWishedAt;
    }
}