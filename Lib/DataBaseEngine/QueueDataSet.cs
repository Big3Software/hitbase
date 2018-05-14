namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class QueueDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.QueueDataSetTableAdapters
{
    public partial class QueueTableAdapter
    {
        public QueueTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }

        public void Add(int id, int action, string cdarchivID, string cddbID)
        {
            Insert(id, action, cdarchivID, cddbID);
        }
    }
}
