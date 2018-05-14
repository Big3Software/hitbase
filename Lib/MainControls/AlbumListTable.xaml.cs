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
    /// Interaction logic for AlbumListTable.xaml
    /// </summary>
    public partial class AlbumListTable : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private ColumnFieldCollection trackListFields;

        public CurrentViewMode CurrentViewMode { get; set; }

        public AlbumListTable()
        {
            InitializeComponent();
      
        }

        public void FillList()
        {
            CreateHeader();
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

            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.MediumTable:
                    sql = "SELECT Medium.Name, COUNT(*) AS TotalCount " +
                        "FROM CD LEFT JOIN " +
                        "Medium ON CD.MediumID = Medium.MediumID " +
                        "GROUP BY Medium.Name";
                    if (Condition != null && Condition.Count > 0)
                    {
                        sql += " HAVING Medium.Name LIKE '" + Condition[0].Value + "%'";
                    }
                    break;
            }

            List<AlbumListItem> items = new List<AlbumListItem>();

            using (DataBaseView view = DataBaseView.Create(this.DataBase, sql))
            {
                object[] values;

                while ((values = view.Read()) != null)
                {
                    AlbumListItem trackListItem = new AlbumListItem();
                    trackListItem.Title = values[0].ToString();
                    trackListItem.Count = (int)values[1];

                    items.Add(trackListItem);
                }

            }

            e.Result = items;
        }

        void bwTrackList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //ObservableCollection<TrackListItem> items = e.Result as ObservableCollection<TrackListItem>;
            List<AlbumListItem> items = e.Result as List<AlbumListItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            dataGrid.ItemsSource = lcv;

            if (dataGrid.Items.Count > 0)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[0]);
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

            AlbumListItem trackListItem = row as AlbumListItem;

            if (trackListItem.Title.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                found = true;
            }

            return found;
        }


        private void CreateHeader()
        {
            dataGrid.Columns.Clear();

            switch (CurrentViewMode)
            {
                case MainControls.CurrentViewMode.MediumTable:
                    AddTextColumn(StringTable.Medium, 200, "Title");
                    AddTextColumn(StringTable.Count, 50, "Count");
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

        public class AlbumListItem 
        {
            public string Title { get; set; }

            public int Count { get; set; }

            public long Length { get; set; }

            public double Rating { get; set; }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject) != null)
            {
                AlbumListItem item = dataGrid.SelectedItem as AlbumListItem;
                if (item != null)
                {
                    switch (CurrentViewMode)
                    {
                        case CurrentViewMode.MediumTable:
                            {
                                ChangeViewCommandParameters changeViewParams = new ChangeViewCommandParameters();
                                changeViewParams.ViewMode = MainControls.CurrentViewMode.AlbumTable;

                                Big3.Hitbase.DataBaseEngine.Condition condition = new Big3.Hitbase.DataBaseEngine.Condition();
                                condition.Add(new SingleCondition(Field.Medium, Operator.Equal, item.Title));
                                changeViewParams.Condition = condition;

                                CatalogViewCommands.ChangeView.Execute(changeViewParams, this);
                                break;
                            }
                    }
                }
            }
        }


        public bool Closing()
        {
            return true;
        }
    }
}
