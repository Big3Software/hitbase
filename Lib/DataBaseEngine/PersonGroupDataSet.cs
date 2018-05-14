namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class PersonGroupDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.PersonGroupDataSetTableAdapters
{
    public partial class PersonGroupTableAdapter
    {
        public PersonGroupTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }

        public PersonGroupDataSet.PersonGroupRow GetPersonGroupByName(string name, bool createIfNotFound)
        {
            PersonGroupDataSet personGroupDataset = new PersonGroupDataSet();

            int numberOfPersonGroups = FillByName(personGroupDataset.PersonGroup, name);

            if (numberOfPersonGroups > 0)
                return personGroupDataset.PersonGroup[0];
            else
            {
                if (createIfNotFound && !string.IsNullOrEmpty(name))
                {
                    this.Insert(name, name, null, null, null, null, null, null, null, null);
                    
                    //Updating, um ID zu erhalten                 
                    numberOfPersonGroups = FillByName(personGroupDataset.PersonGroup, name);

                    if (numberOfPersonGroups > 0)
                        return personGroupDataset.PersonGroup[0];
                }

                return null;
            }
        }

        public PersonGroupDataSet.PersonGroupRow GetPersonGroupById(int id)
        {
            PersonGroupDataSet personGroupDataset = new PersonGroupDataSet();

            int numberOfPersonGroups = FillById(personGroupDataset.PersonGroup, id);

            if (numberOfPersonGroups > 0)
                return personGroupDataset.PersonGroup[0];
            else
                return null;
        }
    }
}
