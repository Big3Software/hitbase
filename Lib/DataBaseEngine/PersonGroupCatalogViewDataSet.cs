namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class PersonGroupCatalogViewDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.PersonGroupCatalogViewDataSetTableAdapters
{
    public partial class PersonGroupTableAdapter
    {
        public PersonGroupTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
    }
}