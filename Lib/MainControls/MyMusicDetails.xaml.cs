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
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Controls;
using System.IO;
using System.Windows.Media.Effects;
using System.Data;
using System.Diagnostics;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewDetails.xaml
    /// </summary>
    public partial class MyMusicDetails : UserControl, IAlbumView, INotifyPropertyChanged
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;

        public MyMusicDetails()
        {
            InitializeComponent();
        }

        public ShowItemType ShowItemType { get; set; }

        private int albumColumnWidth = 250;

        public int AlbumColumnWidth
        {
            get { return albumColumnWidth; }
            set 
            { 
                albumColumnWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AlbumColumnWidth"));
            }
        }

        private int numberColumnWidth = 50;

        public int NumberColumnWidth
        {
            get { return numberColumnWidth; }
            set
            {
                numberColumnWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("NumberColumnWidth"));
            }
        }

        private int titleColumnWidth = 200;

        public int TitleColumnWidth
        {
            get { return titleColumnWidth; }
            set
            {
                titleColumnWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TitleColumnWidth"));
            }
        }

        private int lengthColumnWidth = 50;

        public int LengthColumnWidth
        {
            get { return lengthColumnWidth; }
            set
            {
                lengthColumnWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LengthColumnWidth"));
            }
        }

        private int ratingColumnWidth = 130;

        public int RatingColumnWidth
        {
            get { return ratingColumnWidth; }
            set
            {
                ratingColumnWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RatingColumnWidth"));
            }
        }

        private int composerColumnWidth = 150;

        public int ComposerColumnWidth
        {
            get { return composerColumnWidth; }
            set
            {
                composerColumnWidth = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ComposerColumnWidth"));
            }
        }

        private void itemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);

            if (listBoxItem != null && listBoxItem.DataContext is Track)
            {
                HitbaseCommands.OpenTrack.Execute(listBoxItem.DataContext, this);

                ListBoxItem lbItem = VisualTreeExtensions.FindParent<ListBoxItem>(listBoxItem);
                AlbumViewItem item = lbItem.DataContext as AlbumViewItem;
                UpdateAlbumItem(item);
            }
            else
            {
                if (listBoxItem != null && listBoxItem.DataContext is AlbumViewItem)
                {
                    OpenCD(this, new EventArgs());

                    AlbumViewItem item = listBoxItem.DataContext as AlbumViewItem;
                    UpdateAlbumItem(item);
                }
            }
        }

        #region IAlbumView Members

        public void FillList()
        {
            LoadColumnConfiguration();

            this.itemsControl.ItemsSource = null;
            //GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;

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
            FieldCollection fc = new FieldCollection();
            fc.AddRange( new Field[] { Field.CDID, Field.Title, Field.ArchiveNumber, Field.CDCoverFront, Field.YearRecorded, 
                Field.TrackNumber, Field.TrackTitle, Field.TrackLength, Field.TrackRating, Field.TrackSoundFile,
                Field.ArtistCDName, Field.ArtistCDSaveAs, Field.ArtistTrackName, Field.Category, Field.ComposerTrackName});

            int count = 0;

            SortFieldCollection sfc = new SortFieldCollection();
            sfc.Add(Field.ArtistCDSaveAs);
            sfc.Add(Field.Title);
            sfc.Add(Field.CDID);
            // Die Verzeichnisansicht immer sortiert nach Dateiname
            if (this.ShowItemType == ShowItemType.Directory)
                sfc.Add(Field.TrackSoundFile);
            else
                sfc.Add(Field.TrackNumber);

            SafeObservableCollection<AlbumViewItemBase> items = new SafeObservableCollection<AlbumViewItemBase>();

            AlbumViewItem newItem = null;
            string lastArtist = "";
            string lastArtistTitle = "";
            string lastTitle = "";
            int lastcdid = 0;

            Big3.Hitbase.DataBaseEngine.Condition searchCondition = Big3.Hitbase.DataBaseEngine.Condition.Combine(Condition, ConditionFromTree);

            using (DataBaseView view = TrackView.CreateView(DataBase, fc, sfc, 0, searchCondition))
            {
                // Überall auf die Indizes 1 addieren, da die erste Spalte die TrackID ist.
                int colArtistName = fc.IndexOf(Field.ArtistCDName) + 1;
                int colArtistSaveAs = fc.IndexOf(Field.ArtistCDSaveAs) + 1;
                int colTitle = fc.IndexOf(Field.Title) + 1;
                int colCDID = fc.IndexOf(Field.CDID) + 1;
                int colFrontCover = fc.IndexOf(Field.CDCoverFront) + 1;
                int colCategory = fc.IndexOf(Field.Category) + 1;
                int colArchiveNumber = fc.IndexOf(Field.ArchiveNumber) + 1;
                int colYearRecorded = fc.IndexOf(Field.YearRecorded) + 1;
                int colTrackNumber = fc.IndexOf(Field.TrackNumber) + 1;
                int colTrackTitle = fc.IndexOf(Field.TrackTitle) + 1;
                int colTrackLength = fc.IndexOf(Field.TrackLength) + 1;
                int colTrackRating = fc.IndexOf(Field.TrackRating) + 1;
                int colTrackArtist = fc.IndexOf(Field.ArtistTrackName) + 1;
                int colTrackComposer = fc.IndexOf(Field.ComposerTrackName) + 1;
                int colTrackSoundFile = fc.IndexOf(Field.TrackSoundFile) + 1;
                int colTrackID = 0;

                object[] values;

                while ((values = view.Read()) != null)
                {
                    string artistDisplay = values[colArtistName] is DBNull ? "" : (string)values[colArtistName];
                    string artist = values[colArtistSaveAs] is DBNull ? "" : (string)values[colArtistSaveAs];
                    string title = values[colTitle] is DBNull ? "" : (string)values[colTitle];
                    int cdid = (int)values[colCDID];

                    if (cdid != lastcdid)
                    {
                        if (newItem != null)
                        {
                            if (newItem.Artist != lastArtistTitle)
                            {
                                AlbumViewTitle albumTitle = new AlbumViewTitle();
                                albumTitle.Title = newItem.Artist;
                                items.Add(albumTitle);

                                lastArtistTitle = newItem.Artist;
                            }

                            items.Add(newItem);
                        }

                        newItem = new AlbumViewItem();
                        newItem.ID = cdid;
                        newItem.Artist = artistDisplay;
                        newItem.Title = title;
                        newItem.ImageFilename = values[colFrontCover] is DBNull ? "" : (string)values[colFrontCover];
                        newItem.Genre = values[colCategory] is DBNull ? "" : (string)values[colCategory];
                        newItem.ArchiveNumber = values[colArchiveNumber] is DBNull ? "" : (string)values[colArchiveNumber];
                        int yearRecorded = values[colYearRecorded] is DBNull ? 0 : (int)values[colYearRecorded];
                        if (yearRecorded > 0)
                            newItem.Year = yearRecorded.ToString();

                        newItem.Tracks = new SafeObservableCollection<Track>();
                        lastArtist = artist;
                        lastTitle = title;
                    }

                    if (newItem != null)
                    {
                        Track track = new Track();
                        track.TrackNumber = (int)values[colTrackNumber];
                        track.Title = values[colTrackTitle] is DBNull ? "" : (string)values[colTrackTitle];
                        track.Length = (int)values[colTrackLength];
                        track.Rating = values[colTrackRating] is DBNull ? 0 : (int)values[colTrackRating];
                        track.Artist = values[colTrackArtist] is DBNull ? "" : (string)values[colTrackArtist];
                        track.Composer = values[colTrackComposer] is DBNull ? "" : (string)values[colTrackComposer];
                        track.Soundfile = values[colTrackSoundFile] is DBNull ? "" : (string)values[colTrackSoundFile];
                        track.CDID = cdid;
                        track.ID = (int)values[colTrackID];

                        newItem.Tracks.Add(track);
                    }

                    //toolStripStatusProgressBar.Value = (int)(100.0 / TrackView.Rows.Count * count);

                    count++;

                    lastcdid = cdid;
                }
            }


            if (newItem != null)
            {
                if (newItem.Artist != lastArtistTitle)
                {
                    AlbumViewTitle albumTitle = new AlbumViewTitle();
                    albumTitle.Title = newItem.Artist;
                    items.Add(albumTitle);
                }

                items.Add(newItem);
            }

            e.Result = items;
        }

        void bwAlbumView_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            SafeObservableCollection<AlbumViewItemBase> items = e.Result as SafeObservableCollection<AlbumViewItemBase>;
            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            itemsControl.ItemsSource = lcv;

            if (FillListCompleted != null)
                FillListCompleted(this, new EventArgs());
        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            string filterString = FullTextSearch.ToLower();

            bool found = false;

            AlbumViewItem avi = row as AlbumViewItem;

            if (avi == null)
                return false;

            if (CatalogView.CompareString(avi.Artist, filterString))
            {
                return true;
            }
            if (CatalogView.CompareString(avi.Title, filterString))
            {
                return true;
            }
            if (CatalogView.CompareString(avi.Genre, filterString))
            {
                return true;
            }
            if (CatalogView.CompareString(avi.Year, filterString))
            {
                return true;
            }
            if (CatalogView.CompareString(avi.ArchiveNumber, filterString))
            {
                return true;
            }
            
            foreach (Track t in avi.Tracks)
            {
                if (CatalogView.CompareString(t.Artist, filterString))
                {
                    found = true;
                    break;
                }
                if (CatalogView.CompareString(t.Title, filterString))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public void UpdateList()
        {
            if (itemsControl.ItemsSource is ListCollectionView)
                ((ListCollectionView)itemsControl.ItemsSource).Refresh();
        }

        private void ImagePlayNow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Now);
        }

        private void ImagePlayNext_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.Next);
        }

        private void ImagePlayLast_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddTrackFromSearchResult((DependencyObject)e.OriginalSource, AddTracksToPlaylistType.End);
        }


        private void AddTrackFromSearchResult(DependencyObject originalSource, AddTracksToPlaylistType addToPlaylistType)
        {
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack(originalSource);

            List<int> trackIds = new List<int>();
            trackIds.Add(track.ID);
            HitbaseCommands.AddTracksToPlaylist.Execute(
                new AddTracksToPlaylistParameter() { TrackIds = trackIds, AddTracksType = addToPlaylistType }, this);
            /*if (track != null)
            {
                AddTracksToPlaylist(new string[] { track.Soundfile }, addToPlaylistType);
            }*/
        }

        Big3.Hitbase.DataBaseEngine.Track GetTrack(DependencyObject dep)
        {
            ListBoxItem lbi = VisualTreeExtensions.FindParent<ListBoxItem>(dep);

            if (lbi != null)
            {
                ListBox lb = VisualTreeExtensions.FindParent<ListBox>(dep);
                Track track = lb.ItemContainerGenerator.ItemFromContainer(lbi) as Track;

                return track;
            }

            return null;
        }


        #endregion



        #region IAlbumView Members


        public DataBase DataBase
        {
            get;
            set;
        }

        public string FullTextSearch { get; set; }

        public FieldCollection GroupBy
        {
            get;
            set;
        }

        public SortFieldCollection SortFields { get; set; }

        public int NumberOfItems
        {
            get
            {
                int numberOfItems = 0;

                foreach (object item in itemsControl.Items)
                {
                    AlbumViewItem albumViewItem = item as AlbumViewItem;
                    if (albumViewItem != null)
                        numberOfItems += albumViewItem.Tracks.Count;
                }

                return numberOfItems;
            }
        }

        public Big3.Hitbase.DataBaseEngine.Condition Condition { get; set; }

        public DataBaseEngine.Condition ConditionFromTree
        {
            get;
            set;
        }

        public event EventHandler FillListCompleted;

        public List<int> SelectedCDIDs
        {
            get 
            {
                List<int> cdids = new List<int>();

                AlbumViewItem avi = itemsControl.SelectedItem as AlbumViewItem;
                if (avi != null)
                    cdids.Add(avi.ID);
                
                return cdids;
            }
        }

        public event EventHandler OpenCD;

        public event EventHandler OpenTrack;

        #endregion

        private void ImagePlayPreListen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Big3.Hitbase.DataBaseEngine.Track track = GetTrack(e.OriginalSource as DependencyObject);

            HitbaseCommands.PreListenTrack.Execute(track, this);
        }

        // Damit das Mausrad im verschachtelten ListBox nicht abgefangen wird
        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;

            ((ListBox)sender).RaiseEvent(e2);
        }

        private void AddTracksToPlaylistPlayNow_Click(object sender, RoutedEventArgs e)
        {
            AddTracksFromSearchResult(sender, AddTracksToPlaylistType.Now);
        }

        private void AddTracksToPlaylistPlayNext_Click(object sender, RoutedEventArgs e)
        {
            AddTracksFromSearchResult(sender, AddTracksToPlaylistType.Next);
        }

        private void AddTracksToPlaylistPlayLast_Click(object sender, RoutedEventArgs e)
        {
            AddTracksFromSearchResult(sender, AddTracksToPlaylistType.End);
        }

        private void AddTracksToPlaylistPreListen_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = VisualTreeExtensions.FindParent<ContextMenu>(sender as DependencyObject);
            ListBox listBox = cm.PlacementTarget as ListBox;

            if (listBox.SelectedItems.Count == 0)
                return;

            Track track = listBox.SelectedItems[0] as Track;

            HitbaseCommands.PreListenTrack.Execute(track, this);
        }

        private void AddTracksFromSearchResult(object sender, AddTracksToPlaylistType addTracksToPlaylistType)
        {
            MenuItem mi = sender as MenuItem;

            ListBox listBox = mi.DataContext as ListBox;

            List<int> trackIds = new List<int>();

            foreach (Track track in listBox.SelectedItems)
            {
                trackIds.Add(track.ID);
            }

            HitbaseCommands.AddTracksToPlaylist.Execute(
                new AddTracksToPlaylistParameter() { TrackIds = trackIds, AddTracksType = addTracksToPlaylistType }, this);
        }

        private bool columnResizeInProgress = false;
        private double clickedResizePosX = -1;
        private double clickedOriginalSizeX = -1;
        private int clickedResizeColumn = -1;

        private void HeaderTextBlock_MouseMove(object sender, RoutedEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            Point pt = Mouse.GetPosition(GridHeader);
            int col = (int)tb.GetValue(Grid.ColumnProperty);

            if (columnResizeInProgress)
            {
                double newWidth = clickedOriginalSizeX + (pt.X - clickedResizePosX);
                if (newWidth < 10)
                {
                    newWidth = 10;
                }

                // Die erste Spalte darf nicht kleiner sein
                if (clickedResizeColumn == 0 && newWidth < 150)
                    newWidth = 150;

                GridHeader.ColumnDefinitions[clickedResizeColumn].Width = new GridLength(newWidth, GridUnitType.Pixel);

                switch (clickedResizeColumn)
                {
                    case 0:
                        AlbumColumnWidth = (int)newWidth - 70;
                        break;
                    case 1:
                        NumberColumnWidth = (int)newWidth;
                        break;
                    case 2:
                        TitleColumnWidth = (int)newWidth;
                        break;
                    case 3:
                        LengthColumnWidth = (int)newWidth;
                        break;
                    case 4:
                        RatingColumnWidth = (int)newWidth;
                        break;
                    case 5:
                        ComposerColumnWidth = (int)newWidth;
                        break;
                    default:
                        break;
                }

                e.Handled = true;
            }
            else
            {
                pt = Mouse.GetPosition(tb);

                if (pt.X < 2 || pt.X > tb.ActualWidth - 2)
                    tb.Cursor = Cursors.SizeWE;
                else
                    tb.Cursor = null;
            }
        }

        private void HeaderTextBlock_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (columnResizeInProgress)
                return;

            TextBlock tb = sender as TextBlock;

            Point pt = Mouse.GetPosition(tb);
            if (pt.X < 2 || pt.X > tb.ActualWidth - 2)
            {
                int col = (int)tb.GetValue(Grid.ColumnProperty);

                if (pt.X < 2)       // Dann die vorherige Spalte verkleinern
                    col--;

                pt = Mouse.GetPosition(GridHeader);

                clickedResizePosX = pt.X;
                
                clickedOriginalSizeX = GridHeader.ColumnDefinitions[col].ActualWidth;
                tb.CaptureMouse();
                columnResizeInProgress = true;
                clickedResizeColumn = col;
                e.Handled = true;
            }
        }

        private void HeaderTextBlock_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (columnResizeInProgress)
            {
                Mouse.Capture(null);
                columnResizeInProgress = false;
                e.Handled = true;

                SaveColumnConfiguration();
            }
        }

        private void SaveColumnConfiguration()
        {
            int[] columnWidths =
            {
                AlbumColumnWidth,
                NumberColumnWidth,
                TitleColumnWidth,
                LengthColumnWidth,
                RatingColumnWidth,
                ComposerColumnWidth
            };

            string columns = String.Join(",", Array.ConvertAll<int, String>(columnWidths, Convert.ToString));

            Configuration.Settings.SetValue("MyMusicDetailsColumns", columns);
        }

        private void LoadColumnConfiguration()
        {
            string columns = (string)Configuration.Settings.GetValue("MyMusicDetailsColumns", "");

            if (string.IsNullOrEmpty(columns))
                return;

            string[] columnWidths = columns.Split(',');
            if (columnWidths.Length == 6)
            {
                AlbumColumnWidth = Misc.Atoi(columnWidths[0]);
                NumberColumnWidth = Misc.Atoi(columnWidths[1]);
                TitleColumnWidth = Misc.Atoi(columnWidths[2]);
                LengthColumnWidth = Misc.Atoi(columnWidths[3]);
                RatingColumnWidth = Misc.Atoi(columnWidths[4]);
                ComposerColumnWidth = Misc.Atoi(columnWidths[5]);

                GridHeader.ColumnDefinitions[0].Width = new GridLength(AlbumColumnWidth+70, GridUnitType.Pixel);
                GridHeader.ColumnDefinitions[1].Width = new GridLength(NumberColumnWidth, GridUnitType.Pixel);
                GridHeader.ColumnDefinitions[2].Width = new GridLength(TitleColumnWidth, GridUnitType.Pixel);
                GridHeader.ColumnDefinitions[3].Width = new GridLength(LengthColumnWidth, GridUnitType.Pixel);
                GridHeader.ColumnDefinitions[4].Width = new GridLength(RatingColumnWidth, GridUnitType.Pixel);
                GridHeader.ColumnDefinitions[5].Width = new GridLength(ComposerColumnWidth, GridUnitType.Pixel);
            }
        }

        private void UpdateAlbumItem(AlbumViewItem albumItem)
        {
            CD cd = DataBase.GetCDById(albumItem.ID);

            albumItem.ArchiveNumber = cd.ArchiveNumber;
            albumItem.Artist = cd.Artist;
            albumItem.Genre = cd.Category;
            albumItem.ImageFilename = cd.CDCoverFrontFilename;
            albumItem.Title = cd.Title;
            albumItem.Year = cd.YearRecorded == 0 ? "" : cd.YearRecorded.ToString();
            albumItem.Tracks.Clear();
            foreach (Track track in cd.Tracks)
            {
                albumItem.Tracks.Add(track);
            }

           // ListCollectionView lcv = this.itemsControl.ItemsSource as ListCollectionView;
           // lcv.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;



        public bool Closing()
        {
            return true;
        }

        private void RatingUserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RatingUserControl ratingUserControl = sender as RatingUserControl;
            Track track = ratingUserControl.DataContext as Track;

            CD cd = DataBase.GetCDById(track.CDID);
            int trackIndex = cd.FindTrackIndexByTrackID(track.ID);

            cd.SetTrackValueToField(trackIndex, Field.TrackRating, ratingUserControl.Rating);

            Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(cd, track.ID);

            Big3.Hitbase.SoundFilesManagement.SynchronizeCatalogWorker.Instance.ScanFile(cd.Tracks[trackIndex].Soundfile);
        }

        private void ItemContainer_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

    }

    public class AlbumViewDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            MyMusicDetails albumViewDetails = VisualTreeExtensions.FindParent<MyMusicDetails>(container);

            if (item is AlbumViewTitle)
                return albumViewDetails.FindResource("AlbumViewTitleDataTemplate") as DataTemplate;
            else
                return albumViewDetails.FindResource("AlbumViewItemDataTemplate") as DataTemplate; 
        }
    }

    public class TrackColorForegroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string soundFile = null;

            Track track = value as Track;
            if (track != null)
                soundFile = track.Soundfile;

            MyMusicListItem listItem = value as MyMusicListItem;
            if (listItem != null)
                soundFile = listItem.Soundfile;
            if (string.IsNullOrEmpty(soundFile))
            {
                if (Settings.Current.CurrentColorStyle == ColorStyle.Black)
                    return Brushes.White;
                else
                    return Brushes.Black;
            }
            else
            {
                if (File.Exists(soundFile))
                {
                    if (Settings.Current.CurrentColorStyle == ColorStyle.Black)
                        return Brushes.LightGreen;
                    else
                        return Brushes.DarkGreen;
                }
                else
                {
                    if (Settings.Current.CurrentColorStyle == ColorStyle.Black)
                    {
                        SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 192, 192));
                        solidColorBrush.Freeze();
                        return solidColorBrush;
                    }
                    else
                    {
                        return Brushes.DarkRed;
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
