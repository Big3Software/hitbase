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
using System.IO;
using System.Windows.Media.Effects;
using System.Collections.ObjectModel;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewTable.xaml
    /// </summary>
    public partial class AlbumViewSymbols : UserControl, IAlbumView, INotifyPropertyChanged
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;

        private ColumnFieldCollection cdListFields;

        public ColumnFieldCollection CdListFields
        {
            get { return cdListFields; }
            set { cdListFields = value; }
        }

        private SortFieldCollection CdListSort = new SortFieldCollection();

        public ColumnFieldCollection CurrentFields
        {
            get;
            set;
        }

        public AlbumViewSymbols()
        {
            InitializeComponent();
        
            ColumnFieldCollection defaultcdListFields = new ColumnFieldCollection();
            defaultcdListFields.Add(Field.ArtistCDName);
            defaultcdListFields.Add(Field.ArtistCDSaveAs);
            defaultcdListFields.Add(Field.Title);
            defaultcdListFields.Add(Field.CDCoverFront);
            defaultcdListFields.Add(Field.Category);
            defaultcdListFields.Add(Field.ArchiveNumber);
            defaultcdListFields.Add(Field.YearRecorded);
            cdListFields = defaultcdListFields;
            //cdListFields = ColumnFieldCollection.LoadFromRegistry("Catalog", defaultcdListFields);

            SortFieldCollection defaultCdListSort = new SortFieldCollection();
            defaultCdListSort.Add(new SortField(Field.ArtistCDName, SortDirection.Ascending));
            defaultCdListSort.Add(new SortField(Field.Title, SortDirection.Ascending));
            CdListSort = defaultCdListSort;
            //CdListSort = SortFieldCollection.LoadFromRegistry("CatalogSort", defaultCdListSort);
        }

        public void FillList()
        {
            listBox.ItemsSource = null;

            //AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCDList_RunWorkerCompleted);
                backgroundWorker.DoWork += new DoWorkEventHandler(bwCDList_DoWork);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void bwCDList_DoWork(object sender, DoWorkEventArgs e)
        {
            int count = 0;

            SafeObservableCollection<AlbumViewItem> items = new SafeObservableCollection<AlbumViewItem>();

            Big3.Hitbase.DataBaseEngine.Condition searchCondition = Big3.Hitbase.DataBaseEngine.Condition.Combine(Condition, ConditionFromTree);

            using (DataBaseView view = AlbumView.CreateView(this.DataBase, this.CdListFields.GetFields(), this.CdListSort, 0, searchCondition))
            {
                object[] values;

                while ((values = view.Read()) != null)
                {
                    AlbumViewItem newItem = new AlbumViewItem();
                    FillAlbumViewItem(newItem, values);
                    items.AddItemFromThread(newItem);

                    count++;
                }
            }

            e.Result = items;
        }

        private static AlbumViewItem FillAlbumViewItem(AlbumViewItem item, object[] values)
        {
            item.ID = (int)values[0];

            string artistDisplay = (string)values[1];
            string artist = values[2] is DBNull ? "" : (string)values[2];
            string title = values[3] as string;

            item.ID = (int)values[0];
            item.Artist = artistDisplay;
            item.Title = title;
            item.ImageFilename = values[4] is DBNull ? "" : (string)values[4];
            item.Genre = values[5] is DBNull ? "" : (string)values[5];
            item.ArchiveNumber = values[6] is DBNull ? "" : (string)values[6];
            int yearRecorded = values[7] is DBNull ? 0 : (int)values[7];
            if (yearRecorded > 0)
                item.Year = yearRecorded.ToString();

            return item;
        }

        void bwCDList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SafeObservableCollection<AlbumViewItem> items = e.Result as SafeObservableCollection<AlbumViewItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;

            listBox.ItemsSource = lcv;

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

            AlbumViewItem cdItem = row as AlbumViewItem;

            if (CatalogView.CompareString(cdItem.Artist, filterString))
            {
                found = true;
            }

            if (CatalogView.CompareString(cdItem.Title, filterString))
            {
                found = true;
            }

            if (CatalogView.CompareString(cdItem.Year, filterString))
            {
                found = true;
            }

            if (CatalogView.CompareString(cdItem.Genre, filterString))
            {
                found = true;
            }

            if (CatalogView.CompareString(cdItem.ArchiveNumber, filterString))
            {
                found = true;
            }

            return found;
        }

        private void UpdateAlbumViewItem(AlbumViewItem albumViewItem)
        {
            DataBaseView view = AlbumView.CreateView(DataBase, CdListFields.GetFields(), CdListSort, albumViewItem.ID);

            object[] values = view.Read();

            FillAlbumViewItem(albumViewItem, values);

            ListCollectionView lcv = this.listBox.ItemsSource as ListCollectionView;
            lcv.Refresh();
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

        public SortFieldCollection SortFields
        {
            get
            {
                return CdListSort;
            }
            set
            {
                CdListSort = value;
            }
        }

        public event EventHandler FillListCompleted;

        public int NumberOfItems
        {
            get
            {
                return this.listBox.Items.Count;
            }
        }

        public List<int> SelectedCDIDs
        {
            get 
            {
                List<int> cdids = new List<int>();
                AlbumViewItem item = listBox.SelectedItem as AlbumViewItem;
                if (item != null)
                    cdids.Add(item.ID);

                return cdids;
            }
        }

        public event EventHandler OpenCD;

        #endregion


        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<ListBoxItem>(e.OriginalSource as DependencyObject) != null)
            {
                AlbumViewItem selItem = listBox.SelectedItem as AlbumViewItem;
                if (selItem != null)
                {
                    OpenCD(this, new EventArgs());
                    UpdateAlbumViewItem(selItem);
                }
            }
        }

        private void ChoosePictureUserControl_ImageChanged(object sender, EventArgs e)
        {
            ChoosePictureUserControl choosePictureUserControl = sender as ChoosePictureUserControl;
            
            ListBoxItem item = VisualTreeExtensions.FindParent<ListBoxItem>(choosePictureUserControl as DependencyObject);

            if (item != null)
            {
                AlbumViewItem selItem = item.DataContext as AlbumViewItem;

                selItem.ImageFilename = choosePictureUserControl.ImageFilename;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public bool Closing()
        {
            return true;
        }
    }

    public class SelectedIdsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<int> ids = new List<int>();

            foreach (AlbumViewItem item in (System.Collections.IList)value)
            {
                ids.Add(item.ID);
            }

            return ids.ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
