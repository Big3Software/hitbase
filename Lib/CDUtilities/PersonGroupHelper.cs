using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Controls;
using System.ComponentModel;
using System.Data;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.SoundEngine;
using System.IO;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.CDUtilities
{
    public class PersonGroupHelper
    {
        public delegate void UpdateSoundfilesCompletedDelegate();

        public static void UpdateSoundfiles(DataBase dataBase, int oldId, string oldPersonGroup, string newPersongroup, UpdateSoundfilesCompletedDelegate updateCompleted)
        {
            WaitProgressWindow waitProgress = new WaitProgressWindow();
            waitProgress.Show();
            waitProgress.progressControl.textBlockStatus.Text = StringTable.UpdatingPersonGroups;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                DataTable dt = dataBase.ExecuteFreeSql("SELECT Soundfile from Track inner join cd on track.cdid = cd.cdid WHERE track.artistID = " + oldId + " or track.composerId = " + oldId + " or cd.artistid = " + oldId + " or cd.composerid = " + oldId);

                waitProgress.Dispatcher.Invoke((Action)(() =>
                {
                    waitProgress.progressControl.progressBar.Maximum = dt.Rows.Count;
                }));

                foreach (DataRow row in dt.Rows)
                {
                    if (waitProgress.Canceled)
                        break;

                    string filename = row[0] as string;

                    if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                    {
                        SoundFileInformation sfi = SoundFileInformation.GetSoundFileInformation(filename);
                        if (string.Compare(sfi.Artist, oldPersonGroup, true) == 0)
                        {
                            sfi.Artist = newPersongroup;
                            SoundFileInformation.WriteMP3Tags(sfi, Field.ArtistTrackName);
                        }
                        if (string.Compare(sfi.Composer, oldPersonGroup, true) == 0)
                        {
                            sfi.Composer = newPersongroup;
                            SoundFileInformation.WriteMP3Tags(sfi, Field.ComposerTrackName);
                        }
                        if (string.Compare(sfi.AlbumArtist, oldPersonGroup, true) == 0)
                        {
                            sfi.AlbumArtist = newPersongroup;
                            SoundFileInformation.WriteMP3Tags(sfi, Field.ArtistCDName);
                        }
                    }

                    waitProgress.Dispatcher.Invoke((Action)(() =>
                    {
                        waitProgress.progressControl.progressBar.Value++;
                    }));
                }
            };
            bw.RunWorkerCompleted += delegate
            {
                waitProgress.Close();
                if (!waitProgress.Canceled)
                {
                    if (updateCompleted != null)
                        updateCompleted();
                }
            };
            bw.RunWorkerAsync();
        }
    }
}
