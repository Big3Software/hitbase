namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class IndexDataSet {
        partial class IndexDataTable
        {
        }
    }
}

namespace Big3.Hitbase.DataBaseEngine.IndexDataSetTableAdapters
{
    public partial class IndexTableAdapter
    {
        public IndexTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}