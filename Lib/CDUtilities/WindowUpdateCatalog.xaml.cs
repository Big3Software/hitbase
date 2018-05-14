using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using Microsoft.Win32;
using Big3.Hitbase.DataBaseEngine;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for WindowUpdateCatalog.xaml
    /// </summary>
    public partial class WindowUpdateCatalog : Window
    {
        public WindowUpdateCatalog()
        {
            InitializeComponent();
        }

        private string updateCatalogFilename;

        public string UpdateCatalogFilename
        {
            get { return updateCatalogFilename; }
            set { updateCatalogFilename = value; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string msg = string.Format(StringTable.UpdateCatalog, updateCatalogFilename, SqlCeUpgrade.GetBackupFilename(updateCatalogFilename));
            this.textBlockConvertCatalog.Text = msg;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }

        private void buttonConvert_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Application.Current.Shutdown();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = ".hdbx";
            sfd.OverwritePrompt = true;
            sfd.Title = StringTable.CreateCatalog;
            sfd.Filter = StringTable.FilterHitbase;

            if (sfd.ShowDialog(this) == true)
            {
                DataBase.Create(sfd.FileName);
                this.updateCatalogFilename = sfd.FileName;
                DialogResult = true;
                Close();
            }
        }
    }
}
