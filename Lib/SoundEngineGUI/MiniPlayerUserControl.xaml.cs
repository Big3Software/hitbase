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
using System.Windows.Threading;
using Big3.Hitbase.Miscellaneous;
using System.IO;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.SoundEngineGUI
{
    /// <summary>
    /// Interaction logic for MiniPlayerUserControl.xaml
    /// </summary>
    public partial class MiniPlayerUserControl : UserControl
    {
        private Track currentTrack;

        public DataBase DataBase { get; set; }

        public event EventHandler Closed;

        private bool lastIsPlaying = false;
        private string lastPlayingFilename;

        public MiniPlayerUserControl()
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            if (SoundEngine.SoundEngine.PreListen.IsPlaying)
            {
                if (!lastIsPlaying || lastPlayingFilename != SoundEngine.SoundEngine.PreListen.Filename)
                {
                    SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(SoundEngine.SoundEngine.PreListen.Filename);
                    TextBlockTrackName.Text = sfi.Artist + " - " + sfi.Title;
                    TextBlockTrackName.ToolTip = sfi.Artist + " - " + sfi.Title;
                    ProgressBar.Maximum = sfi.Length;

                    BitmapImage bi = GetImage(sfi);
                    if (bi != null)
                    {
                        ImageCover.Source = bi;
                    }
                    else
                    {
                        ImageCover.Source = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/CDCover.png"));
                    }

                    ButtonPlay.Content = FindResource("PauseButtonPath");
                }

                int position = SoundEngine.SoundEngine.PreListen.PlayPosition;
                TextBlockStatus.Text = Misc.GetShortTimeString(position);
                ProgressBar.Value = position;
            }
            else
            {
                //Visibility = Visibility.Hidden;
                TextBlockStatus.Text = "";
                //ImageCover.Source = null;
                ProgressBar.Value = 0;
            }

            lastIsPlaying = SoundEngine.SoundEngine.PreListen.IsPlaying;
            lastPlayingFilename = SoundEngine.SoundEngine.PreListen.Filename;
        }

        private bool showPlaylistButtons = true;
        public bool ShowPlaylistButtons
        {
            get
            {
                return showPlaylistButtons;
            }
            set
            {
                showPlaylistButtons = value;

                this.ImagePlayNow.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.ImagePlayNext.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.ImagePlayLast.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

/*        public void Play(Track track)
        {
            currentTrack = track;

            try
            {
                if (string.IsNullOrEmpty(track.Soundfile) || !System.IO.File.Exists(track.Soundfile))
                    return;

                SoundEngine.SoundEngine.PreListen.Play(track.Soundfile);

                SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(track.Soundfile);
                TextBlockTrackName.Text = track.Artist + " - " + track.Title;
                TextBlockTrackName.ToolTip = track.Artist + " - " + track.Title;
                ProgressBar.Maximum = sfi.Length;

                BitmapImage bi = GetImage(track);
                if (bi != null)
                {
                    ImageCover.Source = bi;
                }
                else
                {
                    ImageCover.Source = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/CDCover.png"));
                }

                ButtonPlay.Content = FindResource("PauseButtonPath");
            }
            catch (Exception e)
            {
                TextBlockTrackName.Text = Big3.Hitbase.SharedResources.StringTable.Error;
                TextBlockTrackName.ToolTip = e.Message;
            }
        }*/

        /// <summary>
        /// Liefert das Bild der MP3-Datei zurück. Falls nicht vorhanden, wird noch gesucht, ob die CD ein Bild
        /// hat, um dann dieses zurückzuliefern.
        /// </summary>
        /// <returns></returns>
        private BitmapImage GetImage(SoundFileInformation sfi)
        {
            if (sfi.Images != null && sfi.Images.Count > 0)
            {
                MemoryStream ms = new MemoryStream(sfi.Images[0]);
                return ImageLoader.GetBitmapImageFromMemoryStream(ms);
            }

            return null;
        }


        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!SoundEngine.SoundEngine.PreListen.IsPlaying && !string.IsNullOrEmpty(lastPlayingFilename))
            {
                SoundEngine.SoundEngine.PreListen.Play(lastPlayingFilename);
                return;
            }

            if (!SoundEngine.SoundEngine.PreListen.IsPaused)
            {
                SoundEngine.SoundEngine.PreListen.Pause(true);
                ButtonPlay.Content = FindResource("PlayButtonPath");
            }
            else
            {
                SoundEngine.SoundEngine.PreListen.Pause(false);
                ButtonPlay.Content = FindResource("PauseButtonPath");
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            SoundEngine.SoundEngine.PreListen.Stop();
            ButtonPlay.Content = FindResource("PlayButtonPath");
        }

        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ImagePlayNow_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/PlayNowHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/PlayNow.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/PlayNextHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/PlayNext.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/PlayLastHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/SoundEngineGUI;component/Images/PlayLast.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTrackToPlaylist((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Now);
        }

        private void ImagePlayNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTrackToPlaylist((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Next);
        }

        private void ImagePlayLast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTrackToPlaylist((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.End);
        }

        private void AddTrackToPlaylist(DependencyObject originalSource, AddTracksToPlaylistType addToPlaylistType)
        {
            if (currentTrack != null)
            {
                List<int> trackIds = new List<int>();
                trackIds.Add(currentTrack.ID);

                HitbaseCommands.AddTracksToPlaylist.Execute(new AddTracksToPlaylistParameter() { TrackIds = trackIds, AddTracksType = addToPlaylistType }, Application.Current.MainWindow);

                RequestClose();
                return;
            }

            if (!string.IsNullOrEmpty(lastPlayingFilename))
            {
                List<string> filenames = new List<string>();
                filenames.Add(lastPlayingFilename);

                HitbaseCommands.AddTracksToPlaylist.Execute(new AddTracksToPlaylistParameter() { Filenames = filenames, AddTracksType = addToPlaylistType }, Application.Current.MainWindow);

                RequestClose();
            }
        }

        private void RequestClose()
        {
            SoundEngine.SoundEngine.PreListen.Stop();

            if (Closed != null)
                Closed(this, new EventArgs());
        }

        private void ProgressBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double percentage = 100 / ProgressBar.ActualWidth * e.GetPosition(ProgressBar).X;

            SoundEngine.SoundEngine.PreListen.PlayPosition = (int)(ProgressBar.Maximum / 100.0 * percentage);

            e.Handled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RequestClose();
        }
    }
}
