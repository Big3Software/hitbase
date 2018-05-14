using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.DataBaseEngine;
using System.ComponentModel;
using Big3.Hitbase.SoundEngine;
using System.IO;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Configuration;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Big3.Hitbase.SoundFilesManagement
{
    /// Diese Klasse synchronisiert im Hintergrund alle überwachten Verzeichnisse mit dem Katalog
    public class SynchronizeCatalogWorker
    {
        public int NumberOfItems { get; set; }

        public int NumberOfChangedItems { get; set; }

        public bool IsRunning { get; set; }

        private DataBase dataBase;

        static SynchronizeCatalogWorker instance = null;

        public event EventHandler SyncStarted;
        public event EventHandler SyncFinished;

        private HashSet<string> allFiles = new HashSet<string>();

        private BackgroundWorker bw = new BackgroundWorker();

#if DEBUG
        private bool loggingActive = false;
        string logFilename;
#endif

        private SynchronizeCatalogWorker()
        {
        }

        public static SynchronizeCatalogWorker Instance
        {
            get
            {
                if (instance == null)
                    throw new Exception("SynchronizeCatalogWorker not yet initialized!");
                return instance;
            }
        }

        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        public static void Create(DataBase db)
        {
            instance = new SynchronizeCatalogWorker();

            instance.dataBase = db;
        }

        /// <summary>
        /// Startet die Synchronisierung im Hintergrund.
        /// </summary>
        /// <param name="dataBase"></param>
        public void Start()
        {
            NumberOfItems = 0;
            NumberOfChangedItems = 0;

            bw.WorkerSupportsCancellation = true;

            // Wenn kein Verzeichnis angegeben ist, dann nichts machen.
            if (dataBase.Master.MonitoredDirectories.Count == 0)
                return;

#if DEBUG
            if (loggingActive)
            {
                logFilename = Path.Combine(Path.GetDirectoryName(dataBase.DataBasePath), "synclog.txt");
                if (File.Exists(logFilename))
                {
                    File.Delete(logFilename);
                }
            }
#endif

            if (SyncStarted != null)
                SyncStarted(this, new EventArgs());

            IsRunning = true;

            bw.DoWork += delegate
            {
                allFiles.Clear();

                bool allDirectoriesFound = true;

                // Zunächst die vorhandenen und neuen Dateien synchronisieren...
                foreach (string directory in dataBase.Master.MonitoredDirectories)
                {
                    if (Directory.Exists(directory))
                    {
                        SynchronizeDirectory(directory);
                    }
                    else
                    {
                        allDirectoriesFound = false;
                    }
                }

                // ... und jetzt noch die aus der Datenbank löschen, die es nicht mehr gibt.
                // nur machen, wenn die Verzeichnisse alle gefunden wurden, ansonsten wird eventuell zu viel gelöscht
                // Beispiel: Externe Festplatte, die nicht angeschlossen ist.
                if (allDirectoriesFound)
                {
                    SynchronizeDeletedTracks();
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                IsRunning = false;
                if (SyncFinished != null)
                    SyncFinished(this, new EventArgs());
            };
            bw.RunWorkerAsync();
        }

        private void SynchronizeDeletedTracks()
        {
            if (bw.CancellationPending)
                return;

            List<Tuple<string,int>> cdids = dataBase.GetAllMusicFiles();

            foreach (Tuple<string,int> track in cdids)
            {
                if (!allFiles.Contains(track.Item1))
                {
                    dataBase.DeleteTrack(track.Item2);
                }

                if (bw.CancellationPending)
                    return;
            }

            // Jetzt noch alle Alben löschen, für die es keinen Track mehr gibt
            dataBase.DeleteEmptyAlbums();
        }

        private void SynchronizeDirectory(string directory)
        {
            if (bw.CancellationPending)
                return;

            // Zuerst alle Unterverzeichnisse
            string[] subDirs = null;

            try
            {
                subDirs = Directory.GetDirectories(directory);
            }
            catch
            {
            }

            if (subDirs != null)
            {
                foreach (string subDir in subDirs)
                    SynchronizeDirectory(subDir);
            }

            // und dann die Dateien
            string[] filenames;

            try
            {
                filenames = Directory.GetFiles(directory);
            }
            catch
            {
                // Wahrscheinlich ein Verzeichnis, auf das wir keinen Zugriff haben.
                return;
            }

            foreach (string filename in filenames)
            {
                if (bw.CancellationPending)
                    break;

#if DEBUG
                Stopwatch sw = new Stopwatch();
                if (loggingActive)
                {
                    sw.Start();
                }
#endif
                bool added = ScanFile(filename);
#if DEBUG
                if (loggingActive)
                {
                    sw.Stop();
                    string log = string.Format("{0}\t{1}\t{2}\r\n", filename, sw.ElapsedMilliseconds, added);
                    File.AppendAllText(logFilename, log);
                }
#endif
            }
        }

        public bool ScanFile(string filename)
        {
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (SoundEngine.SoundEngine.IsSupportedFileType(filename) && fi.Length / 1024 >= Settings.Current.ManageSoundFilesMinSize)
                {
                    NumberOfItems++;

                    // Prüfen, ob sich die Datei geändert hat
                    //string sql = string.Format("Select SoundFileLastModified FROM Track where SoundFile = '{0}'", DataBase.SqlPrepare(filename));
                    //object lastModified = dataBase.ExecuteScalar(sql);
                    // Fehler umgehen: http://connect.microsoft.com/SQLServer/feedback/details/683114/getting-exception-the-data-area-passed-to-a-system-call-is-too-small-in-sql-server-compact-4
                    string sql = string.Format("Select SoundFileLastModified FROM Track where SoundFile = @param1");
                    object lastModified = dataBase.ExecuteScalarWithParameter(sql, filename);

                    allFiles.Add(filename);
                    
                    if (lastModified != null)
                    {
                        if (!(lastModified is DBNull) &&
                            (fi.LastWriteTime - (DateTime)lastModified).TotalSeconds < 1)
                        {
                            // Datei hat sich nicht geändert, also nichts tun.
                            return false;
                        }
                    }

                    // Prüfen, ob die Datei in einer CD refernziert ist (also nicht in einem MP3-Album!)
                    // Dann darf man damit auch nichts machen, da die Datei nicht synchronisiert wird.
                    if (dataBase.IsSoundfileNotSynchronized(filename))
                    {
                        return false;
                    }

                    SoundFileInformation sfInfo = SoundFileInformation.GetSoundFileInformation(filename);

                    NumberOfChangedItems++;

                    if (sfInfo.ID3Version == 0)
                    {
                        string artist = "";
                        string title = "";
                        Misc.GetAlbumInfoByFilename(filename, "- ", out artist, out title);
                        sfInfo.Artist = artist;
                        sfInfo.Title = title;
                    }

                    if (string.IsNullOrEmpty(sfInfo.Artist))
                        sfInfo.Artist = StringTable.UnknownArtist;

                    if (string.IsNullOrEmpty(sfInfo.Album))
                        sfInfo.Album = StringTable.UnknownAlbum;

                    // CD suchen (Interpret und Album)
                    int cdid = dataBase.GetCDIDByArtistAndTitle(!string.IsNullOrEmpty(sfInfo.AlbumArtist) ? sfInfo.AlbumArtist : sfInfo.Artist, sfInfo.Album);
                    if (cdid == 0)
                    {
                        // CD ist noch nicht vorhanden. Jetzt also erst mal anlegen.
                        CD newCD = new CD();
                        /*Zur Zeit nicht mehr! if (Settings.Current.ManageSoundFilesCreateOneCollection)
                        {
                            newCD.Artist = Settings.Current.ManageSoundFilesCreateOneCollectionTitle1;
                            newCD.Title = Settings.Current.ManageSoundFilesCreateOneCollectionTitle2;
                            newCD.Sampler = true;
                        }
                        else*/
                        {
                            newCD.Artist = sfInfo.AlbumArtist.Left(100);
                            if (string.IsNullOrEmpty(newCD.Artist))
                                newCD.Artist = sfInfo.Artist.Left(100);

                            if (sfInfo.AlbumArtist != sfInfo.Artist)
                                newCD.Sampler = true;

                            newCD.Title = sfInfo.Album.Left(100);
                            newCD.Category = sfInfo.Genre;
                            newCD.YearRecorded = sfInfo.Year;
                        }
                        newCD.Type = AlbumType.ManagedSoundFiles;
                        newCD.Tracks = new SafeObservableCollection<Track>();
                        newCD.Save(dataBase);

                        cdid = newCD.ID;
                    }

                    // Prüfen, ob der Track schon vorhanden ist
                    Track track;
                    Track trackFound = dataBase.GetTrackBySoundfile(filename);
                    if (trackFound != null)
                    {
                        track = trackFound;
                    }
                    else
                    {
                        track = new Track();
                    }

                    track.TrackNumber = sfInfo.TrackNumber;
                    track.Artist = sfInfo.Artist.Left(100);
                    track.Title = sfInfo.Title.Left(100);
                    track.Composer = sfInfo.Composer.Left(100);
                    track.YearRecorded = sfInfo.Year;
                    track.Category = sfInfo.Genre;
                    track.Soundfile = filename;
                    track.Length = sfInfo.Length;
                    track.Rating = sfInfo.Rating;
                    track.Language = sfInfo.Language;
                    track.PlayCount = sfInfo.PlayCount;
                    track.SoundFileLastModified = fi.LastWriteTime;
                    track.CDID = cdid;

                    if (sfInfo.Images != null && sfInfo.Images.Count > 0)
                    {
                        dataBase.UpdateFrontCover(cdid, sfInfo.Artist, sfInfo.Album, sfInfo.Images[0]);
                    }

                    track.Save(dataBase);

                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public void Cancel()
        {
            bw.CancelAsync();
        }
    }
}
