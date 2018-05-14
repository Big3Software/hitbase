using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.RecordMedium
{
    public partial class ExternalEncoder : Form
    {
        public ExternalEncoder()
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            textExternalProg.Text = Settings.Current.RecordUser1ExeParameter;
        }
        int externProgNum = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectfile = new OpenFileDialog();
            selectfile.Filter = "EXE Dateien (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
            selectfile.Title = "Bitte wählen Sie ein externes Programm";
            selectfile.ShowDialog();
            textExternalProg.Text = selectfile.FileName;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Settings.Current.RecordUser1ExeParameter = textExternalProg.Text;
        }
    }
}
