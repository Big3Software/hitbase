using System;
using System.Collections.Generic;
using System.Text;
using Big3.Hitbase.DataBaseEngine.CategoryDataSetTableAdapters;

namespace Big3.Hitbase.DataBaseEngine
{
    public class Categories : List<Category>
    {
        private DataBase dataBase;

        public Categories(DataBase db)
        {
            dataBase = db;
        }

        public void AddNew(string category)
        {
            // Nicht vorhanden, also neue Kategorie anlegen
            CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
            CategoryDataSet.CategoryDataTable cdt = cta.GetData();
            cdt.AddCategoryRow(category, GetNextOrder());
            cta.Update(cdt);

            dataBase.UpdateCategories();
        }

        public void Add(CategoryDataSet.CategoryRow category)
        {
            Category newCategory = new Category();
            newCategory.CategoryID = category.CategoryID;
            newCategory.Name = category.Name;
            newCategory.Order = category.Order;
            this.Add(newCategory);
        }

        public Category GetByName(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                    return this[i];
            }

            return null;
        }

        public int GetIdByName(string name)
        {
            Category category = GetByName(name);

            if (category != null)
                return category.CategoryID;
            else
                return 0;
        }

        /// <summary>
        /// Liefert die nächste freie Ordnungszahl zurück.
        /// </summary>
        /// <returns></returns>
        private int GetNextOrder()
        {
            int highestOrder = 0;
            foreach (Category cat in this)
            {
                if (cat.Order > highestOrder)
                    highestOrder = cat.Order;
            }

            return highestOrder + 1;
        }

        public int GetIdByName(string name, bool createIfNew)
        {
            if (string.IsNullOrEmpty(name))
                return 0;

            Category category = GetByName(name);

            if (category != null)
                return category.CategoryID;
            else
            {
                // Nicht vorhanden, also neue Kategorie anlegen
                CategoryTableAdapter cta = new CategoryTableAdapter(dataBase);
                CategoryDataSet.CategoryDataTable cdt = cta.GetData();
                cdt.AddCategoryRow(name, cdt.Rows.Count);
                cta.Update(cdt);

                int categoryid = (int)(decimal)dataBase.ExecuteScalar("SELECT @@IDENTITY");

                dataBase.UpdateCategories();

                return categoryid;
            }
        }

        public Category GetById(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].CategoryID == id)
                    return this[i];
            }

            return null;
        }

        public List<string> Names
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Category category in this)
                    names.Add(category.Name);

                return names;
            }
        }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
