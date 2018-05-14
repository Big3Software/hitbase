using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.DataBaseEngine
{
    public class AlbumView
    {
        public static DataBaseView CreateView(DataBase db, FieldCollection fc, SortFieldCollection sfc, int cdid = 0, Condition condition = null)
        {
            string allFields = GetColumns(db, fc);
            string sortFields = GetSortColumns(db, sfc);

            bool containsArtistField = fc.Any(field => FieldHelper.IsCDArtistField(field));
            bool containsComposerField = fc.Any(field => FieldHelper.IsCDComposerField(field));
            bool containsCategoryField = fc.Any(field => field == Field.Category);
            bool containsMediumField = fc.Any(field => field == Field.Medium);
            bool containsCDSetField = fc.Any(field => field == Field.CDSet);

            if (condition != null)
            {
                containsArtistField |= condition.Any(singleCond => FieldHelper.IsCDArtistField(singleCond.Field));
                containsComposerField |= condition.Any(singleCond => FieldHelper.IsCDComposerField(singleCond.Field));
                containsCategoryField |= condition.Any(singleCond => singleCond.Field == Field.Category);
                containsMediumField |= condition.Any(singleCond => singleCond.Field == Field.Medium);
                containsCDSetField |= condition.Any(singleCond => singleCond.Field == Field.CDSet);
            }

            string sql;

            if (!string.IsNullOrEmpty(allFields))
                sql = "SELECT CDID, " + allFields + " FROM CD ";
            else
                sql = "SELECT CDID FROM CD";

            if (containsArtistField)
                sql += "INNER JOIN PersonGroup as Artist ON CD.ArtistID = Artist.PersonGroupID ";
            if (containsComposerField)
                sql += "LEFT JOIN PersonGroup as Composer ON CD.ComposerID = Composer.PersonGroupID ";
            if (containsCDSetField)
                sql += "LEFT JOIN [Set] ON CD.SetID = [Set].SetID ";
            if (containsCategoryField)
                 sql += "LEFT JOIN Category ON CD.CategoryID = Category.CategoryID ";
            if (containsMediumField)
                sql += "LEFT JOIN Medium ON CD.MediumID = Medium.MediumID ";

            if (cdid != 0)
                sql += " WHERE CD.CDID=" + cdid.ToString();

            if (condition != null && condition.Count > 0)
            {
                string where = GetSqlCondition(db, condition);
                if (!string.IsNullOrEmpty(where))
                {
                    sql += " WHERE ";
                    sql += where;
                }
            }

            if (!string.IsNullOrEmpty(sortFields))
            {
                sql += " ORDER BY " + sortFields;
            }

            DataBaseView view = DataBaseView.Create(db, sql);

            return view;
        }

        private static string GetSqlCondition(DataBase db, Condition condition)
        {
            string stringCondition = "";

            int index = 0;
            Logical lastLogical = Logical.None;

            foreach (SingleCondition singleCondition in condition)
            {
                if (singleCondition.Field == Field.None)
                    continue;

                string columnField = GetColumn(db, singleCondition.Field);

                string opName = Condition.GetNameOfOperatorForFilter(singleCondition.Operator);

                string singleCondString = columnField + " " + opName + " ";

                string val = "";

                if (singleCondition.Value != null)
                {
                    val = singleCondition.Value.ToString();

                    if (singleCondition.Field == Field.Date)
                    {
                        val = db.ParseDate(val);
                    }
                }
                else
                {
                    if (DataBase.GetTypeByField(singleCondition.Field) == typeof(int))
                    {
                        val = "0";
                    }
                }

                bool conditionValid = false;

                if (singleCondition.Operator != Operator.NotEmpty && singleCondition.Operator != Operator.Empty)
                {
                    if (DataBase.GetTypeByField(singleCondition.Field) == typeof(string))
                    {
                        val = val.Replace("'", "''");
                        if (singleCondition.Operator == Operator.Contains || singleCondition.Operator == Operator.NotContains)
                        {
                            singleCondString += "'%" + val + "%'";
                        }
                        else
                        {
                            if (singleCondition.Operator == Operator.StartsWith)
                            {
                                singleCondString += "'" + val + "%'";
                            }
                            else
                            {
                                singleCondString += "'" + val + "'";
                            }
                        }

                        conditionValid = true;
                    }
                    else
                    {
                        if (singleCondition.Value != null)
                        {
                            if (DataBase.GetTypeByField(singleCondition.Field) == typeof(int))
                            {
                                if (singleCondition.Value is string && singleCondition.Field == Field.TotalLength)
                                {
                                    singleCondString += Misc.ParseTimeString(val).ToString();
                                }
                                else
                                {
                                    singleCondString += Misc.Atoi(singleCondition.Value.ToString()).ToString();
                                }

                                conditionValid = true;
                            }
                            else if (DataBase.GetTypeByField(singleCondition.Field) == typeof(bool))
                            {
                                int intValue = 0;
                                if (Int32.TryParse(val, out intValue))
                                {
                                    // Bei bool-Feldern nur nach 0 und 1 suchen können
                                    if (intValue == 0 || intValue == 1)
                                    {
                                        singleCondString += intValue.ToString();
                                        conditionValid = true;
                                    }
                                }
                            }
                            else
                            {
                                singleCondString += singleCondition.Value.ToString();
                                conditionValid = true;
                            }
                        }
                    }
                }
                else
                {
                    if (singleCondition.Operator == Operator.Empty)
                    {
                        singleCondString = string.Format("({0} IS NULL OR {0}='')", columnField);
                        conditionValid = true;
                    }
                    if (singleCondition.Operator == Operator.NotEmpty)
                    {
                        singleCondString = string.Format("({0} IS NOT NULL AND {0}<>'')", columnField);
                        conditionValid = true;
                    }
                }

                if (conditionValid)
                {
                    if (index > 0)
                    {
                        if (lastLogical == Logical.Or)
                            stringCondition += " OR ";
                        else
                            stringCondition += " AND ";
                    }

                    stringCondition += singleCondString;
                }

                lastLogical = singleCondition.Logical;

                index++;
            }

            return stringCondition;
        }

        private static string GetColumns(DataBase db, FieldCollection fc)
        {
            string allFields = "";

            foreach (Field field in fc)
            {
                if (!string.IsNullOrEmpty(allFields))
                    allFields += ", ";

                string columnField = GetColumn(db, field);

                allFields += columnField;
            }

            return allFields;
        }

        private static string GetSortColumns(DataBase db, SortFieldCollection sfc)
        {
            string allFields = "";

            foreach (SortField field in sfc)
            {
                if (!string.IsNullOrEmpty(allFields))
                    allFields += ", ";

                string columnField = GetColumn(db, field.Field);

                if (field.SortDirection == SortDirection.Descending)
                    columnField += " DESC";

                allFields += columnField;
            }

            return allFields;
        }

        private static string GetColumn(DataBase db, Field field)
        {
            string columnField = "";

            string colName = DataBase.GetDataColumnByField(field).ColumnName;

            if (field == Field.Identity)
                colName = "[Identity]";

            string prefix = "CD";

            if (FieldHelper.IsCDArtistField(field))
            {
                prefix = "Artist";
            }

            if (FieldHelper.IsCDComposerField(field))
            {
                prefix = "Composer";
            }

            if (field == Field.CDSet)
                prefix = "[Set]";

            if (field == Field.Category)
                prefix = "Category";

            if (field == Field.Medium)
                prefix = "Medium";

            if (!string.IsNullOrEmpty(prefix))
                columnField += prefix + "." + colName;
            else
                columnField += colName;

            return columnField;
        }
    }
}
