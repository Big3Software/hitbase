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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Big3.Hitbase.Miscellaneous;
using System.Threading.Tasks;
using System.Windows.Threading;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.CDUtilities;
using Microsoft.Win32;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Controls;
using Big3.Hitbase.SoundEngine.CDText;
using System.IO;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for MainCDUserControl.xaml
    /// </summary>
    public partial class MainCDUserControl : UserControl, IMainTabInterface
    {
        public readonly CD CD;

        public CDEngine CDEngine { get; set; }
        public Playlist Playlist { get; private set; }
        private DataBase dataBase;

        private int autoPlayTrackNumber;

        // Es kann nur einen geben..... ;-)
        public static Hitbase.RecordMedium.RecordEngine RecordEngine = new RecordMedium.RecordEngine();

        private DispatcherTimer dtRecord = new DispatcherTimer();

        public MainCDUserControl(DataBase db, CD cd, CDEngine cdEngine, int autoPlayTrackNumber, Playlist playlist)
        {
            InitializeComponent();

            this.dataBase = db;
            this.CD = cd;
            this.CDEngine = cdEngine;
            this.Playlist = playlist;
            this.autoPlayTrackNumber = autoPlayTrackNumber;

            this.cdUserControl.DataBase = db;


            if (cdEngine.IsCDInDrive)
            {
                ReadCDInformation();

                StackPanelInsertCD.Visibility = System.Windows.Visibility.Collapsed;
                cdUserControl.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                StackPanelInsertCD.Visibility = System.Windows.Visibility.Visible;
                cdUserControl.Visibility = System.Windows.Visibility.Collapsed;
            }

            dtRecord.Interval = TimeSpan.FromMilliseconds(100);
            dtRecord.Tick += new EventHandler(dtRecord_Tick);

        }

        void cdEngine_CDEjected(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (Settings.Current.AutoSaveCDs)
                {
                    SaveCD();
                }

                UpdateTabCaption();

                Big3.Hitbase.SharedResources.HitbaseCommands.DeleteCDTracksFromPlaylist.Execute(CDEngine.DriveLetter, Application.Current.MainWindow);

                StackPanelInsertCD.Visibility = System.Windows.Visibility.Visible;
                cdUserControl.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        void cdEngine_CDInserted(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ReadCDInformation();

                StackPanelInsertCD.Visibility = System.Windows.Visibility.Collapsed;
                cdUserControl.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        void dtRecord_Tick(object sender, EventArgs e)
        {
            if (RecordEngine.RecordRunning)
            {
                this.cdUserControl.SetRecordProgress(RecordEngine.CurrentTrackRecording, RecordEngine.CurrentTrackRecordingProgress);
            }
            else
            {
                this.cdUserControl.SetRecordProgress(-1, -1);
            }

            if (Playlist.IsPlaying)
                this.cdUserControl.SetActiveTrack(this.Playlist.CurrentPlaylistItem);
            else
                this.cdUserControl.SetActiveTrack(null);
        }

        private bool FindCDInDataBase()
        {
            return dataBase.FillCDByIdentity(CD);
        }

        private void SearchCDInCDArchive()
        {
            CCDArchive cdarchive = new CCDArchive();

            int searchError = 0;
            cdarchive.Search(CD, 1, ref searchError, 0, IntPtr.Zero);

            // Falls die CD eine Kategorie hat, die nicht vorhanden ist, diese anlegen
            dataBase.AllCategories.GetIdByName(CD.Category, true);
        }

        public void SearchCD()
        {
            SearchCDInCDArchive();
        }

        /// <summary>
        /// Die CD-Daten asynchron laden.
        /// </summary>
        public void ReadCDInformation()
        {
            cdUserControl.IsEnabled = false;
            GridWaitProgress.Visibility = System.Windows.Visibility.Visible;

            CD.Clear();
            //CD = new DataBaseEngine.CD();
            //cdUserControl.CD = null;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                // CD-Informationen im Hintergrund einlesen, damit Hitbase nicht blockiert ist.
                CDInfo cdInfo = SoundEngine.SoundEngine.Instance.ReadCDInformation(CDEngine.DriveLetter);

                if (cdInfo != null)
                {
                    bool mp3AudioCD = false;

                    if (cdInfo.IsPureDataCD())
                    {
                        Dispatcher.Invoke((Action)delegate
                            {
                                WpfMessageBoxResult result = WPFMessageBox.Show(System.Windows.Application.Current.MainWindow,
                                    StringTable.PureDataCDTitle, StringTable.PureDataCD,
                                    "/Big3.Hitbase.SharedResources;component/Images/Info.png",
                                    WpfMessageBoxButtons.YesNo, "PureDataCDScanFiles", false, 300);

                                if (result == WpfMessageBoxResult.Yes)
                                {
                                    ScanDataCD(CDEngine.DriveLetter.ToString() + ":\\");
                                    mp3AudioCD = true;
                                }
                            });
                    }

                    if (!mp3AudioCD)
                    {
                        CopyCDInfoToCD(cdInfo);
                    }

                    if (!FindCDInDataBase())
                    {
                        ReadCDTextInformation(cdInfo);

                        if (CD.Tracks.Count > 0 && string.IsNullOrEmpty(CD.Artist) && string.IsNullOrEmpty(CD.Title) && CD.Tracks.All(x => string.IsNullOrEmpty(x.Artist)))
                        {
                            SearchCDInCDArchive();
                        }

                        if (Settings.Current.AutoDateToday && this.dataBase.Master.DateType != DateType.None)
                        {
                            CD.Date = Misc.GetDate();
                        }

                        if (Settings.Current.AutoSearchCover)
                        {
                            if (!string.IsNullOrEmpty(CD.Artist) && !string.IsNullOrEmpty(CD.Title))
                            {
                                CDCoverAmazon.GetCDCover(this.CD);
                            }
                        }

                        if (Settings.Current.AutoSearchLyrics)
                        {
                            Dispatcher.Invoke((Action)delegate
                            {
                                waitProgress.WaitText = StringTable.SearchLyrics;
                            });

                            foreach (var track in this.CD.Tracks)
                            {
                                if (!string.IsNullOrEmpty(track.Artist) && !string.IsNullOrEmpty(track.Title))
                                {
                                    string lyrics = LyricsSearch.Search(track.Artist, track.Title);
                                    if (!string.IsNullOrEmpty(lyrics))
                                        track.Lyrics = lyrics;
                                }
                            }
                        }
                    }
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                cdUserControl.IsEnabled = true;

                GridWaitProgress.Visibility = System.Windows.Visibility.Collapsed;

                cdUserControl.CD = CD;

                // Automatisch CD kopieren?
                if (CDEngine.IsCDInDrive && Big3.Hitbase.Configuration.Settings.Current.RecordAutoCDCopy)
                {
                    int numsoundfile = 0;

                    foreach (Track track in CD.Tracks)
                    {
                        if (track.SoundFileExists)
                        {
                            numsoundfile += 1;
                        }
                    }

                    // Wenn bereits für alle Tracks ein Soudnfile vorhanden ist mache nix...
                    if (numsoundfile != CD.NumberOfTracks)
                    {
                        HitbaseCommands.StartRecord.Execute(null, this);
                    }
                }

                UpdateTabCaption();

                // Automatisch abspielen? Wenn im Explorer ein CD-Laufwerk doppelgeklickt wurde.
                if (autoPlayTrackNumber > 0)
                {
                    if (this.CD != null && this.CD.Tracks != null && this.CD.Tracks.Count >= autoPlayTrackNumber)
                    {
                        AddToPlaylist(this.CD.Tracks[autoPlayTrackNumber - 1].ID, AddTracksToPlaylistType.Now);
                    }

                    autoPlayTrackNumber = 0;
                }
            };

            bw.RunWorkerAsync();
        }

        private void ScanDataCD(string directory)
        {
            CD.Tracks.ClearFromThread();
            CD.TotalLength = 0;

            //directory = @"D:\mp3test";
            ScanDataCDRecursive(directory);

            CD.NumberOfTracks = CD.Tracks.Count;
            CD.Identity = GetMP3CDMediaIdentity();

            CD.Type = AlbumType.MusicDataCD;

            // Testen, ob es ein Sampler ist
            foreach (Track track in CD.Tracks)
            {
                if (track.Artist != CD.Artist && !string.IsNullOrEmpty(track.Artist) && !string.IsNullOrEmpty(track.Artist))
                {
                    CD.Sampler = true;
                    break;
                }
            }
        }

        private void ScanDataCDRecursive(string directory)
        {
            string[] files = Directory.GetFileSystemEntries(directory);

            foreach (string filename in files)
            {
                if (Directory.Exists(filename))
                {
                    ScanDataCDRecursive(filename);
                }
                else
                {
                    if (Big3.Hitbase.SoundEngine.SoundEngine.IsSupportedFileType(filename))
                    {
                        SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(filename);
                        Track cdTrack = new Track();
                        SoundFileInformation.FillAlbumAndTrackFromSoundFileInformation(CD, cdTrack, sfi);
                        CD.Tracks.AddItemFromThread(cdTrack);

                        CD.TotalLength += cdTrack.Length;
                    }
                }
            }

        }

        // Berechnet eine "Pseudo"-Media-Identity aus der virtuellen CD. Ist für MP3-CDs nötig, die
        // in der Datenbank gespeichert werden sollen.
        // Die Identity ist wie folgt aufgebaut: "Maaabbbbccddee"
        // aaa = Anzahl Lieder MOD 1000
        // bbbb = Gesamtlänge der Lieder in Millisekunden MOD 10000
        // cc,dd,ee = Jeweils Länge von Lied 1, 2, 3 in Sekunden MOD 100 ("00", wenn Lied nicht vorhanden)
        private string GetMP3CDMediaIdentity()
        {
	        int iTrack1Length=0;
	        int iTrack2Length=0;
	        int iTrack3Length=0;

	        if (CD.Tracks.Count >= 1)
		        iTrack1Length = CD.Tracks[0].Length/1000;
	        if (CD.Tracks.Count >= 2)
		        iTrack2Length = CD.Tracks[1].Length/1000;
	        if (CD.Tracks.Count >= 3)
		        iTrack3Length = CD.Tracks[2].Length/1000;

	        String sIdentity;
	        sIdentity = string.Format("M{0:D3}{1:D4}{2:D2}{3:D2}{4:D2}", CD.Tracks.Count % 1000, CD.TotalLength % 10000, iTrack1Length % 100, iTrack2Length % 100, iTrack3Length % 100);

	        return sIdentity;
        }



        private void ReadCDTextInformation(CDInfo cdInfo)
        {
            if (Settings.Current.DisableCDText)
                return;

            CDDrive cdDrive = new CDDrive();

            cdDrive.Open(CDEngine.DriveLetter);

            Ripper ripper = new Ripper(cdDrive);

            ripper.ReadCDText(CD);

            cdDrive.Close();
        }

        private void UpdateTabCaption()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(CDUserControl.CDTitleChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void CopyCDInfoToCD(CDInfo cdInfo)
        {
            CD.Tracks.ClearFromThread();

            int trackNumber = 1;
            foreach (CDInfoTrack track in cdInfo.Tracks)
            {
                Track cdTrack = new Track();
                cdTrack.TrackNumber = trackNumber;
                cdTrack.Length = track.Length;
                cdTrack.StartPosition = track.StartTime;
                cdTrack.CD = CD;
                cdTrack.CDDriveLetter = CDEngine.DriveLetter;
                CD.Tracks.AddItemFromThread(cdTrack);

                trackNumber++;
            }
 
            CD.NumberOfTracks = cdInfo.Tracks.Count;
            CD.TotalLength = cdInfo.TotalLength;
            CD.Identity = cdInfo.Identity;
        }

        private void CommandBindingSaveCurrentCD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveCD();
        }


        public void ShowRecordOptions()
        {
            Hitbase.RecordMedium.RecordMedium recordMedium = new RecordMedium.RecordMedium(this.CD, this.dataBase, this.CDEngine.DriveLetter, new NativeWindowWrapper(Window.GetWindow(this)).Handle);

            recordMedium.ShowDialog(new NativeWindowWrapper(Window.GetWindow(this)));
        }

        public int[] GetSelectedTrackIndices()
        {
            return cdUserControl.GetSelectedTrackIndices();
        }

        public void StartRecord(bool selectedTracksOnly)
        {
            if (RecordEngine.RecordRunning)
                return;

            int[] selectedTracks = cdUserControl.GetSelectedTrackIndices();

            if (selectedTracksOnly)
            {
                if (!RecordEngine.CheckEmptyFields(this.CD, this.dataBase, this.CDEngine.DriveLetter, selectedTracks))
                    return;
            }
            else
            {
                if (!RecordEngine.CheckEmptyFields(this.CD, this.dataBase, this.CDEngine.DriveLetter))
                    return;
            }

            Task.Factory.StartNew(() =>
            {
                if (selectedTracksOnly)
                {
                    RecordEngine.RecordNow(this.CD, this.dataBase, this.CDEngine.DriveLetter, selectedTracks);
                }
                else
                {
                    RecordEngine.RecordNow(this.CD, this.dataBase, this.CDEngine.DriveLetter);
                }
            });
        }

        public void CancelRecord()
        {
            RecordEngine.Canceled = true;
        }

        bool isInitialized = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
            {
                dtRecord.Start();

                CDEngine.CDInserted += cdEngine_CDInserted;
                CDEngine.CDEjected += cdEngine_CDEjected;

                isInitialized = true;
            }
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (isInitialized)
            {
                dtRecord.Stop();

                if (Settings.Current.AutoSaveCDs)
                {
                    SaveCD();
                }

                CDEngine.CDInserted -= cdEngine_CDInserted;
                CDEngine.CDEjected -= cdEngine_CDEjected;

                isInitialized = false;
            }
        }

        public void SaveCD()
        {
            TextBox tb = Keyboard.FocusedElement as TextBox;
            if (tb != null)
            {
                var bindExp = tb.GetBindingExpression(TextBox.TextProperty);
                if (bindExp != null)
                {
                    bindExp.UpdateSource();
                }
            }

            CD.Save(dataBase);
            
            // Nach dem Speichern laden, damit die Track-IDs gefüllt sind
            dataBase.FillCDByIdentity(CD);

        }

        public bool Closing()
        {
            // Wenn gerade eine Kopiervorgang läuft... tja, watt nu?
            if (RecordEngine.RecordRunning && !RecordEngine.Canceled)
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
                        RecordEngine.Canceled = true;
                        HitbaseCommands.CloseTab.Execute(null, this);
                    }
                });


                return false;
            }

            cdUserControl.SaveConfiguration();

            if (Settings.Current.AutoSaveCDs)
            {
                SaveCD();
            }

            //Big3.Hitbase.SharedResources.HitbaseCommands.DeleteCDTracksFromPlaylist.Execute(CDEngine.DriveLetter, Application.Current.MainWindow);

            return true;
        }

        private void CommandBindingAddTracksToPlaylistNow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int trackId = (int)e.Parameter;

            AddToPlaylist(trackId, AddTracksToPlaylistType.Now);
            e.Handled = true;
        }

        private void CommandBindingAddTracksToPlaylistNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int trackId = (int)e.Parameter;

            AddToPlaylist(trackId, AddTracksToPlaylistType.Next);
            e.Handled = true;
        }

        private void CommandBindingAddTracksToPlaylistLast_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int trackId = (int)e.Parameter;

            AddToPlaylist(trackId, AddTracksToPlaylistType.End);
            e.Handled = true;
        }

        private void AddToPlaylist(int trackId, AddTracksToPlaylistType addtoPlaylistType)
        {
            AddCDTracksToPlaylistParameter addTracksParams = new AddCDTracksToPlaylistParameter();
            addTracksParams.AddTracksType = addtoPlaylistType;
            addTracksParams.PlayTrackId = trackId;

            for (int i = 0; i < this.CD.Tracks.Count; i++)
            {
                if (this.CD.Type == AlbumType.AudioCD)
                    this.CD.Tracks[i].CDDriveLetter = CDEngine.DriveLetter;
                addTracksParams.Tracks.Add(this.CD.Tracks[i]);
            }

            addTracksParams.ClearPlaylist = true;
            addTracksParams.CD = this.CD;
            Big3.Hitbase.SharedResources.HitbaseCommands.AddTracksToPlaylist.Execute(addTracksParams, Application.Current.MainWindow);
        }


        public string GetCDTitle()
        {
            if (CDEngine.IsCDInDrive)
                return CD.Artist + " - " + CD.Title + " (" + CDEngine.DriveLetter + ":)";
            else
                return String.Format("<{0}> ({1}:\\)", StringTable.Empty, CDEngine.DriveLetter);
        }


        public void Restore(RegistryKey regKey)
        {
        }

        public void Save(RegistryKey regKey)
        {
            regKey.SetValue("DriveLetter", Char.ToString(CDEngine.DriveLetter));
        }
    }
}
