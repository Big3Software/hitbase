using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using Big3.Hitbase.DataBaseEngine.CDQueryDataSetTableAdapters;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Linq;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;
using System.Xml.Serialization;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.DataBaseEngine
{
    public enum PersonGroupType
    {
        Unknown,
        Group,
        Single,
        Orchestra,
        Duo
    }

    public enum SexType
    {
        Unknown,
        Feminin,
        Masculin,
        Mixed
    }

    public delegate bool PrepareProgressDelegate(double percentage);


    public class DataBase
    {
        private SqlCeConnection connection;

        public SqlCeConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        private string dataBasePath;
        /// <summary>
        /// Der vollständige Pfad zur Datenbank.
        /// </summary>
        public string DataBasePath
        {
            get { return dataBasePath; }
            set { dataBasePath = value; }
        }

        private MasterConfig master;
        /// <summary>
        /// Die Datenbank-Felder und sonstige Einstellungen
        /// </summary>
        public MasterConfig Master
        {
            get { return master; }
            set { master = value; }
        }

        /// <summary>
        /// Alle verfügbaren Genres...
        /// </summary>
        public Categories AllCategories
        {
            get;
            set;
        }

        /// <summary>
        /// Alle verfügbaren Medien
        /// </summary>
        public Mediums AllMediums
        {
            get;
            set;
        }

        /// <summary>
        /// Alle verfügbaren Labels.
        /// </summary>
        public List<string> AllLabels
        {
            get;
            set;
        }

        /// <summary>
        /// Alle verfügbaren Languages.
        /// </summary>
        public List<string> AllLanguages
        {
            get;
            set;
        }

        private bool readOnly;
        /// <summary>
        /// True, wenn die Datenbank im Readonly-Modus geöffnet wurde.
        /// </summary>
        public bool ReadOnly
        {
            get { return readOnly; }
        }

        public const int MaximumNumberOfCodes = 26;

        /// <summary>
        /// Alle Kennzeichen
        /// </summary>
        public string[] Codes
        {
            get;
            set;
        }

        /// <summary>
        /// True, wenn die Datenbank geöffnet ist.
        /// </summary>
        public bool IsOpened { get; set; }

        /// <summary>
        /// Datenbank öffnen
        /// </summary>
        /// <param name="filename"></param>
        public void Open(string filename)
        {
            Open(filename, false, false);
        }

                /// <summary>
        /// Datenbank öffnen
        /// </summary>
        /// <param name="filename"></param>
        public void Open(string filename, bool readOnly, bool setDataBaseDefaults)
        {
            DataBasePath = filename;
            
            this.readOnly = readOnly;

            string dbfilename = filename;
            string connString = string.Format("Data Source=\"{0}\";Max Database Size=4091", dbfilename);

            var engine = new System.Data.SqlServerCe.SqlCeEngine(connString);
            engine.EnsureVersion40(filename);

            connection = new SqlCeConnection(connString);

            connection.Open();

            if (setDataBaseDefaults)
            {
                SetDataBaseDefaults();
                UpdateDataBaseSchema();
            }

            Master = new MasterConfig();
            Master.ReadConfig(this);

            AllCategories = GetAvailableCategories();
            AllMediums = GetAvailableMediums();
            Codes = GetAvailableCodes();
            AllLanguages = GetAvailableLanguages();
            AllLabels = GetAvailableLabels();

            IsOpened = true;
        }

        /// <summary>
        /// Aktualisiert das Datenbankschema 
        /// </summary>
        private void UpdateDataBaseSchema()
        {
            // Änderungen für Hitbase 2012
            DataTable dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Track' AND COLUMN_NAME = 'SoundFileLastModified'");
            if (dt.Rows.Count < 1)
            {
                string sql = "ALTER TABLE [Track] ADD SoundFileLastModified datetime";
                ExecuteScalar(sql);
            }

            if (!FindDataBaseIndex("Track", "SoundFileIndex"))
                ExecuteScalar("CREATE INDEX \"SoundFileIndex\" ON \"TRACK\" (SoundFile)");

            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Track' AND COLUMN_NAME = 'PlayCount'");
            if (dt.Rows.Count < 1)
            {
                string sql = "ALTER TABLE [Track] ADD PlayCount int";
                ExecuteScalar(sql);
            }

            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD' AND COLUMN_NAME = 'Created'");
            if (dt.Rows.Count < 1)
            {
                string sql = "ALTER TABLE [CD] ADD Created datetime";
                ExecuteScalar(sql);
            }

            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD' AND COLUMN_NAME = 'LastModified'");
            if (dt.Rows.Count < 1)
            {
                string sql = "ALTER TABLE [CD] ADD LastModified datetime";
                ExecuteScalar(sql);
            }

            //ExecuteScalar("DROP TABLE [Url];");
            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Url'");
            if (dt.Rows.Count < 1)
            {
                ExecuteScalar("CREATE TABLE [Url] ([UrlID] int NOT NULL IDENTITY (1,1), " +
                              "[ReferenceID] int NOT NULL, [ReferenceType] int NOT NULL, " +
                                "[UrlType] nvarchar(100) NULL, [Url] nvarchar(1000) NULL);");
                ExecuteScalar("ALTER TABLE [Url] ADD CONSTRAINT [PK_Url] PRIMARY KEY ([UrlID]);");
                ExecuteScalar("CREATE UNIQUE INDEX [UQ__Url__00000000000001D6] ON [Url] ([UrlID] ASC);");
                ExecuteScalar("CREATE INDEX [Index1] ON [Url] ([ReferenceID] ASC);");
                ExecuteScalar("CREATE INDEX [Index2] ON [Url] ([ReferenceType] ASC);");

            }

            //ExecuteScalar("DROP TABLE [GroupParticipant];");
            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GroupParticipant'");
            if (dt.Rows.Count < 1)
            {
                ExecuteScalar("CREATE TABLE [GroupParticipant] ([GroupParticipantID] int NOT NULL  IDENTITY (1,1), " +
                              "[PersonGroupID] int NOT NULL, [PersonGroupParticipantID] int NOT NULL, [RoleID] int NOT NULL, " +
                              "[Begin] nvarchar(8) NOT NULL, [End] nvarchar(100) NOT NULL);");
                ExecuteScalar("ALTER TABLE [GroupParticipant] ADD CONSTRAINT [PK_GroupParticipant] PRIMARY KEY ([GroupParticipantID]);");
                ExecuteScalar("CREATE UNIQUE INDEX [UQ__GroupParticipant__00000000000001EC] ON [GroupParticipant] ([GroupParticipantID] ASC);");
                ExecuteScalar("CREATE INDEX [Index1] ON [GroupParticipant] ([PersonGroupID] ASC);");
            }

            //ExecuteScalar("DROP TABLE [Search];");
            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Search'");
            if (dt.Rows.Count < 1)
            {
                ExecuteScalar("CREATE TABLE [Search] ( " +
                              "[SearchID] int NOT NULL  IDENTITY (1,1) " +
                              ", [Name] nvarchar(100) NOT NULL " +
                              ", [Condition] ntext NULL " +
                              ", [Type] int NOT NULL);");
                ExecuteScalar("ALTER TABLE [Search] ADD CONSTRAINT [PK_Search] PRIMARY KEY ([SearchID]);");
                ExecuteScalar("CREATE UNIQUE INDEX [UQ__Search__0000000000000207] ON [Search] ([SearchID] ASC);");
                ExecuteScalar("CREATE UNIQUE INDEX [UQ__Search__000000000000020C] ON [Search] ([Name] ASC);");

            }
        }

        private bool FindDataBaseIndex(string tableName, string indexName)
        {
            DataTable dtIndexes = connection.GetSchema("INDEXES");
            bool soundFileIndexFound = false;
            foreach (DataRow row in dtIndexes.Rows)
            {
                if (row["TABLE_NAME"].ToString() == tableName && row["INDEX_NAME"].ToString() == indexName)
                {
                    soundFileIndexFound = true;
                }
            }

            return soundFileIndexFound;
        }

        /// <summary>
        /// Datenbank schließen
        /// </summary>
        public void Close()
        {
            if (connection != null)
                connection.Close();

            IsOpened = false;
        }

        /// <summary>
        /// Legt eine neue Datenbank an. Hierzu wird das Template einfach nur kopiert.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static void Create(string filename)
        {
            string templatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\template.sdf";

            File.Copy(templatePath, filename, true);

            // Noch Schreibschutz aufheben, falls vorhanden
            File.SetAttributes(filename, FileAttributes.Normal);

            // Jetzt ein paar Standardwerte setzen
            CreateDataBaseDefaults(filename);
        }

        /// <summary>
        /// Standardwerte anlegen (z.B. Genres, Medien, Datumsfeld)
        /// </summary>
        /// <param name="filename"></param>
        private static void CreateDataBaseDefaults(string filename)
        {
            DataBase db = new DataBase();
            db.Open(filename);

            /*db.AllCategories.AddNew("Rock");
            db.AllCategories.AddNew("Pop");
            db.AllCategories.AddNew("Klassik");
            db.AllCategories.AddNew("Heavy Metal");
            db.AllCategories.AddNew("Soundtrack");
            db.AllCategories.AddNew("Sound effects");
            db.AllCategories.AddNew("Country");
            db.AllCategories.AddNew("Rap");
            db.AllCategories.AddNew("Reggae");
            db.AllCategories.AddNew("Blues");
            db.AllCategories.AddNew("Jazz");
            db.AllCategories.AddNew("Hip Hop");
            db.AllCategories.AddNew("Dance");
            db.AllCategories.AddNew("Soul");
            db.AllCategories.AddNew("Techno");
            db.AllCategories.AddNew("Diverses");*/



            db.AllCategories.AddNew("Blues");
            db.AllCategories.AddNew("Classic Rock");
            db.AllCategories.AddNew("Country");
            db.AllCategories.AddNew("Dance");
            db.AllCategories.AddNew("Disco");
            db.AllCategories.AddNew("Funk");
            db.AllCategories.AddNew("Grunge");
            db.AllCategories.AddNew("Hip-Hop");
            db.AllCategories.AddNew("Jazz");
            db.AllCategories.AddNew("Metal");
            db.AllCategories.AddNew("New Age");
            db.AllCategories.AddNew("Oldies");
            db.AllCategories.AddNew("Other");
            db.AllCategories.AddNew("Pop");
            db.AllCategories.AddNew("R&B");
            db.AllCategories.AddNew("Rap");
            db.AllCategories.AddNew("Reggae");
            db.AllCategories.AddNew("Rock");
            db.AllCategories.AddNew("Techno");
            db.AllCategories.AddNew("Industrial");
            db.AllCategories.AddNew("Alternative");
            db.AllCategories.AddNew("Ska");
            db.AllCategories.AddNew("Death Metal");
            db.AllCategories.AddNew("Pranks");
            db.AllCategories.AddNew("Soundtrack");
            db.AllCategories.AddNew("Euro-Techno");
            db.AllCategories.AddNew("Ambient");
            db.AllCategories.AddNew("Trip-Hop");
            db.AllCategories.AddNew("Vocal");
            db.AllCategories.AddNew("Jazz&Funk");
            db.AllCategories.AddNew("Fusion");
            db.AllCategories.AddNew("Trance");
            db.AllCategories.AddNew("Classical");
            db.AllCategories.AddNew("Instrumental");
            db.AllCategories.AddNew("Acid");
            db.AllCategories.AddNew("House");
            db.AllCategories.AddNew("Game");
            db.AllCategories.AddNew("Sound Clip");
            db.AllCategories.AddNew("Gospel");
            db.AllCategories.AddNew("Noise");
            db.AllCategories.AddNew("Alternative Rock");
            db.AllCategories.AddNew("Bass");
            db.AllCategories.AddNew("Soul");
            db.AllCategories.AddNew("Punk");
            db.AllCategories.AddNew("Space");
            db.AllCategories.AddNew("Meditative");
            db.AllCategories.AddNew("Instrumental Pop");
            db.AllCategories.AddNew("Instrumental Rock");
            db.AllCategories.AddNew("Ethnic");
            db.AllCategories.AddNew("Gothic");
            db.AllCategories.AddNew("Darkwave");
            db.AllCategories.AddNew("Techno-Industrial");
            db.AllCategories.AddNew("Electronic");
            db.AllCategories.AddNew("Pop-Folk");
            db.AllCategories.AddNew("Eurodance");
            db.AllCategories.AddNew("Dream");
            db.AllCategories.AddNew("Southern Rock");
            db.AllCategories.AddNew("Comedy");
            db.AllCategories.AddNew("Cult");
            db.AllCategories.AddNew("Gangsta");
            db.AllCategories.AddNew("Top 40");
            db.AllCategories.AddNew("Christian Rap");
            db.AllCategories.AddNew("Pop/Funk");
            db.AllCategories.AddNew("Jungle");
            db.AllCategories.AddNew("Native US");
            db.AllCategories.AddNew("Cabaret");
            db.AllCategories.AddNew("New Wave");
            db.AllCategories.AddNew("Psychedelic");
            db.AllCategories.AddNew("Rave");
            db.AllCategories.AddNew("Showtunes");
            db.AllCategories.AddNew("Trailer");
            db.AllCategories.AddNew("Lo-Fi");
            db.AllCategories.AddNew("Tribal");
            db.AllCategories.AddNew("Acid Punk");
            db.AllCategories.AddNew("Acid Jazz");
            db.AllCategories.AddNew("Polka");
            db.AllCategories.AddNew("Retro");
            db.AllCategories.AddNew("Musical");
            db.AllCategories.AddNew("Rock & Roll");
            db.AllCategories.AddNew("Hard Rock");
            db.AllCategories.AddNew("Folk");
            db.AllCategories.AddNew("Folk-Rock");
            db.AllCategories.AddNew("National Folk");
            db.AllCategories.AddNew("Swing");
            db.AllCategories.AddNew("Fast Fusion");
            db.AllCategories.AddNew("Bebop");
            db.AllCategories.AddNew("Latin");
            db.AllCategories.AddNew("Revival");
            db.AllCategories.AddNew("Celtic");
            db.AllCategories.AddNew("Bluegrass");
            db.AllCategories.AddNew("Avantgarde");
            db.AllCategories.AddNew("Gothic Rock");
            db.AllCategories.AddNew("Progressive Rock");
            db.AllCategories.AddNew("Psychedelic Rock");
            db.AllCategories.AddNew("Symphonic Rock");
            db.AllCategories.AddNew("Slow Rock");
            db.AllCategories.AddNew("Big Band");
            db.AllCategories.AddNew("Chorus");
            db.AllCategories.AddNew("Easy Listening");
            db.AllCategories.AddNew("Acoustic");
            db.AllCategories.AddNew("Humour");
            db.AllCategories.AddNew("Speech");
            db.AllCategories.AddNew("Chanson");
            db.AllCategories.AddNew("Opera");
            db.AllCategories.AddNew("Chamber Music");
            db.AllCategories.AddNew("Sonata");
            db.AllCategories.AddNew("Symphony");
            db.AllCategories.AddNew("Booty Bass");
            db.AllCategories.AddNew("Primus");
            db.AllCategories.AddNew("Porn Groove");
            db.AllCategories.AddNew("Satire");
            db.AllCategories.AddNew("Slow Jam");
            db.AllCategories.AddNew("Club");
            db.AllCategories.AddNew("Tango");
            db.AllCategories.AddNew("Samba (Musik)");
            db.AllCategories.AddNew("Folklore");
            db.AllCategories.AddNew("Ballad");
            db.AllCategories.AddNew("Power Ballad");
            db.AllCategories.AddNew("Rhytmic Soul");
            db.AllCategories.AddNew("Freestyle");
            db.AllCategories.AddNew("Duet");
            db.AllCategories.AddNew("Punk Rock");
            db.AllCategories.AddNew("Drum Solo");
            db.AllCategories.AddNew("Acapella");
            db.AllCategories.AddNew("Euro-House");
            db.AllCategories.AddNew("Dance Hall");
            db.AllCategories.AddNew("Goa");
            db.AllCategories.AddNew("Drum & Bass");
            db.AllCategories.AddNew("Club-House");
            db.AllCategories.AddNew("Hardcore");
            db.AllCategories.AddNew("Terror");
            db.AllCategories.AddNew("Indie");
            db.AllCategories.AddNew("BritPop");
            db.AllCategories.AddNew("Negerpunk");
            db.AllCategories.AddNew("Polsk Punk");
            db.AllCategories.AddNew("Beat");
            db.AllCategories.AddNew("Christian Gangsta Rap");
            db.AllCategories.AddNew("Heavy Metal");
            db.AllCategories.AddNew("Black Metal");
            db.AllCategories.AddNew("Crossover");
            db.AllCategories.AddNew("Contemporary Christian");
            db.AllCategories.AddNew("Christian Rock");
            db.AllCategories.AddNew("Merengue");
            db.AllCategories.AddNew("Salsa");
            db.AllCategories.AddNew("Thrash Metal");
            db.AllCategories.AddNew("Anime");
            db.AllCategories.AddNew("JPop");
            db.AllCategories.AddNew("Synthpop");











            db.AllMediums.AddNew("CD");
            db.AllMediums.AddNew("CD-Single");
            db.AllMediums.AddNew("LP");
            db.AllMediums.AddNew("MC");
            db.AllMediums.AddNew("DVD");

            // Datumsfeld (Kaufdatum)
            db.Master.DateName = StringTable.PurchaseDate;
            db.Master.WriteConfig(db);

            db.Close();
        }

        public CDQueryDataSet ExecuteCDQuery()
        {
            CDTableAdapter cdAdap = new CDTableAdapter(this);
            PersonGroupTableAdapter personGroupAdap = new PersonGroupTableAdapter(this);
            CategoryTableAdapter categoryAdap = new CategoryTableAdapter(this);
            MediumTableAdapter mediumAdap = new MediumTableAdapter(this);
            SetTableAdapter setAdap = new SetTableAdapter(this);

            CDQueryDataSet CDQuery = new CDQueryDataSet();

            cdAdap.Fill(CDQuery.CD);
            personGroupAdap.Fill(CDQuery.PersonGroup);
            categoryAdap.Fill(CDQuery.Category);
            mediumAdap.Fill(CDQuery.Medium);
            setAdap.Fill(CDQuery.Set);

            return CDQuery;
        }

        public CDQueryDataSet ExecuteTrackQuery()
        {
            CDTableAdapter cdAdap = new CDTableAdapter(this);
            TrackTableAdapter trackAdap = new TrackTableAdapter(this);
            PersonGroupTableAdapter personGroupAdap = new PersonGroupTableAdapter(this);
            CategoryTableAdapter categoryAdap = new CategoryTableAdapter(this);
            MediumTableAdapter mediumAdap = new MediumTableAdapter(this);
            SetTableAdapter setAdap = new SetTableAdapter(this);

            CDQueryDataSet CDQuery = new CDQueryDataSet();

            cdAdap.Fill(CDQuery.CD);
            trackAdap.Fill(CDQuery.Track);
            personGroupAdap.Fill(CDQuery.PersonGroup);
            categoryAdap.Fill(CDQuery.Category);
            mediumAdap.Fill(CDQuery.Medium);
            setAdap.Fill(CDQuery.Set);

            return CDQuery;
        }

        public DataView GetCDQueryView(CDQueryDataSet cdQuery, FieldCollection fields, Condition condition, SortFieldCollection sortedFields)
        {
            DataTable dt = new DataTable();
            Dictionary<Field, String> sortColumnFieldsDictionary = new Dictionary<Field, string>();
            Dictionary<Field, String> filterColumnFieldsDictionary = new Dictionary<Field, string>();

            // Zunächst die Spalten definieren
            for (int col = 0; col < fields.Count; col++)
            {
                DataColumn dataColumn = new DataColumn();
                dataColumn.Caption = GetNameOfField(fields[col]);
                dataColumn.DataType = typeof(string);
                dataColumn.ExtendedProperties.Add("FieldName", fields[col]);
                dt.Columns.Add(dataColumn);
            }

            // Jetzt die Spalten für die Sortierung
            FieldCollection sortFieldList = new FieldCollection();
            for (int col = 0; col < sortedFields.Count; col++)
            {
                if (!sortColumnFieldsDictionary.ContainsKey(sortedFields[col].Field))
                {
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.Caption = GetNameOfField(sortedFields[col].Field);
                    dataColumn.DataType = GetTypeByField(sortedFields[col].Field);
                    dataColumn.ExtendedProperties.Add("FieldName", sortedFields[col].Field);
                    dt.Columns.Add(dataColumn);
                    sortColumnFieldsDictionary.Add(sortedFields[col].Field, dataColumn.ColumnName);
                    sortFieldList.Add(sortedFields[col].Field);
                }
            }

            // Jetzt die Spalten für die Condition
            FieldCollection conditionFieldList = new FieldCollection();
            if (condition != null)
            {
                for (int col = 0; col < condition.Count; col++)
                {
                    if (!filterColumnFieldsDictionary.ContainsKey(condition[col].Field))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.Caption = GetNameOfField(condition[col].Field);
                        dataColumn.DataType = GetTypeByField(condition[col].Field);
                        dataColumn.ExtendedProperties.Add("FieldName", condition[col].Field);
                        dt.Columns.Add(dataColumn);
                        filterColumnFieldsDictionary.Add(condition[col].Field, dataColumn.ColumnName);
                        conditionFieldList.Add(condition[col].Field);
                    }
                }
            }

            // Alle Inhalte füllen
            for (int i = 0; i < cdQuery.CD.Count; i++)
            {
                List<object> values = new List<object>();
                for (int col = 0; col < fields.Count; col++)
                {
                    object value = cdQuery.CD[i].GetStringByField(this, fields[col]);

                    values.Add(value);
                }

                for (int col = 0; col < sortFieldList.Count; col++)
                {
                    object value = cdQuery.CD[i].GetValueByField(sortFieldList[col]);

                    values.Add(value);
                }

                if (condition != null)
                {
                    for (int col = 0; col < conditionFieldList.Count; col++)
                    {
                        object value = cdQuery.CD[i].GetValueByField(conditionFieldList[col]);

                        values.Add(value);
                    }
                }

                dt.Rows.Add(values.ToArray());
            }

            // Sortierung definieren
            string sortString = "";
            foreach (SortField sortField in sortedFields)
            {
                if (!string.IsNullOrEmpty(sortString))
                    sortString += ", ";

                if (sortColumnFieldsDictionary.ContainsKey(sortField.Field))
                {
                    sortString += sortColumnFieldsDictionary[sortField.Field];

                    if (sortField.SortDirection == SortDirection.Descending)
                        sortString += " DESC";
                }
            }

            dt.DefaultView.Sort = sortString;

            if (condition != null)
            {
                string filterString = "";
                // Filter definieren
                foreach (SingleCondition cond in condition)
                {
                    if (!string.IsNullOrEmpty(filterString))
                        filterString += " AND ";

                    if (filterColumnFieldsDictionary.ContainsKey(cond.Field))
                    {
                        if (GetTypeByField(cond.Field) == typeof(string))
                        {
                            filterString += filterColumnFieldsDictionary[cond.Field];

                            filterString += " " + Condition.GetNameOfOperatorForFilter(cond.Operator) + " ";

                            if (cond.Operator == Operator.Contains || cond.Operator == Operator.NotContains)
                            {
                                filterString += "'%" + cond.Value.ToString() + "%'";
                            }
                            else
                            {
                                if (cond.Operator == Operator.StartsWith)
                                    filterString += "'" + cond.Value.ToString() + "%'";
                                else
                                    filterString += "'" + cond.Value.ToString() + "'";
                            }
                        }
                        else
                        {
                            filterString += filterColumnFieldsDictionary[cond.Field];

                            filterString += " " + Condition.GetNameOfOperatorForFilter(cond.Operator) + " ";

                            filterString += cond.Value.ToString();
                        }
                    }
                }

                dt.DefaultView.RowFilter = filterString;
            }

            return dt.DefaultView;
        }

        public object ExecuteScalar(string sqlStatement)
        {
            SqlCeCommand command = new SqlCeCommand(sqlStatement, connection);
            
            return command.ExecuteScalar();
        }

        public object ExecuteScalarWithParameter(string sqlStatement, string parameter1)
        {
            SqlCeCommand command = new SqlCeCommand(sqlStatement, connection);

            command.Parameters.Add(new SqlCeParameter("@param1", parameter1));

            return command.ExecuteScalar();
        }

        public object ExecuteScalarWithParameter(string sqlStatement, string parameter1, string parameter2)
        {
            SqlCeCommand command = new SqlCeCommand(sqlStatement, connection);

            command.Parameters.Add(new SqlCeParameter("@param1", parameter1));
            command.Parameters.Add(new SqlCeParameter("@param2", parameter2));

            return command.ExecuteScalar();
        }

        public object ExecuteScalar(string sqlStatement, SqlCeTransaction trans)
        {
            SqlCeCommand command = new SqlCeCommand(sqlStatement, connection);
            command.Transaction = trans;
            return command.ExecuteScalar();
        }

        /// <summary>
        /// Führt die angegebene SQL-Ausweisung aus und liefert die Anzahl der geänderten Records zurück.
        /// (Für UPDATE, INSERT und DELETE)
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sqlStatement)
        {
            SqlCeCommand command = new SqlCeCommand(sqlStatement, connection);

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Führt die angegebene SQL-Ausweisung aus und liefert die Anzahl der geänderten Records zurück.
        /// (Für UPDATE, INSERT und DELETE)
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sqlStatement, SqlCeTransaction trans)
        {
            SqlCeCommand command = new SqlCeCommand(sqlStatement, connection, trans);

            return command.ExecuteNonQuery();
        }

        public PersonGroupDataSet.PersonGroupRow GetPersonGroupRowByName(string name, bool createIfNotFound)
        {
            Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupTableAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this);

            PersonGroupDataSet.PersonGroupRow personGroupRow = personGroupTableAdapter.GetPersonGroupByName(name, createIfNotFound);

            return personGroupRow;
        }

        public PersonGroupDataSet.PersonGroupRow GetPersonGroupRowById(int personGroupId)
        {
            Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter personGroupTableAdapter = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this);

            PersonGroupDataSet.PersonGroupRow personGroupRow = personGroupTableAdapter.GetPersonGroupById(personGroupId);

            return personGroupRow;
        }

        public CD GetCDByIdentity(string identity)
        {
            try
            {
                string sql = string.Format("SELECT CDID FROM CD WHERE [Identity]='{0}'", identity);
                object id = ExecuteScalar(sql);
                if (id != null)
                    return GetCDById((int)id);
                else
                    return null;
            }
            catch (Exception e)
            {
                Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
                formEx.ShowDialog();
                return null;
            }
        }

        public CDQueryDataSet GetCDQueryById(int id)
        {
            CDTableAdapter cdAdap = new CDTableAdapter(this);
            TrackTableAdapter trackAdap = new TrackTableAdapter(this);
            PersonGroupTableAdapter personGroupAdap = new PersonGroupTableAdapter(this);
            SetTableAdapter setAdap = new SetTableAdapter(this);
            CategoryTableAdapter catAdap = new CategoryTableAdapter(this);
            MediumTableAdapter mediumAdap = new MediumTableAdapter(this);

            CDQueryDataSet CDQuery = new CDQueryDataSet();

            cdAdap.FillById(CDQuery.CD, id);
            trackAdap.FillByCDID(CDQuery.Track, CDQuery.CD[0].CDID);
            personGroupAdap.ClearBeforeFill = false;
            personGroupAdap.FillById(CDQuery.PersonGroup, CDQuery.CD[0].ArtistID);
            if (!CDQuery.CD[0].IsComposerIDNull() && CDQuery.CD[0].ComposerID != CDQuery.CD[0].ArtistID)
                personGroupAdap.FillById(CDQuery.PersonGroup, CDQuery.CD[0].ComposerID);
            foreach (CDQueryDataSet.TrackRow row in CDQuery.Track.Rows)
            {
                if (row.ArtistID != CDQuery.CD[0].ArtistID)
                    personGroupAdap.FillById(CDQuery.PersonGroup, row.ArtistID);
                if (!row.IsComposerIDNull() && row.ComposerID != CDQuery.CD[0].ComposerID)
                    personGroupAdap.FillById(CDQuery.PersonGroup, row.ComposerID);
            }
//            personGroupAdap.Fill(CDQuery.PersonGroup);
            catAdap.Fill(CDQuery.Category);
            mediumAdap.Fill(CDQuery.Medium);
            setAdap.Fill(CDQuery.Set);

            return CDQuery;
        }

        public CD GetCDById(int id)
        {
            CDQueryDataSet CDQuery = GetCDQueryById(id);

            CD CD = new CD();

            CopyCDData(CDQuery, CD);

            return CD;

        }

        /// <summary>
        /// Füllt die CD-Daten anhand der Identity.
        /// </summary>
        /// <param name="cd"></param>
        /// <returns>true, wenn die CD gefunden wurde, ansonsten false.</returns>
        public bool FillCDByIdentity(CD cd)
        {
            string sql = string.Format("SELECT CDID FROM CD WHERE [Identity]='{0}'", cd.Identity);
            object id = ExecuteScalar(sql);
            if (id != null)
            {
                CDQueryDataSet CDQuery = GetCDQueryById((int)id);

                // Überflüssige Tracks löschen, kann sein, dass welche gelöscht wurden.
                for (int i=cd.Tracks.Count-1;i>=CDQuery.Track.Count;i--)
                    cd.Tracks.RemoveAtFromThread(i);

                CopyCDData(CDQuery, cd);

                return true;
            }

            return false;
        }

        private void CopyCDData(CDQueryDataSet CDQuery, CD CD)
        {
            foreach (Field field in FieldHelper.GetAllCDFields(false))
            {
                CD.SetValueToField(field, CDQuery.CD[0].GetValueByField(field));
            }
            CD.ID = CDQuery.CD[0].CDID;

            for (int i = 0; i < CDQuery.Track.Count; i++)
            {
                foreach (Field field in FieldHelper.GetAllTrackFields(false))
                {
                    CD.SetTrackValueToField(i, field, CDQuery.Track[i].GetValueByField(this, field));
                }

                CD.Tracks[i].ID = CDQuery.Track[i].TrackID;
                CD.Tracks[i].CDID = CD.ID;
            }

            LoadParticipants(CD);
            LoadPrograms(CD);
            LoadIndexes(CD);
        }

        public CDQueryDataSet GetTrackQueryById(int id)
        {
            TrackTableAdapter trackAdap = new TrackTableAdapter(this);
            PersonGroupTableAdapter personGroupAdap = new PersonGroupTableAdapter(this);
            CategoryTableAdapter catAdap = new CategoryTableAdapter(this);

            CDQueryDataSet CDQuery = new CDQueryDataSet();

            trackAdap.FillById(CDQuery.Track, id);


            personGroupAdap.ClearBeforeFill = false;
            personGroupAdap.FillById(CDQuery.PersonGroup, CDQuery.Track[0].ArtistID);
            if (!CDQuery.Track[0].IsComposerIDNull() && CDQuery.Track[0].ComposerID != CDQuery.Track[0].ArtistID)
                personGroupAdap.FillById(CDQuery.PersonGroup, CDQuery.Track[0].ComposerID);

            //personGroupAdap.Fill(CDQuery.PersonGroup);
            catAdap.Fill(CDQuery.Category);

            return CDQuery;
        }

        public Track GetTrackById(int id)
        {
            CDQueryDataSet CDQuery = GetTrackQueryById(id);

            // Track nicht vorhanden
            if (CDQuery.Track.Count < 1)
                return null;

            Track track = new Track();

            foreach (Field field in FieldHelper.GetAllTrackFields(false))
            {
                track.SetValueToField(field, CDQuery.Track[0].GetValueByField(this, field));
            }

            track.ID = id;
            track.CDID = CDQuery.Track[0].CDID;

            return track;
        }

        public Track GetTrackBySoundfile(string soundfile)
        {
            TrackTableAdapter trackAdap = new TrackTableAdapter(this);
            CDQueryDataSet CDQuery = new CDQueryDataSet();

            trackAdap.FillBySoundfile(CDQuery.Track, soundfile);

            if (CDQuery.Track.Count < 1)
                return null;

            Track track = new Track();

            foreach (Field field in FieldHelper.GetAllTrackFields(false))
            {
                track.SetValueToField(field, CDQuery.Track[0].GetValueByField(this, field));
            }

            track.ID = CDQuery.Track[0].TrackID;
            track.CDID = CDQuery.Track[0].CDID;

            return track;
        }

        /// <summary>
        /// Prüft, ob das Soundfile nicht snychronisiert wird (d.h. Teil einer CD, MusicCD, etc). Es werden nur ManagedSoundFiles-Alben synchronisiert.
        /// </summary>
        /// <param name="soundfile"></param>
        /// <returns></returns>
        public bool IsSoundfileNotSynchronized(string soundfile)
        {
            string sql = string.Format("SELECT CD.[Type] FROM Track inner join CD on track.cdid = cd.cdid where Soundfile = @param1");

            object result = ExecuteScalarWithParameter(sql, soundfile) ;

            if (result != null && (AlbumType)(int)result != AlbumType.ManagedSoundFiles)
                return true;
            else
                return false;
        }



        public string GetSoundfileByTrackId(int trackId)
        {
            string sql = string.Format("SELECT Soundfile FROM Track where TrackID = {0}", trackId);
            
            string result = ExecuteScalar(sql) as string;

            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public PersonGroup GetPersonGroupByName(string name, bool createIfNotFound)
        {
            if (name == null)
                return null;

            Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter pgta = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this);
            PersonGroupDataSet.PersonGroupDataTable dt = pgta.GetDataByName(name);

            if (dt.Rows.Count == 0)
            {
                if (!createIfNotFound)
                    return null;

                pgta.Insert(name, name, null, null, null, null, null, null, null, null);
                dt = pgta.GetDataByName(name);
            }

            return GetPersonGroupByDataTable(dt);
        }

        public PersonGroup GetPersonGroupById(int id)
        {
            Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter pgta = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(this);
            PersonGroupDataSet.PersonGroupDataTable dt = pgta.GetDataById(id);

            return GetPersonGroupByDataTable(dt);
        }

        private PersonGroup GetPersonGroupByDataTable(PersonGroupDataSet.PersonGroupDataTable dt)
        {
            PersonGroup personGroup = new PersonGroup();

            if (dt.Rows.Count == 1)
            {
                personGroup.ID = dt[0].PersonGroupID;
                personGroup.Name = dt[0].Name;
                personGroup.SaveAs = dt[0].SaveAs;
                personGroup.Type = dt[0].IsTypeNull() ? PersonGroupType.Unknown : (PersonGroupType)dt[0].Type;
                personGroup.Sex = dt[0].IsSexNull() ? SexType.Unknown : (SexType)dt[0].Sex;
                personGroup.Country = dt[0].Country;
                personGroup.Birthday = dt[0].BirthDay;
                personGroup.DayOfDeath = dt[0].DayOfDeath;
                personGroup.ImageFilename = dt[0].ImageFilename;
                personGroup.Homepage = dt[0].URL;
                personGroup.Comment = dt[0].Comment;
            }

            // Jetzt die URLs auslesen
            UrlDataSetTableAdapters.UrlTableAdapter urlta = new UrlDataSetTableAdapters.UrlTableAdapter(this);
            UrlDataSet.UrlDataTable urlDataTable = urlta.GetDataByPersonGroupID(personGroup.ID);
            foreach (UrlDataSet.UrlRow row in urlDataTable.Rows)
            {
                Url newUrl = new Url();
                newUrl.UrlType = row.UrlType;
                newUrl.Link = row.Url;
                personGroup.Urls.Add(newUrl);
            }

            // Jetzt noch die Mitwirkenden der Gruppe
            GroupParticipantDataSetTableAdapters.GroupParticipantTableAdapter participantTableAdapter = new GroupParticipantDataSetTableAdapters.GroupParticipantTableAdapter(this);
            GroupParticipantDataSet.GroupParticipantDataTable participantsDataTable = participantTableAdapter.GetDataByPersonGroupID(personGroup.ID);
            foreach (GroupParticipantDataSet.GroupParticipantRow row in participantsDataTable.Rows)
            {
                GroupParticipant newParticipant = new GroupParticipant();
                newParticipant.Name = GetPersonGroupNameById(row.PersonGroupParticipantID);
                newParticipant.Role = this.GetRoleById(row.RoleID).Name;
                newParticipant.Begin = row.Begin;
                newParticipant.End = row.End;

                personGroup.Participants.Add(newParticipant);
            }

            return personGroup;
        }

        private string GetPersonGroupNameById(int personGroupId)
        {
            string name = "";
            
            name = ExecuteScalar("Select Name from PersonGroup WHERE PersonGroupId=" + personGroupId.ToString()) as string;

            return name;

        }

        private void LoadPrograms(CD CD)
        {
            ProgramDataSetTableAdapters.ProgramTableAdapter programsta = new ProgramDataSetTableAdapters.ProgramTableAdapter(this);
            ProgramDataSet.ProgramDataTable pdt = programsta.GetDataByCdId(CD.ID);

            CD.Programs = new ProgramList();

            foreach (ProgramDataSet.ProgramRow p in pdt)
            {
                Program newProgram = new Program();
                newProgram.Name = p.Name;

                string[] tracks = p.Tracks.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                List<int> trackList = new List<int>();
                foreach (string track in tracks)
                    trackList.Add(Convert.ToInt32(track));
                newProgram.Tracks = trackList.ToArray();
                newProgram.IsStandard = p.IsStandard;

                CD.Programs.Add(newProgram);
            }
        }

        private void LoadIndexes(CD CD)
        {
            IndexDataSetTableAdapters.IndexTableAdapter indexesta = new IndexDataSetTableAdapters.IndexTableAdapter(this);
            IndexDataSet.IndexDataTable idt = indexesta.GetDataByCdId(CD.ID);

            CD.Indexes = new IndexList();

            foreach (IndexDataSet.IndexRow index in idt)
            {
                Index newIndex = new Index(index.Name, index.Position);

                CD.Indexes.Add(newIndex);
            }
        }

        private void LoadParticipants(CD CD)
        {
            ParticipantDataSetTableAdapters.ParticipantTableAdapter participantsta = new ParticipantDataSetTableAdapters.ParticipantTableAdapter(this);
            ParticipantDataSet.ParticipantDataTable pdt = participantsta.GetDataByCdId(CD.ID);

            CD.Participants = new ParticipantList();

            foreach (ParticipantDataSet.ParticipantRow p in pdt)
        	{
                Participant newParticipant = new Participant();
                newParticipant.Name = GetPersonGroupRowById(p.PersonGroupID).Name;
                newParticipant.Role = GetRoleById(p.RoleID).Name;
                newParticipant.TrackNumber = p.TrackNumber;
                newParticipant.Comment = p.Comment;

                CD.Participants.Add(newParticipant);
	        }
        }

        public static string[] GetAvailablePersonGroupTypes()
        {
            string[] artistTypes = new string[5];

            artistTypes[0] = "<" + StringTable.Unknown + ">";
            artistTypes[1] = StringTable.GroupMulti;
            artistTypes[2] = StringTable.GroupSingle;
            artistTypes[3] = StringTable.Orchester;
            artistTypes[4] = StringTable.Duo;

            return artistTypes;
        }

        public static string[] GetAvailablePersonGroupSex()
        {
            string[] artistSex = new string[4];

            artistSex[0] = "<" + StringTable.Unknown + ">";
            artistSex[1] = StringTable.Feminin;
            artistSex[2] = StringTable.Masculin;
            artistSex[3] = StringTable.Mixed;

            return artistSex;
        }

        public static string GetNameOfPersonGroupType(PersonGroupType artistType)
        {
            string[] artistTypes = GetAvailablePersonGroupTypes();

            // Darf eigentlich nicht passieren. Damit Hitbase aber nicht abstürzt,
            // hier eine Sicherheitsabfrage.
            if ((int)artistType < 0 || (int)artistType >= artistTypes.Length)
                return artistTypes[(int)PersonGroupType.Unknown];

            return artistTypes[(int)artistType];
        }

        public static string GetNameOfPersonGroupSex(SexType sex)
        {
            string[] artistSex = GetAvailablePersonGroupSex();

            // Darf eigentlich nicht passieren. Damit Hitbase aber nicht abstürzt,
            // hier eine Sicherheitsabfrage.
            if ((int)sex < 0 || (int)sex >= artistSex.Length)
                return artistSex[(int)SexType.Unknown];

            return artistSex[(int)sex];
        }

        public void SetPersonGroupImage(int personGroupId, string filename)
        {
            string sql = string.Format("UPDATE PersonGroup Set ImageFilename='{0}' Where PersonGroupId={1}", filename, personGroupId);
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Liefert alle bereits erfassten Sprachen der Datenbank zurück.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAvailableLanguages()
        {
            SqlCeCommand command = new SqlCeCommand("SELECT DISTINCT Language FROM CD ORDER BY Language", Connection);

            SqlCeDataReader reader = command.ExecuteReader();

            List<String> languages = new List<string>();
            while (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader[0].ToString()))
                    languages.Add(reader[0].ToString());
            }
            reader.Close();

            command = new SqlCeCommand("SELECT DISTINCT Language FROM Track ORDER BY Language", Connection);

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                string language = reader[0].ToString();
                if (!string.IsNullOrEmpty(language))
                {
                    if (!languages.Contains(language))
                        languages.Add(language);
                }
            }
            reader.Close();

            return languages.OrderBy(x => x).ToList();
        }

        public System.Collections.IEnumerable GetAvailableLocations()
        {
            SqlCeCommand command = new SqlCeCommand("SELECT DISTINCT Location FROM CD ORDER BY Location", Connection);

            SqlCeDataReader reader = command.ExecuteReader();

            List<String> locations = new List<string>();
            while (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader[0].ToString()))
                    locations.Add(reader[0].ToString());
            }
            reader.Close();

            return locations;
        }

        /// <summary>
        /// Liefert alle bereits erfassten Labels der Datenbank zurück.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAvailableLabels()
        {
            SqlCeCommand command = new SqlCeCommand("SELECT DISTINCT Label FROM CD ORDER BY Label", Connection);

            SqlCeDataReader reader = command.ExecuteReader();

            List<String> languages = new List<string>();
            while (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader[0].ToString()))
                    languages.Add(reader[0].ToString());
            }
            reader.Close();

            return languages;
        }

        public static List<String> GetAvailableCountries(PersonGroupDataSet.PersonGroupDataTable personGroupDataTable)
        {
            // Vorhandene Länder ermitteln
            List<String> distinctValues = new List<String>();
            foreach (PersonGroupDataSet.PersonGroupRow row in personGroupDataTable.Rows)
            {
                if (row.Country != null && !distinctValues.Contains(row.Country) && row.Country.Length > 0)
                    distinctValues.Add(row.Country);
            }

            return distinctValues;
        }

        public string[] GetAvailableCountries()
        {
            SqlCeCommand command = new SqlCeCommand("SELECT DISTINCT Country FROM PersonGroup ORDER BY Country", Connection);

            SqlCeDataReader reader = command.ExecuteReader();

            List<String> countries = new List<string>();
            while (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader[0].ToString()))
                    countries.Add(reader[0].ToString());
            }
            reader.Close();

            return countries.ToArray();
        }

        public Categories GetAvailableCategories()
        {
            SqlCeCommand command = new SqlCeCommand("SELECT [Name], CategoryID, [Order] FROM Category ORDER BY [Name]", Connection);

            SqlCeDataReader reader = command.ExecuteReader();

            Categories categories = new Categories(this);
            while (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader[0].ToString()))
                {
                    Category category = new Category();
                    category.CategoryID = (int)reader[1];
                    category.Name = reader[0].ToString();
                    category.Order = (int)reader[2];

                    categories.Add(category);
                }
            }
            reader.Close();

            return categories;
        }

        public Mediums GetAvailableMediums()
        {
            MediumDataSetTableAdapters.MediumTableAdapter mta = new MediumDataSetTableAdapters.MediumTableAdapter(this);
            MediumDataSet.MediumDataTable dt = mta.GetData();

            Mediums mediums = new Mediums(this);
            foreach (MediumDataSet.MediumRow medium in dt.Rows)
            {
                mediums.Add(medium);
            }

            return mediums;
        }

        private string[] GetAvailableCodes()
        {
            SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM [Code]", connection);
            SqlCeDataReader reader = cmd.ExecuteReader();

            string[] codes = new string[26];
            while (reader.Read())
            {
                string letter = (string)reader["letter"];
                string description = (string)reader["description"];
                if (!string.IsNullOrEmpty(letter) && letter[0] >= 'A' && letter[0] <= 'Z')
                {
                    int index = letter[0] - 65;
                    codes[index] = description;
                }
            }

            return codes;
        }

        public void SaveCodes()
        {
	        int i;
	        String sqlStr;
        	
	        if (ReadOnly)
		        return;
        	
            SqlCeTransaction trans = connection.BeginTransaction();
        	
	        ExecuteNonQuery("DELETE FROM [Code]", trans);
        	
	        for (i=0;i<26;i++)
	        {
		        if (!string.IsNullOrEmpty(Codes[i]))
		        {
			        string code = DataBase.SqlPrepare(Codes[i]);
                    string letter = Convert.ToChar(65 + i).ToString();
			        sqlStr = string.Format("INSERT INTO [Code] (letter, description) VALUES ('{0}', '{1}')", letter, code);
			        ExecuteNonQuery(sqlStr, trans);
		        }
	        }
        	
            trans.Commit();
        }

        /// <summary>
        /// Ersetzt ungültige Zeichen im angegebenen String, um ihn für ein SQL-Statement (Value) benutzen zu können.
        /// </summary>
        /// <param name="sqlParam"></param>
        /// <returns></returns>
        public static string SqlPrepare(string sqlParam)
        {
            return sqlParam.Replace("'", "''");
        }

        /// <summary>
        /// Liefert alle definierten Sets zurück.
        /// </summary>
        /// <returns></returns>
        public string[] GetAvailableSets()
        {
            SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM [Set] ORDER BY [Name]", connection);
            SqlCeDataReader reader = cmd.ExecuteReader();

            List<string> sets = new List<string>();
            while (reader.Read())
            {
                sets.Add((string)reader["Name"]);
            }

            return sets.ToArray();
        }

        public int GetIdByCategory(string category)
        {
            CategoryDataSetTableAdapters.CategoryTableAdapter cta = new CategoryDataSetTableAdapters.CategoryTableAdapter(this);
            CategoryDataSet.CategoryDataTable dt = cta.GetDataByName(category);

            if (dt.Rows.Count > 0)
                return dt[0].CategoryID;

            return -1;
        }

        public int GetIdByMedium(string medium)
        {
            MediumDataSetTableAdapters.MediumTableAdapter mta = new MediumDataSetTableAdapters.MediumTableAdapter(this);
            MediumDataSet.MediumDataTable dt = mta.GetDataByName(medium);

            if (dt.Rows.Count > 0)
                return dt[0].MediumID;

            return -1;
        }

        public int GetIdBySet(string setName, bool createIfNotFound)
        {
            SetDataSetTableAdapters.SetTableAdapter sta = new Big3.Hitbase.DataBaseEngine.SetDataSetTableAdapters.SetTableAdapter(this);
            SetDataSet.SetDataTable dt = sta.GetDataByName(setName);

            if (dt.Rows.Count > 0)
            {
                return dt[0].SetID;
            }
            else
            {
                if (createIfNotFound && !string.IsNullOrEmpty(setName))
                {
                    SetDataSet.SetRow setRow = dt.NewSetRow();
                    setRow.Name = setName;
                    dt.AddSetRow(setRow);
                    sta.Update(setRow);
                    int setid = (int)(decimal)ExecuteScalar("SELECT @@IDENTITY");
                    return setid;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool FindArtist(string Text, out string foundText)
        {
            return FindGeneric("PersonGroup", "Name", Text, out foundText);
        }

        public bool FindCDTitle(string Text, out string foundText)
        {
            return FindGeneric("CD", "Title", Text, out foundText);
        }

        public bool FindTrackTitle(string Text, out string foundText)
        {
            return FindGeneric("Track", "Title", Text, out foundText);
        }

        private bool FindGeneric(string table, string field, string Text, out string foundText)
        {
            bool found = false;
            foundText = null;

            try
            {
                string sqlQuery = string.Format("SELECT TOP(1) {2} FROM {1} WHERE {2} like '{0}%' order by {2}", Text, table, field);
                SqlCeCommand command = new SqlCeCommand(sqlQuery, Connection);

                SqlCeDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader[0].ToString()))
                    {
                        foundText = reader[0].ToString();
                        found = true;
                    }
                }
                reader.Close();
            }
            catch (Exception e)
            {
            }

            return found;
        }

        /// <summary>
        /// Prüft, ob die angegebene Archivnummer bereits in der Datenbank vorhanden ist.
        /// </summary>
        /// <param name="archiveNumber"></param>
        /// <returns>Die ID des Datensatzes, oder 0 wenn nicht vorhanden.</returns>
        public int CheckArchiveNumber(string archiveNumber)
        {
            if (String.IsNullOrEmpty(archiveNumber))
                return 0;

            string sql = string.Format("select CDID from cd where ArchiveNumber = '{0}'", archiveNumber);

            object result = ExecuteScalar(sql);

            if (result != null)
            {
                return (int)result;
            }

            return 0;
        }


        /// <summary>
        /// Liefert die nächste freie Archivnummer zurück .
        /// </summary>
        /// <returns>Die nächste freie Archivnummer (als String, weil das Feld in der DB ein String ist.)</returns>
        public string GetNextFreeArchiveNumber()
        {
            if (Settings.Current.FreeArchiveNumberSearchWithGaps)
                return GetNextFreeArchiveNumberWithGaps();
            else
                return GetNextFreeArchiveNumberWithoutGaps();
        }

        /// <summary>
        /// Liefert die nächste freie Archivnummer zurück (ohne Lückensuche).
        /// </summary>
        /// <returns>Die nächste freie Archivnummer (als String, weil das Feld in der DB ein String ist.)</returns>
        private string GetNextFreeArchiveNumberWithoutGaps()
        {
            try
            {

                SqlCeCommand command = new SqlCeCommand("SELECT ArchiveNumber FROM CD", Connection);

                SqlCeDataReader reader = command.ExecuteReader();

                int maxNumber = 0;
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        int archiveNumber = 0;
                        Int32.TryParse((string)reader[0], out archiveNumber);
                        if (archiveNumber > maxNumber)
                            maxNumber = archiveNumber;
                    }
                }
                reader.Close();

                if (maxNumber > 0)
                {
                    return string.Format("{0}", maxNumber + 1);
                }
            }
            catch (Exception e)
            {
                string msg = string.Format(StringTable.ErrorDetermineNextFreeArchiveNumber, e.Message);
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return "";
        }


        /// <summary>
        /// Liefert die nächste freie Archivnummer zurück (mit Lückensuche).
        /// </summary>
        /// <returns>Die nächste freie Archivnummer (als String, weil das Feld in der DB ein String ist.)</returns>
        private string GetNextFreeArchiveNumberWithGaps()
        {
            try
            {
                SqlCeCommand command = new SqlCeCommand("SELECT ArchiveNumber FROM CD", Connection);

                SqlCeDataReader reader = command.ExecuteReader();

                // Erst alle Archivnummern als int in einer Liste speichern
                List<int> allArchiveNumbers = new List<int>();      
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        int archiveNumber = 0;
                        Int32.TryParse((string)reader[0], out archiveNumber);
                        allArchiveNumbers.Add(archiveNumber);
                    }
                }

                reader.Close();

                // Jetzt sortieren
                allArchiveNumbers.Sort();
                
                // Liste eindeutig machen
                IEnumerable<int> distictList = allArchiveNumbers.Distinct();
                
                //Jetzt die Lücke finden
                int lastNumber = -1;
                foreach (int archivNumber in distictList)
                {
                    if (lastNumber != -1 && archivNumber != lastNumber + 1)
                        return (lastNumber + 1).ToString();

                    lastNumber = archivNumber;
                }

                if (lastNumber == -1)
                    return "1";
                else
                    return (lastNumber + 1).ToString();
            }
            catch (Exception e)
            {
                string msg = string.Format(StringTable.ErrorDetermineNextFreeArchiveNumber, e.Message);
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return "";
        }

        /// <summary>
        /// Löscht die angegebene Kategorie. Alle vorhandenen CDs/Tracks mit der zu löschenden Kategorie
        /// werden mit einer vorhandenen ersetzt.
        /// </summary>
        /// <param name="categoryToDelete"></param>
        /// <param name="newCategory"></param>
        /// <returns></returns>
        public void DeleteCategory(string categoryToDelete, string newCategory)
        {
            SqlCeTransaction trans = connection.BeginTransaction();

            try
            {
                int id, idnew;
                string sqlStr;

                id = GetIdByCategory(categoryToDelete);

                if (!string.IsNullOrEmpty(newCategory))
                    idnew = GetIdByCategory(newCategory);
                else
                    idnew = 0;

                sqlStr = string.Format("DELETE FROM Category WHERE CategoryID={0}", id);
                ExecuteNonQuery(sqlStr);

                sqlStr = string.Format("UPDATE CD SET CategoryID={0} WHERE CategoryID={1}", idnew, id);
                ExecuteNonQuery(sqlStr);

                sqlStr = string.Format("UPDATE Track SET CategoryID={0} WHERE CategoryID={1}", idnew, id);
                ExecuteNonQuery(sqlStr);

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
        /// Löscht das angegebene Medium. Alle vorhandenen CDs mit dem zu löschenden Medium
        /// werden mit einem vorhandenen ersetzt.
        /// </summary>
        /// <param name="categoryToDelete"></param>
        /// <param name="newCategory"></param>
        /// <returns></returns>
        public void DeleteMedium(string mediumToDelete, string newMedium)
        {
            SqlCeTransaction trans = connection.BeginTransaction();

            try
            {
                int id, idnew;
                string sqlStr;

                id = GetIdByMedium(mediumToDelete);

                if (!string.IsNullOrEmpty(newMedium))
                    idnew = GetIdByMedium(newMedium);
                else
                    idnew = 0;

                sqlStr = string.Format("DELETE FROM Medium WHERE MediumID={0}", id);
                ExecuteNonQuery(sqlStr);

                sqlStr = string.Format("UPDATE CD SET MediumID={0} WHERE MediumID={1}", idnew, id);
                ExecuteNonQuery(sqlStr);

                trans.Commit();
            }
            catch (Exception e)
            {
                trans.Rollback();

                Big3.Hitbase.Miscellaneous.FormUnhandledException formEx = new Big3.Hitbase.Miscellaneous.FormUnhandledException(e);
                formEx.ShowDialog();
            }
        }


        #region Role Utilities
        public RoleDataSet.RoleRow GetRoleById(int id)
        {
            Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters.RoleTableAdapter roleTableAdapter = new Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters.RoleTableAdapter(this);

            return roleTableAdapter.GetRoleById(id);
        }

        public RoleDataSet.RoleRow GetRoleByName(string role, bool createIfNotFound)
        {
            Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters.RoleTableAdapter roleTableAdapter = new Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters.RoleTableAdapter(this);

            return roleTableAdapter.GetRoleByName(role, createIfNotFound);
        }

        public ObservableCollection<Role> GetAllRoles()
        {
            Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters.RoleTableAdapter roleAdapter = new Big3.Hitbase.DataBaseEngine.RoleDataSetTableAdapters.RoleTableAdapter(this);
            RoleDataSet roleDataset = new RoleDataSet();
            
            roleAdapter.Fill(roleDataset.Role);
            roleDataset.Role.DefaultView.Sort = "Name ASC";

            ObservableCollection<Role> roles = new ObservableCollection<Role>();

            foreach (DataRow role in roleDataset.Role.DefaultView.Table.Rows)
            {
                Role newRole = new Role();
                newRole.ID = (int)role["RoleID"];
                newRole.Name = (string)role["Name"];

                roles.Add(newRole);
            }

            return roles;
        }

        #endregion

        /// <summary>
        /// Liefert den Datentyp des angegebenen Feldes zurück
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Type GetTypeByField(Field field)
        {
            DataColumn col = GetDataColumnByField(field);

            return col.DataType;
        }

        /// <summary>
        /// Liefert die maximale Länge von String-Feldern zurück (um z.B. die Eingabe zu begrenzen)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static int GetMaxStringLength(Field field)
        {
            if (GetTypeByField(field) == typeof(string))
            {
                DataColumn dc = GetDataColumnByField(field);
                return dc.MaxLength;
            }

            throw new Exception("only allowed for string fields");
        }

        /// <summary>
        /// Liefert die Spalte der Tabelle CD zurück (nur die reinen CD-Felder ohne Relations-IDs)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static System.Data.DataColumn GetDataColumnByField(Field field)
        {
            CDQueryDataSet CDQuery = new CDQueryDataSet();

            switch (field)
            {
                case Field.CDID: return CDQuery.CD.CDIDColumn;

                case Field.Identity: return CDQuery.CD.IdentityColumn;
                case Field.Title: return CDQuery.CD.TitleColumn;
                case Field.Sampler: return CDQuery.CD.IsSamplerColumn;
                case Field.TotalLength: return CDQuery.CD.LengthColumn;
                case Field.NumberOfTracks: return CDQuery.CD.NumberOfTracksColumn;
                case Field.DiscNumberInCDSet: return CDQuery.CD.SetNumberColumn;
                case Field.Comment: return CDQuery.CD.CommentColumn;
                case Field.Copyright: return CDQuery.CD.CopyrightColumn;
                case Field.Label: return CDQuery.CD.LabelColumn;
                case Field.YearRecorded: return CDQuery.CD.YearRecordedColumn;
                case Field.Date: return CDQuery.CD.DateColumn;
                case Field.Codes: return CDQuery.CD.CodesColumn;
                case Field.ArchiveNumber: return CDQuery.CD.ArchiveNumberColumn;
                case Field.Rating: return CDQuery.CD.RatingColumn;
                case Field.Language: return CDQuery.CD.LanguageColumn;
                case Field.Location: return CDQuery.CD.LocationColumn;
                case Field.UPC: return CDQuery.CD.UPCColumn;
                case Field.Price: return CDQuery.CD.PriceColumn;
                case Field.CDCoverFront: return CDQuery.CD.FrontCoverColumn;
                case Field.CDCoverBack: return CDQuery.CD.BackCoverColumn;
                case Field.CDCoverLabel: return CDQuery.CD.CDLabelCoverColumn;
                case Field.OriginalCD: return CDQuery.CD.IsOriginalColumn;
                case Field.Homepage: return CDQuery.CD.URLColumn;
                case Field.AlbumType: return CDQuery.CD.TypeColumn;
                case Field.Created: return CDQuery.CD.CreatedColumn;
                case Field.LastModified: return CDQuery.CD.LastModifiedColumn;
                case Field.User1: return CDQuery.CD.User1Column;
                case Field.User2: return CDQuery.CD.User2Column;
                case Field.User3: return CDQuery.CD.User3Column;
                case Field.User4: return CDQuery.CD.User4Column;
                case Field.User5: return CDQuery.CD.User5Column;

                case Field.CDSet: return CDQuery.Set.NameColumn;
                case Field.Category: return CDQuery.Category.NameColumn;
                case Field.Medium: return CDQuery.Medium.NameColumn;

                case Field.ArtistCDName: 
                case Field.ComposerCDName:
                case Field.ArtistTrackName:
                case Field.ComposerTrackName:
                    return CDQuery.PersonGroup.NameColumn;
                case Field.ArtistCDSaveAs:
                case Field.ComposerCDSaveAs:
                case Field.ArtistTrackSaveAs:
                case Field.ComposerTrackSaveAs:
                    return CDQuery.PersonGroup.SaveAsColumn;
                case Field.ArtistCDSex:
                case Field.ComposerCDSex:
                case Field.ArtistTrackSex:
                case Field.ComposerTrackSex:
                    return CDQuery.PersonGroup.SexColumn;
                case Field.ArtistCDType:
                case Field.ComposerCDType:
                case Field.ArtistTrackType:
                case Field.ComposerTrackType:
                    return CDQuery.PersonGroup.TypeColumn;
                case Field.ArtistCDCountry:
                case Field.ComposerCDCountry:
                case Field.ArtistTrackCountry:
                case Field.ComposerTrackCountry:
                    return CDQuery.PersonGroup.CountryColumn;
                case Field.ArtistCDHomepage:
                case Field.ComposerCDHomepage:
                case Field.ArtistTrackHomepage:
                case Field.ComposerTrackHomepage:
                    return CDQuery.PersonGroup.URLColumn;
                case Field.ArtistCDDateOfBirth:
                case Field.ComposerCDDateOfBirth:
                case Field.ArtistTrackDateOfBirth:
                case Field.ComposerTrackDateOfBirth:
                    return CDQuery.PersonGroup.BirthDayColumn;
                case Field.ArtistCDDateOfDeath:
                case Field.ComposerCDDateOfDeath:
                case Field.ArtistTrackDateOfDeath:
                case Field.ComposerTrackDateOfDeath:
                    return CDQuery.PersonGroup.DayOfDeathColumn;
                case Field.ArtistCDComment:
                case Field.ComposerCDComment:
                case Field.ArtistTrackComment:
                case Field.ComposerTrackComment:
                    return CDQuery.PersonGroup.CommentColumn;
                case Field.ArtistCDImageFilename:
                case Field.ComposerCDImageFilename:
                case Field.ArtistTrackImageFilename:
                case Field.ComposerTrackImageFilename:
                    return CDQuery.PersonGroup.ImageFilenameColumn;

                case Field.TrackTitle: return CDQuery.Track.TitleColumn;
                case Field.TrackLength: return CDQuery.Track.LengthColumn;
                case Field.TrackNumber: return CDQuery.Track.TrackNumberColumn;
                case Field.TrackBpm: return CDQuery.Track.BpmColumn;
                case Field.TrackCodes: return CDQuery.Track.CodesColumn;
                case Field.TrackCategory: return CDQuery.Category.NameColumn;
                case Field.TrackComment: return CDQuery.Track.CommentColumn;
                case Field.TrackLyrics: return CDQuery.Track.LyricsColumn;
                case Field.TrackSoundFile: return CDQuery.Track.SoundFileColumn;
                case Field.TrackYearRecorded: return CDQuery.Track.YearRecordedColumn;
                case Field.TrackRating: return CDQuery.Track.RatingColumn;
                case Field.TrackChecksum: return CDQuery.Track.ChecksumColumn;
                case Field.TrackLanguage: return CDQuery.Track.LanguageColumn;
                case Field.TrackID: return CDQuery.Track.TrackIDColumn;
                case Field.TrackPlayCount: return CDQuery.Track.PlayCountColumn;
                case Field.TrackSoundFileLastModified: return CDQuery.Track.SoundFileLastModifiedColumn;

                case Field.TrackUser1: return CDQuery.Track.User1Column;
                case Field.TrackUser2: return CDQuery.Track.User2Column;
                case Field.TrackUser3: return CDQuery.Track.User3Column;
                case Field.TrackUser4: return CDQuery.Track.User4Column;
                case Field.TrackUser5: return CDQuery.Track.User5Column;

                default:
                    throw new NotImplementedException("missing field implementation: " + field.ToString());
            }
        }

        /// <summary>
        /// Liefert die Standardgröße für die Spalte in einer Tabelle
        /// für das angegebene Feld zurück.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public int GetDefaultColumnWidthOfField(Field field)
        {
            switch (field)
            {
                case Field.DiscNumberInCDSet:
                case Field.Sampler:
                case Field.TotalLength:
                case Field.NumberOfTracks:
                case Field.YearRecorded:
                case Field.Date:
                case Field.Codes:
                case Field.ArchiveNumber:
                case Field.Rating:
                case Field.Price:
                case Field.OriginalCD:
                case Field.TrackLength:
                case Field.TrackNumber:
                case Field.TrackBpm:
                case Field.TrackCodes:
                case Field.TrackYearRecorded:
                case Field.TrackRating:
                case Field.ArtistCDDateOfBirth:
                case Field.ArtistCDDateOfDeath:
                case Field.ArtistTrackDateOfBirth:
                case Field.ArtistTrackDateOfDeath:
                case Field.ComposerCDDateOfBirth:
                case Field.ComposerCDDateOfDeath:
                case Field.ComposerTrackDateOfBirth:
                case Field.ComposerTrackDateOfDeath:
                    return 60;
                case Field.Category:
                case Field.Medium:
                case Field.Language:
                case Field.Location:
                case Field.UPC:
                case Field.Identity:
                case Field.TrackCategory:
                case Field.TrackChecksum:
                case Field.TrackLanguage:
                case Field.ArtistCDType:
                case Field.ArtistCDSex:
                case Field.ArtistCDCountry:
                case Field.ArtistTrackType:
                case Field.ArtistTrackSex:
                case Field.ArtistTrackCountry:
                case Field.ComposerCDType:
                case Field.ComposerCDSex:
                case Field.ComposerCDCountry:
                case Field.ComposerTrackType:
                case Field.ComposerTrackSex:
                case Field.ComposerTrackCountry:
                    return 100;
                default:            // Der Rest bekommt ne Einheitsgröße
                    return 200;
            }
        }

        public string GetDataBaseColumnOfField(Field field)
        {
            // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!???????????
            return "";
        }

        public string GetNameOfField(Field field)
        {
            return GetNameOfField(field, false);
        }

        public string GetNameOfFieldFull(Field field, bool compact = false)
        {
            string fieldName = GetNameOfField(field, false);


            if (FieldHelper.IsCDField(field))
            {
                if (field != Field.Title || !compact)
                    fieldName += " (Album)";
            }
            else
            {
                if (!compact)
                    fieldName += " (Track)";
            }

            return fieldName;
        }

        /// <summary>
        /// Liefert den Namen des Feldes zurück.
        /// </summary>
        /// <param name="field">Feld</param>
        /// <param name="subFieldOnly">true, wenn nur der Untername des Feldes zurückgeliefert werden soll.
        /// z.B. wenn Feld = Field.ArtistCDName, dann kommt nur "Name" zurück, anstatt "Interpret, Name"</param>
        /// <returns></returns>
        public string GetNameOfField(Field field, bool subFieldOnly)
        {
            switch (field)
            {
                case Field.CDID:
                    return "CDID";

                case Field.Title:
                    return StringTable.Album;
                case Field.CDSet:
                    return StringTable.CDSet;
                case Field.DiscNumberInCDSet:
                    return StringTable.DiscNumberInCDSet;
                case Field.TotalLength:
                    return StringTable.TotalLength;
                case Field.NumberOfTracks:
                    return StringTable.NumberOfTracks;
                case Field.Sampler:
                    return StringTable.Sampler;
                case Field.Category:
                    return StringTable.Category;
                case Field.Medium:
                    return StringTable.Medium;
                case Field.Label:
                    return StringTable.Label;
                case Field.Location:
                    return StringTable.Location;
                case Field.Language:
                    return StringTable.Language;
                case Field.Rating:
                    return StringTable.Rating;
                case Field.Date:
                    return string.IsNullOrEmpty(Master.DateName) ? StringTable.Date : Master.DateName;
                case Field.Codes:
                    return StringTable.Codes;
                case Field.ArchiveNumber:
                    return StringTable.ArchiveNumber;
                case Field.UPC:
                    return StringTable.UPC;
                case Field.Homepage:
                    return StringTable.Homepage;
                case Field.Comment:
                    return StringTable.Comment;
                case Field.Copyright:
                    return StringTable.Copyright;
                case Field.Price:
                    return StringTable.Price;
                case Field.YearRecorded:
                    return StringTable.YearRecorded;
                case Field.AlbumType:
                    return StringTable.AlbumType;
                case Field.User1:
                    return string.IsNullOrEmpty(Master.UserCDFields[0].Name) ? StringTable.CDUserField + " 1" : Master.UserCDFields[0].Name;
                case Field.User2:
                    return string.IsNullOrEmpty(Master.UserCDFields[1].Name) ? StringTable.CDUserField + " 2" : Master.UserCDFields[1].Name;
                case Field.User3:
                    return string.IsNullOrEmpty(Master.UserCDFields[2].Name) ? StringTable.CDUserField + " 3" : Master.UserCDFields[2].Name;
                case Field.User4:
                    return string.IsNullOrEmpty(Master.UserCDFields[3].Name) ? StringTable.CDUserField + " 4" : Master.UserCDFields[3].Name;
                case Field.User5:
                    return string.IsNullOrEmpty(Master.UserCDFields[4].Name) ? StringTable.CDUserField + " 5" : Master.UserCDFields[4].Name;
                case Field.CDCoverFront:
                    return StringTable.CDCoverFront;
                case Field.CDCoverBack:
                    return StringTable.CDCoverBack;
                case Field.CDCoverLabel:
                    return StringTable.CDCoverLabel;
                case Field.OriginalCD:
                    return StringTable.OriginalCD;
                case Field.Identity:
                    return StringTable.Identity;
                case Field.Created:
                    return StringTable.Created;
                case Field.LastModified:
                    return StringTable.LastModified;

                case Field.TrackNumber:
                    return StringTable.TrackNumber;
                case Field.TrackTitle:
                    return StringTable.Title;
                case Field.TrackLength:
                    return StringTable.TrackLength;
                case Field.TrackBpm:
                    return StringTable.TrackBpm;
                case Field.TrackCategory:
                    return StringTable.TrackCategory;
                case Field.TrackChecksum:
                    return StringTable.TrackChecksum;
                case Field.TrackCodes:
                    return StringTable.TrackCodes;
                case Field.TrackComment:
                    return StringTable.TrackComment;
                case Field.TrackLanguage:
                    return StringTable.TrackLanguage;
                case Field.TrackLyrics:
                    return StringTable.TrackLyrics;
                case Field.TrackRating:
                    return StringTable.TrackRating;
                case Field.TrackSoundFile:
                    return StringTable.TrackSoundFile;
                case Field.TrackYearRecorded:
                    return StringTable.TrackYearRecorded;
                case Field.TrackSoundFileLastModified:
                    return StringTable.TrackSoundFileLastModified;
                case Field.TrackPlayCount:
                    return StringTable.TrackPlayCount;
                case Field.TrackUser1:
                    return string.IsNullOrEmpty(Master.UserTrackFields[0].Name) ? StringTable.TrackUserField + " 1" : Master.UserTrackFields[0].Name;
                case Field.TrackUser2:
                    return string.IsNullOrEmpty(Master.UserTrackFields[1].Name) ? StringTable.TrackUserField + " 2" : Master.UserTrackFields[1].Name;
                case Field.TrackUser3:
                    return string.IsNullOrEmpty(Master.UserTrackFields[2].Name) ? StringTable.TrackUserField + " 3" : Master.UserTrackFields[2].Name;
                case Field.TrackUser4:
                    return string.IsNullOrEmpty(Master.UserTrackFields[3].Name) ? StringTable.TrackUserField + " 4" : Master.UserTrackFields[3].Name;
                case Field.TrackUser5:
                    return string.IsNullOrEmpty(Master.UserTrackFields[4].Name) ? StringTable.TrackUserField + " 5" : Master.UserTrackFields[4].Name;

                case Field.ArtistCDName:
                    if (subFieldOnly)
                        return StringTable.Name;
                    else
                        return StringTable.Artist + ", " + StringTable.Name;
                case Field.ArtistCDSaveAs:
                    if (subFieldOnly)
                        return StringTable.SaveAs;
                    else
                        return StringTable.Artist + ", " + StringTable.SaveAs;
                case Field.ArtistCDType:
                    if (subFieldOnly)
                        return StringTable.Type;
                    else
                        return StringTable.Artist + ", " + StringTable.Type;
                case Field.ArtistCDSex:
                    if (subFieldOnly)
                        return StringTable.Sex;
                    else
                        return StringTable.Artist + ", " + StringTable.Sex;
                case Field.ArtistCDCountry:
                    if (subFieldOnly)
                        return StringTable.Country;
                    else
                        return StringTable.Artist + ", " + StringTable.Country;
                case Field.ArtistCDComment:
                    if (subFieldOnly)
                        return StringTable.Comment;
                    else
                        return StringTable.Artist + ", " + StringTable.Comment;
                case Field.ArtistCDDateOfBirth:
                    if (subFieldOnly)
                        return StringTable.DateOfBirth;
                    else
                        return StringTable.Artist + ", " + StringTable.DateOfBirth;
                case Field.ArtistCDDateOfDeath:
                    if (subFieldOnly)
                        return StringTable.DateOfDeath;
                    else
                        return StringTable.Artist + ", " + StringTable.DateOfDeath;
                case Field.ArtistCDHomepage:
                    if (subFieldOnly)
                        return StringTable.Homepage;
                    else
                        return StringTable.Artist + ", " + StringTable.Homepage;
                case Field.ArtistCDImageFilename:
                    if (subFieldOnly)
                        return StringTable.ImageFilename;
                    else
                        return StringTable.Artist + ", " + StringTable.ImageFilename;

                case Field.ComposerCDName:
                    if (subFieldOnly)
                        return StringTable.Name;
                    else
                        return StringTable.Composer + ", " + StringTable.Name;
                case Field.ComposerCDSaveAs:
                    if (subFieldOnly)
                        return StringTable.SaveAs;
                    else
                        return StringTable.Composer + ", " + StringTable.SaveAs;
                case Field.ComposerCDType:
                    if (subFieldOnly)
                        return StringTable.Type;
                    else
                        return StringTable.Composer + ", " + StringTable.Type;
                case Field.ComposerCDSex:
                    if (subFieldOnly)
                        return StringTable.Sex;
                    else
                        return StringTable.Composer + ", " + StringTable.Sex;
                case Field.ComposerCDCountry:
                    if (subFieldOnly)
                        return StringTable.Country;
                    else
                        return StringTable.Composer + ", " + StringTable.Country;
                case Field.ComposerCDComment:
                    if (subFieldOnly)
                        return StringTable.Comment;
                    else
                        return StringTable.Composer + ", " + StringTable.Comment;
                case Field.ComposerCDDateOfBirth:
                    if (subFieldOnly)
                        return StringTable.DateOfBirth;
                    else
                        return StringTable.Composer + ", " + StringTable.DateOfBirth;
                case Field.ComposerCDDateOfDeath:
                    if (subFieldOnly)
                        return StringTable.DateOfDeath;
                    else
                        return StringTable.Composer + ", " + StringTable.DateOfDeath;
                case Field.ComposerCDHomepage:
                    if (subFieldOnly)
                        return StringTable.Homepage;
                    else
                        return StringTable.Composer + ", " + StringTable.Homepage;
                case Field.ComposerCDImageFilename:
                    if (subFieldOnly)
                        return StringTable.ImageFilename;
                    else
                        return StringTable.Composer + ", " + StringTable.ImageFilename;




                case Field.ArtistTrackName:
                    if (subFieldOnly)
                        return StringTable.Name;
                    else
                        return StringTable.Artist + ", " + StringTable.Name;
                case Field.ArtistTrackSaveAs:
                    if (subFieldOnly)
                        return StringTable.SaveAs;
                    else
                        return StringTable.Artist + ", " + StringTable.SaveAs;
                case Field.ArtistTrackType:
                    if (subFieldOnly)
                        return StringTable.Type;
                    else
                        return StringTable.Artist + ", " + StringTable.Type;
                case Field.ArtistTrackSex:
                    if (subFieldOnly)
                        return StringTable.Sex;
                    else
                        return StringTable.Artist + ", " + StringTable.Sex;
                case Field.ArtistTrackCountry:
                    if (subFieldOnly)
                        return StringTable.Country;
                    else
                        return StringTable.Artist + ", " + StringTable.Country;
                case Field.ArtistTrackComment:
                    if (subFieldOnly)
                        return StringTable.Comment;
                    else
                        return StringTable.Artist + ", " + StringTable.Comment;
                case Field.ArtistTrackDateOfBirth:
                    if (subFieldOnly)
                        return StringTable.DateOfBirth;
                    else
                        return StringTable.Artist + ", " + StringTable.DateOfBirth;
                case Field.ArtistTrackDateOfDeath:
                    if (subFieldOnly)
                        return StringTable.DateOfDeath;
                    else
                        return StringTable.Artist + ", " + StringTable.DateOfDeath;
                case Field.ArtistTrackHomepage:
                    if (subFieldOnly)
                        return StringTable.Homepage;
                    else
                        return StringTable.Artist + ", " + StringTable.Homepage;
                case Field.ArtistTrackImageFilename:
                    if (subFieldOnly)
                        return StringTable.ImageFilename;
                    else
                        return StringTable.Artist + ", " + StringTable.ImageFilename;

                case Field.ComposerTrackName:
                    if (subFieldOnly)
                        return StringTable.Name;
                    else
                        return StringTable.Composer + ", " + StringTable.Name;
                case Field.ComposerTrackSaveAs:
                    if (subFieldOnly)
                        return StringTable.SaveAs;
                    else
                        return StringTable.Composer + ", " + StringTable.SaveAs;
                case Field.ComposerTrackType:
                    if (subFieldOnly)
                        return StringTable.Type;
                    else
                        return StringTable.Composer + ", " + StringTable.Type;
                case Field.ComposerTrackSex:
                    if (subFieldOnly)
                        return StringTable.Sex;
                    else
                        return StringTable.Composer + ", " + StringTable.Sex;
                case Field.ComposerTrackCountry:
                    if (subFieldOnly)
                        return StringTable.Country;
                    else
                        return StringTable.Composer + ", " + StringTable.Country;
                case Field.ComposerTrackComment:
                    if (subFieldOnly)
                        return StringTable.Comment;
                    else
                        return StringTable.Composer + ", " + StringTable.Comment;
                case Field.ComposerTrackDateOfBirth:
                    if (subFieldOnly)
                        return StringTable.DateOfBirth;
                    else
                        return StringTable.Composer + ", " + StringTable.DateOfBirth;
                case Field.ComposerTrackDateOfDeath:
                    if (subFieldOnly)
                        return StringTable.DateOfDeath;
                    else
                        return StringTable.Composer + ", " + StringTable.DateOfDeath;
                case Field.ComposerTrackHomepage:
                    if (subFieldOnly)
                        return StringTable.Homepage;
                    else
                        return StringTable.Composer + ", " + StringTable.Homepage;
                case Field.ComposerTrackImageFilename:
                    if (subFieldOnly)
                        return StringTable.ImageFilename;
                    else
                        return StringTable.Composer + ", " + StringTable.ImageFilename;
                default:
                    System.Diagnostics.Debug.Assert(false, "GetNameOfField: Unknown field");
                    return "???";
            }
        }

        /// <summary>
        /// Prüfen, ob die angegebene Kategorie benutzt wird.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsCategoryUsedCount(int categoryId, out int countCD, out int countTrack)
        {
            string sql;

            sql = string.Format("SELECT Count(*) from CD where CategoryID={0}", categoryId);
            countCD = (int)ExecuteScalar(sql);

            sql = string.Format("SELECT Count(*) from Track where CategoryID={0}", categoryId);
            countTrack = (int)ExecuteScalar(sql);

            return (countCD + countTrack) > 0;
        }

        /// <summary>
        /// Prüfen, ob das angegebene Medium benutzt wird.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsMediumUsed(int mediumId)
        {
            string sql;

            sql = string.Format("SELECT Count(*) from CD where MediumID={0}", mediumId);
            int countCD = (int)ExecuteScalar(sql);

            return (countCD > 0);
        }

        public void UpdateCategories()
        {
            AllCategories = GetAvailableCategories();
        }

        public void UpdateMediums()
        {
            AllMediums = GetAvailableMediums();
        }

        /// <summary>
        /// Liefert den Type des ten benutzerdefinierten Feldes zurück (für CD).
        /// </summary>
        /// <param name="fieldNumber"></param>
        /// <returns></returns>
        public UserFieldType GetUserFieldType(Field field)
        {
            try
            {
                switch (field)
                {
                    case Field.User1:
                        return (UserFieldType)Master.UserCDFields[0].Type;
                    case Field.User2:
                        return (UserFieldType)Master.UserCDFields[1].Type;
                    case Field.User3:
                        return (UserFieldType)Master.UserCDFields[2].Type;
                    case Field.User4:
                        return (UserFieldType)Master.UserCDFields[3].Type;
                    case Field.User5:
                        return (UserFieldType)Master.UserCDFields[4].Type;
                    case Field.TrackUser1:
                        return (UserFieldType)Master.UserTrackFields[0].Type;
                    case Field.TrackUser2:
                        return (UserFieldType)Master.UserTrackFields[1].Type;
                    case Field.TrackUser3:
                        return (UserFieldType)Master.UserTrackFields[2].Type;
                    case Field.TrackUser4:
                        return (UserFieldType)Master.UserTrackFields[3].Type;
                    case Field.TrackUser5:
                        return (UserFieldType)Master.UserTrackFields[4].Type;
                    default:
                        return UserFieldType.Text;
                }
            }
            catch (Exception e)
            {
                return UserFieldType.Text;
            }
        }

        /// <summary>
        /// Wandelt das benutzerdefinierte Feld in den entsprechenden Datentypen um.
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="fieldNumber"></param>
        /// <returns></returns>
        public object GetUserFieldValue(CD cd, int fieldNumber)
        {
            try
            {
                switch (fieldNumber)
                {
                    case 0: return GetUserFieldValue(cd.UserField1, Field.User1);
                    case 1: return GetUserFieldValue(cd.UserField2, Field.User2);
                    case 2: return GetUserFieldValue(cd.UserField3, Field.User3);
                    case 3: return GetUserFieldValue(cd.UserField4, Field.User4);
                    case 4: return GetUserFieldValue(cd.UserField5, Field.User5);
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Liefert eine Textdarstellung des angegebenen benutzerdefinierten Feldes zurück.
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetDisplayStringByUserField(string stringValue, Field field)
        {
            if (string.IsNullOrEmpty(stringValue))
                return "";

            switch (GetUserFieldType(field))
            {
                case UserFieldType.Boolean:
                    if (stringValue == "1" || String.Compare(stringValue, StringTable.Yes, true) == 0)
                        return StringTable.Yes;
                    else
                        return StringTable.No;
                case UserFieldType.Date:
                    return FormatDate(stringValue);
                case UserFieldType.Currency:
                    {
                        return Misc.FormatCurrencyValue(Misc.Atoi(stringValue));
                    }
                default:
                    return stringValue;
            }
        }

        /// <summary>
        /// Wandelt das benutzerdefinierte Feld in den entsprechenden Datentypen um.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="fieldNumber"></param>
        /// <returns></returns>
        public object GetUserFieldValue(Track track, int fieldNumber)
        {
            try
            {
                switch (fieldNumber)
                {
                    case 0: return GetUserFieldValue(track.UserField1, Field.TrackUser1);
                    case 1: return GetUserFieldValue(track.UserField2, Field.TrackUser2);
                    case 2: return GetUserFieldValue(track.UserField3, Field.TrackUser3);
                    case 3: return GetUserFieldValue(track.UserField4, Field.TrackUser4);
                    case 4: return GetUserFieldValue(track.UserField5, Field.TrackUser5);
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Wandelt den String in den entsprechenden Datentypen um.
        /// </summary>
        /// <param name="valueString"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public object GetUserFieldValue(string valueString, Field field)
        {
            switch (GetUserFieldType(field))
            {
                case UserFieldType.Boolean:
                    if (string.Compare(valueString, StringTable.Yes, true) == 0)
                        return CheckState.Checked;
                    else
                        //                        if (string.Compare(valueString, StringTable.No, true) == 0)
                        return CheckState.Unchecked;
                //                        else
                //                            return CheckState.Indeterminate;
                default:
                    return valueString;
            }
        }

        public void SetUserFieldValue(Track track, int fieldNumber, object value)
        {
            switch (fieldNumber)
            {
                case 0:
                    track.UserField1 = ConvertToString(value, GetUserFieldType(Field.TrackUser1));
                    break;
                case 1:
                    track.UserField2 = ConvertToString(value, GetUserFieldType(Field.TrackUser2));
                    break;
                case 2:
                    track.UserField3 = ConvertToString(value, GetUserFieldType(Field.TrackUser3));
                    break;
                case 3:
                    track.UserField4 = ConvertToString(value, GetUserFieldType(Field.TrackUser4));
                    break;
                case 4:
                    track.UserField5 = ConvertToString(value, GetUserFieldType(Field.TrackUser5));
                    break;
                default:
                    return;
            }
        }

        private string ConvertToString(object value, UserFieldType userFieldType)
        {
            switch (userFieldType)
            {
                case UserFieldType.Boolean:
                    CheckState checkState = (CheckState)value;
                    if (checkState == CheckState.Checked)
                        return StringTable.Yes;
                    else
                        //                        if (checkState == CheckState.Unchecked)
                        return StringTable.No;
                //                        else
                //                            return "";
                default:
                    return value.ToString();
            }
        }

        public string FormatDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return "";
            }

            if (date.Length < 8)
                return date;

            switch (this.Master.DateType)
            {
                case DateType.YYYYMMDD:
                    return date.Mid(6, 2) + "." + date.Mid(4, 2) + "." + date.Mid(0, 4);

                case DateType.YYYYMM:
                    return date.Mid(4, 2) + "." + date.Mid(0, 4);

                case DateType.YYYY:
                    return date.Mid(0, 4);

                default:
                    return date;
            }
        }

        public string ParseDate(string date)
        {
	        int day, month, year;
	
	        if (!DateCheckFormat(date) || string.IsNullOrEmpty(date))
	        {
		        return "";
        	}

            string[] parts = date.Split('.');

	        switch (Master.DateType)
	        {
	            case DateType.YYYYMMDD:
                    day = Misc.Atoi(parts[0]);
                    month = Misc.Atoi(parts[1]);
                    year = Misc.Atoi(parts[2]);
            		return string.Format("{0:D4}{1:D2}{2:D2}", year, month, day);
		
            	case DateType.YYYYMM:
                    month = Misc.Atoi(parts[0]);
                    year = Misc.Atoi(parts[1]);
                    return string.Format("{0:D4}{1:D2}01", year, month);
		
	            case DateType.YYYY:
                    year = Misc.Atoi(parts[0]);
                    return string.Format("{0:D4}0101", year);
		
	            default:
                    return date;
        	}
        }

        bool DateCheckFormat(String date)
        {
            int day=0, month=0, year=0;

            if (string.IsNullOrEmpty(date))
                return true;

            string[] parts = date.Split('.');

            switch (Master.DateType)
            {
                case DateType.YYYYMMDD:
                    if (parts.Length < 3)
                        return false;
                    day = Misc.Atoi(parts[0]);
                    month = Misc.Atoi(parts[1]);
                    year = Misc.Atoi(parts[2]);
                    break;

                case DateType.YYYYMM:
                    if (parts.Length < 2)
                        return false;
                    day = 1;
                    month = Misc.Atoi(parts[0]);
                    year = Misc.Atoi(parts[1]);
                    break;

                case DateType.YYYY:
                    if (parts.Length < 1)
                        return false;
                    day = 1;
                    month = 1;
                    year = Misc.Atoi(parts[0]);
                    break;

                default:
                    return true;
            }

            if (day < 1 || day > 31)
                return false;
            if (month < 1 || month > 12)
                return false;

            if ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30)
                return false;

            if (month == 2 && (day > 29 || day > 28 && ((year % 4) != 0 || (year % 200) == 0)))
                return false;

            if (year < 1000 || year > 9999)
                return false;
   
            return true;
        }

        public void AddCategoriesToComboBox(ComboBox cb)
        {
            if (Settings.Current.AutoSortGenres)
            {
                foreach (Category cat in from x in AllCategories orderby x.Name select x)
                    cb.Items.Add(cat.Name);
            }
            else
            {
                foreach (Category cat in from x in AllCategories orderby x.Order select x)
                    cb.Items.Add(cat.Name);
            }
        }

        public void AddMediaToComboBox(ComboBox cb)
        {
            if (Settings.Current.AutoSortMediums)
            {
                foreach (Medium med in from x in AllMediums orderby x.Name select x)
                    cb.Items.Add(med.Name);
            }
            else
            {
                foreach (Medium med in from x in AllMediums orderby x.Order select x)
                    cb.Items.Add(med.Name);
            }
        }

        public void AddLabelsToComboBox(ComboBox cb)
        {
            foreach (string label in AllLabels)
                cb.Items.Add(label);
        }

        public void AddCodesToComboBox(ComboBox cb, bool bShowEmpty)
        {
	        String rcstr;
    	
        	rcstr = StringTable.None;
	        cb.Items.Add("<" + rcstr + ">");
	
	        for (int i=0;i<MaximumNumberOfCodes;i++)
	        {
		        if (bShowEmpty || !string.IsNullOrEmpty(Codes[i]))
                {
			        String str;
			        str = string.Format("{0}: {1}", 'A' + i, Codes[i]);
			        cb.Items.Add(str);
		        }
	        }
        }

        public string GetArtistText(bool isSampler)
        {
	        String str;
	
	        if (isSampler)
		        str = String.Format("{0} 1", StringTable.Title);
	        else
		        str = StringTable.Artist;
	
	        return str;
        }

        public string GetTitleText(bool isSampler)
        {
	        String str;
	
	        if (isSampler)
		        str = String.Format("{0} 2", StringTable.Title);
	        else
		        str = StringTable.Album;
	
	        return str;
        }

        /// <summary>
        /// Der angegebene Datensatz wird geladen. Alle gefundenen CDs werden im
        /// Array übergeben (nur die Record-Number).
        /// </summary>
        /// <param name="cd"></param>
        /// <returns></returns>
        int[] Find(CD cd)
        {
            int recordNumber;

            List<int> recordNumberList = new List<int>();

            recordNumber = CheckForIdentity(cd);

            if (recordNumber != null)
                recordNumberList.Add(recordNumber);

            if (recordNumber == null)     // Identity nicht gefunden
            {
                recordNumber = CheckForXMCDIdentity(cd);
                if (recordNumber != null)
                    recordNumberList.Add(recordNumber);
            }
           
            // XMCD-Identity nicht gefunden 
            if (recordNumber == null)     // Identity nicht gefunden
            {
                recordNumber = CheckForOldIdentity(cd, recordNumberList);
            }

           return recordNumberList.ToArray();
        }


        /// Prüft anhand der Identity der CD, ob diese schon in der Datanbank vorhanden ist!
        int CheckForIdentity(CD cd)
        {
	        return CheckForIdentity(cd.Identity);
        }

        /// Prüft anhand der Identity der CD, ob diese schon in der Datanbank vorhanden ist!
        int CheckForIdentity(String identity)
        {
	        if (string.IsNullOrEmpty(identity))
		        return 0;

            string sql = string.Format("SELECT CDID FROM CD WHERE Identity='{0}'", identity);
            object cdid = ExecuteScalar(sql);
            if (cdid != null)
                return (int)cdid;
            else
                return 0;
        }

        /// <summary>
        /// Prüft anhand der XMCD-Identity der CD, ob diese schon in der Datanbank vorhanden ist!
        /// </summary>
        /// <returns></returns>
        int CheckForXMCDIdentity(CD cd)
        {
            string sql = string.Format("SELECT CDID FROM CD WHERE Identity='XMCD{0:8x}'", GetCDDBDiscID(cd));
            object cdid = ExecuteScalar(sql);
            if (cdid != null)
                return (int)cdid;
            else
                return 0;
        }

        private string GetCDDBDiscID(CD cd)
        {
            return ""; //!!!!!!!!!!!!!!!!!!!!!!!
        }

        // Prüft anhand des alten Format (Gesamtlänge + Länge der ersten drei Lieder),
        // ob die CD schon in der Datenbank ist.
        // Da möglicherweise mehrere CDs gefunden werden können, wird eine Liste
        // der gefundenen CDs zurückgeliefert.
        int CheckForOldIdentity(CD cd, List<int> recordNumberArray)
        {
            return 0;
	       /*!!!!!!!!!!!!!!!! CString s;
	        DWORD dwRecordNumber = 0;
	        CDaoRecordset RecordSet(&m_pdb->m_db);
	        COleVariant var;
        	
	        if (!m_dwTotalLength)
		        return 0;
        	
	        s.Format(L"SELECT CD.IDCD FROM CD WHERE CD.dwGesamtLaenge = %ld", m_dwTotalLength);
        	
	        RecordSet.Open(dbOpenDynaset, s, dbSeeChanges);
        	
	        DWORD dwLength1=0, dwLength2=0, dwLength3=0;
	        if (m_wNumberOfTracks > 0)
		        dwLength1 = GetTrack(0)->m_dwLength;
	        if (m_wNumberOfTracks > 1)
		        dwLength2 = GetTrack(1)->m_dwLength;
	        if (m_wNumberOfTracks > 2)
		        dwLength3 = GetTrack(2)->m_dwLength;
	        if (!RecordSet.IsEOF())
	        {      // Da nur nach Gesamtlaenge gesucht wurde, können mehrere gefunden worden sein.
		        s.Format((CString)"SELECT CD.IDCD "+
			        "FROM CD, Lied AS L1, Lied AS L2, Lied AS L3 "+
			        "WHERE CD.dwGesamtlaenge = %ld " +
			        "and (CD.IDCD=L1.IDCD "+
			        "and CD.IDCD=L2.IDCD "+
			        "and CD.IDCD=L3.IDCD "+
			        "and (L1.wLiednummer=1 and L1.dwLaenge = %ld) "+
			        "and (L2.wLiednummer=2 and L2.dwLaenge = %ld) "+
			        "and (L3.wLiednummer=3 and L3.dwLaenge = %ld))",
			        m_dwTotalLength, dwLength1,
			        dwLength2, dwLength3);
		        RecordSet.Close();
		        RecordSet.Open(dbOpenDynaset, s, dbSeeChanges);
        		
		        while (!RecordSet.IsEOF())
		        {
			        var = RecordSet.GetFieldValue(L"IDCD");
			        if (pRecordNumberArray)
				        pRecordNumberArray->Add(var.lVal);
			        else
			        {
				        dwRecordNumber = var.lVal;
				        break;
			        }
        			
			        RecordSet.MoveNext();
		        }
	        }
        	
	        RecordSet.Close();

	        if (pRecordNumberArray && pRecordNumberArray->GetSize() > 0)
		        return pRecordNumberArray->GetAt(0);
	        else
		        return dwRecordNumber;*/
        }

        public DataTable ExecuteFreeSql(string sql)
        {
            SqlCeCommand command = new SqlCeCommand(sql, Connection);

            SqlCeDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            while (reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);
                dt.Rows.Add(values);
            }
            reader.Close();

            return dt;
        }

        /// <summary>
        /// Die CD mit der angegebenen ID löschen. Es werden auch alle Tracks, und
        /// alle abhängigen Daten der CD gelöscht (Mitwirkende, Programme, etc.)
        /// </summary>
        /// <param name="cdid"></param>
        public void DeleteCd(int cdid)
        {
            string sql;

            SqlCeTransaction trans = connection.BeginTransaction();

            try
            {
                sql = string.Format("DELETE FROM CD WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                sql = string.Format("DELETE FROM Track WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                sql = string.Format("DELETE FROM Program WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                sql = string.Format("DELETE FROM Queue WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                sql = string.Format("DELETE FROM Participant WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                sql = string.Format("DELETE FROM [Index] WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                sql = string.Format("DELETE FROM LoanedCD WHERE CDID={0}", cdid);
                ExecuteNonQuery(sql);
                
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
        /// Alle CDs des angegebenen Interpreten löschen
        /// </summary>
        /// <param name="artistID"></param>
        public void DeleteAllCDsOfArtist(int artistID)
        {
            CDDataSetTableAdapters.CDTableAdapter cdAdap = new Big3.Hitbase.DataBaseEngine.CDDataSetTableAdapters.CDTableAdapter(this);
            CDDataSet.CDDataTable cdDataTable = cdAdap.GetDataByArtistID(artistID);

            foreach (CDDataSet.CDRow cd in cdDataTable.Rows)
            {
                DeleteCd(cd.CDID);
            }
        }

        /// <summary>
        /// Default-Einstellungen (bei leerer Datenbank) eintragen
        /// </summary>
        private void SetDataBaseDefaults()
        {
            // Default Dialog eintragen
			Big3.Hitbase.DataBaseEngine.DialogDataSetTableAdapters.DialogTableAdapter ta = new Big3.Hitbase.DataBaseEngine.DialogDataSetTableAdapters.DialogTableAdapter(this);
			DialogDataSet dialogDataset = new DialogDataSet();

			ta.FillByCategoryID(dialogDataset.Dialog, 0);

			if (dialogDataset.Dialog.Rows.Count < 1)		// Kein Dialog vorhanden...
			{
                /* Gibts nicht mehr in Hitbase 2012
				// Standard dialog
				String standardDialogFilename = String.Format("{0}\\Templates\\standard dialog.xml", Big3.Hitbase.Miscellaneous.Misc.GetApplicationPath());
				String xmlDialog = System.IO.File.ReadAllText(standardDialogFilename);

				DialogDataSet.DialogRow dialogRow = dialogDataset.Dialog.AddDialogRow(0, xmlDialog);

				ta.Update(dialogRow);*/
			}
        }

        public void Export(Form formParent, string filename)
        {
            DataBase dbExport = new DataBase();
            dbExport.Open(filename);
            ImportExport(formParent, StringTable.ExportInProgress, this, dbExport);
            dbExport.Close();
        }

        private void ImportExport(Form formParent, string formTitle, DataBase dbSource, DataBase dbTarget)
        {
            formParent.Enabled = false;
            FormProgress formProgress = new FormProgress();
            try
            {
                formProgress.Text = formTitle;
                formProgress.Show(formParent);

                CDTableAdapter cta = new CDTableAdapter(dbSource);
                CDQueryDataSet.CDDataTable cdt = cta.GetData();

                formProgress.ProgressBar.Maximum = cdt.Rows.Count;
                formProgress.ProgressBar.Value = 0;

                MultiAnswer lastAnswer = MultiAnswer.None;

                int newCDs = 0;
                int updatedCDs = 0;
                int count = 0;

                foreach (CDQueryDataSet.CDRow cdRow in cdt)
                {
                    formProgress.LabelProgress.Text = string.Format(StringTable.NumberFromTo, count, cdt.Rows.Count);
                    CD cd = dbSource.GetCDById(cdRow.CDID);

                    CD cdFound = dbTarget.GetCDByIdentity(cd.Identity);

                    if (cdFound == null)
                    {
                        cd.ID = 0;          // Damit die CD hinzugefügt wird (neu)
                        cd.Save(dbTarget);
                        newCDs++;
                    }
                    else
                    {
                        // CD ist im Ziel-Katalog schon vorhanden. Was tun?
                        if (lastAnswer != MultiAnswer.NoAll && lastAnswer != MultiAnswer.YesAll)
                        {
                            FormCDAlreadyExists formCDAlreadyExists = new FormCDAlreadyExists();
                            formCDAlreadyExists.LabelArtist.Text = cd.Artist;
                            formCDAlreadyExists.LabelTitle.Text = cd.Title;

                            if (formCDAlreadyExists.ShowDialog(formProgress) == DialogResult.Cancel)
                                break;

                            lastAnswer = formCDAlreadyExists.Answer;
                        }

                        if (lastAnswer == MultiAnswer.YesAll || lastAnswer == MultiAnswer.Yes)
                        {
                            // Erst die vorhandene CD löschen..
                            dbTarget.DeleteCd(cdFound.ID);

                            // ... und dann die neue schreiben
                            cd.ID = 0;          // Damit die CD hinzugefügt wird (neu)
                            cd.Save(dbTarget);

                            updatedCDs++;
                        }
                    }

                    if (formProgress.Canceled)
                        break;

                    formProgress.ProgressBar.Value++;
                    count++;

                    Application.DoEvents();
                }
            }
            finally
            {
                formParent.Enabled = true;
                formProgress.Close();
                formProgress = null;
            }
        }

        public void Import(Form formParent, string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".hdb")
            {
                // Da anscheinend viele versuche, die alte Datenbank zu importieren, geben wir
                // besser eine sprechende Fehlermeldung aus.

                MessageBox.Show(StringTable.WrongDatabaseFormat, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataBase dbImport = new DataBase();
            dbImport.Open(filename);
            ImportExport(formParent, StringTable.ImportInProgress, dbImport, this);
            dbImport.Close();
        }

        public List<Tuple<string, int>> GetAllMusicFiles()
        {
            string sql = string.Format("SELECT soundfile, trackid from track inner join cd on track.cdid = cd.cdid where cd.type=3");
            
            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<Tuple<string, int>> ids = new List<Tuple<string, int>>();

            while (reader.Read())
            {
                string soundfile = "";

                if (!reader.IsDBNull(0))
                    soundfile = (string)reader[0];

                ids.Add(new Tuple<string, int>(soundfile, (int)reader[1]));
            }

            return ids;
        }

        /// <summary>
        /// Liefert alle Unterverzeichnisse der Soundfiles zurück, die mit angegebenen Pfad beginnen.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <returns></returns>
        public List<string> GetAllMusicFilesSubDirectories(string parentPath)
        {
            string sql = string.Format("SELECT soundfile from track order by soundfile");

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<string> soundfiles = new List<string>();
            HashSet<string> checkSoundFiles = new HashSet<string>();

            while (reader.Read())
            {
                string soundfile = "";

                if (!reader.IsDBNull(0))
                    soundfile = (string)reader[0];

                if (string.IsNullOrEmpty(parentPath) || soundfile.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase))
                {
                    string subDir = soundfile.Mid(parentPath.Length);
                    int slashPos = subDir.IndexOf('\\');

                    // UNC-Pfad testen
                    if (string.IsNullOrEmpty(parentPath) && subDir.StartsWith("\\\\"))
                    {
                        slashPos = subDir.IndexOf('\\', 2);
                        slashPos = subDir.IndexOf('\\', slashPos+1);
                    }
                    if (slashPos > 0)
                    {
                        subDir = subDir.Left(slashPos);

                        string subDirLowerCase = subDir.ToLower();
                        if (!checkSoundFiles.Contains(subDirLowerCase))
                        {
                            soundfiles.Add(subDir);
                            checkSoundFiles.Add(subDirLowerCase);
                        }
                    }
                }
            }

            return soundfiles;
        }

        /// <summary>
        /// Alle Alben löschen, für die es keinen Track mehr gibt. 
        /// </summary>
        /// <returns>Liefert die Anzahl der Alben zurück, die gelöscht wurden.</returns>
        public int DeleteEmptyAlbums()
        {
            string sql = string.Format("DELETE FROM CD WHERE CD.Type=3 AND NOT EXISTS (SELECT CDID from Track where track.cdid = cd.cdid)");

            int deletedCount = ExecuteNonQuery(sql);

            return deletedCount;
        }

        public void DeleteTrack(int idTrack)
        {
            string sql = string.Format("DELETE FROM Track where TrackId = {0}", idTrack);

            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Liefert die cdid der CD mit dem angegebenen Interpreten und Titel zurück (nur Soundfile-CDs)
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public int GetCDIDByArtistAndTitle(string artist, string title)
        {
            //string sqlArtist = SqlPrepare(artist);
            //string sqlTitle = SqlPrepare(title);

            string sql = string.Format("SELECT CD.CDID FROM CD INNER JOIN PersonGroup ON CD.ArtistID = PersonGroup.PersonGroupID " +
                                       "WHERE (CD.Title = @param1) AND (PersonGroup.Name = @param2) AND (CD.Type = 3)");

            object cdid = ExecuteScalarWithParameter(sql, title, artist);

            if (cdid == null)
                return 0;

            return (int)cdid;
        }

        public List<string> GetAlbumAllPersonGroupFirstChar(bool composer)
        {
            string sql = string.Format("SELECT DISTINCT SUBSTRING(SaveAs, 1, 1) from PersonGroup");

            if (composer)
                sql += " WHERE EXISTS (SELECT ComposerID FROM CD WHERE CD.ComposerID = PersonGroup.PersonGroupID)";
            else
                sql += " WHERE EXISTS (SELECT ArtistID FROM CD WHERE CD.ArtistID = PersonGroup.PersonGroupID)";

            sql += " ORDER BY SUBSTRING(SaveAs, 1, 1)";

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<string> chars = new List<string>();

            bool otherFound = false;
            while (reader.Read())
            {
                string firstChar = reader[0] is DBNull ? " " : ((string)reader[0]).ToUpper();
                if (string.IsNullOrEmpty(firstChar) || firstChar[0] < 'A' || firstChar[0] > 'Z')
                {
                    if (!otherFound)
                        chars.Insert(0, StringTable.Other);

                    otherFound = true;
                }
                else
                {
                    chars.Add(firstChar);
                }
            }

            return chars;
        }


        public List<string> GetAllPersonGroupFirstChar(bool composer)
        {
            string sql = string.Format("SELECT DISTINCT SUBSTRING(SaveAs, 1, 1) from PersonGroup");

            if (composer)
                sql += " WHERE EXISTS (SELECT ComposerID FROM Track WHERE Track.ComposerID = PersonGroup.PersonGroupID)";
            else
                sql += " WHERE EXISTS (SELECT ArtistID FROM Track WHERE Track.ArtistID = PersonGroup.PersonGroupID)";

            sql += " ORDER BY SUBSTRING(SaveAs, 1, 1)";

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<string> chars = new List<string>();

            bool otherFound = false;
            while (reader.Read())
            {
                string firstChar = reader[0] is DBNull ? " " : ((string)reader[0]).ToUpper();
                if (string.IsNullOrEmpty(firstChar) || firstChar[0] < 'A' || firstChar[0] > 'Z')
                {
                    if (!otherFound)
                        chars.Add(StringTable.Other);

                    otherFound = true;
                }
                else
                {
                    chars.Add(firstChar);
                }
            }

            return chars;
        }

        public List<PersonGroupModel> GetAllPersonGroupsByFirstChar(char firstChar, bool composer)
        {
            string sql = string.Format("SELECT [Name], [Type], [Sex], [SaveAs] from PersonGroup");

            if (firstChar == '?')
            {
                sql += " WHERE SaveAs < 'A' OR SaveAs > 'ZZZZZ'";
            }
            else
            {
                sql += " WHERE SaveAs >= '" + firstChar + "' AND SaveAs < '" + firstChar + "ZZZZZ'";
            }

            if (composer)
                sql += " AND EXISTS (SELECT ComposerID FROM Track WHERE Track.ComposerID = PersonGroup.PersonGroupId)";
            else
                sql += " AND EXISTS (SELECT ArtistID FROM Track WHERE Track.ArtistID = PersonGroup.PersonGroupId)";
            
            sql += " ORDER BY [SaveAs]";

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<PersonGroupModel> names = new List<PersonGroupModel>();

            while (reader.Read())
            {
                PersonGroupModel personGroup = new PersonGroupModel();
                personGroup.Name = (string)reader[0];
                personGroup.Type = reader.IsDBNull(1) ? PersonGroupType.Unknown : (PersonGroupType)(int)reader[1];
                personGroup.Sex = reader.IsDBNull(2) ? SexType.Unknown : (SexType)(int)reader[2];

                names.Add(personGroup);
            }

            return names;
        }

        public List<PersonGroupModel> GetAlbumAllPersonGroupsByFirstChar(char firstChar, bool composer)
        {
            string sql = string.Format("SELECT [Name], [Type], [Sex], [SaveAs] from PersonGroup");

            if (firstChar == '?')
            {
                sql += " WHERE (SaveAs < 'A' OR SaveAs > 'ZZZZZ')";
            }
            else
            {
                sql += " WHERE SaveAs >= '" + firstChar + "' AND SaveAs < '" + firstChar + "ZZZZZ'";
            }

            if (composer)
                sql += " AND EXISTS (SELECT ComposerID FROM CD WHERE CD.ComposerID = PersonGroup.PersonGroupId)";
            else
                sql += " AND EXISTS (SELECT ArtistID FROM CD WHERE CD.ArtistID = PersonGroup.PersonGroupId)";

            sql += " ORDER BY [SaveAs]";

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<PersonGroupModel> names = new List<PersonGroupModel>();

            while (reader.Read())
            {
                PersonGroupModel personGroup = new PersonGroupModel();
                personGroup.Name = (string)reader[0];
                personGroup.Type = reader.IsDBNull(1) ? PersonGroupType.Unknown : (PersonGroupType)(int)reader[1];
                personGroup.Sex = reader.IsDBNull(2) ? SexType.Unknown : (SexType)(int)reader[2];

                names.Add(personGroup);
            }

            return names;
        }

        public List<string> GetAllParticipantFirstChar()
        {
            string sql = string.Format("SELECT DISTINCT SUBSTRING(SaveAs, 1, 1) from PersonGroup");

            sql += " WHERE EXISTS (SELECT PersonGroupID FROM Participant WHERE Participant.PersonGroupID = PersonGroup.PersonGroupID)";

            sql += " ORDER BY SUBSTRING(SaveAs, 1, 1)";

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<string> chars = new List<string>();

            bool otherFound = false;
            while (reader.Read())
            {
                string firstChar = reader[0] is DBNull ? " " : ((string)reader[0]).ToUpper();
                if (firstChar[0] < 'A' || firstChar[0] > 'Z')
                {
                    if (!otherFound)
                        chars.Add(StringTable.Other);

                    otherFound = true;
                }
                else
                {
                    chars.Add(firstChar);
                }
            }

            return chars;
        }

        public List<PersonGroupModel> GetParticipantsByFirstChar(char firstChar)
        {
            string sql = "SELECT [Name], [Type], [Sex] from PersonGroup";

            if (firstChar == '?')
            {
                sql += " WHERE SaveAs < 'A' OR SaveAs > 'ZZZZZ'";
            }
            else
            {
                sql += " WHERE SaveAs >= '" + firstChar + "' AND SaveAs < '" + firstChar + "ZZZZZ'";
            }


            sql += " AND EXISTS (SELECT PersonGroupID FROM Participant WHERE Participant.PersonGroupID = PersonGroup.PersonGroupID)";


            sql += " ORDER BY SUBSTRING(SaveAs, 1, 1)";

            SqlCeCommand command = new SqlCeCommand(sql, connection);
            SqlCeDataReader reader = command.ExecuteReader();
            List<PersonGroupModel> names = new List<PersonGroupModel>();

            while (reader.Read())
            {
                PersonGroupModel personGroup = new PersonGroupModel();
                personGroup.Name = (string)reader[0];
                personGroup.Type = reader.IsDBNull(1) ? PersonGroupType.Unknown : (PersonGroupType)(int)reader[1];
                personGroup.Sex = reader.IsDBNull(2) ? SexType.Unknown : (SexType)(int)reader[2];
                names.Add(personGroup);
            }

            return names;
        }

        public string GetFrontCoverByCdId(int cdId)
        {
            object frontCover = ExecuteScalar("SELECT FrontCover FROM CD WHERE CDID=" + cdId.ToString());

            if (frontCover == null || frontCover is DBNull)
                return "";

            return frontCover.ToString();
        }

        /// <summary>
        /// Prüft, ob die angegebenen CD mit der ID (cdid) kein Cover zugewiesen hat.
        /// Schreibt dann die Image-Daten auf die Platte und weißt den Pfad der CD zu.
        /// </summary>
        /// <param name="cdid"></param>
        /// <param name="imageBytes"></param>
        public void UpdateFrontCover(int cdid, string artist, string title, byte[] imageBytes)
        {
            string sql = string.Format("Select frontcover from CD where cdid={0}", cdid);
            object frontcover = ExecuteScalar(sql);
            if (frontcover != null && !string.IsNullOrEmpty(frontcover.ToString()))
            {
                // OK, die CD hat wohl schon ein Image zugewiesen. 
                return;
            }

            string filename = String.Format("{0} - {1}.jpg", Misc.FilterFilenameChars(artist), Misc.FilterFilenameChars(title));
            filename = Misc.GetCDCoverFilename(filename);

            File.WriteAllBytes(filename, imageBytes);

            sql = string.Format("Update CD Set frontcover='{0}' where cdid={1}", DataBase.SqlPrepare(filename), cdid);
            ExecuteNonQuery(sql);
        }

        public void UpdateTrackNumber(string filename, int trackNumber)
        {
            string sql;
            sql = string.Format("Update track Set TrackNumber=@param1 where SoundFile=@param2");
            ExecuteScalarWithParameter(sql, trackNumber.ToString(), filename);
        }

        /// <summary>
        /// Liefert die CDID der angegebenen TrackID zurück.
        /// </summary>
        /// <param name="trackid"></param>
        /// <returns></returns>
        public int GetCDIDByTrackID(int trackid)
        {
            string sql = string.Format("SELECT CDID FROM Track WHERE TrackID={0}", trackid);

            return (int)ExecuteScalar(sql);
        }

        /// <summary>
        /// Fügt eine neue persönliche Suche in die Datenbank ein.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="condition"></param>
        public void AddSearch(string name, Condition condition, int type)
        {
            Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter sta = new Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter(this);
            SearchDataSet.SearchDataTable dt = new SearchDataSet.SearchDataTable();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Condition));
            StringWriter sw = new StringWriter();
            xmlSerializer.Serialize(sw, condition);

            dt.AddSearchRow(name, sw.ToString(), type);
            sta.Update(dt);
            sw.Close();
        }

        /// <summary>
        /// Löscht die gespeicherte Suche mit der angegebenen ID.
        /// </summary>
        /// <param name="searchId"></param>
        public void DeleteSearch(int searchId)
        {
            string sql = string.Format("DELETE FROM Search Where SearchID=" + searchId.ToString());

            ExecuteNonQuery(sql);
        }

        public void UpdateSearch(int searchId, string name, Condition condition, int type)
        {
            Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter sta = new Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter(this);
            SearchDataSet.SearchDataTable dt = sta.GetDataById(searchId);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Condition));
            StringWriter sw = new StringWriter();
            xmlSerializer.Serialize(sw, condition);

            SearchDataSet.SearchRow row = dt.Rows[0] as SearchDataSet.SearchRow;
            row.Condition = sw.ToString();
            row.Name = name;
            row.Type = type;
            
            sta.Update(dt);

            sw.Close();
        }

        /// <summary>
        /// Liefert die Search-ID der Suche mit dem angegebenen Namen zurück. 0, wenn sie nicht gefunden wurde.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetSearchIdByName(string name)
        {
            Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter sta = new Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters.SearchTableAdapter(this);
            SearchDataSet.SearchDataTable dt = sta.GetDataByName(name);

            if (dt.Rows.Count > 0)
                return ((SearchDataSet.SearchRow)dt.Rows[0]).SearchID;
            else
                return 0;
        }

        public List<int> GetAvailableTrackYears()
        {
            string sql = String.Format("SELECT YearRecorded FROM Track Group by YearRecorded");

            DataTable dt = ExecuteFreeSql(sql);

            List<int> years = new List<int>();

            foreach (DataRow row in dt.Rows)
            {
                years.Add((int)row[0]);
            }

            sql = String.Format("SELECT YearRecorded FROM CD Group by YearRecorded");

            dt = ExecuteFreeSql(sql);

            foreach (DataRow row in dt.Rows)
            {
                int year = (int)row[0];
                if (!years.Contains(year))
                    years.Add((int)row[0]);
            }

            years.Sort();

            return years;
        }

        /// <summary>
        /// Liefert alle CDs der angegebenen Person/Gruppe zurück (auf Track- oder AlbumBasis).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<AlbumCDSetModel> GetAllAlbumsAndCDSetsByPersonGroup(int personGroupId, bool composer, bool withTracks)
        {
            List<AlbumCDSetModel> albumsAndCDSets = new List<AlbumCDSetModel>();

            string sql;

            string artistComposerField = "";
            if (composer)
                artistComposerField = "ComposerId";
            else
                artistComposerField = "ArtistId";

            sql = "SELECT CD.Title, CD.NumberOfTracks, CD.Length, CD.SetID, [Set].Name, CD.CDID, CD.[Type], Medium.Name as MediumName, CD.SetNumber FROM CD ";
 
            if (withTracks)
                sql += "INNER JOIN [Track] ON Track.CDID = CD.CDID";
            
            sql += string.Format(" LEFT JOIN [Set] ON CD.SetID = [Set].SetID "+
                "LEFT JOIN [Medium] ON CD.MediumID = Medium.MediumID Where {0}." +
                artistComposerField + " = {1} GROUP BY CD.Title, CD.NumberOfTracks, CD.Length, CD.SetID, [Set].Name, CD.CDID, CD.[Type], Medium.Name, CD.SetNumber " +
                "ORDER BY [Set].Name, CD.Title", withTracks ? "Track" : "CD", personGroupId);

            DataTable dt = ExecuteFreeSql(sql);

            AlbumCDSetModel lastCDSet = null;
            int? lastCDSetId = null;

            foreach (DataRow row in dt.Rows)
            {
                string title = (string)row[0];
                int numberOfTracks = (int)row[1];
                int length = (int)row[2];
                int? setId = (int?)row[3];
                string setName = row.IsNull(4) ? "" : (string)row[4];
                int cdid = (int)row[5];
                int type = (int)row[6];
                string mediumName = row.IsNull(7) ? "" : (string)row[7];
                int setNumber = row.IsNull(8) ? 0 : (int)row[8];

                AlbumCDSetModel album = new AlbumCDSetModel();

                string formattedTitle = "";

                if (numberOfTracks == 1)
                {
                    if (length == 0)
                        formattedTitle = string.Format("{0} [{1} {2}]", title, numberOfTracks, StringTable.Track);
                    else
                        formattedTitle = string.Format("{0} [{1} {2}, {3}]", title, numberOfTracks, StringTable.Track, Misc.GetShortTimeString(length));
                }
                else
                {
                    if (numberOfTracks == 0)
                    {
                        if (length == 0)
                            formattedTitle = string.Format("{0}", title);
                        else
                            formattedTitle = string.Format("{0} [{1}]", title, Misc.GetShortTimeString(length));
                    }
                    else
                    {
                        if (length == 0)
                            formattedTitle = string.Format("{0} [{1} {2}]", title, numberOfTracks, StringTable.Tracks);
                        else
                            formattedTitle = string.Format("{0} [{1} {2}, {3}]", title, numberOfTracks, StringTable.Tracks, Misc.GetShortTimeString(length));
                    }
                }

                if (setId == null || setId == 0)
                {
                    album.Title = formattedTitle;
                    album.CDID = cdid;
                    album.IsCDSet = false;
                    album.AlbumType = (AlbumType)type;
                    album.Medium = mediumName;
                    albumsAndCDSets.Add(album);
                }
                else
                {
                    if (lastCDSetId == setId)
                        album = lastCDSet;
                    else
                        albumsAndCDSets.Add(album);

                    album.Title = setName;
                    album.CDSetAlbums.Add(new AlbumCDSetModel() { 
                        Title = formattedTitle, 
                        IsCDSet = false, 
                        CDID = cdid, 
                        AlbumType = (AlbumType)type, 
                        Medium = mediumName,
                        CDSetNumber = setNumber 
                    });

                    album.IsCDSet = true;
                    lastCDSetId = setId;
                    lastCDSet = album;
                }

            }

            albumsAndCDSets.Sort((a, b) => a.Title.CompareTo(b.Title)); 

            return albumsAndCDSets;
        }

        /// <summary>
        /// Liefert alle CDs der angegebenen Person/Gruppe zurück.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<AlbumCDSetModel> GetAllAlbumsAndCDSetsByParticipant(int personGroupId)
        {
            List<AlbumCDSetModel> albumsAndCDSets = new List<AlbumCDSetModel>();

            string sql;

            sql = String.Format("SELECT CD.Title, CD.NumberOfTracks, CD.Length, CD.SetID, [Set].Name, CD.CDID, CD.[Type], Medium.Name as MediumName, CD.SetNumber FROM CD " +
                "INNER JOIN [Participant] ON Participant.CDID = CD.CDID LEFT JOIN [Set] ON CD.SetID = [Set].SetID " +
                "LEFT JOIN [Medium] ON CD.MediumID = Medium.MediumID Where Participant.PersonGroupID = {0} " +
                "GROUP BY CD.Title, CD.NumberOfTracks, CD.Length, CD.SetID, [Set].Name, CD.CDID, CD.[Type], Medium.Name, CD.SetNumber " +
                "ORDER BY [Set].Name, CD.Title", personGroupId);

            DataTable dt = ExecuteFreeSql(sql);

            AlbumCDSetModel lastCDSet = null;
            int? lastCDSetId = null;

            foreach (DataRow row in dt.Rows)
            {
                string title = (string)row[0];
                int numberOfTracks = (int)row[1];
                int length = (int)row[2];
                int? setId = (int?)row[3];
                string setName = row.IsNull(4) ? "" : (string)row[4];
                int cdid = (int)row[5];
                int type = (int)row[6];
                string mediumName = row.IsNull(7) ? "" : (string)row[7];
                int setNumber = row.IsNull(8) ? 0 : (int)row[8];

                AlbumCDSetModel album = new AlbumCDSetModel();

                string formattedTitle = "";

                if (numberOfTracks == 1)
                {
                    if (length == 0)
                        formattedTitle = string.Format("{0} [{1} {2}]", title, numberOfTracks, StringTable.Track);
                    else
                        formattedTitle = string.Format("{0} [{1} {2}, {3}]", title, numberOfTracks, StringTable.Track, Misc.GetShortTimeString(length));
                }
                else
                {
                    if (numberOfTracks == 0)
                    {
                        if (length == 0)
                            formattedTitle = string.Format("{0}", title);
                        else
                            formattedTitle = string.Format("{0} [{1}]", title, Misc.GetShortTimeString(length));
                    }
                    else
                    {
                        if (length == 0)
                            formattedTitle = string.Format("{0} [{1} {2}]", title, numberOfTracks, StringTable.Track);
                        else
                            formattedTitle = string.Format("{0} [{1} {2}, {3}]", title, numberOfTracks, StringTable.Tracks, Misc.GetShortTimeString(length));
                    }
                }

                if (setId == null || setId == 0)
                {
                    album.Title = formattedTitle;
                    album.CDID = cdid;
                    album.IsCDSet = false;
                    album.AlbumType = (AlbumType)type;
                    album.Medium = mediumName;
                    albumsAndCDSets.Add(album);
                }
                else
                {
                    if (lastCDSetId == setId)
                        album = lastCDSet;
                    else
                        albumsAndCDSets.Add(album);

                    album.Title = setName;
                    album.CDSetAlbums.Add(new AlbumCDSetModel()
                    {
                        Title = formattedTitle,
                        IsCDSet = false,
                        CDID = cdid,
                        AlbumType = (AlbumType)type,
                        Medium = mediumName,
                        CDSetNumber = setNumber
                    });

                    album.IsCDSet = true;
                    lastCDSetId = setId;
                    lastCDSet = album;
                }

            }

            albumsAndCDSets.Sort((a, b) => a.Title.CompareTo(b.Title));

            return albumsAndCDSets;
        }

        /// <summary>
        /// Komprimiert (Shrinks) die angegeben Datenbank
        /// </summary>
        /// <param name="filename"></param>
        public void Compress()
        {
            string connString = string.Format("Data Source=\"{0}\"", this.DataBasePath);

            var engine = new System.Data.SqlServerCe.SqlCeEngine(connString);
            
            engine.Compact(connString);
        }

        /// <summary>
        /// Die Datenbank so vorbereiten, dass man sie auf My.Hitbase hochladen kann.
        /// </summary>
        public void PrepareForUpload(PrepareProgressDelegate onPrepareProgress, bool dontCopyCoversToDatabase)
        {
            // Images-Tabelle in der Datenbank anlegen
            GenerateImageTable();

            if (dontCopyCoversToDatabase)
                return;

            // Images von der Festplatte in die Datenbank schreiben
            DataTable dt = ExecuteFreeSql("SELECT CDID, FrontCover FROM CD");

            int count = 0;
            Dictionary<string, int> imageToImageIdDict = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                double percentage = 100.0 / (double)dt.Rows.Count * (double)count;

                if (onPrepareProgress != null)
                {
                    if (!onPrepareProgress(percentage))
                        break;
                }

                string frontCover = "";

                if (!row.IsNull(1))
                    frontCover = (string)row[1];

                int cdId = (int)row[0];

                if (!string.IsNullOrEmpty(frontCover) && File.Exists(Misc.FindCover(frontCover)))
                {
                    string frontCoverFilename = Misc.FindCover(frontCover);
                    //byte[] imageBytes = File.ReadAllBytes(frontCover);

                    byte[] imageBytes = null;

                    try
                    {
                        imageBytes = Misc.CreateThumbnail(frontCoverFilename, 200, 200, 40);
                    }
                    catch
                    {

                    }

                    int imageId = -1;

                    if (imageBytes != null && !imageToImageIdDict.ContainsKey(frontCoverFilename))
                    {
                        SqlCeCommand cmd = new SqlCeCommand("INSERT INTO [Images] (ImageData) " +
                        " VALUES (@mypic)", this.Connection);

                        cmd.Parameters.Add("@mypic", SqlDbType.Image, imageBytes.Length).Value = imageBytes;

                        cmd.ExecuteNonQuery();

                        imageId = (int)(decimal)ExecuteScalar("SELECT @@IDENTITY");

                        imageToImageIdDict.Add(frontCoverFilename, imageId);
                    }
                    else
                    {
                        if (imageToImageIdDict.ContainsKey(frontCoverFilename))
                        {
                            imageId = imageToImageIdDict[frontCoverFilename];
                        }
                    }

                    if (imageId > 0)
                    {
                        string sql = string.Format("Update CD Set FrontCoverImageId = {0} WHERE CDID = {1}", imageId, cdId);
                        ExecuteNonQuery(sql);
                    }
                }

                count++;
            }
        }

        private void GenerateImageTable()
        {
            DataTable dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Images'");
            if (dt.Rows.Count < 1)
            {
                ExecuteScalar("CREATE TABLE [Images] (" +
                        "[ImageID] int NOT NULL  IDENTITY (1,1) " +
                        ", [ImageData] image NULL)");

                ExecuteScalar("ALTER TABLE [Images] ADD CONSTRAINT [PK_Images] PRIMARY KEY ([ImageID]);");

                ExecuteScalar("CREATE UNIQUE INDEX [UQ__Images__000000000000000A] ON [Images] ([ImageID] ASC);");

            }

            dt = ExecuteFreeSql("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CD' AND COLUMN_NAME = 'FrontCoverImageId'");
            if (dt.Rows.Count < 1)
            {
                string sql = "ALTER TABLE [CD] ADD FrontCoverImageId int";
                ExecuteScalar(sql);
            }

        }

        public string[] GetAllFrontCovers(bool checkExist)
        {
            string sql = "SELECT FrontCover From CD Where FrontCover is not null AND FrontCover <> ''";
            DataTable dt = this.ExecuteFreeSql(sql);

            List<string> frontCovers = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (!checkExist || File.Exists(row[0] as string))
                {
                    frontCovers.Add(row[0] as string);
                }
            }

            return frontCovers.ToArray();
        }

    }

    public class PersonGroupModel
    {
        public string Name { get; set; }
        public SexType Sex { get; set; }
        public PersonGroupType Type { get; set; }
    }

    public class AlbumCDSetModel 
    {
        public AlbumCDSetModel()
        {
            CDSetAlbums = new List<AlbumCDSetModel>();
        }

        public string Title { get; set; }

        public List<AlbumCDSetModel> CDSetAlbums { get; set; }
        
        public bool IsCDSet { get; set; }

        public int CDID { get; set; }

        public string Medium { get; set; }

        public AlbumType AlbumType { get; set; }

        public int CDSetNumber { get; set; }
    }

    public class DataBaseDateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataBase db = parameter as DataBase;
            return db.FormatDate(value is DBNull ? "" : (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataBase db = parameter as DataBase;
            return db.ParseDate((string)value);
        }
    }

    public class UserFieldDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value is DBNull)
                return "";

            string date = (string)value;

            if (date.Length < 8)
                return "";

            return date.Mid(6, 2) + "." + date.Mid(4, 2) + "." + date.Mid(0, 4);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MaxLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Field field;
            Enum.TryParse<Field>((string)parameter, out field);

            return DataBase.GetMaxStringLength(field);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
