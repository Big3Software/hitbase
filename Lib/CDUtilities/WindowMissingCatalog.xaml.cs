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
    /// Interaction logic for WindowMissingCatalog.xaml
    /// </summary>
    public partial class WindowMissingCatalog : Window
    {
        public WindowMissingCatalog()
        {
            InitializeComponent();
        }

        private string missingCatalogFilename;

        public string MissingCatalogFilename
        {
            get { return missingCatalogFilename; }
            set { missingCatalogFilename = value; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string msg = string.Format(StringTable.MissingCatalog, missingCatalogFilename);
            this.textBlockMissingCatalog.Text = msg;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = StringTable.FilterHitbase;
            openFileDialog.DefaultExt = ".hdbx";
            if (openFileDialog.ShowDialog(this) == true)
            {
                this.missingCatalogFilename = openFileDialog.FileName;
                DialogResult = true;
                Close();
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
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
                this.missingCatalogFilename = sfd.FileName;
                DialogResult = true;
                Close();
            }
        }
    }
}
