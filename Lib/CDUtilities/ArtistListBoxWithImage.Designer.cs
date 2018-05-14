namespace Big3.Hitbase.CDUtilities
{
    partial class ArtistListBoxWithImage
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
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Black;
            // 
            // ArtistListBoxWithImage
            // 
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ItemHeight = 80;
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ArtistListBoxWithImage_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ArtistListBoxWithImage_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ImageList imageList;
    }
}
