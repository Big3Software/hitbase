namespace Big3.Hitbase.RecordMedium
{
    partial class RecordSelectTracks
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.listTracks = new System.Windows.Forms.ListView();
            this.columnTrackNumber = new System.Windows.Forms.ColumnHeader();
            this.columnTrackname = new System.Windows.Forms.ColumnHeader();
            this.listSelectedTracks = new System.Windows.Forms.ListView();
            this.columnSelectedNumber = new System.Windows.Forms.ColumnHeader();
            this.columnSelectedTrackName = new System.Windows.Forms.ColumnHeader();
            this.buttonMove = new System.Windows.Forms.Button();
            this.buttonMoveAll = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonRemoveAll = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(203, 499);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAbort.Location = new System.Drawing.Point(311, 499);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonAbort.TabIndex = 2;
            this.buttonAbort.Text = "A&bbrechen";
            this.buttonAbort.UseVisualStyleBackColor = true;
            // 
            // listTracks
            // 
            this.listTracks.AllowDrop = true;
            this.listTracks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnTrackNumber,
            this.columnTrackname});
            this.listTracks.FullRowSelect = true;
            this.listTracks.GridLines = true;
            this.listTracks.Location = new System.Drawing.Point(12, 61);
            this.listTracks.Name = "listTracks";
            this.listTracks.Size = new System.Drawing.Size(250, 432);
            this.listTracks.TabIndex = 3;
            this.listTracks.UseCompatibleStateImageBehavior = false;
            this.listTracks.View = System.Windows.Forms.View.Details;
            this.listTracks.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listTracks.DoubleClick += new System.EventHandler(this.buttonMove_Click);
            // 
            // columnTrackNumber
            // 
            this.columnTrackNumber.Text = "Nr.";
            this.columnTrackNumber.Width = 30;
            // 
            // columnTrackname
            // 
            this.columnTrackname.Text = "Titel";
            this.columnTrackname.Width = 210;
            // 
            // listSelectedTracks
            // 
            this.listSelectedTracks.AllowDrop = true;
            this.listSelectedTracks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSelectedNumber,
            this.columnSelectedTrackName});
            this.listSelectedTracks.FullRowSelect = true;
            this.listSelectedTracks.GridLines = true;
            this.listSelectedTracks.HideSelection = false;
            this.listSelectedTracks.Location = new System.Drawing.Point(301, 61);
            this.listSelectedTracks.Name = "listSelectedTracks";
            this.listSelectedTracks.Size = new System.Drawing.Size(250, 432);
            this.listSelectedTracks.TabIndex = 4;
            this.listSelectedTracks.UseCompatibleStateImageBehavior = false;
            this.listSelectedTracks.View = System.Windows.Forms.View.Details;
            this.listSelectedTracks.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listSelectedTracks.DoubleClick += new System.EventHandler(this.buttonRemove_Click);
            // 
            // columnSelectedNumber
            // 
            this.columnSelectedNumber.Text = "Nr.";
            this.columnSelectedNumber.Width = 30;
            // 
            // columnSelectedTrackName
            // 
            this.columnSelectedTrackName.Text = "Titel";
            this.columnSelectedTrackName.Width = 210;
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(268, 200);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(27, 23);
            this.buttonMove.TabIndex = 5;
            this.buttonMove.Text = ">";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // buttonMoveAll
            // 
            this.buttonMoveAll.Location = new System.Drawing.Point(268, 240);
            this.buttonMoveAll.Name = "buttonMoveAll";
            this.buttonMoveAll.Size = new System.Drawing.Size(27, 23);
            this.buttonMoveAll.TabIndex = 6;
            this.buttonMoveAll.Text = ">>";
            this.buttonMoveAll.UseVisualStyleBackColor = true;
            this.buttonMoveAll.Click += new System.EventHandler(this.buttonMoveAll_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(268, 280);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(27, 23);
            this.buttonRemove.TabIndex = 7;
            this.buttonRemove.Text = "<";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonRemoveAll
            // 
            this.buttonRemoveAll.Location = new System.Drawing.Point(268, 320);
            this.buttonRemoveAll.Name = "buttonRemoveAll";
            this.buttonRemoveAll.Size = new System.Drawing.Size(27, 23);
            this.buttonRemoveAll.TabIndex = 8;
            this.buttonRemoveAll.Text = "<<";
            this.buttonRemoveAll.UseVisualStyleBackColor = true;
            this.buttonRemoveAll.Click += new System.EventHandler(this.buttonRemoveAll_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonUp.Location = new System.Drawing.Point(557, 240);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(21, 32);
            this.buttonUp.TabIndex = 9;
            this.buttonUp.Text = "ñ";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonDown.Location = new System.Drawing.Point(557, 278);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(21, 32);
            this.buttonDown.TabIndex = 10;
            this.buttonDown.Text = "ò";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Verfügbare Tracks auf Medium";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(301, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Ausgewählte Tracks für die Aufnahme";
            // 
            // RecordSelectTracks
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAbort;
            this.ClientSize = new System.Drawing.Size(589, 534);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonRemoveAll);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonMoveAll);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.listSelectedTracks);
            this.Controls.Add(this.listTracks);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.buttonOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordSelectTracks";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Verfügbare Tracks";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.ListView listTracks;
        private System.Windows.Forms.ColumnHeader columnTrackNumber;
        private System.Windows.Forms.ColumnHeader columnTrackname;
        private System.Windows.Forms.ListView listSelectedTracks;
        private System.Windows.Forms.ColumnHeader columnSelectedNumber;
        private System.Windows.Forms.ColumnHeader columnSelectedTrackName;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.Button buttonMoveAll;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonRemoveAll;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}