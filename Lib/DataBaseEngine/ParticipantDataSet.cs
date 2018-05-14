namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class ParticipantDataSet {
        partial class ParticipantDataTable
        {
        }
    }
}

namespace Big3.Hitbase.DataBaseEngine.ParticipantDataSetTableAdapters
{
    public partial class ParticipantTableAdapter
    {
        public ParticipantTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
    }
}
