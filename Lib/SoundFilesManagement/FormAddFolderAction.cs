using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.CDUtilities;
using Big3.Hitbase.DataBaseEngine;
using System.IO;
using XPTable.Models;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.Configuration;
using System.Diagnostics;
using Big3.Hitbase.SoundEngine;
using System.Collections.ObjectModel;

namespace Big3.Hitbase.SoundFilesManagement
{
    public partial class FormAddFolderAction : Form
    {
        DataBase dataBase;
        string folder;
        Timer t = new Timer();
        CD newCD = new CD();

        public CD NewCD
        {
            get { return newCD; }
            set { newCD = value; }
        }
        bool canceled = false;

        Stopwatch sw = new Stopwatch();
        int count = 0;
        int countAll = 0;

        public FormAddFolderAction(DataBase db, string folder)
        {
            dataBase = db;
            this.folder = folder;

            InitializeComponent();

            newCD.Artist = Settings.Current.SamplerFixedArtistText;
            newCD.Sampler = true;

            Table.TableModel = new XPTable.Models.TableModel();
            Table.ColumnModel = new XPTable.Models.ColumnModel();
            Table.ColumnModel.Columns.Add(new XPTable.Models.TextColumn("Artist", 210));
            Table.ColumnModel.Columns.Add(new XPTable.Models.TextColumn("Album", 210));
            Table.ColumnModel.Columns.Add(new XPTable.Models.TextColumn("Title", 210));

            Tools.FormatXPTable(Table);

            t.Interval = 200;
            t.Enabled = true;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            t.Stop();

            StartSearch();
        }

        private void StartSearch()
        {
            count = 0;
            countAll = 0;

            sw.Start();

            // Zuerst die Anzahl der Dateien zählen
            SearchFiles(folder, true);

            progressBar1.Maximum = countAll;
            count = 0;
            countAll = 0;

            // Jetzt gehts los!
            if (canceled)
                return;

            SearchFiles(folder, false);

            if (string.IsNullOrEmpty(newCD.Title))
            {
                newCD.Title = Big3.Hitbase.SharedResources.StringTable.SoundCollection;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();

            buttonOK.Enabled = true;
        }

        private void SearchFiles(string directory, bool countOnly)
        {
            if (canceled)
                return;

            // Zuerst alle Unterverzeichnisse
            string[] subDirs = null;

            labelCurrentDirectory.Text = directory;

            if (sw.ElapsedMilliseconds >= 500)
            {
                Application.DoEvents();
                sw.Reset();
                sw.Start();
            }

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
                    SearchFiles(subDir, countOnly);
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
                if (canceled)
                    break;

                if (!countOnly)
                {
                    FileInfo fi = new FileInfo(filename);
                    if (SoundEngine.SoundEngine.IsSupportedFileType(filename))
                    {
                        SoundFileInformation sfInfo = SoundFileInformation.GetSoundFileInformation(filename);

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

                        //string cdKey;

                        //if (Settings.Current.ManageSoundFilesCreateOneCollection)
                        //    cdKey = "DUMMY";  // Damit wandern alle Tracks in ein Album
                        //else
                        //    cdKey = string.Format("{0}_{1}", sfInfo.Artist, sfInfo.Album);
                        CD cd;

                        //if (newCD.ContainsKey(cdKey))
                        {
                            cd = newCD;
                        }
                        /*else
                        {
                            CD newCD = new CD();
                            //if (Settings.Current.ManageSoundFilesCreateOneCollection)
                            {
                                newCD.Artist = Settings.Current.SamplerFixedArtistText;
                                newCD.Title = Big3.Hitbase.SharedResources.StringTable.SoundCollection;
                                newCD.Sampler = true;
                            }
                            else
                            {
                                newCD.Artist = sfInfo.Artist.Left(100);
                                newCD.Title = sfInfo.Album.Left(100);
                                newCD.Category = sfInfo.Genre;
                            }
                            newCD.Type = AlbumType.SoundFiles;
                            newCD.Tracks = new SafeObservableCollection<Track>();
                            newCDs.Add(cdKey, newCD);
                            cd = newCD;
                        }*/

                        Track track = new Track();
                        track.TrackNumber = sfInfo.TrackNumber;
                        track.Artist = sfInfo.Artist.Left(100);
                        track.Title = sfInfo.Title.Left(100);
                        track.Composer = sfInfo.Composer.Left(100);
                        track.Category = sfInfo.Genre;
                        track.Soundfile = filename;
                        track.Length = sfInfo.Length;
                        track.Rating = sfInfo.Rating;
                        track.Language = sfInfo.Language;
                        cd.TotalLength += track.Length;
                        cd.Tracks.Add(track);

                        if (string.IsNullOrEmpty(cd.Title))
                            cd.Title = sfInfo.Album;

                        count++;

                        Row row = new Row();
                        row.Cells.Add(new Cell(sfInfo.Artist));
                        row.Cells.Add(new Cell(sfInfo.Album));
                        row.Cells.Add(new Cell(sfInfo.Title));
                        int index = Table.TableModel.Rows.Add(row);
                        Table.EnsureVisible(index, 0);
                        labelAddedFiles.Text = count.ToString();
                        if (sw.ElapsedMilliseconds >= 500)
                        {
                            labelFoundFiles.Text = countAll.ToString();

                            Application.DoEvents();
                            sw.Reset();
                            sw.Start();
                        }

                    }

                    progressBar1.Value = countAll;
                }

                countAll++;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            
            /*foreach (CD cd in newCDs.Values)
            {
                cd.NumberOfTracks = cd.Tracks.Count;
                cd.Save(dataBase);
            }*/
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            canceled = true;
        }
    }
}
