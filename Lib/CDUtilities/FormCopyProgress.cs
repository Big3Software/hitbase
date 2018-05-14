using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Miscellaneous;
using System.IO;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCopyProgress : Form
    {
        public bool Canceled = false;
        private string[] filesToCopy;
        private string targetDir;

        public FormCopyProgress(string[] files, string targetDirectory)
        {
            InitializeComponent();

            filesToCopy = files;
            targetDir = targetDirectory;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Canceled = true;
        }

        private void FormCopyProgress_Load(object sender, EventArgs e)
        {
            // Das Kopieren der Dateien machen wir in einem eigenen Thread,
            // damit die Animation flüssig läuft
            backgroundWorker.RunWorkerAsync();

/*            int locationX, locationY;
            locationX = this.Owner.Location.X + (this.Owner.Width / 2) -
(this.Width / 2);
            locationY = this.Owner.Location.Y + (this.Owner.Height / 2) -
(this.Height / 2);
            this.Location = new Point(locationX, locationY);*/
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bool answerSaved = false;
                DialogResult lastAnswer = DialogResult.None;
                progressBar.Minimum = 0;
                progressBar.Maximum = filesToCopy.Length;

                //string targetDir = ((ComboBoxItem)comboBoxDrive.SelectedItem).DriveInfo.RootDirectory.ToString();

                foreach (string file in filesToCopy)
                {
                    string targetFilename = Path.Combine(targetDir, Path.GetFileName(file)); ;

                    labelStatus.Text = Path.GetFileName(file);
                    bool overwrite = true;
                    if (File.Exists(targetFilename))
                    {
                        if (answerSaved)
                        {
                            if (lastAnswer == DialogResult.No)
                                overwrite = false;
                        }
                        else
                        {
                            DialogResult res = MessageBoxEx.Show(this, string.Format(StringTable.FileExists, targetFilename), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, ref answerSaved);

                            if (answerSaved)
                                lastAnswer = res;

                            if (res == DialogResult.Cancel)
                                break;
                            if (res == DialogResult.No)
                                overwrite = false;
                        }
                    }

                    if (overwrite)
                    {
                        File.Copy(file, targetFilename, true);
                    }
                    progressBar.Value++;
                    Application.DoEvents();

                    if (Canceled)
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }
}