namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class MasterDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.MasterDataSetTableAdapters
{
    public partial class MasterTableAdapter
    {
        public MasterTableAdapter(DataBase db) : this()
        {
            Connection = db.Connection;
        }
    }
}