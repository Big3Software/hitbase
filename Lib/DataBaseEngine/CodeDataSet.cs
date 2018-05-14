namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class CodeDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.CodeDataSetTableAdapters
{
    public partial class CodeTableAdapter
    {
        public CodeTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}
