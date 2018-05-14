using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlServerCe;
using System.Data;
using System.Diagnostics;

namespace Big3.Hitbase.DataBaseEngine
{
    /// <summary>
    /// Konvertiert eine mdb Datei in eine sdf Datei
    /// </summary>
    public class MdbToSdfConverter
    {
        private enum SortDirection
        {
            Ascending,
            Descending
        }

        private class Index
        {
            public string Name;
            public bool Unique;
            public bool PrimaryKey;
            public List<IndexElement> Columns = new List<IndexElement>();
        }

        private class IndexElement
        {
            public string Column;
            public SortDirection SortDirection;
        }

        private class Column
        {
            public string ColumnName;
            public long OrdinalPosition;
            public long Length;
            public string DataType;
            public SqlDbType SqlDbType;
            public bool IsNullable;
            public bool IsAutoIncrement;
        }

        OleDbConnection hdbConnection;
        SqlCeConnection sdfConnection;

        public void Convert(string inputFilename, string outputFilename)
        {
            string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;OLE DB Services=-4;Data Source=\"{0}\"", inputFilename);

            hdbConnection = new OleDbConnection(connString);
            hdbConnection.Open();

            connString = string.Format("Data Source='{0}'", outputFilename);
            sdfConnection = new SqlCeConnection(connString);
            sdfConnection.Open();

            Convert();

            hdbConnection.Close();
            sdfConnection.Close();
        }

        private void Convert()
        {
            // Alle Tabellen der HDB-Datenbank ermitteln
            DataTable dt = hdbConnection.GetSchema("Tables");

            foreach (System.Data.DataRow row in dt.Rows)
            {
                string tableName = row["TABLE_NAME"].ToString();
                string tableType = row["TABLE_TYPE"].ToString();

                if (tableType == "TABLE")
                {
                    CreateTable(tableName);
                }
            }
        }

        /// <summary>
        /// Die Daten kopieren. Hierbei die neue Struktur beachten
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        private void CopyData(string tableName, List<Column> columns)
        {
            string cmd = string.Format("SELECT * from [{0}]", tableName);
            OleDbCommand comm = new OleDbCommand(cmd, hdbConnection);
            OleDbDataReader reader = comm.ExecuteReader();

            SqlCeDataAdapter adap = new SqlCeDataAdapter(cmd, sdfConnection);
            adap.InsertCommand = CreateInsertCommand(tableName, columns);
            adap.InsertCommand.Connection = sdfConnection;
            DataTable dt = new DataTable();
            adap.Fill(dt);
            
            while (reader.Read())
            {
                DataRow row = dt.NewRow();

                List<object> values = new List<object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader.GetValue(i));
                }

