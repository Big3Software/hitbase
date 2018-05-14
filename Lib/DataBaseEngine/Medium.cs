using System;
using System.Collections.Generic;
using System.Text;
using Big3.Hitbase.DataBaseEngine.MediumDataSetTableAdapters;

namespace Big3.Hitbase.DataBaseEngine
{
    public class Mediums : List<Medium>
    {
        private DataBase dataBase;

        public Mediums(DataBase db)
        {
            dataBase = db;
        }

        public void AddNew(string medium)
        {
            // Nicht vorhanden, also neues Medium anlegen
            MediumTableAdapter mta = new MediumTableAdapter(dataBase);
            MediumDataSet.MediumDataTable mdt = mta.GetData();
            mdt.AddMediumRow(medium, GetNextOrder());
            mta.Update(mdt);

            dataBase.UpdateMediums();
        }

        public void Add(MediumDataSet.MediumRow medium)
        {
            Medium newMedium = new Medium();
            newMedium.MediumID = medium.MediumID;
            newMedium.Name = medium.Name;
            newMedium.Order = medium.Order;
            this.Add(newMedium);
        }

        public Medium GetByName(string name)
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
            Medium medium = GetByName(name);

            if (medium != null)
                return medium.MediumID;
            else
                return 0;
        }
        
        public Medium GetById(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].MediumID == id)
                    return this[i];
            }

            return null;
        }

        /// <summary>
        /// Liefert die nächste freie Ordnungszahl zurück.
        /// </summary>
        /// <returns></returns>
        private int GetNextOrder()
        {
            int highestOrder = 0;
            foreach (Medium medium in this)
            {
                if (medium.Order > highestOrder)
                    highestOrder = medium.Order;
            }

            return highestOrder + 1;
        }


        public List<string> Names
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Medium medium in this)
                    names.Add(medium.Name);

                return names;
            }
        }
    }

    public class Medium
    {
        public int MediumID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
