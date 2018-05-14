namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class ProgramDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.ProgramDataSetTableAdapters
{
    public partial class ProgramTableAdapter
    {
        public ProgramTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}