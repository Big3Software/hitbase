using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Configuration;
using System.IO;
using ID3;
using Big3.Hitbase.Miscellaneous;
using ID3.ID3v2Frames.BinaryFrames;
using Microsoft.Win32;

namespace Big3.Hitbase.SoundEngine2
{
    public class SoundEngine
    {
        public static FMOD.System FMODSystem;
        public static FMOD.Sound FMODSound;
        public static FMOD.Channel FMODChannel;
        public static int CurrentOutputDevicePreListen = -1;

        static SoundEngine()
        {
            InitSoundEngine();
        }

        private static void InitSoundEngine()
        {
            if (FMODSystem != null)
                FMODSystem.release();

            FMOD.Factory.System_Create(ref FMODSystem);
            FMODSystem.setDriver(Settings.Current.OutputDevicePreListen);
            FMODSystem.setDSPBufferSize(500, 8);
            CurrentOutputDevicePreListen = Settings.Current.OutputDevicePreListen;
            FMODSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)0);
        }

        public static string[] GetOutputDevices()
        {
            List<string> outputDevices = new List<string>();

            int numdrivers = 0;
            SoundEngine.FMODSystem.getNumDrivers(ref numdrivers);

            for (int i = 0; i < numdrivers; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Capacity = 1000;
                FMOD.GUID dummy = new FMOD.GUID();
                SoundEngine.FMODSystem.getDriverInfo(i, sb, 1000, ref dummy);
                outputDevices.Add(sb.ToString());
            }

            return outputDevices.ToArray();
        }

        public static void Play(string filename)
        {
            if (IsPlaying())
                Stop();

            // Wenn sich das Ausgabegerät geändert hat, neu initialisieren
            if (CurrentOutputDevicePreListen != Settings.Current.OutputDevicePreListen)
                InitSoundEngine();

            FMOD.RESULT res = SoundEngine.FMODSystem.createStream(filename, FMOD.MODE.OPENONLY, ref FMODSound);

            if (res == FMOD.RESULT.OK)
            {
                FMODSystem.playSound(FMOD.CHANNELINDEX.FREE, FMODSound, false, ref FMODChannel);
            }
        }

        public static void Stop()
        {
            if (SoundEngine.IsPlaying())
            {
                FMODChannel.stop();
                FMODSound.release();
                FMODChannel = null;
                FMODSound = null;
            }
        }

        public static void Pause(bool paused)
        {
            if (SoundEngine.IsPlaying())
            {
                FMODChannel.setPaused(paused);
            }
        }

        public static bool IsPaused()
        {
            bool paused = false;

            if (SoundEngine.IsPlaying())
            {
                FMODChannel.getPaused(ref paused);
            }

            return paused;
        }

        public static bool IsPlaying()
        {
            if (FMODChannel != null)
            {
                bool isPlaying = false;
                FMODChannel.isPlaying(ref isPlaying);
                return isPlaying;
            }

            return false;
        }

        public static int GetPlayPosition()
        {
            if (!IsPlaying())
                return 0;

            uint position = 0;
            FMODChannel.getPosition(ref position, FMOD.TIMEUNIT.MS);

            return (int)position;
        }

        public static void SetPlayPosition(int position)
        {
            if (IsPlaying())
            {
                FMODChannel.setPosition((uint)position, FMOD.TIMEUNIT.MS);
            }
        }

        /// <summary>
        /// Liefert TRUE zurück, wenn es sich um ein unterstütztes Sound-Format handelt.
        /// Dabei wird nur die Extension geprüft.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsSupportedFileType(string filename)
        {
	        string ext = Path.GetExtension(filename).ToLower();

        	if (ext == ".mp3" || ext == ".wav" || ext == ".wma" || ext == ".asf" || ext == ".ogg" ||
		        ext == ".mp2" || ext == ".mpg" || ext == ".mpeg" || ext == ".ogm" || ext == ".mp4" || ext == ".lnk" ||
		        ext == ".flac")
		        return true;

        	return false;
        }

        public static SoundFileInformation GetSoundFileInformation(string filename)
        {
            SoundFileInformation sfInfo = new SoundFileInformation();

            try
            {
                if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                    return sfInfo;

                ID3Info id3Info = new ID3Info(filename, true);

                int id3Version = Settings.Current.UseID3Version;

                if (id3Info.ID3v1Info != null && id3Info.ID3v1Info.HaveTag && id3Version != 1)
                {
                    sfInfo.Artist = id3Info.ID3v1Info.Artist.Trim();
                    sfInfo.Title = id3Info.ID3v1Info.Title.Trim();
                    sfInfo.Comment = id3Info.ID3v1Info.Comment.Trim();
                    sfInfo.Album = id3Info.ID3v1Info.Album.Trim();
                    sfInfo.Year = id3Info.ID3v1Info.Year.Trim();
                    sfInfo.Genre = MapID3GenreToHitbaseGenre(id3Info.ID3v1Info.Genre);
                    sfInfo.TrackNumber = id3Info.ID3v1Info.TrackNumber;
                    sfInfo.ID3Version = 1;
                }

                if (id3Info.ID3v2Info != null && id3Info.ID3v2Info.HaveTag && id3Version != 0)
                {
                    sfInfo.Artist = id3Info.ID3v2Info.GetTextFrame("TPE1").Trim();
                    sfInfo.Title = id3Info.ID3v2Info.GetTextFrame("TIT2").Trim();
                    sfInfo.Comment = id3Info.ID3v2Info.GetTextFrame("COMM").Trim();
                    sfInfo.Album = id3Info.ID3v2Info.GetTextFrame("TALB").Trim();
                    sfInfo.Year = id3Info.ID3v2Info.GetTextFrame("TYER").Trim();
                    String trackNumber = id3Info.ID3v2Info.GetTextFrame("TRCK");
                    sfInfo.TrackNumber = Misc.Atoi(trackNumber);
                    sfInfo.Genre = id3Info.ID3v2Info.GetTextFrame("TCON").Trim();
                    sfInfo.Composer = id3Info.ID3v2Info.GetTextFrame("TCOM").Trim();
                    sfInfo.Language = id3Info.ID3v2Info.GetTextFrame("TLAN").Trim();

                    // Das mit dem Rating geht irgendwie anders. Erst mal raus
                    //string rating = id3Info.ID3v2Info.GetTextFrame("POPM").Trim();
                    //sfInfo.Rating = Misc.Atoi(rating);

                    string bpm = id3Info.ID3v2Info.GetTextFrame("TBPM").Trim();
                    sfInfo.BPM = Misc.Atoi(bpm);

                    string length = id3Info.ID3v2Info.GetTextFrame("TLEN");
                    if (Misc.Atoi(length) > 0)
                        sfInfo.Length = Misc.Atoi(length);
                    
                    sfInfo.Images = new List<byte[]>();
                    if (id3Info.ID3v2Info.AttachedPictureFrames.Items.Length > 0)
                    {
                        ID3.ID3v2Frames.BinaryFrames.BinaryFrame binaryFrame = id3Info.ID3v2Info.AttachedPictureFrames.Items[0];
                        foreach (AttachedPictureFrame apf in id3Info.ID3v2Info.AttachedPictureFrames.Items)
                        {
                            sfInfo.Images.Add(apf.Data.ToArray());
                        }
                    }

                    sfInfo.ID3Version = 2;
                }

                if (sfInfo.Length == 0)
                {
                    FMOD.Sound sound = null;
                    FMOD.RESULT res = SoundEngine.FMODSystem.createStream(filename, FMOD.MODE.OPENONLY, ref sound);
                    uint length = 0;
                    if (sound != null)
                    {
                        sound.getLength(ref length, FMOD.TIMEUNIT.MS);
                        sound.release();

                        sfInfo.Length = (int)length;
                    }
                }
            }
            catch
            {
                // Zuerst mal hier alle Fehler ignorieren.
            }

            return sfInfo;
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
        /* 62 */ "Pop/Funk",
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
        /* 125 */ "Dance Hall"
	        };

	        if (id3Genre > 125)
		        return "";

	        return genres[(int)id3Genre];
        }

        public static void WriteMP3Tags(string filename, CD cd, int track)
        {
			ID3Info id3Info = new ID3Info(filename, true);

			int id3Version = Settings.Current.UseID3Version;

            int year = cd.Tracks[track].YearRecorded != 0 ? cd.Tracks[track].YearRecorded : cd.YearRecorded;

            if (id3Version == 0 || id3Version == 2)
			{
				// HACK JUS 08.04.2007: Damit ID3v2 Tags auch geschrieben werden, wenn kein ID3v2 Header vorhanden ist!
				// Ab und zu mal schauen, ob es vielleicht eine neue Version der Lib gibt
				// http://www.codeproject.com/cs/library/Do_Anything_With_ID3.asp
				id3Info.ID3v1Info.HaveTag = true;		

				id3Info.ID3v1Info.Artist = cd.Sampler ? cd.Tracks[track].Artist : cd.Artist;
				id3Info.ID3v1Info.Title = cd.Tracks[track].Title;
				id3Info.ID3v1Info.Comment = cd.Comment;
				id3Info.ID3v1Info.TrackNumber = (byte)(track + 1);
				id3Info.ID3v1Info.Album = cd.Title;

				id3Info.ID3v1Info.Year = year != 0 ? year.ToString() : "";
			}

			if (id3Version == 1 || id3Version == 2)
			{
				// HACK JUS 08.04.2007: Damit ID3v2 Tags auch geschrieben werden, wenn kein ID3v2 Header vorhanden ist!
				// Ab und zu mal schauen, ob es vielleicht eine neue Version der Lib gibt
				// http://www.codeproject.com/cs/library/Do_Anything_With_ID3.asp
				id3Info.ID3v2Info.HaveTag = true;		

				id3Info.ID3v2Info.SetMinorVersion(3);
				id3Info.ID3v2Info.SetTextFrame("TPE1", cd.Sampler ? cd.Tracks[track].Artist : cd.Artist);
				id3Info.ID3v2Info.SetTextFrame("TIT2", cd.Tracks[track].Title);
				id3Info.ID3v2Info.SetTextWithLanguageFrame("COMM", cd.Comment);
				id3Info.ID3v2Info.SetTextFrame("TRCK", (track+1).ToString());
				id3Info.ID3v2Info.SetTextFrame("TALB", cd.Title);
				id3Info.ID3v2Info.SetTextFrame("TYER", year != 0 ? year.ToString() : "");

                id3Info.ID3v2Info.SetTextFrame("TCON", cd.Category);
                id3Info.ID3v2Info.SetTextFrame("TCOM", cd.Composer);
                id3Info.ID3v2Info.SetTextFrame("TLAN", cd.Language);

                // Das mit dem Rating geht irgendwie anders. Erst mal raus
                //                id3Info.ID3v2Info.PopularimeterFrames.Add(.SetTextFrame("POPM", cd.Rating > 0 ? cd.Rating.ToString() : "");

                id3Info.ID3v2Info.SetTextFrame("TBPM", cd.Tracks[track].Bpm > 0 ? cd.Tracks[track].Bpm.ToString() : "");

                id3Info.ID3v2Info.SetTextFrame("TLEN", cd.Tracks[track].Length > 0 ? cd.Tracks[track].Length.ToString() : "");
                try
                {
                    if (!string.IsNullOrEmpty(cd.CDCoverFrontFilename))
                    {
                        byte[] filefrontcover = File.ReadAllBytes(cd.CDCoverFrontFilename);
                        string mimetype;
                        mimetype = GetMIMEType(Path.GetExtension(cd.CDCoverFrontFilename));
                        MemoryStream memstream = new MemoryStream(filefrontcover);

                        AttachedPictureFrame item = new AttachedPictureFrame((FrameFlags)0, "", 0, mimetype, AttachedPictureFrame.PictureTypes.Cover_Front, memstream);
                        id3Info.ID3v2Info.AttachedPictureFrames.Add(item);
                    }
                }
                catch
                {
                    // Pech gehabt - kein Cover
                }

                try
                {
                    if (!string.IsNullOrEmpty(cd.Tracks[track].Lyrics))
                    {
                        ID3.ID3v2Frames.TextFrames.TextWithLanguageFrame textFrame = new ID3.ID3v2Frames.TextFrames.TextWithLanguageFrame("USLT", 0, cd.Tracks[track].Lyrics, "", TextEncodings.UTF_16, "und");
                        id3Info.ID3v2Info.TextWithLanguageFrames.Add(textFrame);
                    }
                }
                catch
                {
                    // Sicherheitshalber mal abgefragt
                }
            }

            try
            {
                switch (id3Version)
                {
                    case 0:
                        id3Info.ID3v1Info.Save();
                        break;
                    case 1:
                        id3Info.ID3v2Info.Save();
                        break;
                    case 2:
                        id3Info.ID3v1Info.Save();
                        id3Info.ID3v2Info.Save();
                        break;
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString(), System.Windows.Forms.Application.ProductName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
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
    }
}
