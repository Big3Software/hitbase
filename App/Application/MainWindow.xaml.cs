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
using Big3.Hitbase.SoundEngine;
using System.Windows.Threading;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.MainControls;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SoundFilesManagement;
using Big3.Hitbase.SharedResources;
using Microsoft.Win32;
using System.IO;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.CatalogView3D;
using Big3.Hitbase.Controls;
using System.Runtime.InteropServices;
using Big3.Hitbase.SoundEngineGUI;
using System.Diagnostics;
using Big3.Hitbase.SharedResources.Themes;
using System.Windows.Shell;
using Big3.Hitbase.RecordMedium;
using System.Windows.Interop;
using System.Net;
using System.Reflection;
using Big3.Hitbase.CDCover;
using System.Windows.Controls.Ribbon;

namespace Big3.Hitbase.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, INotifyPropertyChanged, IModalService
    {
        public DataBase DataBase { get; set; }

        public Playlist CurrentPlaylist { get; private set; }

        public Wishlist Wishlist { get; private set; }

        private double navigationBarWidth = 0;
        private double playlistWidth = 0;

        StatisticsUserControl statisticsUserControl;

        DispatcherTimer dtSynchronizeCatalog = new DispatcherTimer();

        DispatcherTimer dtWishList = new DispatcherTimer();

        DispatcherTimer dtCloseSplashScreen = new DispatcherTimer();

        ThumbButtonInfo tbiPlay = new ThumbButtonInfo();

        [DllImport("user32.dll")]
        public static extern int RegisterWindowMessage(String strMessage);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        int queryMessage;

        public MainWindow()
        {
            CurrentPlaylist = new Playlist();
            Wishlist = new Wishlist();

            InitializeComponent();

            LocalizeRibbon();

            System.Windows.Forms.Application.EnableVisualStyles();

            OpenDataBase();

            if (DataBase == null)       // Starten abgebrochen
                return;

            dtSynchronizeCatalog.Interval = TimeSpan.FromSeconds(1);
            dtSynchronizeCatalog.Tick += new EventHandler(dtSynchronizeCatalog_Tick);
            dtSynchronizeCatalog.Start();

            dtWishList.Interval = TimeSpan.FromSeconds(1);
            dtWishList.Tick += new EventHandler(dtWishList_Tick);
            dtWishList.Start();

/*            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(10).Duration();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();*/

            FillMainTree();

            CurrentPlaylist.CurrentTrackChanged += new EventHandler(CurrentPlaylist_CurrentTrackChanged);
            CurrentPlaylist.PlayStatusChanged += new EventHandler(CurrentPlaylist_PlayStatusChanged);

            playlistControl.Playlist = CurrentPlaylist;
            PlayerControl.Playlist = CurrentPlaylist;

            WishlistUserControl.Wishlist = Wishlist;

            mainTreeUserControl.ItemClicked += new ItemClickedHandler(mainTreeUserControl_ItemClicked);
            
            UpdateWindowState();

            UpdateView();
            
            // Set last values from last record
            InitRecordSettings();

            RestorePinnedTabs();

            if (tabControl.Items.Count == 0 && !Settings.Current.DontShowFirstSteps)
            {
                AddFirstStepsTab();
            }

            this.AddHandler(CDUserControl.CDTitleChangedEvent, new RoutedEventHandler(CDTitleChanged));

            InitWindows7TaskBar();

            queryMessage = RegisterWindowMessage("QueryCancelAutoPlay");

            if (App.IsBetaVersion)
            {
                buttonActivateHitbase.Visibility = System.Windows.Visibility.Collapsed;
                buttonBuyHitbase.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (Big3.Hitbase.Miscellaneous.Register.GetSerialActivation())
                {
                    buttonActivateHitbase.Visibility = System.Windows.Visibility.Collapsed;
                    buttonBuyHitbase.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            FillRecentFileList();

            DoAutoChecks();

            OpenCommandLineFile();
        }

        private void OpenCommandLineFile()
        {
            if (!string.IsNullOrEmpty(App.FileToOpen))
            {
                // So was hamme hier denn alles so...?

                string extension = System.IO.Path.GetExtension(App.FileToOpen).ToLower();
                switch (extension)
                {
                    // 1. Musikdateien
                    case ".mp3":
                    case ".wav":
                    case ".ogg":
                    case ".flac":
                        AddTracksToPlaylistParameter addTracksParams = new AddTracksToPlaylistParameter();
                        addTracksParams.AddTracksType = AddTracksToPlaylistType.Now;
                        addTracksParams.Filenames.Add(App.FileToOpen);
                        HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, this);
                        break;

                    // 2. Playlisten
                    case ".m3u":
                    case ".hvc":
                        CurrentPlaylist.LoadFromFile(App.FileToOpen, false);
                        CurrentPlaylist.Play();
                        break;
                    default:
                        {
                            if (App.FileToOpen.Length == 2 && App.FileToOpen[1] == ':' || App.FileToOpen.ToLower().EndsWith(".cda"))
                            {
                                char driveLetter = App.FileToOpen[0];
                                                                
                                MainCDTreeItem item = mainTreeUserControl.GetCDTreeItemDriveLetter(driveLetter);

                                int trackNumber = 1;

                                if (App.FileToOpen.ToLower().EndsWith(".cda"))
                                {
                                    trackNumber = Misc.Atoi(App.FileToOpen.Mid(8, 2));          // z.b. "k:\track01.cda"
                                }

                                OpenTreeItem(item, true, trackNumber);
                            }
                            break;
                        }
                }
            }
        }

        private void UpdateView()
        {
            if (!Settings.Current.ShowNavigationBar)
                HideNavigationBar(true);
            if (!Settings.Current.ShowPlaylist)
                HidePlaylist(true);
            if (!Settings.Current.ShowPlayer)
                HidePlayer();

            UpdateWindowState();
            PlayerControl.UpdateWindowState();
        }

        private void FillRecentFileList()
        {
            RecentFileListPanel.Children.Clear();

            foreach (string filename in Settings.Current.RecentFileList)
            {
                MenuItem menuItem = new MenuItem();
/*                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Children.Add(new TextBlock() { Text = System.IO.Path.GetFileName(filename) });
                stackPanel.Children.Add(new TextBlock() { Text = System.IO.Path.GetDirectoryName(filename) });
                */
                menuItem.Tag = filename;
                menuItem.Click += new RoutedEventHandler(MenuItemRecentList_Click);
                menuItem.Header = System.IO.Path.GetFileName(filename);
                menuItem.ToolTip = filename;
                menuItem.Icon = ImageLoader.FromResource("Document32.png");
                RecentFileListPanel.Children.Add(menuItem);
            }
        }

        protected override void OnSourceInitialized(EventArgs e) 
        { 
            base.OnSourceInitialized(e); 
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource; 
            source.AddHook(WndProc);

            try
            {
                RibbonWindowExtensions.LoadState(this, this.MyRibbon);
            }
            catch (Exception ex)
            {
                UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(ex);
                unhandledExceptionWindow.ShowDialog();
            }
        }

        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_SCREENSAVE = 0xF140;
        private const int SC_MONITORPOWER = 0xF170;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)        
        {
            if (msg == queryMessage)
            {
                SetWindowLong((new WindowInteropHelper(this)).Handle, 0, 1);
                handled = true;
                return new IntPtr(1);
            }

            if (CurrentPlaylist.IsPlaying && Settings.Current.DisableScreenSaver)
            {
                if (msg == WM_SYSCOMMAND &&
                    ((((long)wParam & 0xFFF0) == SC_SCREENSAVE) ||
                    ((long)wParam & 0xFFF0) == SC_MONITORPOWER))
                {
                    handled = true;
                }
            }

            // Handle messages...            
            return IntPtr.Zero;        
        }

        private void InitWindows7TaskBar()
        {
            this.TaskbarItemInfo = new TaskbarItemInfo();

            ThumbButtonInfo tbiPrev = new ThumbButtonInfo();
            tbiPrev.ImageSource = ImageLoader.FromResource("TaskBarPrev.png");
            tbiPrev.Command = HitbaseCommands.PrevTrack;
            tbiPrev.CommandTarget = this;
            tbiPrev.Description = StringTable.PrevTrack;
            this.TaskbarItemInfo.ThumbButtonInfos.Add(tbiPrev);

            tbiPlay.ImageSource = ImageLoader.FromResource("TaskBarPlay.png");
            tbiPlay.Command = HitbaseCommands.Play;
            tbiPlay.CommandTarget = this;
            tbiPlay.Description = StringTable.Play;
            this.TaskbarItemInfo.ThumbButtonInfos.Add(tbiPlay);

            ThumbButtonInfo tbiNext = new ThumbButtonInfo();
            tbiNext.ImageSource = ImageLoader.FromResource("TaskBarNext.png");
            tbiNext.Command = HitbaseCommands.NextTrack;
            tbiNext.CommandTarget = this;
            tbiNext.Description = StringTable.NextTrack;
            this.TaskbarItemInfo.ThumbButtonInfos.Add(tbiNext);
        }

        void CurrentPlaylist_CurrentTrackChanged(object sender, EventArgs e)
        {
            UpdateWindows7TaskBar();
        }

        void CurrentPlaylist_PlayStatusChanged(object sender, EventArgs e)
        {
            UpdateWindows7TaskBar();
        }

        private void UpdateWindows7TaskBar()
        {
            if (CurrentPlaylist.IsPaused || !CurrentPlaylist.IsPlaying)
            {
                tbiPlay.ImageSource = ImageLoader.FromResource("TaskBarPlay.png");
                tbiPlay.Command = HitbaseCommands.Play;
                tbiPlay.CommandTarget = this;
                tbiPlay.Description = StringTable.Play;
            }
            else
            {
                tbiPlay.ImageSource = ImageLoader.FromResource("TaskBarPause.png");
                tbiPlay.Command = HitbaseCommands.Pause;
                tbiPlay.CommandTarget = this;
                tbiPlay.Description = StringTable.Pause;
            }
        }


        void dtCloseSplashScreen_Tick(object sender, EventArgs e)
        {
            if (App.SplashScreen != null)
            {
                App.SplashScreen.CloseSplashScreen();
            }

            dtCloseSplashScreen.Stop();
        }

        void dtWishList_Tick(object sender, EventArgs e)
        {
            if (this.Wishlist == null)
                return;

            foreach (WishlistItem item in this.Wishlist)
            {
                if (!item.AlreadyReminded && item.Reminder < DateTime.Now && item.Reminder != DateTime.MinValue)
                {
                    item.AlreadyReminded = true;

                    FormWishlistReminder formWishlistReminder = new FormWishlistReminder(this.Wishlist, item, DataBase);
                    formWishlistReminder.Show(new NativeWindowWrapper(this));
                }
            }
        }

        private void CDTitleChanged(object sender, RoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = null;
            CDUserControl cdUserControl = e.OriginalSource as CDUserControl;

            if (cdUserControl != null)
            {
                mainCDUserControl = VisualTreeExtensions.FindParent<MainCDUserControl>(cdUserControl);
            }
            else
            {
                mainCDUserControl = e.OriginalSource as MainCDUserControl;
            }

            if (mainCDUserControl == null)
                return;

            string cdTitle = mainCDUserControl.GetCDTitle();

            foreach (TabItem item in this.tabControl.Items)
            {
                if (item.Content == mainCDUserControl)
                {
                    TabItemModel tabItemModel = item.DataContext as TabItemModel;
                    tabItemModel.Title = cdTitle;
                }
            }

            mainTreeUserControl.UpdateCDDrive(mainCDUserControl.CDEngine.DriveLetter, cdTitle);
        }

        private DateTime lastRefreshed = DateTime.Now;
        private bool lastTimeRunning = false;
        private bool alreadyRefreshed = false;

        void dtSynchronizeCatalog_Tick(object sender, EventArgs e)
        {
            if (SynchronizeCatalogWorker.Instance.IsRunning)
            {
                TextBlockStatus2.Text = string.Format(StringTable.NumberOfItems, SynchronizeCatalogWorker.Instance.NumberOfItems);

                CatalogView catalogView = GetActiveTabContent() as CatalogView;
                if (catalogView != null)
                {
                    // Ich sag mal alle 10 Sekunden aktualisieren
                    // Erst mal nicht! Nur wenn die ersten 100 Files gescannt wurden.
                    if (SynchronizeCatalogWorker.Instance.NumberOfChangedItems > 100 && !alreadyRefreshed)
                    {
                        alreadyRefreshed = true;
                        catalogView.FillList();
                        lastRefreshed = DateTime.Now;
                    }
                }
            }
            else
            {
                if (lastTimeRunning)
                {
                    alreadyRefreshed = true;

                    CatalogView catalogView = GetActiveTabContent() as CatalogView;
                    if (catalogView != null && SynchronizeCatalogWorker.Instance.NumberOfChangedItems > 0)
                    {
                        catalogView.FillList();
                    }
                }
            }

            lastTimeRunning = SynchronizeCatalogWorker.Instance.IsRunning;
        }

        private void CommandBindingShowMainStatusText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool show = (bool)e.Parameter;

            StatusPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CommandBindingSetMainStatusText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatusPanel.Visibility = Visibility.Visible;

            string[] text = ((string)e.Parameter).Split("\n".ToArray());

            if (text.Length == 1)
            {
                TextBlockStatus1.Text = text[0];
                TextBlockStatus2.Text = "";
            }
            else
            {
                TextBlockStatus1.Text = text[0];
                TextBlockStatus2.Text = text[1];
            }
        }

        private void UpdateWindowState()
        {
            toggleButtonViewNavigationBar.IsChecked = Settings.Current.ShowNavigationBar;
            toggleButtonViewPlaylist.IsChecked = Settings.Current.ShowPlaylist;
            toggleButtonViewPlayer.IsChecked = Settings.Current.ShowPlayer;
            toggleButtonShowFrequencyBand.IsChecked = Settings.Current.ShowFrequencyBand;
            toggleButtonShowLyrics.IsChecked = Settings.Current.ShowLyrics;

            SplitButtonCrossFade.IsChecked = Settings.Current.CrossFadeActive;

            recordNormalize.IsChecked = Settings.Current.NormalizeActive;
        }

        private void OpenDataBase()
        {
            if (string.IsNullOrEmpty(Settings.Current.LastDataBase))
            {
                Settings.Current.LastDataBase = System.IO.Path.Combine(Misc.GetPersonalHitbaseFolder(), "cd-katalog.hdbx");
                if (!File.Exists(Settings.Current.LastDataBase))
                    DataBase.Create(Settings.Current.LastDataBase);
            }

            // Prüfen, ob der Katalog noch vorhanden ist. Wenn nicht, dann muss einer ausgewählt werden.
            if (!File.Exists(Settings.Current.LastDataBase))
            {
                WindowMissingCatalog windowMissingCatalog = new WindowMissingCatalog();
                windowMissingCatalog.MissingCatalogFilename = Settings.Current.LastDataBase;
                if (windowMissingCatalog.ShowDialog() == false)
                {
                    Close();
                    return;
                }

                Settings.Current.LastDataBase = windowMissingCatalog.MissingCatalogFilename;
            }

            // Prüfen, ob es ein alter Katalog ist, und dann fragen, ob er konvertiert werden soll.
            if (SqlCeUpgrade.DetermineVersion(Settings.Current.LastDataBase) < SqlCeUpgrade.SQLCEVersion.SQLCE40)
            {
                WindowUpdateCatalog windowUpdateCatalog = new WindowUpdateCatalog();
                windowUpdateCatalog.UpdateCatalogFilename = Settings.Current.LastDataBase;
                if (windowUpdateCatalog.ShowDialog() == false)
                {
                    Close();
                    return;
                }

                Settings.Current.LastDataBase = windowUpdateCatalog.UpdateCatalogFilename;
            }

            // Muss ich hier wieder auf null setzen:
            // When you create a Window object, it checks to see if Application.Current is non-null, 
            // and if so, it checks to see if MainWindow is null. If it's null, it will set itself as
            // the MainWindow. This ensures that the first Window created is the main window (standard
            // and expected behavior).

            //App.Current.MainWindow = this;            

            DataBase = new DataBase();

            OpenDataBase(Settings.Current.LastDataBase);

            this.Title = System.IO.Path.GetFileName(Settings.Current.LastDataBase) + " - " + System.Windows.Forms.Application.ProductName;
        }

        private void OpenDataBase(string filename)
        {
            DataBase.Open(filename, false, true);

            AddToRecentFileList(filename);

            FillRecentFileList();

            DoAutoBackup(filename);

            SynchronizeCatalogWorker.Create(DataBase);
            this.mainTreeUserControl.DataBase = DataBase;
            this.playlistControl.DataBase = DataBase;
            this.WishlistUserControl.DataBase = DataBase;

            SynchronizeCatalogWorker.Instance.SyncStarted += new EventHandler(Instance_SyncStarted);
            SynchronizeCatalogWorker.Instance.SyncFinished += new EventHandler(Instance_SyncFinished);
            SynchronizeCatalogWorker.Instance.Start();
        }

        /// <summary>
        /// Automatische Sicherheitskopie anlegen
        /// Es wird pro Monat eine neue Datei angelegt.
        /// </summary>
        /// <param name="filename"></param>
        void DoAutoBackup(String filename)
        {
	        if (!Settings.Current.AutoBackup || string.IsNullOrEmpty(Settings.Current.AutoBackupDirectory))
		        return;

            try
            {
                string backupExtension = string.Format(".hdbx.lastautobackup.{0:D4}{1:D2}", DateTime.Now.Year, DateTime.Now.Month);

                String backupFile = System.IO.Path.ChangeExtension(filename, backupExtension);
                backupFile = System.IO.Path.Combine(Settings.Current.AutoBackupDirectory, System.IO.Path.GetFileName(backupFile));

                System.IO.File.Copy(filename, backupFile, true);
            }
            catch (Exception e)
            {
                UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(e, StringTable.AutoBackFailure);
                unhandledExceptionWindow.ShowDialog();
            }
        }

        private void AddToRecentFileList(string filename)
        {
            int foundIndex = -1;
            for (int i = 0; i < Settings.Current.RecentFileList.Count; i++)
            {
                if (string.Compare(Settings.Current.RecentFileList[i], filename, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    foundIndex = i;
                    break;
                }
            }

            // Wenn die Datei schon in der Liste ist, dann löschen, damit der Eintrag nach oben wandert
            if (foundIndex >= 0)
            {
                Settings.Current.RecentFileList.RemoveAt(foundIndex);
            }

            Settings.Current.RecentFileList.Insert(0, filename);
            if (Settings.Current.RecentFileList.Count > 7)
            {
                Settings.Current.RecentFileList.RemoveAt(7);
            }
        }

        void Instance_SyncStarted(object sender, EventArgs e)
        {
            StatusPanel.Visibility = System.Windows.Visibility.Visible;
            TextBlockStatus1.Text = "Synchronisierung wird durchgeführt...";
        }

        void Instance_SyncFinished(object sender, EventArgs e)
        {
            StatusPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        void albumView_AddTracksToPlaylist(string[] filenames, AddTracksToPlaylistType addToPlaylistType)
        {
        }

        private void FillMainTree()
        {
            mainTreeUserControl.FillTree();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
        }                    

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            CurrentPlaylist.Stop();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
//            Community.CommunityRoom cr = new Community.CommunityRoom();

//            cr.Show();

        }

        void dt_Tick(object sender, EventArgs e)
        {
            //WaveDataUserControl1.DrawWaveData3();//.InvalidateVisual();
            //WaveDataUserControl2.DrawSpectrum2();//.InvalidateVisual();
        }

        void mainTreeUserControl_ItemClicked(object sender, MainTreeItem selectedTreeItem, bool openInNewTab)
        {
            OpenTreeItem(selectedTreeItem, openInNewTab, 0);
        }

        void OpenTreeItem(MainTreeItem selectedTreeItem, bool openInNewTab, int autoPlayTrackNumber)
        {
            // Eventuell CD speichern, sonst gehen die Daten verloren
            if (Settings.Current.AutoSaveCDs)
            {
                HitbaseCommands.SaveCurrentCD.Execute(null, this);
            }

            MainPlaylistTreeItem playlistTreeItem = selectedTreeItem as MainPlaylistTreeItem;
            if (playlistTreeItem != null && playlistTreeItem.ItemType == PlaylistItemType.Playlist)
            {
                CurrentPlaylist.LoadFromFile(playlistTreeItem.Filename, true);
                return;
            }

            MainCatalogTreeItem catalogTreeItem = selectedTreeItem as MainCatalogTreeItem;
            if (catalogTreeItem != null)
            {
                if (catalogTreeItem.ShowItemType == ShowItemType.Track)     // Hierfür haben wir keine Anzeige im Tab
                    return;

                CatalogView catalogView;
                catalogView = new CatalogView();
                catalogView.ShowItemType = catalogTreeItem.ShowItemType;
                catalogView.DataBase = DataBase;
                catalogView.ConditionFromTree = catalogTreeItem.FilterCondition;
                SetCurrentTab(catalogTreeItem.Title, catalogTreeItem.ImageResourceString, catalogView, openInNewTab);

                switch (catalogTreeItem.ShowItemType)
                {
                    case ShowItemType.None:
                        break;
                    case ShowItemType.MyMusic:
                        if (Settings.Current.MyMusicViewType == 0)
                            catalogView.CurrentViewMode = CurrentViewMode.MyMusicDetails;
                        if (Settings.Current.MyMusicViewType == 1)
                            catalogView.CurrentViewMode = CurrentViewMode.MyMusicTable;

                        catalogView.FillList();
                        break;
                    case ShowItemType.Artist:
                        catalogView.CurrentViewMode = CurrentViewMode.MyMusicDetails;
                        catalogView.FillList();
                        break;
                    case ShowItemType.ArtistAll:
                    case ShowItemType.ArtistFirstChar:
                        catalogView.CurrentViewMode = CurrentViewMode.ArtistTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.AlbumAll:
                    case ShowItemType.AlbumArtistFirstChar:
                    case ShowItemType.AlbumArtist:
                        if (Settings.Current.AlbumViewType == 0)
                            catalogView.CurrentViewMode = CurrentViewMode.AlbumSymbols;
                        if (Settings.Current.AlbumViewType == 1)
                            catalogView.CurrentViewMode = CurrentViewMode.AlbumTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.Album:
                        catalogView.CurrentViewMode = CurrentViewMode.MyMusicDetails;
                        catalogView.FillList();
                        break;
                    case ShowItemType.Composer:
                        catalogView.CurrentViewMode = CurrentViewMode.MyMusicDetails;
                        catalogView.FillList();
                        break;
                    case ShowItemType.ComposerAll:
                    case ShowItemType.ComposerFirstChar:
                        catalogView.CurrentViewMode = CurrentViewMode.ComposerTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.Genre:
                        catalogView.CurrentViewMode = CurrentViewMode.GenreTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.Medium:
                        catalogView.CurrentViewMode = CurrentViewMode.MediumTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.YearAll:
                    case ShowItemType.YearDecade:
                    case ShowItemType.Year:
                        catalogView.CurrentViewMode = CurrentViewMode.YearTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.Rating:
                        catalogView.CurrentViewMode = CurrentViewMode.RatingTable;
                        catalogView.FillList();
                        break;
                    case ShowItemType.CDSet:
                        catalogView.CurrentViewMode = CurrentViewMode.MyMusicDetails;
                        catalogView.FillList();
                        break;
                    case ShowItemType.Directory:
                        if (Settings.Current.DirectoryViewType == 0)
                            catalogView.CurrentViewMode = CurrentViewMode.MyMusicDetails;
                        if (Settings.Current.DirectoryViewType == 1)
                            catalogView.CurrentViewMode = CurrentViewMode.MyMusicTable;

                        catalogView.FillList();
                        break;
                    default:
                        break;
                }
                
                return;
            }

            MainStatisticsTreeItem statisticsItem = selectedTreeItem as MainStatisticsTreeItem;
            if (statisticsItem != null)
            {
                statisticsUserControl = new StatisticsUserControl(DataBase, statisticsItem.ShowItemType);

                SetCurrentTab(statisticsItem.Title, statisticsItem.ImageResourceString, statisticsUserControl, openInNewTab);
            }

            MainCDTreeItem cdItem = selectedTreeItem as MainCDTreeItem;
            if (cdItem != null)
            {
                // Wenn für das CD-Laufwerk bereits ein Tab offen ist, dieses einfach anzeigen
                TabItem cdTabItem = FindTabForCDDrive(cdItem.CDEngine.DriveLetter);
                if (cdTabItem == null)
                {
                    MainCDUserControl cdUserControl = new MainCDUserControl(DataBase, cdItem.CD, cdItem.CDEngine, autoPlayTrackNumber, this.CurrentPlaylist);

                    SetCurrentTab(cdItem.Title, cdItem.ImageResourceString, cdUserControl, openInNewTab);
                }
                else
                {
                    tabControl.SelectedItem = cdTabItem;
                }
            }

            MainSearchTreeItem searchItem = selectedTreeItem as MainSearchTreeItem;
            if (searchItem != null && searchItem.SearchId != 0)
            {
                CatalogView catalogView;
                catalogView = new CatalogView();
                catalogView.DataBase = DataBase;
                catalogView.ConditionFilter = searchItem.Condition;

                SetCurrentTab(searchItem.Title, searchItem.ImageResourceString, catalogView, openInNewTab);

                catalogView.CurrentViewMode = searchItem.ViewMode;
                //catalogView.FillList();
                catalogView.ShowExtendedSearch();
            }
        }

        private TabItem FindTabForCDDrive(char driveLetter)
        {
            foreach (TabItem item in this.tabControl.Items)
            {
                MainCDUserControl mainCDUserControl = item.Content as MainCDUserControl;

                if (mainCDUserControl != null && mainCDUserControl.CDEngine.DriveLetter == driveLetter)
                {
                    return item;
                }
            }

            return null;
        }

        private TabItem AddNewTab(string header, string imageResourceString, UserControl content)
        {
            TabItem newTabItem = new TabItem();
            newTabItem.Style = FindResource("MyTabItemStyle") as Style;
            newTabItem.DataContext = CreateTabHeader(imageResourceString, header);
            newTabItem.Content = content;
            tabControl.Items.Add(newTabItem);
            tabControl.SelectedItem = newTabItem;

            return newTabItem;
        }

        private TabItemModel CreateTabHeader(string imageResourceString, string header)
        {
            TabItemModel model = new TabItemModel();

            model.ImageResourceString = imageResourceString;
            model.Title = header;

            return model;
            /*StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            if (imageSource != null)
            {
                Image img = new Image();
                img.Source = imageSource;
                img.Margin = new Thickness(0, 0, 4, 0);
                img.Width = 16;
                img.Height = 16;

                sp.Children.Add(img);
            }

            TextBlock tb = new TextBlock();
            tb.Text = header;
            sp.Children.Add(tb);
            
            return sp;*/
        }

        private void SetCurrentTab(string header, string imageResourceString, UserControl content, bool newTab = false)
        {
            TabItem currentItem = tabControl.SelectedItem as TabItem;

            if (currentItem == null || newTab)
            {
                // Noch kein Tab offen, also eins anlegen
                AddNewTab(header, imageResourceString, content);
                return;
            }

            if (!TabChangeAllowed(currentItem) && !newTab)
            {
                MessageUserControl messageUserControl = new MessageUserControl();
                messageUserControl.HeaderText = StringTable.RecordRunningCancel;
                messageUserControl.Text = StringTable.RecordRunningText;
                messageUserControl.Image = "/Big3.Hitbase.SharedResources;component/Images/Info.png";
                messageUserControl.WpfMessageBoxButtons = WpfMessageBoxButtons.YesNo;
                GlobalServices.ModalService.NavigateTo(messageUserControl, StringTable.RecordRunningTitle, delegate(bool returnValue)
                {
                    if (returnValue == true)
                    {
                        MainCDUserControl.RecordEngine.Canceled = true;
                        SetCurrentTab(header, imageResourceString, content, newTab);
                    }
                });

                return;
            }

            TabItemModel currentTabItemModel = currentItem.DataContext as TabItemModel;

            if (!newTab)
            {
                IMainTabInterface mainTabInterface = currentItem.Content as IMainTabInterface;

                if (mainTabInterface != null)
                {
                    if (!mainTabInterface.Closing())
                        return;
                }
            }

            currentItem.Style = FindResource("MyTabItemStyle") as Style;

            TabItemModel newTabItemModel = CreateTabHeader(imageResourceString, header);
            if (currentTabItemModel != null)        // Pin-Status merken
                newTabItemModel.IsPinned = currentTabItemModel.IsPinned;
            currentItem.DataContext = newTabItemModel;
            currentItem.Content = content;

            UpdateRibbonState();
        }

        private bool TabChangeAllowed(TabItem currentItem)
        {
            if (currentItem.Content is MainCDUserControl)
            {
                if (MainCDUserControl.RecordEngine.RecordRunning && !MainCDUserControl.RecordEngine.Canceled)
                {
                    return false;
                }
            }

            return true;
        }

        private void buttonViewNavigationBar_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.ShowNavigationBar)
            {
                HideNavigationBar();
                Settings.Current.ShowNavigationBar = false;
            }
            else
            {
                ShowNavigationBar();
                Settings.Current.ShowNavigationBar = true;
            }

            UpdateWindowState();
        }

        private void HideNavigationBar(bool disableAnimation = false)
        {
            DoubleAnimation dblAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(disableAnimation ? 1 : 500).Duration());
            BackEase be = new BackEase();
            be.EasingMode = EasingMode.EaseIn;
            dblAnim.EasingFunction = be;
            
            ScaleTransform st = new ScaleTransform();
            st.BeginAnimation(ScaleTransform.ScaleXProperty, dblAnim);
            mainTreeUserControl.LayoutTransform = st;
            ResizeGripperTree.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ShowNavigationBar()
        {
            DoubleAnimation dblAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500).Duration());
            BackEase be = new BackEase();
            dblAnim.EasingFunction = be;

            ScaleTransform st = new ScaleTransform();
            st.BeginAnimation(ScaleTransform.ScaleXProperty, dblAnim);
            mainTreeUserControl.LayoutTransform = st;
            ResizeGripperTree.Visibility = System.Windows.Visibility.Visible;
        }

        private void HidePlaylist(bool disableAnimation = false)
        {
            DoubleAnimation dblAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(disableAnimation ? 1 : 500).Duration());
            BackEase be = new BackEase();
            be.EasingMode = EasingMode.EaseIn;
            dblAnim.EasingFunction = be;

            ScaleTransform st = new ScaleTransform();
            st.BeginAnimation(ScaleTransform.ScaleXProperty, dblAnim);
            DockPanelRight.LayoutTransform = st;
            ResizeGripperPlaylist.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ShowPlaylist()
        {
            DoubleAnimation dblAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500).Duration());
            BackEase be = new BackEase();
            dblAnim.EasingFunction = be;

            ScaleTransform st = new ScaleTransform();
            st.BeginAnimation(ScaleTransform.ScaleXProperty, dblAnim);
            DockPanelRight.LayoutTransform = st;
            ResizeGripperPlaylist.Visibility = System.Windows.Visibility.Visible;
        }

        private void buttonViewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.ShowPlaylist)
            {
                HidePlaylist();
                Settings.Current.ShowPlaylist = false;
            }
            else
            {
                ShowPlaylist();
                Settings.Current.ShowPlaylist = true;
            }

            UpdateWindowState();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /*
        private void buttonAddMusicFiles_Click(object sender, RoutedEventArgs e)
        {
            FormAddFolder formAddFolder = new FormAddFolder(DataBase);

            if (formAddFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataBase.Master.MonitoredDirectories.Add(formAddFolder.Folder);
                DataBase.Master.WriteConfig(DataBase);

                SynchronizeCatalogWorker.Instance.Start();
                // Passiert jetzt im Hintergrund!
                
                //FormAddFolderAction formAddFolderAction = new FormAddFolderAction(DataBase, formAddFolder.Folder);

                //formAddFolderAction.ShowDialog();
            }
        }
        */

        private void CommandBindingAddTrackToPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddTracksToPlaylist(e, AddTracksToPlaylistType.Now);
        }
        
        private void CommandBindingAddTrackToPlaylistNow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddTracksToPlaylist(e, AddTracksToPlaylistType.Now);
        }

        private void CommandBindingAddTrackToPlaylistNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddTracksToPlaylist(e, AddTracksToPlaylistType.Next);
        }

        private void CommandBindingAddTrackToPlaylistLast_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddTracksToPlaylist(e, AddTracksToPlaylistType.End);
        }

        private void AddTracksToPlaylist(ExecutedRoutedEventArgs e, AddTracksToPlaylistType addToPlaylistType)
        {
            AddTracksToPlaylistParameter parameters = e.Parameter as AddTracksToPlaylistParameter;

            if (parameters != null)
            {
                if (parameters.Filenames.Count > 0)
                {
                    CurrentPlaylist.AddTracks(parameters.Filenames.ToArray(), parameters.AddTracksType, parameters.InsertIndex);
                }

                if (parameters.TrackIds.Count > 0)
                {
                    List<Track> tracks = new List<Track>();

                    foreach (int trackid in parameters.TrackIds)
                    {
                        Track track = DataBase.GetTrackById(trackid);
                        tracks.Add(track);
                    }
                    CurrentPlaylist.AddTracks(this.DataBase, tracks.ToArray(), parameters.AddTracksType, parameters.InsertIndex);
                }
            }

            AddCDTracksToPlaylistParameter addCDTracksParameters = e.Parameter as AddCDTracksToPlaylistParameter;

            if (addCDTracksParameters != null)
            {
                if (addCDTracksParameters.ClearPlaylist)
                {
                    CurrentPlaylist.ClearAll();
                }

                int playTrackIndex = 0;
                if (addCDTracksParameters.PlayTrackId > 0)
                {
                    int index = 0;

                    foreach (Track track in addCDTracksParameters.Tracks)
                    {
                        if (track.ID == addCDTracksParameters.PlayTrackId)
                        {
                            playTrackIndex = index;
                            break;
                        }

                        index++;
                    }
                }

                CurrentPlaylist.AddTracks(this.DataBase, addCDTracksParameters.Tracks.ToArray(), addCDTracksParameters.AddTracksType, addCDTracksParameters.InsertIndex, true, playTrackIndex);

                if (addCDTracksParameters.CD != null)
                {
                    playlistControl.CDPlaylistTitle = addCDTracksParameters.CD.Artist + " - " + addCDTracksParameters.CD.Title;
                }
            }

            if (e.Parameter is int)
            {
                int trackId = (int)e.Parameter;

                if (trackId != 0)
                {
                    Track track = DataBase.GetTrackById(trackId);

                    CurrentPlaylist.AddTracks(this.DataBase, new Track[] { track }, addToPlaylistType);
                }

                e.Handled = true;
            }
        }

        private void CommandBindingPreListenTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Current.PreListenVirgin)
            {
                FormPreListenVirgin formPreListenVirgin = new FormPreListenVirgin();
                if (formPreListenVirgin.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                Settings.Current.PreListenVirgin = false;
            }
            
            Track track = e.Parameter as Track;

            FadeInPreListenPlayer();

            PreListenPlayer.DataBase = DataBase;
            PreListenPlay(track);
        }

        private void PreListenPlay(Track track)
        {
            try
            {
                if (string.IsNullOrEmpty(track.Soundfile) || !System.IO.File.Exists(track.Soundfile))
                    return;

                SoundEngine.SoundEngine.PreListen.Play(track.Soundfile);
            }
            catch (Exception e)
            {
                //TextBlockTrackName.Text = Big3.Hitbase.SharedResources.StringTable.Error;
                //TextBlockTrackName.ToolTip = e.Message;
            }
        }

        private void CommandBindingOpenInNewTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenTreeItem(e.Parameter as MainTreeItem, true, 0);
        }

        private void CommandBindingCloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem ti = e.Parameter as TabItem;

            if (ti == null)
            {
                ti = tabControl.SelectedItem as TabItem;
            }
            
            if (ti != null)
            {
                CloseTab(ti);
            }
        }

        private void CloseTab(TabItem ti)
        {
            IMainTabInterface mainTabInterface = ti.Content as IMainTabInterface;

            if (mainTabInterface != null)
            {
                if (!mainTabInterface.Closing())
                    return;
            }

            tabControl.Items.Remove(ti);
        }

        private void CreateNewCatalog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = ".hdbx";
            sfd.OverwritePrompt = true;
            sfd.Title = StringTable.CreateCatalog;
            sfd.Filter = StringTable.FilterHitbase;

            if (sfd.ShowDialog(this) == true)
            {
                DataBase.Create(sfd.FileName);

                OpenCatalog(sfd.FileName);
            }
        }

        private void CommandBindingOpenCatalog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".hdbx";
            ofd.Filter = StringTable.FilterHitbase;
            if (ofd.ShowDialog(this) == true)
            {
                OpenCatalog(ofd.FileName);
            }
        }

        private void OpenCatalog(string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = "\"" + filename + "\" /NOSPLASHSCREEN";
            startInfo.FileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            Process.Start(startInfo);
            //DataBase.Open(ofd.FileName);
            Configuration.Settings.Current.LastDataBase = filename;
        }   

        #region IModalService Members

        private Stack<BackNavigationEventHandler> _backFunctions
            = new Stack<BackNavigationEventHandler>();

        void IModalService.NavigateTo(UserControl uc, string title, BackNavigationEventHandler backFromDialog)
        {
            NavigateModal(uc, title, backFromDialog, true);
        }

        void IModalService.NavigateTo(UserControl uc, string title, BackNavigationEventHandler backFromDialog, bool allowClose)
        {
            NavigateModal(uc, title, backFromDialog, allowClose);
        }

        private void NavigateModal(UserControl uc, string title, BackNavigationEventHandler backFromDialog, bool allowClose)
        {
            DoubleAnimation da = new DoubleAnimation(0.5, TimeSpan.FromMilliseconds(400).Duration());
            da.BeginTime = TimeSpan.FromMilliseconds(400);
            modalParentGrid.BeginAnimation(Grid.OpacityProperty, da);
            modalParentGrid.IsHitTestVisible = false;

            SimulatedWindow sw = new SimulatedWindow();
            sw.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            sw.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sw.Title = title;
            sw.AllowClose = allowClose;

            sw.WindowContent = uc;
            modalGrid.Children.Clear();
            modalGrid.Children.Add(sw);

            _backFunctions.Push(backFromDialog);
        }

        void IModalService.CloseModal()
        {
            if (modalGrid.Children.Count > 0)
            {
                SimulatedWindow sw = modalGrid.Children[0] as SimulatedWindow;
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        void IModalService.GoBackward(bool dialogReturnValue)
        {
            DoubleAnimation da = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400).Duration());
            modalParentGrid.BeginAnimation(Grid.OpacityProperty, da);
            modalParentGrid.IsHitTestVisible = true;
//            modalGrid.Children.RemoveAt(modalGrid.Children.Count - 1);

            //UIElement element = modalGrid.Children[modalGrid.Children.Count - 1];
            //element.IsEnabled = true;

            BackNavigationEventHandler handler = _backFunctions.Pop();
            if (handler != null)
                handler(dialogReturnValue);
        }

        #endregion

        private void CommandBindingConfigureMusicLibrary_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigureMusicLibraryUserControl configureUC = new ConfigureMusicLibraryUserControl();
            configureUC.DataBase = DataBase;
            GlobalServices.ModalService.NavigateTo(configureUC, StringTable.ConfigureMusicLibrary, delegate(bool returnValue)
            {
                if (returnValue == true)
                {
                    SynchronizeCatalogWorker.Instance.Start();

                    // Wenn über die Startseite konfiguriert, dann automatisch Katalog anzeigen
                    if (GetActiveTabContent() is FirstStepsUserControl)
                    {
                        OpenTreeItem(mainTreeUserControl.MyMusicTreeItem, false, 0);
                    }
                }
            });

        }

        private void MainWnd_Closing(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentPlaylist.Name))
            {
                this.playlistControl.SavePlaylist();
            }

            foreach (TabItem tabItem in tabControl.Items)
            {
                IMainTabInterface mainTabInterface = tabItem.Content as IMainTabInterface;

                if (mainTabInterface != null)
                {
                    if (!mainTabInterface.Closing())
                        return;
                }
            }

            if (SynchronizeCatalogWorker.IsInitialized)
            {
                // Synchronisierung stoppen
                // Uff... keine bessere Lösung gefunden!
                SynchronizeCatalogWorker.Instance.Cancel();
                while (SynchronizeCatalogWorker.Instance.IsRunning)
                {
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }

            // Prüfen, ob gerade ein CD-Kopiervorgang läuft
            MainCDUserControl.RecordEngine.Canceled = true;
            while (MainCDUserControl.RecordEngine.RecordRunning)
            {
                System.Threading.Thread.Sleep(100);
                System.Windows.Forms.Application.DoEvents();
            }

            if (DataBase != null)
            {
                RibbonWindowExtensions.SaveState(this, this.MyRibbon);

                Configuration.Settings.Current.Save();

                SavePinnedTabs();

                Settings.SaveWindowPlacement(this, "MainWindow");
            }

        }

        private void SavePinnedTabs()
        {
            int index = 0;

            // Alte Einträge löschen
            Registry.CurrentUser.DeleteSubKeyTree(Global.HitbaseRegistryKey + "\\PinnedTabs", false);

            // Gepinnte Tabs speichern
            foreach (TabItem tabItem in tabControl.Items)
            {
                TabItemModel tabItemModel = tabItem.DataContext as TabItemModel;
                if (tabItemModel.IsPinned)
                {
                    IMainTabInterface mainTabInterface = tabItem.Content as IMainTabInterface;

                    if (mainTabInterface != null)
                    {
                        using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Global.HitbaseRegistryKey + "\\PinnedTabs\\" + index.ToString()))
                        {
                            mainTabInterface.Save(regKey);

                            regKey.SetValue("Title", tabItemModel.Title);
                            regKey.SetValue("Image", tabItemModel.ImageResourceString);

                            if (tabItem.Content is CatalogView)
                            {
                                regKey.SetValue("Type", "CatalogView");
                            }

                            if (tabItem.Content is MainCDUserControl)
                            {
                                regKey.SetValue("Type", "MainCDUserControl");
                            }

                            if (tabItem.Content is StatisticsUserControl)
                            {
                                regKey.SetValue("Type", "StatisticsUserControl");
                            }
                        }
                    }
                }

                index++;
            }
        }

        private void RestorePinnedTabs()
        {
            try
            {
                int index = 0;

                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey + "\\PinnedTabs"))
                {
                    if (regKey == null)
                        return;

                    string[] subKeyNames = regKey.GetSubKeyNames();

                    if (subKeyNames == null)
                        return;

                    foreach (string subKey in subKeyNames)
                    {
                        string title = "";
                        string type = "";

                        using (RegistryKey regSubKey = Registry.CurrentUser.OpenSubKey(Global.HitbaseRegistryKey + "\\PinnedTabs\\" + subKey))
                        {
                            title = (string)regSubKey.GetValue("Title", "");
                            string imageUrl = (string)regSubKey.GetValue("Image", "");

                            UserControl userControl = null;
                            type = (string)regSubKey.GetValue("Type", "");

                            switch (type)
                            {
                                case "CatalogView":
                                    userControl = new CatalogView() { DataBase = DataBase };

                                    break;
                                case "MainCDUserControl":
                                    {
                                        string driveLetter = (string)regSubKey.GetValue("DriveLetter", "");

                                        MainCDTreeItem item = mainTreeUserControl.GetCDTreeItemDriveLetter(driveLetter[0]);

                                        userControl = new MainCDUserControl(DataBase, item.CD, item.CDEngine, 0, this.CurrentPlaylist);

                                        break;
                                    }
                                case "StatisticsUserControl":
                                    {
                                        userControl = new StatisticsUserControl(DataBase, StatisticType.Overview);

                                        break;
                                    }
                            }

                            IMainTabInterface mainTabInterface = userControl as IMainTabInterface;
                            if (mainTabInterface != null)
                                mainTabInterface.Restore(regSubKey);

                            TabItem newTabItem = AddNewTab(title, imageUrl, userControl);
                            TabItemModel tabItemModel = newTabItem.DataContext as TabItemModel;
                            tabItemModel.IsPinned = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(ex);
                unhandledExceptionWindow.ShowDialog();
            }
        }


        private void buttonSQL_Click(object sender, RoutedEventArgs e)
        {
            FormSql formSql = new FormSql(DataBase);
            formSql.ShowDialog();
        }

        #region resize tree
        private bool resizeTreeActive = false;
        private double orgWidth = 0;
        private double orgMousePosX = 0;
        private void ResizeGripperTree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                resizeTreeActive = true;
                orgWidth = mainTreeUserControl.Width;
                orgMousePosX = Mouse.GetPosition(this).X;
                ResizeGripperTree.CaptureMouse();
            }
        }

        private void ResizeGripperTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizeTreeActive)
            {
                double newWidth = orgWidth + (Mouse.GetPosition(this).X - orgMousePosX);
                if (newWidth < 50)
                    newWidth = 50;
                    
                mainTreeUserControl.Width = newWidth;
            }
        }

        private void ResizeGripperTree_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (resizeTreeActive)
            {
                ResizeGripperTree.ReleaseMouseCapture();
                resizeTreeActive = false;
                Settings.Current.MainTreeWidth = (int)mainTreeUserControl.Width;
            }
        }
        #endregion

        #region resize playlist
        private bool resizePlaylistActive = false;
        private double orgWidthPlaylist = 0;
        private double orgMousePosXPlaylist = 0;
        private void ResizeGripperPlaylist_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                resizePlaylistActive = true;
                orgWidthPlaylist = DockPanelRight.Width;
                orgMousePosXPlaylist = Mouse.GetPosition(this).X;
                ResizeGripperPlaylist.CaptureMouse();
            }
        }

        private void ResizeGripperPlaylist_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizePlaylistActive)
            {
                double newWidth = orgWidthPlaylist + (orgMousePosXPlaylist - Mouse.GetPosition(this).X);
                if (newWidth < 50)
                    newWidth = 50;

                DockPanelRight.Width = newWidth;
            }
        }

        private void ResizeGripperPlaylist_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (resizePlaylistActive)
            {
                ResizeGripperPlaylist.ReleaseMouseCapture();
                resizePlaylistActive = false;

                Settings.Current.PlaylistWidth = (int)DockPanelRight.Width;
            }
        }
        #endregion

        #region TabItem aus TabControl in eigenes Fenster rausziehen

        bool dragOutPending = false;
        bool dragOutActive = false;
        private TabItem dragOutTabItem = null;
        private TabControlDecoupledWindow dragOutWindow = null;

        private void tabControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem ti = VisualTreeExtensions.FindParent<TabItem>(e.OriginalSource as DependencyObject);
            System.Windows.Controls.Button closeButton = VisualTreeExtensions.FindParent<System.Windows.Controls.Button>(e.OriginalSource as DependencyObject);

            if (ti != null && closeButton == null)
            {
                if (ti == tabControl.SelectedItem)
                {
                    // Drag&Drop, um ein Tab aus dem TabControl zu lösen
                    tabControl.CaptureMouse();
                    dragOutPending = true;
                    dragOutActive = false;
                    dragOutTabItem = ti;
                    e.Handled = true;
                }
            }
        }

        private void tabControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragOutPending && !dragOutActive)
            {
                Point pt = Mouse.GetPosition(tabControl);
                if (!IsPointOverTabItemArea(tabControl.PointToScreen(pt)))
                {
                    dragOutActive = true;
                    UserControl uc = dragOutTabItem.Content as UserControl;
                    dragOutTabItem.Content = null;
                    tabControl.Items.Remove(dragOutTabItem);
                    dragOutWindow = new TabControlDecoupledWindow();
                    dragOutWindow.Top = this.PointToScreen(Mouse.GetPosition(this)).Y - 15;
                    dragOutWindow.Left = this.PointToScreen(Mouse.GetPosition(this)).X - 50;
                    dragOutWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    dragOutWindow.LocationChanged += new EventHandler(dragOutWindow_LocationChanged);
                    dragOutWindow.Content = uc;
                    dragOutWindow.Title = ((TabItemModel)dragOutTabItem.DataContext).Title;
                    dragOutWindow.ImageResourceString = ((TabItemModel)dragOutTabItem.DataContext).ImageResourceString;

                    dragOutWindow.Icon = (ImageSource)ImageLoader.FromResource(((TabItemModel)dragOutTabItem.DataContext).ImageResourceString).GetAsFrozen();
                    //double height = dragOutWindow.Icon.Height;
                   
                    dragOutWindow.Show();
                }
            }

            if (dragOutActive && e.LeftButton == MouseButtonState.Pressed)
            {
                dragOutWindow.Top = this.PointToScreen(Mouse.GetPosition(this)).Y - 15;
                dragOutWindow.Left = this.PointToScreen(Mouse.GetPosition(this)).X - 50;

             /*   Point pt = Mouse.GetPosition(tabControl);
                if (IsPointOverTabItemArea(tabControl.PointToScreen(pt)))
                {
                    UserControl uc = dragOutWindow.Content as UserControl;
                    dragOutWindow.Content = null;
                    dragOutWindow.Close();

                    dragOutTabItem = AddNewTab(dragOutWindow.Title, uc);
                    dragOutActive = false;
                }*/
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
          public Int32 X;
          public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        void dragOutWindow_LocationChanged(object sender, EventArgs e)
        {
            //if (dragOutActive)
            {
                Win32Point winpt = new Win32Point();
                GetCursorPos(ref winpt);
                Point pt = new Point(winpt.X, winpt.Y);
                if (IsPointOverTabItemArea(pt))
                {
                    UserControl uc = dragOutWindow.Content as UserControl;
                    dragOutWindow.LocationChanged -= new EventHandler(dragOutWindow_LocationChanged);
                    dragOutWindow.Content = null;
                    dragOutWindow.Close();

                    dragOutTabItem = AddNewTab(dragOutWindow.Title, dragOutWindow.ImageResourceString, uc);
                    dragOutActive = false;
                    dragOutPending = true;

                    // Drag&Drop, um ein Tab aus dem TabControl zu lösen
                    tabControl.CaptureMouse();

                }
            }
        }

        /// <summary>
        /// Prüft, ob die angegebenen Position (Screen-Koordinate) über dem TabItem-Bereich liegt.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private bool IsPointOverTabItemArea(Point pt)
        {
            Rect rectTabItemArea = new Rect();
            rectTabItemArea.Location = tabControl.PointToScreen(new Point(0, 0));
            rectTabItemArea.Width = tabControl.ActualWidth;
            rectTabItemArea.Height = 25;

            if (rectTabItemArea.Contains(pt))
                return true;
            else
                return false;
        }

        private void tabControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dragOutPending)
            {
                tabControl.ReleaseMouseCapture();
                dragOutPending = false;
                dragOutActive = false;
            }
        }

        #endregion

        private void CommandBindingEditCategories_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormCategories formCategories = new FormCategories(DataBase);

            formCategories.ShowDialog();
        }

        private void CommandBindingEditMediums_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormMediums formMediums = new FormMediums(DataBase);

            formMediums.ShowDialog();
        }

        private void CommandBindingAddNewAlbum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CD newCD = new CD();

            if (Settings.Current.AutoDateToday && this.DataBase.Master.DateType != DateType.None)
            {
                newCD.Date = Misc.GetDate();
            }

            WindowAlbum windowAlbum = new WindowAlbum(newCD, DataBase);
            windowAlbum.Owner = this;
            windowAlbum.ShowDialog();
        }

        private void CommandBindingSearchAmazon_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormSearchAmazon formSearchAmazon = new FormSearchAmazon(DataBase);

            formSearchAmazon.ShowDialog();
        }

        private void CommandBindingLoanManager_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormLoanManager formLoanManager = new FormLoanManager(DataBase);

            formLoanManager.ShowDialog();
        }

        private void CommandBindingEditCodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormCodes formCodes = new FormCodes(DataBase);

            formCodes.ShowDialog();
        }

        private void CommandBindingChangeCode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormChangeCodes formCodes = new FormChangeCodes(DataBase);

            formCodes.ShowDialog();
        }

        private void CommandBindingWishlist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WishlistUserControl.Visibility == System.Windows.Visibility.Visible)
                FadeOutWishlist();
            else
                FadeInWishlist();
        }

        private void CommandBindingPrintAlbum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int cdid = (int)e.Parameter;

            CD cd = DataBase.GetCDById(cdid);

            FormPrintCatalog printCatalog = new FormPrintCatalog(DataBase, cd);
            printCatalog.CurrentCD = false;
            printCatalog.ShowDialog(new NativeWindowWrapper(this));
        }

        private void CommandBindingCDLoaned_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int cdid = (int)e.Parameter;

            // Zuerst prüfen, ob die CD bereits verliehen ist.
            int count = (int)DataBase.ExecuteScalar("Select count(*) from LoanedCD WHERE CDID=" + cdid.ToString());
            if (count > 0)
            {
                MessageBoxResult res = MessageBox.Show(StringTable.AlreadyLoaned, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (res == MessageBoxResult.Yes)
                {
                    HitbaseCommands.LoanManager.Execute(null, this);
                }
                return;
            }

            LoanedCDDataSet.LoanedCDDataTable loanedCDs = new LoanedCDDataSet.LoanedCDDataTable();
            LoanedCDDataSet.LoanedCDRow loanedCDRow = loanedCDs.NewLoanedCDRow();
            loanedCDRow.CDID = cdid;

            FormLoanProperties formLoan = new FormLoanProperties(DataBase, loanedCDRow);
            if (formLoan.ShowDialog(new NativeWindowWrapper(this)) == System.Windows.Forms.DialogResult.OK)
            {
                // JUS: Workaround: Bei einigen Datenbanken (vielleicht mal ne alte Beta?) war die CDID in der Tabelle LoanedCD eine Identity-Spalte.
                // Macht keinen Sinn, aber so kann man hier den alten Bug umgehen
                try
                {
                    DataBase.ExecuteScalar("SET IDENTITY_INSERT LoanedCD ON");
                }
                catch
                { }

                Big3.Hitbase.DataBaseEngine.LoanedCDDataSetTableAdapters.LoanedCDTableAdapter cdta = new Big3.Hitbase.DataBaseEngine.LoanedCDDataSetTableAdapters.LoanedCDTableAdapter(DataBase);
                loanedCDs.AddLoanedCDRow(loanedCDRow);
                cdta.Update(loanedCDs);
            }
        }

        private void CommandBindingOpenAlbum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int cdid = (int)e.Parameter;

            CD cd = DataBase.GetCDById(cdid);
            WindowAlbum windowAlbum = new WindowAlbum(cd, DataBase);
            windowAlbum.Owner = this;

            windowAlbum.ShowDialog();
        }

        private void CommandBindingOpenTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int trackid = (int)e.Parameter;

            int cdid = DataBase.GetCDIDByTrackID(trackid);
            CD cd = DataBase.GetCDById(cdid);
            
            // Track innerhalb des Albums suchen
            int trackIndex = -1;
            for (int i = 0; i < cd.Tracks.Count; i++)
            {
                if (cd.Tracks[i].ID == trackid)
                {
                    trackIndex = i;
                    break;
                }
            }
            
            WindowAlbum windowAlbum = new WindowAlbum(cd, DataBase, trackIndex);
            windowAlbum.Owner = this;

            windowAlbum.ShowDialog();
        }

        private void CommandBindingSelectCodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TextBoxCodesWPF textBox = e.Parameter as TextBoxCodesWPF;

            if (textBox != null)
            {
                FormChooseCodes formChooseCodes = new FormChooseCodes(DataBase, textBox.Text);

                if (formChooseCodes.ShowDialog(new NativeWindowWrapper(this)) == System.Windows.Forms.DialogResult.OK)
                {
                    textBox.Text = formChooseCodes.Codes;
                }
            }
        }

        private void CommandBindingEditRoles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormEditRoles formEditRoles = new FormEditRoles(DataBase);

            formEditRoles.ShowDialog(new NativeWindowWrapper(this));
        }

        private void CommandBindingEditCDSets_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormEditCDSets formEditCDSets = new FormEditCDSets(DataBase);

            formEditCDSets.ShowDialog(new NativeWindowWrapper(this));
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRibbonState();
        }

        private void UpdateRibbonState()
        {
            if (tabControl.SelectedItem != null && ((TabItem)tabControl.SelectedItem).Content is MainCDUserControl)
            {
                if (ribbonContextGroupCD.Visibility == System.Windows.Visibility.Collapsed)
                    ribbonTabItemCD.IsSelected = true;
                ribbonContextGroupCD.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ribbonContextGroupCD.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void PreListenPlayer_Closed(object sender, EventArgs e)
        {
            FadeOutPreListenPlayer();
        }

        private void FadeInPreListenPlayer()
        {
            if (PreListenPlayer.Visibility == System.Windows.Visibility.Visible)
                return;

            PreListenPlayer.Visibility = System.Windows.Visibility.Visible;
            PreListenPlayer.Opacity = 0;
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400).Duration());
            da.BeginTime = TimeSpan.FromMilliseconds(200);
            PreListenPlayer.BeginAnimation(FrameworkElement.OpacityProperty, da);
            DoubleAnimation daHeight = new DoubleAnimation(0, 80, TimeSpan.FromMilliseconds(200).Duration());
            daHeight.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn };
            PreListenPlayer.BeginAnimation(FrameworkElement.HeightProperty, daHeight);
        }

        private void FadeOutPreListenPlayer()
        {
            DoubleAnimation da = new DoubleAnimation(1,0,TimeSpan.FromMilliseconds(400).Duration());
            PreListenPlayer.BeginAnimation(FrameworkElement.OpacityProperty, da);
            DoubleAnimation daHeight = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200).Duration());
            daHeight.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn };
            daHeight.BeginTime = TimeSpan.FromMilliseconds(400);
            daHeight.Completed += new EventHandler(daHeight_Completed);
            PreListenPlayer.BeginAnimation(FrameworkElement.HeightProperty, daHeight);
        }

        void daHeight_Completed(object sender, EventArgs e)
        {
            PreListenPlayer.Visibility = System.Windows.Visibility.Collapsed;
            PreListenPlayer.BeginAnimation(FrameworkElement.HeightProperty, null);
            PreListenPlayer.BeginAnimation(FrameworkElement.OpacityProperty, null);
        }

        private void Wishlist_Closed(object sender, EventArgs e)
        {
            FadeOutWishlist();
        }

        private void FadeInWishlist()
        {
            if (WishlistUserControl.Visibility == System.Windows.Visibility.Visible)
                return;

            WishlistUserControl.Visibility = System.Windows.Visibility.Visible;
            WishlistUserControl.Opacity = 0;
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400).Duration());
            da.BeginTime = TimeSpan.FromMilliseconds(200);
            WishlistUserControl.BeginAnimation(FrameworkElement.OpacityProperty, da);
            DoubleAnimation daHeight = new DoubleAnimation(0, 150, TimeSpan.FromMilliseconds(200).Duration());
            daHeight.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn };
            WishlistUserControl.BeginAnimation(FrameworkElement.HeightProperty, daHeight);
        }

        private void FadeOutWishlist()
        {
            DoubleAnimation da = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400).Duration());
            WishlistUserControl.BeginAnimation(FrameworkElement.OpacityProperty, da);
            DoubleAnimation daHeight = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200).Duration());
            daHeight.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn };
            daHeight.BeginTime = TimeSpan.FromMilliseconds(400);
            daHeight.Completed += new EventHandler(daHeightWishlist_Completed);
            WishlistUserControl.BeginAnimation(FrameworkElement.HeightProperty, daHeight);
        }

        void daHeightWishlist_Completed(object sender, EventArgs e)
        {
            WishlistUserControl.Visibility = System.Windows.Visibility.Collapsed;
            WishlistUserControl.BeginAnimation(FrameworkElement.HeightProperty, null);
            WishlistUserControl.BeginAnimation(FrameworkElement.OpacityProperty, null);
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SynchronizeCatalogWorker.Instance.Cancel();
        }

        private void recordMP3(object sender, RoutedEventArgs e)
        {
            CheckMP3();

            recordMenuMP3Init();
            Settings.Current.RecordLastQuickSelectedFormat = 0;
            UpdateRecordFormatRibbon();
        }

        private void UpdateRecordFormatRibbon()
        {
            this.recordtypeMP3.IsChecked = (Settings.Current.RecordLastQuickSelectedFormat == 0);
            this.recordtypeWMA.IsChecked = (Settings.Current.RecordLastQuickSelectedFormat == 1);
            this.recordtypeOGG.IsChecked = (Settings.Current.RecordLastQuickSelectedFormat == 2);
            this.recordtypeFLAC.IsChecked = (Settings.Current.RecordLastQuickSelectedFormat == 3);
            this.recordtypeWAVE.IsChecked = (Settings.Current.RecordLastQuickSelectedFormat == 4);
            this.recordtypeUSER.IsChecked = (Settings.Current.RecordLastQuickSelectedFormat == 5);
        }

        private void recordWMA(object sender, RoutedEventArgs e)
        {
            recordMenuWMAInit();
            Settings.Current.RecordLastQuickSelectedFormat = 1;
            UpdateRecordFormatRibbon();
        }

        private void recordOGG(object sender, RoutedEventArgs e)
        {
            CheckOGG();

            recordMenuOGGInit();
            Settings.Current.RecordLastQuickSelectedFormat = 2;
            UpdateRecordFormatRibbon();
        }

        private void recordFLAC(object sender, RoutedEventArgs e)
        {
            CheckFLAC();

            recordMenuFLACInit();
            Settings.Current.RecordLastQuickSelectedFormat = 3;
            UpdateRecordFormatRibbon();
        }

        private void recordWAVE(object sender, RoutedEventArgs e)
        {
            recordMenuWAVEInit();
            Settings.Current.RecordLastQuickSelectedFormat = 4;
            UpdateRecordFormatRibbon();
        }

        private void recordUSER(object sender, RoutedEventArgs e)
        {
            recordMenuUSERInit();
            Settings.Current.RecordLastQuickSelectedFormat = 5;
            UpdateRecordFormatRibbon();

            Big3.Hitbase.RecordMedium.ExternalEncoder externalEncoder = new RecordMedium.ExternalEncoder();

            externalEncoder.ShowDialog();
        }

        /// <summary>
        /// Init settings for record from CD
        /// </summary>
        private void InitRecordSettings()
        {
            switch (Settings.Current.RecordLastQuickSelectedFormat)
            {
                case 0: // MP3
                    recordtypeMP3.IsChecked = true;
                    recordMenuMP3Init();
                    break;
                case 1: // WMA
                    recordtypeWMA.IsChecked = true;
                    recordMenuWMAInit();
                    break;
                case 2: // OGG
                    recordtypeOGG.IsChecked = true;
                    recordMenuOGGInit();
                    break;
                case 3: // FLAC
                    recordtypeFLAC.IsChecked = true;
                    recordMenuFLACInit();
                    break;
                case 4: // WAVE
                    recordtypeWAVE.IsChecked = true;
                    recordMenuWAVEInit();
                    break;
                case 5: // USER
                    recordtypeUSER.IsChecked = true;
                    recordMenuUSERInit();
                    break;
                default:
                    recordtypeMP3.IsChecked = true;
                    recordMenuMP3Init();
                    break;
            }
            if (Settings.Current.RecordAutoCDCopy == true)
                recordAutoCDCopy.IsChecked = true;
            else
                recordAutoCDCopy.IsChecked = false;

            if (Settings.Current.RecordAutoEject == true)
                recordEjectAfterCopy.IsChecked = true;
            else
                recordEjectAfterCopy.IsChecked = false;

            if (Settings.Current.RecordAutoCreateM3U == true)
                recordAutoCreateM3U.IsChecked = true;
            else
                recordAutoCreateM3U.IsChecked = false;

            if (Settings.Current.RecordSaveCDCover == true)
                recordSaveCDCover.IsChecked = true;
            else
                recordSaveCDCover.IsChecked = false;

            UpdateRecordSpeed();
        }

        private void UpdateRecordSpeed()
        {
            recordSpeed1x.IsChecked = (Settings.Current.RecordSpeed == 1);
            recordSpeed2x.IsChecked = (Settings.Current.RecordSpeed == 2);
            recordSpeed4x.IsChecked = (Settings.Current.RecordSpeed == 4);
            recordSpeed8x.IsChecked = (Settings.Current.RecordSpeed == 8);
            recordSpeed12x.IsChecked = (Settings.Current.RecordSpeed == 12);
            recordSpeed16x.IsChecked = (Settings.Current.RecordSpeed == 16);
            recordSpeed24x.IsChecked = (Settings.Current.RecordSpeed == 24);
            recordSpeed32x.IsChecked = (Settings.Current.RecordSpeed == 32);
            recordSpeed48x.IsChecked = (Settings.Current.RecordSpeed == 48);
            recordSpeedMax.IsChecked = (Settings.Current.RecordSpeed == 0);
        }

        /// <summary>
        /// Init verfügbare Einträge für MP3 Menü 
        /// </summary>
        private void recordMenuMP3Init()
        {
            while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }

            recordQuality.Items.Add(new RibbonMenuItem() { Header = "64  KBit/s (kleine Größe)" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "96  KBit/s" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "128 KBit/s" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "160 KBit/s" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "192 KBit/s (empfohlen)" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "256 KBit/s" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "320 KBit/s (optimale Qualität)" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Erweitert...", Command = HitbaseCommands.ShowExtendedQualityDialog });

            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.Click += new RoutedEventHandler(menuItemMP3Quality_Click);
            }

            UpdateRecordMP3QualityRibbon();
        }

        void menuItemMP3Quality_Click(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem menuItem = sender as RibbonMenuItem;
            RibbonMenuItem parentMenuItem = menuItem.Parent as RibbonMenuItem;

            int index = parentMenuItem.Items.IndexOf(menuItem);
            Settings.Current.RecordLastQuickSelectedMP3Quality = index;
            UpdateRecordMP3QualityRibbon();
        }

        private void UpdateRecordMP3QualityRibbon()
        {
            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.IsChecked = false;
            }

            if (Settings.Current.RecordLastQuickSelectedMP3Quality < recordQuality.Items.Count)
                ((RibbonMenuItem)recordQuality.Items[Settings.Current.RecordLastQuickSelectedMP3Quality]).IsChecked = true;
            else
                ((RibbonMenuItem)recordQuality.Items[4]).IsChecked = true;

        }

        /// <summary>
        /// Init verfügbare Einträge für WMA Menü 
        /// </summary>
        private void recordMenuWMAInit()
        {
            while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Für Format WMA sind keine Optionen verfügbar.", IsEnabled = false });

            /*while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "64  KBit/s (kleine Größe)" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "96  KBit/s" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "128 KBit/s" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "160 KBit/s" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "192 KBit/s (empfohlen)" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "256 KBit/s" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "320 KBit/s (optimale Qualität)" });
            //recordQuality.Items.Add(new RibbonMenuItem() { Header = "Erweitert...", Command = HitbaseCommands.ShowExtendedQualityDialog });

            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.Click += new RoutedEventHandler(menuItemWMAQuality_Click);
            }

            UpdateRecordWMAQualityRibbon();*/
        }

        void menuItemWMAQuality_Click(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem menuItem = sender as RibbonMenuItem;
            RibbonMenuItem parentMenuItem = menuItem.Parent as RibbonMenuItem;

            int index = parentMenuItem.Items.IndexOf(menuItem);
            Settings.Current.RecordLastQuickSelectedWMAQuality = index;
            UpdateRecordWMAQualityRibbon();
        }

        private void UpdateRecordWMAQualityRibbon()
        {
            /*foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.IsChecked = false;
            }

            if (Settings.Current.RecordLastQuickSelectedWMAQuality < recordQuality.Items.Count)
                ((RibbonMenuItem)recordQuality.Items[Settings.Current.RecordLastQuickSelectedWMAQuality]).IsChecked = true;
            else
                ((RibbonMenuItem)recordQuality.Items[4]).IsChecked = true;*/

        }

        /// <summary>
        /// Init verfügbare Einträge für Ogg Menü 
        /// </summary>
        private void recordMenuOGGInit()
        {
            while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }
            //-q 10| 460…500 kbps VBR
            //-q 9 | 300…340 kbps VBR
            //-q 8 | 240…280 kbps VBR
            //-q 7 | 205…245 kbps VBR
            //-q 6 | 170…210 kbps VBR (empfohlen)
            //-q 5 | 145…185 kbps VBR
            //-q 4 | 110…150 kbps VBR
            //-q 3 | 90…130 kbps VBR
            //-q 2 | 80…100 kbps VBR
            //-q 1 | 65…85 kbps VBR
            //-q 0 | 50…70 kbps VBR
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "minimale Qualität/Dateigröße", Name = "RecordQualityI1" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "geringe Qualität/Dateigröße" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "mittlere Qualität/Dateigröße" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "optimale Qualität/Dateigröße(empfohlen)" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "hohe Qualität/Dateigröße" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "maximale Qualität/Dateigröße" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Erweitert...", Command = HitbaseCommands.ShowExtendedQualityDialog });

            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.Click += new RoutedEventHandler(menuItemOGGQuality_Click);
            }

            UpdateRecordOGGQualityRibbon();
        }

        void menuItemOGGQuality_Click(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem menuItem = sender as RibbonMenuItem;
            RibbonMenuItem parentMenuItem = menuItem.Parent as RibbonMenuItem;

            int index = parentMenuItem.Items.IndexOf(menuItem);
            Settings.Current.RecordLastQuickSelectedOGGQuality = index;
            UpdateRecordOGGQualityRibbon();
        }

        private void UpdateRecordOGGQualityRibbon()
        {
            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.IsChecked = false;
            }

            if (Settings.Current.RecordLastQuickSelectedOGGQuality < recordQuality.Items.Count)
                ((RibbonMenuItem)recordQuality.Items[Settings.Current.RecordLastQuickSelectedOGGQuality]).IsChecked = true;
            else
                ((RibbonMenuItem)recordQuality.Items[3]).IsChecked = true;

        }

        /// <summary>
        /// Init verfügbare Einträge für FLAC Menü 
        /// </summary>
        private void recordMenuFLACInit()
        {
            while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }

            RibbonMenuItem miHighSpeed = new RibbonMenuItem() { Header = "Hohe Geschwindigkeit/Dateigröße" };
            miHighSpeed.Click += new RoutedEventHandler(miHighSpeed_Click);
            recordQuality.Items.Add(miHighSpeed);
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "mittlere Geschwindigkeit/Dateigröße(empfohlen)" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Geringe Geschwindigkeit/Dateigröße" });
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Erweitert...", Command = HitbaseCommands.ShowExtendedQualityDialog });
            // flac "-0" bis "-8" - "-5" empfohlen
            //textFlacParameter.Text = "-5 %1 %2";

            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.Click += new RoutedEventHandler(menuItemFLACQuality_Click);
            }

            UpdateRecordFLACQualityRibbon();
        }

        void menuItemFLACQuality_Click(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem menuItem = sender as RibbonMenuItem;
            RibbonMenuItem parentMenuItem = menuItem.Parent as RibbonMenuItem;

            int index = parentMenuItem.Items.IndexOf(menuItem);
            Settings.Current.RecordLastQuickSelectedFLACQuality = index;
            UpdateRecordFLACQualityRibbon();
        }

        private void UpdateRecordFLACQualityRibbon()
        {
            foreach (RibbonMenuItem menuItem in recordQuality.Items)
            {
                menuItem.IsChecked = false;
            }

            if (Settings.Current.RecordLastQuickSelectedFLACQuality < recordQuality.Items.Count)
                ((RibbonMenuItem)recordQuality.Items[Settings.Current.RecordLastQuickSelectedFLACQuality]).IsChecked = true;
            else
                ((RibbonMenuItem)recordQuality.Items[1]).IsChecked = true;

        }


        void miHighSpeed_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Init verfügbare Einträge für FLAC Menü 
        /// </summary>
        private void recordMenuWAVEInit()
        {
            while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Für Format WAVE sind keine Optionen verfügbar.", IsEnabled = false });
        }

        /// <summary>
        /// Init verfügbare Einträge für FLAC Menü 
        /// </summary>
        private void recordMenuUSERInit()
        {
            while (recordQuality.Items.Count > 0) { recordQuality.Items.RemoveAt(0); }
            recordQuality.Items.Add(new RibbonMenuItem() { Header = "Für Format Benutzerdefiniert sind keine Optionen verfügbar.", IsEnabled = false });
        }

        private void recordAutoCDCopy_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordAutoCDCopy = true;
        }

        private void recordAutoCDCopy_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordAutoCDCopy = false;
        }

        private void recordEjectAfterCopy_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordAutoEject = true;
        }

        private void recordEjectAfterCopy_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordAutoEject = false;
        }

        private void recordAutoCreateM3U_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordAutoCreateM3U = true;
        }

        private void recordAutoCreateM3U_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordAutoCreateM3U = false;
        }

        private void recordSaveCDCover_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordSaveCDCover = true;
        }

        private void recordSaveCDCover_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Current.RecordSaveCDCover = false;
        }

        private void ShowOptions(int currentPage = 0)
        {
            Big3.Hitbase.NativeGUI.OptionsDialog optionsDialog = new NativeGUI.OptionsDialog();
            optionsDialog.Show(currentPage);

            if (Settings.Current.ScrobbleActive)
            {
                this.CurrentPlaylist.InitScrobble();
            }
            Settings.Current.Save();
        }

        private object GetActiveTabContent()
        {
            if (this.tabControl == null)
                return null;

            TabItem tabItem = this.tabControl.SelectedItem as TabItem;

            if (tabItem != null)
            {
                return tabItem.Content;
            }

            return null;
        }

        private void CommandBindingSaveCurrentCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                mainCDUserControl.SaveCD();
            }
        }


        private void CommandBindingShowRecordOptions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                mainCDUserControl.ShowRecordOptions();
            }
        }

        private void CommandBindingStartRecord_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StartRecord(false);
        }

        private void StartRecord(bool selectedTracksOnly)
        {
            HitbaseCommands.SaveCurrentCD.Execute(null, this);

            bool copyOk = true;

            switch (Settings.Current.RecordLastQuickSelectedFormat)
            {
                case 0: // MP3
                    copyOk = CheckMP3();
                    break;
                case 2: // OGG
                    copyOk = CheckOGG();
                    break;
                case 3: // FLAC
                    copyOk = CheckFLAC();
                    break;
            }

            if (!copyOk)
                return;

            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                mainCDUserControl.StartRecord(selectedTracksOnly);
            }
        }

        private void CommandBindingCancelRecord_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                mainCDUserControl.CancelRecord();
            }
        }

        private void CommandBindingEditDatabaseFields_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormDataBaseFields formDataBaseFields = new FormDataBaseFields(this.DataBase);

            formDataBaseFields.ShowDialog(new NativeWindowWrapper(this));
        }

        private void CommandBindingBurnCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // OK
            string winsysdir = System.Environment.GetFolderPath(Environment.SpecialFolder.System);

            if (!File.Exists(winsysdir + "\\imapi2.dll"))
            {
                IMAPI2Missing imapi = new IMAPI2Missing();
                imapi.Owner = this;
                imapi.ShowDialog();

                return;
            }

            BurnMedium formBurnMedia = new BurnMedium(DataBase, CurrentPlaylist);
            if (formBurnMedia.IsInitialized)
                formBurnMedia.Show();
        }

        private void CommandBindingStartRecord_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !MainCDUserControl.RecordEngine.RecordRunning && IsCDInDriveForCurrentTab();

            if (GroupBoxRecordRunning != null)
            {
                if (MainCDUserControl.RecordEngine.RecordRunning)
                {
                    GroupBoxRecordRunning.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    GroupBoxRecordRunning.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void CommandBindingCopyFields_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormTransferFields formTransferFields = new FormTransferFields(DataBase);
            formTransferFields.ShowDialog();
        }

        private void CommandBindingSearchAndReplace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormSearchAndReplace formSearchAndReplace = new FormSearchAndReplace(DataBase);
            formSearchAndReplace.ShowDialog();
        }

        private void CommandBindingSearchCDInCDArchive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                mainCDUserControl.SearchCD();
            }
        }

        private void CommandBindingSendCatalogToInternet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CCDArchive cdarchive = new CCDArchive();

            // Zuerst prüfen, ob überhaupt ein Archiv zum Uploaden vorhanden ist!
            if (cdarchive.IsUploadableArchivAvailable() == 0)
            {
                if (MessageBox.Show(StringTable.NoUploadableCDArchive, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ShowOptions(2);
                }

                // Nochmal prüfen!
                if (cdarchive.IsUploadableArchivAvailable() == 0)
                    return;
            }

            UploadCatalogWindow uploadCatalogWindow = new UploadCatalogWindow();
            uploadCatalogWindow.DataBase = DataBase;
            uploadCatalogWindow.Owner = this;
            uploadCatalogWindow.ShowDialog();
        }

        private void HidePlayer()
        {
            this.PlayerControl.Visibility = System.Windows.Visibility.Collapsed;
            this.PlayerGridRow.Height = new GridLength(0);
            Settings.Current.ShowPlayer = false;
        }

        private void ShowPlayer()
        {
            this.PlayerControl.Visibility = System.Windows.Visibility.Visible;
            this.PlayerGridRow.Height = new GridLength(100);
            Settings.Current.ShowPlayer = true;
        }

        private void CommandBindingShowPlayer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.PlayerControl.Visibility == System.Windows.Visibility.Visible)
            {
                HidePlayer();
            }
            else
            {
                ShowPlayer();
            }

            UpdateWindowState();
        }

        private void CommandBindingAddToWishlist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is WishlistItem)
            {
                WishlistItem newItem = e.Parameter as WishlistItem;
                int cdId = DataBase.GetCDIDByTrackID(newItem.TrackID);

                newItem.ImageFilename = DataBase.GetFrontCoverByCdId(cdId);

                newItem.Time = DateTime.Now;
                this.Wishlist.Add(newItem);
                if (WishlistUserControl.Visibility == System.Windows.Visibility.Collapsed)
                    FadeInWishlist();
            }
            else
            {
                if (e.Parameter is List<WishlistItem>)
                {
                    List<WishlistItem> newItems = e.Parameter as List<WishlistItem>;
                    foreach (WishlistItem newItem in newItems)
                    {
                        int cdId = DataBase.GetCDIDByTrackID((int)newItem.TrackID);

                        newItem.ImageFilename = DataBase.GetFrontCoverByCdId(cdId);

                        newItem.Time = DateTime.Now;
                        this.Wishlist.Add(newItem);
                    }
                    if (WishlistUserControl.Visibility == System.Windows.Visibility.Collapsed)
                        FadeInWishlist();
                }
                else
                {
                    Track track = DataBase.GetTrackById((int)e.Parameter);
                    CD cd = DataBase.GetCDById(track.CDID);

                    WishlistItem newItem = new WishlistItem();
                    newItem.TrackID = track.ID;
                    newItem.Artist = track.Artist;
                    newItem.Title = track.Title;
                    newItem.ImageFilename = cd.CDCoverFrontFilename;

                    FormWishlistItem formWishlistItem = new FormWishlistItem(newItem);
                    if (formWishlistItem.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        newItem.Time = DateTime.Now;
                        this.Wishlist.Add(newItem);
                        if (WishlistUserControl.Visibility == System.Windows.Visibility.Collapsed)
                            FadeInWishlist();
                    }
                }
            }
        }

        private void CommandBindingSaveCondition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.mainTreeUserControl.RefreshPersonalSearches();

            this.mainTreeUserControl.UpdateLayout();

            if (e.Parameter != null && e.Parameter is string)
                this.mainTreeUserControl.EditPersonalSearch((string)e.Parameter);
        }
    
        private void CommandBindingNewPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Playlist.CreateEmptyPlaylist();
        }

        private void CommandBindingPlayInFullScreen2D_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MosaicAlbum mosaicAlbum = new MosaicAlbum(DataBase, CurrentPlaylist, false);
            mosaicAlbum.Show();
        }

        private void CommandBindingPlayInFullScreen3D_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MosaicAlbum mosaicAlbum = new MosaicAlbum(DataBase, CurrentPlaylist, true);
            mosaicAlbum.Show();
        }

        private void MainWnd_Loaded(object sender, RoutedEventArgs e)
        {
            dtCloseSplashScreen.Interval = TimeSpan.FromSeconds(1);
            dtCloseSplashScreen.Tick += new EventHandler(dtCloseSplashScreen_Tick);
            dtCloseSplashScreen.Start();

            Settings.RestoreWindowPlacement(this, "MainWindow");

            DockPanelRight.Width = Settings.Current.PlaylistWidth;
            mainTreeUserControl.Width = Settings.Current.MainTreeWidth;
        }

        private void CommandBindingAddView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddViewCommandParameters addViewParams = e.Parameter as AddViewCommandParameters;

            CatalogView catalogView;
            catalogView = new CatalogView();
            catalogView.DataBase = DataBase;
            catalogView.ConditionFilter = addViewParams.Condition;
            catalogView.ConditionFromTree = addViewParams.ConditionFromTree;
            catalogView.CurrentViewMode = addViewParams.ViewMode;
            catalogView.FillList();
            SetCurrentTab(addViewParams.Title, addViewParams.ImageResourceString, catalogView, true);
        }

        private void CommandBindingSearchCDToCDArchive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CD cd = null;

            if (e.Parameter is int)
            {
                int cdid = (int)e.Parameter;

                cd = DataBase.GetCDById(cdid);
            }
            else
            {
                MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

                if (mainCDUserControl != null)
                    cd = mainCDUserControl.CD;
            }

            if (cd == null)
                return;

            CCDArchive cdarchive = new CCDArchive();
            int result = 0;
            cdarchive.Upload(cd, ref result, IntPtr.Zero, (int)(CDArchivUploadOptions.UPLOAD_OPTIONS_VERBOSE|CDArchivUploadOptions.UPLOAD_OPTIONS_CD_UPDATE));
        }

        private void CommandBindingChooseSkin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            /*string skinName = "Blue";

            switch ((string)e.Parameter)
            {
                case "Default":
                    skinName = "Blue";
                    break;
                case "Black":
                    skinName = "Black";
                    break;
                case "Silver":
                    skinName = "Silver";
                    break;
            }

            foreach (ResourceDictionary rd in this.Resources.MergedDictionaries)
            {
                if (rd.Source.OriginalString.Contains("Fluent"))
                {
                    rd.Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Office2010/" + skinName + ".xaml");
                    break;
                }
            }*/

            switch ((string)e.Parameter)
            {
                case "Default":
                    Settings.Current.CurrentColorStyle = ColorStyle.Default;
                    break;
                case "Black":
                    Settings.Current.CurrentColorStyle = ColorStyle.Black;
                    break;
                case "Silver":
                    Settings.Current.CurrentColorStyle = ColorStyle.Silver;
                    break;
            }

            ThemeManager.SetTheme((string)e.Parameter);
        }

        private void CommandBindingOpenTrackLocation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is int && (int)e.Parameter > 0)
            {
                Track track = DataBase.GetTrackById((int)e.Parameter);

                if (track != null && !string.IsNullOrEmpty(track.Soundfile))
                {
                    Misc.ShowFileInExplorer(track.Soundfile);
                }
            }

            if (e.Parameter is string)
            {
                string filename = e.Parameter as string;

                if (filename != null && !string.IsNullOrEmpty(filename))
                {
                    Misc.ShowFileInExplorer(filename);
                }
            }
        }

        private void CommandBindingOpenTrackLocation_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is int && (int)e.Parameter > 0)
            {
                string filename = DataBase.GetSoundfileByTrackId((int)e.Parameter);

                if (!string.IsNullOrEmpty(filename))
                {
                    e.CanExecute = true;
                    return;
                }
            }

            if (e.Parameter is string)
            {
                string filename = (string)e.Parameter;

                if (!string.IsNullOrEmpty(filename))
                {
                    e.CanExecute = true;
                    return;
                }
            }

            e.CanExecute = false;
        }

        private void CommandBindingPrevTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CurrentPlaylist.PlayPrev();
        }

        private void CommandBindingPlay_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            // Wenn die Playliste leer ist, und man auf einer CD-tab steht, dann die CD spielen.
            if (CurrentPlaylist.Count == 0 && mainCDUserControl != null)
            {
                HitbaseCommands.AddTracksToPlaylistNow.Execute(0, mainCDUserControl);
                return;
            }

            if (!CurrentPlaylist.IsPlaying)
            {
                CurrentPlaylist.Play();
            }
            else
            {
                if (CurrentPlaylist.IsPaused)
                {
                    CurrentPlaylist.Pause(false);
                }
                else
                {
                    CurrentPlaylist.Pause(true);
                }
            }
        }

        private void CommandBindingNextTrack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CurrentPlaylist.PlayNext();
        }

        private void CommandBindingPause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentPlaylist.IsPlaying)
                this.CurrentPlaylist.Pause(true);
            else
                this.CurrentPlaylist.Pause(false);
        }

        private void CommandBindingCopyToExternalMedium_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<string> filenames = new List<string>();

            filenames.AddRange(this.CurrentPlaylist.Select(x => x.Info.Filename));
            FormCopyTracksToExternalMedium formCopyTracksToExternalMedium = new FormCopyTracksToExternalMedium(filenames.ToArray());

            formCopyTracksToExternalMedium.ShowDialog();
        }

        private void CommandBindingEditPersonGroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormEditArtists formEditArtist = new FormEditArtists(DataBase);
            formEditArtist.Show();
        }

        private void CommandBindingUploadToMyHitbase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Community.MyHitbaseUserControl myHitbaseUserControl = new Community.MyHitbaseUserControl();
            myHitbaseUserControl.DataBase = DataBase;
            GlobalServices.ModalService.NavigateTo(myHitbaseUserControl, StringTable.UploadToMyHitbase, delegate(bool returnValue)
            {
                if (returnValue == true)
                {
                    
                }
            });

        }

        private void CommandBindingShowExtendedQualityDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (Settings.Current.RecordLastQuickSelectedFormat)
            {
                case 0: // MP3
                    ShowMP3Settings();
                    /*Big3.Hitbase.RecordMedium.MP3Settings mp3Settings = new RecordMedium.MP3Settings();
                    mp3Settings.ShowDialog();*/
                    break;
                case 1: // WMA
                    Big3.Hitbase.RecordMedium.WMASettings wmaSettings = new RecordMedium.WMASettings();
                    wmaSettings.ShowDialog();
                    break;
                case 2: // OGG
                    Big3.Hitbase.RecordMedium.OggVorbisSettings oggSettings = new RecordMedium.OggVorbisSettings();
                    oggSettings.ShowDialog();
                    break;
                case 3: // FLAC
                    Big3.Hitbase.RecordMedium.FlacSettings flacSettings = new RecordMedium.FlacSettings();
                    flacSettings.ShowDialog();
                    break;
                case 4: // WAVE
                    break;
                case 5: // USER
                    break;
            }
        }

        private void ShowMP3Settings()
        {
            MP3SettingsUserControl mp3SettingsUserControl = new MP3SettingsUserControl();
            GlobalServices.ModalService.NavigateTo(mp3SettingsUserControl, StringTable.MP3Settings, delegate(bool returnValue)
            {
                if (returnValue == true)
                {

                }
            });

        }

        private void CommandBindingBrowseCDArchive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WindowBrowseCDArchive windowBrowseCDArchive = new WindowBrowseCDArchive(DataBase);
            windowBrowseCDArchive.Owner = this;
            windowBrowseCDArchive.ShowDialog();
        }

        private bool CheckMP3()
        {
            // Prüfen, ob die lame_enc.dll vorhanden ist
            string lameEncPath = System.IO.Path.Combine(Misc.GetApplicationPath(), "lame_enc.dll");
            bool retVal = false;

            if (GetDLLInfo.GetUnmanagedDllType(lameEncPath) != 32)
            {
                WindowMP3HowTo windowMP3HowTo = new WindowMP3HowTo();
                windowMP3HowTo.ShowDialog();

                retVal = windowMP3HowTo.IsOK;
            }
            else
            {
                retVal = true;
            }

            return retVal;
        }

        private bool CheckFLAC()
        {
            // Prüfen, ob die flac.exe vorhanden ist
            string lameEncPath = System.IO.Path.Combine(Misc.GetApplicationPath(), "flac.exe");
            if (!File.Exists(lameEncPath))
            {
                WindowFLACHowTo windowFlacHowTo = new WindowFLACHowTo();
                windowFlacHowTo.ShowDialog();
            }

            return File.Exists(lameEncPath);
        }

        private bool CheckOGG()
        {
            // Prüfen, ob die oggenc.exe vorhanden ist
            string lameEncPath = System.IO.Path.Combine(Misc.GetApplicationPath(), "oggenc2.exe");
            if (!File.Exists(lameEncPath))
            {
                WindowOGGHowTo windowOggHowTo = new WindowOGGHowTo();
                windowOggHowTo.ShowDialog();
            }

            return File.Exists(lameEncPath);
        }

        private void CommandBindingLinkCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                if (mainCDUserControl.CD.ID > 0)
                {
                    if (MessageBox.Show(StringTable.CDAlreadyLinked, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                WindowLinkCD windowLinkCD = new WindowLinkCD(DataBase, mainCDUserControl.CD);
                windowLinkCD.Owner = this;
                if (windowLinkCD.ShowDialog() == true)
                {
                    mainCDUserControl.ReadCDInformation();
                }
            }
        }

        private void CommandBindingAdjustSpelling_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is int && e.Parameter != null)
            {
                CD cd = DataBase.GetCDById((int)e.Parameter);

                if (cd == null)
                    return;

                WpfMessageBoxResult result = WPFMessageBox.Show(this, StringTable.AskAdjustSpellingTitle,
                    StringTable.AdjustSpelling, 
                    "/Big3.Hitbase.SharedResources;component/Images/Question48.png", WpfMessageBoxButtons.YesNo, 
                    "AdjustSpellingDontAskAgain", false);

                if (result == WpfMessageBoxResult.Yes)
                {
                    cd.AdjustSpelling(Settings.Current.AdjustSpelling);

                    cd.Save(DataBase);
                }
            }
            else
            {
                MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

                if (mainCDUserControl != null)
                {
                    mainCDUserControl.CD.AdjustSpelling();
                }
            }
        }

        private void CommandBindingCopyCDArtistToTracks_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                foreach (Track track in mainCDUserControl.CD.Tracks)
                    track.Artist = mainCDUserControl.CD.Artist;
            }
        }

        private void CommandBindingCopyCDArtistToTracks_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                e.CanExecute = mainCDUserControl.CD.Sampler;
            }
        }

        private void SplitButtonCrossFade_DropDownOpened(object sender, EventArgs e)
        {
            MenuItemCrossFadeOff.IsChecked = (!Settings.Current.CrossFadeActive);
            MenuItemCrossFade2Seconds.IsChecked = (Settings.Current.CrossFadeDefaultSeconds == 2 && Settings.Current.CrossFadeActive);
            MenuItemCrossFade5Seconds.IsChecked = (Settings.Current.CrossFadeDefaultSeconds == 5 && Settings.Current.CrossFadeActive);
            MenuItemCrossFade10Seconds.IsChecked = (Settings.Current.CrossFadeDefaultSeconds == 10 && Settings.Current.CrossFadeActive);
            MenuItemCrossFadeUserDefined.IsChecked = (Settings.Current.CrossFadeDefaultSeconds != 0 &&
                Settings.Current.CrossFadeDefaultSeconds != 2 &&
                Settings.Current.CrossFadeDefaultSeconds != 5 &&
            Settings.Current.CrossFadeDefaultSeconds != 10) && Settings.Current.CrossFadeActive;
        }

        private void MenuItemCrossFade_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemCrossFadeOff)
            {
                if (Settings.Current.CrossFadeActive)
                    Settings.Current.CrossFadeActive = false;
                else
                    Settings.Current.CrossFadeActive = true;
            }

            if (sender == MenuItemCrossFade2Seconds)
            {
                Settings.Current.CrossFadeDefaultSeconds = 2;
                Settings.Current.CrossFadeActive = true;
            }
            if (sender == MenuItemCrossFade5Seconds)
            {
                Settings.Current.CrossFadeDefaultSeconds = 5;
                Settings.Current.CrossFadeActive = true;
            }
            if (sender == MenuItemCrossFade10Seconds)
            {
                Settings.Current.CrossFadeDefaultSeconds = 10;
                Settings.Current.CrossFadeActive = true;
            }
            if (sender == MenuItemCrossFadeUserDefined)
            {
                CrossFadeUserDefinedSecondsUserControl crossFadeUserDefinedSecondsUserControl = new CrossFadeUserDefinedSecondsUserControl();
                crossFadeUserDefinedSecondsUserControl.Seconds = Settings.Current.CrossFadeDefaultSeconds;
                GlobalServices.ModalService.NavigateTo(crossFadeUserDefinedSecondsUserControl, StringTable.UserDefinedCrossFade, delegate(bool returnValue)
                {
                    if (returnValue == true)
                    {
                        Settings.Current.CrossFadeDefaultSeconds = crossFadeUserDefinedSecondsUserControl.Seconds;
                        Settings.Current.CrossFadeActive = true;
                    }
                });

            }

            UpdateWindowState();

            e.Handled = true;
        }

        private void CommandBindingPlayInFullScreen2D_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPlaylist.Count > 0;
        }

        private void CommandBindingPlayInFullScreen3D_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPlaylist.Count > 0;
        }

        private void CommandBindingAboutHitbase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WindowAbout windowAbout = new WindowAbout();
            windowAbout.Owner = this;
            windowAbout.ShowDialog();
        }

        private void CommandBindingShowNormalizeOptions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NormalizeOptionsUserControl normalizeOptionsUserControl = new NormalizeOptionsUserControl();
            GlobalServices.ModalService.NavigateTo(normalizeOptionsUserControl, StringTable.NormalizeOptions, delegate(bool returnValue)
            {
                if (returnValue == true)
                {
                    UpdateWindowState();
                }
            });

        }

        private void CommandBindingOpenCDDrive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mainTreeUserControl.OpenFirstCDDrive();
        }

        private void CommandBindingFirstSteps_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddFirstStepsTab();
        }

        private void AddFirstStepsTab()
        {
            AddNewTab(StringTable.FirstSteps, "GreenFlag.png", new FirstStepsUserControl());
        }

        private void CommandBindingPlayAlbumNow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int cdid = (int)e.Parameter;

            AddCDToPlaylist(cdid, AddTracksToPlaylistType.Now);
        }

        private void AddCDToPlaylist(int cdid, AddTracksToPlaylistType addTracksType)
        {
            CD cd = DataBase.GetCDById(cdid);

            CurrentPlaylist.AddTracks(DataBase, cd.Tracks.ToArray(), addTracksType);
        }

        private void CommandBindingPlayAlbumNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int cdid = (int)e.Parameter;

            AddCDToPlaylist(cdid, AddTracksToPlaylistType.Next);
        }

        private void CommandBindingPlayAlbumLast_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int cdid = (int)e.Parameter;

            AddCDToPlaylist(cdid, AddTracksToPlaylistType.End);

        }

        private void CommandBindingCopyToClipboard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int[] recordIds;
            
            if (e.Parameter is int)
                recordIds = new int[] { (int)e.Parameter };
            else
                recordIds = (int[])e.Parameter;

            FormCopyToClipboard formCopyToClipboard = new FormCopyToClipboard(DataBase, recordIds, false);

            formCopyToClipboard.ShowDialog();
        }

        private void CommandBindingSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = DataBase.DataBasePath;
            saveFileDialog.DefaultExt = "hdbx";
            saveFileDialog.Filter = StringTable.FilterHitbase;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.AddExtension = true;
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(DataBase.DataBasePath);

            if (saveFileDialog.ShowDialog() == true)
            {
                string newName = saveFileDialog.FileName;
                string newNameFullPath = System.IO.Path.GetFullPath(newName);
                string catalogFullPath = System.IO.Path.GetFullPath(DataBase.DataBasePath);

                // Prüfen, ob man den Katalog unter gleichem Namen speichern will.
                if (string.Compare(newNameFullPath, catalogFullPath, true) == 0)
                {
                    MessageBox.Show(StringTable.CantSaveAsSameFilename, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                File.Copy(DataBase.DataBasePath, newNameFullPath, true);
            }
        }

        private void MenuItemRecentList_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string filename = (string)menuItem.Tag;

            if (!File.Exists(filename))
            {
                if (MessageBox.Show(string.Format(StringTable.CatalogNotFound, filename),
                    System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Settings.Current.RecentFileList.Remove(filename);
                    FillRecentFileList();
                }

                return;
            }


            OpenCatalog(filename);
        }

        private bool cdWasEjected = false;

        private void CommandBindingEjectCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDTreeItem mainCDTreeItem = e.Parameter as MainCDTreeItem;
            CDEngine cdEngine = null;

            if (mainCDTreeItem != null)
            {
                cdEngine = mainCDTreeItem.CDEngine;
            }

            if (e.Parameter == null)
            {
                MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

                if (mainCDUserControl != null)
                {
                    cdEngine = mainCDUserControl.CDEngine;
                }
            }

            if (cdEngine != null)
            {
                if (cdWasEjected)
                {
                    cdEngine.CloseTray();
                    cdWasEjected = false;
                }
                else
                {
                    cdEngine.OpenTray();
                    cdWasEjected = true;
                }
            }
        }

        private void CommandBindingEjectCD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainCDTreeItem mainCDTreeItem = e.Parameter as MainCDTreeItem;

            e.CanExecute = true;

            /*if (mainCDTreeItem != null)
            {
                e.CanExecute = mainCDTreeItem.CDEngine.IsCDInDrive;
            }

            if (e.Parameter == null)
            {
                MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

                if (mainCDUserControl != null)
                {
                    e.CanExecute = mainCDUserControl.CDEngine.IsCDInDrive;
                }
            }*/
        }

        private void CommandBindingPinTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem tabItem = e.Parameter as TabItem;
            TabItemModel tabItemModel = tabItem.DataContext as TabItemModel;
            tabItemModel.IsPinned = tabItemModel.IsPinned ? false : true;

            int index = tabControl.Items.IndexOf(tabItem);
            tabControl.Items.RemoveAt(index);
            if (tabItemModel.IsPinned)
            {
                int pinIndex = 0;
                foreach (TabItem item in tabControl.Items)
                {
                    TabItemModel itemModel = item.DataContext as TabItemModel;
                    if (!itemModel.IsPinned)
                    {
                        break;
                    }

                    pinIndex++;
                }
                tabControl.Items.Insert(pinIndex, tabItem);
                ((TabItem)tabItem).IsSelected = true;
            }
            else
            {
                tabControl.Items.Insert(tabControl.Items.Count, tabItem);
                ((TabItem)tabItem).IsSelected = true;
            }
        }

        private void CommandBindingCompactDataBase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SynchronizeCatalogWorker.Instance.IsRunning)
                return;

            DataBase.Close();
            DataBase.Compress();
            DataBase.Open(DataBase.DataBasePath);

            MessageBox.Show(StringTable.CompressCompleted, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CommandBindingAddAlbumToPlaylistNow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddAlbumToPlaylist((int)e.Parameter, AddTracksToPlaylistType.Now);
        }

        private void AddAlbumToPlaylist(int cdid, AddTracksToPlaylistType addTracksToPlaylistType)
        {
            CD cd = DataBase.GetCDById(cdid);
            if (cd != null)
            {
                CurrentPlaylist.AddTracks(this.DataBase, cd.Tracks.ToArray(), addTracksToPlaylistType);
            }

        }

        private void CommandBindingAddAlbumToPlaylistNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddAlbumToPlaylist((int)e.Parameter, AddTracksToPlaylistType.Next);
        }

        private void CommandBindingAddAlbumToPlaylistLast_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddAlbumToPlaylist((int)e.Parameter, AddTracksToPlaylistType.End);
        }

        private void CommandBindingShowVisualization_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            VisualEffectsUserControl visualEffectsUserControl = new VisualEffectsUserControl((string)e.Parameter);

            string imageSource = "";
            switch ((string)e.Parameter)
            {
                case "Plasma":
                    imageSource = "Plasma.png";
                    break;
                case "Fire":
                    imageSource = "Fire.png";
                    break;
            }

            AddNewTab("Visualisierung", imageSource, visualEffectsUserControl);
        }

        private void CommandBindingAddNewAlbumFromDirectory_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormAddFolder formAddFolder = new FormAddFolder();

            if (formAddFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FormAddFolderAction formAddFolderAction = new FormAddFolderAction(DataBase, formAddFolder.Folder);

                if (formAddFolderAction.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (formAddFolderAction.NewCD.Tracks.Count == 0)
                    {
                        MessageBox.Show(StringTable.NoTracksFound, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        formAddFolderAction.NewCD.NumberOfTracks = formAddFolderAction.NewCD.Tracks.Count;

                        WindowAlbum windowAlbum = new WindowAlbum(formAddFolderAction.NewCD, DataBase);
                        if (windowAlbum.ShowDialog() == true)
                        {
                            //formAddFolderAction.NewCD.Save(DataBase);
                        }
                    }
                }
            }
        }

        private void CommandBindingOpenHitbaseHomepage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("http://www.hitbase.de");
        }

        private void CommandBindingOpenHitbaseForum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("http://www.hitbase.de/forum");
        }

        private void CommandBindingOpenCDArchiveHomepage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("http://www.cdarchiv.de");
        }

        private void CommandBindingShowHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string helpPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\hitbase.pdf";
            Process.Start(helpPath);
        }

       
        private void SplitButtonCrossFade_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.CrossFadeActive)
                Settings.Current.CrossFadeActive = false;
            else
                Settings.Current.CrossFadeActive = true;

            UpdateWindowState();
        }

        private void CommandBindingCopyToExternalMedium_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPlaylist.Count > 0;
        }

        private void CommandBindingExportHTML_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Big3.Hitbase.NativeGUI.HTMLExportDialog htmlExportDlg = new NativeGUI.HTMLExportDialog();
            htmlExportDlg.Show(DataBase);
        }

        private void CommandBindingExportTXT_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Big3.Hitbase.NativeGUI.TXTExportDialog txtExportDlg = new NativeGUI.TXTExportDialog();
            txtExportDlg.Show(DataBase);
        }

        void DoAutoChecks()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                if (Settings.Current.AutoCheckNewVersion)
                {
                    AutoCheckNewVersion();
                }

                if (Settings.Current.AutoCheckAnnouncement)
                {
                    AutoCheckAnnouncement();
                }
            };
            bw.RunWorkerAsync();
        }

        void AutoCheckNewVersion()
        {
	        String fileVersion;
	
	        fileVersion = Misc.GetFileVersion(Misc.GetApplicationPath() + "\\hitbase.exe");

	        int iMajor, iMinor, iBuild;
	        String[] versionParts = fileVersion.Split('.');
	        iMajor = Convert.ToInt32(versionParts[0]);
	        iMinor = Convert.ToInt32(versionParts[1]);
	        iBuild = Convert.ToInt32(versionParts[2]);

	        // Ok, jetzt habe ich die Version von hitbase.exe
	        // Schauen wir dann mal im Internet nach, welche Version denn da aktuell ist.

	        String sInternetVersion;

	        WebClient wc = new WebClient();
            wc.UseDefaultCredentials = true;
            wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
            sInternetVersion = wc.DownloadString("http://" + Settings.Current.HitbaseServer + "//version.txt");

	        // Prüfen, ob die Datei vorhanden ist! Wenn die Datei nicht vorhanden ist,
	        // kommt trotzdem was zurück, da die meisten WWW-Server eine Fehlerseite
	        // erzeugen.

	        if (sInternetVersion.Left(7) == "VERSION")
	        {
		        sInternetVersion = sInternetVersion.Mid(sInternetVersion.IndexOf("\n")+1);

		        String sVersionDate = sInternetVersion.Mid(sInternetVersion.IndexOf("\n")+1);

		        int iInternetMajor, iInternetMinor, iInternetBuild;
		        iInternetMajor = Misc.Atoi(sInternetVersion.Left(sInternetVersion.IndexOf('.')));
		        iInternetMinor = Misc.Atoi(sInternetVersion.Mid(sInternetVersion.IndexOf('.')+1));
		        iInternetBuild = Misc.Atoi(sInternetVersion.Mid(sInternetVersion.LastIndexOf('.')+1));

		        if (iInternetMajor > iMajor ||
			        iInternetMajor == iMajor && iInternetMinor > iMinor ||
			        iInternetMajor == iMajor && iInternetMinor == iMinor && iInternetBuild > iBuild)
		        {
			        String str;
			        DateTime dt = new DateTime(Misc.Atoi(sVersionDate.Left(4)), Misc.Atoi(sVersionDate.Mid(4, 2)), Misc.Atoi(sVersionDate.Mid(6,2)));
			        String sDate;
			        sDate = dt.ToShortDateString();

			        str = string.Format(StringTable.NewVersionDetails, StringTable.NewVersionAvailable, iMajor, iMinor, iBuild, iInternetMajor, iInternetMinor, iInternetBuild, sDate);
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WPFMessageBox msgBox = new WPFMessageBox();
                        msgBox.HeaderText = StringTable.NewVersionAvailableTitle;
                        msgBox.Text = str;
                        msgBox.Image = "/Big3.Hitbase.SharedResources;component/Images/NewVersionAvailable.png";
                        msgBox.Owner = this;
                        msgBox.WpfMessageBoxButtons = WpfMessageBoxButtons.YesNo;
                        msgBox.ZoomImage = false;
                        msgBox.ShowInTaskbar = true;
                        msgBox.ShowDialog();

                        if (msgBox.WpfMessageBoxResult == WpfMessageBoxResult.Yes)
                        {
                            Process.Start(Settings.Current.HitbaseServer);
                        }
                    }));
                }
	        }
        }

        void AutoCheckAnnouncement()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                FormAnnouncements.CheckAnnouncement();
            }));
        }

        private void MainWnd_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                bool supportedFiletype = false;

                foreach (string fileLoc in filePaths)
                {
                    if (File.Exists(fileLoc))
                    {
                        string ext = System.IO.Path.GetExtension(fileLoc).ToLower();

                        if (ext == ".hvc" || ext == ".m3u")
                        {
                            CurrentPlaylist.LoadFromFile(fileLoc, true);
                            // Nur eine Playlist erlauben
                            supportedFiletype = true;
                            break;
                        }
                    }
                }

                if (supportedFiletype)
                    e.Handled = true;
            }
        }

        private void MainWnd_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                bool supportedFiletype = false;

                foreach (string fileLoc in filePaths)
                {
                    if (File.Exists(fileLoc))
                    {
                        string ext = System.IO.Path.GetExtension(fileLoc).ToLower();

                        if (ext == ".hvc" || ext == ".m3u")
                        {
                            supportedFiletype = true;
                            break;
                        }
                    }
                }

                if (supportedFiletype)
                {
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                }
/*                else
                {
                    e.Effects = DragDropEffects.None;
                }*/
                
            }
        }

        private void CommandBindingOrderHitbase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
	        Process.Start("http://www.hitbase.de/hitbase/2012/bestelle.htm");
        }

        private void CommandBindingActivateHitbase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string brandPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\brand.exe";
            
            int retValue = Misc.StartProcessAndWait(brandPath);

            if (retValue == 1)
            {
                if (Big3.Hitbase.Miscellaneous.Register.GetSerialActivation())
                {
                    buttonActivateHitbase.Visibility = System.Windows.Visibility.Collapsed;
                    buttonBuyHitbase.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void CommandBindingStartRecordSelectedTracks_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StartRecord(true);
        }

        private void CommandBindingStartRecordSelectedTracks_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = !MainCDUserControl.RecordEngine.RecordRunning && mainCDUserControl.GetSelectedTrackIndices().Length > 0;

            if (MainCDUserControl.RecordEngine.RecordRunning)
            {
                GroupBoxRecordRunning.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                GroupBoxRecordRunning.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void tabControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                TabItem ti = VisualTreeExtensions.FindParent<TabItem>(e.OriginalSource as DependencyObject);

                if (ti != null)
                {
                    CloseTab(ti);

                    e.Handled = true;
                }
            }
        }

        private void CommandBindingShowFrequencyBand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Current.ShowFrequencyBand)
            {
                Settings.Current.ShowFrequencyBand = false;
            }
            else
            {
                Settings.Current.ShowFrequencyBand = true;
            }

            UpdateWindowState();
            PlayerControl.UpdateWindowState();
        }

        private void CommandBindingShowLyrics_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Settings.Current.ShowLyrics)
            {
                Settings.Current.ShowLyrics = false;
            }
            else
            {
                Settings.Current.ShowLyrics = true;
            }

            UpdateWindowState();
            PlayerControl.UpdateWindowState();
        }

        private void MainWnd_Closed(object sender, EventArgs e)
        {
            mainTreeUserControl.CloseCDEngines();
            DataBase.Close();

            SoundEngine.SoundEngine.Instance.Uninitialize();
        }

        private void CommandBindingOpenInNewTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainCatalogTreeItem catalogTreeItem = e.Parameter as MainCatalogTreeItem;

            e.CanExecute = true;

            if (catalogTreeItem != null)
            {
                if (catalogTreeItem.ShowItemType == ShowItemType.Track)
                    e.CanExecute = false;
            }
        }

        private void CommandBindingShowPartyMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Browse3DWindow browse3DWindow = new Browse3DWindow(DataBase, this.CurrentPlaylist, this.Wishlist);
            browse3DWindow.Show();
        }

        private void CommandBindingPrintCatalog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FormPrintCatalog printCatalog = new FormPrintCatalog(DataBase);
            printCatalog.CurrentCD = false;
            printCatalog.ShowDialog(new NativeWindowWrapper(this));

        }

        private void CommandBindingSendEMail_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IntPtr windowHandle = (new NativeWindowWrapper(this)).Handle;
            Mapi mapi = new Mapi();
            mapi.Logon(windowHandle);
            mapi.Attach(DataBase.DataBasePath);
            // Mach ich hier im Hintergrund, da ansonsten das Application-Menü noch stehen bleibt
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                mapi.Send("", "");
            };
            bw.RunWorkerCompleted += delegate
            {
                mapi.Logoff();
            };

            bw.RunWorkerAsync();
        }

        private void CommandBindingShowOptions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowOptions();
        }

        private void CommandBindingNewCatalog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewCatalog();
        }

        private void CommandBindingExit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void LocalizeRibbon()
        {
            FieldInfo pi;

            pi = typeof(RibbonContextMenu).GetField("AddToQATText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, StringTable.RibbonContextAddToQATText);
            pi = typeof(RibbonContextMenu).GetField("RemoveFromQATText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, StringTable.RibbonContextRemoveFromQATText);
            pi = typeof(RibbonContextMenu).GetField("ShowQATAboveText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, StringTable.RibbonContextShowQATAboveText);
            pi = typeof(RibbonContextMenu).GetField("ShowQATBelowText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, StringTable.RibbonContextShowQATBelowText);
            pi = typeof(RibbonContextMenu).GetField("MaximizeTheRibbonText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, StringTable.RibbonContextMaximizeTheRibbonText);
            pi = typeof(RibbonContextMenu).GetField("MinimizeTheRibbonText", (BindingFlags.NonPublic | BindingFlags.Static));
            pi.SetValue(null, StringTable.RibbonContextMinimizeTheRibbonText);

        }

        private void CommandBindingPrintCDCover_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                CDCoverWindow cdCoverWindow = new CDCoverWindow(this.DataBase, mainCDUserControl.CD);

                cdCoverWindow.ShowDialog();
            }
        }

        private void CommandBindingDeleteCDTracksFromPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.CurrentPlaylist.IsPlaying && this.CurrentPlaylist.CurrentPlaylistItem != null && this.CurrentPlaylist.CurrentPlaylistItem.Info.Filename.StartsWith("cd:"))
                this.CurrentPlaylist.Stop();
            this.CurrentPlaylist.DeleteCDTracks(e.Parameter as string);
            this.CurrentPlaylist.Name = StringTable.UnsavedPlaylist;
        }

        private void CommandBindingSetRecordSpeed_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int newSpeed = Misc.Atoi((string)e.Parameter);
            Settings.Current.RecordSpeed = newSpeed;
            UpdateRecordSpeed();
        }

        private void CommandBindingPrintCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                FormPrintCatalog printCatalog = new FormPrintCatalog(DataBase, mainCDUserControl.CD);
                printCatalog.CurrentCD = false;
                printCatalog.ShowDialog(new NativeWindowWrapper(this));
            }
        }

        private void CommandBindingPrintCD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsCDInDriveForCurrentTab();
        }

        private void CommandBindingLinkCD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsCDInDriveForCurrentTab();
        }

        private bool IsCDInDriveForCurrentTab()
        {
            MainCDUserControl mainCDUserControl = GetActiveTabContent() as MainCDUserControl;

            if (mainCDUserControl != null)
            {
                return mainCDUserControl.CDEngine.IsCDInDrive;
            }
            else
            {
                return false;
            }
        }

        private void CommandBindingSaveCurrentCD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsCDInDriveForCurrentTab();
        }

        private void CommandBindingSearchCDInCDArchive_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsCDInDriveForCurrentTab();
        }

        private void CommandBindingSendCDToCDArchive_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsCDInDriveForCurrentTab() || e.Parameter is int;
        }

        private void CommandBindingAdjustSpelling_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsCDInDriveForCurrentTab();
        }
    }
}
