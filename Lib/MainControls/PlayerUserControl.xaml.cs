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
using Big3.Hitbase.SoundEngine;
using System.IO;
using System.ComponentModel;
using System.Windows.Threading;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for PlayerUserControl.xaml
    /// </summary>
    public partial class PlayerUserControl : UserControl, INotifyPropertyChanged
    {
        private bool detailView1Visible = true;

        PlayerTrackDetailViewUserControl playerTrackDetailViewUserControl1 = new PlayerTrackDetailViewUserControl();
        PlayerTrackDetailViewUserControl playerTrackDetailViewUserControl2 = new PlayerTrackDetailViewUserControl();

        // Timer fürs Spulen
        DispatcherTimer dtSeek = new DispatcherTimer();
        DateTime dtPlayNextPressed;
        bool seekInProgress = false;
        bool seekInProgressForward = false;

        public PlayerUserControl()
        {
            InitializeComponent();

            TrackDetailTransitionBox.Content = playerTrackDetailViewUserControl1;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SliderVolume.Value = 10;
            }

            dtSeek.Interval = TimeSpan.FromMilliseconds(100).Duration();
            dtSeek.Tick += new EventHandler(dtSeek_Tick);

            //UpdateViewSplitButton();
        }

        private Playlist playlist;
        public Playlist Playlist
        {
            get
            {
                return playlist;
            }
            set
            {
                playlist = value;
                playlist.CurrentTrackChanged += new EventHandler(playlist_CurrentTrackChanged);
                playlist.PlayStatusChanged += new EventHandler(playlist_PlayStatusChanged);
                WaveDataUserControl.Playlist = value;
                this.DataContext = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Playlist"));
                }
            }
        }

        public PlaylistItem Track
        {
            get
            {
                if (playlist != null)
                    return playlist.CurrentPlaylistItem;
                else
                    return null;
            }
        }

        public ImageSource TrackImage
        {
            get
            {
                if (Track != null)
                {
                    return Track.TrackImage;
                }
                return null;
            }
        }


        void playlist_CurrentTrackChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Track"));
                PropertyChanged(this, new PropertyChangedEventArgs("TrackImage"));
            }

            if (Playlist.IsPlaying)
            {
                if (Settings.Current.ShowFrequencyBand)
                {
                    WaveDataUserControl.Visibility = Visibility.Visible;
                }
                if (Settings.Current.ShowLyrics)
                {
                    LyricsUserControl.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                WaveDataUserControl.Visibility = Visibility.Collapsed;
                LyricsUserControl.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (detailView1Visible)
            {
                playerTrackDetailViewUserControl2.Track = playlist.CurrentPlaylistItem;
                detailView1Visible = false;
                TrackDetailTransitionBox.Content = playerTrackDetailViewUserControl2;
            }
            else
            {
                playerTrackDetailViewUserControl1.Track = playlist.CurrentPlaylistItem;
                detailView1Visible = true;
                TrackDetailTransitionBox.Content = playerTrackDetailViewUserControl1;
            }

            UpdatePlayButton();
        }

        private void UpdatePlayButton()
        {
            if (Playlist.IsPaused || !Playlist.IsPlaying)
            {
                ButtonPlay.Content = FindResource("PlayPath");
            }
            else
            {
                ButtonPlay.Content = FindResource("PausePath");
            }
        }

        void playlist_PlayStatusChanged(object sender, EventArgs e)
        {
            UpdatePlayButton();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            Playlist.Stop();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundEngine.SoundEngine.Instance.Volume = (float)e.NewValue / 10;
        }

        private void ButtonPlayPrev_Click(object sender, RoutedEventArgs e)
        {
            Playlist.PlayPrev();
        }

        private void ButtonPlayNext_Click(object sender, RoutedEventArgs e)
        {
            Playlist.PlayNext();
        }

        private void ButtonMute_Click(object sender, RoutedEventArgs e)
        {
            if (SoundEngine.SoundEngine.Instance.IsMuted)
            {
                SoundEngine.SoundEngine.Instance.IsMuted = false;
                ImageMute.Source = ImageLoader.FromResource("Sound.png");
                ButtonMute.ToolTip = StringTable.VolumeOff;
            }
            else
            {
                SoundEngine.SoundEngine.Instance.IsMuted = true;
                ImageMute.Source = ImageLoader.FromResource("SoundMute.png");
                ButtonMute.ToolTip = StringTable.VolumeOn;
            }
        }

        private void ButtonPlayNext_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!seekInProgress && e.LeftButton == MouseButtonState.Pressed)
            {
                seekInProgressForward = true;
                dtPlayNextPressed = DateTime.Now;
                dtSeek.Start();
            }
        }

        void dtSeek_Tick(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (DateTime.Now - dtPlayNextPressed > TimeSpan.FromMilliseconds(500))
                {
                    if (seekInProgressForward)
                    {
                        Playlist.CurrentTrackPlayPosition += 1000;
                        ButtonPlayNext.CaptureMouse();
                    }
                    else
                    {
                        Playlist.CurrentTrackPlayPosition -= 1000;
                        ButtonPlayPrev.CaptureMouse();
                    }
                    seekInProgress = true;
                }
            }
        }

        private void ButtonPlayNext_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (seekInProgress)
            {
                seekInProgress = false;
                dtSeek.Stop();
                ButtonPlayNext.ReleaseMouseCapture();
                e.Handled = true;
            }
            else
            {
                dtSeek.Stop();
                ButtonPlayNext.ReleaseMouseCapture();

                NextTrack();
            }
        }

        private void NextTrack()
        {
            Playlist.PlayNext();
        }

        private void ToggleButtonRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.RepeatType == RepeatType.None)
            {
                Playlist.RepeatType = RepeatType.Single;
                ToggleButtonRepeat.ToolTip = StringTable.RepeatAll;
                ImageRepeat.Source = ImageLoader.FromResource("RepeatSingle.png");
                return;
            }

            if (Playlist.RepeatType == RepeatType.Single)
            {
                Playlist.RepeatType = RepeatType.All;
                ToggleButtonRepeat.ToolTip = StringTable.RepeatOff;
                ImageRepeat.Source = ImageLoader.FromResource("Repeat.png");
                return;
            }

            if (Playlist.RepeatType == RepeatType.All)
            {
                Playlist.RepeatType = RepeatType.None;
                ToggleButtonRepeat.ToolTip = StringTable.RepeatSingle;
                ImageRepeat.Source = ImageLoader.FromResource("Repeat.png");
                return;
            }
        }

        private void ToggleButtonShuffle_Checked(object sender, RoutedEventArgs e)
        {
            if (Playlist.ShuffleActive)
                ToggleButtonShuffle.ToolTip = StringTable.ShuffleOff;
            else
                ToggleButtonShuffle.ToolTip = StringTable.ShuffleOn;
        }

        private void ButtonPlayPrev_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!seekInProgress && e.LeftButton == MouseButtonState.Pressed)
            {
                seekInProgressForward = false;
                dtPlayNextPressed = DateTime.Now;
                dtSeek.Start();
            }
        }

        private void ButtonPlayPrev_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (seekInProgress)
            {
                seekInProgress = false;
                dtSeek.Stop();
                ButtonPlayPrev.ReleaseMouseCapture();
                e.Handled = true;
            }
            else
            {
                dtSeek.Stop();
                ButtonPlayPrev.ReleaseMouseCapture();

                PrevTrack();
            }
        }

        private void PrevTrack()
        {
            if (Playlist.CurrentTrackPlayPosition > 3000 || (Playlist.CurrentTrack == 0 && !Playlist.ShuffleActive))
            {
                Playlist.CurrentTrackPlayPosition = 0;
            }
            else
            {
                Playlist.PlayPrev();
            }
        }

        public void UpdateWindowState()
        {
            if (Settings.Current.ShowFrequencyBand)
            {
                WaveDataUserControl.SoundDisplayType = SoundDisplayType.FrequencyBand;
                WaveDataUserControl.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                WaveDataUserControl.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (Settings.Current.ShowLyrics)
            {
                LyricsUserControl.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                LyricsUserControl.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /*private void MenuItemSignal_Click(object sender, RoutedEventArgs e)
        {
            WaveDataUserControl.SoundDisplayType = SoundDisplayType.Signal;

            UpdateViewSplitButton();
        }

        private void MenuItemFrequencyBand_Click(object sender, RoutedEventArgs e)
        {
            WaveDataUserControl.SoundDisplayType = SoundDisplayType.FrequencyBand;

            UpdateViewSplitButton();
        }

        private void MenuItemOff_Click(object sender, RoutedEventArgs e)
        {
            WaveDataUserControl.SoundDisplayType = SoundDisplayType.None;

            UpdateViewSplitButton();
        }

        private void MenuItemLyrics_Click(object sender, RoutedEventArgs e)
        {
            WaveDataUserControl.SoundDisplayType = SoundDisplayType.Lyrics;

            UpdateViewSplitButton();
        }

        private void splitButtonView_Click(object sender, RoutedEventArgs e)
        {
            switch (WaveDataUserControl.SoundDisplayType)
            {
                case SoundDisplayType.Signal:
                    WaveDataUserControl.SoundDisplayType = SoundDisplayType.FrequencyBand;
                    break;
                case SoundDisplayType.FrequencyBand:
                    WaveDataUserControl.SoundDisplayType = SoundDisplayType.Lyrics;
                    break;
                case SoundDisplayType.Lyrics:
                    WaveDataUserControl.SoundDisplayType = SoundDisplayType.None;
                    break;
                case SoundDisplayType.None:
                    WaveDataUserControl.SoundDisplayType = SoundDisplayType.Signal;
                    break;
            }

            UpdateViewSplitButton();
        }*/

        /*private void UpdateViewSplitButton()
        {
            MenuItemLyrics.IsChecked = WaveDataUserControl.SoundDisplayType == SoundDisplayType.Lyrics;
            MenuItemOff.IsChecked = WaveDataUserControl.SoundDisplayType == SoundDisplayType.None;
            MenuItemFrequencyBand.IsChecked = WaveDataUserControl.SoundDisplayType == SoundDisplayType.FrequencyBand;
            MenuItemSignal.IsChecked = WaveDataUserControl.SoundDisplayType == SoundDisplayType.Signal;

            switch (WaveDataUserControl.SoundDisplayType)
            {
                case SoundDisplayType.None:
                    splitButtonView.Image = ImageLoader.FromResource("None.png");
                    WaveDataUserControl.Visibility = System.Windows.Visibility.Collapsed;
                    LyricsUserControl.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case SoundDisplayType.Signal:
                    splitButtonView.Image = ImageLoader.FromResource("Signal.png");
                    WaveDataUserControl.Visibility = System.Windows.Visibility.Visible;
                    LyricsUserControl.Visibility = System.Windows.Visibility.Collapsed;
                    break; 
                case SoundDisplayType.FrequencyBand:
                    splitButtonView.Image = ImageLoader.FromResource("Frequencies.png");
                    WaveDataUserControl.Visibility = System.Windows.Visibility.Visible;
                    LyricsUserControl.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case SoundDisplayType.Lyrics:
                    splitButtonView.Image = ImageLoader.FromResource("Lyrics.png");
                    WaveDataUserControl.Visibility = System.Windows.Visibility.Collapsed;
                    LyricsUserControl.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
        }*/

    }

    public class RepeatTypeCheckedConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RepeatType repeatType = (RepeatType)value;

            switch (repeatType)
            {
                case RepeatType.None:
                    return false;
                case RepeatType.Single:
                case RepeatType.All:
                    return true;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


