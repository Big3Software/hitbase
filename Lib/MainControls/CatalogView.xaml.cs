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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Controls;
using Big3.Hitbase.DataBaseEngine.TrackDataSetTableAdapters;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SoundEngine;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.CDUtilities;
using System.Windows.Media.Animation;
using Big3.Hitbase.Configuration;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for CatalogListView.xaml
    /// </summary>
    public partial class CatalogView : UserControl, IMainTabInterface
    {
        public delegate void OpenContextMenuHandler(object sender, ContextMenuEventArgs e);
        public event OpenContextMenuHandler OpenContextMenu;

        public event EventHandler DoubleClick;
        public event EventHandler SelectionChanged;

        public event EventHandler DoDragDrop;
        public event EventHandler Refresh;

        private DataBase dataBase;

        public DataBase DataBase
        {
            get 
            { 
                return dataBase; 
            }
            set 
            { 
                dataBase = value; 
                this.ExtendedSearchUserControl.DataBase = value; 
            }
        }


        private Big3.Hitbase.DataBaseEngine.Condition conditionFilter = new DataBaseEngine.Condition();
        public Big3.Hitbase.DataBaseEngine.Condition ConditionFilter 
        {
            get
            {
                return conditionFilter;
            }
            set
            {
                conditionFilter = value;
            }
        }
        public Big3.Hitbase.DataBaseEngine.Condition ConditionFromTree { get; set; }



        public Track SelectedTrack { get; set; }
        public AlbumViewItem SelectedAlbum { get; set; }

        private bool albumView1Visible = true;

        private CurrentViewMode currentViewMode;
        public CurrentViewMode CurrentViewMode
        {
            get
            {
                return currentViewMode;
            }
            set
            {
                if (currentViewMode != value)
                {
                    SetNewViewMode(value);
                }
            }
        }

        public ShowItemType ShowItemType { get; set; }

        private void SetNewViewMode(MainControls.CurrentViewMode value)
        {
            currentViewMode = value;

            ExtendedSearchUserControl.FieldType = FieldType.TrackAndCD;

            switch (value)
            {
                case CurrentViewMode.None:
                    break;
                case CurrentViewMode.MyMusicDetails:
                    View = new MyMusicDetails() { ShowItemType = this.ShowItemType };
                    splitButtonView.Image = ImageLoader.FromResource("ViewDetails.png");
                    if (this.ShowItemType == MainControls.ShowItemType.Directory)
                    {
                        Settings.Current.DirectoryViewType = 0;
                    }
                    else
                    {
                        Settings.Current.MyMusicViewType = 0;
                    }
                    break;
                case CurrentViewMode.MyMusicTable:
                    splitButtonView.Image = ImageLoader.FromResource("ViewTable.png");

                    if (this.ShowItemType == MainControls.ShowItemType.Directory)
                    {
                        Settings.Current.DirectoryViewType = 1;
                    }
                    else
                    {
                        Settings.Current.MyMusicViewType = 1;
                    }
                    View = new MyMusicTable() { ShowItemType = this.ShowItemType };
                    break;
                case CurrentViewMode.AlbumTable:
                    View = new AlbumViewTable();
                    splitButtonView.Image = ImageLoader.FromResource("ViewTable.png");
                    Settings.Current.AlbumViewType = 1;
                    ExtendedSearchUserControl.FieldType = FieldType.CD;
                    break;
                case CurrentViewMode.AlbumSymbols:
                    View = new AlbumViewSymbols();
                    splitButtonView.Image = ImageLoader.FromResource("ViewImages.png");
                    Settings.Current.AlbumViewType = 0;

                    ExtendedSearchUserControl.FieldType = FieldType.CD;
                    break;
                case CurrentViewMode.MediumTable:
                    AlbumListTable albumView = new AlbumListTable();
                    splitButtonView.Image = ImageLoader.FromResource("ViewTable.png");
                    albumView.CurrentViewMode = currentViewMode;
                    View = albumView;
                    break;
                case CurrentViewMode.PersonGroupDetails:
                    splitButtonView.Image = ImageLoader.FromResource("ViewImages.png");
                    View = new PersonGroupViewDetails();
                    break;
                case CurrentViewMode.PersonGroupTable:
                    splitButtonView.Image = ImageLoader.FromResource("ViewTable.png");
                    View = new PersonGroupViewTable();
                    break;
                case CurrentViewMode.ArtistTable:
                case CurrentViewMode.ComposerTable:
                case CurrentViewMode.GenreTable:
                case CurrentViewMode.YearTable:
                case CurrentViewMode.RatingTable:
                    splitButtonView.Image = ImageLoader.FromResource("ViewTable.png");
                    TrackListTable trackList = new TrackListTable();
                    trackList.CurrentViewMode = currentViewMode;
                    View = trackList;
                    break;
                case CurrentViewMode.ArtistDetails:
                case CurrentViewMode.ComposerDetails:
                    splitButtonView.Image = ImageLoader.FromResource("ViewImages.png");
                    TrackListDetails trackListDetails = new TrackListDetails();
                    trackListDetails.CurrentViewMode = currentViewMode;
                    View = trackListDetails;
                    break;
                default:
                    break;
            }

            MenuItemCoverView.Visibility = (currentViewMode == MainControls.CurrentViewMode.AlbumSymbols ||
                                           currentViewMode == MainControls.CurrentViewMode.AlbumTable ||
                                           currentViewMode == MainControls.CurrentViewMode.ArtistDetails ||
                                           currentViewMode == MainControls.CurrentViewMode.ArtistTable ||
                                           currentViewMode == MainControls.CurrentViewMode.ComposerDetails ||
                                           currentViewMode == MainControls.CurrentViewMode.ComposerTable) ? 
                                           System.Windows.Visibility.Visible :
                                           System.Windows.Visibility.Collapsed;

            MenuItemAlbumView.Visibility = (currentViewMode == MainControls.CurrentViewMode.MyMusicDetails ||
                                           currentViewMode == MainControls.CurrentViewMode.MyMusicTable) ?
                                           System.Windows.Visibility.Visible :
                                           System.Windows.Visibility.Collapsed;

            if (currentViewMode == MainControls.CurrentViewMode.ArtistDetails ||
                currentViewMode == MainControls.CurrentViewMode.ArtistTable)
            {
                MenuItemCoverView.Header = StringTable.ImageView;
            }
            else
            {
                MenuItemCoverView.Header = StringTable.AlbumView;
            }

            this.ExtendedSearchToggleButton.Visibility =
                (CurrentViewMode == MainControls.CurrentViewMode.MyMusicTable ||
                CurrentViewMode == MainControls.CurrentViewMode.MyMusicDetails ||
                CurrentViewMode == MainControls.CurrentViewMode.AlbumSymbols ||
                CurrentViewMode == MainControls.CurrentViewMode.AlbumTable) ?
                Visibility.Visible :
                Visibility.Collapsed;

            if (albumView1Visible)
            {
                albumView1Visible = false;
                AlbumViewTransitionBox.Content = View as UserControl;
            }
            else
            {
                albumView1Visible = true;
                AlbumViewTransitionBox.Content = View as UserControl;
            }

            View.OpenCD += new EventHandler(View_OpenCD);
            View.FillListCompleted += new EventHandler(View_FillListCompleted);
        }

        void View_OpenCD(object sender, EventArgs e)
        {
            if (View.SelectedCDIDs.Count > 0)
            {
                int selectedIDCD = View.SelectedCDIDs[0];
                CD cd = DataBase.GetCDById(selectedIDCD);
                WindowAlbum windowAlbum = new WindowAlbum(cd, DataBase);
                windowAlbum.Owner = Window.GetWindow(this);

                windowAlbum.ShowDialog();
            }
        }

        private IAlbumView View;

        /// <summary>
        /// Hiernach wird die Ausgabe gefiltert.
        /// </summary>
        public string FilterString 
        {
            set
            {
                View.FullTextSearch = value;
            }
        }

        public FieldCollection GroupBy
        {
            get
            {
                return View.GroupBy;
            }
            set
            {
                View.GroupBy = value;
            }
        }

        // Kann ich nicht im XAML setzen (WPF regression!)
        ScaleTransform extendedSearchScaleTransform;
        public CatalogView()
        {
            InitializeComponent();

            ExtendedSearchUserControl.Condition = this.ConditionFilter;
            extendedSearchScaleTransform = new ScaleTransform();
            ExtendedSearchUserControl.LayoutTransform = extendedSearchScaleTransform;
            ExtendedSearchUserControl.StartSearch += new EventHandler(ExtendedSearchUserControl_StartSearch);
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
            if (View == null)
                return;

            //MainGridAlbumView.Content = View;
            View.DataBase = DataBase;
            View.Condition = ConditionFilter;
            View.ConditionFromTree = ConditionFromTree;

            if (SearchTextBox.HasText)
            {
                FilterString = SearchTextBox.Text;
            }
            else
            {
                FilterString = "";
            }

            WaitProgress.Visibility = System.Windows.Visibility.Visible;

            View.FillList();
        }

        void View_FillListCompleted(object sender, EventArgs e)
        {
            WaitProgress.Visibility = System.Windows.Visibility.Collapsed;

            UpdateStatusBar();
        }

        public void UpdateList()
        {
            View.UpdateList();

            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            TextBlockNumberOfItems.Text = string.Format(StringTable.NumberOfItems, View.NumberOfItems);
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

        private void MenuItemExtendedView_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentViewMode == CurrentViewMode.MyMusicTable)
            {
                CurrentViewMode = CurrentViewMode.MyMusicDetails;
            }
            if (CurrentViewMode == CurrentViewMode.AlbumTable)
                CurrentViewMode = CurrentViewMode.AlbumSymbols;
            if (CurrentViewMode == CurrentViewMode.PersonGroupTable)
                CurrentViewMode = CurrentViewMode.PersonGroupDetails;
            if (CurrentViewMode == MainControls.CurrentViewMode.ArtistTable)
                CurrentViewMode = CurrentViewMode.ArtistDetails;
            if (CurrentViewMode == MainControls.CurrentViewMode.ComposerTable)
                CurrentViewMode = CurrentViewMode.ComposerDetails;
                
            FillList();
        }

        private void MenuItemDetails_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentViewMode == MainControls.CurrentViewMode.MyMusicDetails)
            {
                CurrentViewMode = CurrentViewMode.MyMusicTable;
            }
            if (CurrentViewMode == MainControls.CurrentViewMode.AlbumSymbols)
                CurrentViewMode = CurrentViewMode.AlbumTable;
            if (CurrentViewMode == MainControls.CurrentViewMode.PersonGroupDetails)
                CurrentViewMode = CurrentViewMode.PersonGroupTable;
            if (CurrentViewMode == MainControls.CurrentViewMode.ComposerDetails)
                CurrentViewMode = CurrentViewMode.ComposerTable;
            if (CurrentViewMode == MainControls.CurrentViewMode.ArtistDetails)
                CurrentViewMode = CurrentViewMode.ArtistTable;
            FillList();
        }


        private void OpenCD(int cdid)
        {
            CD cd = DataBase.GetCDById(cdid);
        }

        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.HasText)
            {
                FilterString = SearchTextBox.Text;
            }
            else
            {
                FilterString = "";
            }
            UpdateList();
        }

        void ExtendedSearchUserControl_StartSearch(object sender, EventArgs e)
        {
            this.ConditionFilter = ExtendedSearchUserControl.Condition;
            FillList();
        }


        private void splitButtonView_Click(object sender, RoutedEventArgs e)
        {
            switch (CurrentViewMode)
            {
                case CurrentViewMode.MyMusicDetails:
                    CurrentViewMode = CurrentViewMode.MyMusicTable;
                    break;
                case CurrentViewMode.MyMusicTable:
                    CurrentViewMode = CurrentViewMode.MyMusicDetails;
                    break;

                case CurrentViewMode.AlbumSymbols:
                    CurrentViewMode = CurrentViewMode.AlbumTable;
                    break;
                case CurrentViewMode.AlbumTable:
                    CurrentViewMode = CurrentViewMode.AlbumSymbols;
                    break;
                case CurrentViewMode.PersonGroupDetails:
                    CurrentViewMode = CurrentViewMode.PersonGroupTable;
                    break;
                case CurrentViewMode.PersonGroupTable:
                    CurrentViewMode = CurrentViewMode.PersonGroupDetails;
                    break;

                case MainControls.CurrentViewMode.ArtistTable:
                    CurrentViewMode = CurrentViewMode.ArtistDetails;
                    break;

                case MainControls.CurrentViewMode.ArtistDetails:
                    CurrentViewMode = CurrentViewMode.ArtistTable;
                    break;

                case MainControls.CurrentViewMode.ComposerTable:
                    CurrentViewMode = CurrentViewMode.ComposerDetails;
                    break;

                case MainControls.CurrentViewMode.ComposerDetails:
                    CurrentViewMode = CurrentViewMode.ComposerTable;
                    break;
                default:
                    break;
            }

            FillList();
        }

        private void CommandBindingDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.View.SelectedCDIDs.Count == 0)
                return;

            if (this.View.SelectedCDIDs.Count == 1)
            {
                CD cd = DataBase.GetCDById(this.View.SelectedCDIDs[0]);

                if (cd == null)
                    return;

                string msg = string.Format("{0}:\t{1}\r\n{2}:\t\t{3}", StringTable.Artist, cd.Artist, StringTable.Album, cd.Title);

                if (cd.Type == AlbumType.ManagedSoundFiles)
                {
                    if (cd.Tracks.Count == 1)
                        msg += string.Format("\r\n\r\n" + StringTable.DeletePhysicallySingle);
                    else
                        msg += string.Format("\r\n\r\n" + StringTable.DeletePhysicallyMulti, cd.Tracks.Count);
                    WpfMessageBoxResult result = WPFMessageBox.Show(Window.GetWindow(this),
                        StringTable.DeleteAlbum,
                        msg,
                        ImageLoader.GetImageFilenameOrDefault(cd.CDCoverFrontFilename),
                        WpfMessageBoxButtons.DeleteCancel);

                    if (result == WpfMessageBoxResult.Delete)
                    {
                        foreach (Track track in cd.Tracks)
                        {
                            try
                            {
                                File.Delete(track.Soundfile);
                            }
                            catch
                            {
                                // Nicht so schlimm, wenn das nicht klappt.
                            }
                        }
                        DataBase.DeleteCd(cd.ID);
                        View.FillList();
                    }
                }
                else
                {
                    WpfMessageBoxResult result = WPFMessageBox.Show(Window.GetWindow(this),
                        StringTable.DeleteAlbum,
                        msg,
                        ImageLoader.GetImageFilenameOrDefault(cd.CDCoverFrontFilename),
                        WpfMessageBoxButtons.DeleteCancel);

                    if (result == WpfMessageBoxResult.Delete)
                    {
                        DataBase.DeleteCd(cd.ID);
                        View.FillList();
                    }
                }

                return;
            }

            if (this.View.SelectedCDIDs.Count > 1)
            {
                string msg = string.Format(StringTable.DeleteMultipleCDs, this.View.SelectedCDIDs.Count);

                WpfMessageBoxResult result = WPFMessageBox.Show(Window.GetWindow(this),
                    msg,
                    "",
                    "/Big3.Hitbase.SharedResources;component/Images/CDCover.png",
                    WpfMessageBoxButtons.DeleteCancel);

                if (result == WpfMessageBoxResult.Delete)
                {
                    foreach (int cdid in this.View.SelectedCDIDs)
                    {
                        DataBase.DeleteCd(cdid);
                    }

                    View.FillList();
                }
            }

            e.Handled = true;
        }

        private void CommandBindingDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = View.SelectedCDIDs.Count > 0;
        }

        private void CommandBindingRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FillList();
        }

        private void CommandChangeView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeViewCommandParameters changeViewParams = e.Parameter as ChangeViewCommandParameters;

            CurrentViewMode = changeViewParams.ViewMode;
            ConditionFilter = changeViewParams.Condition;
            FillList();
        }

        private void CommandBindingSaveCondition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string searchName = "";

            for (int i = 0; i < 99; i++)
            {
                if (i > 0)
                {
                    searchName = string.Format("{0} ({1})", StringTable.UntitledSearch, i + 1);
                }
                else
                {
                    searchName = StringTable.UntitledSearch;
                }

                if (this.DataBase.GetSearchIdByName(searchName) <= 0)
                    break;
            }

            this.DataBase.AddSearch(searchName, this.ConditionFilter, (int)CurrentViewMode);

            ConditionCommands.SaveCondition.Execute(searchName, Application.Current.MainWindow);
        }

        public void ShowExtendedSearch()
        {
            ExtendedSearchUserControl.Condition = ConditionFilter;

            ExtendedSearchToggleButton.IsChecked = true;

            FadeInExtendedSearchUserControl();
        }

        private void ButtonExtendedSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ExtendedSearchUserControl.Visibility == System.Windows.Visibility.Collapsed)
            {
                FadeInExtendedSearchUserControl();
            }
            else
            {
                FadeOutExtendedSearchUserControl();

                if (ConditionFilter != null)
                {
                    ConditionFilter.Clear();
                    ExtendedSearchUserControl.Condition = ConditionFilter;

                    FillList();
                }
            }
        }

        private void FadeInExtendedSearchUserControl()
        {
            ExtendedSearchUserControl.Visibility = System.Windows.Visibility.Visible;
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200).Duration());
            ExtendedSearchUserControl.BeginAnimation(UserControl.OpacityProperty, da);

            this.extendedSearchScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);

            this.ButtonSaveCondition.Visibility = System.Windows.Visibility.Visible;
        }

        private void FadeOutExtendedSearchUserControl()
        {
            DoubleAnimation da = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200).Duration());
            da.Completed += delegate
            {
                ExtendedSearchUserControl.Visibility = System.Windows.Visibility.Collapsed;
                this.ButtonSaveCondition.Visibility = System.Windows.Visibility.Collapsed;
            };

            ExtendedSearchUserControl.BeginAnimation(UserControl.OpacityProperty, da);
            this.extendedSearchScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, da);
        }

        private void CommandBindingOpenTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Track track = null;
            if (e.Parameter is int)
            {
                track = DataBase.GetTrackById((int)e.Parameter);
            }
            else
            {
                track = e.Parameter as Track;
            }

            if (track != null)
            {
                CD cd = DataBase.GetCDById(track.CDID);
                int trackIndex = cd.FindTrackIndexByTrackID(track.ID);
                WindowAlbum windowAlbum = new WindowAlbum(cd, DataBase, trackIndex);
                windowAlbum.Owner = Window.GetWindow(this);

                windowAlbum.ShowDialog();
            }
        }

        private void MenuItemCoverView_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentViewMode == CurrentViewMode.AlbumTable)
                CurrentViewMode = CurrentViewMode.AlbumSymbols;

            if (CurrentViewMode == CurrentViewMode.ArtistTable)
                CurrentViewMode = CurrentViewMode.ArtistDetails;

            if (CurrentViewMode == CurrentViewMode.ComposerTable)
                CurrentViewMode = CurrentViewMode.ComposerDetails;

            FillList();
        }


        public bool Closing()
        {
            if (this.View != null)
                return this.View.Closing();
            else
                return true;
        }

        public void Restore(RegistryKey regKey)
        {
            string xml = (string)regKey.GetValue("ConditionFromTree", "");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Big3.Hitbase.DataBaseEngine.Condition));
            using (StringReader sr = new StringReader(xml))
            {
                Big3.Hitbase.DataBaseEngine.Condition condition = (Big3.Hitbase.DataBaseEngine.Condition)xmlSerializer.Deserialize(sr);

                int viewMode = (int)regKey.GetValue("CurrentViewMode", 0);
                CurrentViewMode = (CurrentViewMode)viewMode;
                this.ConditionFromTree = condition;
                FillList();
            }

        }

        public void Save(RegistryKey regKey)
        {
            regKey.SetValue("CurrentViewMode", (int)CurrentViewMode);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Big3.Hitbase.DataBaseEngine.Condition));
            StringWriter sw = new StringWriter();
            xmlSerializer.Serialize(sw, ConditionFromTree);

            regKey.SetValue("ConditionFromTree", (string)sw.ToString());

            sw.Close();
        }

        private void CommandBindingEditSort_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SortFieldCollection sfc = View.SortFields;
            bool cdOnly = false;

            if (View is AlbumViewTable || View is AlbumViewSymbols)
                cdOnly = true;

            SortUserControl sortUserControl = new SortUserControl(DataBase, cdOnly, sfc);
            GlobalServices.ModalService.NavigateTo(sortUserControl, StringTable.Sort, delegate(bool returnValue)
            {
                if (returnValue == true)
                {
                    View.SortFields = sortUserControl.SortFields;
                    FillList();
                }
            });
        }

        private void CommandBindingEditSort_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = View != null && View.SortFields != null;
        }

        internal static bool CompareString(string str, string filter)
        {
            if (filter.EndsWith("*"))
            {
                filter = filter.Left(filter.Length - 1);
                if (str != null && str.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return true;
                }
            }
            else
            {
                if (str != null && str.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void CommandBindingSearch_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SearchTextBox.Focus();
        }


    }

    public class AlbumViewItemBase
    {
    }

    public class AlbumViewTitle : AlbumViewItemBase
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
    }

    public class AlbumViewItem : AlbumViewItemBase, INotifyPropertyChanged
    {
        public override string ToString()
        {
            return Artist + " - " + Title;
        }

        public int ID { get; set; }

        private string artist;
        public string Artist
        {
            get { return artist; }
            set { artist = value; FirePropertyChanged("Artist"); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; FirePropertyChanged("Title"); }
        }

        private string genre;
        public string Genre
        {
            get { return genre; }
            set { genre = value; FirePropertyChanged("Genre"); }
        }

        private string year;
        public string Year
        {
            get { return year; }
            set { year = value; FirePropertyChanged("Year"); }
        }

        private string archiveNumber;
        public string ArchiveNumber
        {
            get { return archiveNumber; }
            set { archiveNumber = value; FirePropertyChanged("ArchiveNumber"); }
        }

        private string imageFilename;
        public string ImageFilename
        {
            get { return imageFilename; }
            set { imageFilename = value; FirePropertyChanged("ImageFilename"); }
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public SafeObservableCollection<Track> Tracks { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public enum CurrentViewMode
    {
        None,
        MyMusicDetails,
        MyMusicTable,
        AlbumTable,
        AlbumSymbols,
        PersonGroupDetails,
        PersonGroupTable,
        ArtistTable,
        ArtistDetails,
        ComposerTable,
        ComposerDetails,
        GenreTable,
        MediumTable,
        YearTable,
        RatingTable
    }

    public interface IAlbumView
    {
        void FillList();
        void UpdateList();
        bool Closing();

        DataBase DataBase { get; set; }
        Big3.Hitbase.DataBaseEngine.Condition Condition { get; set; }
        Big3.Hitbase.DataBaseEngine.Condition ConditionFromTree { get; set; }
        string FullTextSearch { get; set; }
        FieldCollection GroupBy { get; set; }
        SortFieldCollection SortFields { get; set; }
        int NumberOfItems { get; }
        List<int> SelectedCDIDs { get; }

        event EventHandler OpenCD;

        event EventHandler FillListCompleted;
    }
}

