namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class CDDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.CDDataSetTableAdapters
{
    public partial class CDTableAdapter
    {
        public CDTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}
