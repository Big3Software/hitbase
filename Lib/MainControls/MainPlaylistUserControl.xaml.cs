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
using System.Collections.Specialized;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Big3.Hitbase.DataBaseEngine;
using System.IO;

namespace Big3.Hitbase.MainControls
{
    public enum PlaylistViewMode
    {
        None,
        Single,
        Multiple
    }

    /// <summary>
    /// Interaction logic for MainPlaylistUserControl.xaml
    /// </summary>
    public partial class MainPlaylistUserControl : UserControl, INotifyPropertyChanged, Big3.Hitbase.Controls.DragDrop.IDropTarget
    {
        TrackDetailViewUserControl trackDetailViewUserControl1 = new TrackDetailViewUserControl();
        TrackDetailViewUserControl trackDetailViewUserControl2 = new TrackDetailViewUserControl();

        private bool detailView1Visible = true;

        private PlaylistViewMode currentViewMode = PlaylistViewMode.Single;
        public PlaylistViewMode CurrentViewMode 
        {
            get
            {
                return currentViewMode;
            }
            set
            {
                currentViewMode = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentViewMode"));
            }
        }

        private DataBase dataBase;

        public DataBase DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        public MainPlaylistUserControl()
        {
            DataContext = this;

            InitializeComponent();

            ((INotifyCollectionChanged)listBoxPlaylist.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(MainPlaylistUserControl_CollectionChanged);

            TrackDetailTransitionBox.Content = trackDetailViewUserControl1;

            DispatcherTimer dtMainPlaylist = new DispatcherTimer();
            dtMainPlaylist.Interval = TimeSpan.FromMilliseconds(1000);
            dtMainPlaylist.Tick += new EventHandler(dtMainPlaylist_Tick);
            dtMainPlaylist.Start();
        }

        private bool lastPlaying = false;
        void dtMainPlaylist_Tick(object sender, EventArgs e)
        {
            if (Playlist != null)
            {
                if (Playlist.IsPlaying && 
                    Playlist.CurrentTrackPlayPosition >= Playlist.CurrentPlaylistItem.Info.Length - 10000)
                {
                    if (NextTrackDetailUserControl.Visibility == System.Windows.Visibility.Collapsed && (Playlist.CurrentTrack < Playlist.Count - 1 || Playlist.RepeatType != RepeatType.None || Playlist.ShuffleActive))
                    {
                        NextTrackDetailUserControl.Track = Playlist[Playlist.PreviewGetNextTrack()];
                        NextTrackDetailUserControl.Visibility = System.Windows.Visibility.Visible;
                        DoubleAnimation dblAnim = new DoubleAnimation(200, 0, TimeSpan.FromMilliseconds(1000).Duration());
                        dblAnim.EasingFunction = new BackEase();
                        TranslateTransform tt = new TranslateTransform();
                        tt.BeginAnimation(TranslateTransform.XProperty, dblAnim);
                        NextTrackDetailUserControl.RenderTransform = tt;
                    }
                }
                else
                {
                    NextTrackDetailUserControl.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (Playlist.IsPlaying)
                {
                    TextBlockTotalLength.Text = Miscellaneous.Misc.GetTextFromSeconds((int)(playlist.TotalRemainLength / 1000), true);
                }

                if (lastPlaying == true && Playlist.IsPlaying == false)
                {
                    Playlist.Stop();
                }

                lastPlaying = Playlist.IsPlaying;
            }
        }

        void MainPlaylistUserControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (listBoxPlaylist.Items.Count == 1)
                TextBlockNumberOfItems.Text = listBoxPlaylist.Items.Count.ToString() + " " + StringTable.Soundfile;
            else
                TextBlockNumberOfItems.Text = listBoxPlaylist.Items.Count.ToString() + " " + StringTable.Soundfiles;

            TextBlockTotalLength.Text = Miscellaneous.Misc.GetTextFromSeconds((int)(playlist.TotalRemainLength / 1000), true);
        }

        public string PlaylistName
        {
            get
            {
                return "Test";
            }
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
                listBoxPlaylist.ItemsSource = value;
                playlist.CurrentTrackChanged += new EventHandler(playlist_CurrentTrackChanged);
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Playlist"));
            }
        }

        private string cdPlaylistTitle;

        public string CDPlaylistTitle
        {
            get { return cdPlaylistTitle; }
            set
            {
                cdPlaylistTitle = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CDPlaylistTitle"));
            }
        }

        void playlist_CurrentTrackChanged(object sender, EventArgs e)
        {
            if (detailView1Visible)
            {
                trackDetailViewUserControl2.Track = playlist.CurrentPlaylistItem;
                detailView1Visible = false;
                TrackDetailTransitionBox.Content = trackDetailViewUserControl2;
            }
            else
            {
                trackDetailViewUserControl1.Track = playlist.CurrentPlaylistItem;
                detailView1Visible = true;
                TrackDetailTransitionBox.Content = trackDetailViewUserControl1;
            }
        }

