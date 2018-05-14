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
using Big3.Hitbase.SoundEngine;
using System.ComponentModel;
using System.IO;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for TrackDetailViewUserControl.xaml
    /// </summary>
    public partial class TrackDetailViewUserControl : UserControl, INotifyPropertyChanged
    {
        public TrackDetailViewUserControl()
        {
            InitializeComponent();
            GridTrackDetails.Visibility = Visibility.Hidden;
        }

        private PlaylistItem _track;
        public PlaylistItem Track 
        {
            get
            {
                return _track;
            }
            set
            {
                _track = value;
                GridTrackDetails.Visibility = value != null ? Visibility.Visible : Visibility.Hidden;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Track"));
                    PropertyChanged(this, new PropertyChangedEventArgs("TrackImage"));
                }
            }
        }

        public ImageSource TrackImage
        {
            get
            {
                if (Track != null && Track.Info != null)
                {
                    return Track.TrackImage;
                }

                return null;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
