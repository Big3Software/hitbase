using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.DataBaseEngine.CDQueryDataSetTableAdapters;
using Big3.Hitbase.CatalogView3D;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.Miscellaneous;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using Big3.Hitbase.Controls;
using System.Windows.Controls;
using Big3.Hitbase.Configuration;
using System.Windows.Media.Imaging;
using Big3.Hitbase.SoundEngine2;
using Big3.Hitbase.SoundEngineGUI;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Linq;

namespace Big3.Hitbase.CatalogView
{
    public partial class FormCatalog : Form
    {
        private class TreeNodeType
        {
            public TreeNodeType(NodeType nt, int id)
            {
                NodeType = nt;
                ID = id;
            }

            public NodeType NodeType { get; set; }
            public int ID { get; set; }
        }

        public enum NodeType
        {
            Unknown=0,
            Artist=1,
            CD=2,
            Track=3
        }

        public enum ListType
        {
            None=0,
            CDTree,
            CDList,
            TrackTree,
            TrackList,
            AlbumView
        }

        private DataBase dataBase;

        private ColumnFieldCollection cdListFields;

        public ColumnFieldCollection CdListFields
        {
            get { return cdListFields; }
            set { cdListFields = value; }
        }

        private ColumnFieldCollection trackListFields;

        public ColumnFieldCollection TrackListFields
        {
            get { return trackListFields; }
            set { trackListFields = value; }
        }

        /// <summary>
        /// Hiernach wird die Ausgabe gefiltert.
        /// </summary>
        private string FilterString = "";

        public ListType CurrentListType = (ListType)Settings.Current.LastCatalogListType;
        private CDQueryDataSet TrackQuery = null;
        private CDQueryDataSet CDQuery = null;

        private ImageList imgList = new ImageList();

        private ShowPictureForm formShowPicture = new ShowPictureForm();
        private FormLyrics formLyrics = new FormLyrics();

        private CatalogListView dataGrid = new CatalogListView();
        private AlbumView albumView = new AlbumView();
        private CatalogTreeView treeView = new CatalogTreeView();
        private DispatcherTimer timerSearch = new DispatcherTimer();

        private bool cdTreeLoadedWithTracks = false;

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private System.Windows.Forms.Panel panelTreeViewCD = new System.Windows.Forms.Panel();

        private ListType lastListType = ListType.None;

        public Condition Condition
        {
            get;
            set;
        }

        private SortFieldCollection CdListSort = new SortFieldCollection();
        private SortFieldCollection TrackListSort = new SortFieldCollection();

        CueBannerTextBox searchTextBox = new CueBannerTextBox();

        // Die Events werden benötigt, um alte C++ Komponenten starten zu können
        public event OnAddTrackToWishlist AddTrackToWishlist;
        public event OnAddTracksToPlaylist AddTracksToPlaylist;
        public event OnUploadCD UploadCD;
        public event OnPrintCDCover PrintCDCover;

        private FormCD treeviewPreviewFormCD;

        public FormCatalog(DataBase db, Condition cond) : base()
        {
            Condition = cond;
            dataBase = db;
            InitializeComponent();

            elementHost.Child = dataGrid;
            elementHost.Visible = true;

            dataGrid.Margin = new System.Windows.Thickness(0);

            ColumnFieldCollection defaultcdListFields = new ColumnFieldCollection();
            defaultcdListFields.Add(Field.ArtistCDName);
            defaultcdListFields.Add(Field.Title);
            defaultcdListFields.Add(Field.NumberOfTracks);
            defaultcdListFields.Add(Field.TotalLength);
            defaultcdListFields.Add(Field.Category);
            defaultcdListFields.Add(Field.ArchiveNumber);
            defaultcdListFields.Add(Field.Rating);
            cdListFields = ColumnFieldCollection.LoadFromRegistry("Catalog", defaultcdListFields);

            ColumnFieldCollection defaultTrackListFields = new ColumnFieldCollection();
            defaultTrackListFields.Add(Field.ArtistTrackName);
            defaultTrackListFields.Add(Field.TrackTitle);
            defaultTrackListFields.Add(Field.TrackLength);
            defaultTrackListFields.Add(Field.TrackRating);
            trackListFields = ColumnFieldCollection.LoadFromRegistry("CatalogTrack", defaultTrackListFields);

            albumView.DataBase = dataBase;
            albumView.AddTracksToPlaylist += new OnAddTracksToPlaylist(albumView_AddTracksToPlaylist);
            albumView.OpenContextMenu += new AlbumView.OpenContextMenuHandler(albumView_OpenContextMenu);
            albumView.SelectionChanged += new EventHandler(albumView_SelectionChanged);
            albumView.Refresh += new EventHandler(dataGrid_Refresh);

            SortFieldCollection defaultCdListSort = new SortFieldCollection();
            defaultCdListSort.Add(new SortField(Field.ArtistCDName, SortDirection.Ascending));
            defaultCdListSort.Add(new SortField(Field.Title, SortDirection.Ascending));
            CdListSort = SortFieldCollection.LoadFromRegistry("CatalogSort", defaultCdListSort);

            SortFieldCollection defaultTrackListSort = new SortFieldCollection();
            defaultTrackListSort.Add(new SortField(Field.ArtistTrackName, SortDirection.Ascending));
            defaultTrackListSort.Add(new SortField(Field.TrackTitle, SortDirection.Ascending));
            TrackListSort = SortFieldCollection.LoadFromRegistry("CatalogTrackSort", defaultTrackListSort);

            treeView.DataBase = dataBase;
            treeView.OpenContextMenu += new CatalogTreeView.OpenContextMenuHandler(treeView_OpenContextMenu);
            treeView.DoDragDrop += new EventHandler(treeView_DoDragDrop);
            treeView.SelectionChanged += new EventHandler(treeView_SelectionChanged);
            treeView.DeleteItem += new EventHandler(treeView_DeleteItem);
            treeView.DoubleClick += new EventHandler(treeView_DoubleClick);
            treeView.Refresh += new EventHandler(dataGrid_Refresh);

            dataGrid.DataBase = dataBase;
            dataGrid.OpenContextMenu += new Big3.Hitbase.CatalogView.CatalogListView.OpenContextMenuHandler(OpenContextMenu);
            dataGrid.OpenHeaderContextMenu += new CatalogListView.OpenContextMenuHandler(dataGrid_OpenHeaderContextMenu);
            dataGrid.DoubleClick += new EventHandler(dataGrid_DoubleClick);
            dataGrid.SelectionChanged += new EventHandler(dataGrid_SelectionChanged);
            dataGrid.DoDragDrop += new EventHandler(dataGrid_DoDragDrop);
            dataGrid.dataGrid.Sorting += new DataGridSortingEventHandler(dataGrid_Sorting);
            dataGrid.DeleteItem += new EventHandler(dataGrid_DeleteItem);
            dataGrid.Refresh += new EventHandler(dataGrid_Refresh);

            searchTextBox.Width = 150;
            searchTextBox.Left = Width - 170;
            searchTextBox.Top = 2;
            searchTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            searchTextBox.CueBanner = StringTable.Search;
            searchTextBox.TextChanged += new EventHandler(searchTextBox_TextChanged);
            this.Controls.Add(searchTextBox);
            searchTextBox.BringToFront();

            Settings.RestoreWindowPlacement(this, "FormCatalog");

            timerSearch.Interval = TimeSpan.FromMilliseconds(500);
            timerSearch.Tick += new EventHandler(timerSearch_Tick);
        }

        void dataGrid_Refresh(object sender, EventArgs e)
        {
            FillList(true);
        }

        void treeView_DoubleClick(object sender, EventArgs e)
        {
            Track track = GetSelectedTrack();

            if (track != null)
            {
                if (!string.IsNullOrEmpty(track.Soundfile) && File.Exists(track.Soundfile))
                    MiniPlayerWindow.PreListen(track);
                else
                    OpenTrackProperties();
            }
            else
            {
                OpenCD();
            }
        }

        void albumView_AddTracksToPlaylist(string[] filenames, AddToPlaylistType addToPlaylistType)
        {
            if (AddTracksToPlaylist != null)
                AddTracksToPlaylist(filenames, addToPlaylistType);
        }

        void treeView_DeleteItem(object sender, EventArgs e)
        {
            DeleteCD();
        }

        void dataGrid_DeleteItem(object sender, EventArgs e)
        {
            DeleteCD();
        }

        void dataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (CurrentListType == ListType.CDList)
            {
                CdListSort.Clear();
                foreach (DataGridColumn col in dataGrid.Columns)
                {
                    int colIndex = dataGrid.Columns.IndexOf(col);

                    if (col.SortDirection == ListSortDirection.Ascending)
                        CdListSort.Add(new SortField(CdListFields[colIndex].Field, SortDirection.Ascending));
                    if (col.SortDirection == ListSortDirection.Descending)
                        CdListSort.Add(new SortField(CdListFields[colIndex].Field, SortDirection.Descending));
                }
            }

            if (CurrentListType == ListType.TrackList)
            {
                TrackListSort.Clear();
                int colIndex = dataGrid.Columns.IndexOf(e.Column);

                TrackListSort.Add(new SortField(TrackListFields[colIndex-1].Field,
                    e.Column.SortDirection == ListSortDirection.Ascending ?
                    SortDirection.Ascending : SortDirection.Descending));
            }
        }

        void treeView_SelectionChanged(object sender, EventArgs e)
        {
            SetSelectedItemInCDForm();
        }

        void SetSelectedItemInCDForm()
        {
            CD cd = GetSelectedCD();

            if (cd != null)
            {
                treeviewPreviewFormCD.SetCD(cd);
            }
            else
            {
                Track track = GetSelectedTrack();

                if (track != null)
                {
                    CD cd1 = dataBase.GetCDById(track.CDID);

                    int trackIndex = cd1.FindTrackIndexByTrackID(track.ID);

                    if (trackIndex >= 0)
                    {
                        treeviewPreviewFormCD.SetCD(cd1, trackIndex);
                    }
                }
            }

            ShowCDCover();
            ShowLyrics();

            UpdateWindowState();
        }

