using System;
using System.Collections.Generic;
using System.Text;
using Big3.Hitbase.DataBaseEngine.MasterDataSetTableAdapters;

namespace Big3.Hitbase.DataBaseEngine
{
    /// <summary>
    /// Enthält alle Datenbank-Felder und weitere Datenbank-spezifische
    /// Einstellungen (z.B. auch die aktuelle Version der Datenbank)
    /// </summary>
    public class MasterConfig
    {
        /// <summary>
        /// Die Anzahl der benutzerdefinierten Felder
        /// </summary>
        public static int MaximumNumberOfUserFields = 5;

        /// <summary>
        /// Die Datenbankversion
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Der Name des Datum-Feldes
        /// </summary>
        public string DateName { get; set; }

        /// <summary>
        /// Der Typ des Datum-Feldes
        /// </summary>
        public DateType DateType { get; set; }

        /// <summary>
        /// Die Liste aller benutzerdefinierten CD-Felder
        /// </summary>
        public List<UserField> UserCDFields { get; set; }

        /// <summary>
        /// Die Liste aller benutzerdefinierten Track-Felder
        /// </summary>
        public List<UserField> UserTrackFields { get; set; }

        /// <summary>
        /// Verzeichnisse, die für die Musikdateien überwacht werden sollen 
        /// </summary>
        public List<string> MonitoredDirectories { get; set; }

        public void ReadConfig(DataBase db)
        {
            MasterDataSet masterDataSet = new MasterDataSet();
            MasterTableAdapter ta = new MasterTableAdapter(db);
            ta.Fill(masterDataSet.Master);

            int version = 0;
            if (Int32.TryParse(GetValue(masterDataSet.Master, "Version", "", ""), out version))
                Version = version;

            DateName = GetValue(masterDataSet.Master, "DateName", "", "");
            int dateType = 0;
            if (Int32.TryParse(GetValue(masterDataSet.Master, "DateType", "", ""), out dateType))
                DateType = (DateType)dateType;

            UserCDFields = new List<UserField>();
            UserTrackFields = new List<UserField>();
            for (int i = 0; i < MaximumNumberOfUserFields; i++)
            {
                string cdField = string.Format("CD{0}", i+1);
                UserField uf = new UserField();
                uf.Name= GetValue(masterDataSet.Master, "UserFieldName", cdField, "");
                int fieldType = 0;
                if (Int32.TryParse(GetValue(masterDataSet.Master, "UserFieldType", cdField, ""), out fieldType))
                    uf.Type = (UserFieldType)fieldType;
                UserCDFields.Add(uf);

                string trackField = string.Format("Track{0}", i + 1);
                UserField ufTrack = new UserField();
                ufTrack.Name = GetValue(masterDataSet.Master, "UserFieldName", trackField, "");
                int fieldTypeTrack = 0;
                if (Int32.TryParse(GetValue(masterDataSet.Master, "UserFieldType", trackField, ""), out fieldTypeTrack))
                    ufTrack.Type = (UserFieldType)fieldTypeTrack;
                UserTrackFields.Add(ufTrack);
            }

            MonitoredDirectories = new List<string>();
            int count = 0;
            while (true)
            {
                string directory = GetValue(masterDataSet.Master, "MonitorDir", count.ToString(), "");

                if (string.IsNullOrEmpty(directory))
                    break;

                MonitoredDirectories.Add(directory);

                count++;
            }
        }

        public void WriteConfig(DataBase db)
        {
            MasterDataSet masterDataSet = new MasterDataSet();
            MasterTableAdapter ta = new MasterTableAdapter(db);
            ta.Fill(masterDataSet.Master);

            SetValue(masterDataSet.Master, "DateName", "", DateName);
            SetValue(masterDataSet.Master, "DateType", "", ((int)DateType).ToString());

            for (int i = 0; i < MaximumNumberOfUserFields; i++)
            {
                string cdField = string.Format("CD{0}", i + 1);
                SetValue(masterDataSet.Master, "UserFieldName", cdField, UserCDFields[i].Name);
                SetValue(masterDataSet.Master, "UserFieldType", cdField, ((int)UserCDFields[i].Type).ToString());

                string trackField = string.Format("Track{0}", i + 1);
                SetValue(masterDataSet.Master, "UserFieldName", trackField, UserTrackFields[i].Name);
                SetValue(masterDataSet.Master, "UserFieldType", trackField, ((int)UserTrackFields[i].Type).ToString());
            }

            ta.Update(masterDataSet);

            // Alte Einträge löschen
            foreach (MasterDataSet.MasterRow row in masterDataSet.Master)
            {
                if (row.Code.StartsWith("MonitorDir"))
                {
                    ta.Delete(row.Code, row.CodeSub);
                }
            }

            for (int i = 0; i < MonitoredDirectories.Count; i++)
            {
                ta.Insert("MonitorDir", i.ToString(), MonitoredDirectories[i]);
            }
        }

        private string GetValue(MasterDataSet.MasterDataTable masterDataTable, string code, string codeSub, string defaultValue)
        {
            foreach (MasterDataSet.MasterRow row in masterDataTable.Rows)
            {
                if (row.Code == code && row.CodeSub == codeSub)
                    return row.Content;
            }

            return defaultValue;
        }

        private void SetValue(MasterDataSet.MasterDataTable masterDataTable, string code, string codeSub, string value)
        {
            foreach (MasterDataSet.MasterRow row in masterDataTable.Rows)
            {
                if (row.Code == code && row.CodeSub == codeSub)
                {
                    row.Content = value;
                    return;
                }
            }

            masterDataTable.AddMasterRow(code, codeSub, value);
        }
    }

    public enum DateType
    {
        YYYYMMDD=0,
        YYYYMM,
        YYYY,
        None
    }

    public enum UserFieldType
    {
        Text=0,
        Number,
        Boolean,
        Currency,
        Date
    }

    public class UserField
    {
        /// <summary>
        /// Der Name des benutzerdefinierten Feldes
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Der Typ des benutzerdefinierten Feldes
        /// </summary>
        public UserFieldType Type { get; set; }
    }
}
