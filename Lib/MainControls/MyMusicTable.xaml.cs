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
using Big3.Hitbase.CDUtilities;
using System.Collections;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for AlbumViewDetails.xaml
    /// </summary>
    public partial class MyMusicTable : UserControl, IAlbumView
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private CDQueryDataSet CDQuery = null;

        private ColumnFieldCollection trackListFields;
        private SortFieldCollection trackListSort = new SortFieldCollection();
        private FieldCollection internalFields = new FieldCollection();

        public ShowItemType ShowItemType { get; set; }

        public SortFieldCollection SortFields
        {
            get
            {
                return trackListSort;
            }
            set
            {
                trackListSort = value;
                if (ShowItemType == MainControls.ShowItemType.Directory)
                    trackListSort.SaveToRegistry("DirectoryTableSort");
                else
                    trackListSort.SaveToRegistry("MyMusicTableSort");
            }
        }
        public ColumnFieldCollection CurrentFields
        {
            get;
            set;
        }

        public MyMusicTable()
        {
            InitializeComponent();

            ColumnFieldCollection defaultTrackListFields = new ColumnFieldCollection();
            defaultTrackListFields.Add(Field.ArtistTrackName);
            defaultTrackListFields.Add(Field.TrackTitle);
            defaultTrackListFields.Add(Field.Title);
            defaultTrackListFields.Add(Field.TrackRating);
            defaultTrackListFields.Add(Field.TrackCategory);
            defaultTrackListFields.Add(Field.ComposerTrackName);
            defaultTrackListFields.Add(Field.TrackYearRecorded);
            defaultTrackListFields.Add(Field.TrackSoundFile);
            trackListFields = ColumnFieldCollection.LoadFromRegistry("MyMusicTable", defaultTrackListFields);

            // Brauch ich für die farbliche Einfärbung der Tracks
            internalFields.Add(Field.TrackSoundFile);
        }

        #region IAlbumView Members

        public void FillList()
        {
            if (dataGrid.Columns.Count > 0)
            {
                SaveDataGridColumnWidths();
            }

            SortFieldCollection defaultCdListSort = new SortFieldCollection();
            if (ShowItemType == MainControls.ShowItemType.Directory)
            {
                defaultCdListSort.Add(new SortField(Field.TrackSoundFile, SortDirection.Ascending));
                trackListSort = SortFieldCollection.LoadFromRegistry("DirectoryTableSort", defaultCdListSort);
            }
            else
            {
                defaultCdListSort.Add(new SortField(Field.ArtistTrackName, SortDirection.Ascending));
                defaultCdListSort.Add(new SortField(Field.TrackTitle, SortDirection.Ascending));
                trackListSort = SortFieldCollection.LoadFromRegistry("MyMusicTableSort", defaultCdListSort);
            }

            CreateHeader(trackListFields);
            this.dataGrid.ItemsSource = null;

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
            SafeObservableCollection<MyMusicListItem> items = new SafeObservableCollection<MyMusicListItem>();

            int count = 0;

            FieldCollection fc = new FieldCollection();

            int internalFieldsCount = 0;
            foreach (Field field in internalFields)
            {
                if (this.trackListFields.SingleOrDefault(x => x.Field == field) == null)
                {
                    fc.Add(field);
                    internalFieldsCount++;
                }
            }

            fc.AddRange(this.trackListFields.GetFields());

            int soundFileIndex = fc.IndexOf(Field.TrackSoundFile);

            Big3.Hitbase.DataBaseEngine.Condition searchCondition = Big3.Hitbase.DataBaseEngine.Condition.Combine(Condition, ConditionFromTree);

            using (DataBaseView view = TrackView.CreateView(this.DataBase, fc, this.trackListSort, 0, searchCondition))
            {
                object[] values;

                while ((values = view.Read()) != null)
                {
                    MyMusicListItem newItem = new MyMusicListItem();

                    FillRowValues(newItem, values, soundFileIndex, internalFieldsCount);
                    items.AddItemFromThread(newItem);

                    count++;
                }
            }

            e.Result = items;
        }

        private void FillRowValues(MyMusicListItem newItem, object[] values, int soundFileIndex, int internalFieldsCount)
        {
            FieldCollection fc = this.trackListFields.GetFields();

            newItem.ID = (int)values[0];
            newItem.Soundfile = values[soundFileIndex+1] is DBNull ? "" : (string)values[soundFileIndex+1];

            newItem.Items = new object[values.Length - internalFieldsCount - 1];

            for (int i = internalFieldsCount + 1; i < values.Length ; i++)
            {
                if (fc[i - internalFieldsCount - 1] == Field.Comment)
                {
                    newItem.Comment = values[i] is DBNull ? "" : (string)values[i];
                }
                else
                {
                    newItem.Items[i - internalFieldsCount - 1] = values[i];
                }
            }
        }

        void bwTrackList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SafeObservableCollection<MyMusicListItem> items = e.Result as SafeObservableCollection<MyMusicListItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;

            if (GroupBy != null && GroupBy.Count > 0)
            {
            }

            dataGrid.ItemsSource = lcv;

            LoadSorting();

            MyMusicTableSort mySort = new MyMusicTableSort(DataBase, dataGrid, this.trackListFields, this.trackListSort);
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
            foreach (SortField sortField in this.trackListSort)
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

        private void SaveSorting()
        {
            this.trackListSort.Clear();

            foreach (DataGridColumn col in dataGrid.Columns)
            {
                Field field = (Field)col.GetValue(DataGridExtensions.FieldProperty);

                if (col.SortDirection != null)
                {
                    if (col.SortDirection == ListSortDirection.Ascending)
                        this.trackListSort.Add(new SortField(field, SortDirection.Ascending));
                    if (col.SortDirection == ListSortDirection.Descending)
                        this.trackListSort.Add(new SortField(field, SortDirection.Descending));
                }
            }
        }


        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FullTextSearch))
                return true;

            string filterString = FullTextSearch.ToLower();

            bool found = false;

            MyMusicListItem trackItem = row as MyMusicListItem;

            foreach (object value in trackItem.Items)
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


        private void dataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;   // prevent the built-in sort from sorting            
            PerformCustomSort(e.Column);

            SaveSorting();
        }

        private void PerformCustomSort(DataGridColumn column)
        {
            Field field = (Field)column.GetValue(DataGridExtensions.FieldProperty);
            SortDirection sortDirection = column.SortDirection == ListSortDirection.Ascending ? SortDirection.Ascending : SortDirection.Descending;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                trackListSort.Clear();

            SortField sortFieldFound = trackListSort.FirstOrDefault(x => x.Field == field);

            if (sortFieldFound != null)
                sortFieldFound.SortDirection = sortDirection;
            else
                trackListSort.Add(new SortField(field, sortDirection));

            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            column.SortDirection = direction;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            MyMusicTableSort mySort = new MyMusicTableSort(DataBase, dataGrid, this.trackListFields, this.trackListSort);
            lcv.CustomSort = mySort;    // provide our own sort    
        }

        public void UpdateList()
        {
            if (dataGrid.ItemsSource is ListCollectionView)
                ((ListCollectionView)dataGrid.ItemsSource).Refresh();
        }

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

        public int NumberOfItems
        {
            get
            {
                return dataGrid.Items.Count;
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

                MyMusicListItem avi = dataGrid.SelectedItem as MyMusicListItem;
                if (avi != null)
                    cdids.Add(avi.ID);
                
                return cdids;
            }
        }

        public event EventHandler OpenCD;

        #endregion

        /// <summary>
        /// Legt den Header der Track-Tabellenansicht an.
        /// </summary>
        /// <param name="fields"></param>
        private void CreateHeader(ColumnFieldCollection fields)
        {
            dataGrid.Columns.Clear();

            CreatePlayControlColumn();

            int i = 0;

            foreach (ColumnField field in fields)
            {
                Type columnType = DataBase.GetTypeByField(field.Field);
                int width = field.Width;
                string columnName = DataBase.GetNameOfFieldFull(field.Field, true);

                switch (field.Field)
                {
                    case Field.Rating:
                    case Field.TrackRating:
                        {
                            DataGridTemplateColumn nc = new DataGridTemplateColumn();

                            System.Windows.DataTemplate template = new System.Windows.DataTemplate();

                            System.Windows.FrameworkElementFactory factoryRatingControl = new System.Windows.FrameworkElementFactory(typeof(RatingUserControl));

                            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Items[" + i + "]");
                            //binding.Converter = int32Conv;
                            factoryRatingControl.SetBinding(RatingUserControl.RatingProperty, binding);
                            factoryRatingControl.SetValue(RatingUserControl.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Left);
                            factoryRatingControl.AddHandler(RatingUserControl.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RatingCell_MouseLeftButtonDown));
                            //factoryRatingControl.SetValue(RatingUserControl.ReadOnlyProperty, true);
                            nc.SetValue(DataGridExtensions.FieldProperty, field.Field);

                            template.VisualTree = factoryRatingControl;
                            nc.CanUserSort = true;
                            nc.CellTemplate = template;
                            nc.Header = columnName;
                            nc.Width = width;
                            nc.SortMemberPath = "Items[" + i + "]";
                            //nc.IsReadOnly = true;
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
                            if (field.Field == Field.TotalLength || field.Field == Field.TrackLength)
                                binding.Converter = new Big3.Hitbase.Miscellaneous.LengthConverter();
                            else if (field.Field == Field.Price)
                                binding.Converter = new PriceConverter();
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
                            else if (field.Field == Field.YearRecorded || field.Field == Field.TrackYearRecorded)
                                binding.Converter = new IntHideZeroConverter();
                            else if (DataBase.GetTypeByField(field.Field) == typeof(int))
                                binding.Converter = new MyInt32Converter();
                            else if (DataBase.GetTypeByField(field.Field) == typeof(bool))
                                binding.Converter = new BoolConverter();

                            nc.Binding = binding;
                            nc.Header = columnName;
                            nc.Width = width;
                            nc.IsReadOnly = !SoundFileInformation.CanChangeID3Tag(field.Field);
                            if (DataBase.GetTypeByField(field.Field) == typeof(string))
                            {
                                nc.MaxLength = DataBase.GetMaxStringLength(field.Field);
                            }

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
                field == Field.User5 && DataBase.Master.UserCDFields[4].Type == UserFieldType.Date ||
                field == Field.TrackUser1 && DataBase.Master.UserTrackFields[0].Type == UserFieldType.Date ||
                field == Field.TrackUser2 && DataBase.Master.UserTrackFields[1].Type == UserFieldType.Date ||
                field == Field.TrackUser3 && DataBase.Master.UserTrackFields[2].Type == UserFieldType.Date ||
                field == Field.TrackUser4 && DataBase.Master.UserTrackFields[3].Type == UserFieldType.Date ||
                field == Field.TrackUser5 && DataBase.Master.UserTrackFields[4].Type == UserFieldType.Date);
        }

        private void RatingCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>((DependencyObject)e.OriginalSource);
            DataGridCell cell = VisualTreeExtensions.FindParent<DataGridCell>((DependencyObject)e.OriginalSource);
            DataGridColumn column = cell.Column;
            RatingUserControl ratingUserControl = sender as RatingUserControl;
            MyMusicListItem musicItem = row.DataContext as MyMusicListItem;

            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (this.dataGrid.SelectedItems.Count > 1)
            {
                WpfMessageBoxResult result = WPFMessageBox.Show(Window.GetWindow(this),
                    StringTable.MultiTagEdit, StringTable.MultiTagEditWarning,
                    "/Big3.Hitbase.SharedResources;component/Images/RenameLarge.png",
                    WpfMessageBoxButtons.YesNo, "MultiTagEdit", false);

                if (result == WpfMessageBoxResult.Yes)
                {
                    Field field = (Field)column.GetValue(DataGridExtensions.FieldProperty);
                    int columnIndex = column.DisplayIndex - 1;
                    musicItem.Items[columnIndex] = ratingUserControl.Rating;
                    PerformMultiEdit(musicItem, field, columnIndex);
                }
            }
            else
            {
                Field field = (Field)column.GetValue(DataGridExtensions.FieldProperty);
                musicItem.Items[column.DisplayIndex - 1] = ratingUserControl.Rating;
                EditTrackField(column.DisplayIndex - 1, field, musicItem);
            }
        }

        private void ButtonMultiLineEdit_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            DataGridCell cell = VisualTreeExtensions.FindParent<DataGridCell>(e.OriginalSource as DependencyObject);

            Field field = (Field)cell.Column.GetValue(Big3.Hitbase.Controls.DataGridExtensions.FieldProperty);
            MyMusicListItem myMusicListItem = row.DataContext as MyMusicListItem;

            WindowMultiline wm = new WindowMultiline();
            wm.Owner = Window.GetWindow(this);
            object textValue = myMusicListItem.Comment;
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


        /// <summary>
        /// Legt die erste Spalte mit den Play-Controls für die Wiedergabe an.
        /// </summary>
        private void CreatePlayControlColumn()
        {
            DataGridTemplateColumn nc = new DataGridTemplateColumn();

            System.Windows.DataTemplate template = new System.Windows.DataTemplate();

            System.Windows.FrameworkElementFactory factoryAddToPlaylistUserControl = new System.Windows.FrameworkElementFactory(typeof(AddToPlaylistUserControl));

            factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PlayNowEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPlayNow));
            factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PlayNextEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPlayNext));
            factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PlayLastEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPlayLast));
            factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PreListenEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPreListen));

            template.VisualTree = factoryAddToPlaylistUserControl;
            nc.CanUserSort = true;
            nc.CellTemplate = template;
            nc.Header = "";
            nc.Width = 66;
            nc.IsReadOnly = true;

            dataGrid.Columns.Add(nc);

        }

        private void CommandBindingChooseColumns_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormChooseColumnFields formChooseFields = new FormChooseColumnFields(this.DataBase, FieldType.TrackAndCD, trackListFields);

            SaveDataGridColumnWidths();

            if (formChooseFields.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                trackListFields = formChooseFields.SelectedFields;
                CreateHeader(trackListFields);
                FillList();
            }
        }

        private void SaveDataGridColumnWidths()
        {
            // Spaltenbreiten auslesen
            int i = 0;
            SortedList<int, ColumnField> displayColumns = new SortedList<int, ColumnField>();
            SortFieldCollection sfc = new SortFieldCollection();

            bool firstColumn = true;
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                // Erste Spalte überspringen (Play-Buttons)
                if (firstColumn)
                {
                    firstColumn = false;
                    continue;
                }

                ColumnField cf = new ColumnField();

                cf.Width = (int)col.Width.DisplayValue;
                cf.Field = trackListFields[i].Field;
                displayColumns.Add(col.DisplayIndex, cf);

                i++;
            }

            ColumnFieldCollection cfc = new ColumnFieldCollection();
            foreach (ColumnField cf in displayColumns.Values)
            {
                cfc.Add(cf);
            }

            trackListFields = cfc;
            cfc.SaveToRegistry("MyMusicTable");
            if (ShowItemType == MainControls.ShowItemType.Directory)
                trackListSort.SaveToRegistry("DirectoryTableSort");
            else
                trackListSort.SaveToRegistry("MyMusicTableSort");
        }

        private void MenuItemAddTracksToPlaylistPlayNow_Click(object sender, RoutedEventArgs e)
        {
            AddTracksFromSearchResult(AddTracksToPlaylistType.Now);
        }

        private void MenuItemAddTracksToPlaylistPlayNext_Click(object sender, RoutedEventArgs e)
        {
            AddTracksFromSearchResult(AddTracksToPlaylistType.Next);
        }

        private void MenuItemAddTracksToPlaylistPlayLast_Click(object sender, RoutedEventArgs e)
        {
            AddTracksFromSearchResult(AddTracksToPlaylistType.End);
        }

        private void MenuItemAddTracksToPlaylistPreListen_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItems.Count == 0)
                return;

            MyMusicListItem item = dataGrid.SelectedItems[0] as MyMusicListItem;
            Track track = DataBase.GetTrackById(item.ID);

            HitbaseCommands.PreListenTrack.Execute(track, this);
        }

        private void AddTracksFromSearchResult(AddTracksToPlaylistType addTracksToPlaylistType)
        {
            List<string> filenames = new List<string>();

            foreach (MyMusicListItem item in dataGrid.SelectedItems)
            {
                string soundfile = DataBase.GetSoundfileByTrackId(item.ID);
                filenames.Add(soundfile);
            }

            AddTracksFromSearchResult(filenames, addTracksToPlaylistType);
        }

        private void AddTracksFromSearchResult(string filename, AddTracksToPlaylistType addTracksToPlaylistType)
        {
            List<string> filenames = new List<string>();
            filenames.Add(filename);

            AddTracksFromSearchResult(filenames, addTracksToPlaylistType);
        }

        private void AddTracksFromSearchResult(List<string> filenames, AddTracksToPlaylistType addTracksToPlaylistType)
        {
            HitbaseCommands.AddTracksToPlaylist.Execute(
                new AddTracksToPlaylistParameter() { Filenames = filenames, AddTracksType = addTracksToPlaylistType }, this);
        }

        private void AddToPlaylistPlayNow(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridRow dataGridRow = VisualTreeExtensions.FindParent<DataGridRow>((System.Windows.DependencyObject)sender);
            MyMusicListItem myMusicListItem = dataGridRow.DataContext as MyMusicListItem;

            if (myMusicListItem != null)
            {
                string soundFile = DataBase.GetSoundfileByTrackId(myMusicListItem.ID);
                AddTracksFromSearchResult(soundFile, AddTracksToPlaylistType.Now);
            }
        }

        private void AddToPlaylistPlayNext(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridRow dataGridRow = VisualTreeExtensions.FindParent<DataGridRow>((System.Windows.DependencyObject)sender);
            MyMusicListItem myMusicListItem = dataGridRow.DataContext as MyMusicListItem;

            if (myMusicListItem != null)
            {
                string soundFile = DataBase.GetSoundfileByTrackId(myMusicListItem.ID);
                AddTracksFromSearchResult(soundFile, AddTracksToPlaylistType.Next);
            }
        }
        private void AddToPlaylistPlayLast(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridRow dataGridRow = VisualTreeExtensions.FindParent<DataGridRow>((System.Windows.DependencyObject)sender);
            MyMusicListItem myMusicListItem = dataGridRow.DataContext as MyMusicListItem;

            if (myMusicListItem != null)
            {
                string soundFile = DataBase.GetSoundfileByTrackId(myMusicListItem.ID);
                AddTracksFromSearchResult(soundFile, AddTracksToPlaylistType.End);
            }
        }

        private void AddToPlaylistPreListen(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridRow dataGridRow = VisualTreeExtensions.FindParent<DataGridRow>((System.Windows.DependencyObject)sender);
            MyMusicListItem myMusicListItem = dataGridRow.DataContext as MyMusicListItem;

            if (myMusicListItem != null)
            {
                Track track = DataBase.GetTrackById(myMusicListItem.ID);

                HitbaseCommands.PreListenTrack.Execute(track, this);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveDataGridColumnWidths();
        }

        private void dataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row != null)
            {
                dataGrid.CancelEdit(DataGridEditingUnit.Row);

                MyMusicListItem listItem = ((MyMusicListItem)row.DataContext);
                Track track = DataBase.GetTrackById(listItem.ID);
                HitbaseCommands.OpenTrack.Execute(track, this);
                e.Handled = true;
                UpdateMyMusicList(listItem);
            }
        }

        private bool isManualEditCommit;

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (!isManualEditCommit)
                {
                    isManualEditCommit = true;
                    try
                    {
                        MyMusicListItem musicItem = e.Row.DataContext as MyMusicListItem;

                        DataGrid grid = (DataGrid)sender;
                        grid.CommitEdit(DataGridEditingUnit.Row, true);

                        if (this.dataGrid.SelectedItems.Count > 1)
                        {
                            WpfMessageBoxResult result = WPFMessageBox.Show(Window.GetWindow(this),
                                StringTable.MultiTagEdit, StringTable.MultiTagEditWarning,
                                "/Big3.Hitbase.SharedResources;component/Images/RenameLarge.png",
                                WpfMessageBoxButtons.YesNo, "MultiTagEdit", false);

                            if (result == WpfMessageBoxResult.Yes)
                            {
                                Field field = (Field)e.Column.GetValue(DataGridExtensions.FieldProperty);
                                int columnIndex = e.Column.DisplayIndex - 1;
                                PerformMultiEdit(musicItem, field, columnIndex);
                            }
                        }
                        else
                        {
                            Field field = (Field)e.Column.GetValue(DataGridExtensions.FieldProperty);

                            EditTrackField(e.Column.DisplayIndex - 1, field, musicItem);
                        }
                    }
                    finally
                    {
                        isManualEditCommit = false;
                    }
                }
            }
        }

        private void PerformMultiEdit(MyMusicListItem musicItem, Field field, int columnIndex)
        {
            WaitProgressUserControl waitProgressUserControl = new WaitProgressUserControl();
            GlobalServices.ModalService.NavigateTo(waitProgressUserControl, StringTable.MultiTagEdit, delegate(bool returnValue)
            {
            }, false);

            waitProgressUserControl.progressBar.Maximum = dataGrid.SelectedItems.Count;

            List<MyMusicListItem> selectedItems = new List<MyMusicListItem>();
            foreach (MyMusicListItem item in dataGrid.SelectedItems)
            {
                selectedItems.Add(item);
            }

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                for (int i=0;i<selectedItems.Count;i++)
                {
                    MyMusicListItem item = selectedItems[i];

                    item.Items[columnIndex] = musicItem.Items[columnIndex];
                    EditTrackField(columnIndex, field, item);

                    if (waitProgressUserControl.Canceled)
                        break;

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        waitProgressUserControl.progressBar.Value++;
                    }));
                }
            };
            bw.RunWorkerCompleted += delegate(object completedSender, RunWorkerCompletedEventArgs completedEventArgs)
            {
                if (completedEventArgs.Error != null)
                {
                    UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(completedEventArgs.Error);
                    unhandledExceptionWindow.ShowDialog();
                }

                GlobalServices.ModalService.CloseModal();

                UpdateList();
            };
            bw.RunWorkerAsync();
        }

        private void EditTrackField(int column, Field field, MyMusicListItem musicItem)
        {
#if false
            int cdId = DataBase.GetCDIDByTrackID(musicItem.ID);
            CD cd = DataBase.GetCDById(cdId);
            int trackIndex = cd.FindTrackIndexByTrackID(musicItem.ID);

            object newValue = musicItem.Items[column];

            if (FieldHelper.IsCDField(field))
                cd.SetValueToField(field, newValue);
            else
                cd.SetTrackValueToField(trackIndex, field, newValue);

            //cd.Save(DataBase, cd.Tracks[trackIndex]);

            Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(cd, musicItem.ID);

            Big3.Hitbase.SoundFilesManagement.SynchronizeCatalogWorker.Instance.ScanFile(cd.Tracks[trackIndex].Soundfile);
#endif
            SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(musicItem.Soundfile);
            
            object newValue = musicItem.Items[column];

            if (newValue != null)
                sfi.SetField(field, newValue.ToString());
            else
                sfi.SetField(field, "");

            Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(sfi, field);

            Big3.Hitbase.SoundFilesManagement.SynchronizeCatalogWorker.Instance.ScanFile(musicItem.Soundfile);
        }

        private void UpdateMyMusicList(MyMusicListItem myMusicListItem)
        {
            FieldCollection fc = new FieldCollection();

            int internalFieldsCount = 0;
            foreach (Field field in internalFields)
            {
                if (this.trackListFields.SingleOrDefault(x => x.Field == field) == null)
                {
                    fc.Add(field);
                    internalFieldsCount++;
                }
            }

            fc.AddRange(this.trackListFields.GetFields());

            int soundFileIndex = fc.IndexOf(Field.TrackSoundFile);

            DataBaseView view = TrackView.CreateView(DataBase, fc, trackListSort, myMusicListItem.ID);

            object[] values = view.Read();

            FillRowValues(myMusicListItem, values, soundFileIndex, internalFieldsCount);

            ListCollectionView lcv = this.dataGrid.ItemsSource as ListCollectionView;
            lcv.Refresh();
        }

        private void CommandBindingRename_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.dataGrid.BeginEdit();
        }

        public bool Closing()
        {
            this.SaveDataGridColumnWidths();
            return true;
        }

        private void CommandBindingCopyToClipboard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int[] recordIds = GetSelectedIds();

            FormCopyToClipboard formCopyToClipboard = new FormCopyToClipboard(DataBase, recordIds, true);

            formCopyToClipboard.ShowDialog();

            e.Handled = true;
        }

        private int[] GetSelectedIds()
        {
            List<int> ids = new List<int>();

            foreach (MyMusicListItem item in dataGrid.SelectedItems)
                ids.Add(item.ID);

            return ids.ToArray();
        }
    }


    public class MyMusicTableSort : IComparer
    {
        public MyMusicTableSort(DataBase db, DataGrid dataGrid, ColumnFieldCollection fields, SortFieldCollection sfc)
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

        private List<DataGridSortColumn> columnSortDirections;

        private ColumnFieldCollection Fields { get; set; }

        private DataGrid DataGrid
        {
            get;
            set;
        }

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
                // Erst Spalte abziehen
                columnIndex = sortItem.Index - 1;

                MyMusicListItem albumItem1 = (MyMusicListItem)X;
                MyMusicListItem albumItem2 = (MyMusicListItem)Y;

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

    internal class MyMusicListItem : INotifyPropertyChanged
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

        public string Soundfile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }


}
