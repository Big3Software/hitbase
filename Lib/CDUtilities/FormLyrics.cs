using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormLyrics : Form
    {
        public FormLyrics()
        {
            InitializeComponent();
        }

        public string Lyrics
        {
            set
            {
                textBoxLyrics.Text = value;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
