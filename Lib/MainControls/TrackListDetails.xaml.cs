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
using Big3.Hitbase.Controls;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using System.IO;
using System.Data;
using System.Collections.ObjectModel;
using Big3.Hitbase.CDUtilities;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewTable.xaml
    /// </summary>
    public partial class TrackListDetails : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private ColumnFieldCollection trackListFields;

        public CurrentViewMode CurrentViewMode { get; set; }

        public TrackListDetails()
        {
            InitializeComponent();
       
        }

        public void FillList()
        {
            this.listBox.ItemsSource = null;

            //AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwTrackList_RunWorkerCompleted);
                backgroundWorker.DoWork += new DoWorkEventHandler(bwTrackList_DoWork);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void bwTrackList_DoWork(object sender, DoWorkEventArgs e)
        {
            string sql = "";

            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.ArtistDetails:
                case MainControls.CurrentViewMode.ComposerDetails:
                    sql = GetArtistDetailsSql();
                    if (Condition != null && Condition.Count > 0 && Condition[0].Value != null)
                    {
                        sql += " HAVING PersonGroup.Name LIKE '" + Condition[0].Value + "%'";
                    }
                    break;

                //case MainControls.CurrentViewMode.ComposerTable:
                //    sql = "SELECT PersonGroup.Name, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength " +
                //        "FROM Track INNER JOIN " +
                //        "PersonGroup ON Track.ComposerID = PersonGroup.PersonGroupID " +
                //        "GROUP BY PersonGroup.Name";
                //    if (Condition != null && Condition.Count > 0)
                //    {
                //        sql += " HAVING PersonGroup.Name LIKE '" + Condition[0].Value + "%'";
                //    }
                //    break;
                         
                //case MainControls.CurrentViewMode.GenreTable:
                //    sql = "SELECT Category.Name, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength " +
                //        "FROM Track INNER JOIN " +
                //        "Category ON Track.CategoryID = Category.CategoryID " +
                //        "GROUP BY Category.Name";
                //    if (Condition != null && Condition.Count > 0)
                //    {
                //        sql += " HAVING Category.Name LIKE '" + Condition[0].Value + "%'";
                //    }
                //    break;
                //case MainControls.CurrentViewMode.YearTable:
                //    sql = "SELECT Track.YearRecorded, COUNT(*) AS TotalCount, SUM(CAST(Track.Length as bigint)) AS TotalLength " +
                //        "FROM Track " +
                //        "GROUP BY Track.YearRecorded";
                //    break;
                //case MainControls.CurrentViewMode.RatingTable:
                //    sql = "SELECT Track.Rating, COUNT(*) AS TotalCount, SUM(CAST(Track.Length as bigint)) AS TotalLength " +
                //        "FROM Track " +
                //        "GROUP BY Track.Rating";
                //    if (Condition != null && Condition.Count > 0)
                //    {
                //        sql += " HAVING Track.Rating = " + Condition[0].Value + "";
                //    }
                //    break;
            }

            SafeObservableCollection<TrackListItem> items = new SafeObservableCollection<TrackListItem>();

            using (DataBaseView view = DataBaseView.Create(this.DataBase, sql))
            {
                object[] values;

                while ((values = view.Read()) != null)
                {
                    TrackListItem trackListItem = new TrackListItem();

                    ReadValues(values, trackListItem);

                    items.AddItemFromThread(trackListItem);
                }

            }

            e.Result = items;
        }

        private void ReadValues(object[] values, TrackListItem trackListItem)
        {
            trackListItem.ID = (int)values[0];
            trackListItem.Name = values[1].ToString();
            trackListItem.ImageFilename = (values[2] is DBNull || values[2] == null) ? "" : values[2].ToString();
            trackListItem.BirthDay = (values[3] is DBNull || values[3] == null) ? "" : values[3].ToString();
            trackListItem.DayOfDeath = (values[4] is DBNull || values[4] == null) ? "" : values[4].ToString();
            trackListItem.Sex = values[5] is DBNull ? SexType.Unknown : (SexType)values[5];
            trackListItem.Type = values[6] is DBNull ? PersonGroupType.Unknown : (PersonGroupType)values[6];
            trackListItem.Url = (values[7] is DBNull || values[7] == null) ? "" : values[7].ToString();
            trackListItem.LandOfOrigin = (values[8] is DBNull || values[8] == null) ? "" : values[8].ToString();
            trackListItem.Count = (int)values[9];
            trackListItem.Length = (long)values[10];
            trackListItem.Rating = (values[11] is DBNull) ? 0 : (int)values[11];

            string timeSpan = "";
            if (!string.IsNullOrEmpty(trackListItem.BirthDay))
            {
                timeSpan += Misc.FormatDate(trackListItem.BirthDay) + " - ";
            }
            if (!String.IsNullOrEmpty(trackListItem.DayOfDeath))
            {
                if (string.IsNullOrEmpty(timeSpan))
                    timeSpan += "- ";

                timeSpan += Misc.FormatDate(trackListItem.DayOfDeath);
            }
            trackListItem.TimeSpan = timeSpan;

            trackListItem.TypeAndSex = string.Format("{0}, {1}", DataBaseEngine.DataBase.GetNameOfPersonGroupType(trackListItem.Type),
                    DataBaseEngine.DataBase.GetNameOfPersonGroupSex(trackListItem.Sex));
        }

        string GetArtistDetailsSql(int id = 0)
        {
            string sql = "";

            if (this.CurrentViewMode == MainControls.CurrentViewMode.ArtistDetails)
            {
                sql = "SELECT PersonGroup.PersonGroupID, PersonGroup.Name, PersonGroup.ImageFilename, PersonGroup.BirthDay, PersonGroup.DayOfDeath, PersonGroup.Sex, PersonGroup.Type, PersonGroup.Url, PersonGroup.Country, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength, AVG(Track.Rating) " +
                    "FROM Track INNER JOIN " +
                    "PersonGroup ON Track.ArtistID = PersonGroup.PersonGroupID " +
                    "GROUP BY PersonGroup.PersonGroupID, PersonGroup.Name, PersonGroup.SaveAs, PersonGroup.ImageFilename, PersonGroup.BirthDay, PersonGroup.DayOfDeath, PersonGroup.Sex, PersonGroup.Type, PersonGroup.Url, PersonGroup.Country ";
            }

            if (this.CurrentViewMode == MainControls.CurrentViewMode.ComposerDetails)
            {
                sql = "SELECT PersonGroup.PersonGroupID, PersonGroup.Name, PersonGroup.ImageFilename, PersonGroup.BirthDay, PersonGroup.DayOfDeath, PersonGroup.Sex, PersonGroup.Type, PersonGroup.Url, PersonGroup.Country, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength, AVG(Track.Rating) " +
                    "FROM Track INNER JOIN " +
                    "PersonGroup ON Track.ComposerID = PersonGroup.PersonGroupID " +
                    "GROUP BY PersonGroup.PersonGroupID, PersonGroup.Name, PersonGroup.SaveAs, PersonGroup.ImageFilename, PersonGroup.BirthDay, PersonGroup.DayOfDeath, PersonGroup.Sex, PersonGroup.Type, PersonGroup.Url, PersonGroup.Country ";
            }

            if (id != 0)
                sql += string.Format(" HAVING PersonGroup.PersonGroupID={0}", id);
                    
            sql += " ORDER BY PersonGroup.SaveAs";

            return sql;
        }

        string GetSql(int id = 0)
        {
            switch (CurrentViewMode)
            {
                case CurrentViewMode.ArtistDetails:
                case CurrentViewMode.ComposerDetails:
                    return GetArtistDetailsSql(id);
                default:
                    return "";
            }
        }

        void UpdateTrackListItem(TrackListItem item)
        {
            string sql = GetSql(item.ID);

            using (DataBaseView view = DataBaseView.Create(this.DataBase, sql))
            {
                object[] values;

                values = view.Read();

                if (values != null)
                {
                    ReadValues(values, item);
                }
            }
        }

        void bwTrackList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //ObservableCollection<TrackListItem> items = e.Result as ObservableCollection<TrackListItem>;
            SafeObservableCollection<TrackListItem> items = e.Result as SafeObservableCollection<TrackListItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            listBox.ItemsSource = lcv;

            if (listBox.Items.Count > 0)
            {
                listBox.UpdateLayout();
                listBox.ScrollIntoView(listBox.Items[0]);
            }


            if (FillListCompleted != null)
                FillListCompleted(this, new EventArgs());

            //UpdateStatusBar();
        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            string filterString = FullTextSearch.ToLower();

            bool found = false;

            TrackListItem trackListItem = row as TrackListItem;

            if (CatalogView.CompareString(trackListItem.Name, filterString))
            {
                found = true;
            }

            return found;
        }



        #region IAlbumView Members

        public void UpdateList()
        {
            ((ListCollectionView)listBox.ItemsSource).Refresh();
        }

        public DataBaseEngine.DataBase DataBase
        {
            get;
            set;
        }

        public DataBaseEngine.Condition Condition
        {
            get;
            set;
        }

        public DataBaseEngine.Condition ConditionFromTree
        {
            get;
            set;
        }

        public string FullTextSearch
        {
            get;
            set;
        }

        public FieldCollection GroupBy
        {
            get;
            set;
        }

        public SortFieldCollection SortFields { get; set; }

        public event EventHandler FillListCompleted;

        public int NumberOfItems
        {
            get
            {
                return listBox.Items.Count;
            }
        }


        public List<int> SelectedCDIDs
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler OpenCD;

        #endregion

        public class TrackListItem  : INotifyPropertyChanged
        {
            private int id; public int ID { get { return id; } set { id = value; FirePropertyChanged("ID"); } }

            private string name; public string Name { get { return name; } set { name = value; FirePropertyChanged("Name"); } }

            private int count; public int Count { get { return count; } set { count = value; FirePropertyChanged("Count"); } }

            private long length; public long Length { get { return length; } set { length = value; FirePropertyChanged("Length"); } }

            private int rating; public int Rating { get { return rating; } set { rating = value; FirePropertyChanged("Rating"); } }

            private string imageFilename; public string ImageFilename { get { return imageFilename; } set { imageFilename = value; FirePropertyChanged("ImageFilename"); } }

            private string birthDay; public string BirthDay { get { return birthDay; } set { birthDay = value; FirePropertyChanged("BirthDay"); } }

            private string dayOfDeath; public string DayOfDeath { get { return dayOfDeath; } set { dayOfDeath = value; FirePropertyChanged("DayOfDeath"); } }

            private SexType sex; public SexType Sex { get { return sex; } set { sex = value; FirePropertyChanged("Sex"); } }

            private PersonGroupType type; public PersonGroupType Type { get { return type; } set { type = value; FirePropertyChanged("Type"); } }

            private string url; public string Url { get { return url; } set { url = value; FirePropertyChanged("Url"); } }

            private string landOfOrigin; public string LandOfOrigin { get { return landOfOrigin; } set { landOfOrigin = value; FirePropertyChanged("LandOfOrigin"); } }

            private string typeAndSex; public string TypeAndSex { get { return typeAndSex; } set { typeAndSex = value; FirePropertyChanged("TypeAndSex"); } }

            private string timeSpan; public string TimeSpan { get { return timeSpan; } set { timeSpan = value; FirePropertyChanged("TimeSpan"); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void FirePropertyChanged(string property)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void itemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CurrentViewMode == MainControls.CurrentViewMode.ArtistDetails ||
                CurrentViewMode == MainControls.CurrentViewMode.ComposerDetails)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ListBoxItem item = VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
                    if (item != null)
                    {
                        TrackListItem tli = item.DataContext as TrackListItem;
                        PersonGroup personGroup = DataBase.GetPersonGroupByName(tli.Name, false);

                        PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase,
                            CurrentViewMode == MainControls.CurrentViewMode.ArtistDetails ? PersonType.Artist : PersonType.Composer, 
                            personGroup);
                        personGroupWindow.ChangeAllSoundFiles = true;
                        personGroupWindow.Owner = Window.GetWindow(this);
                        personGroupWindow.ShowDialog();

                        UpdateTrackListItem(tli);
                    }
                }
            }
        }

        private void ChoosePictureUserControl_ImageChanged(object sender, EventArgs e)
        {
            ChoosePictureUserControl choosePictureUserControl = sender as ChoosePictureUserControl;

            ListBoxItem item = VisualTreeExtensions.FindParent<ListBoxItem>(choosePictureUserControl as DependencyObject);

            if (item != null)
            {
                TrackListItem selItem = item.DataContext as TrackListItem;

                selItem.ImageFilename = choosePictureUserControl.ImageFilename;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;

            TrackListItem clickedItem = hyperlink.DataContext as TrackListItem;

            if (clickedItem != null)
            {
                System.Diagnostics.Process.Start(clickedItem.Url);
            }
        }


        public bool Closing()
        {
            return true;
        }
    }
}
