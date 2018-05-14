using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Miscellaneous;
using System.ComponentModel;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.SharedResources;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Big3.Hitbase.DataBaseEngine;
using System.Threading.Tasks;
using System.Threading;
using Big3.Hitbase.SoundEngine.Last.fm;
using System.Runtime.InteropServices;

namespace Big3.Hitbase.SoundEngine
{
    public class Playlist : SafeObservableCollection<PlaylistItem>, INotifyPropertyChanged
    {
        [DllImport("shlwapi.dll")]
        private static extern bool PathRelativePathTo(StringBuilder path,
          string from, int attrFrom, string to, int attrTo);

        private List<int> shuffleTrackList = null;
        private int shuffleTrackListPosition = 0;
        private bool crossFadeActive = false;
        ScrobblerClass scrobble = new ScrobblerClass();

        public Playlist()
        {
            Name = StringTable.UnsavedPlaylist;

            DispatcherTimer playlistDispatcherTimer = new DispatcherTimer();
            playlistDispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            playlistDispatcherTimer.Tick += new EventHandler(playlistDispatcherTimer_Tick);
            playlistDispatcherTimer.Start();

            CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Playlist_CollectionChanged);
        }

        public event EventHandler CurrentTrackChanged;
        public event EventHandler PlayStatusChanged;

