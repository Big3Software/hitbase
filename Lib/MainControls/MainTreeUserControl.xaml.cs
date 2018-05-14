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
using System.Collections.ObjectModel;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SharedResources;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using Big3.Hitbase.Controls;
using Big3.Hitbase.CDUtilities;

namespace Big3.Hitbase.MainControls
{
    public delegate void ItemClickedHandler(object sender, MainTreeItem selectedItem, bool openInNewTab);

    public enum PlaylistItemType
    {
        None,
        AllPlaylists,
        Playlist
    }

    /// <summary>
    /// Interaction logic for MainTreeUserControl.xaml
    /// </summary>
    public partial class MainTreeUserControl : UserControl
    {
        private MainTreeItemCollection mainTreeItemCollection = new MainTreeItemCollection();
        private MainPlaylistTreeItem allPlaylistItem = new MainPlaylistTreeItem();
        private MainSearchTreeItem personalSearches;
        MainCatalogTreeItem groupYearTreeItem = new MainCatalogTreeItem();
        MainCatalogTreeItem newItemMyMusic = new MainCatalogTreeItem();

        public event ItemClickedHandler ItemClicked;

        public DataBase DataBase { get; set; }

        public MainCatalogTreeItem MyMusicTreeItem { get { return newItemMyMusic; } }

        public MainTreeUserControl()
        {
            InitializeComponent();
        }

        public void FillTree()
        {
            AddPlayLists();

            AddMyMusic();

            AddCDDrives();

            RefreshPersonalSearches();

            TreeView.ItemsSource = mainTreeItemCollection;
        }

        private void AddPlayLists()
        {
            allPlaylistItem.Title = "Wiedergabelisten";
            allPlaylistItem.ImageResourceString = "Playlist.png";
            allPlaylistItem.ItemType = PlaylistItemType.AllPlaylists;
            allPlaylistItem.Margin = new Thickness(0, 5, 0, 0);

            UpdatePlaylists();

            allPlaylistItem.IsExpanded = true;
            mainTreeItemCollection.Add(allPlaylistItem);

            // Das Verzeichnis überwachen
            FileSystemWatcher fsw = new FileSystemWatcher(Misc.GetPersonalHitbasePlaylistFolder());
            fsw.Deleted += new FileSystemEventHandler(fsw_Deleted);
            fsw.Renamed += new RenamedEventHandler(fsw_Renamed);
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);
            fsw.EnableRaisingEvents = true;
        }

