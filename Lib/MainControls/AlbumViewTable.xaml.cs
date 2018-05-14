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
using Big3.Hitbase.CDUtilities;
using System.Collections;
using System.Collections.ObjectModel;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewTable.xaml
    /// </summary>
    public partial class AlbumViewTable : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;

        private ColumnFieldCollection cdListFields;

        public CurrentViewMode CurrentViewMode { get; set; }

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

        public AlbumViewTable()
        {
            InitializeComponent();

            ColumnFieldCollection defaultcdListFields = new ColumnFieldCollection();
            defaultcdListFields.Add(Field.ArtistCDName);
            defaultcdListFields.Add(Field.Title);
            defaultcdListFields.Add(Field.NumberOfTracks);
            defaultcdListFields.Add(Field.TotalLength);
            defaultcdListFields.Add(Field.Category);
            defaultcdListFields.Add(Field.ArchiveNumber);
            defaultcdListFields.Add(Field.Rating);
            cdListFields = ColumnFieldCollection.LoadFromRegistry("AlbumViewTable", defaultcdListFields);

            SortFieldCollection defaultCdListSort = new SortFieldCollection();
            defaultCdListSort.Add(new SortField(Field.ArtistCDName, SortDirection.Ascending));
            defaultCdListSort.Add(new SortField(Field.Title, SortDirection.Ascending));
            CdListSort = SortFieldCollection.LoadFromRegistry("AlbumViewTableSort", defaultCdListSort);
        }

        private bool isManualEditCommit;
        void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (!isManualEditCommit)
                {
                    isManualEditCommit = true;
                    try
                    {
                        DataGrid grid = (DataGrid)sender;
                        grid.CommitEdit(DataGridEditingUnit.Row, true);

                        AlbumItem item = e.Row.Item as AlbumItem;

                        Field field = (Field)e.Column.GetValue(DataGridExtensions.FieldProperty);
                        CD cd = DataBase.GetCDById(item.ID);
                        if (e.EditingElement is TextBox)
                        {
                            object newValue = item.Items[e.Column.DisplayIndex];
                            cd.SetValueToField(field, newValue);
                        }

                        if (field == Field.Rating)
                        {
                            AlbumItem albumItem = e.Row.Item as AlbumItem;
                            cd.SetValueToField(field, albumItem.Items[e.Column.DisplayIndex]);
                        }

                        cd.Save(this.DataBase);

                        Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(cd);
                    }
                    finally
                    {
                        isManualEditCommit = false;
                    }
                }
            }
        }

        public void FillList()
        {
            CreateHeader(CdListFields);
            dataGrid.ItemsSource = null;

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
            SafeObservableCollection<AlbumItem> items = new SafeObservableCollection<AlbumItem>();

            int count = 0;

            Big3.Hitbase.DataBaseEngine.Condition searchCondition = Big3.Hitbase.DataBaseEngine.Condition.Combine(Condition, ConditionFromTree);

            using (DataBaseView view = AlbumView.CreateView(this.DataBase, this.CdListFields.GetFields(), new SortFieldCollection(), 0, searchCondition))
            {
                object[] values;

                while ((values = view.Read()) != null)
                {
                    AlbumItem newItem = new AlbumItem();
                    newItem.ID = (int)values[0];

                    newItem.Items = new object[values.Length - 1];
                    FillRowValues(newItem, values);
                    items.AddItemFromThread(newItem);

                    count++;
                }
            }

            e.Result = items;
        }

        private void FillRowValues(AlbumItem newItem, object[] values)
        {
            FieldCollection fc = this.CdListFields.GetFields();

            for (int i = 1; i < values.Length; i++)
            {
                if (fc[i - 1] == Field.Comment)
                    newItem.Comment = values[i] is DBNull ? "" : (string)values[i];
                else
                    newItem.Items[i - 1] = values[i];
            }
        }

        void bwCDList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SafeObservableCollection<AlbumItem> items = e.Result as SafeObservableCollection<AlbumItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;

            if (GroupBy != null && GroupBy.Count > 0)
            {
            }

            dataGrid.ItemsSource = lcv;

            LoadSorting();

            MySort mySort = new MySort(DataBase, dataGrid, this.cdListFields, this.CdListSort);
            lcv.CustomSort = mySort;    // provide our own sort    

            if (dataGrid.Items.Count > 0)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[0]);
            }

            if (FillListCompleted != null)
                FillListCompleted(this, new EventArgs());

            SaveDataGridColumnWidths();


            //UpdateStatusBar();
        }

        private void LoadSorting()
        {
            foreach (SortField sortField in this.CdListSort)
            {
                foreach (DataGridColumn col in dataGrid.Columns)
                {
                    Field field = (Field)col.GetValue(DataGridExtensions.FieldProperty);

                    if (field == sortField.Field)
                    {
                        if (sortField.SortDirection == SortDirection.Ascending)
                            col.SortDirection = ListSortDirection.Ascending;
                        else if (sortField.SortDirection == SortDirection.Descending)
                            col.SortDirection = ListSortDirection.Descending;
                        else
                            col.SortDirection = null;
                    }
                }
            }

        }

        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            string filterString = FullTextSearch.ToLower();

            bool found = false;

            AlbumItem cdItem = row as AlbumItem;

            foreach (object value in cdItem.Items)
            {
                if (value != null)
                {
                    if (CatalogView.CompareString(value.ToString(), filterString))
                    {
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }


        private void CreateHeader(ColumnFieldCollection fields)
        {
            dataGrid.Columns.Clear();

            //Int32Converter int32Conv = new Int32Converter();

            int i = 0;

            foreach (ColumnField field in fields)
            {
                Type columnType = DataBase.GetTypeByField(field.Field);
                int width = field.Width;
                string columnName = DataBase.GetNameOfField(field.Field, false);

                switch (field.Field)
                {
                    case Field.Rating:
                        {
                            DataGridRatingColumn nc = new DataGridRatingColumn();

                            System.Windows.DataTemplate template = new System.Windows.DataTemplate();

                            System.Windows.FrameworkElementFactory factoryRatingControl = new System.Windows.FrameworkElementFactory(typeof(RatingUserControl));

                            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Items[" + i + "]");
                            binding.Mode = BindingMode.TwoWay;         
                            //binding.Converter = int32Conv;
                            factoryRatingControl.SetBinding(RatingUserControl.RatingProperty, binding);
                            factoryRatingControl.SetValue(RatingUserControl.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Left);
                            factoryRatingControl.AddHandler(RatingUserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RatingCell_MouseLeftButtonDown));
                            
                            nc.SetValue(DataGridExtensions.FieldProperty, field.Field);
                            //factoryRatingControl.SetValue(RatingUserControl.ReadOnlyProperty, true);

                            template.VisualTree = factoryRatingControl;
                            nc.CanUserSort = true;
                            nc.CellTemplate = template;
                            //nc.CellEditingTemplate = template;
                            nc.Header = columnName;
                            nc.Width = width;
                            // Auf Read-Only setzen, da wir das manuell machen (im MouseLeftButtonDown)
                            nc.IsReadOnly = true;
                            nc.SortMemberPath = "Items[" + i + "]";
                            
                            dataGrid.Columns.Add(nc);
                            break;
                        }
                    case Field.Comment:
                        {
                            DataGridTemplateColumn newMultilineColumn = new DataGridTemplateColumn();
                            newMultilineColumn.Width = field.Width;
                            newMultilineColumn.Header = DataBase.GetNameOfField(field.Field);
                            newMultilineColumn.SetValue(DataGridExtensions.FieldProperty, field.Field);

                            DataTemplate multilineCelltemplate = this.FindResource("CommentTemplate") as DataTemplate;
                            newMultilineColumn.CellTemplate = multilineCelltemplate;
                            newMultilineColumn.CellEditingTemplate = multilineCelltemplate;

                            dataGrid.Columns.Add(newMultilineColumn);
                            break;
                        }
                    default:
                        {
                            DataGridMaxLengthTextColumn nc = new DataGridMaxLengthTextColumn();
                            nc.SetValue(DataGridExtensions.FieldProperty, field.Field);
                            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Items[" + i + "]");
                            if (field.Field == Field.TotalLength)
                                binding.Converter = new Big3.Hitbase.Miscellaneous.LengthConverter();
                            else if (field.Field == Field.Price)
                            {
                                binding.Converter = new PriceConverter();
                            }
                            else if (field.Field == Field.Date)
                            {
                                binding.Converter = new DataBaseDateConverter();
                                binding.ConverterParameter = this.DataBase;
                            }
                            else if (field.Field == Field.AlbumType)
                            {
                                binding.Converter = new AlbumTypeConverter();
                                nc.IsReadOnly = true;
                            }
                            else if (IsUserFieldDateFormat(field.Field))
                            {
                                binding.Converter = new UserFieldDateConverter();
                            }
                            else if (DataBase.GetTypeByField(field.Field) == typeof(int))
                                binding.Converter = new MyInt32Converter();
                            else if (DataBase.GetTypeByField(field.Field) == typeof(bool))
                                binding.Converter = new BoolConverter();
                            
                            nc.Binding = binding;
                            nc.Header = columnName;
                            nc.Width = width;

                            if (DataBase.GetTypeByField(field.Field) == typeof(string))
                            {
                                nc.MaxLength = DataBase.GetMaxStringLength(field.Field);
                            }
                            //nc.IsReadOnly = true;

                            dataGrid.Columns.Add(nc);
                            break;
                        }
                }

                i++;
            }

            CurrentFields = fields;
        }

        private bool IsUserFieldDateFormat(Field field)
        {
            return (field == Field.User1 && DataBase.Master.UserCDFields[0].Type == UserFieldType.Date ||
                field == Field.User2 && DataBase.Master.UserCDFields[1].Type == UserFieldType.Date ||
                field == Field.User3 && DataBase.Master.UserCDFields[2].Type == UserFieldType.Date ||
                field == Field.User4 && DataBase.Master.UserCDFields[3].Type == UserFieldType.Date ||
                field == Field.User5 && DataBase.Master.UserCDFields[4].Type == UserFieldType.Date);
        }

        private void RatingCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>((DependencyObject)e.OriginalSource);
            AlbumItem albumItem = row.DataContext as AlbumItem;

            RatingUserControl ratingUserControl = sender as RatingUserControl;
            int rating = ratingUserControl.Rating;

            CD cd = DataBase.GetCDById(albumItem.ID);
            cd.Rating = rating;
            cd.Save(DataBase);
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
                return this.dataGrid.Items.Count;
            }
        }

        public List<int> SelectedCDIDs
        {
            get 
            {
                List<int> cdids = new List<int>();

                foreach (AlbumItem item in dataGrid.SelectedItems)
                {
                    cdids.Add(item.ID);
                }

                return cdids;
            }
        }

        public event EventHandler OpenCD;

        #endregion

        private void ButtonMultiLineEdit_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            DataGridCell cell = VisualTreeExtensions.FindParent<DataGridCell>(e.OriginalSource as DependencyObject);

            Field field = (Field)cell.Column.GetValue(Big3.Hitbase.Controls.DataGridExtensions.FieldProperty);
            AlbumItem album = row.DataContext as AlbumItem;

            WindowMultiline wm = new WindowMultiline();
            wm.Owner = Window.GetWindow(this);
            object textValue = album.Comment;
            if (textValue != null)
                wm.Text = textValue.ToString();
            wm.Title = DataBase.GetNameOfField(field);
            if (wm.ShowDialog() == true)
            {
                // Im Moment hier nur Read-Only
/*                cell.IsEditing = true;
                track.SetValueToField(field, wm.Text);
                cell.IsEditing = false;*/
            }
        }

        private void DockPanelMultiLineEdit_MouseEnter(object sender, RoutedEventArgs e)
        {
            Button row = VisualTreeExtensions.FindVisualChildByName<Button>(e.OriginalSource as DependencyObject, "MultiLineEditButton");
            row.Visibility = System.Windows.Visibility.Visible;
        }

        private void DockPanelMultiLineEdit_MouseLeave(object sender, RoutedEventArgs e)
        {
            Button row = VisualTreeExtensions.FindVisualChildByName<Button>(e.OriginalSource as DependencyObject, "MultiLineEditButton");
            row.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void dataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject) != null)
            {
                AlbumItem selItem = dataGrid.SelectedItem as AlbumItem;
                if (selItem != null)
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Row);

                    OpenCD(this, new EventArgs());
                    UpdateAlbumItem(selItem);
                    e.Handled = true;
                }
            }
        }

        private void CommandBindingChooseColumns_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormChooseColumnFields formChooseFields = new FormChooseColumnFields(this.DataBase, FieldType.CD, cdListFields);

            if (formChooseFields.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cdListFields = formChooseFields.SelectedFields;
                FillList();
            }
        }

        private void CommandBindingCopyToClipboard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int[] recordIds = GetSelectedIds();

            FormCopyToClipboard formCopyToClipboard = new FormCopyToClipboard(DataBase, recordIds, false);

            formCopyToClipboard.ShowDialog();
        }

        private int[] GetSelectedIds()
        {
            List<int> ids = new List<int>();

            foreach (AlbumItem item in dataGrid.SelectedItems)
                ids.Add(item.ID);

            return ids.ToArray();
        }

        private void dataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;   // prevent the built-in sort from sorting            
            PerformCustomSort(e.Column);

            SaveSorting();
        }

        private void SaveSorting()
        {
            this.CdListSort.Clear();

            foreach (DataGridColumn col in dataGrid.Columns)
            {
                Field field = (Field)col.GetValue(DataGridExtensions.FieldProperty);

                if (col.SortDirection != null)
                {
                    if (col.SortDirection == ListSortDirection.Ascending)
                        this.CdListSort.Add(new SortField(field, SortDirection.Ascending));
                    if (col.SortDirection == ListSortDirection.Descending)
                        this.CdListSort.Add(new SortField(field, SortDirection.Descending));
                }
            }
        }

        private void PerformCustomSort(DataGridColumn column)
        {
            Field field = (Field)column.GetValue(DataGridExtensions.FieldProperty);
            SortDirection sortDirection = column.SortDirection == ListSortDirection.Ascending ? SortDirection.Ascending : SortDirection.Descending;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                CdListSort.Clear();

            SortField sortFieldFound = CdListSort.FirstOrDefault(x => x.Field == field);

            if (sortFieldFound != null)
                sortFieldFound.SortDirection = sortDirection;
            else
                CdListSort.Add(new SortField(field, sortDirection));
            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            column.SortDirection = direction;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            MySort mySort = new MySort(DataBase, dataGrid, this.cdListFields, CdListSort);
            lcv.CustomSort = mySort;    // provide our own sort    
        }

        private void SaveDataGridColumnWidths()
        {
            // Spaltenbreiten auslesen
            int i = 0;
            SortedList<int, ColumnField> displayColumns = new SortedList<int, ColumnField>();

            foreach (DataGridColumn col in dataGrid.Columns)
            {
                ColumnField cf = new ColumnField();

                cf.Width = (int)col.Width.DisplayValue;
                cf.Field = CdListFields[i].Field;

                if (col.DisplayIndex == -1)
                {
                    displayColumns.Add(i, cf);
                }
                else
                {
                    displayColumns.Add(col.DisplayIndex, cf);
                }

                i++;
            }

            ColumnFieldCollection cfc = new ColumnFieldCollection();
            foreach (ColumnField cf in displayColumns.Values)
            {
                cfc.Add(cf);
            }

            CdListFields = cfc;
            cfc.SaveToRegistry("AlbumViewTable");
            CdListSort.SaveToRegistry("AlbumViewTableSort");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveDataGridColumnWidths();
        }

        private void UpdateAlbumItem(AlbumItem albumItem)
        {
            DataBaseView view = AlbumView.CreateView(DataBase, CdListFields.GetFields(), CdListSort, albumItem.ID);

            object[] values = view.Read();

            FillRowValues(albumItem, values);

            ListCollectionView lcv = this.dataGrid.ItemsSource as ListCollectionView;
            lcv.Refresh();
        }

        private void CommandBindingRename_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.dataGrid.BeginEdit();
        }



        public bool Closing()
        {
            return true;
        }

        private void dataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            this.SaveDataGridColumnWidths();
        }

        private void dataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Resize der Spalten
            if (e.OriginalSource is System.Windows.Controls.Primitives.Thumb)
            {
                this.SaveDataGridColumnWidths();

            }
        }
    }

    public class MySort : IComparer
    {
        public MySort(DataBase db, DataGrid dataGrid, ColumnFieldCollection fields, SortFieldCollection sfc)
        {
            DataBase = db;
            DataGrid = dataGrid;
            Fields = fields;

            columnSortDirections = new List<DataGridSortColumn>();

            foreach (SortField sf in sfc)
            {
                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {
                    Field field = (Field)dataGrid.Columns[i].GetValue(DataGridExtensions.FieldProperty);
                    if (field == sf.Field)
                    {
                        columnSortDirections.Add(new DataGridSortColumn() { Field = field, Index = i, SortDirection = (ListSortDirection)dataGrid.Columns[i].SortDirection });
                        break;
                    }
                }
            }
        }

        private ColumnFieldCollection Fields { get; set; }

        private DataGrid DataGrid
        {
            get;
            set;
        }

        private List<DataGridSortColumn> columnSortDirections;

        private DataBase DataBase { get; set; }

        int StringCompare(string s1, string s2, ListSortDirection direction)
        {
            if (s1 == null)
                s1 = "";
            if (s2 == null)
                s2 = "";
            if (direction == ListSortDirection.Ascending)
                return s1.CompareTo(s2);
            return s2.CompareTo(s1);
        }

        int Int32Compare(int val1, int val2, ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
                return val1.CompareTo(val2);
            return val2.CompareTo(val1);
        }

        int DateTimeCompare(DateTime val1, DateTime val2, ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
                return val1.CompareTo(val2);
            return val2.CompareTo(val1);
        }

        int IComparer.Compare(object X, object Y)
        {
            int result = 0;
            int columnIndex = 0;
            foreach (DataGridSortColumn sortItem in columnSortDirections)
            {
                columnIndex = sortItem.Index;

                AlbumItem albumItem1 = (AlbumItem)X;
                AlbumItem albumItem2 = (AlbumItem)Y;

                if (albumItem1.Items[columnIndex] is Int32)
                {
                    int val1 = albumItem1.Items[columnIndex] is DBNull ? 0 : (int)albumItem1.Items[columnIndex];
                    int val2 = albumItem2.Items[columnIndex] is DBNull ? 0 : (int)albumItem2.Items[columnIndex];

                    result = Int32Compare(val1, val2, sortItem.SortDirection);
                }
                else if (IsDateField(Fields[columnIndex].Field))
                {
                    DateTime val1 = albumItem1.Items[columnIndex] is DBNull ? DateTime.MinValue : (DateTime)albumItem1.Items[columnIndex];
                    DateTime val2 = albumItem2.Items[columnIndex] is DBNull ? DateTime.MinValue : (DateTime)albumItem2.Items[columnIndex];

                    result = DateTimeCompare(val1, val2, sortItem.SortDirection);
                }
                else if (Fields[columnIndex].Field == Field.ArchiveNumber && Settings.Current.SortArchiveNumberNumeric)
                {
                    int val1 = albumItem1.Items[columnIndex] is DBNull ? 0 : Misc.Atoi((string)albumItem1.Items[columnIndex]);
                    int val2 = albumItem2.Items[columnIndex] is DBNull ? 0 : Misc.Atoi((string)albumItem2.Items[columnIndex]);

                    result = Int32Compare(val1, val2, sortItem.SortDirection);
                }
                else
                {
                    string str1 = albumItem1.Items[columnIndex].ToString();
                    string str2 = albumItem2.Items[columnIndex].ToString();

                    result = StringCompare(str1, str2, sortItem.SortDirection);
                }

                if (result != 0)
                    return result;
            }

            return result;
        }

        private bool IsDateField(Field field)
        {
            if (field == Field.Created || field == Field.LastModified)
                return true;

            return false;
        }
    }

    internal class AlbumItem : INotifyPropertyChanged
    {
        private int id;
        public int ID 
        { 
            get
            {
                return id;
            }
            set
            {
                id = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
            }
        }

        public object[] Items { get; set; }

        public string Comment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class DataGridSortColumn
    {
        public int Index { get; set; }
        public Field Field { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }

    public class DataGridRatingColumn : DataGridTemplateColumn
    {
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            return base.CommitCellEdit(editingElement);
        }
    }
}
