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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.CDUtilities
{
    /// <summary>
    /// Interaction logic for WindowAlbum.xaml
    /// </summary>
    public partial class WindowAlbum : Window
    {
        private CD cd;
        private DataBase dataBase;

        private bool saveAlbumOnOK = true;
        public bool SaveAlbumOnOK
        {
            get
            {
                return saveAlbumOnOK;
            }
            set
            {
                saveAlbumOnOK = value;
            }
        }

        public WindowAlbum()
        {
            InitializeComponent();

            Settings.RestoreWindowPlacement(this, "WindowAlbum");
        }

        public WindowAlbum(CD cd, DataBase db) : this()
        {
            this.cd = cd;
            this.dataBase = db;
            cdUserControl.DataBase = db;
            cdUserControl.CD = cd;
        }

        public WindowAlbum(CD cd, DataBase db, int showTrack) : this()
        {
            this.cd = cd;
            this.dataBase = db;
            cdUserControl.DataBase = db;
            cdUserControl.CD = cd;
            if (showTrack >= 0)
                cdUserControl.ShowTrack(showTrack);
        }

        private void CancelDialog()
        {
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SaveAndClose();
        }

        private void SaveAndClose()
        {
            TextBox textBox = Keyboard.FocusedElement as TextBox;
            if (textBox != null)
            {
                BindingExpression be =
                  textBox.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                    be.UpdateSource();
            }

            if (!cd.Sampler)
            {
                foreach (Track track in cd.Tracks)
                {
                    track.Artist = cd.Artist;
                }
            }

            if (saveAlbumOnOK)
            {
                cd.Save(dataBase);

                Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(cd);
            }

            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveAndClose();
            }

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CancelDialog();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.SaveWindowPlacement(this, "WindowAlbum");
        }
    }
}
