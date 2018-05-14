namespace Big3.Hitbase.DataBaseEngine
{


    public partial class SearchDataSet
    {
    }
}

namespace Big3.Hitbase.DataBaseEngine.SearchDataSetTableAdapters
{
    public partial class SearchTableAdapter
    {
        public SearchTableAdapter(DataBase db)
        {
            Connection = db.Connection;
        }
    }
}
