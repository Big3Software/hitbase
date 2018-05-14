namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class MediumDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.MediumDataSetTableAdapters {
    
    
    public partial class MediumTableAdapter {
        public MediumTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
 }
}
