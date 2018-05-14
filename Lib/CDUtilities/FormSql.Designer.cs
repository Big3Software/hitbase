namespace Big3.Hitbase.CDUtilities
{
    partial class FormSql
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
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder2 = new XPTable.Models.DataSourceColumnBinder();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSql = new System.Windows.Forms.TextBox();
            this.buttonExecuteSql = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tableResult = new XPTable.Models.Table();
            this.buttonClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tableResult)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL-Abfrage:";
            // 
            // textBoxSql
            // 
            this.textBoxSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSql.Location = new System.Drawing.Point(15, 37);
            this.textBoxSql.Multiline = true;
            this.textBoxSql.Name = "textBoxSql";
            this.textBoxSql.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSql.Size = new System.Drawing.Size(587, 60);
            this.textBoxSql.TabIndex = 1;
            // 
            // buttonExecuteSql
            // 
            this.buttonExecuteSql.Location = new System.Drawing.Point(15, 103);
            this.buttonExecuteSql.Name = "buttonExecuteSql";
            this.buttonExecuteSql.Size = new System.Drawing.Size(99, 23);
            this.buttonExecuteSql.TabIndex = 2;
            this.buttonExecuteSql.Text = "&Abfrage starten";
            this.buttonExecuteSql.UseVisualStyleBackColor = true;
            this.buttonExecuteSql.Click += new System.EventHandler(this.buttonExecuteSql_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ergebnis:";
            // 
            // tableResult
            // 
            this.tableResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableResult.BorderColor = System.Drawing.Color.Black;
            this.tableResult.DataMember = null;
            this.tableResult.DataSourceColumnBinder = dataSourceColumnBinder2;
            this.tableResult.Location = new System.Drawing.Point(15, 155);
            this.tableResult.Name = "tableResult";
            this.tableResult.Size = new System.Drawing.Size(587, 296);
            this.tableResult.TabIndex = 4;
            this.tableResult.Text = "table1";
            this.tableResult.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(527, 457);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Schließen";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // FormSql
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(614, 492);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tableResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonExecuteSql);
            this.Controls.Add(this.textBoxSql);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "FormSql";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQL-Abfrage";
            ((System.ComponentModel.ISupportInitialize)(this.tableResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSql;
        private System.Windows.Forms.Button buttonExecuteSql;
        private System.Windows.Forms.Label label2;
        private XPTable.Models.Table tableResult;
        private System.Windows.Forms.Button buttonClose;
    }
}