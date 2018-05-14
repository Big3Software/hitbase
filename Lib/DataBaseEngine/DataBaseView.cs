using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace Big3.Hitbase.DataBaseEngine
{
    public class DataBaseView : IDisposable
    {
        private SqlCeDataReader reader;

        public static DataBaseView Create(DataBase db, string sql)
        {
            DataBaseView view = new DataBaseView();

            SqlCeCommand command = new SqlCeCommand(sql, db.Connection);
            view.reader = command.ExecuteReader();

            return view;
        }

        public object[] Read()
        {
            if (reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);
                return values;
            }

            return null;
        }

        public int GetColumnIndex(string fieldName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == fieldName)
                    return i;
            }

            return -1;
        }

        public void Close()
        {
            reader.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
