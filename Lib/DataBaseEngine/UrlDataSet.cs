namespace Big3.Hitbase.DataBaseEngine
{


    public partial class UrlDataSet
    {
    }
}

namespace Big3.Hitbase.DataBaseEngine.UrlDataSetTableAdapters
{
    public partial class UrlTableAdapter
    {
        public UrlTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
    }
}
