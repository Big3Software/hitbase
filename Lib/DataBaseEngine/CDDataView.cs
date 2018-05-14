using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Big3.Hitbase.Configuration;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.DataBaseEngine
{
    public class CDDataView 
    {
        Dictionary<Field, string> fieldsStringDictionary = new Dictionary<Field, string>();
        Dictionary<Field, string> fieldsValueDictionary = new Dictionary<Field, string>();

        FieldCollection fieldsToRead = new FieldCollection();

        private DataTable resultDataTable;
        private DataTable dataTable = new DataTable();

        private string CDIDColumnName;
        private string ArtistIDColumnName;

        public CDDataView(DataBase db, CDQueryDataSet cdQuery, Condition condition, SortFieldCollection sortedFields)
            : this(db, cdQuery, condition, sortedFields, FieldHelper.GetAllCDFields(false))
        {
        }

        public CDDataView(DataBase db, CDQueryDataSet cdQuery, Condition condition, SortFieldCollection sortedFields, FieldCollection fields)
        {
            // Zunächst die Spalten definieren
            for (int col = 0; col < fields.Count; col++)
            {
                AddField(db, fields[col]);
            }

            if (sortedFields != null)
            {
                for (int col = 0; col < sortedFields.Count; col++)
                {
                    Field field = GetRealSortField(sortedFields[col].Field);

                    AddField(db, field);
                }
            }

            if (condition != null)
            {
                // Jetzt noch die Spalten für die Condition definieren
                for (int col = 0; col < condition.Count; col++)
                {
                    if (!fieldsValueDictionary.ContainsKey(condition[col].Field))
                    {
                        AddField(db, condition[col].Field);
                    }
                }
            }

            // Für die ID der CD auch noch eine Spalte vorsehen
            DataColumn dc = new DataColumn();
            dc.Caption = "CDID";
            dc.DataType = typeof(int);
            dataTable.Columns.Add(dc);
            CDIDColumnName = dc.ColumnName;

            // Für die ID des Interpreten auch noch eine Spalte vorsehen
            DataColumn dcArtist = new DataColumn();
            dcArtist.Caption = "ArtistID";
            dcArtist.DataType = typeof(int);
            dataTable.Columns.Add(dcArtist);
            ArtistIDColumnName = dcArtist.ColumnName;

            // Alle Inhalte füllen
            for (int i = 0; i < cdQuery.CD.Count; i++)
            {
                List<object> values = new List<object>();
                for (int col = 0; col < fieldsToRead.Count; col++)
                {
                    object rawValue = cdQuery.CD[i].GetValueByField(fieldsToRead[col]);

                    string stringValue = CDQueryDataSet.CDRow.GetStringByField(db, fieldsToRead[col], rawValue);

                    values.Add(stringValue);

                    if (Settings.Current.SortArchiveNumberNumeric && fieldsToRead[col] == Field.ArchiveNumber)
                        rawValue = Misc.Atoi((string)rawValue);

                    values.Add(rawValue);
                }

                values.Add(cdQuery.CD[i].CDID);
                values.Add(cdQuery.CD[i].ArtistID);
                dataTable.Rows.Add(values.ToArray());
            }

            // Sortierung definieren
            string sortString = "";
            foreach (SortField sortField in sortedFields)
            {
                Field field = GetRealSortField(sortField.Field);

                if (fieldsStringDictionary.ContainsKey(field))
                {
                    if (!string.IsNullOrEmpty(sortString))
                        sortString += ", ";

                    // Bei einigen Feldern wird nach Wert sortiert, bei den meisten nach der String-Repräsentation
                    if (SortFieldByValue(db, field))
                        sortString += fieldsValueDictionary[field];
                    else
                        sortString += fieldsStringDictionary[field];

                    if (sortField.SortDirection == SortDirection.Descending)
                        sortString += " DESC";
                }
            }

            dataTable.DefaultView.Sort = sortString;

            if (condition != null)
            {
                string filterString = "";
                // Filter definieren
                foreach (SingleCondition cond in condition)
                {
                    if (!string.IsNullOrEmpty(filterString))
                    {
                        if (cond.Logical == Logical.Or)
                            filterString += " OR ";
                        else
                            filterString += " AND ";
                    }

                    if (fieldsStringDictionary.ContainsKey(cond.Field))
                    {
                        if (DataBase.GetTypeByField(cond.Field) == typeof(string))
                        {
                            if (cond.Field != Field.Date && cond.Field != Field.ArchiveNumber)
                                filterString += fieldsStringDictionary[cond.Field];
                            else
                                filterString += fieldsValueDictionary[cond.Field];

                            filterString += " " + Condition.GetNameOfOperatorForFilter(cond.Operator) + " ";

                            string val = cond.Value.ToString();

                            val = val.Replace("'", "''");
                            if (cond.Operator == Operator.Contains || cond.Operator == Operator.NotContains)
                            {
                                filterString += "'%" + val + "%'";
                            }
                            else
                            {
                                if (cond.Operator == Operator.StartsWith)
                                {
                                    filterString += "'" + val + "%'";
                                }
                                else
                                {
                                    filterString += "'" + val + "'";
                                }
                            }
                        }
                        else
                        {
                            filterString += fieldsValueDictionary[cond.Field];

                            filterString += " " + Condition.GetNameOfOperatorForFilter(cond.Operator) + " ";

                            filterString += cond.Value.ToString();
                        }
                    }
                }
    
                dataTable.DefaultView.RowFilter = filterString;
            }

            resultDataTable = dataTable.DefaultView.ToTable();
        }

        private Field GetRealSortField(Field field)
        {
            // Sortierung immer nach "Speichern unter"
            if (field == Field.ArtistCDName)
                return Field.ArtistCDSaveAs;

            if (field == Field.ComposerCDName)
                return Field.ComposerCDSaveAs;

            return field;
        }

        private bool SortFieldByValue(DataBase db, Field field)
        {
            if (field == Field.ArchiveNumber || field == Field.NumberOfTracks ||
                field == Field.Date)
                return true;

            if (FieldHelper.IsUserField(field) && db.GetUserFieldType(field) == UserFieldType.Date)
                return true;

            return false;
        }

        private void AddField(DataBase db, Field field)
        {
            if (fieldsValueDictionary.ContainsKey(field))
                return;

            DataColumn dataColumn = new DataColumn();
            dataColumn.Caption = db.GetNameOfField(field);
            dataColumn.DataType = typeof(string);
            dataTable.Columns.Add(dataColumn);
            fieldsStringDictionary.Add(field, dataColumn.ColumnName);

            dataColumn = new DataColumn();
            dataColumn.Caption = db.GetNameOfField(field);
            if (field == Field.ArchiveNumber && Settings.Current.SortArchiveNumberNumeric)
                dataColumn.DataType = typeof(int);
            else
                dataColumn.DataType = DataBase.GetTypeByField(field);
            dataTable.Columns.Add(dataColumn);
            fieldsValueDictionary.Add(field, dataColumn.ColumnName);

            fieldsToRead.Add(field);
        }

        public string GetRowStringValue(int row, Field field)
        {
            string colName = fieldsStringDictionary[field];
            return resultDataTable.Rows[row][colName] is DBNull ? "" : (string)resultDataTable.Rows[row][colName];
        }

        public object GetRowRawValue(int row, Field field)
        {
            string colName = fieldsValueDictionary[field];

            if (resultDataTable.Rows[row][colName] is DBNull)
                return null;

            return resultDataTable.Rows[row][colName];
        }

        public int GetCDID(int row)
        {
            return (int)resultDataTable.Rows[row][CDIDColumnName];
        }

        public int GetArtistID(int row)
        {
            return (int)resultDataTable.Rows[row][ArtistIDColumnName];
        }

        public DataRowCollection Rows
        {
            get
            {
                return resultDataTable.Rows;
            }
        }

        public DataTable DataTable
        {
            get
            {
                return resultDataTable;
            }
        }
    }
}