        private int currentTrack = -1;
        /// <summary>
        /// Liefert den aktuellen spielenden Track-Index zurück.
        /// </summary>
        public int CurrentTrack
        {
            get
            {
                return currentTrack;
            }
            set
            {
                if (currentTrack == value)
                    return;

                if (currentTrack >= 0 && currentTrack < this.Count)
                {
                    this[currentTrack].IsPlaying = false;
                }
                
                currentTrack = value;
                
                if (SoundEngine.Instance.IsPlaying && !crossFadeActive)
                    SoundEngine.Instance.Stop();

                if (currentTrack >= Count)
                {
                    currentTrack = -1;
                }

                if (currentTrack >= 0 && currentTrack < Count)
                {
                    while (currentTrack < Count && currentTrack >= 0)
                    {
                        try
                        {
                            SoundEngine.Instance.Play(this[currentTrack].Info.Filename);

                            if (!this[currentTrack].Info.Filename.StartsWith("cd:"))
                            {
                                SoundFileInformation.GetSoundFileInformation(this[currentTrack].Info, this[currentTrack].Info.Filename);

                                this[currentTrack].Info.Length = SoundEngine.Instance.Length;
                            }
                            this[currentTrack].IsPlaying = true;

                            break;
                        }
                        catch (Exception e)
                        {
                            this[currentTrack].HasError = true;
                            this[currentTrack].ErrorMessage = e.Message + "\r\n\r\n" + this[currentTrack].Info.Filename;

                            if (currentTrack < this.Count - 1)
                                currentTrack++;
                            else
                                currentTrack = -1;
                        }
                    }
                }

                if (currentTrack >= 0 && currentTrack < this.Count)
                {
                    scrobble.NowPlaying(this[currentTrack]);
                }

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentTrack"));
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentPlaylistItem"));
                    PropertyChanged(this, new PropertyChangedEventArgs("NextPlaylistItem"));
                }

                if (CurrentTrackChanged != null)
                {
                    CurrentTrackChanged(this, new EventArgs());
                }
            }
        }

        private bool isCDInPlaylist;
        public bool IsCDInPlaylist
        {
            get
            {
                return isCDInPlaylist;
            }
            set
            {
                isCDInPlaylist = value;
                if (PropertyChanged != null)
                {
                    FirePropertyChanged("IsCDInPlaylist");
                }
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Die Gesamtlänge der Playlist in Millisekunden. Wird immer dynamisch berechnet, also nicht unbedingt die schnellste Property, aber sollte kein Problem sein.
        /// </summary>
        public long TotalLength 
        {
            get
            {
                return GetTotalLength();
            }
        }

        // Liefert die Gesamtlänge der Playlist zurück
        private long GetTotalLength()
        {
            long totalLength = 0;

            foreach (PlaylistItem item in this)
                totalLength += item.Info.Length;

            return totalLength;
        }

        /// <summary>
        /// Die Restspielzeit der Playlist in Millisekunden. Wird immer dynamisch berechnet, also nicht unbedingt die schnellste Property, aber sollte kein Problem sein.
        /// </summary>
        public long TotalRemainLength
        {
            get
            {
                return GetTotalRemainLength();
            }
        }

        // Liefert die Restspielzeit der Playlist zurück
        private long GetTotalRemainLength()
        {
            if (CurrentTrack < 0 || CurrentTrack >= this.Count)
                return 0;

            long totalRemainLength = GetTotalLength();

            for (int i = 0; i < CurrentTrack; i++)
                totalRemainLength -= this[i].Info.Length;

            totalRemainLength -= CurrentTrackPlayPosition;
            
            return totalRemainLength;
        }
         

        private string name;
        /// <summary>
        /// Name der Playlist
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// Das aktuell spielende Playlistitem
        /// </summary>
        public PlaylistItem CurrentPlaylistItem
        {
            get
            {
                if (CurrentTrack < 0 || CurrentTrack >= this.Count)
                    return null;

                return this[CurrentTrack];
            }
        }

        /// <summary>
        /// Das nächste spielende Playlistitem
        /// </summary>
        public PlaylistItem NextPlaylistItem
        {
            get
            {
                if (CurrentTrack < 0 || CurrentTrack >= this.Count - 1)
                    return null;

                return this[CurrentTrack + 1];
            }
        }

        private RepeatType repeatType;
        public RepeatType RepeatType
        {
            get
            {
                return repeatType;
            }
            set
            {
                repeatType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RepeatType"));
            }
        }

        private bool shuffleActive;
        public bool ShuffleActive
        {
            get
            {
                return shuffleActive;
            }
            set
            {
                shuffleActive = value;

                if (shuffleActive == true)
                {
                    shuffleTrackList = GenerateShuffleTrackList();
                    shuffleTrackListPosition = 0;
                    CurrentTrack = shuffleTrackList[0];
                }

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ShuffleActive"));
            }
        }

        public void Play()
        {
            if (ShuffleActive)
            {
                shuffleTrackList = GenerateShuffleTrackList();
                CurrentTrack = shuffleTrackList[0];
            }
            else
            {
                CurrentTrack = 0;
            }
        }

        public void Stop()
        {
            SoundEngine.Instance.StopAll();
            CurrentTrack = -1;
        }

        public bool IsPlaying
        {
            get
            {
                return SoundEngine.Instance.IsPlaying;
            }
        }

        public bool IsPaused
        {
            get
            {
                return SoundEngine.Instance.IsPaused;
            }
        }

        public void Pause(bool paused)
        {
            SoundEngine.Instance.Pause(paused);

            if (PlayStatusChanged != null)
                PlayStatusChanged(this, new EventArgs());
        }

        public void PlayPrev()
        {
            if (ShuffleActive)
            {
                if (shuffleTrackListPosition > 0)
                {
                    shuffleTrackListPosition--;
                }
                else
                {
                    shuffleTrackListPosition = shuffleTrackList.Count - 1;
                }
                CurrentTrack = shuffleTrackList[shuffleTrackListPosition];
            }
            else
            {
                if (CurrentTrack > 0)
                    CurrentTrack--;
            }
        }

        public void PlayNext()
        {
            if (ShuffleActive)
            {
                if (shuffleTrackListPosition < shuffleTrackList.Count - 1)
                {
                    shuffleTrackListPosition++;
                }
                else
                {
                    shuffleTrackListPosition = 0;
                }

                if (shuffleTrackListPosition < shuffleTrackList.Count)
                {
                    CurrentTrack = shuffleTrackList[shuffleTrackListPosition];
                }
            }
            else
            {
                if (CurrentTrack < this.Count - 1)
                    CurrentTrack++;
            }
        }

        void playlistDispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (IsPlaying && CurrentPlaylistItem != null)
            {
                int currentTrackPlayPosition = CurrentTrackPlayPosition;
                if (Settings.Current.CrossFadeActive &&
                    CurrentTrackPlayPosition + Settings.Current.CrossFadeDefaultSeconds * 1000 >= CurrentPlaylistItem.Info.Length && !CurrentPlaylistItem.Info.Filename.StartsWith("cd:"))
                {
                    Scrobble();

                    int nextTrack = GetNextTrack();
                    if (nextTrack >= 0)
                    {
                        crossFadeActive = true;
                        CurrentTrack = nextTrack;
                        crossFadeActive = false;
                    }
                }

                // Am Ende des Liedes angekommen.
                if (CurrentTrackPlayPosition+200 >= CurrentPlaylistItem.Info.Length)
                {
                    Scrobble();

                    int nextTrack = GetNextTrack();
                    CurrentTrack = nextTrack;
                }
            }
        }

        private void Scrobble()
        {
            // Scrobblen
            if (this[currentTrack].Info.Length > 30000)
            {
                scrobble.Scrobble();

                //SoundEngine.Instance.PlayCountFileList.Add(this.CurrentPlaylistItem.Info.Filename);

            }
        }

        public int GetNextTrack()
        {
            if (RepeatType == Hitbase.SoundEngine.RepeatType.Single)
            {
                return CurrentTrack;
            }
            else
            {
                if (RepeatType == Hitbase.SoundEngine.RepeatType.All && CurrentTrack == Count - 1 && !ShuffleActive)
                {
                    return 0;
                }
                else
                {
                    if (ShuffleActive)
                    {
                        if (shuffleTrackListPosition < shuffleTrackList.Count - 1)
                        {
                            shuffleTrackListPosition++;
                        }
                        else
                        {
                            shuffleTrackListPosition = 0;
                        }

                        return shuffleTrackList[shuffleTrackListPosition];
                    }
                    else
                    {
                        if (CurrentTrack < Count - 1)
                            return CurrentTrack + 1;
                        else
                            return -1;
                    }
                }
            }
        }

        public int PreviewGetNextTrack()
        {
            if (RepeatType == Hitbase.SoundEngine.RepeatType.Single)
            {
                return CurrentTrack;
            }
            else
            {
                if (RepeatType == Hitbase.SoundEngine.RepeatType.All && CurrentTrack == Count - 1 && !ShuffleActive)
                {
                    return 0;
                }
                else
                {
                    if (ShuffleActive)
                    {
                        if (shuffleTrackListPosition < shuffleTrackList.Count - 1)
                        {
                            return shuffleTrackList[shuffleTrackListPosition + 1];
                        }
                        else
                        {
                            return shuffleTrackList[0];
                        }
                    }
                    else
                    {
                        if (CurrentTrack < Count - 1)
                            return CurrentTrack + 1;
                        else
                            return -1;
                    }
                }
            }
        }


        private List<int> GenerateShuffleTrackList()
        {
            List<int> trackList = new List<int>();

            for (int i = 0; i < Count; i++)
            {
                if (i != CurrentTrack)
                    trackList.Add(i);
            }

            trackList.Shuffle();

            if (CurrentTrack >= 0)
                trackList.Insert(0, CurrentTrack);

            return trackList;
        }

        void Playlist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ShuffleActive && IsPlaying)
            {
                shuffleTrackList = GenerateShuffleTrackList();
                shuffleTrackListPosition = 0;
            }
        }


        /// <summary>
        /// Fügt eine neue Musikdatei zur Playlist hinzu.
        /// </summary>
        /// <param name="filename"></param>
        public void AddTrack(string filename, int addPosition = -1)
        {
            if (string.IsNullOrEmpty(filename))
                return;

            PlaylistItem newItem = GetPlaylistItem(filename);

            if (newItem != null)
            {
                StartNewItemAnimation(newItem);

                if (addPosition < 0 || addPosition > Count)
                    AddItemFromThread(newItem);
                else
                    Insert(addPosition, newItem);
            }
        }

        /// <summary>
        /// Hiermit werden neue Items animiert.
        /// </summary>
        /// <param name="newItem"></param>
        private static void StartNewItemAnimation(PlaylistItem newItem)
        {
            newItem.IsNew = true;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                Thread.Sleep(1000);
            };
            bw.RunWorkerCompleted += delegate
            {
                newItem.IsNew = false;
            };

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Fügt eine neue Musikdatei zur Playlist hinzu.
        /// </summary>
        /// <param name="filename"></param>
        private void AddTrack(DataBase db, Track track, int addPosition = -1, bool addCD = false)
        {
            PlaylistItem newItem = GetPlaylistItem(db, track);

            if (addCD && track.CDDriveLetter != '\0')
            {
                newItem.Info.Filename = string.Format("cd:{0}:{1}", track.CDDriveLetter, track.TrackNumber - 1);
            }

            if (newItem != null && !string.IsNullOrEmpty(newItem.Info.Filename))
            {
                StartNewItemAnimation(newItem);

                if (addPosition < 0 || addPosition > Count)
                {
                    AddItemFromThread(newItem);
                }
                else
                {
                    Insert(addPosition, newItem);
                }
            }
        }

        /// <summary>
        /// Fügt eine neue Musikdatei zur Playlist hinzu.
        /// </summary>
        /// <param name="filename"></param>
        public void AddTrack(string filename, int length, string artist, string title, int addPosition = -1)
        {
            PlaylistItem newItem = new PlaylistItem(this);

            newItem.Info = new SoundFileInformation();
            newItem.Info.Length = length;
            newItem.Info.Filename = filename;
            newItem.Info.Artist = artist;
            newItem.Info.Title = title;

            if (newItem != null)
            {
                newItem.IsNew = true;

                if (addPosition < 0 || addPosition > Count)
                {
                    AddItemFromThread(newItem);
                }
                else
                {
                    Insert(addPosition, newItem);
                }
            }
        }


        public void AddTracks(string[] filenames, AddTracksToPlaylistType addToPlaylistType, int addPosition = 0)
        {
            int count = 0;
            int playNowFirstTrack = -1;

            foreach (string filename in filenames)
            {
                switch (addToPlaylistType)
                {
                    case AddTracksToPlaylistType.None:
                        break;
                    case AddTracksToPlaylistType.Now:
                        {
                            AddTrack(filename, CurrentTrack + count + 1);

                            if (playNowFirstTrack < 0)
                                playNowFirstTrack = CurrentTrack + 1;

                            break;
                        }
                    case AddTracksToPlaylistType.Next:
                        AddTrack(filename, CurrentTrack + count + 1);
                        break;
                    case AddTracksToPlaylistType.End:
                        AddTrack(filename);
                        break;
                    case AddTracksToPlaylistType.InsertAtIndex:
                        AddTrack(filename, addPosition + count);
                        break;
                    default:
                        break;
                }

                count++;
            }

            if (addToPlaylistType == AddTracksToPlaylistType.Now)
            {
                CurrentTrack = playNowFirstTrack;
            }
        }

        public void AddTracks(DataBase db, Big3.Hitbase.DataBaseEngine.Track[] tracks, AddTracksToPlaylistType addToPlaylistType, int addPosition = 0, bool addCD = false, int autoPlayIndex = 0)
        {
            if (this.IsCDInPlaylist && !addCD)
                return;

            int count = 0;

            int startTrack = Math.Max(0, CurrentTrack);

            if (!IsPlaying && addToPlaylistType == AddTracksToPlaylistType.Next)
            {
                startTrack = -1;
            }

            int playNowFirstTrack = -1;

            foreach (Big3.Hitbase.DataBaseEngine.Track track in tracks)
            {
                switch (addToPlaylistType)
                {
                    case AddTracksToPlaylistType.None:
                        break;
                    case AddTracksToPlaylistType.Now:
                        AddTrack(db, track, startTrack + count, addCD);
                        if (playNowFirstTrack < 0 && count == autoPlayIndex)
                            playNowFirstTrack = startTrack + count;

                        break;
                    case AddTracksToPlaylistType.Next:
                        AddTrack(db, track, startTrack + count + 1, addCD);
                        break;
                    case AddTracksToPlaylistType.End:
                        AddTrack(db, track, -1, addCD);
                        break;
                    case AddTracksToPlaylistType.InsertAtIndex:
                        AddTrack(db, track, addPosition + count, addCD);
                        break;
                    default:
                        break;
                }

                count++;
            }

            if (addToPlaylistType == AddTracksToPlaylistType.Now)
            {
                CurrentTrack = playNowFirstTrack;
            }

            this.IsCDInPlaylist = addCD;
        }


        private PlaylistItem GetPlaylistItem(string filename)
        {
            PlaylistItem playlistItem = new PlaylistItem(this);

            playlistItem.Info = SoundFileInformation.GetSoundFileInformation(filename);

            return playlistItem;
        }

        private PlaylistItem GetPlaylistItem(DataBase db, Track track)
        {
            PlaylistItem playlistItem = new PlaylistItem(this);

            playlistItem.ID = track.ID;
            playlistItem.Info = SoundFileInformation.GetSoundFileInformation(db, track);

            return playlistItem;
        }

        /// <summary>
        /// Die Playliste aus einer Datei laden (z.B. m3u)
        /// </summary>
        /// <param name="filename"></param>
        public void LoadFromFile(string filename, bool asynchron)
        {
            IsCDInPlaylist = false;

            string filenameExtension = Path.GetExtension(filename).ToLower();

            if (this.IsPlaying)
            {
                this.Stop();
            }

            Name = Path.GetFileNameWithoutExtension(filename);

            if (asynchron)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += delegate
                {
                    if (filenameExtension == ".hvc")
                    {
                        LoadFromFileHVC(filename);
                    }
                    if (filenameExtension == ".m3u")
                    {
                        LoadFromFileM3U(filename);
                    }
                };
                bw.RunWorkerCompleted += delegate
                {
                    foreach (PlaylistItem item in this)
                        item.IsNew = false;
                };

                bw.RunWorkerAsync();
            }
            else
            {
                LoadFromFileHVC(filename);
            }
        }

        private void LoadFromFileM3U(string filename)
        {
            string[] allLines = File.ReadAllLines(filename, Encoding.UTF8);

	        if (allLines[0] != "#EXTM3U")        // Das ist eine M3U-datei
	        {
		        return;
	        }

            ClearAll();

        	String sM3UPath = Path.GetDirectoryName(filename);

	        String artist = "";
	        String title = "";
	        int length = 0;
	
	        foreach (string sLine in allLines)
	        {
    	        if (sLine == "#EXTM3U")        // Das ist eine M3U-datei
                    continue;

		        if (sLine.Left(1) == "#")
		        {
			        if (sLine.Left(8) == "#EXTINF:")
			        {
				        String str = sLine.Mid(8);

				        length = Misc.Atoi(str)*1000;
				        int iPos = str.IndexOf(",");
				        if (iPos >= 0)
				        {
                            int hyphenLength = 3;
					        str = str.Mid(iPos+1);
					        iPos = str.IndexOf(" - ");
                            if (iPos < 0)
                            {
                                iPos = str.IndexOf("-");
                                hyphenLength = 1;
                            }
					        if (iPos > 0)
					        {
						        artist = str.Left(iPos);
						        artist.Trim();
						        title = str.Mid(iPos + hyphenLength);
						        title.Trim();
					        }
				        }
			        }
		        }
		        else
		        {
			        //String sAbsFilename;
			        String sPath = Path.Combine(sM3UPath, sLine);
			        //PathCanonicalize(sAbsFilename.GetBuffer(_MAX_PATH), sPath);
			        //sAbsFilename.ReleaseBuffer();
                    AddTrack(sPath, length, artist, title);

                    // Damit die Animation schöner aussieht, hier eine kleine Verzögerung. Kann man
                    // eventuell noch verbessern, indem man das Pause nur für die sichtbaren Elemente macht.
                    System.Threading.Thread.Sleep(10);
                }
	        }
        }


        private void SaveToFileM3U(string filename)
        {
            if (IsCDInPlaylist)
                return;

            File.Delete(filename);
            StreamWriter fsM3U = new StreamWriter(filename);
            fsM3U.WriteLine("#EXTM3U");

	        for (int i=0;i<Items.Count;i++)
	        {
                if (string.IsNullOrEmpty(Items[i].Info.Filename))
                    continue;

                String sFilename = Items[i].Info.Filename.Mid(Items[i].Info.Filename.LastIndexOf('\\') + 1);;

		        String sLine;

		        if (string.IsNullOrEmpty(Items[i].Info.Artist) && string.IsNullOrEmpty(Items[i].Info.Title))
                {
			        //sLine.Format("#EXTINF:%d,%s\n", Items[i].Info.Length/1000, sFilename);
                    sLine = String.Format("#EXTINF:{0},{1}", Items[i].Info.Length/1000, sFilename);
                }
		        else
                {
                    sLine = String.Format("#EXTINF:{0},{1} - {2}", Items[i].Info.Length / 1000, Items[i].Info.Artist, Items[i].Info.Title);
                }

		        fsM3U.WriteLine(sLine);

		        String sRelFilename;
		        // Die Pfade werden nun relativ zur m3u-Datei gespeichert.
                sRelFilename = Misc.GetRelativePath(filename, Items[i].Info.Filename);
		        if (sRelFilename.Left(2) == ".\\")
			        sRelFilename = sRelFilename.Mid(2);

    		    fsM3U.WriteLine(sRelFilename);

            }
            fsM3U.Close();
        }

        private static string RelativePathTo(string from, string to)
        {
            FileInfo fi = new FileInfo(from);
            int attr = (int)(fi.Attributes & FileAttributes.Directory);
            StringBuilder retval = new StringBuilder(259);
            bool ok = PathRelativePathTo(retval, from, attr, to, attr);
            if (!ok) throw new ArgumentException();
            return retval.ToString();
        }

        public void ClearAll()
        {
            if (IsPlaying)
            {
                Stop();
            }

            ClearFromThread();
        }
      

        private void LoadFromFileHVC(string filename)
        {
            string[] allLines = File.ReadAllLines(filename);

            if (allLines.Length < 2)
                return;

            if (GetValue(allLines[0], "FMT") != "hvc1")
                return;

            ClearAll();

            int numberOfTracks = 0;
            int.TryParse(GetValue(allLines[1], "Count"), out numberOfTracks);

            int line = 2;
            for (int i = 0; i < numberOfTracks; i++)
            {
                int length = 0;
                int.TryParse(GetValue(allLines[line + 1], "LENGTH"), out length);
                string trackFilename = GetValue(allLines[line + 2], "FILENAME");
                string artist = GetValue(allLines[line + 3], "ARTIST");
                string title = GetValue(allLines[line + 4], "TITLE");

                string absoluteFilename;
                if (trackFilename.Contains(':'))
                {
                    // wir haben bereits einen absoluten Pfad (wohl anderes Laufwerk)
                    absoluteFilename = trackFilename;
                }
                else 
                { 
                    absoluteFilename = Path.Combine(Path.GetDirectoryName(filename), trackFilename);
                    absoluteFilename = Path.GetFullPath(absoluteFilename);
                }

                AddTrack(absoluteFilename, length, artist, title);

                // Damit die Animation schöner aussieht, hier eine kleine Verzögerung. Kann man
                // eventuell noch verbessern, indem man das Pause nur für die sichtbaren Elemente macht.
                System.Threading.Thread.Sleep(10);

                line += 5;
            }
        }

        private string GetValue(string line, string key)
        {
            string foundKey = string.Empty;

            int pos = line.IndexOf('=');
            if (pos > 0)
            {
                foundKey = line.Substring(0, pos);
            }

            if (foundKey == key)
                return line.Substring(pos + 1);
            else
                return string.Empty;
        }

        /// <summary>
        /// Legt eine neue, leere Wiedergabeliste an.
        /// </summary>
        /// <param name="filename"></param>
        public static void CreateEmptyPlaylist()
        {
            for (int i = 0; i < 99; i++)
            {
                string fullFilename = Path.Combine(Misc.GetPersonalHitbasePlaylistFolder(), StringTable.UntitledPlaylist);

                if (i > 0)
                {
                    fullFilename += " (" + (i + 1).ToString() + ")";
                }

                fullFilename = Path.ChangeExtension(fullFilename, ".hvc");

                if (!File.Exists(fullFilename))
                {
                    Playlist p = new Playlist();
                    p.SaveToFile(fullFilename);
                    break;
                }
            }
        }

        public void SaveToFile(string filename)
        {
            if (IsCDInPlaylist)
                return;

            string filenameExtension = Path.GetExtension(filename).ToLower();

            if (string.IsNullOrEmpty(filenameExtension))
                filename += ".m3u";



            if (filenameExtension == ".hvc")
            {
                SaveToFileHVC(filename);
            }

            if (filenameExtension == ".m3u" || string.IsNullOrEmpty(filenameExtension))
            {
                SaveToFileM3U(filename);
            }
        }

        private void SaveToFileHVC(string filename)
        {
            List<string> allLines = new List<string>();

            allLines.Add("FMT=hvc1");
            allLines.Add(string.Format("Count={0}", this.Count));

            for (int i = 0; i < this.Count; i++)
            {
                string trackFilename = Misc.GetRelativePath(filename, this[i].Info.Filename);

                allLines.Add(string.Format("PARAMS=-1,-1"));
                allLines.Add(string.Format("LENGTH={0}", this[i].Info.Length));
                allLines.Add(string.Format("FILENAME={0}", trackFilename));
                allLines.Add(string.Format("ARTIST={0}", this[i].Info.Artist));
                allLines.Add(string.Format("TITLE={0}", this[i].Info.Title));
            }

            File.WriteAllLines(filename, allLines.ToArray());
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public int CurrentTrackPlayPosition 
        {
            get
            {
                return SoundEngine.Instance.PlayPosition;
            }
            set
            {
                SoundEngine.Instance.PlayPosition = value;
            }
        }

        protected override void RemoveItem(int index)
        {
            //if (index == CurrentTrack)
            //    return;
            if (currentTrack > index)
                currentTrack--;

            base.RemoveItem(index);

            if (this.Count == 0 && IsCDInPlaylist)
            {
                Name = StringTable.UnsavedPlaylist;
                IsCDInPlaylist = false;
            }
        }

        protected override void InsertItem(int index, PlaylistItem item)
        {
            // Dann wird der gerade spielende Track per Drag&Drop verschoben
            if (item.IsPlaying)
            {
                currentTrack = index;
            }
            else
            {
                if (currentTrack != -1 && currentTrack >= index)
                    currentTrack++;
            }

            base.InsertItem(index, item);
        }

        public void InitScrobble()
        {
            scrobble.Init();
        }

        public void DeleteCDTracks(string driveLetter)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (this[i].Info.Filename.StartsWith("cd:" + driveLetter, StringComparison.InvariantCultureIgnoreCase))
                {
                    RemoveItem(i);
                }
            }

            IsCDInPlaylist = false;
        }
    }

    public enum RepeatType
    {
        None,
        Single,
        All
    }

    public class PlaylistItem : INotifyPropertyChanged
    {
        public PlaylistItem(Playlist playlist)
        {
            this.Playlist = playlist;
        }

        /// <summary>
        /// Die Informationen zum Track (Interpret, Titel, etc.)
        /// </summary>
        public SoundFileInformation Info { get; set; }

        public Playlist Playlist { get; set; }

        public int ID { get; set; }

        public System.Windows.Media.ImageSource DefaultTrackImage
        {
            get
            {
                return ImageLoader.FromResource("CDCover.png");
            }
        }

        public System.Windows.Media.ImageSource TrackImage
        {
            get
            {
                return GetTrackImage(false);
            }
        }

        private System.Windows.Media.ImageSource GetTrackImage(bool highQuality)
        {
            try
            {
                byte[] imageBytes = null;
                if (Info.Images != null && Info.Images.Count > 0)
                {
                    imageBytes = Info.Images[0];
                }
                else
                {
                    SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(Info.Filename);
                    if (sfi.Images != null && sfi.Images.Count > 0)
                    {
                        imageBytes = sfi.Images[0];
                    }
                }

                if (imageBytes != null)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    MemoryStream ms = new MemoryStream(imageBytes);
                    image.StreamSource = ms;
                    if (!highQuality)
                    {
                        image.DecodePixelWidth = 100;
                        image.DecodePixelHeight = 100;
                    }
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch
            {
                // Dann ist wohl das Bild kaputt..... 
            }

            return DefaultTrackImage;
        }

        public System.Windows.Media.ImageSource HighQualityTrackImage
        {
            get
            {
                return GetTrackImage(true);
            }
        }


        // Interne Properties für das VM
        private bool isNew = false;
        public bool IsNew 
        {
            get
            {
                return isNew;
            }
            set
            {
                isNew = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsNew"));
            }
        }

        private bool isPlaying = false;
        /// <summary>
        /// Liefert true zurück, wenn dieser Track gerade spielt.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            set
            {
                isPlaying = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsPlaying"));
            }
        }

        private bool hasError = false;
        /// <summary>
        /// true, wenn beim Laden oder Abspielen dieses Playlist-Items ein Fehler aufgetreten ist (Fehlermeldung siehe ErrorMessage)
        /// </summary>
        public bool HasError
        {
            get 
            { 
                return hasError; 
            }
            set 
            { 
                hasError = value;
                
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HasError"));
            }
        }

        private string errorMessage;
        /// <summary>
        /// Fehlermeldung, wenn HasError=true ist
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set 
            { 
                errorMessage = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
