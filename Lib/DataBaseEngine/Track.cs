using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace Big3.Hitbase.DataBaseEngine
{
    public class Track : INotifyPropertyChanged
    {
        // Achtung, die ID ist zur Zeit nur gefüllt, wenn die CD geladen wurden
        public int ID { get; set; }                     // Die Datenbank-ID des Tracks
        public int CDID { get; set; }

        private string artist;
        /// <summary>
        /// Interpret des Tracks
        /// </summary>
        public string Artist
        {
          get { return artist; }
          set 
          { 
              artist = value;
              FirePropertyChanged("Artist");
          }
        }
    
        private int trackNumber;
        /// <summary>
        /// Lied-Nummer
        /// </summary>
        public int TrackNumber
        {
            get
            {
                return trackNumber;
            }
            set
            {
                trackNumber = value;
                FirePropertyChanged("TrackNumber");
            }
        }

        private string title;
        /// <summary>
        /// Titel des Liedes
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                FirePropertyChanged("Title");
            }
        }

        public int Length { get; set; }				// Länge des Liedes in ms
        private int bpm;

        public int Bpm
        {
            get { return bpm; }
            set
            {
                bpm = value;
                FirePropertyChanged("Bpm");
            }
        }

        private string soundfile;
        // Pfad zur aufgenommenen WAV oder MP3-Datei
        public string Soundfile
        {
            get
            {
                return soundfile;
            }
            set
            {
                soundfile = value;
                FirePropertyChanged("Soundfile");
            }
        }
        public string Codes { get; set; }				// Kennzeichen
        public string Comment { get; set; }				// Kommentar

        private string lyrics;
        /// <summary>
        /// Lyrics
        /// </summary>
        public string Lyrics
        {
            get
            {
                return lyrics;
            }
            set
            {
                lyrics = value;
                FirePropertyChanged("Lyrics");
            }
        }
        public string UserField1 { get; set; }           // Benutzerdefinierte Felder
        public string UserField2 { get; set; }           // Benutzerdefinierte Felder
        public string UserField3 { get; set; }           // Benutzerdefinierte Felder
        public string UserField4 { get; set; }           // Benutzerdefinierte Felder
        public string UserField5 { get; set; }           // Benutzerdefinierte Felder
        public int Format { get; set; }				// Format des Liedes (Audio-Track, MP3, WMA, ...)
        public int BitRate { get; set; }				// Bitrate des Liedes (z.b. 128 KBit = 131072)
        public int SampleRate { get; set; }			// Samplerate des Liedes (z.b. 44100 Hz)
        public int Channels { get; set; }				// Anzahl der Kanäle (0 = stereo (default), 1 = mono, 2 = stereo)

        private int yearRecorded;
        // Aufnahme-Jahr (für Sampler) (0 = unbekannt)
        public int YearRecorded
        {
            get
            {
                return yearRecorded; 
            }         
            set
            {
                yearRecorded = value;
                FirePropertyChanged("YearRecorded");
            }
        }

        // Version 10
        public string CheckSum { get; set; }             // MD5 CheckSum (für MP3s)
        private int rating;
        /// <summary>
        /// Rating der CD (z.Zt. 1-5 Sterne)
        /// </summary>
        public int Rating
        {
            get
            {
                return rating;
            }
            set
            {
                rating = value;
                FirePropertyChanged("Rating");
            }
        }


        // Version 11
        
        private string category;
        //Genre
        public string Category 
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                FirePropertyChanged("Category");
            }
        }    

        public string Composer { get; set; }             // Komponist
        
        // Sprache
        private string language;
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
                FirePropertyChanged("Language");
            }
        }

        public DateTime SoundFileLastModified { get; set; }

        public int PlayCount { get; set; }

        /// <summary>
        /// Startposition, falls Track auf einer CD ist.
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// CD-Laufwerk, wenn Track einer CD
        /// </summary>
        public char CDDriveLetter { get; set; }

        /// <summary>
        /// Die CD, auf der der Track ist.
        /// </summary>
        public CD CD { get; set; }

        public Track()
        {
        }

        public bool SoundFileExists
        {
            get
            {
                return !string.IsNullOrEmpty(Soundfile) && System.IO.File.Exists(Soundfile);
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public object GetValueByField(Field field)
        {
            switch (field)
            {
                case Field.TrackTitle:
                    return this.Title;
                case Field.TrackLength:
                    return this.Length;
                case Field.TrackNumber:
                    return this.TrackNumber;
                case Field.TrackBpm:
                    return this.Bpm;
                case Field.TrackCodes:
                    return this.Codes;
                case Field.TrackCategory:
                    return this.Category;
                case Field.TrackComment:
                    return this.Comment;
                case Field.TrackLyrics:
                    return this.Lyrics;
                case Field.TrackSoundFile:
                    return this.Soundfile;
                case Field.TrackYearRecorded:
                    return this.YearRecorded;
                case Field.TrackRating:
                    return this.Rating;
                case Field.TrackChecksum:
                    return this.CheckSum;
                case Field.TrackLanguage:
                    return this.Language;
                case Field.TrackSoundFileLastModified:
                    return this.SoundFileLastModified;
                case Field.TrackPlayCount:
                    return this.PlayCount;
                case Field.TrackUser1:
                    return this.UserField1;
                case Field.TrackUser2:
                    return this.UserField2;
                case Field.TrackUser3:
                    return this.UserField3;
                case Field.TrackUser4:
                    return this.UserField4;
                case Field.TrackUser5:
                    return this.UserField5;
                case Field.ArtistTrackName:
                    return this.Artist;
                case Field.ComposerTrackName:
                    return this.Composer;
                default:
                    throw new Exception("GetValueByField: Unknown field: " + field.ToString());
            }
        }

        public void SetValueToField(Field field, object value)
        {
            switch (field)
            {
                case Field.TrackTitle:
                    Title = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackLength:
                    Length = (int)value;
                    break;
                case Field.TrackNumber:
                    TrackNumber = (int)value;
                    break;
                case Field.TrackBpm:
                    Bpm = (int)value;
                    break;
                case Field.TrackCodes:
                    Codes = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackCategory:
                    Category = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackComment:
                    Comment = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackLyrics:
                    Lyrics = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackSoundFile:
                    Soundfile = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackYearRecorded:
                    YearRecorded = (int)value;
                    break;
                case Field.TrackRating:
                    Rating = (int)value;
                    break;
                case Field.TrackChecksum:
                    CheckSum = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackLanguage:
                    Language = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackSoundFileLastModified:
                    SoundFileLastModified = value == null ? new DateTime(1900, 1, 1) : (DateTime)value;
                    break;
                case Field.TrackPlayCount:
                    PlayCount = value == null ? 0 : (int)value;
                    break;
                case Field.TrackUser1:
                    UserField1 = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackUser2:
                    UserField2 = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackUser3:
                    UserField3 = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackUser4:
                    UserField4 = value is DBNull ? "" : (string)value;
                    break;
                case Field.TrackUser5:
                    UserField5 = value is DBNull ? "" : (string)value;
                    break;
                case Field.ArtistTrackName:
                    Artist = value is DBNull ? "" : (string)value;
                    break;
                case Field.ComposerTrackName:
                    Composer = value is DBNull ? "" : (string)value;
                    break;
                case Field.ArtistTrackSaveAs:
                case Field.ArtistTrackType:
                case Field.ArtistTrackSex:
                case Field.ArtistTrackCountry:
                case Field.ArtistTrackHomepage:
                case Field.ArtistTrackDateOfBirth:
                case Field.ArtistTrackDateOfDeath:
                case Field.ArtistTrackComment:
                case Field.ArtistTrackImageFilename:
                case Field.ComposerTrackSaveAs:
                case Field.ComposerTrackType:
                case Field.ComposerTrackSex:
                case Field.ComposerTrackCountry:
                case Field.ComposerTrackHomepage:
                case Field.ComposerTrackDateOfBirth:
                case Field.ComposerTrackDateOfDeath:
                case Field.ComposerTrackComment:
                case Field.ComposerTrackImageFilename:
                    // Diese Felder können nicht direkt gesetzt werden (andere Tabelle)
                    break;
                default:
                    throw new Exception("SetTrackValueToField: Unknown field: " + field.ToString());
            }
        }

        public void Save(DataBase db)
        {
            CDQueryDataSetTableAdapters.TrackTableAdapter trackta = new CDQueryDataSetTableAdapters.TrackTableAdapter(db);
            CDQueryDataSet.TrackDataTable tdt = trackta.GetDataById(ID);
            CDQueryDataSet.TrackRow trackRow;

            bool newTrack = this.ID == 0;

            if (newTrack)
            {
                trackRow = tdt.NewTrackRow();
            }
            else
            {
                trackRow = tdt[0];
            }

            foreach (Field field in FieldHelper.GetAllTrackFields(true))
            {
                switch (field)
                {
                    case Field.TrackCategory:
                        trackRow.CategoryID = db.AllCategories.GetIdByName(this.Category, true);
                        break;
                    default:
                        {
                            if (field != Field.ArtistTrackName && field != Field.ComposerTrackName)
                            {
                                DataColumn dataColumn = tdt.GetDataColumnByField(field);

                                object val = GetValueByField(field);

                                if (val == null)
                                    trackRow[dataColumn] = DBNull.Value;
                                else
                                    trackRow[dataColumn] = val;
                            }

                            break;
                        }
                }
            }

            if (CDID == 0)
                throw new Exception("CDID must be set!");

            trackRow.CDID = CDID;

            if (!String.IsNullOrEmpty(Artist))
                trackRow.ArtistID = db.GetPersonGroupRowByName(Artist, true).PersonGroupID;
            else
                trackRow.ArtistID = 0;

            if (!String.IsNullOrEmpty(Composer))
                trackRow.ComposerID = db.GetPersonGroupRowByName(Composer, true).PersonGroupID;
            else
                trackRow.ComposerID = 0;

            if (newTrack)
                tdt.AddTrackRow(trackRow);

            trackta.Update(tdt);

        }

        public static string GetPropertyNameByField(Field field)
        {
            switch (field)
            {
                case Field.TrackTitle:
                    return "Title";
                case Field.TrackLength:
                    return "Length";
                case Field.TrackNumber:
                    return "TrackNumber";
                case Field.TrackBpm:
                    return "Bpm";
                case Field.TrackCodes:
                    return "Codes";
                case Field.TrackCategory:
                    return "Category";
                case Field.TrackComment:
                    return "Comment";
                case Field.TrackLyrics:
                    return "Lyrics";
                case Field.TrackSoundFile:
                    return "Soundfile";
                case Field.TrackYearRecorded:
                    return "YearRecorded";
                case Field.TrackRating:
                    return "Rating";
                case Field.TrackChecksum:
                    return "CheckSum";
                case Field.TrackLanguage:
                    return "Language";
                case Field.TrackID:
                    return "ID";
                case Field.TrackSoundFileLastModified:
                    return "SoundFileLastModified";
                case Field.TrackPlayCount:
                    return "PlayCount";
                case Field.TrackUser1:
                    return "UserField1";
                case Field.TrackUser2:
                    return "UserField2";
                case Field.TrackUser3:
                    return "UserField3";
                case Field.TrackUser4:
                    return "UserField4";
                case Field.TrackUser5:
                    return "UserField5";
                case Field.ArtistTrackName:
                    return "Artist";
                case Field.ComposerTrackName:
                    return "Composer";
                default:
                    throw new Exception("GetPropertyNameByField: Unknown field: " + field.ToString());
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
