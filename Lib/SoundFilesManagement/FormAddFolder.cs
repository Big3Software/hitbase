using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using XPTable.Models;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;

namespace Big3.Hitbase.SoundFilesManagement
{
    public partial class FormAddFolder : Form
    {
        public FormAddFolder()
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            UpdateWindowState();
        }

        public string Folder
        {
            get { return textBoxFolder.Text; }
            set { textBoxFolder.Text = value; }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = textBoxFolder.Text;
            fbd.Description = StringTable.SelectSearchFolder;

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                textBoxFolder.Text = fbd.SelectedPath;
            }
        }

        private void FormAddFolder_Load(object sender, EventArgs e)
        {
            textBoxFolder.Text = Settings.Current.ManageSoundFilesLastFolder;
        }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            Settings.Current.ManageSoundFilesLastFolder = textBoxFolder.Text;

            DialogResult = DialogResult.OK;
        }

        private void textBoxFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonOK.Enabled = !string.IsNullOrEmpty(textBoxFolder.Text) && Directory.Exists(textBoxFolder.Text);
        }

    }
}