        private void listBoxPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && listBoxPlaylist.SelectedIndex >= 0 && VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as UIElement) != null)
            {
                Playlist.CurrentTrack = listBoxPlaylist.SelectedIndex;
            }
        }

        private void DeleteSelectedItems()
        {
            if (listBoxPlaylist.SelectedIndex >= 0)
            {
                int oldSelectedIndex = listBoxPlaylist.SelectedIndex;
                int nextFocusIndex;
                ListBoxItem lbi = null;

                for (int i = listBoxPlaylist.SelectedItems.Count - 1; i >= 0; i--)
                {
                    // Das aktuell spielende Lied darf nicht entfernt werden.
                    if (Playlist.CurrentTrack < 0 || listBoxPlaylist.SelectedItems[i] != Playlist[Playlist.CurrentTrack])
                        Playlist.Remove((PlaylistItem)listBoxPlaylist.SelectedItems[i]);
                }

                if (oldSelectedIndex >= listBoxPlaylist.Items.Count)
                    nextFocusIndex = listBoxPlaylist.Items.Count - 1;
                else
                    nextFocusIndex = oldSelectedIndex;
                if (nextFocusIndex >= 0 && nextFocusIndex < listBoxPlaylist.Items.Count)
                    lbi = listBoxPlaylist.ItemContainerGenerator.ContainerFromIndex(nextFocusIndex) as ListBoxItem;

                listBoxPlaylist.SelectedIndex = oldSelectedIndex;
                if (lbi != null)
                    lbi.Focus();
            }
        }

        private void MenuItemExtendedView_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentViewMode(PlaylistViewMode.Multiple);
        }

        private void MenuItemDetails_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentViewMode(PlaylistViewMode.Single);
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void PlayListNameTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Playlist.IsCDInPlaylist)
                return;

            PlayListNameTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            TextBoxPlaylistName.Visibility = System.Windows.Visibility.Visible;
            TextBoxPlaylistName.Focus();
            TextBoxPlaylistName.Text = Playlist.Name;
        }

        private void TextBoxPlaylistName_LostFocus(object sender, RoutedEventArgs e)
        {
            PlaylistNameLostFocus();
        }

        private void PlaylistNameLostFocus()
        {
            if (TextBoxPlaylistName.Visibility == System.Windows.Visibility.Visible)
            {
                EndEditMode();

                Playlist.Name = TextBoxPlaylistName.Text;

                SavePlaylist();
            }
        }

        private void EndEditMode()
        {
            PlayListNameTextBlock.Visibility = System.Windows.Visibility.Visible;
            TextBoxPlaylistName.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void SavePlaylist()
        {
            string fullFilename = System.IO.Path.Combine(Misc.GetPersonalHitbasePlaylistFolder(), System.IO.Path.GetFileName(Playlist.Name));

            Playlist.SaveToFile(fullFilename);
        }

        private void TextBoxPlaylistName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PlaylistNameLostFocus();
            }
        }

        private void splitButtonView_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentViewMode == PlaylistViewMode.Single)
            {
                SetCurrentViewMode(PlaylistViewMode.Multiple);
            }
            else
            {
                SetCurrentViewMode(PlaylistViewMode.Single);
            }
        }

        private void SetCurrentViewMode(PlaylistViewMode currentViewMode)
        {
            CurrentViewMode = currentViewMode;

            if (CurrentViewMode == PlaylistViewMode.Multiple)
            {
                splitButtonView.Image = ImageLoader.FromResource("ViewDetails.png");
                listBoxPlaylist.ItemTemplate = FindResource("PlaylistItemMultipleTemplate") as DataTemplate;
            }
            else
            {
                splitButtonView.Image = ImageLoader.FromResource("View.png");
                listBoxPlaylist.ItemTemplate = FindResource("PlaylistItemSingleTemplate") as DataTemplate;
            }
        }

        private void CommandBindingDeletePlaylistItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteSelectedItems();
        }

        private void CommandBindingDeletePlaylistItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listBoxPlaylist.SelectedIndex >= 0;
        }

        public new void DragOver(Controls.DragDrop.IDropInfo dropInfo)
        {
            if (Playlist.IsCDInPlaylist)
            {
                bool trackOfCDDropped = false;
                if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
                {
                    // Prüfen, ob ein Track einer CD gedropped wurde
                    MainCDUserControl mainCDUserControl = VisualTreeExtensions.FindParent<MainCDUserControl>(dropInfo.DragInfo.VisualSource);
                    if (mainCDUserControl != null)
                    {
                        trackOfCDDropped = true;
                    }
                }
                if (dropInfo.Data is PlaylistItem)
                {
                    trackOfCDDropped = true;
                }
                if (!trackOfCDDropped)
                {
                    dropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dropInfo.DropTargetAdorner = Big3.Hitbase.Controls.DragDrop.DropTargetAdorners.Insert;

            if (dropInfo.Data is PlaylistItem)
            {
                dropInfo.Effects = DragDropEffects.Move;
            }

            if (dropInfo.Data is Big3.Hitbase.CDUtilities.WishlistItem)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            if (dropInfo.Data is AlbumItem)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            if (dropInfo.Data is AlbumViewItem)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            if (dropInfo.Data is MyMusicListItem || dropInfo.Data is List<MyMusicListItem>)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            if (dropInfo.Data is DataObject)
            {
                DataObject dataObject = dropInfo.Data as DataObject;
                if (dataObject.GetDataPresent(DataFormats.FileDrop))
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                }
            }
        }

        public new void Drop(Controls.DragDrop.IDropInfo dropInfo)
        {
            if (dropInfo.Data is PlaylistItem)
            {
                Controls.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }

            if (dropInfo.Data is Big3.Hitbase.CDUtilities.WishlistItem)
            {
                Big3.Hitbase.CDUtilities.WishlistItem wishlistItem = dropInfo.Data as Big3.Hitbase.CDUtilities.WishlistItem;

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                
                addTracksParams.TrackIds.Add(wishlistItem.TrackID);
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (dropInfo.Data is AlbumViewItem)
            {
                AlbumViewItem item = dropInfo.Data as AlbumViewItem;
                CD cd = DataBase.GetCDById(item.ID);

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                foreach (Track track in cd.Tracks)
                    addTracksParams.TrackIds.Add(track.ID);
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (dropInfo.Data is AlbumItem)
            {
                AlbumItem item = dropInfo.Data as AlbumItem;
                CD cd = DataBase.GetCDById(item.ID);

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                foreach (Track track in cd.Tracks)
                    addTracksParams.TrackIds.Add(track.ID);
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (dropInfo.Data is Track || dropInfo.Data is List<Track>)
            {
                // Prüfen, ob ein Track einer CD gedropped wurde
                MainCDUserControl mainCDUserControl = VisualTreeExtensions.FindParent<MainCDUserControl>(dropInfo.DragInfo.VisualSource);
                if (mainCDUserControl != null)
                {
                    Track track = dropInfo.Data as Track;
                    List<Track> trackList = dropInfo.Data as List<Track>;

                    AddCDTracksToPlaylistParameter addTracksParams = new AddCDTracksToPlaylistParameter();
                    addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                    addTracksParams.InsertIndex = dropInfo.InsertIndex;

                    if (track != null)
                    {
                        addTracksParams.Tracks.Add(track);
                    }
                    if (trackList != null)
                    {
                        addTracksParams.Tracks.AddRange(trackList);
                    }
                    HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
                }
                else
                {
                    Track track = dropInfo.Data as Track;
                    List<Track> trackList = dropInfo.Data as List<Track>;

                    AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                    addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                    addTracksParams.InsertIndex = dropInfo.InsertIndex;

                    if (track != null)
                    {
                        addTracksParams.TrackIds.Add(track.ID);
                    }

                    if (trackList != null)
                    {
                        foreach (Track trackItem in trackList)
                            addTracksParams.TrackIds.Add(trackItem.ID);
                    }

                    HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
                }
            }

            if (dropInfo.Data is MyMusicListItem)
            {
                MyMusicListItem item = dropInfo.Data as MyMusicListItem;

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                addTracksParams.TrackIds.Add(item.ID);
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (dropInfo.Data is List<MyMusicListItem>)
            {
                List<MyMusicListItem> items = dropInfo.Data as List<MyMusicListItem>;

                AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                addTracksParams.InsertIndex = dropInfo.InsertIndex;
                foreach (MyMusicListItem item in items)
                {
                    addTracksParams.TrackIds.Add(item.ID);
                }
                HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
            }

            if (dropInfo.Data is DataObject)
            {
                DataObject dataObject = dropInfo.Data as DataObject;
                if (dataObject.GetDataPresent(DataFormats.FileDrop))
                {

                    AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                    addTracksParams.AddTracksType = AddTracksToPlaylistType.InsertAtIndex;
                    addTracksParams.InsertIndex = dropInfo.InsertIndex;
                    foreach (string item in dataObject.GetFileDropList())
                    {
                        addTracksParams.Filenames.Add(item);
                    }
                    HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
                }
            }
        }

        private void CommandBindingOpenTrackLocation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (listBoxPlaylist.SelectedItem != null)
            {
                PlaylistItem item = listBoxPlaylist.SelectedItem as PlaylistItem;

                HitbaseCommands.OpenTrackLocation.Execute(item.Info.Filename, Application.Current.MainWindow);
            }
        }    

        private void CommandBindingOpenTrackLocation_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (listBoxPlaylist.SelectedItem != null)
            {
                PlaylistItem item = listBoxPlaylist.SelectedItem as PlaylistItem;

                e.CanExecute = HitbaseCommands.OpenTrackLocation.CanExecute(item.Info.Filename, Application.Current.MainWindow);
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void CommandBindingSavePlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePlaylist();
        }

        private void CommandBindingSavePlaylist_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (!this.playlist.IsCDInPlaylist);
        }

    }
}
