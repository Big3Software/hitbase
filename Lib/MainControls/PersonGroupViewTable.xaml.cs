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
using Big3.Hitbase.DataBaseEngine.PersonGroupCatalogViewDataSetTableAdapters;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewDetails.xaml
    /// </summary>
    public partial class PersonGroupViewTable : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;

        public PersonGroupViewTable()
        {
            InitializeComponent();
        }

        private void itemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //AlbumViewItem avi = itemsControl.SelectedItem as AlbumViewItem;
//            OpenCD(avi.ID);
        }


        #region IAlbumView Members

        public void FillList()
        {
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
            List<PersonGroupViewItem> items = new List<PersonGroupViewItem>();

            PersonGroupCatalogViewDataSet personGroupsDataSet = new PersonGroupCatalogViewDataSet();
            PersonGroupTableAdapter ta = new PersonGroupTableAdapter(DataBase);
            ta.Fill(personGroupsDataSet.PersonGroup);

            foreach (PersonGroupCatalogViewDataSet.PersonGroupRow row in personGroupsDataSet.PersonGroup)
            {
                // Leeren Interpreten nicht anzeigen. Kann schon mal angelegt werden.
                if (string.IsNullOrEmpty(row.Name))
                    continue;

                PersonGroupViewItem newItem = new PersonGroupViewItem();
                newItem.ID = row.PersonGroupID;
                newItem.Name = row.Name;
                newItem.ImageFilename = row.ImageFilename;
                newItem.URL = row.URL;
                newItem.NumberOfTracks = row.NumberOfTracks;
                newItem.PersonGroupType = DataBaseEngine.DataBase.GetNameOfPersonGroupType(row.IsTypeNull() ? PersonGroupType.Unknown : (PersonGroupType)row.Type);
                newItem.Sex = DataBaseEngine.DataBase.GetNameOfPersonGroupSex(row.IsSexNull() ? SexType.Unknown : (SexType)row.Sex);
                newItem.LandOfOrigin = row.Country;

                newItem.DayOfBirth = Misc.FormatDate(row.BirthDay);
                newItem.DayOfDeath = Misc.FormatDate(row.DayOfDeath);
                items.Add(newItem);
            }

            e.Result = items;
        }

        void bwAlbumView_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            List<PersonGroupViewItem> items = e.Result as List<PersonGroupViewItem>;
            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            dataGrid.ItemsSource = lcv;

            if (FillListCompleted != null)
                FillListCompleted(this, new EventArgs());
        }


        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            PersonGroupViewItem pgvi = row as PersonGroupViewItem;

            if (CatalogView.CompareString(pgvi.Name, FullTextSearch))
                return true;

            if (CatalogView.CompareString(pgvi.LandOfOrigin, FullTextSearch))
                return true;

            return false;
        }

        public void UpdateList()
        {
            if (dataGrid.ItemsSource is ListCollectionView)
                ((ListCollectionView)dataGrid.ItemsSource).Refresh();
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

        public Big3.Hitbase.DataBaseEngine.Condition Condition { get; set; }

        public event EventHandler FillListCompleted;

        public int NumberOfItems
        {
            get
            {
                return dataGrid.Items.Count;
            }
        }

        public List<int> SelectedCDIDs
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler OpenCD;

        #endregion


        public DataBaseEngine.Condition ConditionFromTree
        {
            get;
            set;
        }


        public bool Closing()
        {
            return true;
        }
    }
}
