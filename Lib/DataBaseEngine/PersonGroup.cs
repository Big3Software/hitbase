using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Big3.Hitbase.Miscellaneous;
using System.Data.SqlServerCe;
using System.Data;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.DataBaseEngine
{
    public class PersonGroup : INotifyPropertyChanged
    {
        public PersonGroup()
        {

        }

        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; FirePropertyChanged("ID"); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; FirePropertyChanged("Name"); }
        }

        private string saveAs;
        public string SaveAs
        {
            get { return saveAs; }
            set { saveAs = value; FirePropertyChanged("SaveAs"); }
        }

        private PersonGroupType type;
        public PersonGroupType Type
        {
            get { return type; }
            set { type = value; FirePropertyChanged("Type"); }
        }

        private SexType sex;
        public SexType Sex
        {
            get { return sex; }
            set { sex = value; FirePropertyChanged("Sex"); }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set { comment = value; FirePropertyChanged("Comment"); }
        }

        private string homepage;
        public string Homepage
        {
            get { return homepage; }
            set { homepage = value; FirePropertyChanged("Homepage"); }
        }

        private string country;
        public string Country
        {
            get { return country; }
            set { country = value; FirePropertyChanged("Country"); }
        }

        private string birthday;
        public string Birthday
        {
            get { return birthday; }
            set { birthday = value; FirePropertyChanged("Birthday"); }
        }

        private string dayofdeath;
        public string DayOfDeath
        {
            get { return dayofdeath; }
            set { dayofdeath = value; FirePropertyChanged("DayOfDeath"); }
        }

        private string imageFilename;
        public string ImageFilename
        {
            get { return imageFilename; }
            set { imageFilename = value; FirePropertyChanged("ImageFilename"); }
        }

        private UrlList urls = new UrlList();
        public UrlList Urls
        {
            get { return urls; }
            set { urls = value; FirePropertyChanged("Urls"); }
        }

        private GroupParticipantList participants = new GroupParticipantList();
        public GroupParticipantList Participants
        {
            get { return participants; }
            set { participants = value; FirePropertyChanged("Participants"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Speichert die Person/Gruppe.
        /// </summary>
        /// <param name="db"></param>
        public void Save(DataBase dataBase)
        {
            SqlCeTransaction trans = dataBase.Connection.BeginTransaction(IsolationLevel.ReadCommitted);

            Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter pgta = new Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters.PersonGroupTableAdapter(dataBase);
            pgta.Transaction = trans;
            PersonGroupDataSet.PersonGroupDataTable dt = pgta.GetDataById(id);

            PersonGroupDataSet.PersonGroupRow row = null;

            bool isNew = false;
            int personGroupId = 0;
            if (dt.Rows.Count == 1)
            {
                row = dt[0];
                personGroupId = dt[0].PersonGroupID;
            }
            else
            {
                row = dt.NewPersonGroupRow();
                isNew = true;
            }

            row.Name = Name;
            row.SaveAs = SaveAs;
            row.Type = (int)Type;
            row.Sex = (int)Sex;
            row.Country = Country;
            row.BirthDay = Birthday;
            row.DayOfDeath = DayOfDeath;
            row.ImageFilename = ImageFilename;
            row.URL = Homepage;
            row.Comment = Comment;

            if (isNew)
                dt.AddPersonGroupRow(row);

            pgta.Update(dt);

            if (isNew)
                personGroupId = (int)(decimal)dataBase.ExecuteScalar("SELECT @@IDENTITY", trans);

            // Urls speichern
            string sql = string.Format("DELETE FROM Url Where ReferenceID = {0}", personGroupId);
            dataBase.ExecuteScalar(sql);

            UrlDataSetTableAdapters.UrlTableAdapter urlta = new UrlDataSetTableAdapters.UrlTableAdapter(dataBase);
            urlta.Transaction = trans;
            UrlDataSet.UrlDataTable urlDataTable = new UrlDataSet.UrlDataTable();
            foreach (Url url in Urls)
            {
                urlDataTable.AddUrlRow(personGroupId, 0, url.UrlType, url.Link);
            }
            urlta.Update(urlDataTable);

            // Jetzt noch die Mitwirkenden der Gruppe
            sql = string.Format("DELETE FROM GroupParticipant Where PersonGroupID = {0}", personGroupId);
            dataBase.ExecuteScalar(sql);

            GroupParticipantDataSetTableAdapters.GroupParticipantTableAdapter participantTableAdapter = new GroupParticipantDataSetTableAdapters.GroupParticipantTableAdapter(dataBase);
            participantTableAdapter.Transaction = trans;
            GroupParticipantDataSet.GroupParticipantDataTable participantsDataTable = new GroupParticipantDataSet.GroupParticipantDataTable();
            foreach (GroupParticipant gp in this.Participants)
            {
                if (!string.IsNullOrEmpty(gp.Name))
                {
                    int participantId = dataBase.GetPersonGroupByName(gp.Name, true).ID;
                    int roleId = dataBase.GetRoleByName(gp.Role, true).RoleID;

                    participantsDataTable.AddGroupParticipantRow(personGroupId, participantId, roleId, gp.Begin == null ? "" : gp.Begin, gp.End == null ? "" : gp.End);
                }
            }
            participantTableAdapter.Update(participantsDataTable);

            trans.Commit();
        }

        public string GetStringByField(Field field)
        {
            object val = GetValueByField(field);

            if (val is bool)
            {
                if ((bool)val == true)
                    return StringTable.Yes;
                else
                    return StringTable.No;
            }

            if (val != null)
                return val.ToString();
            else
                return null;
        }

        public object GetValueByField(Field field)
        {
            switch (field)
            {
                case Field.ArtistCDName:
                case Field.ArtistTrackName:
                case Field.ComposerCDName:
                case Field.ComposerTrackName:
                    return Name;
                case Field.ArtistCDSaveAs:
                case Field.ArtistTrackSaveAs:
                case Field.ComposerCDSaveAs:
                case Field.ComposerTrackSaveAs:
                    return SaveAs;
                case Field.ArtistCDCountry:
                case Field.ArtistTrackCountry:
                case Field.ComposerCDCountry:
                case Field.ComposerTrackCountry:
                    return Country;
                case Field.ArtistCDDateOfBirth:
                case Field.ArtistTrackDateOfBirth:
                case Field.ComposerCDDateOfBirth:
                case Field.ComposerTrackDateOfBirth:
                    return Birthday;
                case Field.ArtistCDDateOfDeath:
                case Field.ArtistTrackDateOfDeath:
                case Field.ComposerCDDateOfDeath:
                case Field.ComposerTrackDateOfDeath:
                    return DayOfDeath;
                case Field.ArtistCDHomepage:
                case Field.ArtistTrackHomepage:
                case Field.ComposerCDHomepage:
                case Field.ComposerTrackHomepage:
                    return Homepage;
                case Field.ArtistCDImageFilename:
                case Field.ArtistTrackImageFilename:
                case Field.ComposerCDImageFilename:
                case Field.ComposerTrackImageFilename:
                    return ImageFilename;
                case Field.ArtistCDSex:
                case Field.ArtistTrackSex:
                case Field.ComposerCDSex:
                case Field.ComposerTrackSex:
                    return Sex;
                case Field.ArtistCDType:
                case Field.ArtistTrackType:
                case Field.ComposerCDType:
                case Field.ComposerTrackType:
                    return Type;
                case Field.ArtistCDComment:
                case Field.ArtistTrackComment:
                case Field.ComposerCDComment:
                case Field.ComposerTrackComment:
                    return Comment;
                default:
                    throw new Exception("GetValueFromField: Unknown field: " + field.ToString());
            }
        }
    }

    public class PersonGroupList : SafeObservableCollection<PersonGroup>
    {
        public PersonGroupList()
        {

        }
    }
}
