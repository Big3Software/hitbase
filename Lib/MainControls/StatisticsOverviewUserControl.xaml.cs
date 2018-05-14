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
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Miscellaneous;

namespace Big3.Hitbase.MainControls
{
    /// <summary>
    /// Interaction logic for StatisticsOverviewUserControl.xaml
    /// </summary>
    public partial class StatisticsOverviewUserControl : UserControl
    {
        public StatisticsOverviewUserControl()
        {
            InitializeComponent();
        }

        private bool isInitialized = false;

        public DataBase DataBase { get; set; }

        private ObservableCollection<StatisticsItem> statisticList = new ObservableCollection<StatisticsItem>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            DataGridStatistics.ItemsSource = statisticList;
            
            AddOverviewStatistic(StatisticOverviewType.TrackLengthSum);
            AddOverviewStatistic(StatisticOverviewType.TrackCount);
            AddOverviewStatistic(StatisticOverviewType.TrackLengthLongest);
            AddOverviewStatistic(StatisticOverviewType.TrackLengthShortest);
            AddOverviewStatistic(StatisticOverviewType.TrackLengthAverage);
            AddOverviewStatistic(StatisticOverviewType.TrackCountCDAverage);
            AddOverviewStatistic(StatisticOverviewType.CDCount);
            AddOverviewStatistic(StatisticOverviewType.CDSamplerCount);
            AddOverviewStatistic(StatisticOverviewType.CDTrackMax);
            AddOverviewStatistic(StatisticOverviewType.CDLengthLongest);
            AddOverviewStatistic(StatisticOverviewType.CDLengthShortest);
            AddOverviewStatistic(StatisticOverviewType.CDLengthAverage);
            AddOverviewStatistic(StatisticOverviewType.ArtistCount);
            AddOverviewStatistic(StatisticOverviewType.CDSetsCount);
            AddOverviewStatistic(StatisticOverviewType.CDAssignedNotCount);
            AddOverviewStatistic(StatisticOverviewType.CDAssignedCount);
            AddOverviewStatistic(StatisticOverviewType.CDLoanedCount);
            AddOverviewStatistic(StatisticOverviewType.CDTotalValue);
        }

