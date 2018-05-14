namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class TrackDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.TrackDataSetTableAdapters
{
    public partial class TrackTableAdapter
    {
        public TrackTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}