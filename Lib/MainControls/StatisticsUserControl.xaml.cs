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
using System.Windows.Controls.DataVisualization.Charting;
using System.ComponentModel;
using Big3.Hitbase.DataBaseEngine;
using System.Data;
using Big3.Hitbase.Miscellaneous;
using System.Windows.Media.Animation;
using Big3.Hitbase.SharedResources;
using Big3.Hitbase.Configuration;
using System.Xml.Serialization;
using System.IO;

namespace Big3.Hitbase.MainControls
{
	/// <summary>
	/// Interaction logic for StatisticsUserControl.xaml
	/// </summary>
	public partial class StatisticsUserControl : UserControl, INotifyPropertyChanged, IMainTabInterface
	{
		#region Private members

		private DataBase _dataBase;
        private int lastZoomedIndex = -1;
        private Thickness lastZoomedMargin;
        private bool isChartZoomed = false;

		#endregion

		#region Constructor

        public StatisticsUserControl(DataBase db, StatisticType statisticType)
		{
			_dataBase = db;
            _bigChartActive = false;

            DataContext = this;

  			InitializeComponent();

			StatisticColumns = 2;
            

            if (statisticType == StatisticType.Overview)
            {
                // Spezial Fall -> zeige eine Übersicht ausgewählter Statistiken
                OverviewVisible = true;
                StaticticsShowOverView();
            }
            else
            {
                // Zeige ausgewählte Einzelstatistik
                OverviewVisible = false;
            }

            return;
		}

		#endregion

		#region Properties

        private bool _bigChartActive;
        public bool BigChartActive
        {
            get
            {
                return _bigChartActive; 
            }

            set
            {
                _bigChartActive = value;
                NotifyPropertyChange("BigChartActive");
            }
        }

		private List<Chart> _staticsDisplayed;
		public List<Chart> StaticsDisplayed
		{
			get
			{
				return _staticsDisplayed;
			}

			set
			{
				_staticsDisplayed = value;
				NotifyPropertyChange("StaticsDisplayed");
			}
		}

		private int _statisticColumns;
		public int StatisticColumns
		{
			get
			{
				return _statisticColumns;
			}

			set
			{
				_statisticColumns = value;
				NotifyPropertyChange("StatisticColumns");
			}
		}

		private int _statisticRows;
		public int StatisticRows
		{
			get
			{
				return _statisticRows;
			}

			set
			{
				_statisticRows = value;
				NotifyPropertyChange("StatisticRows");
			}
		}

        private bool _overviewVisible;
        public bool OverviewVisible
        {
            get
            {
                return _overviewVisible;
            }

            set
            {
                _overviewVisible = value;
                NotifyPropertyChange("OverviewVisible");
            }
        }

		#endregion

		//
		// Zeigt die Statistik Übersichtsseite
		//
		private void StaticticsShowOverView()
		{
            LoadConfiguration();
		}

        private void AddStatistic(StatisticType statisticType, StatisticChartType chartType)
        {
            StatisticsItemUserControl chartControl = new StatisticsItemUserControl();

            chartControl.DataBase = _dataBase;
            chartControl.StatisticType = statisticType;
            chartControl.StatisticChartType = chartType;

            ChartGridOverview.Children.Add(chartControl);
        }