        void treeView_DoDragDrop(object sender, EventArgs e)
        {
            DoDragDrop();
        }

        void treeView_OpenContextMenu(object sender, ContextMenuEventArgs e)
        {
            OpenContextMenu(MousePosition);            
        }

        void albumView_OpenContextMenu(object sender, ContextMenuEventArgs e)
        {
            OpenContextMenu(MousePosition);
        }

        private void OpenContextMenu(Point point)
        {
            contextMenuStrip.Show(point);
        }

        void dataGrid_DoDragDrop(object sender, EventArgs e)
        {
            DoDragDrop();
        }

        private void DoDragDrop()
        {
            int[] selCDIDs = GetSelectedCDIDs();
            if (selCDIDs.Length > 0)
            {
                List<string> filenames = new List<string>();
                DataObject dataObject = new DataObject();

                foreach (int cdid in selCDIDs)
                {
                    CD cd = dataBase.GetCDById(cdid);

                    foreach (Track track in cd.Tracks)
                    {
                        if (!string.IsNullOrEmpty(track.Soundfile) && File.Exists(track.Soundfile))
                            filenames.Add(track.Soundfile);
                    }
                }

                dataObject.SetData(DataFormats.FileDrop, filenames.ToArray());
                DragDropEffects finalDropEffect = DoDragDrop(dataObject, DragDropEffects.Copy);
            }
            else
            {
                Track[] selTracks = GetSelectedTracks();
                if (selTracks == null)
                    return;

                DataObject dataObject = new DataObject();
                List<string> filenames = new List<string>();
                foreach (Track track in selTracks)
                {
                    if (!string.IsNullOrEmpty(track.Soundfile) && File.Exists(track.Soundfile))
                        filenames.Add(track.Soundfile);
                }
                dataObject.SetData(DataFormats.FileDrop, filenames.ToArray());
                DragDropEffects finalDropEffect = DoDragDrop(dataObject, DragDropEffects.Copy);
            }
        }

        void dataGrid_OpenHeaderContextMenu(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            contextMenuStripHeader.Show(new Point(MousePosition.X, MousePosition.Y));
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            FillList();
        }

        void dataGrid_SelectionChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        void albumView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        void dataGrid_DoubleClick(object sender, EventArgs e)
        {
            Track track = GetSelectedTrack();

            if (track != null)
            {
                if (!string.IsNullOrEmpty(track.Soundfile) && File.Exists(track.Soundfile))
                    MiniPlayerWindow.PreListen(track);
                else
                    OpenTrackProperties();
            }
            else
            {
                OpenCD();
            }
        }

        private void OpenContextMenu(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenContextMenu(MousePosition);
        }

        private DispatcherTimer timer = new DispatcherTimer();
        private void FormCatalog_Load(object sender, EventArgs e)
        {
            Icon = Icons.hitbase;

            string dbFilename = System.IO.Path.GetFileName(dataBase.DataBasePath);
            Text += string.Format(" - [{0}]", dbFilename);

            dataGrid.dataGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(dataGrid_SelectionChanged);

            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void dataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ShowCDCover();
            ShowLyrics();
        }

        private void UpdateToolbar()
        {
            //toolStripButtonCDTree.Checked = (CurrentListType == ListType.CDTree);
            //toolStripButtonCDList.Checked = (CurrentListType == ListType.CDList);
            //toolStripButtonTrackList.Checked = (CurrentListType == ListType.TrackList);
        }

