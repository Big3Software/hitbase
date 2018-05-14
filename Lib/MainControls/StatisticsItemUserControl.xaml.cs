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
using Big3.Hitbase.DataBaseEngine;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for StatisticsItemUserControl.xaml
    /// </summary>
    public partial class StatisticsItemUserControl : UserControl
    {
        UserControl chartUserControl1;
        UserControl chartUserControl2;

        private int currentChart = 0;

        public StatisticType StatisticType { get; set; }

        public StatisticChartType StatisticChartType { get; set; }

        public DataBase DataBase { get; set; }

        public bool IsZoomed { get; set; }

        private bool isInitialized = false;

        private UserControl currentChartUserControl
        {
            get
            {
                if (currentChart == 0)
                    return chartUserControl1;
                else
                    return chartUserControl2;
            }
        }

        public StatisticsItemUserControl()
        {
            InitializeComponent();

        }

        private string GetStatisticsTitle()
        {
            string title = "";

            switch (StatisticType)
            {
                case StatisticType.Overview: title = StringTable.TEXT_OVERVIEW; break;
                case StatisticType.CDGroupByCategoryCount: title = StringTable.TEXT_STATISTICS_GRAF_1; break;
                case StatisticType.CDGroupByMediumCount: title = StringTable.TEXT_STATISTICS_GRAF_2; break;
                case StatisticType.CDGroupByPriceCount: title = StringTable.TEXT_STATISTICS_GRAF_3; break;
                case StatisticType.CDGroupByNumberOfTracksCount: title = StringTable.TEXT_STATISTICS_GRAF_4; break;
                case StatisticType.CDGroupByLengthCount: title = StringTable.TEXT_STATISTICS_GRAF_5; break;
                case StatisticType.CDGroupBySamplerCount: title = StringTable.TEXT_STATISTICS_GRAF_6; break;
                case StatisticType.CDGroupByRatingCount: title = StringTable.TEXT_STATISTICS_GRAF_7; break;
                case StatisticType.CDGroupByAttributeCount: title = StringTable.TEXT_STATISTICS_GRAF_8; break;
                case StatisticType.CDGroupByLabelCount: title = StringTable.TEXT_STATISTICS_GRAF_9; break;
                case StatisticType.CDGroupByRecordCount: title = StringTable.TEXT_STATISTICS_GRAF_10; break;
                case StatisticType.CDGroupByArtistArtCount: title = StringTable.TEXT_STATISTICS_GRAF_11; break;
                case StatisticType.CDGroupByArtistSexCount: title = StringTable.TEXT_STATISTICS_GRAF_12; break;
                case StatisticType.CDGroupByCountryCount: title = StringTable.TEXT_STATISTICS_GRAF_13; break;
                case StatisticType.TrackGroupByLengthCount: title = StringTable.TEXT_STATISTICS_GRAF_14; break;
                case StatisticType.TrackGroupByRecordCount: title = StringTable.TEXT_STATISTICS_GRAF_15; break;
                case StatisticType.TrackGroupByRating: title = StringTable.TEXT_STATISTICS_GRAF_16; break;
                case StatisticType.TrackGroupByBPMCount: title = StringTable.TEXT_STATISTICS_GRAF_17; break;
                case StatisticType.TrackGroupByAttributeCount: title = StringTable.TEXT_STATISTICS_GRAF_18; break;
                case StatisticType.ArtistTrackMost: title = StringTable.TEXT_STATISTICS_GRAF_19; break;
                case StatisticType.ArtistTrackGroupBySexCount: title = StringTable.TEXT_STATISTICS_GRAF_20; break;
                case StatisticType.ArtistTrackGroupByCountryCount: title = StringTable.TEXT_STATISTICS_GRAF_21; break;
                case StatisticType.ArtistTrackGroupByArtistArtCount: title = StringTable.TEXT_STATISTICS_GRAF_22; break;
                case StatisticType.ArtistCDsMost: title = StringTable.TEXT_STATISTICS_GRAF_23; break;
                case StatisticType.ArtistGroupBySexCount: title = StringTable.TEXT_STATISTICS_GRAF_24; break;
                case StatisticType.ArtistGroupByCountryCount: title = StringTable.TEXT_STATISTICS_GRAF_25; break;
                case StatisticType.ArtistGroupByArtistArtCount: title = StringTable.TEXT_STATISTICS_GRAF_26; break;
            }

            return title;
        }

        private void ChangeStatistic(MainControls.StatisticType statisticType)
        {
            StatisticType = statisticType;

            UserControl newUserControl = CreateStatistic();

            if (currentChart == 0)
            {
                currentChart = 1;
                chartUserControl2 = newUserControl;
                ChartTransitionBox.Content = chartUserControl2;
            }
            else
            {
                currentChart = 0;
                chartUserControl1 = newUserControl;
                ChartTransitionBox.Content = chartUserControl1;
            }

        }

        private UserControl CreateStatistic()
        {
            UserControl newUserControl = null;
            
            if (StatisticType == MainControls.StatisticType.Overview)
            {
                newUserControl = new StatisticsOverviewUserControl();
                ((StatisticsOverviewUserControl)newUserControl).DataBase = DataBase;
                splitButtonSelectChartType.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                newUserControl = new StatisticsChartUserControl();

                ((StatisticsChartUserControl)newUserControl).StatisticType = StatisticType;
                ((StatisticsChartUserControl)newUserControl).StatisticChartType = (StatisticChartType == MainControls.StatisticChartType.None) ? StatisticChartType.Bar : StatisticChartType;
                ((StatisticsChartUserControl)newUserControl).DataBase = DataBase;

                splitButtonSelectChartType.Visibility = System.Windows.Visibility.Visible;

                ((StatisticsChartUserControl)newUserControl).CreateStatistic();
            }

            splitButtonSelectStatistic.Text = GetStatisticsTitle();

            return newUserControl;
        }

        private void CommandBindingOverview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.Overview);
        }

        private void CommandBindingCDGroupByCategoryCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByCategoryCount);
        }

        private void CommandBindingCDGroupByMediumCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByMediumCount);
        }

        private void CommandBindingCDGroupByPriceCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByPriceCount);
        }

        private void CommandBindingCDGroupByNumberOfTracksCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByNumberOfTracksCount);
        }

        private void ButtonZoom_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Image img = VisualTreeExtensions.FindVisualChildByName<Image>(btn, "ImageZoom");

            if (!IsZoomed)
            {
                img.Source = ImageLoader.FromResource("ZoomOut.png");
                ButtonZoom.ToolTip = StringTable.ZoomOutChart;
            }
            else
            {
                img.Source = ImageLoader.FromResource("ZoomIn.png");
                ButtonZoom.ToolTip = StringTable.ZoomInChart;
            }

            ChartCommands.ZoomChart.Execute(this, this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                chartUserControl1 = CreateStatistic();

                ChartTransitionBox.Content = chartUserControl1;
            }
        }

        private void CommandBindingZoomChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatisticsChartUserControl statUserControl = ChartTransitionBox.Content as StatisticsChartUserControl;
            if (statUserControl != null)
            {
                if (IsZoomed)
                {
                    statUserControl.CreateStatistic(false);
                }
                else
                {
                    statUserControl.CreateStatistic(true);
                }

            }

            ChartCommands.ZoomChart.Execute(this, (IInputElement)this.Parent);
        }

        private void CommandBindingPieChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetChartType(StatisticChartType.Pie);
        }

        private void CommandBindingBarChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetChartType(StatisticChartType.Bar);
        }

        private void CommandBindingColumnChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetChartType(StatisticChartType.Column);
        }

        private void CommandBindingLineChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetChartType(StatisticChartType.Line);
        }

        private void SetChartType(StatisticChartType statisticChartType)
        {
            StatisticsChartUserControl statUserControl = ChartTransitionBox.Content as StatisticsChartUserControl;

            StatisticChartType = statisticChartType;
            statUserControl.StatisticChartType = statisticChartType;
            statUserControl.UpdateChart();

            switch (StatisticChartType)
            {
                case StatisticChartType.Pie:
                    splitButtonSelectChartType.Image = ImageLoader.FromResource("StatisticsPie.png");
                    break;
                case StatisticChartType.Bar:
                    splitButtonSelectChartType.Image = ImageLoader.FromResource("StatisticsBar.png");
                    break;
                case StatisticChartType.Column:
                    splitButtonSelectChartType.Image = ImageLoader.FromResource("Statistics.png");
                    break;
                case StatisticChartType.Line:
                    splitButtonSelectChartType.Image = ImageLoader.FromResource("StatisticsLine.png");
                    break;
                default:
                    break;
            }
        }

        private void splitButtonSelectChartType_Click(object sender, RoutedEventArgs e)
        {
            Array chartTypeEnums = Enum.GetValues(typeof(StatisticChartType));

            if ((int)StatisticChartType < chartTypeEnums.Length - 1)
            {
                SetChartType(StatisticChartType + 1);
            }
            else
            {
                SetChartType((StatisticChartType)1);
            }
        }

        private void CommandBindingCDGroupByLengthCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByLengthCount);
        }

        private void CommandBindingCDGroupBySamplerCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupBySamplerCount);
        }

        private void CommandBindingCDGroupByRatingCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByRatingCount);
        }

        private void CommandBindingCDGroupByAttributeCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByAttributeCount);
        }

        private void CommandBindingCDGroupByLabelCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByLabelCount);
        }

        private void CommandBindingCDGroupByRecordCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByRecordCount);
        }

        private void CommandBindingCDGroupByArtistArtCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByArtistArtCount);
        }

        private void CommandBindingCDGroupByArtistSexCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByArtistSexCount);
        }

        private void CommandBindingCDGroupByCountryCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.CDGroupByCountryCount);
        }

        private void CommandBindingTrackGroupByLengthCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.TrackGroupByLengthCount);
        }

        private void CommandBindingTrackGroupByRecordCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.TrackGroupByRecordCount);
        }

        private void CommandBindingTrackGroupByRating_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.TrackGroupByRating);
        }

        private void CommandBindingTrackGroupByBPMCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.TrackGroupByBPMCount);
        }

        private void CommandBindingTrackGroupByAttributeCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.TrackGroupByAttributeCount);
        }
        
        private void CommandBindingArtistTrackMost_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistTrackMost);
        }

        private void CommandBindingArtistTrackGroupBySexCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistTrackGroupBySexCount);
        }

        private void CommandBindingArtistTrackGroupByCountryCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistTrackGroupByCountryCount);
        }

        private void CommandBindingArtistTrackGroupByArtistArtCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistTrackGroupByArtistArtCount);
        }

        private void CommandBindingArtistCDsMost_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistCDsMost);
        }

        private void CommandBindingArtistGroupBySexCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistGroupBySexCount);
        }

        private void CommandBindingArtistGroupByCountryCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistGroupByCountryCount);
        }

        private void CommandBindingArtistGroupByArtistArtCount_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeStatistic(StatisticType.ArtistGroupByArtistArtCount);
        }
    }
}
