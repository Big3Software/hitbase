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
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using Big3.Hitbase.SoundEngineGUI;
using Big3.Hitbase.SoundEngine2;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Controls;
using Big3.Hitbase.DataBaseEngine.TrackDataSetTableAdapters;

namespace Big3.Hitbase.CatalogView
{
    /// <summary>
    /// Interaction logic for CatalogListView.xaml
    /// </summary>
    public partial class AlbumView : UserControl
    {
        public delegate void OpenContextMenuHandler(object sender, ContextMenuEventArgs e);
        public event OpenContextMenuHandler OpenContextMenu;

        public event EventHandler DoubleClick;
        public event EventHandler SelectionChanged;

        public event EventHandler DoDragDrop;
        public event EventHandler Refresh;

        public DataBase DataBase { get; set; }

        public Big3.Hitbase.DataBaseEngine.Condition Condition { get; set; }


        public event OnAddTracksToPlaylist AddTracksToPlaylist;

        public Track SelectedTrack { get; set; }
        public AlbumViewItem SelectedAlbum { get; set; }

        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;

        /// <summary>
        /// Hiernach wird die Ausgabe gefiltert.
        /// </summary>
        private string FilterString = "";

        public AlbumView()
        {
            InitializeComponent();
        }

        private ScrollViewer scrollViewer = null;
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (scrollViewer == null)
                {
                    DependencyObject border = VisualTreeHelper.GetChild(this, 0);
                    if (border != null)
                    {
                        scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                    }
                }

