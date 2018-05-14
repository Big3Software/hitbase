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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CatalogView3D
{
    /// <summary>
    /// Interaction logic for Browse3DWindow.xaml
    /// </summary>
    public partial class Browse3DWindow : Window
    {
        public Browse3DWindow(DataBase dataBase, Playlist playlist, Wishlist wishlist)
        {
            InitializeComponent();

            BrowseCatalog3DUserControl.Playlist = playlist;
            BrowseCatalog3DUserControl.Wishlist = wishlist;
            BrowseCatalog3DUserControl.DataBase = dataBase;

            Closed += new EventHandler(Browse3DWindow_Closed);
        }

        void Browse3DWindow_Closed(object sender, EventArgs e)
        {
            this.Content = null;
        }

        public Playlist Playlist 
        {
            set
            {
                BrowseCatalog3DUserControl.Playlist = value;
            }
        }

        public Wishlist Wishlist
        {
            set
            {
                BrowseCatalog3DUserControl.Wishlist = value;
            }
        }

        void BrowseCatalog3DUserControl_PartyModusStarted(object sender, EventArgs e)
        {
            BrowseCatalog3DUserControl.PartyModusActive = true;
        }

        private void ImageClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.BrowseCatalog3DUserControl.PartyModusActive)
                e.Cancel = true;
        }
    }
}
