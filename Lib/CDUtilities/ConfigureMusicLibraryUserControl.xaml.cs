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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using System.Collections.ObjectModel;
using Big3.Hitbase.Miscellaneous;
using Microsoft.WindowsAPICodePack.Shell;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for ConfiguteMusicLibraryUserControl.xaml
    /// </summary>
    public partial class ConfigureMusicLibraryUserControl : UserControl, IModalUserControl
    {
        public ConfigureMusicLibraryUserControl()
        {
            InitializeComponent();

            listBoxFolders.ItemsSource = allMusicLibraryFolders;
        }

        private ObservableCollection<MusicLibraryFolder> allMusicLibraryFolders = new ObservableCollection<MusicLibraryFolder>();

        public DataBase DataBase { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            /*if (DataBase.Master.MonitoredDirectories.Count == 0 && Microsoft.WindowsAPICodePack.Shell.ShellLibrary.IsPlatformSupported)
            {
                //FillDefaultDirectories();
            }*/

            FillMusicLibraryFolder();


            UpdateWindowState();
        }

        private void FillDefaultDirectories()
        {
            if (Microsoft.WindowsAPICodePack.Shell.ShellLibrary.IsPlatformSupported)
            {
                allMusicLibraryFolders.Clear();

                ShellLibrary sl = Microsoft.WindowsAPICodePack.Shell.ShellLibrary.Load(Microsoft.WindowsAPICodePack.Shell.KnownFolders.MusicLibrary, true);
                foreach (ShellFileSystemFolder sfsf in sl)
                {
                    AddFolder(sfsf.Path);
                }
            }
        }

        private void FillMusicLibraryFolder()
        {
            allMusicLibraryFolders.Clear();

            foreach (string folder in DataBase.Master.MonitoredDirectories)
            {
                AddFolder(folder);
            }

            textBlockAddDefaultDirs.Visibility = allMusicLibraryFolders.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AddFolder(string folder)
        {
            MusicLibraryFolder newFolder = new MusicLibraryFolder();

            string title = System.IO.Path.GetFileName(folder);

            if (ShellLibrary.IsPlatformSupported)
            {
                try
                {
                    // Schmeißt ne Exception, wenn es kein Known Folder ist
                    IKnownFolder kf = KnownFolderHelper.FromPath(folder);
                    title = kf.LocalizedName;
                }
                catch
                { }
            }

            newFolder.Title = title;
            newFolder.Path = folder;
            allMusicLibraryFolders.Add(newFolder);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DataBase.Master.MonitoredDirectories.Clear();

            foreach (MusicLibraryFolder mlf in allMusicLibraryFolders)
            {
                DataBase.Master.MonitoredDirectories.Add(mlf.Path);
            }

            DataBase.Master.WriteConfig(DataBase);

            if (OKClicked != null)
                OKClicked(this, new EventArgs());
        }

        #region IModalUserControl Members

        public event EventHandler OKClicked;

        public event EventHandler CancelClicked;

        #endregion

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = StringTable.ChooseMusicLibraryFolder;
            
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Prüfen, ob der ausgewählte Pfad bereits überwacht wird
                MusicLibraryFolder exists = allMusicLibraryFolders.SingleOrDefault(x => string.Compare(x.Path, folderBrowserDialog.SelectedPath, true) == 0);

                if (exists == null)
                    AddFolder(folderBrowserDialog.SelectedPath);
            }
            UpdateWindowState();
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            allMusicLibraryFolders.Remove((MusicLibraryFolder)listBoxFolders.SelectedItem);
            UpdateWindowState();
        }

        private void listBoxFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            buttonRemove.IsEnabled = listBoxFolders.SelectedIndex >= 0;
            textBlockAddDefaultDirs.Visibility = allMusicLibraryFolders.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void buttonAddStandard_Click(object sender, RoutedEventArgs e)
        {
            this.FillDefaultDirectories();
            UpdateWindowState();
            //FillMusicLibraryFolder();
        }

    }

    public class MusicLibraryFolder : INotifyPropertyChanged
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
