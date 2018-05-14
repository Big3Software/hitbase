using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Miscellaneous;
using System.IO;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.DataBaseEngine
{
    public partial class FormConvertHdbToHdbx : Form
    {
        public FormConvertHdbToHdbx(string originalHdbFilename)
        {
            InitializeComponent();

            textBoxFilename.Text = Path.ChangeExtension(originalHdbFilename, ".hdbx");
        }

        public string Filename
        {
            get
            {
                return textBoxFilename.Text;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = StringTable.FilterHitbase;
            sfd.FileName = textBoxFilename.Text;
            sfd.DefaultExt = "hdbx";
            sfd.OverwritePrompt = false;

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                textBoxFilename.Text = sfd.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Prüfen, ob Datei bereits vorhanden ist, geht zur Zeit nicht
            // Die Datei könnte nämlich bereits in Hitbase offen sein.
            if (File.Exists(textBoxFilename.Text))
            {
                string msg = string.Format(StringTable.CatalogAlreadyExists, textBoxFilename.Text);
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.None;
                return;
            }
        }
    }
}
