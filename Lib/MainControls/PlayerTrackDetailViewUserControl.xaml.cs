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
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for TrackDetailViewUserControl.xaml
    /// </summary>
    public partial class PlayerTrackDetailViewUserControl : UserControl, INotifyPropertyChanged
    {
        private bool isPlaying = false;

        public PlayerTrackDetailViewUserControl()
        {
            InitializeComponent();
            GridTrackDetails.Visibility = Visibility.Hidden;

            DispatcherTimer dtTrackPosition = new DispatcherTimer();
            dtTrackPosition.Interval = TimeSpan.FromMilliseconds(1000);
            dtTrackPosition.Tick += new EventHandler(dtTrackPosition_Tick);
            dtTrackPosition.Start();
        }

        void dtTrackPosition_Tick(object sender, EventArgs e)
        {
            if (Track != null && Track.Playlist != null && Track.Playlist.IsPlaying)
            {
                if (!isPlaying)
                {
                    BeginGlowAnimation();
                }

                isPlaying = true;
                ProgressBarTrack.Value = 1.0 / Track.Info.Length * Track.Playlist.CurrentTrackPlayPosition;

                double lapMarginLeft = 10 + ((ProgressBarTrack.ActualWidth - 10) * ProgressBarTrack.Value) / 2 - textBlockTrackPosition.ActualWidth/2;
                if (lapMarginLeft < 13)
                    lapMarginLeft = 13;
                textBlockTrackPosition.Text = Miscellaneous.Misc.GetShortTimeString(Track.Playlist.CurrentTrackPlayPosition);
                textBlockTrackPosition.Margin = new Thickness(lapMarginLeft, 0, 0, 10);
                
                int remainTime = Track.Playlist.CurrentPlaylistItem.Info.Length - Track.Playlist.CurrentTrackPlayPosition;
                textBlockTrackPositionRemain.Text = Miscellaneous.Misc.GetShortTimeString(remainTime);

                double remMarginRight = (ProgressBarTrack.ActualWidth * (1 - ProgressBarTrack.Value)) / 2 - textBlockTrackPositionRemain.ActualWidth/2;
                if (remMarginRight < 2)
                    remMarginRight = 2;
                textBlockTrackPositionRemain.Margin = new Thickness(0, 0, remMarginRight, 10);
            }
            else
            {
                isPlaying = false;
                ProgressBarTrack.Value = 0;
                textBlockTrackPosition.Text = "";
                textBlockTrackPositionRemain.Text = "";
            }
        }


        private void BeginGlowAnimation()
        {
            LinearGradientBrush lgb = FindResource("CoverGlow") as LinearGradientBrush;
            DoubleAnimation da1 = new DoubleAnimation(-1, 2, TimeSpan.FromMilliseconds(3000).Duration());
            da1.RepeatBehavior = RepeatBehavior.Forever;
            lgb.GradientStops[0].BeginAnimation(GradientStop.OffsetProperty, da1);

            DoubleAnimation da2 = new DoubleAnimation(-0.8, 2, TimeSpan.FromMilliseconds(3000).Duration());
            da2.RepeatBehavior = RepeatBehavior.Forever;
            lgb.GradientStops[1].BeginAnimation(GradientStop.OffsetProperty, da2);

            DoubleAnimation da3 = new DoubleAnimation(-0.6, 2, TimeSpan.FromMilliseconds(3000).Duration());
            da3.RepeatBehavior = RepeatBehavior.Forever;
            lgb.GradientStops[2].BeginAnimation(GradientStop.OffsetProperty, da3);

            CoverGlowGrid.Height = imageCDCover.ActualHeight;
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

        private void ProgressBarTrack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(ProgressBarTrack);
            Track.Playlist.CurrentTrackPlayPosition = (int)((double)Track.Info.Length / ProgressBarTrack.ActualWidth * pt.X);
        }
    }
}
