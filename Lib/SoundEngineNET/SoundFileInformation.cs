using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Miscellaneous;
using System.IO;
using Big3.Hitbase.Configuration;
using Microsoft.Win32;
using Big3.Hitbase.DataBaseEngine;
using System.ComponentModel;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.SoundEngine
{
    public class SoundFileInformation : INotifyPropertyChanged
    {
        /// <summary>
        /// Der Interpret des Tracks
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Der Titel des Tracks
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Die Länge des Tracks in Millisekunden
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Der Dateiname des Tracks
        /// </summary>
        public string Filename { get; set; }

        public string Album { get; set; }

        /// <summary>
        /// Der Interpret des Albums
        /// </summary>
        public string AlbumArtist { get; set; }

        public string Composer { get; set; }
        public string Comment { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public int TrackNumber { get; set; }
        public int Rating { get; set; }
        public string Language { get; set; }
        public string Lyrics { get; set; }
        public int BPM { get; set; }

        public int PlayCount { get; set; }

        public List<byte[]> Images { get; set; }

        public string TitleAndArtist
        {
            get
            {
                return Title + " - " + Artist;
            }
        }

        /// <summary>
        /// Die Version der ID3-Tags (0 = keine gefunden, 1 = ID3v1, 2 = ID3v2)
        /// </summary>
        public int ID3Version { get; set; }

        /// <summary>
        /// Startposition des Tracks, falls es ein Track einer CD ist, ansonsten ist das Feld nicht gefüllt.
        /// </summary>
        public int StartPosition { get; set; }

        public static SoundFileInformation GetSoundFileInformation(string filename)
        {
            SoundFileInformation soundFileInfo = new SoundFileInformation();

            GetSoundFileInformation(soundFileInfo, filename);

            return soundFileInfo;
        }

        public static void GetSoundFileInformation(SoundFileInformation soundFileInfo, string filename)
        {
            soundFileInfo.Filename = filename;

            try
            {
                if (string.IsNullOrEmpty(filename) || !System.IO.File.Exists(filename))
                    return ;

                string ext = Path.GetExtension(filename).ToLower();

                using (TagLib.File tagFile = TagLib.File.Create(filename))
                {
                    if (tagFile.Tag.Performers.Length > 0)
                    {
                        soundFileInfo.Artist = string.Join(";", tagFile.Tag.Performers).Trim();
                        // The AC/DC exception
                        if (soundFileInfo.Artist.Contains("AC;DC"))
                        {
                            soundFileInfo.Artist = soundFileInfo.Artist.Replace("AC;DC", "AC/DC");
                        }
                    }

                    if (tagFile.Tag.FirstAlbumArtist != null)
                        soundFileInfo.AlbumArtist = tagFile.Tag.FirstAlbumArtist.Trim();

                    if (tagFile.Tag.Album != null)
                        soundFileInfo.Album = tagFile.Tag.Album.Trim();

                    if (tagFile.Tag.Title != null)
                        soundFileInfo.Title = tagFile.Tag.Title.Trim();

                    if (tagFile.Tag.Comment != null)
                        soundFileInfo.Comment = tagFile.Tag.Comment.Trim();

                    soundFileInfo.Year = (int)tagFile.Tag.Year;
                    soundFileInfo.TrackNumber = (int)tagFile.Tag.Track;

                    soundFileInfo.Genre = tagFile.Tag.FirstGenre;

                    if (tagFile.Tag.FirstComposer != null)
                        soundFileInfo.Composer = tagFile.Tag.FirstComposer.Trim();
                    // TODO!!!
                    //soundFileInfo.Language = tagFile.Tag.Language.Trim();

                    /*if (id3Info.ID3v2Info.PlayCounter != null)
                    {
                        soundFileInfo.PlayCount = (int)id3Info.ID3v2Info.PlayCounter.Counter;
                    }*/

                    if (tagFile.Tag.Lyrics != null)
                        soundFileInfo.Lyrics = tagFile.Tag.Lyrics.Trim();


                    // Rating ist eine Liste von Einträgen. Wir suchen hier nach dem Eintrag "info@hitbase.de".
                    TagLib.Id3v2.Tag id3v2Tag = tagFile.GetTag(TagLib.TagTypes.Id3v2) as TagLib.Id3v2.Tag;
                    if (id3v2Tag != null)
                    {
                        TagLib.Id3v2.PopularimeterFrame popFrame = TagLib.Id3v2.PopularimeterFrame.Get(id3v2Tag, "info@hitbase.de", false);

                        if (popFrame != null)
                        {
                            soundFileInfo.Rating = popFrame.Rating;
                            soundFileInfo.PlayCount = (int)popFrame.PlayCount;
                        }
                    }

                    soundFileInfo.BPM = (int)tagFile.Tag.BeatsPerMinute;

                    soundFileInfo.Length = (int)tagFile.Properties.Duration.TotalMilliseconds;

                    soundFileInfo.Images = new List<byte[]>();
                    if (tagFile.Tag.Pictures.Length > 0)
                    {
                        foreach (TagLib.IPicture picture in tagFile.Tag.Pictures)
                        {
                            soundFileInfo.Images.Add(picture.Data.ToArray());
                        }
                    }

                    soundFileInfo.ID3Version = 2;

                    if (soundFileInfo.Length == 0)
                    {
                        // Wenn in den ID3-Tags nichts drin stand, dann ermitteln wir die Länge über diesen Weg.
                        soundFileInfo.Length = SoundEngine.GetLengthOfSoundfile(filename);
                    }
                }
            }
            catch
            {
                // Zuerst mal hier alle Fehler ignorieren.
            }
        }

        public static SoundFileInformation GetSoundFileInformation(DataBase db, Track track)
        {
            SoundFileInformation soundFileInfo = new SoundFileInformation();

            if (track.CD != null)
            {
                soundFileInfo.Album = track.CD.Title;
                if (!track.CD.Sampler)
                {
                    soundFileInfo.Artist = track.CD.Artist;
                }
            }

            soundFileInfo.Artist = track.Artist;
            soundFileInfo.Title = track.Title;
            soundFileInfo.Rating = track.Rating;
            soundFileInfo.StartPosition = track.StartPosition;
            soundFileInfo.Length = track.Length;
            soundFileInfo.Filename = track.Soundfile;
            try
            {
                string cdcoverFrontFilename = db.GetFrontCoverByCdId(track.CDID);
                if (!string.IsNullOrEmpty(cdcoverFrontFilename))
                {
                    byte[] filefrontcover = File.ReadAllBytes(Misc.FindCover(cdcoverFrontFilename));
                    soundFileInfo.Images = new List<byte[]>();
                    soundFileInfo.Images.Add(filefrontcover);
                }
            }
            catch { }

            return soundFileInfo;
        }

        public static void FillAlbumAndTrackFromSoundFileInformation(CD cd, Track track, SoundFileInformation sfi)
        {
            track.Artist = sfi.Artist;
            track.Title = sfi.Title;
            track.Category = sfi.Genre;
            track.Comment = sfi.Comment;
            track.Composer = sfi.Composer;
            track.Lyrics = sfi.Lyrics;
            track.Rating = sfi.Rating;
            track.Length = sfi.Length;
            track.TrackNumber = sfi.TrackNumber;
            track.StartPosition = 0;
            track.Bpm = sfi.BPM;
            track.CD = cd;
            track.Soundfile = sfi.Filename;

            cd.Artist = sfi.AlbumArtist;
            cd.Title = sfi.Album;
            
        }

        private static string MapID3GenreToHitbaseGenre(byte id3Genre)
        {
            string[] genres = { 
        /* 00 */ "Blues",
        /* 01 */ "Classic Rock",
        /* 02 */ "Country",
        /* 03 */ "Dance",
        /* 04 */ "Disco",
        /* 05 */ "Funk",
        /* 06 */ "Grunge",
        /* 07 */ "Hip-Hop",
        /* 08 */ "Jazz",
        /* 09 */ "Metal",
        /* 10 */ "New Age",
        /* 11 */ "Oldies",
        /* 12 */ "Other",
        /* 13 */ "Pop",
        /* 14 */ "R&B",
        /* 15 */ "Rap",
        /* 16 */ "Reggae",
        /* 17 */ "Rock",
        /* 18 */ "Techno",
        /* 19 */ "Industrial",
        /* 20 */ "Alternative",
        /* 21 */ "Ska",
        /* 22 */ "Death Metal",
        /* 23 */ "Pranks",
        /* 24 */ "Soundtrack",
        /* 25 */ "Euro-Techno",
        /* 26 */ "Ambient",
        /* 27 */ "Trip-Hop",
        /* 28 */ "Vocal",
        /* 29 */ "Jazz&Funk",
        /* 30 */ "Fusion",
        /* 31 */ "Trance",
        /* 32 */ "Classical",
        /* 33 */ "Instrumental",
        /* 34 */ "Acid",
        /* 35 */ "House",
        /* 36 */ "Game",
        /* 37 */ "Sound Clip",
        /* 38 */ "Gospel",
        /* 39 */ "Noise",
        /* 40 */ "Alternative Rock",
        /* 41 */ "Bass",
        /* 42 */ "Soul",
        /* 43 */ "Punk",
        /* 44 */ "Space",
        /* 45 */ "Meditative",
        /* 46 */ "Instrumental Pop",
        /* 47 */ "Instrumental Rock",
        /* 48 */ "Ethnic",
        /* 49 */ "Gothic",
        /* 50 */ "Darkwave",
        /* 51 */ "Techno-Industrial",
        /* 52 */ "Electronic",
        /* 53 */ "Pop-Folk",
        /* 54 */ "Eurodance",
        /* 55 */ "Dream",
        /* 56 */ "Southern Rock",
        /* 57 */ "Comedy",
        /* 58 */ "Cult",
        /* 59 */ "Gangsta",
        /* 60 */ "Top 40",
        /* 61 */ "Christian Rap",
        /* 62 */ "Pop-Funk",
        /* 63 */ "Jungle",
        /* 64 */ "Native US",
        /* 65 */ "Cabaret",
        /* 66 */ "New Wave",
        /* 67 */ "Psychedelic",
        /* 68 */ "Rave",
        /* 69 */ "Showtunes",
        /* 70 */ "Trailer",
        /* 71 */ "Lo-Fi",
        /* 72 */ "Tribal",
        /* 73 */ "Acid Punk",
        /* 74 */ "Acid Jazz",
        /* 75 */ "Polka",
        /* 76 */ "Retro",
        /* 77 */ "Musical",
        /* 78 */ "Rock & Roll",
        /* 79 */ "Hard Rock",
        /* 80 */ "Folk",
        /* 81 */ "Folk-Rock",
        /* 82 */ "National Folk",
        /* 83 */ "Swing",
        /* 84 */ "Fast Fusion",
        /* 85 */ "Bebop",
        /* 86 */ "Latin",
        /* 87 */ "Revival",
        /* 88 */ "Celtic",
        /* 89 */ "Bluegrass",
        /* 90 */ "Avantgarde",
        /* 91 */ "Gothic Rock",
        /* 92 */ "Progressive Rock",
        /* 93 */ "Psychedelic Rock",
        /* 94 */ "Symphonic Rock",
        /* 95 */ "Slow Rock",
        /* 96 */ "Big Band",
        /* 97 */ "Chorus",
        /* 98 */ "Easy Listening",
        /* 99 */ "Acoustic",
        /* 100 */ "Humour",
        /* 101 */ "Speech",
        /* 102 */ "Chanson",
        /* 103 */ "Opera",
        /* 104 */ "Chamber Music",
        /* 105 */ "Sonata",
        /* 106 */ "Symphony",
        /* 107 */ "Booty Bass",
        /* 108 */ "Primus",
        /* 109 */ "Porn Groove",
        /* 110 */ "Satire",
        /* 111 */ "Slow Jam",
        /* 112 */ "Club",
        /* 113 */ "Tango",
        /* 114 */ "Samba (Musik)",
        /* 115 */ "Folklore",
        /* 116 */ "Ballad",
        /* 117 */ "Power Ballad",
        /* 118 */ "Rhytmic Soul",
        /* 119 */ "Freestyle",
        /* 120 */ "Duet",
        /* 121 */ "Punk Rock",
        /* 122 */ "Drum Solo",
        /* 123 */ "Acapella",
        /* 124 */ "Euro-House",
        /* 125 */ "Dance Hall",
        /* 126 */ "Goa",
        /* 127 */ "Drum & Bass",
        /* 128 */ "Club-House",
        /* 129 */ "Hardcore",
        /* 130 */ "Terror",
        /* 131 */ "Indie",
        /* 132 */ "BritPop",
        /* 133 */ "Negerpunk",
        /* 134 */ "Polsk Punk",
        /* 135 */ "Beat",
        /* 136 */ "Christian Gangsta Rap",
        /* 137 */ "Heavy Metal",
        /* 138 */ "Black Metal",
        /* 139 */ "Crossover",
        /* 140 */ "Contemporary Christian",
        /* 141 */ "Christian Rock",
        /* 142 */ "Merengue",
        /* 143 */ "Salsa",
        /* 144 */ "Thrash Metal",
        /* 145 */ "Anime",
        /* 146 */ "JPop",
        /* 147 */ "Synthpop"
	        };

            if (id3Genre > 147)
                return "";

            return genres[(int)id3Genre];
        }

        public static void WriteMP3Tags(string filename, CD cd, int track)
        {
            // Im Moment nur MP3
            string ext = Path.GetExtension(filename).ToLower();
            if (ext != ".mp3" && ext != ".flac" && ext != ".wma" && ext != ".ogg")
                return;

            using (TagLib.File tagFile = TagLib.File.Create(filename))
            {
                int id3Version = Settings.Current.UseID3Version;

                int year = cd.Tracks[track].YearRecorded != 0 ? cd.Tracks[track].YearRecorded : cd.YearRecorded;
                string category = !String.IsNullOrEmpty(cd.Tracks[track].Category) ? cd.Tracks[track].Category : cd.Category;
                string language = !String.IsNullOrEmpty(cd.Tracks[track].Language) ? cd.Tracks[track].Language : cd.Language;

                tagFile.Tag.Performers = new string[] { cd.Tracks[track].Artist };
                tagFile.Tag.AlbumArtists = new string[] { cd.Artist };
                tagFile.Tag.Title = cd.Tracks[track].Title;
                tagFile.Tag.Comment = cd.Tracks[track].Comment;

                tagFile.Tag.Track = (uint)cd.Tracks[track].TrackNumber;

                tagFile.Tag.Album = cd.Title;
                tagFile.Tag.Year = (uint)year;

                if (category != null)
                    tagFile.Tag.Genres = new string[] { category };

                tagFile.Tag.Composers = new string[] { cd.Tracks[track].Composer };
                //TODO!!!!!!!!!!!!!id3Info.ID3v2Info.SetTextFrame("TLAN", language);

                if (cd.CDSetNumber > 0)
                {
                    tagFile.Tag.Disc = (uint)cd.CDSetNumber;
                }

                //TODO!!!!!!!!id3Info.ID3v2Info.SetTextFrame("PCNT", cd.Tracks[track].PlayCount.ToString());

                // Rating ist eine Liste von Einträgen. Wir suchen hier nach dem Eintrag "info@hitbase.de".
                if (cd.Tracks[track].Rating > 0 && ext == ".mp3")
                {
                    TagLib.Id3v2.PopularimeterFrame popFrame = TagLib.Id3v2.PopularimeterFrame.Get((TagLib.Id3v2.Tag)tagFile.GetTag(TagLib.TagTypes.Id3v2), "info@hitbase.de", true);

                    popFrame.User = "info@hitbase.de";
                    popFrame.Rating = (byte)cd.Tracks[track].Rating;
                    popFrame.PlayCount = (ulong)cd.Tracks[track].PlayCount;
                }


                tagFile.Tag.BeatsPerMinute = (uint)cd.Tracks[track].Bpm;

                //TODO!!!!!!!!!!!!!tagFile.Tag.id3Info.ID3v2Info.SetTextFrame("TLEN", cd.Tracks[track].Length > 0 ? cd.Tracks[track].Length.ToString() : "");
                try
                {
                    if (!string.IsNullOrEmpty(cd.CDCoverFrontFilename))
                    {
                        byte[] filefrontcover = File.ReadAllBytes(Misc.FindCover(cd.CDCoverFrontFilename));
                        string mimetype;
                        mimetype = GetMIMEType(Path.GetExtension(cd.CDCoverFrontFilename));
                        MemoryStream memstream = new MemoryStream(filefrontcover);

                        TagLib.Picture pict = new TagLib.Picture();
                        pict.MimeType = mimetype;
                        pict.Data = memstream.ToArray();
                        pict.Type = TagLib.PictureType.FrontCover;
                        tagFile.Tag.Pictures = new TagLib.IPicture[] { pict };
                    }
                }
                catch
                {
                    // Pech gehabt - kein Cover
                }

                tagFile.Tag.Lyrics = cd.Tracks[track].Lyrics;

                try
                {
                    tagFile.Save();
                }
                catch (Exception e)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                     new Action(delegate
                     {
                         WPFMessageBox msgBox = new WPFMessageBox();
                         msgBox.HeaderText = StringTable.WriteID3TagsAccessDeniedTitle;
                         msgBox.Text = string.Format(StringTable.WriteID3TagsAccessDenied, filename);
                         msgBox.Image = "/Big3.Hitbase.SharedResources;component/Images/Info.png";
                         msgBox.Owner = System.Windows.Application.Current.MainWindow;
                         msgBox.WpfMessageBoxButtons = WpfMessageBoxButtons.OK;
                         msgBox.ZoomImage = false;
                         msgBox.Width = 600;
                         msgBox.Height = 400;
                         msgBox.ShowInTaskbar = false;
                         msgBox.DontShowAgainText = StringTable.DontDisplayAgain;
                         msgBox.SaveAnswer = true;
                         msgBox.SaveAnswerInRegistryKey = "ID3RenameAccessDenied";
                         msgBox.ShowDialogEventually();
                     }));
                }
            }
        }

        public static string GetMIMEType(string Extension)
        {
            RegistryKey RK = Registry.ClassesRoot.OpenSubKey(Extension);
            if (RK == null)
                return "";
            else
                return RK.GetValue("Content Type").ToString();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public static void WriteMP3Tags(CD cd, int trackId = 0)
        {
            for (int i=0;i<cd.Tracks.Count;i++)
            {
                try
                {
                    if ((trackId == 0 || cd.Tracks[i].ID == trackId) &&
                        !string.IsNullOrEmpty(cd.Tracks[i].Soundfile) && File.Exists(cd.Tracks[i].Soundfile))
                    {
                        WriteMP3Tags(cd.Tracks[i].Soundfile, cd, i);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                     new Action(delegate
                     {
                         // Sicherheitshalber mal abgefragt
                         UnhandledExceptionWindow unhandledExceptionWindow = new UnhandledExceptionWindow(e);

                         unhandledExceptionWindow.ShowDialog();
                     }));
                }
            }
        }

        public void SetField(Field field, string newValue)
        {
            switch (field)
            {
                case Field.ArtistCDName:
                    AlbumArtist = newValue;
                    break;
                case Field.ArtistTrackName:
                    Artist = newValue;
                    break;
                case Field.ComposerTrackName:
                    Composer = newValue;
                    break;
                case Field.TrackTitle:
                    Title = newValue;
                    break;
                case Field.Title:
                    Album = newValue;
                    break;
                case Field.TrackCategory:
                    Genre = newValue;
                    break;
                case Field.TrackNumber:
                    TrackNumber = Misc.Atoi(newValue);
                    break;
                case Field.TrackYearRecorded:
                    Year = Misc.Atoi(newValue);
                    break;
                case Field.TrackComment:
                    Comment = newValue;
                    break;
                case Field.TrackRating:
                    Rating = Misc.Atoi(newValue);
                    break;
            }
        }

        public static bool CanChangeID3Tag(Field field)
        {
            switch (field)
            {
                case Field.ArtistTrackName:
                case Field.Title:
                case Field.TrackNumber:
                case Field.TrackCategory:
                case Field.ArtistCDName:
                case Field.ComposerTrackName:
                case Field.TrackTitle:
                case Field.TrackYearRecorded:
                case Field.TrackComment:
                case Field.TrackRating:
                    return true;
            }

            return false;
        }

        public static void WriteMP3Tags(SoundFileInformation sfi, Field field)
        {
            // Im Moment nur MP3
            string ext = Path.GetExtension(sfi.Filename).ToLower();
            if (ext != ".mp3" && ext != ".flac" && ext != ".wma" && ext != ".ogg")
                return;

            using (TagLib.File tagFile = TagLib.File.Create(sfi.Filename))
            {
                int id3Version = Settings.Current.UseID3Version;

                switch (field)
                {
                    case Field.ArtistTrackName:
                        {
                            tagFile.Tag.Performers = new string[] { sfi.Artist };
                            break;
                        }
                    case Field.Title:
                        {
                            tagFile.Tag.Album = sfi.Album;
                            break;
                        }
                    case Field.TrackNumber:
                        {
                            tagFile.Tag.Track = (uint)sfi.TrackNumber;
                        }

                        break;
                    case Field.TrackCategory:
                        {
                            tagFile.Tag.Genres = new string[] { sfi.Genre };
                            break;
                        }

                    case Field.ArtistCDName:
                        {
                            tagFile.Tag.AlbumArtists = new string[] { sfi.AlbumArtist };
                            break;
                        }
                    case Field.ComposerTrackName:
                        {
                            tagFile.Tag.Composers = new string[] { sfi.Composer };
                            break;
                        }
                    case Field.TrackTitle:
                        {
                            tagFile.Tag.Title = sfi.Title;
                            break;
                        }
                    case Field.TrackYearRecorded:
                        {
                            tagFile.Tag.Year = (uint)sfi.Year;
                            break;
                        }
                    case Field.TrackComment:
                        {
                            tagFile.Tag.Comment = sfi.Comment;
                            break;
                        }
                    case Field.TrackRating:
                        {
                            if (sfi.Rating > 0 && ext == ".mp3")
                            {
                                TagLib.Id3v2.PopularimeterFrame popFrame = TagLib.Id3v2.PopularimeterFrame.Get((TagLib.Id3v2.Tag)tagFile.GetTag(TagLib.TagTypes.Id3v2), "info@hitbase.de", true);

                                popFrame.User = "info@hitbase.de";
                                popFrame.Rating = (byte)sfi.Rating;
                                popFrame.PlayCount = (ulong)sfi.PlayCount;
                            }                            
                            break;
                        }

                    case Field.TrackPlayCount:
                        {
                            /*id3Info.ID3v2Info.PlayCounter = new ID3.ID3v2Frames.OtherFrames.PlayCounterFrame((FrameFlags)0);

                            id3Info.ID3v2Info.PlayCounter.Counter = sfi.PlayCount;*/

                            break;
                        }
                }

                tagFile.Save();
            }
        }
    }
}
