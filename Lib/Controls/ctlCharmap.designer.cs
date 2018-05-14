namespace Big3.Hitbase.Controls
{
    partial class ctlCharmap
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            this.mnuSingleCharContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.diesesZeichenInDieZwischenablageKopierenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctlDataGrid = new System.Windows.Forms.DataGridView();
            this.mnuSingleCharContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuSingleCharContext
            // 
            this.mnuSingleCharContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.diesesZeichenInDieZwischenablageKopierenToolStripMenuItem});
            this.mnuSingleCharContext.Name = "mnuSingleCharContext";
            this.mnuSingleCharContext.Size = new System.Drawing.Size(308, 26);
            // 
            // diesesZeichenInDieZwischenablageKopierenToolStripMenuItem
            // 
            this.diesesZeichenInDieZwischenablageKopierenToolStripMenuItem.Name = "diesesZeichenInDieZwischenablageKopierenToolStripMenuItem";
            this.diesesZeichenInDieZwischenablageKopierenToolStripMenuItem.Size = new System.Drawing.Size(307, 22);
            this.diesesZeichenInDieZwischenablageKopierenToolStripMenuItem.Text = "Dieses Zeichen in die Zwischenablage kopieren";
            // 
            // ctlDataGrid
            // 
            this.ctlDataGrid.Location = new System.Drawing.Point(0, 0);
            this.ctlDataGrid.Name = "ctlDataGrid";
            this.ctlDataGrid.Size = new System.Drawing.Size(240, 150);
            this.ctlDataGrid.TabIndex = 1;
            this.ctlDataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ctlDataGrid_CellClick);
            // 
            // ctlCharmap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.ctlDataGrid);
            this.Name = "ctlCharmap";
            this.Size = new System.Drawing.Size(309, 295);
            this.Load += new System.EventHandler(this.ctlCharmap_Load);
            this.mnuSingleCharContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ctlDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip mnuSingleCharContext;
        private System.Windows.Forms.ToolStripMenuItem diesesZeichenInDieZwischenablageKopierenToolStripMenuItem;
        private System.Windows.Forms.DataGridView ctlDataGrid;

        private System.Windows.Forms.DataGridViewTextBoxColumn[] Columns;
    }
}
