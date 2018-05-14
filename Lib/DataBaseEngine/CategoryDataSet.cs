namespace Big3.Hitbase.DataBaseEngine {
    
    
    public partial class CategoryDataSet {
    }
}

namespace Big3.Hitbase.DataBaseEngine.CategoryDataSetTableAdapters
{
    public partial class CategoryTableAdapter
    {
        public CategoryTableAdapter(DataBase db)
            : this()
        {
            Connection = db.Connection;
        }
    }
}