using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using Big3.Hitbase.Miscellaneous;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.DataBaseEngine
{
	public enum CoverType { Front, Back, Label, PersonGroup };

	public enum AlbumType 
	{ 
		AudioCD = 0,           // Normale Audio-CD (Redbook)
		MusicDataCD,           // Musik Daten-CD (mit MP3/WMA/...-Dateien)
        SoundFiles,            // Musikdateien von Festplatte (manuelle)
        ManagedSoundFiles      // Musikdateien von Festplatte (synchronisiert)
	};

	public class CD : INotifyPropertyChanged
	{
		/// <summary>
		/// Datenbank-ID des Datensatzes
		/// </summary>
        private int id;
		public int ID 
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                FirePropertyChanged("ID");
            }
        }

        private string identity;
        // Eindeutige ID der CD
		public string Identity 
        { 
            get
            {
                return identity;
            }
            set
            {
                identity = value;
                FirePropertyChanged("Identity");
            }
        }
                     
        private int totalLength;
        public int TotalLength
        {
            get
            {
                return totalLength;
            }
            set
            {
                totalLength = value;
                FirePropertyChanged("TotalLength");
            }
        }         // Länger der CD in ms

		private int numberOfTracks;
		public int NumberOfTracks
		{ 
			get
			{
				return numberOfTracks;
			}
			set
			{
				//if (value == numberOfTracks)
				//    return;

				// wird nicht mehr gemacht, da man z.B. eine MP3-CD mit nur einem Track haben kann, 
				// NumberOfTrack aber trotzdem die Anzahl der Tracks auf dem Album enthält.
				/*if (Tracks != null)
				{
					// Anzahl der Tracks nur vergrößern
					for (int i = Tracks.Count; i < value; i++)
						Tracks.Add(new Track());
				}
				else
				{
					Tracks = new List<Track>(value);

					for (int i = 0; i < value; i++)
						Tracks.Add(new Track());
				}

				StartPositions = new int[value + 1];*/

				numberOfTracks = value;

                FirePropertyChanged("NumberOfTracks");
			}
		}

        private bool sampler;
        /// <summary>
        /// false = Normale CD, true = Sampler
        /// </summary>
		public bool Sampler 
        {
            get
            {
                return sampler;
            }
            set
            {
                sampler = value;
                FirePropertyChanged("Sampler");
            }
        }

        private int cdSetNumber;
        // Nummer der CD im CD-Set
		public int CDSetNumber 
        { 
            get
            {
                return cdSetNumber;
            }
            set
            {
                cdSetNumber = value;
                FirePropertyChanged("CDSetNumber");
            }
        }

        private string cdSetName;
        // ID des CD-Sets in der CD-Sets Tabelle
		public string CDSetName 
        { 
            get { return cdSetName; }
            set
            {
                cdSetName = value;
                FirePropertyChanged("CDSetName");
            }
        }    
        
        private string artist;

        public string Artist
        {
            get { return artist; }
            set 
            { 
                artist = value;
                FirePropertyChanged("Artist");
            }
        }

        private string category;
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

        private string medium;
        public string Medium
        {
            get
            {
                return medium;
            }             
            set
            {
                medium = value;
                FirePropertyChanged("Medium");
            }
        }

        private string title;
        /// <summary>
        /// Titel der CD
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

        private string date;
        /// <summary>
        /// Datum-Feld (Bedeutung frei definierbar)
        /// </summary>
		public string Date 
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                FirePropertyChanged("Date");
            }
        }

        private string archiveNumber;
        public string ArchiveNumber 
        { 
            get
            {
                return archiveNumber;
            }
            set
            {
                archiveNumber = value;
                FirePropertyChanged("ArchiveNumber");
            }
        }

        private string cdCoverFrontFilename;
        public string CDCoverFrontFilename 
        { 
            get
            {
                return cdCoverFrontFilename;
            }
            set
            {
                cdCoverFrontFilename = value;
                FirePropertyChanged("CDCoverFrontFilename");
            }
        }

        private string codes;
        public string Codes
        {
            get
            {
                return codes;
            }
            set
            {
                codes = value;
                FirePropertyChanged("Codes");
            }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set 
            {
                comment = value;
                FirePropertyChanged("Comment");
            }
        }

        // Benutzerdefinierte Felder
        private string userField1;
		public string UserField1
        {
            get
            {
                return userField1;
            }
            set
            {
                userField1 = value;

                FirePropertyChanged("UserField1");
            }
        }

        private string userField2;
        public string UserField2
        {
            get
            {
                return userField2;
            }
            set
            {
                userField2 = value;

                FirePropertyChanged("UserField2");
            }
        }

        private string userField3;
        public string UserField3
        {
            get
            {
                return userField3;
            }
            set
            {
                userField3 = value;

                FirePropertyChanged("UserField3");
            }
        }

        private string userField4;
        public string UserField4
        {
            get
            {
                return userField4;
            }
            set
            {
                userField4 = value;

                FirePropertyChanged("UserField4");
            }
        }

        private string userField5;
        public string UserField5
        {
            get
            {
                return userField5;
            }
            set
            {
                userField5 = value;

                FirePropertyChanged("UserField5");
            }
        }

        private AlbumType type;
		public AlbumType Type
        { 
            get
            {
                return type;
            }
            set
            {
                type = value;
                FirePropertyChanged("Type");
            }
        }    			   // Typ der CD (Audio-CD, Musik-Daten-CD)

        private int yearRecorded;
		public int YearRecorded 
        {
            get { return yearRecorded; }
            set 
            { 
                yearRecorded = value;
                FirePropertyChanged("YearRecorded");
            }
        }        // Aufnahme-Jahr

        private string copyright;
        public string Copyright 
        {
            get
            {
                return copyright;
            }
            set
            {
                copyright = value;

                FirePropertyChanged("Copyright");
            }
        }           

		// Version 10
        private bool original;
        // Original-CD (kann ins CD-Archiv übertragen werden)
		public bool Original 
        {
            get { return original; }
            set
            {
                original = value;

                FirePropertyChanged("Original");
            }
        }

        private string cdCoverBackFilename;
        // Dateiname des Back-Cover (Grafik)
		public string CDCoverBackFilename 
        { 
            get 
            {
                return cdCoverBackFilename;
            }
            set
            {
                cdCoverBackFilename = value;

                FirePropertyChanged("CDCoverBackFilename");
            }
        }

        private string cdCoverLabelFilename;
        // Dateiname des CD-Labels (Grafik)
		public string CDCoverLabelFilename 
        {
            get { return cdCoverLabelFilename; }
            set
            {
                cdCoverLabelFilename = value;
                FirePropertyChanged("CDCoverLabelFilename");
            }
        }

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

        private string label;
        // Label (z.B. Sony, EMI, etc.)
        public string Label 
        {
            get { return label; }
            set 
            {
                label = value;
                FirePropertyChanged("Label");
            }
        }

        private string url;
        // Verknüpfungen (getrennt mit Semikolon)
		public string URL 
        { 
            get { return url; }
            set
            {
                url = value;
                FirePropertyChanged("URL");
            }
        }

        private int price;
        // Preis (Wähung * 100, z.B. 29,99€ = 2999)
		public int Price 
        { 
            get { return price; }
            set
            {
                price = value;
                FirePropertyChanged("Price");
            }
        }

        private string upc;
        // UPC oder EAN Code
        public string UPC
        {
            get
            {
                return upc;
            }
            set
            {
                upc = value;
                FirePropertyChanged("UPC");
            }
        }

		// Version 11
        private string composer;

        public string Composer
        {
            get { return composer; }
            set 
            { 
                composer = value;
                FirePropertyChanged("Composer");
            }
        }

        private string location;
        // Standort
		public string Location 
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                FirePropertyChanged("Location");
            }
        }

        private string language;
        // Sprache
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

		public ParticipantList Participants { get; set; } // Mitwirkende

        public DateTime? Created { get; set; }           // Wann wurde die CD erfasst
        public DateTime? LastModified { get; set; }      // Wann wurde die CD zuletzt geändert

        public UrlList Urls { get; set; }               // Liste von Links (Urls)

		public ProgramList Programs { get; set; }       // Programm
		public IndexList Indexes { get; set; }          // Indexes

        private SafeObservableCollection<Track> tracks;
        /// <summary>
        /// Tracks
        /// </summary>
        public SafeObservableCollection<Track> Tracks 
        {
            get
            {
                return tracks;
            }
            set
            {
                tracks = value;
                FirePropertyChanged("Tracks");
            }
        }

		///////////////////////////////////////////////////////////////////////

		public CD()
		{
            Tracks = new SafeObservableCollection<Track>();
			Programs = new ProgramList();
			Indexes = new IndexList();
		}

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetStringByFieldEx(DataBase db, Field field)
        {
            if (FieldHelper.IsCDArtistField(field))
            {
                PersonGroup personGroup = db.GetPersonGroupByName(this.Artist, false);
                return personGroup.GetStringByField(field);
            }

            if (FieldHelper.IsCDComposerField(field))
            {
                PersonGroup personGroup = db.GetPersonGroupByName(this.Composer, false);
                return personGroup.GetStringByField(field);
            }

            return GetStringByField(db, field);
        }


		public string GetStringByField(DataBase db, Field field)
		{
			object val = GetValueByField(field);

			if (field == Field.TotalLength)
				return Misc.GetShortTimeString((int)val);

			if (field == Field.Price)
			{
				float floatVal = (float)(int)val / 100;

				return floatVal.ToString("C");
			}

			if (field == Field.Date)
				return db.FormatDate((string)val);

			if (field == Field.YearRecorded && (int)val == 0)
				return "";

			if (val is bool)
			{
				if ((bool)val == true)
					return StringTable.Yes;
				else
					return StringTable.No;
			}

			if (field == Field.User1 || field == Field.User2 || field == Field.User3 ||
				field == Field.User4 || field == Field.User5)
				return db.GetDisplayStringByUserField(val as string, field);

			if (val != null)
				return val.ToString();
			else
				return null;
		}

		public object GetValueByField(Field field)
		{
			switch (field)
			{
				case Field.None:
					return null;
				case Field.Title:
					return this.Title;
				case Field.Sampler:
					return this.Sampler;
				case Field.TotalLength:
					return this.TotalLength;
				case Field.NumberOfTracks:
					return this.NumberOfTracks;
				case Field.Category:
					return this.Category;
				case Field.Medium:
					return this.Medium;
				case Field.Comment:
					return this.Comment;
				case Field.CDSet:
					return this.CDSetName;
				case Field.DiscNumberInCDSet:
					return this.CDSetNumber;
				case Field.Copyright:
					return this.Copyright;
				case Field.Label:
					return this.Label;
				case Field.YearRecorded:
					return this.YearRecorded;
				case Field.Date:
					return this.Date;
				case Field.Codes:
					return this.Codes;
				case Field.ArchiveNumber:
					return this.ArchiveNumber;
				case Field.Rating:
					return this.Rating;
				case Field.Language:
					return this.Language;
				case Field.Location:
					return this.Location;
				case Field.UPC:
					return this.UPC;
				case Field.Price:
					return this.Price;
				case Field.CDCoverFront:
					return this.CDCoverFrontFilename;
				case Field.CDCoverBack:
					return this.CDCoverBackFilename;
				case Field.CDCoverLabel:
					return this.CDCoverLabelFilename;
				case Field.OriginalCD:
					return this.Original;
				case Field.Homepage:
					return this.URL;
				case Field.Identity:
					return this.Identity;
				case Field.AlbumType:
					return this.Type;
                case Field.Created:
                    return this.Created;
                case Field.LastModified:
                    return this.LastModified;
				case Field.User1:
					return this.UserField1;
				case Field.User2:
					return this.UserField2;
				case Field.User3:
					return this.UserField3;
				case Field.User4:
					return this.UserField4;
				case Field.User5:
					return this.UserField5;
				case Field.ArtistCDName:
					return this.Artist;
				case Field.ComposerCDName:
					return this.Composer;
				default:
					throw new Exception("GetValueFromField: Unknown field: " + field.ToString());
			}
		}

        public void SetStringValueToField(Field field, string value)
        {
            Type fieldType = DataBase.GetTypeByField(field);

            if (fieldType == typeof(int))
            {
                int intValue = Misc.Atoi(value);
                SetValueToField(field, intValue);
            }
            else
            {
		        SetValueToField(field, value);
            }
        }

		public void SetValueToField(Field field, object value)
		{
			switch (field)
			{
				case Field.None:
					return;
				case Field.Title:
                    this.Title = value is DBNull ? "" : (string)value;
					break;
				case Field.Sampler:
					this.Sampler = (bool)value;
					break;
				case Field.NumberOfTracks:
					this.NumberOfTracks = (int)value;
					break;
				case Field.TotalLength:
					this.TotalLength = (int)value;
					break;
				case Field.Category:
                    this.Category = value is DBNull ? "" : (string)value;
					break;
				case Field.Medium:
                    this.Medium = value is DBNull ? "" : (string)value;
					break;
				case Field.Comment:
                    this.Comment = value is DBNull ? "" : (string)value;
					break;
				case Field.CDSet:
                    this.CDSetName = value is DBNull ? "" : (string)value;
					break;
				case Field.DiscNumberInCDSet:
					this.CDSetNumber = (int)value;
					break;
				case Field.Copyright:
                    this.Copyright = value is DBNull ? "" : (string)value;
					break;
				case Field.Label:
                    this.Label = value is DBNull ? "" : (string)value;
					break;
				case Field.YearRecorded:
					if (value != null)
					{
						int yearRecorded;
						if (Int32.TryParse(value.ToString(), out yearRecorded))
							YearRecorded = yearRecorded;
					}
					break;
				case Field.Date:
                    this.Date = value is DBNull ? "" : (string)value;
					break;
				case Field.Codes:
                    this.Codes = value is DBNull ? "" : (string)value;
					break;
				case Field.ArchiveNumber:
					this.ArchiveNumber = value is DBNull ? "" : (string)value;
					break;
				case Field.Rating:
					this.Rating = (int)Convert.ToInt32(value);
					break;
				case Field.Language:
                    this.Language = value is DBNull ? "" : (string)value;
					break;
				case Field.Location:
                    this.Location = value is DBNull ? "" : (string)value;
					break;
				case Field.UPC:
                    this.UPC = value is DBNull ? "" : (string)value;
					break;
				case Field.Price:
					this.Price = (int)Convert.ToInt32(value);
					break;
				case Field.CDCoverFront:
                    this.CDCoverFrontFilename = value is DBNull ? "" : (string)value;
					break;
				case Field.CDCoverBack:
                    this.CDCoverBackFilename = value is DBNull ? "" : (string)value;
					break;
				case Field.CDCoverLabel:
                    this.CDCoverLabelFilename = value is DBNull ? "" : (string)value;
					break;
				case Field.OriginalCD:
					this.Original = (bool)value;
					break;
				case Field.Homepage:
                    this.URL = value is DBNull ? "" : (string)value;
					break;
				case Field.Identity:
					this.Identity = value is DBNull ? "" : (string)value;
					break;
				case Field.AlbumType:
					this.Type = (AlbumType)value;
					break;
                case Field.Created:
                    this.Created = (DateTime?)value;
                    break;
                case Field.LastModified:
                    this.LastModified = (DateTime?)value;
                    break;
                case Field.User1:
                    this.UserField1 = value is DBNull ? "" : (string)value;
					break;
				case Field.User2:
                    this.UserField2 = value is DBNull ? "" : (string)value;
					break;
				case Field.User3:
                    this.UserField3 = value is DBNull ? "" : (string)value;
					break;
				case Field.User4:
                    this.UserField4 = value is DBNull ? "" : (string)value;
					break;
				case Field.User5:
                    this.UserField5 = value is DBNull ? "" : (string)value;
					break;
				case Field.ArtistCDName:
                    this.Artist = value is DBNull ? "" : (string)value;
					break;
				case Field.ArtistCDSaveAs:
				case Field.ArtistCDType:
				case Field.ArtistCDSex:
				case Field.ArtistCDCountry:
				case Field.ArtistCDHomepage:
				case Field.ArtistCDDateOfBirth:
				case Field.ArtistCDDateOfDeath:
				case Field.ArtistCDComment:
				case Field.ArtistCDImageFilename:
				case Field.ComposerCDSaveAs:
				case Field.ComposerCDType:
				case Field.ComposerCDSex:
				case Field.ComposerCDCountry:
				case Field.ComposerCDHomepage:
				case Field.ComposerCDDateOfBirth:
				case Field.ComposerCDDateOfDeath:
				case Field.ComposerCDComment:
				case Field.ComposerCDImageFilename:
					// Diese Felder können nicht direkt gesetzt werden (andere Tabelle)
					break;
				case Field.ComposerCDName:
                    this.Composer = value is DBNull ? "" : (string)value;
					break;
				default:
					throw new Exception("SetValueToField: Unknown field: " + field.ToString());
			}
		}

        public string GetTrackStringByFieldEx(DataBase db, int track, Field field)
        {
            if (FieldHelper.IsTrackArtistField(field))
            {
                PersonGroup personGroup = db.GetPersonGroupByName(this.Tracks[track].Artist, false);
                return personGroup.GetStringByField(field);
            }
            if (FieldHelper.IsTrackComposerField(field))
            {
                PersonGroup personGroup = db.GetPersonGroupByName(this.Tracks[track].Composer, false);
                return personGroup.GetStringByField(field);
            }
            return GetTrackStringByField(track, field);

        }

		public string GetTrackStringByField(int track, Field field)
		{
			object val = GetTrackValueByField(track, field);

			if (field == Field.TrackLength)
				return Misc.GetShortTimeString((int)val);

			if (val is bool)
			{
				if ((bool)val == true)
					return StringTable.Yes;
				else
					return StringTable.No;
			}

            if (field == Field.TrackYearRecorded && (int)val == 0)
                return "";

            if (field == Field.TrackBpm && (int)val == 0)
                return "";

            if (field == Field.TrackRating && (int)val == 0)
                return "";

			if (val != null)
				return val.ToString();
			else
				return null;
		}

		public object GetTrackValueByField(int track, Field field)
		{
            return this.Tracks[track].GetValueByField(field);
		}

		public void SetTrackValueToField(int track, Field field, object value)
		{
			// Anzahl der Tracks nur vergrößern
			for (int i = Tracks.Count; i <= track; i++)
				Tracks.AddItemFromThread(new Track());

			this.Tracks[track].SetValueToField(field, value);
		}

        public void InitTracks(int numberOfTracks)
        {
            // Anzahl der Tracks nur vergrößern
            for (int i = Tracks.Count; i < numberOfTracks; i++)
            {
                Track newTrack = new Track();
                newTrack.TrackNumber = i + 1;
                Tracks.Add(newTrack);
            }
        }

		/// <summary>
		/// Speichert die CD
		/// </summary>
		/// <param name="db"></param>
		public void Save(DataBase db)
		{
			Save(db, null);
		}

		/// <summary>
		/// Speichert die CD
		/// </summary>
		/// <param name="db"></param>
		/// <param name="trackToSave">Speichert nur diesen Track (null = alle) und natürlich die CD.</param>
		public void Save(DataBase db, Track trackToSave)
		{
			// Interpret ist Pflicht
			if (String.IsNullOrEmpty(Artist))
				return;

			SqlCeTransaction trans = db.Connection.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				CDQueryDataSetTableAdapters.CDTableAdapter cdta = new CDQueryDataSetTableAdapters.CDTableAdapter(db);
				cdta.Transaction = trans;
				CDQueryDataSet.CDDataTable dt = new CDQueryDataSet.CDDataTable();
				CDQueryDataSet.CDRow cdRow;

				bool newCD = (ID == 0);

				if (!newCD)        // Dann ist die CD schon in der Datenbank
				{
					cdta.FillById(dt, ID);
					if (dt.Rows.Count < 1)
					{
						// Dann ist die CD mit der angegebenen ID nicht vorhanden
						throw new ItemNotFoundException(ID);
					}
					cdRow = dt[0];
				}
				else
				{
					cdRow = dt.NewCDRow();
				}

				foreach (Field field in FieldHelper.GetAllCDFields(true))
				{
					switch (field)
					{
						case Field.Medium:
							cdRow.MediumID = db.AllMediums.GetIdByName(Medium);
							break;
						case Field.Category:
							cdRow.CategoryID = db.AllCategories.GetIdByName(Category, true);
							break;
						case Field.CDSet:
							cdRow.SetID = db.GetIdBySet(CDSetName, true);
							break;
                        case Field.LastModified:
                            cdRow.LastModified = DateTime.Now;
                            break;
                        case Field.Created:
                            if (newCD)
                                cdRow.Created = DateTime.Now;
                            break;
						default:
							DataColumn dataColumn = dt.GetDataColumnByField(field);

							object val = this.GetValueByField(field);
											
							if (val == null)
								cdRow[dataColumn] = DBNull.Value;
							else
								cdRow[dataColumn] = val;
							break;
					}
				}

				cdRow.ArtistID = db.GetPersonGroupRowByName(Artist, true).PersonGroupID;
				if (!String.IsNullOrEmpty(Composer))
					cdRow.ComposerID = db.GetPersonGroupRowByName(Composer, true).PersonGroupID;
				else
					cdRow.ComposerID = 0;

				if (newCD)
					dt.AddCDRow(cdRow);

				cdta.Update(dt);

				int cdid;

				if (newCD)
				{
					cdid = (int)(decimal)db.ExecuteScalar("SELECT @@IDENTITY", trans);
					ID = cdid;
				}
				else
				{
					cdid = cdRow.CDID;
				}

				// Trackdaten schreiben
				CDQueryDataSetTableAdapters.TrackTableAdapter trackta = new CDQueryDataSetTableAdapters.TrackTableAdapter(db);
				trackta.Transaction = trans;
				CDQueryDataSet.TrackDataTable tdt = new CDQueryDataSet.TrackDataTable();

				if (!newCD)        // Dann ist die CD schon in der Datenbank
				{
					if (trackToSave != null)
					{
						if (trackToSave.ID != 0)        // Dann ist der Track schon vorhanden
						{
							trackta.FillById(tdt, trackToSave.ID);    
						}
					}
					else
					{
                        // Hier ist es einfacher, wenn wir alle Tracks der CD löschen und neu speichern.
                        //Wieder ausgebaut, muss auch so gehen! Gibt sonst Probleme beim Update
                        //db.ExecuteScalar("DELETE FROM Track WHERE CDID=" + ID.ToString());

						trackta.FillByCDID(tdt, ID);
					}
				}

				int trackNumber = 0;
				foreach (Track track in Tracks)
				{
					// Prüfen, ob wir beim Track sind, den wir speichern wollen
					if (trackToSave != null && track != trackToSave)
						continue;

					CDQueryDataSet.TrackRow trackRow = null;

					bool newTrack = newCD || track.ID == 0;

					if (newTrack)
					{
						trackRow = tdt.NewTrackRow();
					}
					else
					{
                        foreach (CDQueryDataSet.TrackRow searchTrack in tdt)
                        {
                            if (searchTrack.TrackID == track.ID)
                            {
                                trackRow = searchTrack;
                                break;
                            }
                        }
					}

                    if (trackRow == null)
                    {
                        throw new Exception("internal save error: track not found!");
                    }

					foreach (Field field in FieldHelper.GetAllTrackFields(true))
					{
						switch (field)
						{
							case Field.TrackCategory:
								trackRow.CategoryID = db.AllCategories.GetIdByName(track.Category, true);
								break;
                            case Field.TrackSoundFileLastModified:
                                break;
							default:
								{
									if (field != Field.ArtistTrackName && field != Field.ComposerTrackName)
									{
										DataColumn dataColumn = tdt.GetDataColumnByField(field);

										object val = track.GetValueByField(field);

										// Prüfen, ob die Tracknummer stimmt
										/*if (!newCD && !newTrack && trackRow.TrackNumber != trackNumber + 1)
										{
											throw new Exception("db error: TrackNumber does not match!");
										}*/

										if (val == null)
											trackRow[dataColumn] = DBNull.Value;
										else
											trackRow[dataColumn] = val;
									}

									break;
								}
						}
					}

					trackRow.CDID = cdid;

					if (Sampler)
					{
						if (!String.IsNullOrEmpty(track.Artist))
							trackRow.ArtistID = db.GetPersonGroupRowByName(track.Artist, true).PersonGroupID;
						else
							trackRow.ArtistID = 0;
					}
					else
					{
						trackRow.ArtistID = cdRow.ArtistID;
					}

					if (!String.IsNullOrEmpty(track.Composer))
						trackRow.ComposerID = db.GetPersonGroupRowByName(track.Composer, true).PersonGroupID;
					else
						trackRow.ComposerID = 0;

					//Geht nicht mehr if (newTrack && trackRow.TrackNumber == 0)
					//    trackRow.TrackNumber = trackNumber + 1;

					trackNumber++;

                    if (newTrack)
                    {
                        tdt.AddTrackRow(trackRow);
                    }
				}

                // Wurde ein Track gelöscht?
                if (!newCD && Tracks.Count < tdt.Rows.Count)
                {
                    for (int i = tdt.Rows.Count - 1; i >= 0; i--)
                    {
                        bool exists = false;

                        foreach (Track existTrack in Tracks)
                        {
                            if (existTrack.ID == tdt[i].TrackID)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            tdt[i].Delete();
                        }
                    }                    
                }

				trackta.Update(tdt);

				// Wenn nur ein Track gespeichert wird, werden die Mitwirkenden nicht gespeichert.
				if (trackToSave == null)
					SaveParticipants(db, cdid);

				trans.Commit();
			}
			catch (Exception e)
			{
				trans.Rollback();

				Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
				formEx.ShowDialog();
			}
		}

		/// <summary>
		/// Speichert die Mitwirkenden
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cdId"></param>
		private void SaveParticipants(DataBase db, int cdId)
		{
			try
			{
				// Zuerst alle Mitwirkenden der CD löschen. Ist so einfacher. ;-)
				String sql;
				sql = string.Format("DELETE FROM Participant Where CDID={0}", cdId);

				db.ExecuteNonQuery(sql);

				if (Participants == null)
					return;

				ParticipantDataSetTableAdapters.ParticipantTableAdapter participantsta = new ParticipantDataSetTableAdapters.ParticipantTableAdapter(db);
				ParticipantDataSet.ParticipantDataTable pdt = new ParticipantDataSet.ParticipantDataTable();

				foreach (Participant p in Participants)
				{
					int personGroupId = db.GetPersonGroupRowByName(p.Name, true).PersonGroupID;
					int roleId = db.GetRoleByName(p.Role, true).RoleID;
					pdt.AddParticipantRow(personGroupId, roleId, cdId, p.TrackNumber, p.Comment);
				}

				participantsta.Update(pdt);
			}
			catch (Exception e)
			{
				Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
				formEx.ShowDialog();
			}
		}

		/// <summary>
		/// Speichert die Programme
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cdId"></param>
		public void SavePrograms(DataBase db, int cdId)
		{
			try
			{
				// Zuerst alle Programme der CD löschen. Ist so einfacher. ;-)
				String sql;
				sql = string.Format("DELETE FROM Program Where CDID={0}", cdId);

				db.ExecuteNonQuery(sql);

				ProgramDataSetTableAdapters.ProgramTableAdapter programsta = new ProgramDataSetTableAdapters.ProgramTableAdapter(db);
				ProgramDataSet.ProgramDataTable pdt = new ProgramDataSet.ProgramDataTable();

				foreach (Program p in Programs)
				{
					string progList = "";
					
					// Die Programmliste wird mit ", " zwischen den Tracks als String gespeichert.
					for (int i=0;i<p.Tracks.Length;i++)
					{
						progList += p.Tracks[i].ToString();
						if (i < p.Tracks.Length - 1)
							progList += ", ";
					}

					pdt.AddProgramRow(cdId, p.Name, progList, p.IsStandard);
				}

				programsta.Update(pdt);
			}
			catch (Exception e)
			{
				Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
				formEx.ShowDialog();
			}
		}

		/// <summary>
		/// Speichert die Indexe
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cdId"></param>
		public void SaveIndexes(DataBase db, int cdId)
		{
			try
			{
				// Zuerst alle Indexe der CD löschen. Ist so einfacher. ;-)
				String sql;
				sql = string.Format("DELETE FROM [Index] Where CDID={0}", cdId);

				db.ExecuteNonQuery(sql);

				IndexDataSetTableAdapters.IndexTableAdapter indexesta = new IndexDataSetTableAdapters.IndexTableAdapter(db);
				IndexDataSet.IndexDataTable idt = new IndexDataSet.IndexDataTable();

				foreach (Index index in Indexes)
				{
					idt.AddIndexRow(cdId, index.Name, index.Position);
				}

				indexesta.Update(idt);
			}
			catch (Exception e)
			{
				Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
				formEx.ShowDialog();
			}
		}

		/// <summary>
		/// Die Gesamtlänge der CD neu berechnen
		/// </summary>
		public void UpdateTotalTime()
		{
			int i;
	
			TotalLength = 0;
			for (i = 0;i < Tracks.Count;i++)
				TotalLength += Tracks[i].Length;
		}

		private uint cddb_sum(uint n)
		{
			uint	ret = 0;

			while (n > 0)
			{
				ret += (n % 10);
				n /= 10;
			}

			return ret;
		}


		public uint GetCDDBDiscID()
		{
			uint	t, n = 0;

			for (int i = 0; i < this.Tracks.Count; i++)
			{
				n += cddb_sum((uint)(this.Tracks[i].StartPosition/1000));
			}

			//t = m_dwTotalLength/1000+1;

			t = GetRoundedTime(GetCDDBDiscLength());

			return ((n % 0xff) << 24 | t << 8 | (uint)NumberOfTracks);
		}

		public uint GetRoundedTime(uint t)
		{
			uint minutes = t/1000/60;
			uint seconds = t/1000%60;

			uint first = (uint)this.Tracks[0].StartPosition;
			uint minutesFirst = first/1000/60;
			uint secondsFirst = first/1000%60;

			return minutes*60+seconds - (minutesFirst*60+secondsFirst);
		}

		// Liefert die tatsächliche Länge der CD zurück auch wenn der letzte Track ein Datentrack ist.
		public uint GetCDDBDiscLength()
		{
			uint offsetInFrames = (uint)Tracks[NumberOfTracks - 1].StartPosition * 75 / 1000;
			uint lengthInFrames = (uint)Tracks[NumberOfTracks-1].Length * 75 / 1000;

			// Die + 1 ist hier wohl nötig, da die Länge des letzten Frames beim MCI-Interface ein Frame zu kurz ist 
			// (siehe freedb Doku).
			//(offset_minutes * 60 * 75) + (offset_seconds * 75) + offset_frames +
		//(length_minutes * 60 * 75) + (length_seconds * 75) + length_frames + 1 = X

			uint realLength = (offsetInFrames + lengthInFrames + 1) * 1000 / 75;

			return realLength;
		}

        // Schreibweise von Titel und Interpreten anpassen
		public bool AdjustSpelling()
        {
            return AdjustSpelling(Settings.Current.AdjustSpelling);
        }

		// Schreibweise von Titel und Interpreten anpassen
		public bool AdjustSpelling(int iAdjustSpelling)
		{
			Artist = AdjustString(Artist, iAdjustSpelling);
			Composer = AdjustString(Composer, iAdjustSpelling);
			Title = AdjustString(Title, iAdjustSpelling);

			for (int i=0;i<Tracks.Count;i++)
			{
				Tracks[i].Artist = AdjustString(Tracks[i].Artist, iAdjustSpelling);
				Tracks[i].Composer = AdjustString(Tracks[i].Composer, iAdjustSpelling);
				Tracks[i].Title = AdjustString(Tracks[i].Title, iAdjustSpelling);
			}

			return true;
		}

		String AdjustString(String sString, int iAdjustSpelling)
		{
			if (String.IsNullOrEmpty(sString))
				return sString;

			switch (iAdjustSpelling)
			{
				case 0:  // keine Änderung
					break;
				case 1: // Erster Buchstabe groß, rest klein (Ausnahme: Wort "I", englisch für Ich).
					sString = sString.ToLower();
					sString = Char.ToUpper(sString[0]) + sString.Substring(1);

					// Ausnahme: Wort "I", englishc für Ich.
					sString = sString.Replace(" i ", " I ");
					if (sString.StartsWith("i "))
						sString = "I " + sString.Substring(2);

					if (sString.EndsWith(" i"))
						sString = sString.Substring(0, sString.Length-2) + " I";

					break;
				case 2: // Erster Buchstabe jedes Wortes groß
					{
						bool bWordBegin = true;
						const String sWordDelimiters = " ().:,;\"!/[]{}+*-<>=";
						for (int i=0;i<sString.Length;i++)
						{
							if (sWordDelimiters.IndexOf(sString[i]) >= 0)
								bWordBegin = true;
							else
							{
								if (Char.IsLetter(sString[i]))
								{
									if (bWordBegin)
									{
										sString = sString.Substring(0, i) + Char.ToUpper(sString[i]) + sString.Substring(i+1);
										bWordBegin = false;
									}
									else
										sString = sString.Substring(0, i) + Char.ToLower(sString[i]) + sString.Substring(i+1);
								}
							}
						}
						break;
					}
				default:
					break;
				}

			return sString;
		}

		public string GetCDCoverFilename(CoverType coverType)
		{
            return CD.GetCDCoverFilename(Artist, Title, coverType);
		}

        static public string GetCDCoverFilename(string artist, string title, CoverType coverType)
        {
            if (!String.IsNullOrEmpty(artist) && !String.IsNullOrEmpty(title))
            {
                String filename = String.Format("{0} - {1}.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));

                switch (coverType)
                {
                    case CoverType.Front:
                        filename = String.Format("{0} - {1}.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));
                        break;
                    case CoverType.Back:
                        filename = String.Format("{0} - {1} - Back.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));
                        break;
                    case CoverType.Label:
                        filename = String.Format("{0} - {1} - Label.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));
                        break;
                    default:
                        break;
                }

                filename = Misc.GetCDCoverFilename(filename);

                return filename;
            }

            return "";

        }


		/// <summary>
		/// Liefert den Trackindex des Tracks mit der angegebenen ID zurück.
		/// </summary>
		/// <param name="trackID"></param>
		/// <returns></returns>
		public int FindTrackIndexByTrackID(int trackID)
		{
			for (int i = 0; i < Tracks.Count; i++)
			{
				if (Tracks[i].ID == trackID)
					return i;
			}

			return -1;
		}

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void Clear()
        {
            // Alle Felder leeren

            this.ID = 0;
			this.Title = "";
            this.Sampler = false;
			this.NumberOfTracks = 0;
			this.TotalLength = 0;
			this.Category = "";
			this.Medium = "";
			this.Comment = "";
			this.CDSetName = "";
			this.CDSetNumber = 0;
			this.Copyright = "";
			this.Label = "";
			this.YearRecorded = 0;
			this.Date = "";
			this.Codes = "";
			this.ArchiveNumber = "";
			this.Rating = 0;
			this.Language = "";
			this.Location = "";
			this.UPC = "";
			this.Price = 0;
			this.CDCoverFrontFilename = "";
			this.CDCoverBackFilename = "";
			this.CDCoverLabelFilename = "";
			this.Original = false;
			this.URL = "";
			this.Identity = "";
			this.Type = (AlbumType)0;
            this.Created = null;
            this.LastModified = null;
			this.UserField1 = "";
			this.UserField2 = "";
			this.UserField3 = "";
			this.UserField4 = "";
			this.UserField5 = "";
            this.Artist = "";
            this.Composer = "";
            this.Programs.Clear();
            this.Indexes.Clear();
            if (this.Participants != null)
            {
                this.Participants.Clear();
            }

            Tracks.Clear();
        }
    }

	public class Participant : INotifyPropertyChanged
	{
        private string role;
        /// <summary>
        /// Rolle
        /// </summary>
        public string Role
        {
            get
            {
                return role;
            }
            set
            {
                role = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Role"));
            }
        }

        private string name;
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

		private int trackNumber;
        public int TrackNumber
        {
            get
            {
                return trackNumber;
            }
            set
            {
                trackNumber = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TrackNumber"));
            }
        }

        private string comment;
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Comment"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

	public class ParticipantList : SafeObservableCollection<Participant>
	{
	}

    public class Role : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

	public class Index
	{
		public Index()
		{
		}

		public Index(string name, int position)
		{
			Name = name;
			Position = position;
		}

		public string Name { get; set; }            // Name des Programms
		public int Position { get; set; }           // Position des Indexes
	}

	public class IndexList : SafeObservableCollection<Index>
	{
	}

	public class Program
	{
		public Program()
		{
		}

		public Program(string name, int[] tracks)
		{
			Name = name;
			Tracks = tracks;
			IsStandard = false;
		}

		public Program(string name, string tracks)
		{
			Name = name;

			List<int> trackArray = new List<int>();
			string[] trackList = tracks.Split(',');
			foreach (string track in trackList)
			{
				int t = 0;

				if (Int32.TryParse(track.Trim(), out t))
					trackArray.Add(t);
			}

			Tracks = trackArray.ToArray();
			IsStandard = false;
		}

		public string Name { get; set; }            // Name des Programms
		public int[] Tracks { get; set; }           // Tracks des Programms
		public bool IsStandard { get; set; }        // Ist das Programm ein Standard-Programm?
	}

	public class ProgramList : SafeObservableCollection<Program>
	{
		public int GetDefault()
		{
			for (int i=0;i<Count;i++)
				if (this[i].IsStandard)
					return i;

			return -1;
		}

		public void SetDefault(int index)
		{
			foreach (Program p in this)
				p.IsStandard = false;

			this[index].IsStandard = true;
		}
	}

}
