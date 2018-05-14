namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class SetDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.SetDataSetTableAdapters
{
    public partial class SetTableAdapter
    {
        public SetTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
    }
}
