using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.MediaRipper.Mp3;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.RecordMedium
{
    public partial class MP3Settings : Form
    {
        public MP3Settings()
        {
            InitializeComponent();
        }

        private void editMp3Writer1_Load(object sender, EventArgs e)
        {
            
        }
        public Mp3WriterConfig MP3Config
        {
            get
            {
                return (Mp3WriterConfig)editMp3Writer1.Config;
            }
            set
            {
                editMp3Writer1.Config = value;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // TODO !!!!!!!!!!!
        }
    }
}
