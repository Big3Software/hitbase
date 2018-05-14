using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.DataBaseEngine
{
    public class TrackView
    {
        public static DataBaseView CreateView(DataBase db, FieldCollection fc, SortFieldCollection sfc, int trackid = 0, Condition condition = null, int skip = 0, int take = 0)
        {
            string allFields = GetColumns(db, fc);
            string sortFields = GetSortColumns(db, sfc);

            bool containsArtistField = fc.Any(field => FieldHelper.IsCDArtistField(field)) || sfc.Any(field => FieldHelper.IsCDArtistField(field.Field));
            bool containsComposerField = fc.Any(field => FieldHelper.IsCDComposerField(field)) || sfc.Any(field => FieldHelper.IsCDComposerField(field.Field));
            bool containsCategoryField = fc.Any(field => field == Field.Category) || sfc.Any(field => field.Field == Field.Category);
            bool containsMediumField = fc.Any(field => field == Field.Medium) || sfc.Any(field => field.Field == Field.Medium);
            bool containsCDSetField = fc.Any(field => field == Field.CDSet) || sfc.Any(field => field.Field == Field.CDSet);
            bool containsTrackArtistField = fc.Any(field => FieldHelper.IsTrackArtistField(field)) || sfc.Any(field => FieldHelper.IsTrackArtistField(field.Field));
            bool containsTrackComposerField = fc.Any(field => FieldHelper.IsTrackComposerField(field)) || sfc.Any(field => FieldHelper.IsTrackComposerField(field.Field));
            bool containsTrackCategoryField = fc.Any(field => field == Field.TrackCategory) || sfc.Any(field => field.Field == Field.TrackCategory);
            bool containsCDField = fc.Any(field => FieldHelper.IsCDField(field, true)) || sfc.Any(field => FieldHelper.IsCDField(field.Field, true));

            if (condition != null)
            {
                containsArtistField |= condition.Any(singleCond => FieldHelper.IsCDArtistField(singleCond.Field));
                containsComposerField |= condition.Any(singleCond => FieldHelper.IsCDComposerField(singleCond.Field));
                containsCategoryField |= condition.Any(singleCond => singleCond.Field == Field.Category);
                containsMediumField |= condition.Any(singleCond => singleCond.Field == Field.Medium);
                containsCDSetField |= condition.Any(singleCond => singleCond.Field == Field.CDSet);
                containsTrackArtistField |= condition.Any(singleCond => FieldHelper.IsTrackArtistField(singleCond.Field));
                containsTrackComposerField |= condition.Any(singleCond => FieldHelper.IsTrackComposerField(singleCond.Field));
                containsTrackCategoryField |= condition.Any(singleCond => singleCond.Field == Field.TrackCategory);
                containsCDField |= condition.Any(singleCond => FieldHelper.IsCDField(singleCond.Field, true));
            }

            string sql;

            if (!string.IsNullOrEmpty(allFields))
                sql = "SELECT TrackID, " + allFields + " FROM Track ";
            else
                sql = "SELECT TrackID FROM Track";

            if (containsCDField)
                sql += "INNER JOIN CD ON Track.CDID = CD.CDID ";
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
            if (containsTrackArtistField)
                sql += "INNER JOIN PersonGroup as TrackArtist ON Track.ArtistID = TrackArtist.PersonGroupID ";
            if (containsTrackComposerField)
                sql += "LEFT JOIN PersonGroup as TrackComposer ON Track.ComposerID = TrackComposer.PersonGroupID ";
            if (containsTrackCategoryField)
                sql += "LEFT JOIN Category as TrackCategory ON Track.CategoryID = TrackCategory.CategoryID ";

            if (trackid != 0)
                sql += " WHERE Track.TrackID=" + trackid.ToString();

            if (condition != null)
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

            //sql += " OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY";

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

                bool conditionValid = false;

                if (singleCondition.Operator != Operator.NotEmpty && singleCondition.Operator != Operator.Empty)
                {
                    if (DataBase.GetTypeByField(singleCondition.Field) == typeof(string))
                    {
                        string val = "";

                        if (singleCondition.Value != null)
                        {
                            val = singleCondition.Value.ToString();
                        
                            if (singleCondition.Field == Field.Date)
                            {
                                val = db.ParseDate(val);
                            }
                        }

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
                                if (singleCondition.Value is string && (singleCondition.Field == Field.TotalLength || singleCondition.Field == Field.TrackLength))
                                {
                                    singleCondString += Misc.ParseTimeString(singleCondition.Value.ToString()).ToString();
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
                                if (Int32.TryParse(singleCondition.Value.ToString(), out intValue))
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

        private static string GetSortColumns(DataBase db, SortFieldCollection fc)
        {
            string allFields = "";

            foreach (SortField field in fc)
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

            string prefix = "Track";

            if (FieldHelper.IsCDField(field, true))
            {
                prefix = "CD";
            }

            if (FieldHelper.IsCDArtistField(field))
            {
                prefix = "Artist";
            }

            if (FieldHelper.IsCDComposerField(field))
            {
                prefix = "Composer";
            }

            if (FieldHelper.IsTrackArtistField(field))
            {
                prefix = "TrackArtist";
            }

            if (FieldHelper.IsTrackComposerField(field))
            {
                prefix = "TrackComposer";
            }

            if (field == Field.CDSet)
                prefix = "[Set]";

            if (field == Field.Category)
                prefix = "Category";

            if (field == Field.TrackCategory)
                prefix = "TrackCategory";

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
