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
using System.ComponentModel;
using System.Windows.Threading;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for LyricsUserControl.xaml
    /// </summary>
    public partial class LyricsUserControl : UserControl
    {
        DispatcherTimer dt = new DispatcherTimer();
        double scrollOffset = 0.0;

        public LyricsUserControl()
        {
            InitializeComponent();

            DataContext = this;
/*            dt.Interval = TimeSpan.FromMilliseconds(100);
            dt.Tick += new EventHandler(dt_Tick); */
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (PlaylistItem != null && PlaylistItem.Info != null)
            {
                double trackLength = PlaylistItem.Info.Length / 2;
                double percent = 100 / trackLength * (Playlist.CurrentTrackPlayPosition - trackLength / 2);
                double scrollOffset = LyricsScrollViewer.ScrollableHeight * (percent / 100.0);
                if (scrollOffset < 0)
                    LyricsScrollViewer.ScrollToVerticalOffset(0);
                else
                    LyricsScrollViewer.ScrollToVerticalOffset(scrollOffset);
            }
        }



        public Playlist Playlist
        {
            get { return (Playlist)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Playlist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(Playlist), typeof(LyricsUserControl), new UIPropertyMetadata(null));



        public PlaylistItem PlaylistItem
        {
            get { return (PlaylistItem)GetValue(PlaylistItemProperty); }
            set { SetValue(PlaylistItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaylistItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaylistItemProperty =
            DependencyProperty.Register("PlaylistItem", typeof(PlaylistItem), typeof(LyricsUserControl), new UIPropertyMetadata(null, new PropertyChangedCallback(LyricsUserControl_PlaylistItemChanged)));

        static void LyricsUserControl_PlaylistItemChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            LyricsUserControl lyricsUserControl = (LyricsUserControl)property;
        }

        private void lyricsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //dt.Start();
        }

        private void lyricsUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //dt.Stop();
        }

        private void LyricsScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            LyricsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void LyricsScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            LyricsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }
}