                row.ItemArray = values.ToArray();
                dt.Rows.Add(row);
            }

            adap.Update(dt);
        }

        private SqlCeCommand CreateInsertCommand(string tableName, List<Column> columns)
        {
            string insertSql = string.Format("INSERT INTO [{0}] ", tableName);

            string sqlColumns = "";
            foreach (Column col in columns)
            {
                if (!string.IsNullOrEmpty(sqlColumns))
                    sqlColumns += ", ";
                sqlColumns += "[" + col.ColumnName + "]";
            }

            insertSql += "(" + sqlColumns + ") ";

            insertSql += "VALUES";

            string sqlValues = "";
            int count = 0;
            foreach (Column col in columns)
            {
                if (!string.IsNullOrEmpty(sqlValues))
                    sqlValues += ", ";
                sqlValues += string.Format("@param{0}", count + 1);
                count++;
            }

            insertSql += "(" + sqlValues + ")";

            SqlCeCommand insertCommand = new SqlCeCommand(insertSql);
            count=0;
            foreach (Column col in columns)
            {
                string paramName = string.Format("@param{0}", count+1);
                insertCommand.Parameters.Add(paramName, col.SqlDbType, (int)col.Length, col.ColumnName);
                count++;
            }

            return insertCommand;
        }

        private void CreateTable(string tableName)
        {
            string sql = string.Format("CREATE TABLE [{0}] ", tableName);

            // Jetzt alle Spalten der Tabelle ermitteln
            DataTable dt = hdbConnection.GetSchema("Columns");

            List<Column> columns = new List<Column>();

            foreach (DataRow row in dt.Rows)
            {
                if (row["TABLE_NAME"].ToString() == tableName)
                {
                    foreach (System.Data.DataColumn col in dt.Columns)
                    {
                        Debug.WriteLine(string.Format("{0} = {1}", col.ColumnName, row[col]));
                    }
                    Debug.WriteLine("============================");

                    SqlDbType sqlDbType;
                    string colType = "";
                    int dataType = (int)row["DATA_TYPE"];
                    long colFlags = (long)row["COLUMN_FLAGS"];
                    bool autoIncrement = false;
                    switch (dataType)
                    {
                        case 2:
                            colType = "int";
                            sqlDbType = SqlDbType.Int;
                            break;
                        case 3:
                            colType = "int";
                            sqlDbType = SqlDbType.Int;
                            if (colFlags == 90 && tableName != "LoanedCD")
                                autoIncrement = true;
                            break;  
                        case 7:
                            colType = "datetime";
                            sqlDbType = SqlDbType.DateTime;
                            break;
                        case 11:
                            colType = "bit";
                            sqlDbType = SqlDbType.Bit;
                            break;
                        case 17:
                            colType = "smallint";
                            sqlDbType = SqlDbType.SmallInt;
                            break;
                        case 130:
                            if ((long)row["CHARACTER_MAXIMUM_LENGTH"] == 0)
                            {
                                colType = "ntext";
                                sqlDbType = SqlDbType.NText;
                            }
                            else
                            {
                                colType = string.Format("nvarchar({0})", row["CHARACTER_MAXIMUM_LENGTH"]);
                                sqlDbType = SqlDbType.NVarChar;
                            }
                            break;
                        default:
                            colType = "int";
                            sqlDbType = SqlDbType.Int;
                            break;
                    }

                    Column newColumn = new Column();
                    newColumn.ColumnName = row["COLUMN_NAME"].ToString();
                    newColumn.DataType = colType;
                    newColumn.OrdinalPosition = (long)row["ORDINAL_POSITION"];
                    newColumn.SqlDbType = sqlDbType;
                    newColumn.IsNullable = (bool)row["IS_NULLABLE"];
                    if (!(row["CHARACTER_MAXIMUM_LENGTH"] is DBNull))
                        newColumn.Length = (long)row["CHARACTER_MAXIMUM_LENGTH"];
                    if (autoIncrement)
                        newColumn.IsAutoIncrement = autoIncrement;
                    columns.Add(newColumn);
                }
            }

            List<Index> indexes = new List<Index>();
            FindIndexes(indexes, tableName);

            columns.Sort(new Comparison<Column>(SortColumns));

            string sqlColumns = "";
            foreach (Column col in columns)
            {
                if (!string.IsNullOrEmpty(sqlColumns))
                    sqlColumns += ", ";
                sqlColumns += string.Format("[{0}] {1}", col.ColumnName, col.DataType);
                if (col.IsAutoIncrement)
                    sqlColumns += string.Format(" IDENTITY (1, 1)");
                if (!col.IsNullable || IsInPrimaryKey(indexes, col.ColumnName))
                    sqlColumns += string.Format(" NOT NULL");
            }
            
            sql += "(" + sqlColumns + ")";

            SqlCeCommand cmd = new SqlCeCommand(sql, sdfConnection);
            cmd.ExecuteNonQuery();

            // Contraints
            foreach (Index index in indexes)
            {
                String sqlIndex;

                if (index.PrimaryKey)
                {
                    sqlIndex = string.Format("ALTER TABLE [{0}] ADD CONSTRAINT [{1}] PRIMARY KEY", tableName, index.Name);
                }
                else
                    sqlIndex = string.Format("CREATE {0} INDEX [{1}] ON [{2}]", index.Unique ? "UNIQUE" : "", index.Name, tableName);
                sqlIndex += " (";

                int count = 0;
                foreach (IndexElement column in index.Columns)
                {
                    if (count > 0)
                        sqlIndex += ", ";
                    sqlIndex += "[" + column.Column + "]";
                    count++;
                }

                        
                sqlIndex += ")";
                SqlCeCommand cmdIndex = new SqlCeCommand(sqlIndex, sdfConnection);
                cmdIndex.ExecuteNonQuery();

            }

            //CopyData(tableName, columns);
        }

        private bool IsInPrimaryKey(List<Index> indexes, string columnName)
        {
            foreach (Index index in indexes)
            {
                if (index.PrimaryKey)
                {
                    foreach (IndexElement elem in index.Columns)
                    {
                        if (elem.Column == columnName)
                            return true;
                    }
                }
            }

            return false;
        }

        private void FindIndexes(List<Index> indexes, string tableName)
        {
            DataTable dtIndexes = hdbConnection.GetSchema("Indexes");

            foreach (DataRow row in dtIndexes.Rows)
            {
                if (row["TABLE_NAME"].ToString() == tableName)
                {
                    Index index = null;
                    bool newIndex = true;
                    
                    foreach (Index searchIndex in indexes)
                    {
                        if (searchIndex.Name == (string)row["INDEX_NAME"])
                        {
                            index = searchIndex;
                            newIndex = false;
                        }
                    }

                    if (index == null)
                        index = new Index();
                    index.Name = (string)row["INDEX_NAME"];
                    IndexElement indexElement = new IndexElement();
                    indexElement.Column = (string)row["COLUMN_NAME"];
                    index.Unique = (bool)row["UNIQUE"];
                    index.PrimaryKey = (bool)row["PRIMARY_KEY"];
                    index.Columns.Add(indexElement);
                    if (newIndex)
                        indexes.Add(index);
                }
            }
        }

        private int SortColumns(Column a, Column b)
        {
            if (a.OrdinalPosition < b.OrdinalPosition)
                return -1;
            if (a.OrdinalPosition > b.OrdinalPosition)
                return 1;
            
            return 0;
        }

        private static void DisplayData(System.Data.DataTable table)
        {
            foreach (System.Data.DataRow row in table.Rows)
            {
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    Debug.WriteLine(string.Format("{0} = {1}", col.ColumnName, row[col]));
                }
                Debug.WriteLine("============================");
            }
        }

    }
}
