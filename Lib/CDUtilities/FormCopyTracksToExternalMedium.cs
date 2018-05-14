using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Controls;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public partial class FormCopyTracksToExternalMedium : Form
    {
        public class ComboBoxItem
        {
            public DriveHelper.DiskDrive DriveInfo;

            public ComboBoxItem(DriveHelper.DiskDrive di)
            {
                DriveInfo = di;
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(DriveInfo.VolumeName))
                    return StringTable.RemovableMedium + " (" + DriveInfo.DriveLetter + ")";
                else
                    return DriveInfo.VolumeName + " (" + DriveInfo.DriveLetter + ")";
            }
        }

        FormCopyProgress formProgress;
        private int fileCount;
        private string[] filesToCopy;

        public FormCopyTracksToExternalMedium(string[] files)
        {
            InitializeComponent();

            FormThemeManager.SetTheme(this);

            filesToCopy = files;
        }

        private void FormCopyTracksToExternalMedium_Load(object sender, EventArgs e)
        {
            // Alle externen Laufwerk ermitteln
            pictureBox.Image = Misc.IconToAlphaBitmap(Images.mp3device);

            Visible = true;
            Refresh();
            Update();

            int count = 1;
            foreach (string file in filesToCopy)
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    ListViewItem item = listViewFiles.Items.Add(string.Format("{0}.", count++));
                    item.Tag = file;
                    item.SubItems.Add(file);
                    item.SubItems.Add(string.Format("{0} KB", (fi.Length + 1023) / 1024));
                }
                catch
                {
                }
            }

            List<DriveHelper.DiskDrive> diskDrives = DriveHelper.GetAvailableDisks();
            string[] drives = System.Environment.GetLogicalDrives();

            foreach (DriveHelper.DiskDrive diskDrive in diskDrives)
            {
                try
                {
                    if (diskDrive.InterfaceType == "USB" && diskDrive.Size > 10000000)     // 10000000, damit wir keine Floppys anzeigen
                    {
                        comboBoxDrive.Items.Add(new ComboBoxItem(diskDrive));
                    }
                }
                catch
                {
                }
            }

            if (comboBoxDrive.Items.Count > 0)
            {
                comboBoxDrive.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show(StringTable.NoExternalMediumFound, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            UpdateWindowState();
        }

        private void comboBoxDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDrive.SelectedItem == null)
                return;

            FillFileList();
        }

        /// <summary>
        /// Füllt die Liste mit den aktuellen Dateien auf dem externen Medium.
        /// </summary>
        private void FillFileList()
        {
            DriveHelper.DiskDrive di = ((ComboBoxItem)comboBoxDrive.SelectedItem).DriveInfo;
            FillFileList(di.DriveLetter);

            UpdateWindowState();
        }

        /// <summary>
        /// Füllt die Liste mit den aktuellen Dateien auf dem externen Medium.
        /// </summary>
        /// <param name="drive"></param>
        private void FillFileList(string drive)
        {
            Cursor.Current = Cursors.WaitCursor;

            treeListViewDrive.Nodes.Clear();

            buttonOK.Enabled = false;
            buttonCopyAll.Enabled = false;
            comboBoxDrive.Enabled = false;
            fileCount = 0;
            
            try
            {
                FillFileListRecursive(drive, null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                labelStatus.Text = "";

                Cursor.Current = Cursors.Default;
                buttonOK.Enabled = true;
                buttonCopyAll.Enabled = true;
                comboBoxDrive.Enabled = true;
            }
        }

        private void FillFileListRecursive(string p, TreeListNode parent)
        {
            foreach (string dir in Directory.GetDirectories(p))
            {
                TreeListNode newItem = new TreeListNode();
                newItem.Text = Path.GetFileName(dir);
                newItem.ImageIndex = 0;
                if (parent == null)
                    treeListViewDrive.Nodes.Add(newItem);
                else
                    parent.Nodes.Add(newItem);

                FillFileListRecursive(dir, newItem);
            }

            string[] files = Directory.GetFiles(p, "*.*");
            foreach (string file in files)
            {
                TreeListNode newItem = new TreeListNode();
                newItem.Text = Path.GetFileName(file);
                newItem.ImageIndex = 1;

                FileInfo fi = new FileInfo(file);
                string kbSize = string.Format(string.Format("{0} KB", (fi.Length+1023)/1024));

                if (parent == null)
                {
                    treeListViewDrive.Nodes.Add(newItem);
                    newItem.SubItems.Add(kbSize);
                }
                else
                {
                    parent.Nodes.Add(newItem);
                    newItem.SubItems.Add(kbSize);
                }

                fileCount++;
                labelStatus.Text = string.Format(StringTable.ReadingFiles, fileCount, fileCount == 1 ? StringTable.File : StringTable.Files);
                labelStatus.Update();
                Application.DoEvents();
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            string targetDir = ((ComboBoxItem)comboBoxDrive.SelectedItem).DriveInfo.DriveLetter.ToString() + "\\";
            formProgress = new FormCopyProgress(filesToCopy, targetDir);
            formProgress.ShowDialog(this);
            FillFileList();
        }

        private void buttonCopySelected_Click(object sender, EventArgs e)
        {
            string targetDir = ((ComboBoxItem)comboBoxDrive.SelectedItem).DriveInfo.DriveLetter.ToString() + "\\";

            List<string> selectedFiles = new List<string>();
            foreach (ListViewItem item in listViewFiles.Items)
            {
                if (item.Selected)
                    selectedFiles.Add((string)item.Tag);
            }

            formProgress = new FormCopyProgress(selectedFiles.ToArray(), targetDir);
            formProgress.ShowDialog(this);
            FillFileList();
        }

        private void UpdateWindowState()
        {
            buttonCopySelected.Enabled = (listViewFiles.SelectedItems.Count > 0 && comboBoxDrive.SelectedIndex >= 0);
            buttonCopyAll.Enabled = (comboBoxDrive.SelectedIndex >= 0);
        }

        private void listViewFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

    }
}