        void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                UpdatePlaylists();
            }));
        }

        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                UpdatePlaylists();
            }));
        }

        private void UpdatePlaylists()
        {
            string playlistPath = Misc.GetPersonalHitbasePlaylistFolder();
            allPlaylistItem.Items.Clear();
            if (Directory.Exists(playlistPath))
            {
                foreach (string filename in Directory.GetFiles(playlistPath))
                {
                    string fileExt = System.IO.Path.GetExtension(filename).ToLower();
                    if (fileExt == ".m3u" || fileExt == ".hvc")
                    {
                        MainPlaylistTreeItem playlist = new MainPlaylistTreeItem();
                        playlist.ImageResourceString = "Playlist.png";
                        playlist.Title = System.IO.Path.GetFileNameWithoutExtension(filename);
                        playlist.Filename = filename;
                        playlist.ItemType = PlaylistItemType.Playlist;
                        allPlaylistItem.Items.Add(playlist);
                    }
                }
            }
        }

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>  
            {
                UpdatePlaylists();    
            }));
        }

        private void AddMyMusic()
        {
            newItemMyMusic.Title = StringTable.MyMusic;
            newItemMyMusic.ImageResourceString = "Music.png";
            newItemMyMusic.ShowItemType = ShowItemType.MyMusic;
            newItemMyMusic.Margin = new Thickness(0, 5, 0, 0);
            //newItem1.Padding = new Thickness(0, 5, 0, 0);

            // Meine Musik -> Interpret
            MainCatalogTreeItem artistTreeItem = new MainCatalogTreeItem();
            artistTreeItem.ImageResourceString = "PersonGroupGeneral.png";
            artistTreeItem.Title = StringTable.Artist;
            artistTreeItem.ShowItemType = ShowItemType.ArtistAll;

            MainCatalogTreeItem artistCharTreeItem = new MainCatalogTreeItem();
            artistCharTreeItem.ImageResourceString = "PersonGroupGeneral.png";

            artistCharTreeItem.Title = StringTable.NoData;

            artistTreeItem.Items.Add(artistCharTreeItem);

            newItemMyMusic.Items.Add(artistTreeItem);

            // Meine Musik -> Album
            MainCatalogTreeItem albumTreeItem = new MainCatalogTreeItem();
            albumTreeItem.ImageResourceString = "CD.png";
            albumTreeItem.Title = StringTable.Album;
            albumTreeItem.ShowItemType = ShowItemType.AlbumAll;

            MainCatalogTreeItem albumCharTreeItem = new MainCatalogTreeItem();
            albumCharTreeItem.ImageResourceString = "CD.png";

            albumCharTreeItem.Title = StringTable.NoData;

            albumTreeItem.Items.Add(albumCharTreeItem);

            newItemMyMusic.Items.Add(albumTreeItem);

            // Meine Musik -> Komponist
            MainCatalogTreeItem composerTreeItem = new MainCatalogTreeItem();
            composerTreeItem.ImageResourceString = "PersonGroupGeneral.png";
            composerTreeItem.Title = StringTable.Composer;
            composerTreeItem.ShowItemType = ShowItemType.ComposerAll;

            MainCatalogTreeItem composerCharTreeItem = new MainCatalogTreeItem();
            composerCharTreeItem.ImageResourceString = "PersonGroupGeneral.png";

            composerCharTreeItem.Title = StringTable.NoData;

            composerTreeItem.Items.Add(composerCharTreeItem);

            newItemMyMusic.Items.Add(composerTreeItem);

            // Meine Musik -> Mitwirkender
            MainCatalogTreeItem participantTreeItem = new MainCatalogTreeItem();
            participantTreeItem.ImageResourceString = "PersonGroupGeneral.png";
            participantTreeItem.Title = StringTable.Participant;
            participantTreeItem.ShowItemType = ShowItemType.ParticipantAll;

            MainCatalogTreeItem participantCharTreeItem = new MainCatalogTreeItem();
            participantCharTreeItem.ImageResourceString = "PersonGroupGeneral.png";

            participantCharTreeItem.Title = StringTable.NoData;

            participantTreeItem.Items.Add(participantCharTreeItem);

            newItemMyMusic.Items.Add(participantTreeItem);


            // Genre
            MainCatalogTreeItem groupGenreTreeItem = new MainCatalogTreeItem();
            groupGenreTreeItem.ImageResourceString = "Category.png";
            groupGenreTreeItem.Title = "Genre";
            groupGenreTreeItem.ShowItemType = ShowItemType.Genre;

            MainCatalogTreeItem genreEmptyItem = new MainCatalogTreeItem();
            genreEmptyItem.ImageResourceString = "Category.png";

            genreEmptyItem.Title = StringTable.NoData;

            groupGenreTreeItem.Items.Add(genreEmptyItem);

            newItemMyMusic.Items.Add(groupGenreTreeItem);

            // Medium
            MainCatalogTreeItem groupMediumTreeItem = new MainCatalogTreeItem();
            groupMediumTreeItem.ImageResourceString = "Medium.png";
            groupMediumTreeItem.Title = StringTable.Medium;
            groupMediumTreeItem.ShowItemType = ShowItemType.Medium;

            MainCatalogTreeItem mediumEmptyItem = new MainCatalogTreeItem();
            mediumEmptyItem.ImageResourceString = "Category.png";

            mediumEmptyItem.Title = StringTable.NoData;

            groupMediumTreeItem.Items.Add(mediumEmptyItem);

            newItemMyMusic.Items.Add(groupMediumTreeItem);

            // Jahr
            groupYearTreeItem.Title = "Jahr";
            groupYearTreeItem.ShowItemType = ShowItemType.YearAll;
            groupYearTreeItem.ImageResourceString = "Calendar.png";

            // Dummy
            groupYearTreeItem.Items.Add(new MainTreeItem());

            newItemMyMusic.Items.Add(groupYearTreeItem);

            // Bewertung
            MainCatalogTreeItem groupRatingTreeItem = new MainCatalogTreeItem();
            groupRatingTreeItem.Title = "Bewertung";
            groupRatingTreeItem.ShowItemType = ShowItemType.Rating;
            groupRatingTreeItem.ImageResourceString = "Star.png";

            for (int i = 0; i < 7; i ++)
            {
                MainCatalogTreeItem groupRating2TreeItem = new MainCatalogTreeItem();
                if (i == 0)
                {
                    groupRating2TreeItem.Title = StringTable.Unrated;
                }
                else
                {
                    groupRating2TreeItem.Title = i.ToString();
                    if (i == 1)
                        groupRating2TreeItem.Title += " " + StringTable.Star;
                    else
                        groupRating2TreeItem.Title += " " + StringTable.Stars;
                }

                groupRating2TreeItem.FilterCondition = new DataBaseEngine.Condition();
                groupRating2TreeItem.FilterCondition.Add(new SingleCondition(Field.TrackRating, Operator.Equal, i));
                groupRating2TreeItem.ShowItemType = ShowItemType.Album;
                groupRating2TreeItem.ImageResourceString = "Star.png";
                
                groupRatingTreeItem.Items.Add(groupRating2TreeItem);
            }

            newItemMyMusic.Items.Add(groupRatingTreeItem);

            newItemMyMusic.IsExpanded = true;

            // Verzeichnisse
            MainCatalogTreeItem directoryTreeItem = new MainCatalogTreeItem();
            directoryTreeItem.Title = "Verzeichnisse";
            directoryTreeItem.ShowItemType = ShowItemType.DirectoriesAll;
            directoryTreeItem.ImageResourceString = "Folder.png";

            // Dummy
            directoryTreeItem.Items.Add(new MainTreeItem());

            newItemMyMusic.Items.Add(directoryTreeItem);


            // Statistiken
            MainStatisticsTreeItem statItem = new MainStatisticsTreeItem();
            statItem.Title = StringTable.Statistics;
            statItem.ImageResourceString = "Statistics.png";

           /* Array values = Enum.GetValues(typeof(StatisticType));
            foreach(StatisticType val in values )
            {
                string treesubName = string.Empty;
                switch (val)
	            {
                    case StatisticType.Overview:                         treesubName = StringTable.TEXT_OVERVIEW;     break;
                    case StatisticType.CDGroupByCategoryCount:           treesubName = StringTable.TEXT_STATISTICS_GRAF_1; break;
                    case StatisticType.CDGroupByMediumCount:             treesubName = StringTable.TEXT_STATISTICS_GRAF_2; break;
                    case StatisticType.CDGroupByPriceCount:              treesubName = StringTable.TEXT_STATISTICS_GRAF_3; break;
                    case StatisticType.CDGroupByNumberOfTracksCount:     treesubName = StringTable.TEXT_STATISTICS_GRAF_4; break;
                    case StatisticType.CDGroupByLengthCount:             treesubName = StringTable.TEXT_STATISTICS_GRAF_5; break;
                    case StatisticType.CDGroupBySamplerCount:            treesubName = StringTable.TEXT_STATISTICS_GRAF_6; break;
                    case StatisticType.CDGroupByRatingCount:             treesubName = StringTable.TEXT_STATISTICS_GRAF_7; break;
                    case StatisticType.CDGroupByAttributeCount:          treesubName = StringTable.TEXT_STATISTICS_GRAF_8; break;
                    case StatisticType.CDGroupByLabelCount:              treesubName = StringTable.TEXT_STATISTICS_GRAF_9; break;
                    case StatisticType.CDGroupByRecordCount:             treesubName = StringTable.TEXT_STATISTICS_GRAF_10; break;
                    case StatisticType.CDGroupByArtistArtCount:          treesubName = StringTable.TEXT_STATISTICS_GRAF_11; break;
                    case StatisticType.CDGroupByArtistSexCount:          treesubName = StringTable.TEXT_STATISTICS_GRAF_12; break;
                    case StatisticType.CDGroupByCountryCount:            treesubName = StringTable.TEXT_STATISTICS_GRAF_13; break;
                    case StatisticType.TrackGroupByLengthCount:          treesubName = StringTable.TEXT_STATISTICS_GRAF_14; break;
                    case StatisticType.TrackGroupByRecordCount:         treesubName = StringTable.TEXT_STATISTICS_GRAF_15; break;
                    case StatisticType.TrackGroupByRating:               treesubName = StringTable.TEXT_STATISTICS_GRAF_16; break;
                    case StatisticType.TrackGroupByBPMCount:             treesubName = StringTable.TEXT_STATISTICS_GRAF_17; break;
                    case StatisticType.TrackGroupByAttributeCount:       treesubName = StringTable.TEXT_STATISTICS_GRAF_18; break;
                    case StatisticType.ArtistTrackMost:                  treesubName = StringTable.TEXT_STATISTICS_GRAF_19; break;
                    case StatisticType.ArtistTrackGroupBySexCount:       treesubName = StringTable.TEXT_STATISTICS_GRAF_20; break;
                    case StatisticType.ArtistTrackGroupByCountryCount:   treesubName = StringTable.TEXT_STATISTICS_GRAF_21; break;
                    case StatisticType.ArtistTrackGroupByArtistArtCount: treesubName = StringTable.TEXT_STATISTICS_GRAF_22; break;
                    case StatisticType.ArtistCDsMost:                    treesubName = StringTable.TEXT_STATISTICS_GRAF_23; break;
                    case StatisticType.ArtistGroupBySexCount:            treesubName = StringTable.TEXT_STATISTICS_GRAF_24; break;
                    case StatisticType.ArtistGroupByCountryCount:        treesubName = StringTable.TEXT_STATISTICS_GRAF_25; break;
                    case StatisticType.ArtistGroupByArtistArtCount:      treesubName = StringTable.TEXT_STATISTICS_GRAF_26; break;

                    default:
                        throw new NotImplementedException();
	            }
            
                MainStatisticsTreeItem statisticTreeItemSub = new MainStatisticsTreeItem();
                statisticTreeItemSub.Title = treesubName;
                statisticTreeItemSub.Image = ImageLoader.FromResource("Statistics.png");

                statisticTreeItemSub.ShowItemType = val;

                statItem.Items.Add(statisticTreeItemSub);

            }
            */
            newItemMyMusic.Items.Add(statItem);

            mainTreeItemCollection.Add(newItemMyMusic);
        }

        private void FillYearRecorded()
        {
            groupYearTreeItem.Items.Clear();

            List<int> years = DataBase.GetAvailableTrackYears();
            Dictionary<int, MainCatalogTreeItem> decades = new Dictionary<int, MainCatalogTreeItem>();
            foreach (int year in years)
            {
                int decade = year / 10 * 10;

                if (year != 0 && !decades.ContainsKey(decade))
                {
                    MainCatalogTreeItem groupYear2TreeItem = new MainCatalogTreeItem();
                    groupYear2TreeItem.Title = decade.ToString();
                    groupYear2TreeItem.FilterCondition = new DataBaseEngine.Condition();
                    groupYear2TreeItem.FilterCondition.Add(new SingleCondition(Field.TrackYearRecorded, Operator.GreaterEqual, decade));
                    groupYear2TreeItem.FilterCondition.Add(new SingleCondition(Field.TrackYearRecorded, Operator.Less, decade + 10));
                    groupYear2TreeItem.ShowItemType = ShowItemType.YearDecade;
                    groupYear2TreeItem.ImageResourceString = "Calendar.png";
                    groupYearTreeItem.Items.Add(groupYear2TreeItem);

                    decades.Add(decade, groupYear2TreeItem);
                }

                MainCatalogTreeItem groupYear3TreeItem = new MainCatalogTreeItem();
                if (year == 0)
                    groupYear3TreeItem.Title = StringTable.Undefined;
                else
                    groupYear3TreeItem.Title = year.ToString();

                groupYear3TreeItem.FilterCondition = new DataBaseEngine.Condition();
                groupYear3TreeItem.FilterCondition.Add(new SingleCondition(Field.TrackYearRecorded, Operator.Equal, year, Logical.Or));
                groupYear3TreeItem.FilterCondition.Add(new SingleCondition(Field.YearRecorded, Operator.Equal, year));
                groupYear3TreeItem.ImageResourceString = "Calendar.png";
                groupYear3TreeItem.ShowItemType = ShowItemType.Album;
                if (year == 0)
                    groupYearTreeItem.Items.Add(groupYear3TreeItem);
                else
                    decades[decade].Items.Add(groupYear3TreeItem);
            }
        }

        public void CloseCDEngines()
        {
            foreach (MainTreeItem item in this.mainTreeItemCollection)
            {
                MainCDTreeItem cdTreeItem = item as MainCDTreeItem;

                if (cdTreeItem != null)
                {
                    cdTreeItem.CDEngine.Close();
                }
            }
        }

        private void AddCDDrives()
        {
            BackgroundWorker bw = new BackgroundWorker();
            //bw.DoWork += delegate
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    int index = 0;
                    foreach (DriveInfo drive in drives)
                    {
                        if (drive.DriveType == DriveType.CDRom)
                        {
                            string label;

                            if (drive.IsReady)
                                label = String.Format("{0} ({1})", drive.VolumeLabel, drive.RootDirectory.Name);
                            else
                                label = String.Format("<leer> ({0})", drive.RootDirectory.Name);

                            MainCDTreeItem cdDrive = new MainCDTreeItem();
                            cdDrive.Title = label;
                            cdDrive.ImageResourceString = "CD.png";
                            cdDrive.DriveLetter = drive.RootDirectory.Name[0];
                            cdDrive.CDEngine = new Big3.Hitbase.SoundEngine.CDEngine(cdDrive.DriveLetter);
                            cdDrive.Margin = new Thickness(0, 5, 0, 0);
                            cdDrive.CD = new CD();

                            mainTreeItemCollection.InsertItemFromThread(index++, cdDrive);
                        }
                    }
                }//;
            //bw.RunWorkerAsync();
        }

        public void RefreshPersonalSearches()
        {
            Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter sta = new DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter(this.DataBase);
            SearchDataSet.SearchDataTable searchTable = sta.GetData();

            if (searchTable.Count > 0)
            {
                CreateMainSearchTreeItem();

                personalSearches.Items.Clear();

                foreach (SearchDataSet.SearchRow row in searchTable)
                {
                    Big3.Hitbase.DataBaseEngine.Condition condition;

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Big3.Hitbase.DataBaseEngine.Condition));
                    StringReader sr = new StringReader(row.Condition);

                    condition = (Big3.Hitbase.DataBaseEngine.Condition)xmlSerializer.Deserialize(sr);

                    AddSearch(row.SearchID, row.Name, condition, row.Type);

                    sr.Close();
                }
            }
            else
            {
                if (personalSearches != null)
                {
                    personalSearches.Items.Clear();
                    mainTreeItemCollection.Remove(personalSearches);
                }
            }
        }

        private void CreateMainSearchTreeItem()
        {
            if (personalSearches != null)
                return;

            personalSearches = new MainSearchTreeItem();

            personalSearches.Title = StringTable.PersonalSearches;
            personalSearches.ImageResourceString = "Search.png";
            personalSearches.Margin = new Thickness(0, 5, 0, 0);
            personalSearches.IsExpanded = true;
            mainTreeItemCollection.Add(personalSearches);
        }

        private void AddSearch(int id, string name, Big3.Hitbase.DataBaseEngine.Condition condition, int type)
        {
            MainSearchTreeItem newSearch = new MainSearchTreeItem();
            newSearch.SearchId = id;
            newSearch.Condition = condition;
            newSearch.ViewMode = (CurrentViewMode)type;
            newSearch.Title = name;
            newSearch.IsEditable = true;
            newSearch.ImageResourceString = "Search.png";
            personalSearches.Items.Add(newSearch);
        }

        public void UpdateCDDrive(char letter, string title)
        {
            foreach (MainTreeItem item in this.mainTreeItemCollection)
            {
                MainCDTreeItem cdItem = item as MainCDTreeItem;
                if (cdItem != null && cdItem.DriveLetter == letter)
                {
                    cdItem.Title = title;
                }
            }
        }

        private void TreeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton toggleButton = VisualTreeExtensions.FindParent<ToggleButton>(e.OriginalSource as DependencyObject); 
            TreeViewItem item = VisualTreeExtensions.FindParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            MainTreeItem mainTreeItem = item != null  ? item.DataContext as MainTreeItem : null;
            if (mainTreeItem != null && toggleButton == null)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    if (ItemClicked != null)
                        ItemClicked(this, mainTreeItem, true);
                    item.Focus();
                    e.Handled = true;
                }

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (ItemClicked != null)
                        ItemClicked(this, mainTreeItem, false);
                    item.Focus();
                    e.Handled = true;
                }
            }
        }

        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;

            MainCatalogTreeItem catalogTreeItem = item.DataContext as MainCatalogTreeItem;
            if (catalogTreeItem != null)
            {
                switch (catalogTreeItem.ShowItemType)
                {
                    case ShowItemType.None:
                        break;
                    case ShowItemType.MyMusic:
                        break;
                    case ShowItemType.ArtistAll:
                        {
                            AddPersonGroupFirstChar(catalogTreeItem, false);
                            break;
                        }
                    case ShowItemType.ArtistFirstChar:
                        {
                            AddArtistsByFirstChar(catalogTreeItem, false);
                            break;
                        }
                    case ShowItemType.Artist:
                        {
                            AddAlbumsByPersonGroups(catalogTreeItem, false, true);
                            break;
                        }
                    case ShowItemType.Composer:
                        {
                            AddAlbumsByPersonGroups(catalogTreeItem, true, true);
                            break;
                        }
                    case ShowItemType.ComposerAll:
                        {
                            AddPersonGroupFirstChar(catalogTreeItem, true);
                            break;
                        }
                    case ShowItemType.ComposerFirstChar:
                        {
                            AddArtistsByFirstChar(catalogTreeItem, true);
                            break;
                        }
                    case ShowItemType.AlbumAll:
                        {
                            AddAlbumPersonGroupFirstChar(catalogTreeItem, false);
                            break;
                        }
                    case ShowItemType.AlbumArtistFirstChar:
                        {
                            AddAlbumPersonGroupByFirstChar(catalogTreeItem, false);

                            break;
                        }
                    case ShowItemType.Album:
                        {
                            AddTracksByCD(catalogTreeItem);
                            break;
                        }
                    case ShowItemType.AlbumArtist:
                        {
                            AddAlbumsByPersonGroups(catalogTreeItem, false, false);
                            break;
                        }
                    case ShowItemType.YearAll:
                        FillYearRecorded();
                        break;
                    case ShowItemType.ParticipantAll:
                        {
                            AddParticipantFirstChar(catalogTreeItem);
                            break;
                        }
                    case ShowItemType.ParticipantFirstChar:
                        {
                            AddParticipantsByFirstChar(catalogTreeItem);
                            break;
                        }
                    case ShowItemType.Participant:
                        {
                            AddAlbumsByParticipant(catalogTreeItem);
                            break;
                        }
                    case ShowItemType.Genre:
                        {
                            AddGenres(catalogTreeItem);
                            break;
                        }
                    case ShowItemType.Medium:
                        {
                            AddMediums(catalogTreeItem);
                            break;
                        }
                    case ShowItemType.DirectoriesAll:
                    case ShowItemType.Directory:
                        {
                            AddDirectories(catalogTreeItem);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private void AddMediums(MainCatalogTreeItem catalogTreeItem)
        {
            catalogTreeItem.Items.Clear();

            foreach (Medium medium in DataBase.AllMediums)
            {
                MainCatalogTreeItem mediumTreeItem = new MainCatalogTreeItem();
                mediumTreeItem.ImageResourceString = "Medium.png";
                mediumTreeItem.Title = medium.Name;
                mediumTreeItem.ShowItemType = ShowItemType.Album;

                mediumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                mediumTreeItem.FilterCondition.Add(new SingleCondition(Field.Medium, Operator.Equal, medium.Name));

                catalogTreeItem.Items.Add(mediumTreeItem);
            }

        }

        private void AddGenres(MainCatalogTreeItem catalogTreeItem)
        {
            catalogTreeItem.Items.Clear();

            foreach (Category category in DataBase.GetAvailableCategories())
            {
                MainCatalogTreeItem groupCategoryTreeItem = new MainCatalogTreeItem();
                groupCategoryTreeItem.ImageResourceString = "Category.png";
                groupCategoryTreeItem.Title = category.Name;
                groupCategoryTreeItem.ShowItemType = ShowItemType.Album;

                groupCategoryTreeItem.FilterCondition = new DataBaseEngine.Condition();
                groupCategoryTreeItem.FilterCondition.Add(new SingleCondition(Field.Category, Operator.Equal, category.Name, Logical.Or));
                groupCategoryTreeItem.FilterCondition.Add(new SingleCondition(Field.TrackCategory, Operator.Equal, category.Name));

                catalogTreeItem.Items.Add(groupCategoryTreeItem);
            }

        }

        private void AddDirectories(MainCatalogTreeItem catalogTreeItem)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                string parentPath = catalogTreeItem.Tag as string;

                if (parentPath == null)
                    parentPath = "";

                List<string> subDirs = DataBase.GetAllMusicFilesSubDirectories(parentPath);

                int index = 0;
                foreach (string subDir in subDirs)
                {
                    MainCatalogTreeItem subDirTreeItem = new MainCatalogTreeItem();

                    subDirTreeItem.ImageResourceString = "Folder.png";

                    subDirTreeItem.ShowItemType = ShowItemType.Directory;
                    subDirTreeItem.Title = subDir;
                    subDirTreeItem.Tag = parentPath + subDir + "\\";

                    subDirTreeItem.FilterCondition = new DataBaseEngine.Condition();
                    subDirTreeItem.FilterCondition.Add(new SingleCondition(Field.TrackSoundFile, Operator.StartsWith, parentPath + subDir + "\\"));

                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    subDirTreeItem.Items.AddItemFromThread(new MainTreeItem());

                    catalogTreeItem.Items.AddItemFromThread(subDirTreeItem);


                    index++;
                }
            };

            bw.RunWorkerAsync();

        }

        private string GetParentPath(MainCatalogTreeItem catalogTreeItem)
        {
            string parentPath = "";

            while (catalogTreeItem.ShowItemType == ShowItemType.Directory)
            {
                parentPath = catalogTreeItem.Title + "\\" + parentPath;
            }

            return parentPath;
        }

        private void AddTracksByCD(MainCatalogTreeItem catalogTreeItem)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                CD cd = DataBase.GetCDById(catalogTreeItem.Id);

                int index = 0;
                foreach (Big3.Hitbase.DataBaseEngine.Track track in cd.Tracks)
                {
                    MainCatalogTreeItem trackTreeItem = new MainCatalogTreeItem();

                    trackTreeItem.ImageResourceString = this.GetImageBySoundfile(track.Soundfile);

                    trackTreeItem.ShowItemType = ShowItemType.Track;
                    trackTreeItem.Title = GetTrackTitleText(cd, index);

                    catalogTreeItem.Items.AddItemFromThread(trackTreeItem);

                    index++;
                }
            };

            bw.RunWorkerAsync();
   
        }

        private string GetTrackTitleText(CD cd, int index)
        {
            string text = "";

            if (cd.Sampler)
            {
                text = string.Format("{0}. {1} - {2}", cd.Tracks[index].TrackNumber, cd.Tracks[index].Artist, cd.Tracks[index].Title);
            }
            else
            {
                text = string.Format("{0}. {1}", cd.Tracks[index].TrackNumber, cd.Tracks[index].Title);
            }

            if (cd.Tracks[index].Length > 0)
                text += string.Format(" [{0}]", Misc.GetShortTimeString(cd.Tracks[index].Length));

            return text;
        }

        private string GetImageBySoundfile(string soundfile)
        {
            if (string.IsNullOrEmpty(soundfile))
                return "TrackIcon.png";
            else
                if (File.Exists(soundfile))
                    return "TrackSoundFoundIcon.png";
                else
                    return "TrackSoundNotFoundIcon.png";
        }

        private void AddAlbumsByPersonGroups(MainCatalogTreeItem catalogTreeItem, bool composer, bool withTracks)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                int personGroupId = DataBase.GetPersonGroupByName(catalogTreeItem.Title, false).ID;
                List<AlbumCDSetModel> albums = DataBase.GetAllAlbumsAndCDSetsByPersonGroup(personGroupId, composer, withTracks);

                foreach (AlbumCDSetModel album in albums)
                {
                    MainCatalogTreeItem albumTreeItem = new MainCatalogTreeItem();
                    
                    if (album.IsCDSet)
                    {
                        albumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                        albumTreeItem.FilterCondition.Add(new SingleCondition(Field.CDSet, Operator.Equal, album.Title));
                        albumTreeItem.Id = 0;

                        albumTreeItem.ImageResourceString = "CDSetIcon.png";
                        albumTreeItem.ShowItemType = ShowItemType.CDSet;
                        foreach (AlbumCDSetModel cdSetAlbum in album.CDSetAlbums.OrderBy(x => x.CDSetNumber))
                        {
                            MainCatalogTreeItem cdSetAlbumTreeItem = new MainCatalogTreeItem();
                            cdSetAlbumTreeItem.ImageResourceString = GetImageByMediumAndType(cdSetAlbum.Medium, cdSetAlbum.AlbumType);

                            cdSetAlbumTreeItem.ShowItemType = ShowItemType.Album;
                            cdSetAlbumTreeItem.Title = cdSetAlbum.Title;
                            cdSetAlbumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                            cdSetAlbumTreeItem.Id = cdSetAlbum.CDID;
                            cdSetAlbumTreeItem.FilterCondition.Add(new SingleCondition(Field.CDID, Operator.Equal, cdSetAlbum.CDID));

                            // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                            cdSetAlbumTreeItem.Items.Add(new MainTreeItem());
                            albumTreeItem.Items.AddItemFromThread(cdSetAlbumTreeItem);

                        }
                    }
                    else
                    {
                        albumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                        albumTreeItem.FilterCondition.Add(new SingleCondition(Field.CDID, Operator.Equal, album.CDID));

                        albumTreeItem.Id = album.CDID;

                        albumTreeItem.ShowItemType = ShowItemType.Album;
                        albumTreeItem.ImageResourceString = GetImageByMediumAndType(album.Medium, album.AlbumType);
                        // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                        albumTreeItem.Items.Add(new MainTreeItem());
                    }

                    albumTreeItem.Title = album.Title;

                    catalogTreeItem.Items.AddItemFromThread(albumTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }

        private void AddAlbumsByParticipant(MainCatalogTreeItem catalogTreeItem)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                int personGroupId = DataBase.GetPersonGroupByName(catalogTreeItem.Title, false).ID;
                List<AlbumCDSetModel> albums = DataBase.GetAllAlbumsAndCDSetsByParticipant(personGroupId);

                foreach (AlbumCDSetModel album in albums)
                {
                    MainCatalogTreeItem albumTreeItem = new MainCatalogTreeItem();

                    if (album.IsCDSet)
                    {
                        albumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                        albumTreeItem.FilterCondition.Add(new SingleCondition(Field.CDSet, Operator.Equal, album.Title));
                        albumTreeItem.Id = 0;

                        albumTreeItem.ImageResourceString = "CDSetIcon.png";
                        albumTreeItem.ShowItemType = ShowItemType.CDSet;
                        foreach (AlbumCDSetModel cdSetAlbum in album.CDSetAlbums.OrderBy(x => x.CDSetNumber))
                        {
                            MainCatalogTreeItem cdSetAlbumTreeItem = new MainCatalogTreeItem();
                            cdSetAlbumTreeItem.ImageResourceString = GetImageByMediumAndType(cdSetAlbum.Medium, cdSetAlbum.AlbumType);

                            cdSetAlbumTreeItem.ShowItemType = ShowItemType.Album;
                            cdSetAlbumTreeItem.Title = cdSetAlbum.Title;
                            cdSetAlbumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                            cdSetAlbumTreeItem.Id = cdSetAlbum.CDID;
                            cdSetAlbumTreeItem.FilterCondition.Add(new SingleCondition(Field.CDID, Operator.Equal, cdSetAlbum.CDID));

                            // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                            cdSetAlbumTreeItem.Items.Add(new MainTreeItem());
                            albumTreeItem.Items.AddItemFromThread(cdSetAlbumTreeItem);

                        }
                    }
                    else
                    {
                        albumTreeItem.FilterCondition = new DataBaseEngine.Condition();
                        albumTreeItem.FilterCondition.Add(new SingleCondition(Field.CDID, Operator.Equal, album.CDID));

                        albumTreeItem.Id = album.CDID;

                        albumTreeItem.ShowItemType = ShowItemType.Album;
                        albumTreeItem.ImageResourceString = GetImageByMediumAndType(album.Medium, album.AlbumType);
                        // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                        albumTreeItem.Items.Add(new MainTreeItem());
                    }

                    albumTreeItem.Title = album.Title;

                    catalogTreeItem.Items.AddItemFromThread(albumTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }


        private string GetImageByMediumAndType(string medium, AlbumType albumType)
        {
            if (albumType == AlbumType.MusicDataCD || albumType == AlbumType.SoundFiles || albumType == AlbumType.ManagedSoundFiles)
                return "MP3CDIcon.png";

            return GetImageByMedium(medium);
        }

        private void AddAlbumPersonGroupFirstChar(MainCatalogTreeItem catalogTreeItem, bool composer)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                List<string> chars = DataBase.GetAlbumAllPersonGroupFirstChar(composer);

                foreach (string firstChar in chars)
                {
                    MainCatalogTreeItem groupCharTreeItem = new MainCatalogTreeItem();
                    groupCharTreeItem.ImageResourceString = "PersonGroupGeneral.png";
                    if (composer)
                        groupCharTreeItem.ShowItemType = ShowItemType.AlbumComposerFirstChar;
                    else
                        groupCharTreeItem.ShowItemType = ShowItemType.AlbumArtistFirstChar;
                    groupCharTreeItem.Title = firstChar;
                    groupCharTreeItem.FilterCondition = new DataBaseEngine.Condition();
                    if (firstChar.Length > 1)
                    {
                        // Andere
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.Less, "A", Logical.Or));
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.Greater, "ZZZZ"));
                    }
                    else
                    {
                        //groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.StartsWith, firstChar));
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.GreaterEqual, firstChar, Logical.And));
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.LessEqual, firstChar + "ZZZZZ"));
                    }

                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    groupCharTreeItem.Items.Add(new MainTreeItem());
                    catalogTreeItem.Items.AddItemFromThread(groupCharTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }

        private void AddAlbumPersonGroupByFirstChar(MainCatalogTreeItem catalogTreeItem, bool composer)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                List<PersonGroupModel> personGroups = null;

                if (catalogTreeItem.Title.Length > 1) // Dann ist das wohl "Andere"
                    personGroups = DataBase.GetAlbumAllPersonGroupsByFirstChar('?', composer);
                else
                    personGroups = DataBase.GetAlbumAllPersonGroupsByFirstChar(catalogTreeItem.Title[0], composer);

                foreach (PersonGroupModel personGroup in personGroups)
                {
                    MainCatalogTreeItem groupCharTreeItem = new MainCatalogTreeItem();
                    groupCharTreeItem.ImageResourceString = GetImageByPersonGroup(personGroup.Sex, personGroup.Type);
                    if (composer)
                        groupCharTreeItem.ShowItemType = ShowItemType.AlbumComposer;
                    else
                        groupCharTreeItem.ShowItemType = ShowItemType.AlbumArtist;
                    groupCharTreeItem.Title = personGroup.Name;
                    groupCharTreeItem.FilterCondition = new DataBaseEngine.Condition();
                    groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDName, Operator.Equal, personGroup.Name));

                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    groupCharTreeItem.Items.Add(new MainTreeItem());
                    catalogTreeItem.Items.AddItemFromThread(groupCharTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }


        private void AddPersonGroupFirstChar(MainCatalogTreeItem catalogTreeItem, bool composer)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                List<string> chars = DataBase.GetAllPersonGroupFirstChar(composer);

                foreach (string firstChar in chars)
                {
                    MainCatalogTreeItem groupCharTreeItem = new MainCatalogTreeItem();
                    groupCharTreeItem.ImageResourceString = "PersonGroupGeneral.png";
                    if (composer)
                        groupCharTreeItem.ShowItemType = ShowItemType.ComposerFirstChar;
                    else
                        groupCharTreeItem.ShowItemType = ShowItemType.ArtistFirstChar;
                    groupCharTreeItem.Title = firstChar;
                    groupCharTreeItem.FilterCondition = new DataBaseEngine.Condition();

                    if (firstChar.Length > 1)
                    {
                        // Andere
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.Equal, "?"));
                    }
                    else
                    {
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.StartsWith, firstChar));
                    }

                    //groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.StartsWith, firstChar));
                    
                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    groupCharTreeItem.Items.Add(new MainTreeItem());
                    catalogTreeItem.Items.AddItemFromThread(groupCharTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }

        private void AddArtistsByFirstChar(MainCatalogTreeItem catalogTreeItem, bool composer)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                List<PersonGroupModel> personGroups = null;

                if (catalogTreeItem.Title.Length > 1)
                    personGroups = DataBase.GetAllPersonGroupsByFirstChar('?', composer);
                else
                    personGroups = DataBase.GetAllPersonGroupsByFirstChar(catalogTreeItem.Title[0], composer);

                foreach (PersonGroupModel personGroup in personGroups)
                {
                    MainCatalogTreeItem groupCharTreeItem = new MainCatalogTreeItem();
                    groupCharTreeItem.ImageResourceString = GetImageByPersonGroup(personGroup.Sex, personGroup.Type);
                    if (composer)
                        groupCharTreeItem.ShowItemType = ShowItemType.Composer;
                    else
                        groupCharTreeItem.ShowItemType = ShowItemType.Artist;
                    groupCharTreeItem.Title = personGroup.Name;
                    groupCharTreeItem.FilterCondition = new DataBaseEngine.Condition();
                    if (composer)
                    {
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ComposerTrackName, Operator.Equal, personGroup.Name, Logical.Or));
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ComposerCDName, Operator.Equal, personGroup.Name));
                    }
                    else
                    {
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistTrackName, Operator.Equal, personGroup.Name, Logical.Or));
                        groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDName, Operator.Equal, personGroup.Name));
                    }

                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    groupCharTreeItem.Items.Add(new MainTreeItem());
                    catalogTreeItem.Items.AddItemFromThread(groupCharTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }

        private void AddParticipantFirstChar(MainCatalogTreeItem catalogTreeItem)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                List<string> chars = DataBase.GetAllParticipantFirstChar();

                foreach (string firstChar in chars)
                {
                    MainCatalogTreeItem groupCharTreeItem = new MainCatalogTreeItem();
                    groupCharTreeItem.ImageResourceString = "PersonGroupGeneral.png";
                    groupCharTreeItem.ShowItemType = ShowItemType.ParticipantFirstChar;
                    groupCharTreeItem.Title = firstChar;
                    groupCharTreeItem.FilterCondition = new DataBaseEngine.Condition();
                    groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.StartsWith, firstChar));

                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    groupCharTreeItem.Items.Add(new MainTreeItem());
                    catalogTreeItem.Items.AddItemFromThread(groupCharTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }

        private void AddParticipantsByFirstChar(MainCatalogTreeItem catalogTreeItem)
        {
            BackgroundWorker bw = new BackgroundWorker();
            catalogTreeItem.Items.Clear();

            bw.DoWork += delegate
            {
                List<PersonGroupModel> personGroups = null;

                if (catalogTreeItem.Title.Length > 1) // Dann ist das wohl "Andere"
                    personGroups = DataBase.GetParticipantsByFirstChar('?');
                else
                    personGroups = DataBase.GetParticipantsByFirstChar(catalogTreeItem.Title[0]);

                foreach (PersonGroupModel personGroup in personGroups)
                {
                    MainCatalogTreeItem groupCharTreeItem = new MainCatalogTreeItem();
                    groupCharTreeItem.ImageResourceString = GetImageByPersonGroup(personGroup.Sex, personGroup.Type);
                    groupCharTreeItem.ShowItemType = ShowItemType.Participant;
                    groupCharTreeItem.Title = personGroup.Name;
                    groupCharTreeItem.FilterCondition = new DataBaseEngine.Condition();
                    groupCharTreeItem.FilterCondition.Add(new SingleCondition(Field.ArtistCDSaveAs, Operator.StartsWith, personGroup.Name));

                    // Dummy-Element, damit der Pfeil zum aufklappen angezeigt wird.
                    groupCharTreeItem.Items.Add(new MainTreeItem());
                    catalogTreeItem.Items.AddItemFromThread(groupCharTreeItem);
                }
            };

            bw.RunWorkerAsync();
        }


        private string GetImageByPersonGroup(SexType sexType, PersonGroupType personGroupType)
        {
            switch (sexType)
            {
                case SexType.Feminin:
                    {
                        return personGroupType == PersonGroupType.Single ? "PersonGroupFeminin.png" : "PersonGroupFemininGroup.png";
                    }
                case SexType.Masculin:
                    {
                        return personGroupType == PersonGroupType.Single ? "PersonGroupMasculin.png" : "PersonGroupMasculinGroup.png";
                    }
                default:
                    return "PersonGroupGeneral.png";
            }
        }

        private string GetImageByMedium(string medium)
        {
            //if (mp3cd)
            //    return ImageLoader.FromResource("MP3Icon.png");

            switch (medium.ToUpper())
            {
                case "LP":
                    return "LP.png";
                case "DVD":
                    return "DVDIcon.png";
                case "MC":
                    return "MCIcon.png";
            }

            return "CD.png";
        }

        private void DeletePlaylistCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainPlaylistTreeItem treeItem = e.Parameter as MainPlaylistTreeItem;
            string msg = string.Format(StringTable.ConfirmDeletePlaylist, treeItem.Title);

            if (MessageBox.Show(msg, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                File.Delete(treeItem.Filename);
            }
        }

        private void RenameSearchCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainSearchTreeItem searchTreeItem = e.Parameter as MainSearchTreeItem;

            TreeViewItem treeViewItem = TreeView.ContainerFromItem(searchTreeItem);
            if (treeViewItem != null)
            {
                EditableTextBlock editableTextBlock = VisualTreeExtensions.FindVisualChild<EditableTextBlock>(treeViewItem);
                editableTextBlock.IsInEditMode = true;
                editableTextBlock.EditFinished += new EventHandler(editableTextBlock_EditFinished);
            }
        }

        void editableTextBlock_EditFinished(object sender, EventArgs e)
        {
            TextBox editTextBox = ((RoutedEventArgs)e).OriginalSource as TextBox;
            EditableTextBlock editableTextBlock = sender as EditableTextBlock;

            editableTextBlock.EditFinished -= new EventHandler(editableTextBlock_EditFinished);

            MainSearchTreeItem mainSearchTreeItem = editableTextBlock.DataContext as MainSearchTreeItem;

            int existId = this.DataBase.GetSearchIdByName(editableTextBlock.Text);
            if (existId != mainSearchTreeItem.SearchId && existId > 0)
            {
                editableTextBlock.Text = mainSearchTreeItem.Title;
                return;
            }

            mainSearchTreeItem.Title = editableTextBlock.Text;

            this.DataBase.UpdateSearch(mainSearchTreeItem.SearchId, editableTextBlock.Text, mainSearchTreeItem.Condition, (int)mainSearchTreeItem.ViewMode);
        }

        private void DeleteSearchCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainSearchTreeItem searchTreeItem = e.Parameter as MainSearchTreeItem;

            this.DataBase.DeleteSearch(searchTreeItem.SearchId);

            this.RefreshPersonalSearches();
        }

        private void TreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            MainTreeItem mainTreeItem = item != null ? item.DataContext as MainTreeItem : null;

            if (mainTreeItem != null)
            {
                if (e.Key == Key.Space || e.Key == Key.Return)
                {
                    if (ItemClicked != null)
                        ItemClicked(this, mainTreeItem, false);

                    e.Handled = true;
                }
            }

        }

        public void EditPersonalSearch(string name)
        {
            foreach (MainSearchTreeItem searchItem in this.personalSearches.Items)
            {
                if (searchItem.Title == name)
                {
                    HitbaseCommands.RenameSearch.Execute(searchItem, this);
                    break;
                }
            }
        }

        public void OpenFirstCDDrive()
        {
            foreach (MainTreeItem treeItem in mainTreeItemCollection)
            {
                MainCDTreeItem mainCDTreeItem = treeItem as MainCDTreeItem;
                if (treeItem != null)
                {
                    this.ItemClicked(this, treeItem, true);
                    break;
                }
            }
        }

        private void RenameSearchCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainSearchTreeItem mainSearchTreeItem = e.Parameter as MainSearchTreeItem;

            e.CanExecute = false;

            if (mainSearchTreeItem != null)
            {
                e.CanExecute = mainSearchTreeItem.SearchId > 0;
            }
        }

        private void DeleteSearchCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainSearchTreeItem mainSearchTreeItem = e.Parameter as MainSearchTreeItem;

            e.CanExecute = false;

            if (mainSearchTreeItem != null)
            {
                e.CanExecute = mainSearchTreeItem.SearchId > 0;
            }
        }

        private void PersonGroupPropertiesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCatalogTreeItem mainCatalogTreeItem = e.Parameter as MainCatalogTreeItem;

            if (mainCatalogTreeItem != null)
            {
                PersonGroup personGroup = DataBase.GetPersonGroupByName(mainCatalogTreeItem.Title, false);
                PersonGroupWindow personGroupWindow = new PersonGroupWindow(DataBase, PersonType.Unknown, personGroup);
                personGroupWindow.Owner = Window.GetWindow(this);
                personGroupWindow.ShowDialog();
            }
        }

        private void PersonGroupPropertiesCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainCatalogTreeItem mainCatalogTreeItem = e.Parameter as MainCatalogTreeItem;

            e.CanExecute = false;

            if (mainCatalogTreeItem != null)
            {
                e.CanExecute = true;
            }
        }

        private void CorrectTrackNumbersCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainCatalogTreeItem mainCatalogTreeItem = e.Parameter as MainCatalogTreeItem;

            WaitProgressUserControl waitProgressUserControl = new WaitProgressUserControl();
            GlobalServices.ModalService.NavigateTo(waitProgressUserControl, StringTable.CorrectTrackNumbers, delegate(bool returnValue)
            {
                if (returnValue == true)
                {

                }
            }, false);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                int count = CountCorrectTrackNumbers(mainCatalogTreeItem.Tag as string);

                this.Dispatcher.Invoke((Action)(() =>
                    {
                        waitProgressUserControl.progressBar.Maximum = count;
                    }));
                
                CorrectTrackNumbers(mainCatalogTreeItem.Tag as string, waitProgressUserControl);
            };
            bw.RunWorkerCompleted += delegate(object completedSender, RunWorkerCompletedEventArgs completedEventArgs)
            {
                if (completedEventArgs.Error != null)
                {
                    UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(completedEventArgs.Error);
                    unhandledExceptionWindow.ShowDialog();
                }

                GlobalServices.ModalService.CloseModal();
            };
            bw.RunWorkerAsync();
        }

        private int CountCorrectTrackNumbers(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.mp3");

            int count = 0;
            foreach (string file in files)
            {
                count++;
            }

            string[] subDirs = Directory.GetDirectories(directory);
            foreach (string subDir in subDirs)
            {
                count += CountCorrectTrackNumbers(subDir);
            }

            return count;
        }

        private void CorrectTrackNumbers(string directory, WaitProgressUserControl waitProgressUserControl)
        {
            if (waitProgressUserControl.Canceled)
            {
                return;
            }

            string[] files = Directory.GetFiles(directory, "*.mp3");

            int trackIndex = 1;
            foreach (string file in files.OrderBy(x => x))
            {
                Big3.Hitbase.SoundEngine.SoundFileInformation sfi = Big3.Hitbase.SoundEngine.SoundFileInformation.GetSoundFileInformation(file);

                int trackNumber = sfi.TrackNumber;

                sfi.TrackNumber = trackIndex;

                Big3.Hitbase.SoundEngine.SoundFileInformation.WriteMP3Tags(sfi, Field.TrackNumber);

                DataBase.UpdateTrackNumber(file, trackIndex);
                
                trackIndex++;

                this.Dispatcher.Invoke((Action)(() =>
                {
                    waitProgressUserControl.textBlockStatus.Text = file;
                    waitProgressUserControl.textBlockStatus.ToolTip = file;
                    waitProgressUserControl.progressBar.Value++;
                }));

                if (waitProgressUserControl.Canceled)
                {
                    return;
                }
            }

            string[] subDirs = Directory.GetDirectories(directory);
            foreach (string subDir in subDirs)
            {
                CorrectTrackNumbers(subDir, waitProgressUserControl);
            }
        }

        public MainCDTreeItem GetCDTreeItemDriveLetter(char driveLetter)
        {
            foreach (MainTreeItem item in this.mainTreeItemCollection)
            {
                MainCDTreeItem cdTreeItem = item as MainCDTreeItem;

                if (cdTreeItem != null && Char.ToLower(cdTreeItem.DriveLetter) == Char.ToLower(driveLetter))
                {
                    return cdTreeItem;
                }
            }

            return null;
        }
    }

    public class MainTreeItem : INotifyPropertyChanged
    {
        public MainTreeItem()
        {
            Items = new MainTreeItemCollection();
        }

        private string title;

        public string Title
        {
            get { return title; }
            set 
            { 
                title = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
        }

        public string ImageResourceString { get; set; }

        public MainTreeItemCollection Items { get; private set; }

        private Thickness padding = new Thickness(0, 1, 0, 1);
        public Thickness Padding 
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;
            }
        }

        public Thickness Margin { get; set; }

        public virtual List<MenuItem> ContextMenuItems
        {
            get
            {
                List<MenuItem> contextMenuItems = new List<MenuItem>();

                contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.OpenInNewTab.Text, Command = HitbaseCommands.OpenInNewTab, CommandParameter = this, Icon = ImageLoader.CreateImageFromResource("NewTab.png") });

                return contextMenuItems;
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
                isExpanded = true;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }

        private bool isEditable = false;
        public bool IsEditable
        {
            get
            {
                return isEditable;
            }
            set
            {
                isEditable = value;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class MainPlaylistTreeItem : MainTreeItem
    {
        public PlaylistItemType ItemType { get; set; }

        /// <summary>
        /// Der Dateiname der Playlist
        /// </summary>
        public string Filename { get; set; }

        public override List<MenuItem> ContextMenuItems
        {
            get
            {
                List<MenuItem> contextMenuItems = new List<MenuItem>();// base.ContextMenuItems;

                if (ItemType == PlaylistItemType.AllPlaylists)
                {
                    contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.NewPlaylist.Text, Command = HitbaseCommands.NewPlaylist, CommandParameter = this, Icon = ImageLoader.CreateImageFromResource("NewPlaylist.png") });
                }
                else
                {
                    contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.DeletePlaylist.Text, Command = HitbaseCommands.DeletePlaylist, CommandParameter = this, Icon = ImageLoader.CreateImageFromResource("Delete.png") });
                }

                return contextMenuItems;
            }
        }
    }

    public class MainCatalogTreeItem : MainTreeItem
    {
        /// <summary>
        /// Welche Art von Liste soll im Katalog-Bereich angezeigt werden
        /// </summary>
        public ShowItemType ShowItemType { get; set; }

        /// <summary>
        /// Die Filterbedingung für die Anzeige
        /// </summary>
        public DataBaseEngine.Condition FilterCondition { get; set; }

        /// <summary>
        /// Die Id des angezeigten Elements
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Benutzerspezifische Erweiterungen
        /// </summary>
        public object Tag { get; set; }

        public override List<MenuItem> ContextMenuItems
        {
            get
            {
                List<MenuItem> contextMenuItems = base.ContextMenuItems;

                if (ShowItemType == MainControls.ShowItemType.MyMusic)
                {
                    contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.ConfigureMusicLibrary.Text, Command = HitbaseCommands.ConfigureMusicLibrary, CommandParameter = this });
                }

                if (ShowItemType == MainControls.ShowItemType.AlbumArtist || ShowItemType == MainControls.ShowItemType.Composer || ShowItemType == MainControls.ShowItemType.Artist
                    || ShowItemType == MainControls.ShowItemType.Participant)
                {
                    contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.PersonGroupProperties.Text, Command = HitbaseCommands.PersonGroupProperties,
                                                          CommandParameter = this,
                                                          Icon = ImageLoader.CreateImageFromResource("PersonGroupGeneral.png")
                    });
                }

                if (ShowItemType == MainControls.ShowItemType.Directory)
                {
#if DEBUG
                    contextMenuItems.Add(new MenuItem() { Header = "Tracknummern korrigieren", Command = HitbaseCommands.CorrectTrackNumbers, CommandParameter = this });
#endif
                }

                return contextMenuItems;
            }
        }
    }

    public class MainStatisticsTreeItem : MainTreeItem
    {
        /// <summary>
        /// Welche Art von Statistic soll angezeigt werden
        /// </summary>
        public StatisticType ShowItemType { get; set; }
    }

    public class MainCDTreeItem : MainTreeItem
    {
        /// <summary>
        /// Der Laufwerksbuchstabe des CD-Laufwerks (z.B. 'R')
        /// </summary>
        public char DriveLetter { get; set; }

        /// <summary>
        /// Die CD-Engine (Daten der CD auslesen)
        /// </summary>
        public Big3.Hitbase.SoundEngine.CDEngine CDEngine { get; set; }

        /// <summary>
        /// Die Daten der eingelegten CD.
        /// </summary>
        public CD CD { get; set; }

        public override List<MenuItem> ContextMenuItems
        {
            get
            {
                List<MenuItem> contextMenuItems = base.ContextMenuItems;

                contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.EjectCD.Text, Command = HitbaseCommands.EjectCD, CommandParameter = this, Icon = ImageLoader.CreateImageFromResource("CDEject16.png")});

                return contextMenuItems;
            }
        }
    }

    /// <summary>
    /// Meine Suchen
    /// </summary>
    public class MainSearchTreeItem : MainTreeItem
    {
        public int SearchId { get; set; }

        public Big3.Hitbase.DataBaseEngine.Condition Condition { get; set; }

        public CurrentViewMode ViewMode { get; set; }

        public override List<MenuItem> ContextMenuItems
        {
            get
            {
                List<MenuItem> contextMenuItems = base.ContextMenuItems;

                contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.RenameSearch.Text, Command = HitbaseCommands.RenameSearch, CommandParameter = this });
                contextMenuItems.Add(new MenuItem() { Header = HitbaseCommands.DeleteSearch.Text, Command = HitbaseCommands.DeleteSearch, CommandParameter = this });

                return contextMenuItems;
            }
        }
    }

    public class MainTreeItemCollection : SafeObservableCollection<MainTreeItem>
    {
    }

    public enum ShowItemType
    {
        None,
        MyMusic,
        ArtistAll,
        ArtistFirstChar,
        Artist,
        AlbumAll,
        AlbumArtistFirstChar,
        AlbumComposerFirstChar,
        AlbumArtist,
        AlbumComposer,
        Album,
        ComposerAll,
        ComposerFirstChar,
        Composer,
        Rating,
        Genre,
        YearAll,
        YearDecade,
        Year,
        Medium,
        CDSet,
        Track,
        ParticipantAll,
        ParticipantFirstChar,
        Participant,
        DirectoriesAll,
        Directory
    }

    public enum StatisticType
    {
        Overview,
        //TrackLengthSum,
        //TrackCount,
        //TrackLengthLongest,
        //TrackLengthShortest,
        //TrackLengthAverage,
        //TrackCountCDAverage,
        //CDCount,
        //CDSamplerCount,
        //CDTrackMax,
        //CDLengthAverage,
        //CDLengthLongest,
        //CDLengthShortest,
        //ArtistCount,
        //CDSetsCount,
        //CDAssignedCount,
        //CDAssignedNotCount,
        //CDLoanedCount,
        CDGroupByCategoryCount,
        CDGroupByMediumCount,
        CDGroupByPriceCount,
        CDGroupByNumberOfTracksCount,
        CDGroupByLengthCount,
        CDGroupBySamplerCount,
        CDGroupByRatingCount,
        CDGroupByAttributeCount,
        CDGroupByLabelCount,
        CDGroupByRecordCount,
        CDGroupByArtistArtCount,
        CDGroupByArtistSexCount,
        CDGroupByCountryCount,
        //CDTotalValue,
        TrackGroupByLengthCount,
        TrackGroupByRecordCount,
        TrackGroupByRating,
        TrackGroupByBPMCount,
        TrackGroupByAttributeCount,
        ArtistTrackMost,
        ArtistTrackGroupBySexCount,
        ArtistTrackGroupByCountryCount,
        ArtistTrackGroupByArtistArtCount,
        ArtistCDsMost,
        ArtistGroupBySexCount,
        ArtistGroupByCountryCount,
        ArtistGroupByArtistArtCount
    }
}