                return scrollViewer;
            }
        }

        public ItemCollection Items
        {
            get
            {
                return dataGrid1.Items;
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return dataGrid1.ItemsSource;
            }
            set
            {
                dataGrid1.ItemsSource = value;
            }
        }

        public IList SelectedItems
        {
            get
            {
                return null;
                //return dataGrid1.SelectedItems;
            }
        }

        public void FillList()
        {
            ItemsSource = null;
            GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;

            //AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwAlbumView_RunWorkerCompleted);
                backgroundWorker.DoWork += new DoWorkEventHandler(bwAlbumView_DoWork);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void bwAlbumView_DoWork(object sender, DoWorkEventArgs e)
        {
            TrackDataView TrackView;

            CDQuery = DataBase.ExecuteTrackQuery();

            // Hier die Felder auflisten, die ich für die Albumansicht brauche
            FieldCollection fc = new FieldCollection();
            fc.AddRange(new Field[] { 
                Field.ArtistCDName,
                Field.ArtistCDSaveAs,
                Field.Title,
                Field.CDCoverFront,
                Field.Category,
                Field.ArchiveNumber,
                Field.YearRecorded,
                Field.TrackNumber,
                Field.TrackTitle,
                Field.TrackLength,
                Field.TrackRating,
                Field.ArtistTrackName,
                Field.ComposerTrackName,
                Field.TrackSoundFile
            });

            SortFieldCollection sfc = new SortFieldCollection();
            sfc.Add(Field.ArtistCDSaveAs);
            sfc.Add(Field.Title);
            sfc.Add(Field.CDID);
            sfc.Add(Field.TrackNumber);
            TrackView = new TrackDataView(DataBase, CDQuery, Condition, sfc, fc);

            int count = 0;

            List<AlbumViewItem> items = new List<AlbumViewItem>();

            AlbumViewItem newItem = null;
            string lastArtist = "";
            string lastTitle = "";
            int lastcdid = 0;

            for (int row = 0; row < TrackView.Rows.Count; row++)
            {
                string artistDisplay = TrackView.GetRowStringValue(row, Field.ArtistCDName);
                string artist = TrackView.GetRowStringValue(row, Field.ArtistCDSaveAs);
                string title = TrackView.GetRowStringValue(row, Field.Title);
                int cdid = TrackView.GetCDID(row);

                if (artist != lastArtist || title != lastTitle || cdid != lastcdid)
                {
                    if (newItem != null)
                        items.Add(newItem);

                    newItem = new AlbumViewItem();
                    newItem.ID = cdid;
                    newItem.Artist = artistDisplay;
                    newItem.Title = title;
                    newItem.ImageFilename = TrackView.GetRowStringValue(row, Field.CDCoverFront);
                    newItem.Genre = TrackView.GetRowStringValue(row, Field.Category);
                    newItem.ArchiveNumber = TrackView.GetRowStringValue(row, Field.ArchiveNumber);
                    int yearRecorded = (int)TrackView.GetRowRawValue(row, Field.YearRecorded);
                    if (yearRecorded > 0)
                        newItem.Year = yearRecorded.ToString();

                    newItem.Tracks = new List<Track>();
                    lastArtist = artist;
                    lastTitle = title;
                }

                if (newItem != null)
                {
                    Track track = new Track();
                    track.TrackNumber = (int)TrackView.GetRowRawValue(row, Field.TrackNumber);
                    track.Title = TrackView.GetRowStringValue(row, Field.TrackTitle);
                    track.Length = (int)TrackView.GetRowRawValue(row, Field.TrackLength);
                    track.Rating = (int)TrackView.GetRowRawValue(row, Field.TrackRating);
                    track.Artist = TrackView.GetRowStringValue(row, Field.ArtistTrackName);
                    track.Composer = TrackView.GetRowStringValue(row, Field.ComposerTrackName);
                    track.Soundfile = TrackView.GetRowStringValue(row, Field.TrackSoundFile);
                    track.ID = TrackView.GetTrackID(row);

                    newItem.Tracks.Add(track);
                }

                //toolStripStatusProgressBar.Value = (int)(100.0 / TrackView.Rows.Count * count);

                count++;

                lastcdid = cdid;
            }

            if (newItem != null)
                items.Add(newItem);

            e.Result = items;
        }

        void bwAlbumView_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            List<AlbumViewItem> items = e.Result as List<AlbumViewItem>;
            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            ItemsSource = lcv;
        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FilterString))
                return true;

            string filterString = FilterString.ToLower();

            bool found = false;

            AlbumViewItem avi = row as AlbumViewItem;
            if (avi.Artist != null && avi.Artist.ToLower().IndexOf(filterString) >= 0)
            {
                return true;
            }
            if (avi.Title != null && avi.Title.ToLower().IndexOf(filterString) >= 0)
            {
                return true;
            }
            if (avi.Genre != null && avi.Genre.ToLower().IndexOf(filterString) >= 0)
            {
                return true;
            }
            if (avi.Year != null && avi.Year.ToLower().IndexOf(filterString) >= 0)
            {
                return true;
            }
            foreach (Track t in avi.Tracks)
            {
                if (t.Artist != null && t.Artist.ToLower().IndexOf(filterString) >= 0)
                {
                    found = true;
                    break;
                }
                if (t.Title != null && t.Title.ToLower().IndexOf(filterString) >= 0)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private void ImagePlayNow_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNowHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNow.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNextHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayNext_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayNext.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayLastHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayLast_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayLast.png"));
            img.Source = bmp;
        }

        private void ImagePlayPreListen_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayPreListenHover.png"));
            img.Source = bmp;
        }

        private void ImagePlayPreListen_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)e.OriginalSource;

            BitmapImage bmp = new BitmapImage(new Uri("pack://application:,,,/CatalogView3D;component/Images/PlayPreListen.png"));
            img.Source = bmp;
        }

        private void ImagePlayNow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddToPlaylistType.Now);
        }

        private void ImagePlayNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddToPlaylistType.Next);
        }

        private void ImagePlayLast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddToPlaylistType.End);
        }

        private void ImagePlayPreListen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack((DependencyObject)e.OriginalSource);

            if (track != null)
            {
                MiniPlayerWindow.PreListen(track);
            }
        }

        private void AddTrackFromSearchResult(DependencyObject originalSource, AddToPlaylistType addToPlaylistType)
        {
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack(originalSource);

            if (track != null)
            {
                AddTrackToPlaylist(track.Soundfile, addToPlaylistType);
            }
        }

        private void AddTrackToPlaylist(string soundFile, AddToPlaylistType addToPlaylistType)
        {
            if (AddTracksToPlaylist != null)
            {
                string[] filenames = new string[] { soundFile };
                
                AddTracksToPlaylist(filenames, addToPlaylistType);
            }
        }

        Big3.Hitbase.DataBaseEngine.Track GetTrack(DependencyObject dep)
        {
            while ((dep != null) && !(dep is ContentPresenter))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            DependencyObject depItemsPresenter = dep;
            while ((depItemsPresenter != null) && !(depItemsPresenter is ItemsControl))
            {
                depItemsPresenter = VisualTreeHelper.GetParent(depItemsPresenter);
            }

            if (dep != null)
            {
                ContentPresenter lbi = dep as ContentPresenter;
                ItemsControl itemCtl = depItemsPresenter as ItemsControl;
                Track track = itemCtl.ItemContainerGenerator.ItemFromContainer(lbi) as Track;

                return track;
            }

            return null;
        }

        private void RatingUserControl_ValueChanged(object sender, EventArgs e)
        {
            RatingUserControl rc = (RatingUserControl)sender;
            Track track = GetTrack(rc);

            TrackTableAdapter ta = new TrackTableAdapter(DataBase);
            TrackDataSet.TrackDataTable dtTrack = ta.GetDataById(track.ID);
            dtTrack[0].Rating = rc.Value;
            ta.Update(dtTrack[0]);
        }

        private void GridRow_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            DependencyObject dep = e.OriginalSource as DependencyObject;
            while ((dep != null) && !(dep is ContentPresenter))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            SelectedAlbum = null;
            SelectedTrack = null;

            ContentPresenter cp = dep as ContentPresenter;
            if (cp != null)
            {
                if (cp.DataContext is Track)
                {
                    SelectedTrack = cp.DataContext as Track;
                }
                else
                {
                    AlbumViewItem item = dataGrid1.ItemContainerGenerator.ItemFromContainer(dep) as AlbumViewItem;
                    SelectedAlbum = item;
                }
                if ((SelectedAlbum != null || SelectedTrack != null) && OpenContextMenu != null)
                    OpenContextMenu(sender, e);
            }
        }

        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                if (Refresh != null)
                    Refresh(sender, e);
                e.Handled = true;
                return;
            }
        }

        private void GridRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = e.OriginalSource as DependencyObject;
            while ((dep != null) && !(dep is ContentPresenter))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            AlbumViewItem item = dataGrid1.ItemContainerGenerator.ItemFromContainer(dep) as AlbumViewItem;
            SelectedAlbum = item;

            if (SelectionChanged != null)
                SelectionChanged(sender, new EventArgs());
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return Miscellaneous.Misc.GetShortTimeString((int)value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }


}
