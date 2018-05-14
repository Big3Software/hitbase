using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.Controls
{
    public partial class ShowPictureForm : Form
    {
        public ShowPictureForm()
        {
            InitializeComponent();

            Settings.RestoreWindowPlacement(this, "ShowPicture");
        }

        public Image Image
        {
            get
            {
                return pictureBox.Image;
            }
            set
            {
                pictureBox.Image = value;
            }
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            ClientSize = pictureBox.Image.Size;
        }

        private void ShowPictureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.SaveWindowPlacement(this, "ShowPicture");
        }
    }
}