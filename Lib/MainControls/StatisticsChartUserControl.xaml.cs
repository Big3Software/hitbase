using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Big3.Hitbase.SharedResources;
using System.Windows.Controls.DataVisualization.Charting;
using System.Data;
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for StatisticsChartUserControl.xaml
    /// </summary>
    public partial class StatisticsChartUserControl : UserControl
    {
        public StatisticsChartUserControl()
        {
            InitializeComponent();
        }

        public StatisticType StatisticType { get; set; }

        public StatisticChartType StatisticChartType { get; set; }

        public DataBase DataBase { get; set; }

        private bool moreDetails = false;

        private bool isInitialized = false;

        List<ChartValue> currentChartValues = null;

        private List<ChartValue> StatisticCDGroupByCategoryCount()
        {
            return StatisticSqlGeneral("SELECT Count(Category.CategoryID) AS AnzahlvonIDKategorie, Category.Name " +
                                "FROM CD " +
                                "INNER JOIN Category ON Category.CategoryID = CD.CategoryID " +
                                "GROUP BY CD.CategoryID, Category.Name order by AnzahlvonIDKategorie DESC;");
        }

        private List<ChartValue> StatisticCDGroupByMediumCount()
        {
            return StatisticSqlGeneral("SELECT Count(Medium.MediumID) as MediumCount, Medium.Name " +
                                "FROM CD " +
                                "INNER JOIN Medium ON Medium.MediumID = CD.MediumID " +
                                "GROUP BY CD.MediumID, Medium.Name order by MediumCount DESC;");
        }


        private List<ChartValue> StatisticSqlGeneral(string sql, bool limitResults = true)
        {
            DataTable data = DataBase.ExecuteFreeSql(sql);

            if (data.Rows.Count < 1)
            {
                return null;
            }

            List<ChartValue> categories = new List<ChartValue>();

            int maxCount = moreDetails ? 10 : 5;
            int otherCount = 0;
            Int32 nValueX = 1;
            foreach (DataRow dr in data.Rows)
            {
                // Alle sonstigen Werte zusammenfassen, sonst kann man die Grafik nicht mehr erkennen
                if (nValueX <= maxCount || !limitResults)
                {
                    string FieldDescription = dr[1].ToString();

                    categories.Add(new ChartValue(FieldDescription,  (int)dr[0]));
                }
                else
                {
                    otherCount += (int)dr[0];
                }

                nValueX++;

            }

            if (otherCount > 0)
            {
                categories.Add(new ChartValue(StringTable.Other, otherCount));
            }

            return categories;
        }

        private List<ChartValue> StatisticCDGroupyPriceCount()
        {
            DataTable data = DataBase.ExecuteFreeSql("SELECT Count(1) As Anzahl, (PRICE / 500) As ColName " +
                                                        "FROM CD " +
                                                       "GROUP BY (PRICE / 500);");
            if (data.Rows.Count < 1)
            {
                return null;
            }

            List<ChartValue> categories = new List<ChartValue>();

            foreach (DataRow dr in data.Rows)
            {
                int priceValue = (int)dr[1];
                string FieldDescription = Misc.FormatCurrencyValue(priceValue * 500) + " bis " + Misc.FormatCurrencyValue((priceValue + 1) * 500 - 1);

                categories.Add(new ChartValue(FieldDescription, // + " (" + dr["Anzahl"].ToString() + ")", 
                                        (int)dr["Anzahl"]
                                    ));
            }

            return categories;
        }

        private List<ChartValue> StatisticCDGroupByNumberOfTracksCount()
        {
            return StatisticSqlGeneral("SELECT Count(1) As Anzahl, '0 - 5', '1' as SORTORDER " +
                                            "FROM CD " +
                                            "WHERE (CD.NumberOfTracks <= 5) " +

                                            "UNION " +

                                            "SELECT Count(1) As Anzahl, '6 - 10', '2' as SORTORDER " +
                                            "FROM CD " +
                                            "WHERE (CD.NumberOfTracks > 5 And CD.NumberOfTracks <= 10) " +

                                            "UNION " +

                                            "SELECT Count(1) As Anzahl, '11 - 15', '3' as SORTORDER " +
                                            "FROM CD " +
                                            "WHERE (CD.NumberOfTracks > 10 And CD.NumberOfTracks <= 15) " +

                                            "UNION " +

                                            "SELECT Count(1) As Anzahl, '16 - 20', '4' as SORTORDER " +
                                            "FROM CD " +
                                            "WHERE (CD.NumberOfTracks > 15 And CD.NumberOfTracks <= 20) " +

                                            "UNION " +

                                            "SELECT Count(1) As Anzahl, '> 20', '5' as SORTORDER " +
                                            "FROM CD " +
                                            "WHERE (CD.NumberOfTracks > 20) ");

        }

        private List<ChartValue> StatisticCDGroupByLengthCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, 'Länge <= 5 Min.' As Colname, 1 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length <= 300000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 5 Min. und <= 10 Min.' As Colname, 2 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 300000 And CD.Length <= 600000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 10 Min. und <= 15 Min.' As Colname, 3 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 600000 And CD.Length <= 900000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 15 Min. und <= 20 Min.' As Colname, 4 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 900000 And CD.Length <= 1200000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 20 Min. und <= 25 Min.' As Colname, 5 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 1200000 And CD.Length <= 1500000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 25 Min. und <= 30 Min.' As Colname, 6 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 1500000 And CD.Length <= 1800000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 30 Min. und <= 35 Min.' As Colname, 7 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 1800000 And CD.Length <= 2100000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 35 Min. und <= 40 Min.' As Colname, 8 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 2100000 And CD.Length <= 2400000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 40 Min. und <= 45 Min.' As Colname, 9 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 2400000 And CD.Length <= 2700000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 40 Min. und <= 45 Min.' As Colname, 10 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 2700000 And CD.Length <= 3000000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 45 Min. und <= 50 Min.' As Colname, 11 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 3000000 And CD.Length <= 3300000) " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Länge > 50 Min.' As Colname, 12 as SORTORDER " +
                                      "FROM CD " +
                                      "WHERE (CD.Length > 3300000 And CD.Length <= 3600000) " +
                                      "order by SORTORDER");
        }

        private List<ChartValue> StatisticCDGroupBySamplerCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, 'Sampler' as Beschreibung, 1 as SORTORDER " +
                "FROM cd " +
                "WHERE cd.IsSampler=1 " +

                "UNION " +

                "SELECT Count(1) As Anzahl, 'Kein Sampler' as Beschreibung, 2 as SORTORDER " +
                "FROM cd " +
                "WHERE cd.IsSampler=0 " +

                "order by SORTORDER");
        }

        private List<ChartValue> StatisticCDGroupByRatingCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, Rating " +
                                             "FROM cd " +
                                            "GROUP BY CD.Rating;");
        }

        private List<ChartValue> StatisticCDGroupByAttributeCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, CD.Codes " +
                                            "FROM CD " +
                                            "GROUP BY CD.Codes " +
                                            "HAVING CD.Codes <> '';");
        }

        private List<ChartValue> StatisticCDGroupByLabelCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, CD.Label As ColName " +
                                             "FROM CD " +
                                            "GROUP BY CD.Label " +
                                            " " +
                                          "HAVING CD.Label <> '' ORDER BY Anzahl desc;");
        }

        private List<ChartValue> StatisticCDGroupByRecordCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, CD.YearRecorded FROM CD Group by CD.YearRecorded Having CD.YearRecorded <> 0 Order by Anzahl Desc");
        }

        private List<ChartValue> StatisticCDGroupByArtistArtCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, 'Unbekannt' As ColName, 1 as SORTORDER " +
                              "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                              "WHERE PersonGroup.Type = 0 " +

                              "UNION " +

                              "SELECT Count(1) As Anzahl, 'Gruppe' As ColName, 2 as SORTORDER " +
                              "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                              "WHERE PersonGroup.Type = 1 " +

                              "UNION " +

                              "SELECT Count(1) As Anzahl, 'Einzelinterpret' As ColName, 3 as SORTORDER " +
                              "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                              "WHERE PersonGroup.Type = 2 " +

                              "UNION " +

                              "SELECT Count(1) As Anzahl, 'Orchester' As ColName, 4 as SORTORDER " +
                              "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                              "WHERE PersonGroup.Type = 3 " +

                              "UNION " +

                              "SELECT Count(1) As Anzahl, 'Duo/Duett' As ColName, 5 as SORTORDER " +
                              "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                              "WHERE PersonGroup.Type = 4 " +

                              "ORDER BY SORTORDER", false);
        }

        private List<ChartValue> StatisticCDGroupByArtistSexCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, 'Unbekannt' As ColName, 1 as SORTORDER " +
                                      "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                      "WHERE PersonGroup.Sex = 0 " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Weiblich' As ColName, 2 as SORTORDER " +
                                      "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                      "WHERE PersonGroup.Sex = 1 " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Männlich' As ColName, 3 as SORTORDER " +
                                      "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                      "WHERE PersonGroup.Sex = 2 " +

                                      "UNION " +

                                      "SELECT Count(1) As Anzahl, 'Gemischt' As ColName, 4 as SORTORDER " +
                                      "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                      "WHERE PersonGroup.Sex = 3 " +

                                      "ORDER BY SORTORDER", false);
        }

        private List<ChartValue> StatisticCDGroupByCountryCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) As Anzahl, 'Keine Landzuweisung' As ColName, 1 as SORTORDER " +
                                      "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                      "WHERE PersonGroup.Country = '' or PersonGroup.Country is null " +

                                      "UNION " +

                                      "SELECT Count(1), PersonGroup.Country As ColName, 2 as SORTORDER " +
                                      "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                      "GROUP BY PersonGroup.Country " +
                                      "HAVING PersonGroup.Country <> '' " +

                                      "ORDER BY SORTORDER");
        }

        private List<ChartValue> StatisticTrackGroupByLengthCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(Track.Length) As Anzahl, '> 0 Sek. und <= 30 Sek.' As ColName, 1 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 0 And Track.Length <= 30000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 30 Sek. und <= 1 Min.' As ColName, 2 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 30000 And Track.Length <= 60000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 1 Min. und <= 1:30 Min.' As ColName, 3 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 60000 And Track.Length <= 90000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 1:30 Min. und <= 2:00 Min.' As ColName, 4 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 90000 And Track.Length <= 120000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 2:00 Min. und <= 2:30 Min.' As ColName, 5 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 120000 And Track.Length <= 150000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 2:30 Min. und <= 3:00 Min.' As ColName, 6 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 150000 And Track.Length <= 180000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 3:00 Min. und <= 3:30 Min.' As ColName, 7 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 180000 And Track.Length <= 210000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 3:30 Min. und <= 4:00 Min.' As ColName, 8 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 210000 And Track.Length <= 240000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 4:00 Min. und <= 4:30 Min.' As ColName, 9 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 240000 And Track.Length <= 270000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 4:30 Min. und <= 5:00 Min.' As ColName, 10 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 270000 And Track.Length <= 300000) " +

                                 "UNION " +

                                 "SELECT Count(Track.Length) As Anzahl, '> 5:00 Min.' As ColName, 11 as SORTORDER " +
                                    "FROM Track " +
                                 "WHERE (Track.Length > 300000) " +

                                  "ORDER BY SORTORDER", false);
        }

        private List<ChartValue> StatisticTrackGropupByRecordCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, YearRecorded AS ColName " +
                                    "FROM Track " +
                                  "WHERE YearRecorded > 0 " +
                                  "GROUP BY YearRecorded ORDER BY Anzahl Desc;");
        }

        private List<ChartValue> StatisticTrackGroupByRating()
        {
            return this.StatisticSqlGeneral("SELECT count(1) As Anzahl, Rating As ColName " +
                                             "FROM Track " +
                                            "GROUP BY Rating order by Rating;", false);
        }

        private List<ChartValue> StatisticTrackGroupByBPMCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, (Bpm / 10) AS ColName " +
                                             "FROM Track " +
                                            "WHERE Bpm > 0 " +
                                            "GROUP BY (Bpm / 10);");
        }

        private List<ChartValue> StatisticTrackGroupByAttributeCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, Code.Letter As ColName " +
                                             "FROM Code, Track " +
                                            "WHERE (((Track.Codes) Like '*'+[Letter]+'*')) " +
                                            "GROUP BY Code.Letter;");
        }

        private List<ChartValue> StatisticArtistTrackMost()
        {
            return this.StatisticSqlGeneral("SELECT TOP (10) Count(1) AS Anzahl, PersonGroup.Name As ColName " +
                                            "FROM PersonGroup INNER JOIN Track ON PersonGroup.PersonGroupID = Track.ArtistID " +
                                            "GROUP BY PersonGroup.Name " +
                                            "HAVING (((PersonGroup.Name)<>'' And (PersonGroup.Name) Is Not Null)) " +
                                            "ORDER BY Anzahl DESC;");
        }

        private List<ChartValue> StatisticArtistTrackGroupByArtistArtCount()
        {
            List<ChartValue> chartValues = this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, PersonGroup.Type As ColName " +
                                             "FROM PersonGroup, Track " +
                                            "WHERE (((PersonGroup.PersonGroupID)=[Track].[ArtistID])) " +
                                            "GROUP BY PersonGroup.Type;", false);

            foreach (ChartValue chartValue in chartValues)
            {
                if (string.IsNullOrEmpty(chartValue.Name))
                {
                    chartValue.Name = StringTable.Undefined;
                }
                else
                {
                    chartValue.Name = DataBase.GetNameOfPersonGroupType((PersonGroupType)Misc.Atoi(chartValue.Name));
                }
            }

            return chartValues;
        }

        private List<ChartValue> StatisticArtistTrackGroupBySexCount()
        {
            List<ChartValue> chartValues = this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, PersonGroup.Sex as ColName " +
                                             "FROM PersonGroup, Track " +
                                            "WHERE (((PersonGroup.PersonGroupID)=[Track].[ArtistID])) " +
                                            "GROUP BY PersonGroup.Sex;", false);

            foreach (ChartValue chartValue in chartValues)
            {
                if (string.IsNullOrEmpty(chartValue.Name))
                {
                    chartValue.Name = StringTable.Undefined;
                }
                else
                {
                    chartValue.Name = DataBase.GetNameOfPersonGroupSex((SexType)Misc.Atoi(chartValue.Name));
                }
            }

            return chartValues;
        }

        private List<ChartValue> StatisticArtistTrackGroupByCountryCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, PersonGroup.Country As ColName " +
                                             "FROM PersonGroup, Track " +
                                            "WHERE (((PersonGroup.PersonGroupID)=[Track].[ArtistID])) AND PersonGroup.Country <> '' " +
                                            "GROUP BY PersonGroup.Country");
        }


        private List<ChartValue> StatisticArtistCDsMost()
        {
            return this.StatisticSqlGeneral("SELECT TOP (10) Count(1) AS Anzahl, PersonGroup.Name As ColName " +
                                             "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
                                            "GROUP BY PersonGroup.Name " +
                                          "HAVING (((PersonGroup.Name)<>'' And (PersonGroup.Name) Is Not Null)) " +
                                            "ORDER BY Anzahl DESC;");
        }


        private List<ChartValue> StatisticArtistGroupBySexCount()
        {
            List<ChartValue> chartValues = this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, PersonGroup.Sex As ColName " +
                                             "FROM PersonGroup " +
                                            "GROUP BY PersonGroup.Sex;", false);

            foreach (ChartValue chartValue in chartValues)
            {
                if (string.IsNullOrEmpty(chartValue.Name))
                {
                    chartValue.Name = StringTable.Undefined;
                }
                else
                {
                    chartValue.Name = DataBase.GetNameOfPersonGroupSex((SexType)Misc.Atoi(chartValue.Name));
                }
            }

            return chartValues;
        }

        private List<ChartValue> StatisticArtistGroupByCountryCount()
        {
            return this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, PersonGroup.Country As ColName " +
                                             "FROM PersonGroup " +
                                            "WHERE PersonGroup.Country <> '' " +
                                            "GROUP BY PersonGroup.Country;");
        }

        private List<ChartValue> StatisticArtistGroupByArtistArtCount()
        {
            List<ChartValue> chartValues = this.StatisticSqlGeneral("SELECT Count(1) AS Anzahl, PersonGroup.Type As ColName " +
                                             "FROM PersonGroup " +
                                            "GROUP BY PersonGroup.Type;", false);

            foreach (ChartValue chartValue in chartValues)
            {
                if (string.IsNullOrEmpty(chartValue.Name))
                {
                    chartValue.Name = StringTable.Undefined;
                }
                else
                {
                    chartValue.Name = DataBase.GetNameOfPersonGroupType((PersonGroupType)Misc.Atoi(chartValue.Name));
                }
            }

            return chartValues;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                //CreateStatistic();
            }
        }

        internal void CreateStatistic(bool moreDetails = false)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                List<ChartValue> values = null;

                this.moreDetails = moreDetails;

                switch (StatisticType)
                {
                    case StatisticType.CDGroupByCategoryCount:
                        values = StatisticCDGroupByCategoryCount();
                        break;
                    case StatisticType.CDGroupByMediumCount:
                        values = StatisticCDGroupByMediumCount();
                        break;
                    case StatisticType.CDGroupByPriceCount:
                        values = StatisticCDGroupyPriceCount();
                        break;
                    case StatisticType.CDGroupByNumberOfTracksCount:
                        values = StatisticCDGroupByNumberOfTracksCount();
                        break;
                    case StatisticType.CDGroupByLengthCount:
                        values = StatisticCDGroupByLengthCount();
                        break;
                    case StatisticType.CDGroupBySamplerCount:
                        values = StatisticCDGroupBySamplerCount();
                        break;
                    case StatisticType.CDGroupByRatingCount:
                        values = StatisticCDGroupByRatingCount();
                        break;
                    case StatisticType.CDGroupByAttributeCount:
                        values = StatisticCDGroupByAttributeCount();
                        break;
                    case StatisticType.CDGroupByLabelCount:
                        values = StatisticCDGroupByLabelCount();
                        break;
                    case StatisticType.CDGroupByRecordCount:
                        values = StatisticCDGroupByRecordCount();
                        break;
                    case StatisticType.CDGroupByArtistArtCount:
                        values = StatisticCDGroupByArtistArtCount();
                        break;
                    case StatisticType.CDGroupByArtistSexCount:
                        values = StatisticCDGroupByArtistSexCount();
                        break;
                    case StatisticType.CDGroupByCountryCount:
                        values = StatisticCDGroupByCountryCount();
                        break;
                    case StatisticType.TrackGroupByLengthCount:
                        values = StatisticTrackGroupByLengthCount();
                        break;
                    case StatisticType.TrackGroupByRecordCount:
                        values = StatisticTrackGropupByRecordCount();
                        break;
                    case StatisticType.TrackGroupByRating:
                        values = StatisticTrackGroupByRating();
                        break;
                    case StatisticType.TrackGroupByBPMCount:
                        values = StatisticTrackGroupByBPMCount();
                        break;
                    case StatisticType.TrackGroupByAttributeCount:
                        values = StatisticTrackGroupByAttributeCount();
                        break;
                    case StatisticType.ArtistTrackMost:
                        values = StatisticArtistTrackMost();
                        break;
                    case StatisticType.ArtistTrackGroupBySexCount:
                        values = StatisticArtistTrackGroupBySexCount();
                        break;
                    case StatisticType.ArtistTrackGroupByCountryCount:
                        values = StatisticArtistTrackGroupByCountryCount();
                        break;
                    case StatisticType.ArtistTrackGroupByArtistArtCount:
                        values = StatisticArtistTrackGroupByArtistArtCount();
                        break;
                    case StatisticType.ArtistCDsMost:
                        values = StatisticArtistCDsMost();
                        break;
                    case StatisticType.ArtistGroupBySexCount:
                        values = StatisticArtistGroupBySexCount();
                        break;
                    case StatisticType.ArtistGroupByCountryCount:
                        values = StatisticArtistGroupByCountryCount();
                        break;
                    case StatisticType.ArtistGroupByArtistArtCount:
                        values = StatisticArtistGroupByArtistArtCount();
                        break;
                    default:
                        break;
                }

                e.Result = values;
            };
            bw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                currentChartValues = (List<ChartValue>)e.Result ;

                SetChartValues(currentChartValues);
            };
            bw.RunWorkerAsync();
        }

        internal void UpdateChart()
        {
            SetChartValues(currentChartValues);
        }

        private void SetChartValues(List<ChartValue> values)
        {
            DataPointSeries series = null;

            switch (StatisticChartType)
            {
                case MainControls.StatisticChartType.Pie:
                    {
                        series = new PieSeries();
                        MyChart.ClearValue(Chart.PaletteProperty);
                        MyChart.Template = FindResource("PieChartTemplate") as ControlTemplate;
                        break;
                    }
                case MainControls.StatisticChartType.Bar:
                    {
                        BarSeries barSeries = new BarSeries();
                        barSeries.DependentRangeAxis = new LinearAxis() { Orientation = AxisOrientation.X, Minimum = 0, ShowGridLines = true };

                        CategoryAxis categoryAxis = new CategoryAxis() { Orientation = AxisOrientation.Y, Margin = new Thickness(0, 0, 10, 0) };
                        categoryAxis.MajorTickMarkStyle = new System.Windows.Style();

                        MyChart.Palette = FindResource("MyPalette") as Collection<ResourceDictionary>;

                        MyChart.Template = FindResource("MyChartTemplate") as ControlTemplate;

                        barSeries.IndependentAxis = categoryAxis;

                        series = barSeries;

                        break;
                    }
                case MainControls.StatisticChartType.Column:
                    {
                        ColumnSeries colSeries = new ColumnSeries();
                        colSeries.DependentRangeAxis = new LinearAxis() { Orientation = AxisOrientation.Y, Minimum = 0, ShowGridLines = true };
                        MyChart.Palette = FindResource("MyPalette") as Collection<ResourceDictionary>;
                        MyChart.Template = FindResource("MyChartTemplate") as ControlTemplate;
                        series = colSeries;
                        break;
                    }

                case MainControls.StatisticChartType.Line:
                    {
                        LineSeries lineSeries = new LineSeries();
                        lineSeries.DependentRangeAxis = new LinearAxis() { Orientation = AxisOrientation.Y, Minimum = 0, ShowGridLines = true };
                        MyChart.Palette = FindResource("MyPalette") as Collection<ResourceDictionary>;
                        MyChart.Template = FindResource("MyChartTemplate") as ControlTemplate;
                        series = lineSeries;
                        break;
                    }
            }

            series.AnimationSequence = AnimationSequence.FirstToLast;

            series.IndependentValueBinding = new Binding("Name");
            series.DependentValueBinding = new Binding("Count");

            series.ItemsSource = values;

            MyChart.Series.Clear();
            MyChart.Series.Add(series);
        }


    }

    public class ChartValue
    {
        public ChartValue(string name, int count)
        {
            this.Name = name;
            this.Count = count;
        }

        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class ChartCommands
    {
        public static RoutedCommand ZoomChart = new RoutedCommand("ZoomChart", typeof(ChartCommands));

        public static RoutedCommand Overview = new RoutedCommand("Overview", typeof(ChartCommands));

        public static RoutedCommand PieChart = new RoutedCommand("PieChart", typeof(ChartCommands));
        public static RoutedCommand ColumnChart = new RoutedCommand("ColumnChart", typeof(ChartCommands));
        public static RoutedCommand BarChart = new RoutedCommand("BarChart", typeof(ChartCommands));
        public static RoutedCommand LineChart = new RoutedCommand("LineChart", typeof(ChartCommands));

        public static RoutedCommand CDGroupByCategoryCount = new RoutedCommand("CDGroupByCategoryCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByMediumCount = new RoutedCommand("CDGroupByMediumCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByPriceCount = new RoutedCommand("CDGroupByPriceCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByNumberOfTracksCount = new RoutedCommand("CDGroupByNumberOfTracksCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByLengthCount = new RoutedCommand("CDGroupByLengthCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupBySamplerCount = new RoutedCommand("CDGroupBySamplerCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByRatingCount = new RoutedCommand("CDGroupByRatingCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByAttributeCount = new RoutedCommand("CDGroupByAttributeCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByLabelCount = new RoutedCommand("CDGroupByLabelCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByRecordCount = new RoutedCommand("CDGroupByRecordCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByArtistArtCount = new RoutedCommand("CDGroupByArtistArtCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByArtistSexCount = new RoutedCommand("CDGroupByArtistSexCount", typeof(ChartCommands));
        public static RoutedCommand CDGroupByCountryCount = new RoutedCommand("CDGroupByCountryCount", typeof(ChartCommands));

        public static RoutedCommand TrackGroupByLengthCount = new RoutedCommand("TrackGroupByLengthCount", typeof(ChartCommands));
        public static RoutedCommand TrackGroupByRecordCount = new RoutedCommand("TrackGroupByRecordCount", typeof(ChartCommands));
        public static RoutedCommand TrackGroupByRating = new RoutedCommand("TrackGroupByRating", typeof(ChartCommands));
        public static RoutedCommand TrackGroupByBPMCount = new RoutedCommand("TrackGroupByBPMCount", typeof(ChartCommands));
        public static RoutedCommand TrackGroupByAttributeCount = new RoutedCommand("TrackGroupByAttributeCount", typeof(ChartCommands));

        public static RoutedCommand ArtistTrackMost = new RoutedCommand("ArtistTrackMost", typeof(ChartCommands));
        public static RoutedCommand ArtistTrackGroupBySexCount = new RoutedCommand("ArtistTrackGroupBySexCount", typeof(ChartCommands));
        public static RoutedCommand ArtistTrackGroupByCountryCount = new RoutedCommand("ArtistTrackGroupByCountryCount", typeof(ChartCommands));
        public static RoutedCommand ArtistTrackGroupByArtistArtCount = new RoutedCommand("ArtistTrackGroupByArtistArtCount", typeof(ChartCommands));
        public static RoutedCommand ArtistCDsMost = new RoutedCommand("ArtistCDsMost", typeof(ChartCommands));
        public static RoutedCommand ArtistGroupBySexCount = new RoutedCommand("ArtistGroupBySexCount", typeof(ChartCommands));
        public static RoutedCommand ArtistGroupByCountryCount = new RoutedCommand("ArtistGroupByCountryCount", typeof(ChartCommands));
        public static RoutedCommand ArtistGroupByArtistArtCount = new RoutedCommand("ArtistGroupBySexCount", typeof(ChartCommands));
    }

    public enum StatisticChartType
    {
        None,
        Pie,
        Bar,
        Column,
        Line
    }
}
