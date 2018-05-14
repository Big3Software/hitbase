namespace Big3.Hitbase.DataBaseEngine
{


    public partial class GroupParticipantDataSet
    {
    }
}

namespace Big3.Hitbase.DataBaseEngine.GroupParticipantDataSetTableAdapters
{
    public partial class GroupParticipantTableAdapter
    {
        public GroupParticipantTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
    }
}
