namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class DialogDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.DialogDataSetTableAdapters
{
    public partial class DialogTableAdapter
    {
        public DialogTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}