        private Storyboard myStoryboard;
        // 
        void chart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BigChartActive = true;
            //MessageBox.Show("TODO: Mach groß");

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0.0;
            myDoubleAnimation.To = 520.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            myDoubleAnimation.AutoReverse = false;
            //myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // change the size
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, (Chart)sender);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Rectangle.HeightProperty));
            myStoryboard.Begin(this);


        }

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChange(string PropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}

		#endregion

        private void CommandBindingZoomChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StatisticsItemUserControl clickChart = e.Parameter as StatisticsItemUserControl;

            if (clickChart.IsZoomed)
                ZoomOut(clickChart);
            else
                ZoomIn(clickChart);
        }

        private void ZoomIn(StatisticsItemUserControl clickChart)
        {
            clickChart.IsZoomed = true;
            isChartZoomed = true;

            DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500).Duration());
            this.ChartGridOverview.BeginAnimation(Grid.OpacityProperty, da);

            lastZoomedIndex = this.ChartGridOverview.Children.IndexOf(clickChart);
            this.ChartGridOverview.Children.Remove(clickChart);
            this.ChartGridOverview.Children.Insert(lastZoomedIndex, new Grid());        // Platzhalter

            gridZoomChart.Children.Add(clickChart);

            Point pt = clickChart.PointToScreen(new Point(0, 0));
            Point pt1 = MainGrid.PointFromScreen(pt);

            Point ptSize = clickChart.PointToScreen(new Point(clickChart.ActualWidth, clickChart.ActualHeight));
            Point ptSize1 = MainGrid.PointFromScreen(ptSize);

            ThicknessAnimation marginAnim = new ThicknessAnimation();
            marginAnim.From = new Thickness(pt1.X, pt1.Y, MainGrid.ActualWidth - ptSize1.X, MainGrid.ActualHeight - ptSize1.Y);
            marginAnim.To = new Thickness(0);
            marginAnim.Duration = TimeSpan.FromMilliseconds(500).Duration();
            marginAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            clickChart.BeginAnimation(Grid.MarginProperty, marginAnim);
        }


        private void ZoomOut(StatisticsItemUserControl clickChart)
        {
            clickChart.IsZoomed = false;
            isChartZoomed = false;

            DoubleAnimation da = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500).Duration());
            this.ChartGridOverview.BeginAnimation(Grid.OpacityProperty, da);

            Grid placeholderGrid = this.ChartGridOverview.Children[lastZoomedIndex] as Grid;

            Point pt = placeholderGrid.PointToScreen(new Point(0, 0));
            Point pt1 = MainGrid.PointFromScreen(pt);

            Point ptSize = placeholderGrid.PointToScreen(new Point(placeholderGrid.ActualWidth, placeholderGrid.ActualHeight));
            Point ptSize1 = MainGrid.PointFromScreen(ptSize);

            ThicknessAnimation marginAnim = new ThicknessAnimation();
            marginAnim.From = new Thickness(0);
            marginAnim.To = new Thickness(pt1.X, pt1.Y, MainGrid.ActualWidth - ptSize1.X, MainGrid.ActualHeight - ptSize1.Y);
            marginAnim.Duration = TimeSpan.FromMilliseconds(500).Duration();
            marginAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            marginAnim.Completed += new EventHandler(marginAnim_Completed);

            clickChart.BeginAnimation(Grid.MarginProperty, marginAnim);
        }

        void marginAnim_Completed(object sender, EventArgs e)
        {
            StatisticsItemUserControl clickChart = gridZoomChart.Children[0] as StatisticsItemUserControl;
            gridZoomChart.Children.Remove(clickChart);

            this.ChartGridOverview.Children.RemoveAt(lastZoomedIndex);
            this.ChartGridOverview.Children.Insert(lastZoomedIndex, clickChart);        // Platzhalter
            clickChart.BeginAnimation(Grid.MarginProperty, null);

        }

        private void LoadConfiguration()
        {
            StatisticsConfiguration defaultConfig = new StatisticsConfiguration();

            defaultConfig.Charts.Add(new ChartConfiguration() { StatisticType = StatisticType.Overview, ChartType = StatisticChartType.None });
            defaultConfig.Charts.Add(new ChartConfiguration() { StatisticType = StatisticType.CDGroupByCategoryCount, ChartType = StatisticChartType.Bar });
            defaultConfig.Charts.Add(new ChartConfiguration() { StatisticType = StatisticType.CDGroupByMediumCount, ChartType = StatisticChartType.Column });
            defaultConfig.Charts.Add(new ChartConfiguration() { StatisticType = StatisticType.CDGroupByPriceCount, ChartType = StatisticChartType.Pie });

            StatisticsConfiguration config = StatisticsConfiguration.LoadConfiguration(defaultConfig);

            foreach (ChartConfiguration chart in config.Charts)
            {
                AddStatistic(chart.StatisticType, chart.ChartType);
            }
        }

        private void SaveConfiguration()
        {
            if (isChartZoomed)
                return;

            StatisticsConfiguration config = new StatisticsConfiguration();

            foreach (StatisticsItemUserControl statControl in ChartGridOverview.Children)
            {
                ChartConfiguration chart = new ChartConfiguration();

                chart.ChartType = statControl.StatisticChartType;
                chart.StatisticType = statControl.StatisticType;

                config.Charts.Add(chart);
            }

            config.SaveConfiguration();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveConfiguration();
        }

        public bool Closing()
        {
            return true;
        }

        public void Restore(Microsoft.Win32.RegistryKey regKey)
        {
        }

        public void Save(Microsoft.Win32.RegistryKey regKey)
        {
        }
    }

    [Serializable]
    public class StatisticsConfiguration
    {
        public StatisticsConfiguration()
        {
            Charts = new List<ChartConfiguration>();
        }

        public List<ChartConfiguration> Charts { get; set; }

        internal static StatisticsConfiguration LoadConfiguration(StatisticsConfiguration defaultConfig)
        {
            object statisticConfigString = Settings.GetValue("StatisticsConfiguration", null);
            if (statisticConfigString == null)
                return defaultConfig;
            XmlSerializer bf = new XmlSerializer(typeof(StatisticsConfiguration));
            using (StringReader sr = new StringReader(statisticConfigString.ToString()))
            {
                StatisticsConfiguration statisticConfig = (StatisticsConfiguration)bf.Deserialize(sr);

                if (statisticConfig.Charts.Count == 0)
                    return defaultConfig;
                else
                    return statisticConfig;
            }

        }

        internal void SaveConfiguration()
        {
            XmlSerializer bf = new XmlSerializer(GetType());
            StringWriter sw = new StringWriter();
            bf.Serialize(sw, this);

            Settings.SetValue("StatisticsConfiguration", sw.ToString());

            sw.Close();
        }
    }

    [Serializable]
    public class ChartConfiguration
    {
        public StatisticType StatisticType { get; set; }
        public StatisticChartType ChartType { get; set; }
    }

    public class SquareContainer : ContentControl
    {
        private Size CalculateMaxSize(Size availableSize)
        {
            double maxDimension = Math.Min(availableSize.Height,
                                            availableSize.Width);

            Size maxSize = new Size(maxDimension, maxDimension);

            return maxSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size maxSize = CalculateMaxSize(availableSize);

            if (Content != null && Content is FrameworkElement)
            {
                ((FrameworkElement)Content).Measure(maxSize);
            }

            return maxSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size maxSize = CalculateMaxSize(finalSize);

            if (Content != null && Content is FrameworkElement)
            {
                ((FrameworkElement)Content).Arrange(
                                new Rect(new Point(0, 0), maxSize));
            }

            return maxSize;
        }
    }

}
