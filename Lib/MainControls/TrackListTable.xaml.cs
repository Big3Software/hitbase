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
    public partial class TrackListTable : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private ColumnFieldCollection trackListFields;

        public CurrentViewMode CurrentViewMode { get; set; }

        public ColumnFieldCollection TrackListFields
        {
            get { return trackListFields; }
            set { trackListFields = value; }
        }

        private SortFieldCollection TrackListSort = new SortFieldCollection();

        public ColumnFieldCollection CurrentFields
        {
            get;
            set;
        }

        public TrackListTable()
        {
            InitializeComponent();
        
            ColumnFieldCollection defaultTrackListFields = new ColumnFieldCollection();
            defaultTrackListFields.Add(Field.ArtistTrackName);
            defaultTrackListFields.Add(Field.TrackTitle);
            defaultTrackListFields.Add(Field.TrackLength);
            defaultTrackListFields.Add(Field.TrackRating);
            trackListFields = ColumnFieldCollection.LoadFromRegistry("CatalogTrack", defaultTrackListFields);

            this.dataGrid.ContextMenuOpening += new System.Windows.Controls.ContextMenuEventHandler(TrackListTable_ContextMenuOpening);
        }

        void TrackListTable_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.ArtistTable:
                    this.dataGrid.ContextMenu = FindResource("ContextMenuArtistTable") as ContextMenu;
                    break;
            }
        }

        public void FillList()
        {
            CreateHeader(TrackListFields);
            dataGrid.ItemsSource = null;

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

            Big3.Hitbase.DataBaseEngine.Condition searchCondition = Big3.Hitbase.DataBaseEngine.Condition.Combine(Condition, ConditionFromTree);

            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.ArtistTable:
                    sql = "SELECT PersonGroup.Name, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength " +
                        "FROM Track INNER JOIN " +
                        "PersonGroup ON Track.ArtistID = PersonGroup.PersonGroupID " +
                        "GROUP BY PersonGroup.Name, PersonGroup.SaveAs";
                    if (searchCondition != null && searchCondition.Count > 0)
                    {
                        if (searchCondition[0].Value.ToString() == "?")
                        {
                            sql += " HAVING PersonGroup.SaveAs < 'A' OR PersonGroup.SaveAs > 'ZZZZZ'";
                        }
                        else
                        {
                            sql += " HAVING PersonGroup.SaveAs >= '" + searchCondition[0].Value + "' AND PersonGroup.SaveAs < '" + searchCondition[0].Value + "ZZZZZ'";
                        }
                    }

                    sql += " ORDER BY PersonGroup.SaveAs";

                    break;

                case MainControls.CurrentViewMode.ComposerTable:
                    sql = "SELECT PersonGroup.Name, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength " +
                        "FROM Track INNER JOIN " +
                        "PersonGroup ON Track.ComposerID = PersonGroup.PersonGroupID " +
                        "GROUP BY PersonGroup.Name, PersonGroup.SaveAs ";
                    if (searchCondition != null && searchCondition.Count > 0)
                    {
                        if (searchCondition[0].Value.ToString() == "?")
                        {
                            sql += " HAVING PersonGroup.SaveAs < 'A' OR PersonGroup.SaveAs > 'ZZZZZ'";
                        }
                        else
                        {
                            sql += " HAVING PersonGroup.SaveAs >= '" + searchCondition[0].Value + "' AND PersonGroup.SaveAs < '" + searchCondition[0].Value + "ZZZZZ'";
                        }
                    }

                    sql += " ORDER BY PersonGroup.SaveAs";

                    break;
                         
                case MainControls.CurrentViewMode.GenreTable:
                    sql = "SELECT Category.Name, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength " +
                        "FROM Track LEFT JOIN " +
                        "CD ON Track.CDID = CD.CDID LEFT JOIN " +
                        "Category ON Track.CategoryID = Category.CategoryID OR CD.CategoryID = Category.CategoryID " +
                        "GROUP BY Category.Name";
                    if (searchCondition != null && searchCondition.Count > 0)
                    {
                        sql += " HAVING Category.Name LIKE '" + searchCondition[0].Value + "%'";
                    }
                    break;
                case MainControls.CurrentViewMode.MediumTable:
                    sql = "SELECT Medium.Name, COUNT(*) AS TotalCount, SUM(Cast(Track.Length as bigint)) AS TotalLength " +
                        "FROM Track INNER JOIN " +
                        "CD ON Track.CDID = CD.CDID LEFT JOIN " +
                        "Medium ON CD.MediumID = Medium.MediumID " +
                        "GROUP BY Medium.Name";
                    if (searchCondition != null && searchCondition.Count > 0)
                    {
                        sql += " HAVING Medium.Name LIKE '" + searchCondition[0].Value + "%'";
                    }
                    break;
                case MainControls.CurrentViewMode.YearTable:
                    sql = "SELECT Track.YearRecorded, COUNT(*) AS TotalCount, SUM(CAST(Track.Length as bigint)) AS TotalLength " +
                        "FROM Track " +
                        "GROUP BY Track.YearRecorded";
                    if (searchCondition != null && searchCondition.Count > 0)
                    {
                        if (searchCondition.Count > 1)
                            sql += " HAVING Track.YearRecorded >= " + searchCondition[0].Value + " AND Track.YearRecorded < " + searchCondition[1].Value;
                        else
                            sql += " HAVING Track.YearRecorded=" + searchCondition[0].Value + "";
                    }
                    break;
                case MainControls.CurrentViewMode.RatingTable:
                    sql = "SELECT Track.Rating, COUNT(*) AS TotalCount, SUM(CAST(Track.Length as bigint)) AS TotalLength " +
                        "FROM Track " +
                        "GROUP BY Track.Rating";
                    if (searchCondition != null && searchCondition.Count > 0 && searchCondition[0].Value != null)
                    {
                        sql += " HAVING Track.Rating = " + searchCondition[0].Value + "";
                    }
                    break;
            }

            SafeObservableCollection<TrackListItem> items = new SafeObservableCollection<TrackListItem>();
            Dictionary<string, TrackListItem> dictItems = new Dictionary<string, TrackListItem>();      // Für schnelleren Zugriff
            using (DataBaseView view = DataBaseView.Create(this.DataBase, sql))
            {
                object[] values;

                while ((values = view.Read()) != null)
                {
                    TrackListItem trackListItem = new TrackListItem();
                    trackListItem.Title = values[0].ToString();
                    if (CurrentViewMode == MainControls.CurrentViewMode.YearTable && trackListItem.Title == "0")
                        trackListItem.Title = StringTable.Undefined;
                        
                    trackListItem.Count = (int)values[1];
                    trackListItem.Length = (long)values[2];
                    trackListItem.Rating = 0;
                    dictItems.Add(trackListItem.Title, trackListItem);
                    items.AddItemFromThread(trackListItem);
                }

            }

            // Rating ermitteln
            sql = "";
            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.ArtistTable:
                    {
                        sql = "SELECT PersonGroup.Name, AVG(CAST(Track.Rating AS decimal)) AS Expr3, PersonGroup.SaveAs " +
                            "FROM Track INNER JOIN " +
                            "PersonGroup ON Track.ArtistID = PersonGroup.PersonGroupID " +
                            "WHERE (Track.Rating > 0) " +
                            "GROUP BY PersonGroup.Name, PersonGroup.SaveAs";
                        break;
                    }
                case MainControls.CurrentViewMode.ComposerTable:
                    {
                        sql = "SELECT PersonGroup.Name, AVG(CAST(Track.Rating AS decimal)) AS Expr3 " +
                            "FROM Track INNER JOIN " +
                            "PersonGroup ON Track.ComposerID = PersonGroup.PersonGroupID " +
                            "WHERE (Track.Rating > 0) " +
                            "GROUP BY PersonGroup.Name";
                        break;
                    }

                case MainControls.CurrentViewMode.GenreTable:
                    {
                        sql = "SELECT category.Name, AVG(CAST(Track.Rating AS decimal)) AS Expr3 " +
                                "FROM Track LEFT JOIN " +
                                "CD ON Track.CDID = CD.CDID LEFT JOIN " +
                                "Category AS Category ON Track.CategoryID = Category.CategoryID OR CD.CategoryID = Category.CategoryID " +
                                "WHERE        (Track.Rating > 0) " +
                                "GROUP BY Category.Name";

                        break;
                    }
                case MainControls.CurrentViewMode.YearTable:
                    {
                        sql = "SELECT Track.YearRecorded, AVG(CAST(Track.Rating AS decimal)) AS Expr3 " +
                            "FROM Track " +
                            "WHERE        (Track.Rating > 0) " +
                            "GROUP BY Track.YearRecorded";

                        break;
                    }
            }

            if (sql != "")
            {
                using (DataBaseView view = DataBaseView.Create(this.DataBase, sql))
                {
                    object[] values;

                    while ((values = view.Read()) != null)
                    {
                        string key = values[0].ToString();

                        if (CurrentViewMode == MainControls.CurrentViewMode.YearTable && key == "0")
                            key = StringTable.Undefined;
                        
                        if (dictItems.ContainsKey(key))
                        {
                            TrackListItem trackListItem = dictItems[key];
                            trackListItem.Rating = (double)(decimal)values[1];
                        }
                    }

                }
            }

            e.Result = items;
        }

        void bwTrackList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //ObservableCollection<TrackListItem> items = e.Result as ObservableCollection<TrackListItem>;
                SafeObservableCollection<TrackListItem> items = e.Result as SafeObservableCollection<TrackListItem>;

                ListCollectionView lcv = new ListCollectionView(items);
                lcv.Filter = FilterRow;
                dataGrid.ItemsSource = lcv;

                if (dataGrid.Items.Count > 0)
                {
                    dataGrid.UpdateLayout();
                    dataGrid.ScrollIntoView(dataGrid.Items[0]);
                }
            }
            finally
            {
                if (FillListCompleted != null)
                    FillListCompleted(this, new EventArgs());
            }
            //UpdateStatusBar();
        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            string filterString = FullTextSearch.ToLower();

            bool found = false;

            TrackListItem trackListItem = row as TrackListItem;

            if (CatalogView.CompareString(trackListItem.Title, filterString))
            {
                found = true;
            }

            return found;
        }


        private void CreateHeader(ColumnFieldCollection fields)
        {
            dataGrid.Columns.Clear();

            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.ArtistTable:
                    AddTextColumn(StringTable.Artist, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
                    AddLengthColumn(StringTable.Length, 50, "Length");
                    AddRatingColumn(StringTable.Rating, 100, "Rating");
                    break;
                case MainControls.CurrentViewMode.ComposerTable:
                    AddTextColumn(StringTable.Composer, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
                    AddLengthColumn(StringTable.Length, 50, "Length");
                    AddRatingColumn(StringTable.Rating, 100, "Rating");
                    break;
                case MainControls.CurrentViewMode.GenreTable:
                    AddTextColumn(StringTable.Genre, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
                    AddLengthColumn(StringTable.Length, 50, "Length");
                    AddRatingColumn(StringTable.Rating, 100, "Rating");
                    break;
                case MainControls.CurrentViewMode.MediumTable:
                    AddTextColumn(StringTable.Genre, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
                    AddLengthColumn(StringTable.Length, 50, "Length");
                    AddRatingColumn(StringTable.Rating, 100, "Rating");
                    break;
                case MainControls.CurrentViewMode.YearTable:
                    AddTextColumn(StringTable.Year, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
                    AddLengthColumn(StringTable.Length, 50, "Length");
                    AddRatingColumn(StringTable.Rating, 100, "Rating");
                    break;
                case MainControls.CurrentViewMode.RatingTable:
                    AddRatingColumn(StringTable.Rating, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
                    AddLengthColumn(StringTable.Length, 50, "Length");
                    break;
            }


        }

        private void AddTextColumn(string header, int width, string bindingPath)
        {
            DataGridTextColumn textCol = new DataGridTextColumn();

            textCol.Binding = new Binding(bindingPath);
            textCol.Header = header;
            textCol.Width = width;

            //textCol.SortMemberPath = bindingPath;
            textCol.IsReadOnly = true;

            dataGrid.Columns.Add(textCol);
        }

        private void AddLengthColumn(string header, int width, string bindingPath)
        {
            DataGridTextColumn textCol = new DataGridTextColumn();

            Binding binding = new Binding(bindingPath);
            binding.Converter = new Big3.Hitbase.Miscellaneous.LongLengthConverter();
            textCol.Binding = binding;
            textCol.Header = header;
            textCol.Width = width;

            //textCol.SortMemberPath = bindingPath;
            textCol.IsReadOnly = true;

            dataGrid.Columns.Add(textCol);
        }

        private void AddRatingColumn(string header, int width, string bindingPath)
        {
            DataGridTemplateColumn nc = new DataGridTemplateColumn();

            System.Windows.DataTemplate template = new System.Windows.DataTemplate();

            System.Windows.FrameworkElementFactory factoryRatingControl = new System.Windows.FrameworkElementFactory(typeof(RatingUserControl));

            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(bindingPath);
            //binding.Converter = new Int32Converter();
            factoryRatingControl.SetBinding(RatingUserControl.RatingProperty, binding);
            factoryRatingControl.SetValue(RatingUserControl.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Left);
            factoryRatingControl.SetValue(RatingUserControl.ReadOnlyProperty, true);

            template.VisualTree = factoryRatingControl;

            nc.CellTemplate = template;
            nc.Header = header;
            nc.Width = width;
            nc.IsReadOnly = true;
            dataGrid.Columns.Add(nc);
        }

        #region IAlbumView Members

        public void UpdateList()
        {
            ((ListCollectionView)dataGrid.ItemsSource).Refresh();
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
                return dataGrid.Items.Count;
            }
        }


        public List<int> SelectedCDIDs
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler OpenCD;

        #endregion

        public class TrackListItem : INotifyPropertyChanged
        {
            private string title;
            public string Title 
            {
                get
                {
                    return title;
                }
                set
                {
                    title = value;

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }

            public int Count { get; set; }

            public long Length { get; set; }

            public double Rating { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
                if (row == null)
                    return;

                switch (CurrentViewMode)
                {
                    case MainControls.CurrentViewMode.ArtistTable:
                    case MainControls.CurrentViewMode.ComposerTable:
                        {
                            OpenSelectedPersonGroup();

                            break;
                        }
                    case MainControls.CurrentViewMode.YearTable:
                        {
                            TrackListItem tli = dataGrid.SelectedItem as TrackListItem;

                            int year = Misc.Atoi(tli.Title);

                            AddViewCommandParameters addViewParams = new AddViewCommandParameters();
                            addViewParams.Condition = new DataBaseEngine.Condition();
                            addViewParams.Condition.Add(new SingleCondition() { Field = Field.TrackYearRecorded, Operator = Operator.Equal, Value = year, Logical = Logical.Or });
                            addViewParams.Condition.Add(new SingleCondition() { Field = Field.YearRecorded, Operator = Operator.Equal, Value = year });

                            addViewParams.ImageResourceString = "Calendar.png"; 
                            addViewParams.Title = tli.Title;
                            addViewParams.ViewMode = MainControls.CurrentViewMode.MyMusicDetails;

                            CatalogViewCommands.AddView.Execute(addViewParams, Application.Current.MainWindow);
                            e.Handled = true;
                            break;
                        }
                    case MainControls.CurrentViewMode.RatingTable:
                        {
                            TrackListItem tli = dataGrid.SelectedItem as TrackListItem;

                            int rating = Misc.Atoi(tli.Title);

                            AddViewCommandParameters addViewParams = new AddViewCommandParameters();
                            addViewParams.Condition = new DataBaseEngine.Condition();
                            addViewParams.Condition.Add(new SingleCondition() { Field = Field.TrackRating, Operator = Operator.Equal, Value = rating });

                            addViewParams.ImageResourceString = "Star.png";
                            addViewParams.Title = tli.Title;
                            addViewParams.ViewMode = MainControls.CurrentViewMode.MyMusicDetails;

                            CatalogViewCommands.AddView.Execute(addViewParams, Application.Current.MainWindow);
                            e.Handled = true;
                            break;
                        }
                    case MainControls.CurrentViewMode.GenreTable:
                        {
                            TrackListItem tli = dataGrid.SelectedItem as TrackListItem;

                            string genre = tli.Title;

                            AddViewCommandParameters addViewParams = new AddViewCommandParameters();
                            addViewParams.Condition = new DataBaseEngine.Condition();
                            addViewParams.Condition.Add(new SingleCondition(Field.Category, Operator.Equal, genre, Logical.Or));
                            addViewParams.Condition.Add(new SingleCondition(Field.TrackCategory, Operator.Equal, genre));

                            addViewParams.ImageResourceString = "Category.png";
                            addViewParams.Title = tli.Title;
                            addViewParams.ViewMode = MainControls.CurrentViewMode.MyMusicDetails;

                            CatalogViewCommands.AddView.Execute(addViewParams, Application.Current.MainWindow);
                            e.Handled = true;
                            break;
                        }
                }
            }
        }

        private void OpenSelectedPersonGroup()
        {
            TrackListItem tli = dataGrid.SelectedItem as TrackListItem;
            if (tli == null)
                return;

            PersonGroup personGroup = DataBase.GetPersonGroupByName(tli.Title, false);

            PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase,
                CurrentViewMode == MainControls.CurrentViewMode.ArtistTable ?
                PersonType.Artist : PersonType.Composer, personGroup);
            personGroupWindow.ChangeAllSoundFiles = true;
            personGroupWindow.Owner = Window.GetWindow(this);
            if (personGroupWindow.ShowDialog() == true)
            {
                tli.Title = personGroup.Name;
            }
        }


        public bool Closing()
        {
            return true;
        }

        private void PersonGroupPropertiesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenSelectedPersonGroup();
        }

        private void PersonGroupPropertiesCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
