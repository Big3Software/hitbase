using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.DataBaseEngine
{
    public class CDArchiveLocalFile
    {
        public static bool SearchCDInCDArchiveLocalFile(string cdarchivFilename, CD cd)
        {
            string sql;
            sql = string.Format("SELECT * FROM ((Track INNER JOIN CD ON Track.IDCD = CD.ID) INNER JOIN Artist AS ArtistTrack ON Track.IDArtist = ArtistTrack.ID) INNER JOIN Artist ON CD.IDArtist = Artist.ID WHERE (((CD.sIdentity)='{0}')) ORDER BY Track.wTrackNumber", cd.Identity);

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source='" + cdarchivFilename + "'"))
            {
                conn.Open();

                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(sql, conn);

                using (System.Data.OleDb.OleDbDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())      // CD nicht vorhanden
                    {
                        return false;
                    }

                    cd.Artist = (string)reader["Artist.sArtist"];
                    cd.Title = (string)reader["CD.sTitle"];
                    cd.TotalLength = (int)reader["dwTotalLength"];
                    cd.Sampler = ((byte)reader["bCDSampler"]) == 1 ? true : false;
                    cd.NumberOfTracks = (int)(byte)reader["cNumberOfTracks"];
                    cd.InitTracks(cd.NumberOfTracks);
                    if ((Settings.Current.CDArchiveFields & CDArchiveFields.Category) == CDArchiveFields.Category)
                        cd.Category = (string)reader["sCategory"];
                    if ((Settings.Current.CDArchiveFields & CDArchiveFields.Medium) == CDArchiveFields.Medium)
                        cd.Medium = (string)reader["sMedium"];
                    if ((Settings.Current.CDArchiveFields & CDArchiveFields.Comment) == CDArchiveFields.Comment)
                        cd.Comment = (string)reader["CD.sComment"];
                    cd.YearRecorded = reader["CD.dwYearRecorded"] is DBNull ? 0 : (int)reader["CD.dwYearRecorded"];
                    cd.Copyright = reader["sCopyright"] is DBNull ? "" : (string)reader["sCopyright"];

                    // Neue Felder. Ich frag hier die Exception ab, damit ein altes CD-Archiv auch noch klappt
                    try
                    {
                        cd.UPC = reader["sUPC"] is DBNull ? "" : (string)reader["sUPC"];
                        cd.Label = reader["sLabel"] is DBNull ? "" : (string)reader["sLabel"];
                    }
                    catch
                    {
                    }

                    if (cd.Sampler && Settings.Current.SamplerUseFixedArtist)
                    {
                        cd.Artist = Settings.Current.SamplerFixedArtistText;
                    }

                    for (int i = 0; i < cd.NumberOfTracks; i++)
                    {
                        cd.Tracks[i].Artist = (string)reader["ArtistTrack.sArtist"];
                        cd.Tracks[i].Title = (string)reader["Track.sTitle"];
                        cd.Tracks[i].TrackNumber = (short)reader["wTrackNumber"];
                        cd.Tracks[i].Length = (int)reader["dwLength"];
                        if ((Settings.Current.CDArchiveFields & CDArchiveFields.BPM) == CDArchiveFields.BPM)
                            cd.Tracks[i].Bpm = (short)reader["wBpm"];

                        if ((Settings.Current.CDArchiveFields & CDArchiveFields.TrackComment) == CDArchiveFields.TrackComment && !(reader["Track.sComment"] is DBNull))
                            cd.Tracks[i].Comment = (string)reader["Track.sComment"];

                        cd.Tracks[i].YearRecorded = reader["Track.dwYearRecorded"] is DBNull ? 0 : (int)reader["Track.dwYearRecorded"];

                        reader.Read();
                    }
                }
            }

            return true;
        }


    }
}
