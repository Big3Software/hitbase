using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlServerCe;
using Big3.Hitbase.DataBaseEngine.ConvertDataSetTableAdapters;
using System.Data;
using Big3.Hitbase.DataBaseEngine.MasterDataSetTableAdapters;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.DataBaseEngine
{
    public class HdbToSdfConverter
    {
        public class TableConvert
        {
            public string HdbTableName;
            public string SdfbTableName;
            public List<FieldConvert> Fields;
        }

        public class FieldConvert
        {
            public string HdbFieldName;
            public string SdfFieldName;
            // Ein Datumsfeld in einen String umwandeln "21.03.1988" => "19880321"
            public bool ConvertDateToString;
            
            // Standardwert, wenn ein Feld nicht vorhanden ist.
            public object DefaultValue;

            public FieldConvert(string hdbFieldName, string sdfFieldName) : this(hdbFieldName, sdfFieldName, false)
            {
            }

            public FieldConvert(string hdbFieldName, string sdfFieldName, bool convertDateToString)
                : this(hdbFieldName, sdfFieldName, convertDateToString, null)
            {
            }

            public FieldConvert(string hdbFieldName, string sdfFieldName, bool convertDateToString, object defaultValue)
            {
                HdbFieldName = hdbFieldName;
                SdfFieldName = sdfFieldName;
                ConvertDateToString = convertDateToString;
                DefaultValue = defaultValue;
            }
        }

        private DataBase sdfDatabase;
        private OleDbConnection hdbConnection;
        private FormConvertProgress formConvertProgress = new FormConvertProgress();

        public void Convert(string inputFilename, string outputFilename, bool overwrite)
        {
            formConvertProgress.Show();
            try
            {
                string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;OLE DB Services=-4;Data Source=\"{0}\"", inputFilename);

                hdbConnection = new OleDbConnection(connString);
                hdbConnection.Open();

                string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string templateFilename = Path.Combine(appPath, "template.sdf");
                File.Copy(templateFilename, outputFilename, overwrite);
                File.SetAttributes(outputFilename, FileAttributes.Normal);

                sdfDatabase = new DataBase();
                sdfDatabase.Open(outputFilename, false, false);

                Convert();

                hdbConnection.Close();
                hdbConnection.Dispose();

                //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(hdbConnection);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                sdfDatabase.Close();
            }
            finally
            {
                formConvertProgress.Close();
            }
        }

        private void Convert()
        {
            string sql;

            formConvertProgress.MaximumValue = 15;
            formConvertProgress.CurrentTable = "Master";

            ConvertMasterTable();

            #region CD
            formConvertProgress.CurrentTable = "CD";
            formConvertProgress.Value++;

            List<FieldConvert> fieldsCD = new List<FieldConvert>();

            fieldsCD.Add(new FieldConvert("IDCD", "CDID"));
            fieldsCD.Add(new FieldConvert("dwGesamtlaenge", "Length"));
            fieldsCD.Add(new FieldConvert("cAnzahlLieder", "NumberOfTracks"));
            fieldsCD.Add(new FieldConvert("bCDSampler", "IsSampler"));
            fieldsCD.Add(new FieldConvert("wNummerImSet", "SetNumber"));
            fieldsCD.Add(new FieldConvert("IDCDSet", "SetID"));
            fieldsCD.Add(new FieldConvert("IDArtist", "ArtistID"));
            fieldsCD.Add(new FieldConvert("IDKategorie", "CategoryID"));
            fieldsCD.Add(new FieldConvert("IDMedium", "MediumID"));
            fieldsCD.Add(new FieldConvert("szTitel", "Title"));
            fieldsCD.Add(new FieldConvert("szDatum", "Date"));
            fieldsCD.Add(new FieldConvert("szArchivNummer", "ArchiveNumber"));
            fieldsCD.Add(new FieldConvert("szPfadBitmap", "FrontCover"));
            fieldsCD.Add(new FieldConvert("szKennzeichen", "Codes"));
            fieldsCD.Add(new FieldConvert("szKommentar", "Comment"));
            fieldsCD.Add(new FieldConvert("szFeld1", "User1"));
            fieldsCD.Add(new FieldConvert("szFeld2", "User2"));
            fieldsCD.Add(new FieldConvert("szFeld3", "User3"));
            fieldsCD.Add(new FieldConvert("szFeld4", "User4"));
            fieldsCD.Add(new FieldConvert("szFeld5", "User5"));
            fieldsCD.Add(new FieldConvert("C_Type", "Type"));
            fieldsCD.Add(new FieldConvert("C_YearRecorded", "YearRecorded"));
            fieldsCD.Add(new FieldConvert("C_Copyright", "Copyright"));
            fieldsCD.Add(new FieldConvert("C_BackCoverBitmap", "BackCover"));
            fieldsCD.Add(new FieldConvert("C_CDLabelBitmap", "CDLabelCover"));
            fieldsCD.Add(new FieldConvert("C_Rating", "Rating"));
            fieldsCD.Add(new FieldConvert("C_Label", "Label"));
            fieldsCD.Add(new FieldConvert("C_URL", "URL"));
            fieldsCD.Add(new FieldConvert("C_Price", "Price"));
            fieldsCD.Add(new FieldConvert("C_UPC", "UPC"));
            fieldsCD.Add(new FieldConvert("C_Original", "IsOriginal"));
            fieldsCD.Add(new FieldConvert("C_IDComposer", "ComposerID"));
            fieldsCD.Add(new FieldConvert("C_Location", "Location"));
            fieldsCD.Add(new FieldConvert("C_Language", "Language"));

            CDTableAdapter cdAdap = new CDTableAdapter(sdfDatabase);
            ConvertDataSet.CDDataTable cd = new ConvertDataSet.CDDataTable();
            cdAdap.Fill(cd);

            // Alle Identities einlesen
            Dictionary<int, string> identites = ReadIdentities();

            CopyData("CD", fieldsCD, cd, identites);

            SqlCeCommand cmd = new SqlCeCommand("SET IDENTITY_INSERT CD ON", sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            int count = 0;
            foreach (DataRow row in cd.Rows)
            {
                try
                {
                    cdAdap.Update(row);
                }
                catch (Exception e)
                {
                    FormUnhandledException formUnhandledException = new FormUnhandledException(e);

                    formUnhandledException.ShowDialog();
                }

                if ((count % 100) == 0)
                {
                    formConvertProgress.CurrentTable = string.Format("{0} ({1} Einträge)", "Track", count);
                    formConvertProgress.Refresh();
                    Application.DoEvents();
                }

                count++;
            }


            cmd = new SqlCeCommand("SET IDENTITY_INSERT CD OFF", sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(CDID) FROM CD", sdfDatabase.Connection);
            int maxId = (int)cmd.ExecuteScalar() + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE CD ALTER COLUMN CDID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
  
            #endregion

            #region Track
            formConvertProgress.CurrentTable = "Track";
            formConvertProgress.Value++;

            List<FieldConvert> fieldsTrack = new List<FieldConvert>();
            fieldsTrack.Add(new FieldConvert("IDLied", "TrackID"));
            fieldsTrack.Add(new FieldConvert("IDCD", "CDID"));
            fieldsTrack.Add(new FieldConvert("IDArtist", "ArtistID"));
            fieldsTrack.Add(new FieldConvert("wLiedNummer", "TrackNumber"));
            fieldsTrack.Add(new FieldConvert("szTitel", "Title"));
            fieldsTrack.Add(new FieldConvert("dwLaenge", "Length"));
            fieldsTrack.Add(new FieldConvert("wBpm", "Bpm"));
            fieldsTrack.Add(new FieldConvert("szNameRecDatei", "SoundFile"));
            fieldsTrack.Add(new FieldConvert("szKennzeichen", "Codes"));
            fieldsTrack.Add(new FieldConvert("szKommentar", "Comment"));
            fieldsTrack.Add(new FieldConvert("szLiedtext", "Lyrics"));
            fieldsTrack.Add(new FieldConvert("szFeld1", "User1"));
            fieldsTrack.Add(new FieldConvert("szFeld2", "User2"));
            fieldsTrack.Add(new FieldConvert("szFeld3", "User3"));
            fieldsTrack.Add(new FieldConvert("szFeld4", "User4"));
            fieldsTrack.Add(new FieldConvert("szFeld5", "User5"));
            fieldsTrack.Add(new FieldConvert("L_TrackFormat", "TrackFormat"));
            fieldsTrack.Add(new FieldConvert("L_BitRate", "BitRate"));
            fieldsTrack.Add(new FieldConvert("L_SampleRate", "SampleRate"));
            fieldsTrack.Add(new FieldConvert("L_Channels", "Channels"));
            fieldsTrack.Add(new FieldConvert("L_YearRecorded", "YearRecorded"));
            fieldsTrack.Add(new FieldConvert("L_Checksum", "Checksum"));
            fieldsTrack.Add(new FieldConvert("L_Rating", "Rating"));
            fieldsTrack.Add(new FieldConvert("L_IDCategory", "CategoryID"));
            fieldsTrack.Add(new FieldConvert("L_IDComposer", "ComposerID"));
            fieldsTrack.Add(new FieldConvert("L_Language", "Language"));
            TrackTableAdapter trackAdap = new TrackTableAdapter(sdfDatabase);
            ConvertDataSet.TrackDataTable track = new ConvertDataSet.TrackDataTable();
            trackAdap.Fill(track);

            CopyData("Lied", fieldsTrack, track);

            sql = string.Format("SET IDENTITY_INSERT Track ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            count = 0;
            foreach (DataRow row in track.Rows)
            {
                try
                {
                    trackAdap.Update(row);
                }
                catch (Exception e)
                {
                    FormUnhandledException formUnhandledException = new FormUnhandledException(e);

                    formUnhandledException.ShowDialog();
                }

                if ((count % 100) == 0)
                {
                    formConvertProgress.CurrentTable = string.Format("{0} ({1} Einträge)", "Track", count);
                    formConvertProgress.Refresh();
                    Application.DoEvents();
                }

                count++;
            }

            sql = string.Format("SET IDENTITY_INSERT Track OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(TrackID) FROM Track", sdfDatabase.Connection);
            maxId = (int)cmd.ExecuteScalar() + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE Track ALTER COLUMN TrackID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            #endregion

            #region Artist
            formConvertProgress.CurrentTable = "Artist";
            formConvertProgress.Value++;
     
            List<FieldConvert> fieldsPersonGroup = new List<FieldConvert>();
            fieldsPersonGroup.Add(new FieldConvert("IDArtist", "PersonGroupID"));
            fieldsPersonGroup.Add(new FieldConvert("szArtistName", "Name"));
            fieldsPersonGroup.Add(new FieldConvert("sSortKey", "SaveAs"));
            fieldsPersonGroup.Add(new FieldConvert("nGroup", "Type"));
            fieldsPersonGroup.Add(new FieldConvert("nSex", "Sex"));
            fieldsPersonGroup.Add(new FieldConvert("sComment", "Comment"));
            fieldsPersonGroup.Add(new FieldConvert("A_URL", "URL"));
            fieldsPersonGroup.Add(new FieldConvert("A_Country", "Country"));
            fieldsPersonGroup.Add(new FieldConvert("A_BirthDay", "BirthDay", true));
            fieldsPersonGroup.Add(new FieldConvert("A_DayOfDeath", "DayOfDeath", true));
            fieldsPersonGroup.Add(new FieldConvert("A_ImageFilename", "ImageFilename"));
            PersonGroupTableAdapter personGroupAdap = new PersonGroupTableAdapter(sdfDatabase);
            ConvertDataSet.PersonGroupDataTable personGroup = new ConvertDataSet.PersonGroupDataTable();
            personGroupAdap.Fill(personGroup);

            CopyData("Artist", fieldsPersonGroup, personGroup);

            sql = string.Format("SET IDENTITY_INSERT PersonGroup ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            count = 0;
            foreach (DataRow row in personGroup.Rows)
            {
                try
                {
                    personGroupAdap.Update(row);
                }
                catch (Exception e)
                {
                    FormUnhandledException formUnhandledException = new FormUnhandledException(e);

                    formUnhandledException.ShowDialog();
                }
                if ((count % 100) == 0)
                {
                    formConvertProgress.CurrentTable = string.Format("{0} ({1} Einträge)", "Artist", count);
                    formConvertProgress.Refresh();
                    Application.DoEvents();
                }
                count++;
            }

            sql = string.Format("SET IDENTITY_INSERT PersonGroup OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(PersonGroupID) FROM PersonGroup", sdfDatabase.Connection);
            maxId = (int)cmd.ExecuteScalar() + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE PersonGroup ALTER COLUMN PersonGroupID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion

            #region Category
            formConvertProgress.CurrentTable = "Category";
            formConvertProgress.Value++;
           
            List<FieldConvert> fieldsCategory = new List<FieldConvert>();
            fieldsCategory.Add(new FieldConvert("IDKategorie", "CategoryID"));
            fieldsCategory.Add(new FieldConvert("szKategorieName", "Name"));
            fieldsCategory.Add(new FieldConvert("wOrder", "Order"));
            CategoryTableAdapter categoryAdap = new CategoryTableAdapter(sdfDatabase);
            ConvertDataSet.CategoryDataTable categories = new ConvertDataSet.CategoryDataTable();
            categoryAdap.Fill(categories);

            CopyData("Kategorie", fieldsCategory, categories);

            sql = string.Format("SET IDENTITY_INSERT Category ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            categoryAdap.Update(categories);

            sql = string.Format("SET IDENTITY_INSERT Category OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(CategoryID) FROM Category", sdfDatabase.Connection);
            maxId = (int)cmd.ExecuteScalar() + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE Category ALTER COLUMN CategoryID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion

            #region Medium
            formConvertProgress.CurrentTable = "Medium";
            formConvertProgress.Value++;
           
            List<FieldConvert> fieldsMedium = new List<FieldConvert>();
            fieldsMedium.Add(new FieldConvert("IDMedium", "MediumID"));
            fieldsMedium.Add(new FieldConvert("szMedium", "Name"));
            fieldsMedium.Add(new FieldConvert("wOrder", "Order"));
            MediumTableAdapter mediumAdap = new MediumTableAdapter(sdfDatabase);
            ConvertDataSet.MediumDataTable mediums = new ConvertDataSet.MediumDataTable();
            mediumAdap.Fill(mediums);

            CopyData("Medium", fieldsMedium, mediums);

            sql = string.Format("SET IDENTITY_INSERT Medium ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            mediumAdap.Update(mediums);

            sql = string.Format("SET IDENTITY_INSERT Medium OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(MediumID) FROM Medium", sdfDatabase.Connection);
            maxId = (int)cmd.ExecuteScalar() + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE Medium ALTER COLUMN MediumID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion

            #region CDSet
            formConvertProgress.CurrentTable = "CDSet";
            formConvertProgress.Value++;
         
            List<FieldConvert> fieldsSet = new List<FieldConvert>();
            fieldsSet.Add(new FieldConvert("IDCDSet", "SetID"));
            fieldsSet.Add(new FieldConvert("szCDSetName", "Name"));
            SetTableAdapter setAdap = new SetTableAdapter(sdfDatabase);
            ConvertDataSet.SetDataTable sets = new ConvertDataSet.SetDataTable();
            setAdap.Fill(sets);

            CopyData("CDSet", fieldsSet, sets);

            sql = string.Format("SET IDENTITY_INSERT [Set] ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            setAdap.Update(sets);

            sql = string.Format("SET IDENTITY_INSERT [Set] OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(SetID) FROM [Set]", sdfDatabase.Connection);
            object maxSetID = cmd.ExecuteScalar();

            if (maxSetID is DBNull)
                maxId = 1;
            else
                maxId = (int)maxSetID + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE [Set] ALTER COLUMN SetID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion

            #region Program
            formConvertProgress.CurrentTable = "Program";
            formConvertProgress.Value++;

            List<FieldConvert> fieldsProgram = new List<FieldConvert>();
            fieldsProgram.Add(new FieldConvert("IDCD", "CDID"));
            fieldsProgram.Add(new FieldConvert("szName", "Name"));
            fieldsProgram.Add(new FieldConvert("szLieder", "Tracks"));
            fieldsProgram.Add(new FieldConvert("bStandard", "IsStandard"));
            ProgramTableAdapter programAdap = new ProgramTableAdapter(sdfDatabase);
            ConvertDataSet.ProgramDataTable programs = new ConvertDataSet.ProgramDataTable();
            programAdap.Fill(programs);

            CopyData("Programme", fieldsProgram, programs);

            programAdap.Update(programs);
            #endregion

            #region Index
            formConvertProgress.CurrentTable = "Index";
            formConvertProgress.Value++;
         
            List<FieldConvert> fieldsIndex = new List<FieldConvert>();
            fieldsIndex.Add(new FieldConvert("IDCD", "CDID"));
            fieldsIndex.Add(new FieldConvert("szIndexName", "Name"));
            fieldsIndex.Add(new FieldConvert("dwPosition", "Position"));
            IndexTableAdapter indexAdap = new IndexTableAdapter(sdfDatabase);
            ConvertDataSet.IndexDataTable indexes = new ConvertDataSet.IndexDataTable();
            indexAdap.Fill(indexes);

            CopyData("Index", fieldsIndex, indexes);

            indexAdap.Update(indexes);
            #endregion

            #region Codes
            formConvertProgress.CurrentTable = "Codes";
            formConvertProgress.Value++;
        
            List<FieldConvert> fieldsCode = new List<FieldConvert>();
            fieldsCode.Add(new FieldConvert("cBuchstabe", "Letter"));
            fieldsCode.Add(new FieldConvert("szBedeutung", "Description"));
            CodeTableAdapter codeAdap = new CodeTableAdapter(sdfDatabase);
            ConvertDataSet.CodeDataTable codes = new ConvertDataSet.CodeDataTable();
            codeAdap.Fill(codes);

            CopyData("Kennzeichen", fieldsCode, codes);

            codeAdap.Update(codes);
            #endregion

            #region Queue
            formConvertProgress.CurrentTable = "Queue";
            formConvertProgress.Value++;
         
            List<FieldConvert> fieldsQueue = new List<FieldConvert>();
            fieldsQueue.Add(new FieldConvert("Q_lIDCD", "CDID"));
            fieldsQueue.Add(new FieldConvert("Q_lAction", "Action"));
            fieldsQueue.Add(new FieldConvert("Q_sIdentity", "Identity"));
            fieldsQueue.Add(new FieldConvert("Q_sIdentityCDDB", "IdentityCDDB"));
            QueueTableAdapter queueAdap = new QueueTableAdapter(sdfDatabase);
            ConvertDataSet.QueueDataTable queues = new ConvertDataSet.QueueDataTable();
            queueAdap.Fill(queues);

            CopyData("Queue", fieldsQueue, queues);

            queueAdap.Update(queues);
            #endregion

            #region LoanedCDs
            formConvertProgress.CurrentTable = "LoanedCDs";
            formConvertProgress.Value++;
          
            List<FieldConvert> fieldsLoanedCD = new List<FieldConvert>();
            fieldsLoanedCD.Add(new FieldConvert("IDCD", "CDID"));
            fieldsLoanedCD.Add(new FieldConvert("Kommentar", "Comment"));
            fieldsLoanedCD.Add(new FieldConvert("VerliehenAm", "LoanedDate"));
            fieldsLoanedCD.Add(new FieldConvert("VerliehenAn", "LoanedTo"));
            fieldsLoanedCD.Add(new FieldConvert("RueckgabeTermin", "ReturnDate"));
            LoanedCDTableAdapter loanedCDAdap = new LoanedCDTableAdapter(sdfDatabase);
            ConvertDataSet.LoanedCDDataTable loanedCDs = new ConvertDataSet.LoanedCDDataTable();
            loanedCDAdap.Fill(loanedCDs);

            CopyData("VerlieheneCDs", fieldsLoanedCD, loanedCDs);

            loanedCDAdap.Update(loanedCDs);
            #endregion

            #region Dialog
            formConvertProgress.CurrentTable = "Dialog";
            formConvertProgress.Value++;
          
            List<FieldConvert> fieldsDialog = new List<FieldConvert>();
            fieldsDialog.Add(new FieldConvert("D_IDDialog", "DialogID"));
            fieldsDialog.Add(new FieldConvert("D_IDCategory", "CategoryID"));
            fieldsDialog.Add(new FieldConvert("D_DialogXML", "DialogXML"));
            DialogTableAdapter dialogAdap = new DialogTableAdapter(sdfDatabase);
            ConvertDataSet.DialogDataTable dialogs = new ConvertDataSet.DialogDataTable();
            dialogAdap.Fill(dialogs);

            CopyData("Dialog", fieldsDialog, dialogs);

            sql = string.Format("SET IDENTITY_INSERT [Dialog] ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            dialogAdap.Update(dialogs);

            sql = string.Format("SET IDENTITY_INSERT [Dialog] OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(DialogID) FROM [Dialog]", sdfDatabase.Connection);
            maxSetID = cmd.ExecuteScalar();

            if (maxSetID is DBNull)
                maxId = 1;
            else
                maxId = (int)maxSetID + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE [Dialog] ALTER COLUMN DialogID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion

            #region Role
            formConvertProgress.CurrentTable = "Role";
            formConvertProgress.Value++;
          
            List<FieldConvert> fieldsRole = new List<FieldConvert>();
            fieldsRole.Add(new FieldConvert("R_ID", "RoleID"));
            fieldsRole.Add(new FieldConvert("R_Role", "Name"));
            RoleTableAdapter roleAdap = new RoleTableAdapter(sdfDatabase);
            ConvertDataSet.RoleDataTable roles = new ConvertDataSet.RoleDataTable();
            roleAdap.Fill(roles);

            CopyData("Role", fieldsRole, roles);

            sql = string.Format("SET IDENTITY_INSERT [Role] ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            roleAdap.Update(roles);

            sql = string.Format("SET IDENTITY_INSERT [Role] OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(RoleID) FROM [Role]", sdfDatabase.Connection);
            maxSetID = cmd.ExecuteScalar();

            if (maxSetID is DBNull)
                maxId = 1;
            else
                maxId = (int)maxSetID + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE [Role] ALTER COLUMN RoleID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion

            #region Participant
            formConvertProgress.CurrentTable = "Participant";
            formConvertProgress.Value++;
           
            List<FieldConvert> fieldsParticipant = new List<FieldConvert>();
            fieldsParticipant.Add(new FieldConvert("P_ID", "ParticipantID"));
            fieldsParticipant.Add(new FieldConvert("P_IDArtist", "PersonGroupID"));
            fieldsParticipant.Add(new FieldConvert("P_IDRole", "RoleID"));
            fieldsParticipant.Add(new FieldConvert("P_IDCD", "CDID"));
            fieldsParticipant.Add(new FieldConvert("P_TrackNumber", "TrackNumber"));
            fieldsParticipant.Add(new FieldConvert("P_Comment", "Comment"));
            ParticipantTableAdapter participantAdap = new ParticipantTableAdapter(sdfDatabase);
            ConvertDataSet.ParticipantDataTable participants = new ConvertDataSet.ParticipantDataTable();
            participantAdap.Fill(participants);

            CopyData("Participant", fieldsParticipant, participants);

            sql = string.Format("SET IDENTITY_INSERT [Participant] ON");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            participantAdap.Update(participants);

            sql = string.Format("SET IDENTITY_INSERT [Participant] OFF");
            cmd = new SqlCeCommand(sql, sdfDatabase.Connection);
            cmd.ExecuteNonQuery();

            cmd = new SqlCeCommand("SELECT MAX(ParticipantID) FROM [Participant]", sdfDatabase.Connection);
            maxSetID = cmd.ExecuteScalar();

            if (maxSetID is DBNull)
                maxId = 1;
            else
                maxId = (int)maxSetID + 1;

            cmd = new SqlCeCommand(string.Format("ALTER TABLE [Participant] ALTER COLUMN ParticipantID IDENTITY({0},1)", maxId), sdfDatabase.Connection);
            cmd.ExecuteNonQuery();
            #endregion
        }

        private Dictionary<int, string> ReadIdentities()
        {
            string cmdHdb = string.Format("SELECT * from [Identity]");
            OleDbCommand comm = new OleDbCommand(cmdHdb, hdbConnection);
            OleDbDataReader reader = comm.ExecuteReader();
            Dictionary<int, string> identites = new Dictionary<int, string>();

            while (reader.Read())
            {
                int cdid = (int)reader["IDCD"];
                string identity = (string)reader["szIdentity"];

                if (!identites.ContainsKey(cdid))       // Nur einfache Zuordnung erlaubt
                    identites.Add(cdid, identity);
            }

            return identites;
        }

        private void ConvertMasterTable()
        {
            string cmdHdb = "SELECT * from [Master]";
            OleDbCommand comm = new OleDbCommand(cmdHdb, hdbConnection);
            OleDbDataReader reader = comm.ExecuteReader();

            reader.Read();

            short version = (short)GetValue(reader, "wVersion", (short)0);
            string dateName = (string)GetValue(reader, "szNameDatum", "");
            byte dateType = (byte)GetValue(reader, "cTypDatum", (byte)0);
            string userFieldCD1 = (string)GetValue(reader, "szCDbFeld1", "");
            string userFieldCD2 = (string)GetValue(reader, "szCDbFeld2", "");
            string userFieldCD3 = (string)GetValue(reader, "szCDbFeld3", "");
            string userFieldCD4 = (string)GetValue(reader, "szCDbFeld4", "");
            string userFieldCD5 = (string)GetValue(reader, "szCDbFeld5", "");
            string userFieldTrack1 = (string)GetValue(reader, "szTrackbFeld1", "");
            string userFieldTrack2 = (string)GetValue(reader, "szTrackbFeld2", "");
            string userFieldTrack3 = (string)GetValue(reader, "szTrackbFeld3", "");
            string userFieldTrack4 = (string)GetValue(reader, "szTrackbFeld4", "");
            string userFieldTrack5 = (string)GetValue(reader, "szTrackbFeld5", "");
            byte userFieldTypeCD1 = (byte)GetValue(reader, "cTypCDbFeld1", (byte)0);
            byte userFieldTypeCD2 = (byte)GetValue(reader, "cTypCDbFeld2", (byte)0);
            byte userFieldTypeCD3 = (byte)GetValue(reader, "cTypCDbFeld3", (byte)0);
            byte userFieldTypeCD4 = (byte)GetValue(reader, "cTypCDbFeld4", (byte)0);
            byte userFieldTypeCD5 = (byte)GetValue(reader, "cTypCDbFeld5", (byte)0);
            byte userFieldTypeTrack1 = (byte)GetValue(reader, "cTypTrackbFeld1", (byte)0);
            byte userFieldTypeTrack2 = (byte)GetValue(reader, "cTypTrackbFeld2", (byte)0);
            byte userFieldTypeTrack3 = (byte)GetValue(reader, "cTypTrackbFeld3", (byte)0);
            byte userFieldTypeTrack4 = (byte)GetValue(reader, "cTypTrackbFeld4", (byte)0);
            byte userFieldTypeTrack5 = (byte)GetValue(reader, "cTypTrackbFeld5", (byte)0);

            MasterDataSet masterDataSet = new MasterDataSet();
            MasterTableAdapter ta = new MasterTableAdapter(sdfDatabase);
            masterDataSet.Master.AddMasterRow("Version", "", version.ToString());
            masterDataSet.Master.AddMasterRow("DateName", "", dateName);
            masterDataSet.Master.AddMasterRow("DateType", "", dateType.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldName", "CD1", userFieldCD1);
            masterDataSet.Master.AddMasterRow("UserFieldName", "CD2", userFieldCD2);
            masterDataSet.Master.AddMasterRow("UserFieldName", "CD3", userFieldCD3);
            masterDataSet.Master.AddMasterRow("UserFieldName", "CD4", userFieldCD4);
            masterDataSet.Master.AddMasterRow("UserFieldName", "CD5", userFieldCD5);
            masterDataSet.Master.AddMasterRow("UserFieldName", "Track1", userFieldTrack1);
            masterDataSet.Master.AddMasterRow("UserFieldName", "Track2", userFieldTrack2);
            masterDataSet.Master.AddMasterRow("UserFieldName", "Track3", userFieldTrack3);
            masterDataSet.Master.AddMasterRow("UserFieldName", "Track4", userFieldTrack4);
            masterDataSet.Master.AddMasterRow("UserFieldName", "Track5", userFieldTrack5);
            masterDataSet.Master.AddMasterRow("UserFieldType", "CD1", userFieldTypeCD1.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "CD2", userFieldTypeCD2.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "CD3", userFieldTypeCD3.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "CD4", userFieldTypeCD4.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "CD5", userFieldTypeCD5.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "Track1", userFieldTypeTrack1.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "Track2", userFieldTypeTrack2.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "Track3", userFieldTypeTrack3.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "Track4", userFieldTypeTrack4.ToString());
            masterDataSet.Master.AddMasterRow("UserFieldType", "Track5", userFieldTypeTrack5.ToString());

            ta.Update(masterDataSet);
        }

        private object GetValue(OleDbDataReader reader, string columnName, object defaultValue)
        {
            int index = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(index))
                return defaultValue;
            else
                return reader[columnName];
        }

        private void CopyData(string hdbTableName, List<FieldConvert> Fields, DataTable dt)
        {
            CopyData(hdbTableName, Fields, dt, null);
        }

        private void CopyData(string hdbTableName, List<FieldConvert> Fields, DataTable dt, Dictionary<int, string> identities)
        {
            string cmdHdb = string.Format("SELECT * from [{0}]", hdbTableName);
            OleDbCommand comm = new OleDbCommand(cmdHdb, hdbConnection);
            OleDbDataReader reader = comm.ExecuteReader();

            int count = 0;

            int ConvertedCount = 0;

            while (reader.Read())
            {
                DataRow row = dt.NewRow();

                List<object> values = new List<object>();
                foreach (FieldConvert fc in Fields)
                {
                    if (fc.ConvertDateToString)
                    {
                        int index = reader.GetOrdinal(fc.HdbFieldName);
                        if (reader.IsDBNull(index))
                        {
                            row[fc.SdfFieldName] = DBNull.Value;
                        }
                        else
                        {
                            DateTime date = (DateTime)reader[fc.HdbFieldName];
                            string dateString = string.Format("{0:d4}{1:d2}{2:d2}", date.Year, date.Month, date.Day);
                            row[fc.SdfFieldName] = dateString;
                        }
                    }
                    else
                    {
                        row[fc.SdfFieldName] = reader[fc.HdbFieldName];

                        // Workaround für alten Hitbase-Bug. ArtistName kann wohl NULL sein.
                        if (hdbTableName == "Artist" && fc.HdbFieldName == "szArtistName" && row[fc.SdfFieldName] is DBNull)
                            row[fc.SdfFieldName] = "((undefined))";     // Das hier sollte es nicht geben
                    }

                    count++;
                }

                if (identities != null)
                {
                    if (identities.ContainsKey((int)reader["IDCD"]))
                        row["Identity"] = identities[(int)reader["IDCD"]];
                }
                try
                {
                    dt.Rows.Add(row);
                }
                catch
                {
                    // Hm.... irgendwas ist schiefgegangen. Eventuell primary-key-Verletzung eines
                    // alten Hitbase-Bugs, wo man zwei Interpreten mit gleichem Namen anlegen konnte
                    if (hdbTableName == "Artist")
                    {
                        string oldName = row["Name"].ToString();
                        string newName = row["Name"].ToString() + "_" + ConvertedCount.ToString(); 
                        row["Name"] = newName;
                        ConvertedCount++;
                        string msg = string.Format(StringTable.ConvertNameChanged, oldName, newName);
                        MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // und nochmal versuchen....
                        dt.Rows.Add(row);
                    }
                }

                if ((count % 100) == 0)
                {
                    formConvertProgress.CurrentTable = string.Format("{0} ({1} Einträge)", hdbTableName, count);
                    formConvertProgress.Refresh();
                    Application.DoEvents();
                }
                count++;
            }
        }


    }
}