        private void AddOverviewStatistic(StatisticOverviewType statisticOverviewType)
        {
            StatisticsItem statItem = new StatisticsItem();
            statItem.StatisticOverviewType = statisticOverviewType;
            statisticList.Add(statItem);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                switch (statisticOverviewType)
                {
                    case StatisticOverviewType.TrackLengthSum:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS1;
                            Int64 value = StatisticTrackLengthSum();
                            e.Result = Misc.GetTextFromSeconds((int)(value / 1000), false);
                            break;
                        }
                    case StatisticOverviewType.TrackCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS2;
                            Int32 value = StatisticTrackCount();
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.TrackLengthLongest:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS3;
                            statItem.ShowDetails = true;
                            Int32 value = StatisticTrackLongest();
                            e.Result = Misc.GetShortTimeString(value);
                            statItem.RawValue = value;
                            break;
                        }
                    case StatisticOverviewType.TrackLengthShortest:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS4;
                            statItem.ShowDetails = true;
                            Int32 value = StatisticTrackShortest();
                            statItem.RawValue = value;
                            e.Result = Misc.GetShortTimeString(value);
                            break;
                        }
                    case StatisticOverviewType.TrackLengthAverage:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS5;
                            Int64 value = StatisticTrackAverage();
                            e.Result = Misc.GetShortTimeString(value);
                            break;
                        }
                    case StatisticOverviewType.TrackCountCDAverage:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS6;
                            double value = StatisticTrackCountCDAverage();
                            e.Result = string.Format("{0:0.##}", value);
                            break;
                        }
                    case StatisticOverviewType.CDCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS7;
                            int value = StatisticCDsCount();
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDSamplerCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS8;
                            int value = StatisticCDSamplerCount();
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDTrackMax:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS9;
                            statItem.ShowDetails = true;
                            int value = StatisticCDTrackMax();
                            statItem.RawValue = value;
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDLengthShortest:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS12;
                            statItem.ShowDetails = true;
                            int value = StatisticCDLengthShortest();
                            statItem.RawValue = value;
                            e.Result = Misc.GetShortTimeString(value);
                            break;
                        }
                    case StatisticOverviewType.CDLengthLongest:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS11;
                            statItem.ShowDetails = true;
                            int value = StatisticCDLengthLongest();
                            statItem.RawValue = value;
                            e.Result = Misc.GetShortTimeString(value);
                            break;
                        }
                    case StatisticOverviewType.CDLengthAverage:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS10;
                            long value = StatisticCDLengthAverage();
                            e.Result = Misc.GetShortTimeString(value);
                            break;
                        }
                    case StatisticOverviewType.ArtistCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS13;
                            int value = StatisticArtistCount();
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDSetsCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS14;
                            int value = StatisticCDSetsCount();
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDAssignedNotCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS15;
                            statItem.ShowDetails = true;
                            int value = StatisticCDAssignedNotCount();
                            statItem.RawValue = value;
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDAssignedCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS16;
                            statItem.ShowDetails = true;
                            int value = StatisticCDAssignedCount();
                            statItem.RawValue = value;
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDLoanedCount:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS17;
                            int value = StatisticCDLoanedCount();
                            e.Result = value.ToString();
                            break;
                        }
                    case StatisticOverviewType.CDTotalValue:
                        {
                            statItem.Name = StringTable.TEXT_STATISTICS18;
                            long value = StatisticCDTotalValue();
                            e.Result = Misc.FormatCurrencyValue((int)value);
                            break;
                        }
                }
            };
            bw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                statItem.Value = (string)e.Result;
            };
            bw.RunWorkerAsync();
        }

        #region Statistics Overview Table
        private Int64 StatisticTrackLengthSum()
        {
            // SUM kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
            DataTable dt = DataBase.ExecuteFreeSql("SELECT Length FROM Track where Length > 0");
            Int64 length = 0;
            foreach (DataRow dr in dt.Rows)
                length += (int)dr["Length"];

            return length;
        }

        private int StatisticTrackCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT COUNT(1) FROM Track");
        }

        private int StatisticTrackLongest()
        {
            object val = DataBase.ExecuteScalar("SELECT MAX(Length) FROM Track");
            if (val is DBNull)
                return 0;
            else
                return (int)val;
        }

        private int StatisticTrackShortest()
        {
            object val = DataBase.ExecuteScalar("SELECT MIN(Length) FROM Track where Length > 0");

            if (val is DBNull)
                return 0;
            else
                return (int)val;
        }

        private Int64 StatisticTrackAverage()
        {
            // AVG kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
            DataTable dt = DataBase.ExecuteFreeSql("SELECT Length FROM Track where Length > 0");
            Int64 length = 0;
            foreach (DataRow dr in dt.Rows)
                length += (int)dr["Length"];

            if (dt.Rows.Count != 0)
                return length / dt.Rows.Count;
            else
                return 0;
        }

        private double StatisticTrackCountCDAverage()
        {
            // AVG kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
            DataTable dt = DataBase.ExecuteFreeSql("SELECT NumberOfTracks FROM cd");
            Int64 numberOfTracks = 0;
            foreach (DataRow dr in dt.Rows)
                numberOfTracks += (int)dr["NumberOfTracks"];

            if (dt.Rows.Count != 0)
                return (double)numberOfTracks / (double)dt.Rows.Count;
            else
                return 0;
        }

        private int StatisticCDsCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT count(1) FROM cd");
        }

        private int StatisticCDSamplerCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT count(1) FROM cd where IsSampler <> 0");
        }

        private int StatisticCDTrackMax()
        {
            object val = DataBase.ExecuteScalar("SELECT MAX(NumberOfTracks) FROM cd");

            if (val is DBNull)
                return 0;
            else
                return (int)val;
        }

        private Int64 StatisticCDLengthAverage()
        {
            // AVG kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
            DataTable dt = DataBase.ExecuteFreeSql("SELECT Length FROM cd where Length > 0");
            Int64 length = 0;
            foreach (DataRow dr in dt.Rows)
                length += (int)dr["Length"];

            if (dt.Rows.Count != 0)
                return length / dt.Rows.Count;
            else
                return 0;
        }

        private int StatisticCDLengthLongest()
        {
            object val = DataBase.ExecuteScalar("SELECT MAX(Length) FROM cd");

            if (val is DBNull)
                return 0;
            else
                return (int)val;
        }

        private int StatisticCDLengthShortest()
        {
            object val = DataBase.ExecuteScalar("SELECT min(Length) FROM cd where Length > 0");

            if (val is DBNull)
                return 0;
            else
                return (int)val;
        }

        private int StatisticArtistCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT count(1) FROM PersonGroup WHERE PersonGroupID IN (SELECT ArtistID FROM Track)");
        }

        private int StatisticCDSetsCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT count(1) FROM [Set]");
        }

        private int StatisticCDAssignedCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT COUNT(1) FROM CD WHERE [Identity]<>''");
        }

        private int StatisticCDAssignedNotCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT Count(1) FROM CD WHERE [identity]='' or [identity] is null");
        }

        private int StatisticCDLoanedCount()
        {
            return (int)DataBase.ExecuteScalar("SELECT count (1) FROM LoanedCD");
        }

        private long StatisticCDTotalValue()
        {
            // SUM kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
            DataTable dt = DataBase.ExecuteFreeSql("SELECT PRICE FROM CD");
            long length = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (!dr.IsNull("PRICE"))
                    length += (int)dr["PRICE"];
            }

            return length;
        }

        #endregion

        private void ButtonShowDetails_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow row = VisualTreeExtensions.FindParent<DataGridRow>(sender as DependencyObject);

            if (row != null)
            {
                StatisticsItem item = row.DataContext as StatisticsItem;
                Big3.Hitbase.DataBaseEngine.Condition condition = new DataBaseEngine.Condition();
                string image = "";
                string title = "";
                CurrentViewMode viewMode = CurrentViewMode.None;

                if (item.ShowDetails)
                {
                    switch (item.StatisticOverviewType)
                    {
                        case StatisticOverviewType.TrackLengthLongest:
                        case StatisticOverviewType.TrackLengthShortest:
                            {
                                condition.Add(new SingleCondition(Field.TrackLength, Operator.Equal, item.RawValue));
                                title = item.Name;
                                viewMode = CurrentViewMode.MyMusicTable;
                                image = "Music.png";
                                break;
                            }
                        case StatisticOverviewType.CDTrackMax:
                            {
                                condition.Add(new SingleCondition(Field.NumberOfTracks, Operator.Equal, item.RawValue));
                                title = item.Name;
                                viewMode = CurrentViewMode.AlbumTable;
                                image = "Music.png";
                                break;
                            }
                        case StatisticOverviewType.CDLengthLongest:
                        case StatisticOverviewType.CDLengthShortest:
                            {
                                condition.Add(new SingleCondition(Field.TotalLength, Operator.Equal, item.RawValue));
                                title = item.Name;
                                viewMode = CurrentViewMode.AlbumTable;
                                image = "Music.png";
                                break;
                            }
                        case StatisticOverviewType.CDAssignedCount:
                            {
                                condition.Add(new SingleCondition(Field.Identity, Operator.NotEmpty, null));
                                title = item.Name;
                                viewMode = CurrentViewMode.AlbumTable;
                                image = "Music.png";
                                break;
                            }
                        case StatisticOverviewType.CDAssignedNotCount:
                            {
                                condition.Add(new SingleCondition(Field.Identity, Operator.Empty, null));
                                title = item.Name;
                                viewMode = CurrentViewMode.AlbumTable;
                                image = "Music.png";
                                break;
                            }
                        default:
                            break;
                    }

                    AddViewCommandParameters addViewParams = new AddViewCommandParameters();
                    addViewParams.Condition = condition;
                    addViewParams.ImageResourceString = image;
                    addViewParams.Title = title;
                    addViewParams.ViewMode = viewMode;

                    CatalogViewCommands.AddView.Execute(addViewParams, Application.Current.MainWindow);
                }
            }
        }
    }

    public class StatisticsItem : INotifyPropertyChanged
    {
        private string name;
        public string Name 
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string _value;
        public string Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private object rawValue;

        public object RawValue
        {
            get 
            { 
                return rawValue; 
            }
            set 
            { 
                rawValue = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RawValue"));
            }
        }

        private bool showDetails;
        public bool ShowDetails
        {
            get 
            { 
                return showDetails; 
            }
            set 
            { 
                showDetails = value; 

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ShowDetails"));
            }
        }

        private StatisticOverviewType statisticOverviewType;

        public StatisticOverviewType StatisticOverviewType
        {
            get 
            { 
                return statisticOverviewType; 
            }
            set 
            { 
                statisticOverviewType = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("StatisticOverviewType"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ShowDetailsToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum StatisticOverviewType
    {
        TrackLengthSum,
        TrackCount,
        TrackLengthLongest,
        TrackLengthShortest,
        TrackLengthAverage,
        TrackCountCDAverage,
        CDCount,
        CDSamplerCount,
        CDTrackMax,
        CDLengthAverage,
        CDLengthLongest,
        CDLengthShortest,
        ArtistCount,
        CDSetsCount,
        CDAssignedCount,
        CDAssignedNotCount,
        CDLoanedCount,
        CDTotalValue
    }
}