        private void FillList()
        {
            if (Condition != null && Condition.Count > 0)
            {
                labelFilter.Visible = true;
                labelFilter.Text = String.Format(StringTable.Filter, Condition.GetConditionString(dataBase));
            }
            else
            {
                labelFilter.Visible = false;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                toolStripStatusLabelSearch.Visible = true;
                toolStripStatusProgressBar.Visible = true;

                toolStripStatusProgressBar.Maximum = 100;
                toolStripStatusProgressBar.Value = 0;

                FillList(CurrentListType);

                UpdateStatusBar();

                toolStripStatusLabelSearch.Visible = false;
                toolStripStatusProgressBar.Visible = false;

                UpdateWindowState();
            }
            catch (Exception ex)
            {
                FormUnhandledException formUnhandledException = new FormUnhandledException(ex);
                formUnhandledException.ShowDialog();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void UpdateStatusBar()
        {
            switch (CurrentListType)
            {
                case ListType.AlbumView:
                    if (albumView.Items.Count == 1)
                        toolStripStatusLabelCount.Text = string.Format(StringTable.AlbumCountSingle, albumView.Items.Count);
                    else
                        toolStripStatusLabelCount.Text = string.Format(StringTable.AlbumCountMultiple, albumView.Items.Count);
                    break;
                case ListType.CDTree:
                    toolStripStatusLabelCount.Text = string.Format(StringTable.ArtistCount, treeView.TreeView.Items.Count);
                    break;
                case ListType.TrackList:
                    toolStripStatusLabelCount.Text = string.Format(StringTable.TrackCount, dataGrid.Items.Count);
                    break;
                case ListType.CDList:
                    toolStripStatusLabelCount.Text = string.Format(StringTable.CDCount, dataGrid.Items.Count);
                    break;
            }
        }

        private void FillList(bool clearCache)
        {
            if (clearCache == true)
            {
                TrackQuery = null;
                CDQuery = null;
            }

            FillList();
        }

        private void CreateHeader(ColumnFieldCollection fields)
        {
            dataGrid.Columns.Clear();
            ribbonButtonGroupBy.DropDownItems.Clear();

            Int32Converter int32Conv = new Int32Converter();

            int i = 0;

            // In der Track-Liste auch eine Spalte 
            // für die Steuerung der Wiedergabe (Play, PreListen, etc.)
            if (CurrentListType == ListType.TrackList)
            {
                DataGridTemplateColumn nc = new DataGridTemplateColumn();

                System.Windows.DataTemplate template = new System.Windows.DataTemplate();

                System.Windows.FrameworkElementFactory factoryAddToPlaylistUserControl = new System.Windows.FrameworkElementFactory(typeof(AddToPlaylistUserControl));
                template.VisualTree = factoryAddToPlaylistUserControl;
                factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PlayNowEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPlayNow));
                factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PlayNextEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPlayNext));
                factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PlayLastEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPlayLast));
                factoryAddToPlaylistUserControl.AddHandler(AddToPlaylistUserControl.PreListenEvent, new System.Windows.RoutedEventHandler(AddToPlaylistPreListen));

                nc.CellTemplate = template;
                nc.Header = "";
                nc.Width = 70;
                nc.IsReadOnly = true;
                nc.CanUserResize = false;
                nc.CanUserSort = false;
                dataGrid.Columns.Add(nc);
            }

            foreach (ColumnField field in fields)
            {
                Type columnType = dataBase.GetTypeByField(field.Field);
                int width = field.Width;
                string columnName = dataBase.GetNameOfField(field.Field, false);
                
                if (field.Field == Field.Rating || field.Field == Field.TrackRating)
                {
                    DataGridTemplateColumn nc = new DataGridTemplateColumn();

                    System.Windows.DataTemplate template = new System.Windows.DataTemplate();

                    System.Windows.FrameworkElementFactory factoryRatingControl = new System.Windows.FrameworkElementFactory(typeof(RatingUserControl));

                    System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Items[" + i + "]");
                    binding.Converter = int32Conv;
                    factoryRatingControl.SetBinding(RatingUserControl.ValueProperty, binding);
                    factoryRatingControl.SetValue(RatingUserControl.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Left);
                    factoryRatingControl.SetValue(RatingUserControl.ReadOnlyProperty, true);

                    template.VisualTree = factoryRatingControl;

                    nc.CellTemplate = template;
                    nc.Header = columnName;
                    nc.Width = width;
                    nc.IsReadOnly = true;
                    dataGrid.Columns.Add(nc);
                }
                else
                {
                    DataGridBoundColumn nc = new DataGridTextColumn();
                    DataGridItemValueConverter valueConverter = new DataGridItemValueConverter();

                    System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Items[" + i + "]");
                    binding.Converter = valueConverter;
                    nc.Binding = binding;
                    nc.Header = columnName;
                    nc.Width = width;
                    nc.IsReadOnly = true;
                    dataGrid.Columns.Add(nc);
                }

                RibbonButton dropDownItemGroup = new RibbonButton(columnName);
                dropDownItemGroup.Click += new EventHandler(dropDownItemGroup_Click);
                dropDownItemGroup.Tag = i;
                ribbonButtonGroupBy.DropDownItems.Add(dropDownItemGroup);

                i++;
            }

            ribbonButtonGroupBy.DropDownItems.Add(new RibbonSeparator());
            RibbonButton dropDownItemGroupingOff = new RibbonButton(StringTable.GroupingOff);
            dropDownItemGroupingOff.Click += new EventHandler(dropDownItemGroup_Click);
            dropDownItemGroupingOff.Tag = -1;
            ribbonButtonGroupBy.DropDownItems.Add(dropDownItemGroupingOff);

            dataGrid.CurrentFields = fields;
        }


        private void AddToPlaylistPlayNow(object sender, System.Windows.RoutedEventArgs e)
        {
            ContentPresenter contPres = VisualTreeExtensions.FindParent<ContentPresenter>((System.Windows.DependencyObject)sender);
            DataGridItem item = contPres.Content as DataGridItem;
            if (item != null)
                AddTrackToPlaylist(item.Soundfile, AddToPlaylistType.Now);
        }

        private void AddToPlaylistPlayNext(object sender, System.Windows.RoutedEventArgs e)
        {
            ContentPresenter contPres = VisualTreeExtensions.FindParent<ContentPresenter>((System.Windows.DependencyObject)sender);
            DataGridItem item = contPres.Content as DataGridItem;
            if (item != null)
                AddTrackToPlaylist(item.Soundfile, AddToPlaylistType.Next);
        }

        private void AddToPlaylistPlayLast(object sender, System.Windows.RoutedEventArgs e)
        {
            ContentPresenter contPres = VisualTreeExtensions.FindParent<ContentPresenter>((System.Windows.DependencyObject)sender);
            DataGridItem item = contPres.Content as DataGridItem;
            if (item != null)
                AddTrackToPlaylist(item.Soundfile, AddToPlaylistType.End);
        }

        private void AddToPlaylistPreListen(object sender, System.Windows.RoutedEventArgs e)
        {
            ContentPresenter contPres = VisualTreeExtensions.FindParent<ContentPresenter>((System.Windows.DependencyObject)sender);
            DataGridItem item = contPres.Content as DataGridItem;
            if (item != null)
            {
                Track track = dataBase.GetTrackById(item.ID);
                MiniPlayerWindow.PreListen(track);
            }
        }

        private void AddTrackToPlaylist(string filename, AddToPlaylistType addToPlaylistType)
        {
            if (AddTracksToPlaylist != null)
            {
                string[] filenames = { filename };
                AddTracksToPlaylist(filenames, addToPlaylistType);
            }
        }

        void dropDownItemGroup_Click(object sender, EventArgs e)
        {
            int columnIndex = (int)((RibbonButton)sender).Tag;
            if (columnIndex < 0)        // Gruppierung aus
            {   
                ((ListCollectionView)dataGrid.ItemsSource).GroupDescriptions.Clear();
                ((ListCollectionView)dataGrid.ItemsSource).Refresh();
            }
            else
            {
                PropertyGroupDescription pgd = new PropertyGroupDescription("Items[" + columnIndex.ToString() + "].DisplayValue");
                ((ListCollectionView)dataGrid.ItemsSource).GroupDescriptions.Clear();
                ((ListCollectionView)dataGrid.ItemsSource).GroupDescriptions.Add(pgd);
                ((ListCollectionView)dataGrid.ItemsSource).Refresh();
            }

            if (dataGrid.dataGrid.Items.Count > 0)
            {
                dataGrid.dataGrid.UpdateLayout();
                dataGrid.dataGrid.ScrollIntoView(dataGrid.dataGrid.Items[0]);
            }
        }

        private void FillList(ListType listType)
        {
            toolStripStatusProgressBar.Value = 0;

            ribbonButtonAlbumView.Checked = (listType == ListType.AlbumView);
            ribbonButtonCDTree.Checked = (listType == ListType.CDTree);
            ribbonButtonCDList.Checked = (listType == ListType.CDList);
            ribbonButtonTrackList.Checked = (listType == ListType.TrackList);

            switch (listType)
            {
                case ListType.AlbumView:
                    if (lastListType != listType)
                    {
                        if (treeviewPreviewFormCD != null)
                        {
                            CloseTreeViewFormCD();
                        }

                        elementHost.Child = albumView;
                    }
                    FillAlbumView();
                    break;
                case ListType.CDTree:
                    if (lastListType != listType)
                    {
                        elementHost.Child = treeView;
                        //elementHost.Dock = DockStyle.Left;
                        //elementHost.Width = 300;

                        panelTreeViewCD.Dock = DockStyle.Fill;
                        treeviewPreviewFormCD = new FormCD(null, dataBase);
                        treeView.FormCDElementHost.Child = panelTreeViewCD;

                        treeviewPreviewFormCD.TopLevel = false;
                        treeviewPreviewFormCD.Parent = panelTreeViewCD;
                        treeviewPreviewFormCD.Dock = DockStyle.Fill;
                        treeviewPreviewFormCD.FormBorderStyle = FormBorderStyle.None;
                        treeviewPreviewFormCD.ReadOnly = true;
                        treeviewPreviewFormCD.Embedded = true;
                        //this.Controls.Add(panelTreeViewCD);
                        //Controls.SetChildIndex(panelTreeViewCD, 0);
                        treeviewPreviewFormCD.Show();
                    }
                    FillCDTree();
                    break;
                case ListType.CDList:
                    if (lastListType != listType)
                    {
                        if (treeviewPreviewFormCD != null)
                        {
                            CloseTreeViewFormCD();
                        }

                        elementHost.Child = dataGrid;
                    }
                    FillCDList();
                    break;
                case ListType.TrackList:
                    if (lastListType != listType)
                    {
                        if (treeviewPreviewFormCD != null)
                        {
                            CloseTreeViewFormCD();
                        }

                        elementHost.Child = dataGrid;
                    }
                    FillTrackList();
                    break;
                default:
                    break;
            }

            lastListType = listType;
        }

        private void FillAlbumView()
        {
            albumView.ItemsSource = null;
            albumView.GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;

            AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

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

            CDQuery = dataBase.ExecuteTrackQuery();

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
            TrackView = new TrackDataView(dataBase, CDQuery, Condition, sfc, fc);

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
            albumView.GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            List<AlbumViewItem> items = e.Result as List<AlbumViewItem>;
            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            albumView.ItemsSource = lcv;

            UpdateStatusBar();
        }

        private void FillCDTree()
        {
            FillCDTree(false);
        }

        private void FillCDTree(bool loadWithTracks)
        {
            treeView.TreeView.ItemsSource = null;
            treeView.GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;

            AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCDTree_RunWorkerCompleted);
                if (loadWithTracks || !string.IsNullOrEmpty(FilterString) || (Condition != null && Condition.Count > 0))
                    backgroundWorker.DoWork += new DoWorkEventHandler(bwCDTree_DoWork);
                else
                    backgroundWorker.DoWork += new DoWorkEventHandler(bwCDTree_DoWork_CD);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void bwCDTree_DoWork(object sender, DoWorkEventArgs e)
        {
            TrackDataView TrackView;

            CDQuery = dataBase.ExecuteTrackQuery();

            // Hier die Felder auflisten, die ich für die Baumansicht brauche
            FieldCollection fc = new FieldCollection();
            fc.AddRange(new Field[] { 
                Field.ArtistCDName,
                Field.ArtistCDSaveAs,
                Field.Title,
                Field.ArtistCDType,
                Field.ArtistCDSex,
                Field.TrackTitle,
                Field.TrackSoundFile,
                Field.TrackNumber,
                Field.NumberOfTracks,
                Field.TotalLength,
                Field.Sampler,
                Field.TrackLength,
                Field.ArtistTrackName,
                Field.Medium,
                Field.AlbumType,
                Field.CDSet
            });

            SortFieldCollection sfc = new SortFieldCollection();
            sfc.Add(Field.ArtistCDSaveAs);
            sfc.Add(Field.Title);
            sfc.Add(Field.CDID);
            sfc.Add(Field.TrackNumber);
            TrackView = new TrackDataView(dataBase, CDQuery, Condition, sfc, fc);

            int count = 0;

            string lastArtist = "";
            string lastTitle = "";
            int lastcdid = 0;
            ArtistOverviewItem currentArtistItem = null;
            ArtistOverviewCDItem currentCDItem = null;
            List<ArtistOverviewItem> items = new List<ArtistOverviewItem>();

            for (int i = 0; i < TrackView.Rows.Count; i++)
            {
                string artistDisplay = (string)TrackView.GetRowStringValue(i, Field.ArtistCDName);
                string artist = (string)TrackView.GetRowStringValue(i, Field.ArtistCDSaveAs);
                string title = (string)TrackView.GetRowStringValue(i, Field.Title);
                int cdid = TrackView.GetCDID(i);

                if (artist != lastArtist || title != lastTitle || cdid != lastcdid)
                {
                    if (artist != lastArtist)
                    {
                        currentArtistItem = new ArtistOverviewItem(this.treeView);
                        PersonGroupType personGroupType = (PersonGroupType)TrackView.GetRowRawValue(i, Field.ArtistCDType);
                        SexType sexType = (SexType)TrackView.GetRowRawValue(i, Field.ArtistCDSex);
                        int imageIndex = GetImageIndexFromPersonGroup(personGroupType, sexType);
                        currentArtistItem.Artist = artistDisplay;
                        currentArtistItem.ImageIndex = imageIndex;
                        currentArtistItem.ID = TrackView.GetArtistCDID(i);
                        currentArtistItem.Items = new List<object>();

                        items.Add(currentArtistItem);

                        lastArtist = artist;
                    }

                    string titleText = GetTitleText(TrackView, i);

                    AlbumType albumType = (AlbumType)TrackView.GetRowRawValue(i, Field.AlbumType);
                    bool mp3cd = albumType == AlbumType.MusicDataCD || albumType == AlbumType.SoundFiles;
                    int imgIndex = GetImageByMedium(TrackView.GetRowStringValue(i, Field.Medium), mp3cd);

                    currentCDItem = new ArtistOverviewCDItem(this.treeView);
                    currentCDItem.Parent = currentArtistItem;
                    currentCDItem.ID = TrackView.GetCDID(i);
                    currentCDItem.Title = titleText;
                    object setName = TrackView.GetRowRawValue(i, Field.CDSet);
                    currentCDItem.ImageIndex = imgIndex;
                    currentCDItem.Tracks = new List<ArtistOverviewTrackItem>();

                    if (setName != null)
                    {
                        ArtistOverviewCDSetItem cdSetItem = null;

                        // Suchen, ob das CD-Set schon vorhanden ist
                        foreach (object o in currentArtistItem.Items)
                        {
                            if (o is ArtistOverviewCDSetItem)
                            {
                                ArtistOverviewCDSetItem item = o as ArtistOverviewCDSetItem;
                                if (item.Title == setName.ToString())
                                {
                                    cdSetItem = item;
                                    break;
                                }
                            }
                        }

                        if (cdSetItem == null)
                        {
                            cdSetItem = new ArtistOverviewCDSetItem(treeView);
                            cdSetItem.Parent = currentArtistItem;
                            currentArtistItem.Items.Add(cdSetItem);
                            cdSetItem.CDs = new List<ArtistOverviewCDItem>();
                        }

                        cdSetItem.CDs.Add(currentCDItem);
                        cdSetItem.Title = setName.ToString();
                    }
                    else
                    {
                        currentArtistItem.Items.Add(currentCDItem);
                    }

                    lastTitle = title;
                }

                ArtistOverviewTrackItem trackItem = new ArtistOverviewTrackItem();
                trackItem.ID = TrackView.GetTrackID(i);
                trackItem.Title = GetTrackTitleText(TrackView, i);
                trackItem.Soundfile = TrackView.GetRowStringValue(i, Field.TrackSoundFile);

                currentCDItem.Tracks.Add(trackItem);

                count++;

                lastcdid = cdid;
            }

            e.Result = items;

            cdTreeLoadedWithTracks = true;
        } 

        void bwCDTree_DoWork_CD(object sender, DoWorkEventArgs e)
        {
            CDDataView CDView;

            CDQueryDataSet CDQuery = dataBase.ExecuteCDQuery();

            // Hier die Felder auflisten, die ich für die Baumansicht brauche
            FieldCollection fc = new FieldCollection();
            fc.AddRange(new Field[] { 
                Field.ArtistCDName,
                Field.ArtistCDSaveAs,
                Field.Title,
                Field.ArtistCDType,
                Field.ArtistCDSex,
                Field.NumberOfTracks,
                Field.TotalLength,
                Field.Sampler,
                Field.Medium,
                Field.AlbumType,
                Field.CDSet,
                Field.DiscNumberInCDSet
            });

            SortFieldCollection sfc = new SortFieldCollection();
            sfc.Add(Field.ArtistCDSaveAs);
            sfc.Add(Field.Title);
            sfc.Add(Field.CDID);
            sfc.Add(Field.TrackNumber);
            CDView = new CDDataView(dataBase, CDQuery, Condition, sfc, fc);

            int count = 0;

            string lastArtist = "";
            string lastTitle = "";
            int lastcdid = 0;
            ArtistOverviewItem currentArtistItem = null;
            ArtistOverviewCDItem currentCDItem = null;
            List<ArtistOverviewItem> items = new List<ArtistOverviewItem>();
            bool firstArtist = true;

            for (int i = 0; i < CDView.Rows.Count; i++)
            {
                string artistDisplay = (string)CDView.GetRowStringValue(i, Field.ArtistCDName);
                string artist = (string)CDView.GetRowStringValue(i, Field.ArtistCDSaveAs);
                string title = (string)CDView.GetRowStringValue(i, Field.Title);
                int cdid = CDView.GetCDID(i);

                if (artist != lastArtist || title != lastTitle || cdid != lastcdid)
                {
                    if (artist != lastArtist || firstArtist)
                    {
                        currentArtistItem = new ArtistOverviewItem(this.treeView);
                        PersonGroupType personGroupType = (PersonGroupType)CDView.GetRowRawValue(i, Field.ArtistCDType);
                        SexType sexType = (SexType)CDView.GetRowRawValue(i, Field.ArtistCDSex);
                        int imageIndex = GetImageIndexFromPersonGroup(personGroupType, sexType);
                        currentArtistItem.Artist = artistDisplay;
                        currentArtistItem.ImageIndex = imageIndex;
                        currentArtistItem.ID = CDView.GetArtistID(i);
                        currentArtistItem.Items = new List<object>();

                        items.Add(currentArtistItem);

                        lastArtist = artist;

                        firstArtist = false;
                    }

                    string titleText = GetTitleText(CDView, i);

                    AlbumType albumType = (AlbumType)CDView.GetRowRawValue(i, Field.AlbumType);
                    bool mp3cd = albumType == AlbumType.MusicDataCD || albumType == AlbumType.SoundFiles;
                    int imgIndex = GetImageByMedium(CDView.GetRowStringValue(i, Field.Medium), mp3cd);
                    int cdSetNumer = (int)CDView.GetRowRawValue(i, Field.DiscNumberInCDSet);

                    currentCDItem = new ArtistOverviewCDItem(this.treeView);
                    currentCDItem.ID = CDView.GetCDID(i);
                    currentCDItem.Title = titleText;
                    object setName = CDView.GetRowRawValue(i, Field.CDSet);
                    currentCDItem.ImageIndex = imgIndex;
                    currentCDItem.CDSetNumber = cdSetNumer;
                    currentCDItem.Tracks = new List<ArtistOverviewTrackItem>();

                    if (setName != null)
                    {
                        ArtistOverviewCDSetItem cdSetItem = null;

                        // Suchen, ob das CD-Set schon vorhanden ist
                        foreach (object o in currentArtistItem.Items)
                        {
                            if (o is ArtistOverviewCDSetItem)
                            {
                                ArtistOverviewCDSetItem item = o as ArtistOverviewCDSetItem;
                                if (item.Title == setName.ToString())
                                {
                                    cdSetItem = item;
                                    break;
                                }
                            }
                        }

                        if (cdSetItem == null)
                        {
                            cdSetItem = new ArtistOverviewCDSetItem(treeView);
                            cdSetItem.Parent = currentArtistItem;
                            currentArtistItem.Items.Add(cdSetItem);
                            cdSetItem.CDs = new List<ArtistOverviewCDItem>();
                        }

                        // Position suchen, wo die neue CD eingefügt wird
                        int index = -1;
                        int j = 0;
                        foreach (ArtistOverviewCDItem cdItem in cdSetItem.CDs)
                        {
                            if (cdItem.CDSetNumber >= cdSetNumer)
                                index = j;

                            j++;
                        }

                        if (index >= 0)
                            cdSetItem.CDs.Insert(index, currentCDItem);
                        else
                            cdSetItem.CDs.Add(currentCDItem);
                        cdSetItem.Title = setName.ToString();
                    }
                    else
                    {
                        currentArtistItem.Items.Add(currentCDItem);
                    }

                    lastTitle = title;
                }

                currentCDItem.Tracks.Add(new ArtistOverviewTrackItem());

                count++;

                lastcdid = cdid;
            }

            e.Result = items;

            cdTreeLoadedWithTracks = false;
        }
         
        void bwCDTree_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeView.GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            List<ArtistOverviewItem> items = e.Result as List<ArtistOverviewItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            treeView.TreeView.ItemsSource = lcv;

            UpdateStatusBar();
        }

        private int GetImageByMedium(string medium, bool mp3cd)
        {
            if (mp3cd)
                return 11;

            switch (medium.ToUpper())
            {
                case "LP":
                    return 9;
                case "DVD":
                    return 10;
                case "MC":
                    return 12;
            }

            return 5;
        }

        private string GetTitleText(TrackDataView TrackView, int i)
        {
            string title = TrackView.GetRowStringValue(i, Field.Title);
            int numberOfTracks = (int)TrackView.GetRowRawValue(i, Field.NumberOfTracks);
            string trackText = numberOfTracks > 1 ? StringTable.Tracks : StringTable.Track;
            string length = TrackView.GetRowStringValue(i, Field.TotalLength);

            return string.Format("{0} [{1} {2}, {3}]", title, numberOfTracks, trackText, length);
        }

        private string GetTitleText(CDDataView CDView, int i)
        {
            string title = CDView.GetRowStringValue(i, Field.Title);
            int numberOfTracks = (int)CDView.GetRowRawValue(i, Field.NumberOfTracks);
            string trackText = numberOfTracks > 1 ? StringTable.Tracks : StringTable.Track;
            string length = CDView.GetRowStringValue(i, Field.TotalLength);

            return string.Format("{0} [{1} {2}, {3}]", title, numberOfTracks, trackText, length);
        }

        private void FillCDList()
        {
            dataGrid.GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;
            CreateHeader(CdListFields);
            dataGrid.ItemsSource = null;

            AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

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
            CDDataView CDView;

            if (CDQuery == null)
            {
                CDQuery = dataBase.ExecuteCDQuery();
            }

            CDView = new CDDataView(dataBase, CDQuery, Condition, CdListSort, CdListFields.GetFields());

            int count = 0;

            List<DataGridItem> items = new List<DataGridItem>();

            for (int row = 0; row < CDView.Rows.Count; row++)
            {
                List<DataGridItemValue> cells = new List<DataGridItemValue>();

                for (int col = 0; col < CdListFields.Count; col++)
                {
                    object value = CDView.GetRowRawValue(row, CdListFields[col].Field);

                    string stringValue = CDView.GetRowStringValue(row, CdListFields[col].Field);

                    cells.Add(new DataGridItemValue(value, stringValue, CdListFields[col].Field));
                }

                DataGridItem newItem = new DataGridItem();
                newItem.ID = CDView.GetCDID(row);
                newItem.Items = cells;

                items.Add(newItem);

                count++;
            }

            e.Result = items;
        }

        void bwCDList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGrid.GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            List<DataGridItem> items = e.Result as List<DataGridItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            dataGrid.ItemsSource = lcv;

            if (dataGrid.dataGrid.Items.Count > 0)
            {
                dataGrid.dataGrid.UpdateLayout();
                dataGrid.dataGrid.ScrollIntoView(dataGrid.dataGrid.Items[0]);
            }

            UpdateStatusBar();
        }

        /// <summary>
        /// Die Track-Liste laden
        /// </summary>
        private void FillTrackList()
        {
            dataGrid.GridLoadingCircle.Visibility = System.Windows.Visibility.Visible;
            CreateHeader(TrackListFields);
            dataGrid.ItemsSource = null;

            AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(this.dataGrid.Dispatcher);

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                backgroundWorker.DoWork += new DoWorkEventHandler(bw_DoWork);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TrackDataView TrackView;

            if (TrackQuery == null)
                TrackQuery = dataBase.ExecuteTrackQuery();

            FieldCollection fc = TrackListFields.GetFields();
            if (!fc.Contains(Field.TrackSoundFile))
                fc.Add(Field.TrackSoundFile);
            TrackView = new TrackDataView(dataBase, TrackQuery, Condition, TrackListSort, fc);

            int count = 0;

            List<DataGridItem> items = new List<DataGridItem>();

            for (int row = 0; row < TrackView.Rows.Count; row++)
            {
                List<DataGridItemValue> cells = new List<DataGridItemValue>();
                for (int col = 0; col < TrackListFields.Count; col++)
                {
                    object value = TrackView.GetRowRawValue(row, TrackListFields[col].Field);
                    string stringValue = TrackView.GetRowStringValue(row, TrackListFields[col].Field);

                    cells.Add(new DataGridItemValue(value, stringValue, TrackListFields[col].Field));
                }

                DataGridItem newItem = new DataGridItem();
                newItem.ID = TrackView.GetTrackID(row);
                newItem.Items = cells;

                string soundFilename = TrackView.GetRowStringValue(row, Field.TrackSoundFile);
                newItem.Soundfile = soundFilename;
                newItem.FirstColumnAddToPlaylist = true;

                items.Add(newItem);
                //toolStripStatusProgressBar.Value = (int)(100.0 / TrackView.Rows.Count * count);

                count++;
            }
            
            e.Result = items;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGrid.GridLoadingCircle.Visibility = System.Windows.Visibility.Collapsed;

            List<DataGridItem> items = e.Result as List<DataGridItem>;

            ListCollectionView lcv = new ListCollectionView(items);
            lcv.Filter = FilterRow;
            dataGrid.ItemsSource = lcv;

            if (dataGrid.dataGrid.Items.Count > 0)
            {
                dataGrid.dataGrid.UpdateLayout();
                dataGrid.dataGrid.ScrollIntoView(dataGrid.dataGrid.Items[0]);
            }

            UpdateStatusBar();
        }

        private void ShowCDList()
        {
            SaveDataGridColumnWidths();

            CurrentListType = ListType.CDList;

            UpdateToolbar();

            FillList();
        }

        private void toolStripButtonTrackList_Click(object sender, EventArgs e)
        {
            ShowTrackList();
        }

        private void ShowTrackList()
        {
            if (treeviewPreviewFormCD != null)
            {
                CloseTreeViewFormCD();
            }

            SaveDataGridColumnWidths();

            CurrentListType = ListType.TrackList;

            UpdateToolbar();

            FillList();
        }

        private void OpenCD()
        {
            CD CD = GetSelectedCD();

            if (CD == null)
                return;

            FormCD formCD = new FormCD(CD, dataBase);

            if (formCD.ShowDialog(this) == DialogResult.OK)
            {
                CD.Save(dataBase);
            }
        }

        private CD GetSelectedCD()
        {
            int[] cdids = GetSelectedCDIDs();

            if (cdids.Length > 0)
            {
                return dataBase.GetCDById(cdids[0]);
            }

            return null;
        }

        private Track GetSelectedTrack()
        {
            int[] trackids = GetSelectedTrackIDs();

            if (trackids.Length > 0)
            {
                return dataBase.GetTrackById(trackids[0]);
            }

            return null;
        }

        private int[] GetSelectedCDIDs()
        {
            List<int> cds = new List<int>();

            switch (CurrentListType)
            {
                case ListType.AlbumView:
                    {
                        if (albumView.SelectedAlbum != null)
                            cds.Add(albumView.SelectedAlbum.ID);
                        break;
                    }
                case ListType.CDList:
                    {
                        foreach (object o in dataGrid.SelectedItems)
                        {
                            DataGridItem item = o as DataGridItem;

                            if (item != null)
                                cds.Add(item.ID);
                        }

                        break;
                    }
                case ListType.CDTree:
                    {
                        if (treeView.TreeView.SelectedItem != null && treeView.TreeView.SelectedItem is ArtistOverviewCDItem)
                        {
                            ArtistOverviewCDItem selItem = (ArtistOverviewCDItem)treeView.TreeView.SelectedItem;
                            cds.Add(selItem.ID);
                        }

                        break;
                    }
            }
        
            return cds.ToArray();
        }

        private int[] GetSelectedArtistIDs()
        {
            List<int> artists = new List<int>();

            switch (CurrentListType)
            {
                case ListType.CDTree:
                    {
                        if (treeView.TreeView.SelectedItem != null && treeView.TreeView.SelectedItem is ArtistOverviewItem)
                        {
                            ArtistOverviewItem selItem = (ArtistOverviewItem)treeView.TreeView.SelectedItem;
                            artists.Add(selItem.ID);
                        }

                        break;
                    }
            }

            return artists.ToArray();
        }

        private int[] GetSelectedTrackIDs()
        {
            List<int> tracks = new List<int>();
            switch (CurrentListType)
            {
                case ListType.TrackList:
                    {
                        foreach (object o in dataGrid.SelectedItems)
                        {
                            DataGridItem item = o as DataGridItem;

                            if (item != null)
                                tracks.Add(item.ID);
                        }
                        break;
                    }
                case ListType.CDTree:
                    {
                        if (treeView.TreeView.SelectedItem != null && treeView.TreeView.SelectedItem is ArtistOverviewTrackItem)
                        {
                            ArtistOverviewTrackItem selItem = (ArtistOverviewTrackItem)treeView.TreeView.SelectedItem;
                            tracks.Add(selItem.ID);
                        }

                        break;
                    }
                case ListType.AlbumView:
                    {
                        if (albumView.SelectedTrack != null)
                            tracks.Add(albumView.SelectedTrack.ID);

                        break;
                    }
            }

            return tracks.ToArray();
        }

        private Track[] GetTracksFromSelectedCDs()
        {
            int[] cdids = GetSelectedCDIDs();
            List<Track> tracks = new List<Track>();

            foreach (int cdid in cdids)
            {
                CD cd = dataBase.GetCDById(cdid);

                tracks.AddRange(cd.Tracks);
            }

            return tracks.ToArray();
        }

        private Track[] GetSelectedTracks()
        {
            int[] trackIDs = GetSelectedTrackIDs();

            List<Track> selectedTracks = new List<Track>();
            foreach (int trackID in trackIDs)
                selectedTracks.Add(dataBase.GetTrackById(trackID));
            return selectedTracks.ToArray();
        }

        private Track[] GetAllTracks()
        {
            switch (CurrentListType)
            {
                case ListType.TrackList:
                    {
                        if (dataGrid.Items.Count < 1)
                            return null;

                        List<Track> selectedTracks = new List<Track>();
                        foreach (DataGridItem selectedRow in dataGrid.Items)
                            selectedTracks.Add(dataBase.GetTrackById(selectedRow.ID));
                        return selectedTracks.ToArray();
                    }
                default:
                    return null;
            }
        }

        private void tableView_DoubleClick(object sender, EventArgs e)
        {
            OpenCD();
        }

        private void Loaned()
        {
            CD cd = GetSelectedCD();
            if (cd == null)
                return;

            int cdid = cd.ID;

            LoanedCDDataSet.LoanedCDDataTable loanedCDs = new LoanedCDDataSet.LoanedCDDataTable();
            LoanedCDDataSet.LoanedCDRow loanedCDRow = loanedCDs.NewLoanedCDRow();
            loanedCDRow.CDID = cdid;

            FormLoanProperties formLoanProperties = new FormLoanProperties(dataBase, loanedCDRow);
            if (formLoanProperties.ShowDialog(this) == DialogResult.OK)
            {
                Big3.Hitbase.DataBaseEngine.LoanedCDDataSetTableAdapters.LoanedCDTableAdapter cdta = new Big3.Hitbase.DataBaseEngine.LoanedCDDataSetTableAdapters.LoanedCDTableAdapter(dataBase);
                loanedCDs.AddLoanedCDRow(loanedCDRow);
                cdta.Update(loanedCDs);
            }
        }

        private void DeleteCD()
        {
            int[] artistids = GetSelectedArtistIDs();

            if (artistids.Length > 0)
            {
                PersonGroupDataSet.PersonGroupRow artist = dataBase.GetPersonGroupById(artistids[0]);
                string msg = string.Format(StringTable.DeleteAllCDsOfArtist, artist.Name);
                
                // Alle CDs diese Interpreten löschen
                if (MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dataBase.DeleteAllCDsOfArtist(artistids[0]);
                    FillList();
                }

                return;
            }

            int[] cdids = GetSelectedCDIDs();

            if (cdids.Length < 1)
                return;

            // Wenn nur eine CD markiert ist, bei der Abfrage den Interpret und Titel angeben.
            if (cdids.Length == 1)
            {
                // Prüfen, ob die CD gerade im Laufwerk liegt. Dann kann sie nicht gelöscht werden.
                if (Big3.Hitbase.DataBaseEngine.GlobalUtilities.Current.IsCDInDrive(cdids[0]))
                {
                    MessageBox.Show(StringTable.CantDeleteWhileCDIsInDrive, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CD cd = dataBase.GetCDById(cdids[0]);

                string msg = string.Format(StringTable.DeleteCD, cd.Artist, cd.Title);
                if (MessageBox.Show(msg,
                    Application.ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dataBase.DeleteCd(cdids[0]);
                    FillList(true);
                }
            }
            else
            {
                // Mehr als eine CD
                string msg = string.Format(StringTable.DeleteMultipleCDs, cdids.Length);
                if (MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (int cdid in cdids)
                    {
                        dataBase.DeleteCd(cdid);
                    }

                    FillList(true);
                }
            }
        }

        private void CopyToClipboard()
        {
            bool tracks;
            List<int> recordIds = new List<int>();

            if (GetSelectedTrackIDs().Length > 0)
            {
                recordIds.AddRange(GetSelectedTrackIDs());
                tracks = true;
            }
            else
            {
                recordIds.AddRange(GetSelectedCDIDs());
                tracks = false;
            }

            if (recordIds.Count > 0)
            {
                FormCopyToClipboard formCopyToClipboard = new FormCopyToClipboard(dataBase, recordIds.ToArray(), tracks);
                formCopyToClipboard.ShowDialog(this);
            }
        }

        private void UpdateWindowState()
        {
            bool cdSelected = (GetSelectedCDIDs().Length > 0);
            bool trackSelected = (GetSelectedTrackIDs().Length > 0);

            ribbonButtonDeleteCD.Enabled = cdSelected;
            ribbonButtonAdjustSpelling.Enabled = cdSelected;
            ribbonButtonUploadCD.Enabled = cdSelected;
            ribbonButtonSearchCDCover.Enabled = cdSelected;
            ribbonButtonLoaned.Enabled = cdSelected;
            ribbonButtonCopyCD.Enabled = cdSelected;

            ribbonButtonPreListen.Enabled = trackSelected;
            ribbonButtonDeleteTrack.Enabled = trackSelected;
            ribbonButtonAddToWishlist.Enabled = trackSelected;
            ribbonButtonAddToPlaylist.Enabled = trackSelected || cdSelected;

            ribbonButtonSort.Enabled = (CurrentListType == ListType.CDList || CurrentListType == ListType.TrackList);
            ribbonButtonGroupBy.Enabled = (CurrentListType == ListType.CDList || CurrentListType == ListType.TrackList);

            ribbonButtonSearchLyrics.Enabled = trackSelected || cdSelected;
        }

        private void ShowCDTree()
        {
            SaveDataGridColumnWidths();

            CurrentListType = ListType.CDTree;

            UpdateToolbar();

            FillList();
        }

        private int GetImageIndexFromPersonGroup(PersonGroupType personGroupType, SexType sexType)
        {
            if (sexType == SexType.Feminin)
            {
                if (personGroupType == PersonGroupType.Single)
                    return 1;
                else
                    return 2;
            }

            if (sexType == SexType.Masculin)
            {
                if (personGroupType == PersonGroupType.Single)
                    return 3;
                else
                    return 4;
            }

            return 0;
        }

        private string GetTrackTitleText(TrackDataView trackView, int index)
        {
            bool sampler = (bool)trackView.GetRowRawValue(index, Field.Sampler);
            int length = (int)trackView.GetRowRawValue(index, Field.TrackLength);
            int trackNumber = (int)trackView.GetRowRawValue(index, Field.TrackNumber);
            string artist = (string)trackView.GetRowRawValue(index, Field.ArtistTrackName);
            string title = (string)trackView.GetRowRawValue(index, Field.TrackTitle);

            string text = "";
            
            if (sampler)
            {
                text = string.Format("{0}. {1} - {2}", trackNumber, artist, title);
            }
            else
            {
                text = string.Format("{0}. {1}", trackNumber, title);
            }

            if (length > 0)
                text += string.Format(" [{0}]", Misc.GetShortTimeString(length));

            return text;
        }

        private void ribbonButtonCDList_Click(object sender, EventArgs e)
        {
            ShowCDList();
        }

        private void ribbonButtonTrackList_Click(object sender, EventArgs e)
        {
            ShowTrackList();
        }

        private void ribbonButtonCDTree_Click(object sender, EventArgs e)
        {
            ShowCDTree();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            FormQuickSearch formQuickSearch = new FormQuickSearch();

            if (formQuickSearch.ShowDialog(this) == DialogResult.OK)
            {
                Condition = new Condition();
                if (!string.IsNullOrEmpty(formQuickSearch.textBoxArtist.Text))
                    Condition.Add(new SingleCondition(Field.ArtistCDName, Operator.Contains, formQuickSearch.textBoxArtist.Text));
                if (!string.IsNullOrEmpty(formQuickSearch.textBoxTitle.Text))
                    Condition.Add(new SingleCondition(Field.Title, Operator.Contains, formQuickSearch.textBoxTitle.Text));
                if (!string.IsNullOrEmpty(formQuickSearch.textBoxTrackTitle.Text))
                    Condition.Add(new SingleCondition(Field.TrackTitle, Operator.Contains, formQuickSearch.textBoxTrackTitle.Text));
                FillList();
            }
        }

        private void ribbonButtonExtendedFilter_Click(object sender, EventArgs e)
        {
            FormExtendedSearch formExtendedSearch = new FormExtendedSearch(dataBase, Condition, CurrentListType == ListType.TrackList ? true : false);

            if (formExtendedSearch.ShowDialog(this) == DialogResult.OK)
            {
                Condition = formExtendedSearch.Condition;
                FillList();
            }
        }

        private void ribbonButtonDeleteFilter_Click(object sender, EventArgs e)
        {
            Condition = null;
            FillList();
        }

        void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            timerSearch.Stop();
            timerSearch.Start();
        }

        void timerSearch_Tick(object sender, EventArgs e)
        {
            timerSearch.Stop();

            FilterString = searchTextBox.Text;
            treeView.FilterString = FilterString;

            switch (CurrentListType)
            {
                case ListType.CDTree:
                    {
                        if (!cdTreeLoadedWithTracks)
                        {
                            FillCDTree(true);
                        }
                        else
                        {
                            ((ListCollectionView)treeView.TreeView.ItemsSource).Refresh();
                        }
                        break;
                    }
                case ListType.CDList:
                case ListType.TrackList:
                    {
                        ((ListCollectionView)dataGrid.ItemsSource).Refresh();

                        if (dataGrid.dataGrid.Items.Count > 0)
                        {
                            dataGrid.dataGrid.UpdateLayout();
                            dataGrid.dataGrid.ScrollIntoView(dataGrid.dataGrid.Items[0]);
                        }
                        break;
                    }
                case ListType.AlbumView:
                    ((ListCollectionView)albumView.ItemsSource).Refresh();
                    break;
                default:
                    break;
            }

            UpdateStatusBar();
        }


        private bool FilterRow(object row)
        {
            if (string.IsNullOrEmpty(FilterString))
                return true;

            string filterString = FilterString.ToLower();

            bool found = false;

            switch (CurrentListType)
            {
                case ListType.None:
                    break;
                case ListType.CDTree:
                    {
                        found = treeView.FilterTreeRow(row as ArtistOverviewItem, filterString);

                        break;
                    }
                case ListType.TrackList:
                case ListType.CDList:
                    {
                        DataGridItem cdItem = row as DataGridItem;

                        foreach (object value in cdItem.Items)
                        {
                            if (value != null && value.ToString().IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                found = true;
                                break;
                            }
                        }

                        break;
                    }
                case ListType.AlbumView:
                    {
                        AlbumViewItem avi = row as AlbumViewItem;
                        if (avi.Artist != null && avi.Artist.ToLower().IndexOf(filterString) >= 0)
                        {
                            found = true;
                            break;
                        }
                        if (avi.Title != null && avi.Title.ToLower().IndexOf(filterString) >= 0)
                        {
                            found = true;
                            break;
                        }
                        if (avi.Genre != null && avi.Genre.ToLower().IndexOf(filterString) >= 0)
                        {
                            found = true;
                            break;
                        }
                        if (avi.Year != null && avi.Year.ToLower().IndexOf(filterString) >= 0)
                        {
                            found = true;
                            break;
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
                        break;
                    }
                default:
                    break;
            }

            return found;
        }

        private void ribbonButtonPrint_Click(object sender, EventArgs e)
        {
            FormPrintCatalog formPrintCatalog = new FormPrintCatalog(dataBase);

            formPrintCatalog.ShowDialog(this);
        }

        private void ribbonButtonFilter_Click(object sender, EventArgs e)
        {
            bool showCDTabOnly = (CurrentListType == ListType.CDList);
            FormSearch formSearch = new FormSearch(dataBase, Condition, false, showCDTabOnly);

            if (formSearch.ShowDialog(this) == DialogResult.OK)
            {
                Condition = formSearch.GetCondition();
                FillList();
            }
        }

        private void ribbonButtonSort_Click(object sender, EventArgs e)
        {
            FormSort formSort = new FormSort(dataBase, 
                CurrentListType == ListType.CDList ? FieldType.CD : FieldType.All, 
                CurrentListType == ListType.CDList ? CdListSort : TrackListSort);

            if (formSort.ShowDialog(this) == DialogResult.OK)
            {
                if (CurrentListType == ListType.CDList)
                    CdListSort = formSort.SortFields;
                else
                    TrackListSort = formSort.SortFields;
                RefreshList();
            }
        }

        private void ribbonButtonCDCover_Click(object sender, EventArgs e)
        {
            if (ribbonButtonCDCover.Checked)
            {
                formShowPicture.Dispose();
                ribbonButtonCDCover.Checked = false;
                return;
            }
            if (formShowPicture.IsDisposed)
                formShowPicture = new ShowPictureForm();
            formShowPicture.Show(this);
            formShowPicture.Disposed += new EventHandler(formShowPicture_Disposed);
            ribbonButtonCDCover.Checked = true;

            ShowCDCover();
        }

        void formShowPicture_Disposed(object sender, EventArgs e)
        {
            ribbonButtonCDCover.Checked = false;
        }

        private void ShowCDCover()
        {
            int[] CDIDs = GetSelectedCDIDs();
            if (CDIDs != null && CDIDs.Length > 0 && formShowPicture.IsHandleCreated && !formShowPicture.IsDisposed)
            {
                try
                {
                    CD cd = dataBase.GetCDById(CDIDs[0]);
                    if (string.IsNullOrEmpty(cd.CDCoverFrontFilename))
                        formShowPicture.Image = null;
                    else
                    {
                        // Specify a valid picture file path on your computer.
                        FileStream fs;
                        fs = new FileStream(Misc.FindCover(cd.CDCoverFrontFilename), FileMode.Open, FileAccess.Read);
                        formShowPicture.Image = System.Drawing.Image.FromStream(fs);
                        fs.Close();
                    }
                }
                catch
                {
                    formShowPicture.Image = null;
                }
            }
        }

        private void ShowLyrics()
        {
            Track track = GetSelectedTrack();
            if (track != null && formLyrics.IsHandleCreated && !formLyrics.IsDisposed)
            {
                try
                {
                    formLyrics.Lyrics = track.Lyrics;
                }
                catch
                {
                    formLyrics.Lyrics = null;
                }
            }
        }

        private void ribbonButtonLyrics_Click(object sender, EventArgs e)
        {
            if (ribbonButtonLyrics.Checked)
            {
                formLyrics.Dispose();
                ribbonButtonLyrics.Checked = false;
                return;
            }
            if (formLyrics.IsDisposed)
                formLyrics = new FormLyrics();
            formLyrics.Show(this);
            formLyrics.Disposed += new EventHandler(formLyrics_Disposed);
            ribbonButtonLyrics.Checked = true;

            ShowLyrics();
        }

        void formLyrics_Disposed(object sender, EventArgs e)
        {
            ribbonButtonLyrics.Checked = false;
        }

        private void ribbonButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            SaveDataGridColumnWidths();

            FillList(true);
        }

        private void ribbonButtonProperties_Click(object sender, EventArgs e)
        {
            if (GetSelectedCDIDs().Length > 0)
                OpenCD();

            if (GetSelectedTrackIDs().Length > 0)
                OpenTrackProperties();

            if (GetSelectedArtistIDs().Length > 0)
                OpenArtist();
        }

        private void OpenTrackProperties()
        {
            int[] selTracksIDs = GetSelectedTrackIDs();
            if (selTracksIDs.Length < 1)
                return;

            Track track = dataBase.GetTrackById(selTracksIDs[0]);
            CD CD = dataBase.GetCDById(track.CDID);

            if (CD == null)
                return;

            FormCD formCD = new FormCD(CD, dataBase);
            formCD.ShowTrack = true;

            int trackIndex = 0;
            for (int i = 0; i < CD.Tracks.Count; i++)
            {
                if (CD.Tracks[i].TrackNumber == track.TrackNumber)
                {
                    trackIndex = i;
                    break;
                }
            }

            formCD.CurrentTrack = trackIndex;

            if (formCD.ShowDialog(this) == DialogResult.OK)
            {
                CD.Save(dataBase);
            }
        }

        private void OpenArtist()
        {
            int artistId = GetSelectedArtistIDs()[0];
            Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupTableAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(dataBase);
            PersonGroupDataSet.PersonGroupRow personGroupRow = personGroupTableAdapter.GetPersonGroupById(artistId);
   
            FormArtistProperties fap = new FormArtistProperties(dataBase, PersonType.Artist, personGroupRow);
            if (fap.ShowDialog(this) == DialogResult.OK)
            {
                personGroupTableAdapter.Update(personGroupRow);
                FillList();
            }
        }

        private void ribbonButtonDeleteCD_Click(object sender, EventArgs e)
        {
            DeleteCD();
        }

        private void ribbonButtonCopyCDToClipboard_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void ribbonButtonAddToWishlist_Click(object sender, EventArgs e)
        {
            Track[] tracks = GetSelectedTracks();

            if (tracks != null)
            {
                if (AddTrackToWishlist != null)
                    AddTrackToWishlist(this, tracks[0]);
            }
        }

        private void ribbonButtonLoaned_Click(object sender, EventArgs e)
        {
            Loaned();
        }

        private void ribbonButtonDeleteTrack_Click(object sender, EventArgs e)
        {
            Track[] tracks = GetSelectedTracks();

            foreach (Track track in tracks)
                DeleteSoundfile(track);
        }

        private void DeleteSoundfile(Track track)
        {
            if (MessageBox.Show(string.Format(StringTable.DeleteSoundfile, track.Soundfile), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                File.Delete(track.Soundfile);

                TrackTableAdapter ta = new TrackTableAdapter(dataBase);
                CDQueryDataSet.TrackDataTable tracks = ta.GetDataById(track.ID);
                if (tracks.Count > 0)
                {
                    tracks[0].SoundFile = "";
                    ta.Update(tracks);
                    FillList(true);
                }
            }
        }

        private void ribbonButtonPlay_Click(object sender, EventArgs e)
        {

        }

        private void FormCatalog_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveDataGridColumnWidths();

            Settings.SaveWindowPlacement(this, "FormCatalog");
            Settings.Current.LastCatalogListType = (int)CurrentListType;

            GC.Collect();
        }

        private void SaveDataGridColumnWidths()
        {
            // Spaltenbreiten auslesen
            int i = 0;
            SortedList<int, ColumnField> displayColumns = new SortedList<int, ColumnField>();
            SortFieldCollection sfc = new SortFieldCollection();

            foreach (DataGridColumn col in dataGrid.dataGrid.Columns)
            {
                ColumnField cf = new ColumnField();

                if (CurrentListType == ListType.CDList)
                {
                    cf.Width = (int)col.Width.DisplayValue;
                    cf.Field = CdListFields[i].Field;
                    displayColumns.Add(col.DisplayIndex, cf);
                }
                if (CurrentListType == ListType.TrackList && i > 0)
                {
                    cf.Width = (int)col.Width.DisplayValue;
                    cf.Field = TrackListFields[i-1].Field;
                    displayColumns.Add(col.DisplayIndex, cf);
                }

                i++;
            }

            ColumnFieldCollection cfc = new ColumnFieldCollection();
            foreach (ColumnField cf in displayColumns.Values)
            {
                cfc.Add(cf);
            }

            if (CurrentListType == ListType.CDList)
            {
                CdListFields = cfc;
                cfc.SaveToRegistry("Catalog");
                CdListSort.SaveToRegistry("CatalogSort");
            }
            if (CurrentListType == ListType.TrackList)
            {
                TrackListFields = cfc;
                cfc.SaveToRegistry("CatalogTrack");
                TrackListSort.SaveToRegistry("CatalogTrackSort");
            }
        }

        private void ribbonButtonUploadCD_Click(object sender, EventArgs e)
        {
            CD cd = GetSelectedCD();

            if (cd != null && UploadCD != null)
            {
                UploadCD(this, cd);
            }
        }

        private void ribbonButtonSearchCDCover_Click(object sender, EventArgs e)
        {
            CD cd = GetSelectedCD();

            if (cd == null)
                return;

            CDCoverAmazon.GetCDCover(cd);

            cd.Save(dataBase);
        }

        private void ribbonButtonAdjustSpelling_Click(object sender, EventArgs e)
        {
            AdjustSpelling();
        }

        private void AdjustSpelling()
        {
            CD cd = GetSelectedCD();

            if (cd == null)
                return;

            MessageBoxEx msgBox = new MessageBoxEx(StringTable.AdjustSpelling, MessageBoxButtons.YesNo, MessageBoxIcon.Question, "AdjustSpellingDontAskAgain");
            msgBox.Size = new Size(450, 170);

            if (msgBox.ShowDialog(this) == DialogResult.Yes)
            {
                cd.AdjustSpelling(Settings.Current.AdjustSpelling);

                cd.Save(dataBase);
            }
        }

        private void ribbonButtonPrintCatalog_Click(object sender, EventArgs e)
        {
            CD cd = GetSelectedCD();
            
            FormPrintCatalog formPrintCatalog;
            
            if (cd != null)
                formPrintCatalog = new FormPrintCatalog(dataBase, cd);
            else
                formPrintCatalog = new FormPrintCatalog(dataBase);

            formPrintCatalog.ShowDialog(this);
        }

        private void ribbonButtonExport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = StringTable.FilterHitbase;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                dataBase.Export(this, openFileDialog.FileName);
            }
        }

        private void ribbonButtonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = StringTable.FilterHitbase;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                dataBase.Import(this, openFileDialog.FileName);
                RefreshList();
            }
        }

        private void ribbonButtonPrintCDCover_Click(object sender, EventArgs e)
        {
            CD cd = GetSelectedCD();

            if (cd == null)
                return;

            if (PrintCDCover != null)
                PrintCDCover(this, cd);
        }

        private void ribbonButtonAddToPlaylistNow_Click(object sender, EventArgs e)
        {
            AddToPlaylist(AddToPlaylistType.Now);
        }

        private void AddToPlaylist(AddToPlaylistType addToPlaylistType)
        {
            Track[] tracks;

            tracks = GetTracksFromSelectedCDs();

            if (tracks == null || tracks.Length == 0)
            {
                tracks = GetSelectedTracks();
            }

            if (tracks != null && AddTracksToPlaylist != null)
            {
                List<string> filenames = new List<string>();
                foreach (Track track in tracks)
                    filenames.Add(track.Soundfile);

                AddTracksToPlaylist(filenames.ToArray(), addToPlaylistType);
            }
        }

        private void ribbonButtonAddToPlaylistNext_Click(object sender, EventArgs e)
        {
            AddToPlaylist(AddToPlaylistType.Next);
        }

        private void ribbonButtonAddToPlaylistEnd_Click(object sender, EventArgs e)
        {
            AddToPlaylist(AddToPlaylistType.End);
        }

        private void ribbonButtonAddToPlaylistAll_Click(object sender, EventArgs e)
        {
            if (AddTracksToPlaylist != null)
            {
                Track[] tracks = GetAllTracks();

                if (tracks == null)
                    return;

                List<string> filenames = new List<string>();
                foreach (Track track in tracks)
                {
                    if (!string.IsNullOrEmpty(track.Soundfile) && File.Exists(track.Soundfile))
                        filenames.Add(track.Soundfile);
                }

                AddTracksToPlaylist(filenames.ToArray(), AddToPlaylistType.Next);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool trackSelected = GetSelectedTrackIDs().Length > 0;
            bool cdSelected = GetSelectedCDIDs().Length > 0;
            bool artistSelected = GetSelectedArtistIDs().Length > 0;

            printCDCoverToolStripMenuItem.Enabled = cdSelected;
            playNowToolStripMenuItem.Enabled = trackSelected || cdSelected;
            playNextToolStripMenuItem.Enabled = trackSelected || cdSelected;
            playLastToolStripMenuItem.Enabled = trackSelected || cdSelected;
            preListenToolStripMenuItem.Enabled = trackSelected;
            addToWishlistToolStripMenuItem.Enabled = trackSelected;
            deleteContextToolStripMenuItem.Enabled = cdSelected || artistSelected;
            verliehenToolStripMenuItem.Enabled = cdSelected;
            sendToCDArchiveToolStripMenuItem.Enabled = cdSelected;
            copyToClipboardToolStripMenuItem.Enabled = cdSelected || trackSelected;
            adjustSpellingToolStripMenuItem.Enabled = cdSelected;
            printToolStripMenuItem.Enabled = cdSelected;
            chooseColumnsToolStripMenuItem2.Enabled = (CurrentListType == ListType.CDList || CurrentListType == ListType.TrackList);
        }

        private void chooseColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormChooseColumnFields formChooseColumnFields = null;
            
            if (CurrentListType == ListType.CDList)
                formChooseColumnFields = new FormChooseColumnFields(dataBase, FieldType.CD, this.cdListFields);
            else
                formChooseColumnFields = new FormChooseColumnFields(dataBase, FieldType.All, this.trackListFields);

            if (formChooseColumnFields.ShowDialog(this) == DialogResult.OK)
            {
                if (CurrentListType == ListType.CDList)
                    CdListFields = formChooseColumnFields.SelectedFields;
                else
                    trackListFields = formChooseColumnFields.SelectedFields;

                FillList();
            }
        }

        private void ribbonButtonCDOverview_Click(object sender, EventArgs e)
        {
            if (treeviewPreviewFormCD != null)
            {
                CloseTreeViewFormCD();
            }

            CurrentListType = ListType.AlbumView;
            FillList();
        }

        private void CloseTreeViewFormCD()
        {
            //this.Controls.Remove(panelTreeViewCD);
            treeviewPreviewFormCD.Close();
            treeviewPreviewFormCD = null;
            //elementHost.Dock = DockStyle.Fill;
        }

        private void preListenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Track track = GetSelectedTrack();

            if (track != null)
                MiniPlayerWindow.PreListen(track);
        }

        private void ribbonButtonPreListen_Click(object sender, EventArgs e)
        {
            Track track = GetSelectedTrack();

            if (track != null)
                MiniPlayerWindow.PreListen(track);
        }

        private void ribbonButtonSearchLyrics_Click(object sender, EventArgs e)
        {
            CD cd = GetSelectedCD();

            if (cd != null)
            {
                LyricsSearch.SearchLyrics(cd);

                cd.Save(dataBase);
            }
            else
            {
                Track track = GetSelectedTrack();

                CD cd1 = dataBase.GetCDById(track.CDID);

                var trackFromCD = from t in cd1.Tracks 
                                  where t.ID == track.ID
                                  select t;

                LyricsSearch.SearchLyrics(cd1, trackFromCD.First());
                
                cd1.Save(dataBase);
            }
           
        }

        private void ribbonButtonCopyCD_Click(object sender, EventArgs e)
        {
            CD cd = GetSelectedCD();

            if (cd == null)
                return;

            cd.ID = 0;          // Damit wird die CD kopiert
            cd.Identity = "";   // Identity löschen

            FormCD formCD = new FormCD(cd, dataBase);
            if (formCD.ShowDialog(this) == DialogResult.OK)
            {
                cd.Save(dataBase);
                FillList(true);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPrintCatalog formPrintCatalog = new FormPrintCatalog(dataBase, GetSelectedCD());

            formPrintCatalog.ShowDialog(this);
        }
    }

    internal class DataGridItemValue
    {
        internal DataGridItemValue(object value, string displayValue, Field field)
        {
            Value = value;
            DisplayValue = displayValue;
            Field = field;
        }

        public object Value { get; set; }
        public string DisplayValue { get; set; }
        public Field Field { get; set; }

        public override string ToString()
        {
            return DisplayValue;
        }
    }

    internal class DataGridItem
    {
        public int ID { get; set; }
        public bool FirstColumnAddToPlaylist { get; set; }
        public List<DataGridItemValue> Items { get; set; }
        public System.Windows.Media.Brush TextColor 
        {
            get
            {
                if (!string.IsNullOrEmpty(Soundfile))
                {
                    if (File.Exists(Soundfile))
                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 192, 0));
                    else
                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(192, 0, 0));
                }
                else
                {
                    return System.Windows.Media.Brushes.Black;
                }
            }
        }
        public string Soundfile { get; set; }
    }

    [ValueConversion(typeof(DataGridItemValue), typeof(int))]
    public class DataGridItemValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridItemValue val = value as DataGridItemValue;

            if (val == null)
                return "";
            else
                return val.DisplayValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }


    public class ArtistOverviewItem : INotifyPropertyChanged
    {
        CatalogTreeView treeView;

        public ArtistOverviewItem(CatalogTreeView treeView)
        {
            this.treeView = treeView;
        }

        public int ImageIndex { get; set; }

        public BitmapImage Image
        {
            get
            {
                return GetImageByIndex(ImageIndex);
            }
        }

        public int ID { get; set; }
        public string Artist { get; set; }

        private bool isSelected = false;
        public bool IsSelected 
        { 
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }

        public string TextSearch 
        { 
            get 
            { 
                return Artist; 
            }
        }

        /// <summary>
        /// CDs oder CD-Sets
        /// </summary>
        public List<object> Items { get; set; }

        public System.Windows.HierarchicalDataTemplate Template
        {
            get
            {
                return treeView.FindResource("CDTemplate") as System.Windows.HierarchicalDataTemplate;
            }
        }

        public static BitmapImage GetImageByIndex(int index)
        {
            switch (index)
            {
                case 0: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/PersonGroupGeneral.png")); 
                case 1: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/PersonGroupFeminin.png")); 
                case 2: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/PersonGroupFemininGroup.png"));
                case 3: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/PersonGroupMasculin.png")); 
                case 4: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/PersonGroupMasculinGroup.png"));
                case 5: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/CDIcon.png")); 
                case 6: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/TrackIcon.png"));
                case 7: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/TrackSoundFoundIcon.png"));
                case 8: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/TrackSoundNotFoundIcon.png"));
                case 9: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/LPIcon.png"));
                case 10: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/DVDIcon.png"));
                case 11: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/MP3CDIcon.png"));
                case 12: return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/MCIcon.png"));
            }

            return null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class ArtistOverviewCDSetItem
    {
        private CatalogTreeView catalogTreeView;

        public ArtistOverviewCDSetItem(CatalogTreeView catalogTreeView)
        {
            this.catalogTreeView = catalogTreeView;
        }

        public int ImageIndex { get; set; }

        public BitmapImage Image
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/CDSetIcon.png")); 
            }
        }

        public System.Windows.HierarchicalDataTemplate Template
        {
            get
            {
                return catalogTreeView.FindResource("CDSetTemplate") as System.Windows.HierarchicalDataTemplate;
            }
        }

        public ArtistOverviewItem Parent { get; set; }

        public int ID { get; set; }
        public string Title { get; set; }
        public List<ArtistOverviewCDItem> CDs { get; set; }

        public string TextSearch { get { return Title; } }

    }

    public class ArtistOverviewCDItem
    {
        private CatalogTreeView catalogTreeView;

        public ArtistOverviewCDItem(CatalogTreeView catalogTreeView)
        {
            this.catalogTreeView = catalogTreeView;
        }

        public int ImageIndex { get; set;}

        public BitmapImage Image
        {
            get
            {
                return ArtistOverviewItem.GetImageByIndex(ImageIndex);
            }
        }

        public ArtistOverviewItem Parent { get; set; }

        public System.Windows.HierarchicalDataTemplate Template
        {
            get
            {
                return catalogTreeView.FindResource("CDTemplate") as System.Windows.HierarchicalDataTemplate;
            }
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public int CDSetNumber { get; set; }
        public List<ArtistOverviewTrackItem> Tracks { get; set; }

        public string TextSearch { get { return Title; } }
    }

    public class ArtistOverviewTrackItem
    {
//        public int ImageIndex { get; set; }

        public BitmapImage Image
        {
            get
            {
                int trackImageIndex = 6;

                if (string.IsNullOrEmpty(Soundfile))
                {
                    trackImageIndex = 6;
                }
                else
                {
                    if (File.Exists(Soundfile))
                        trackImageIndex = 7;
                    else
                        trackImageIndex = 8;
                }

                return ArtistOverviewItem.GetImageByIndex(trackImageIndex);
            }
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string Soundfile { get; set; }

        public string TextSearch { get { return Title; } }
    }

    public class AlbumViewItem
    {
        public int ID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Year { get; set; }
        public string ArchiveNumber { get; set; }

        private string imageFilename;
        public string ImageFilename 
        {
            get
            {
//                if (string.IsNullOrEmpty(imageFilename) || !File.Exists(imageFilename))
//                    return "pack://application:,,,/CatalogView;component/Images/CDCover.png";
//                else
                    return imageFilename;
            }
            set
            {
                imageFilename = value;
            }
        }

        public BitmapImage Image
        {
            get
            {
                if (string.IsNullOrEmpty(ImageFilename))
                {
                    // Innerhalb der Tracks nach einem Image suchen
                    for (int i = 0; i < Tracks.Count; i++)
                    {
                        SoundFileInformation sfi = SoundEngine.GetSoundFileInformation(Tracks[i].Soundfile);
                        if (sfi.Images != null && sfi.Images.Count > 0)
                        {
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            MemoryStream ms = new MemoryStream(sfi.Images[0]);
                            image.StreamSource = ms;
                            image.EndInit();
                            return image;
                        }
                    }

                    return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/CDCover.png"));
                }
                else
                {
                    if (ImageFilename != null && File.Exists(Misc.FindCover(ImageFilename)))
                        return new BitmapImage(new Uri(Misc.FindCover(ImageFilename)));
                    else
                        return new BitmapImage(new Uri("pack://application:,,,/CatalogView;component/Images/CDCover.png")); 
                }
            }
        }

        public List<Track> Tracks { get; set; }
    }

    public class CDItemDataTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate
            SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is ArtistOverviewCDItem)
            {
                ArtistOverviewCDItem artistCDItem = item as ArtistOverviewCDItem;

                System.Windows.HierarchicalDataTemplate hdt = artistCDItem.Template;// new System.Windows.HierarchicalDataTemplate();

                return hdt;
            }

            if (item is ArtistOverviewCDSetItem)
            {
                ArtistOverviewCDSetItem artistCDSetItem = item as ArtistOverviewCDSetItem;

                System.Windows.HierarchicalDataTemplate hdt = artistCDSetItem.Template;// new System.Windows.HierarchicalDataTemplate();

                return hdt;
            }

            /*if (item != null && item is AuctionItem)
            {
                AuctionItem auctionItem = item as AuctionItem;
                Window window = Application.Current.MainWindow;

                switch (auctionItem.SpecialFeatures)
                {
                    case SpecialFeatures.None:
                        return 
                            window.FindResource("AuctionItem_None") 
                            as DataTemplate;
                    case SpecialFeatures.Color:
                        return 
                            window.FindResource("AuctionItem_Color") 
                            as DataTemplate;
                }
            }*/

            return null;
        }
    }

    [ValueConversion(typeof(DataGridItemValue), typeof(int))]
    public class Int32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridItemValue val = value as DataGridItemValue;

            if (val == null)
                return 0;
            else
                return Misc.Atoi(val.Value.ToString());
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }
}
