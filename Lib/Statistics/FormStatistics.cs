using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;    // Date

using Big3.Hitbase.Configuration;
using Big3.Hitbase.Statistics;
using Big3.Hitbase.Miscellaneous;
using Big3.Hitbase.DataBaseEngine;
using System.Data.SqlServerCe;
using Big3.Hitbase.CatalogView;
using System.IO;

namespace Big3.Hitbase.Statistics
{
	public enum Statistic
	{
		Overview,
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
		CDGroupByCategoryCount,
		CDGroupyMediumCount,
		CDGroupyPriceCount,
		CDGroupByNumberOfTracksCount,
		CDGroupByLengthCount,
		CDGroupBySamplerCount,
		CDGroupByRatingCount,
		CDGroupByAttributeCount,
		CDGroupByLabelCount,
		CDGroupByRecordCount,
		CDGroupByArtistArtCount,
		CDGroupByArtistSexCount,
		CDGroupByCountryCount,
		CDTotalValue,
		TrackGroupByLengthCount,
		TrackGropupByRecordCount,
		TrackGroupByRating,
		TrackGroupByBPMCount,
		TrackGroupByAttributeCount,
		ArtistTrackMost,
		ArtistTrackGroupBySexCount,
		ArtistTrackGroupByCountryCount,
		ArtistTrackGroupByArtistArtCount,
		ArtistCDsMost,
		ArtistGroupBySexCount,
		ArtistGroupByCountryCount,
		ArtistGroupByArtistArtCount,
		TextOverview
	}

	public enum StatisticListType
	{
		Text,
		Graphic,
		All
	}
	public enum StatisticChartTypes
	{
		Pie,
		Line,
		Bar,
		Donut
	}
	public partial class FormStatistics : Form
	{
		DataBase dataBase;

		private class GraphicPoint
		{
			public String sName;
			public Double nValue;
		}

		private PrintDocument printDoc = new PrintDocument();
		private System.Windows.Forms.DataVisualization.Charting.Chart[] oGraphOverview = new System.Windows.Forms.DataVisualization.Charting.Chart[9];
		private System.Windows.Forms.DataVisualization.Charting.ChartArea[] oChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea[9];
		private System.Windows.Forms.DataVisualization.Charting.Legend[] oChartLegend = new System.Windows.Forms.DataVisualization.Charting.Legend[9];
		private System.Windows.Forms.DataVisualization.Charting.Series[] oChartSeries = new System.Windows.Forms.DataVisualization.Charting.Series[9];
		private System.Windows.Forms.DataVisualization.Charting.Title[] oChartTitle = new System.Windows.Forms.DataVisualization.Charting.Title[9];

		private StatisticChartTypes ChartStyle;

		private int ZommLevelStart;
		private int ZommLevelEnd;

		public FormStatistics(DataBase db)
		{
			int IndexX;
			int IndexY;
			dataBase = db;

			InitializeComponent();

			ChartStyle = StatisticChartTypes.Bar;

			for (IndexY = 0; IndexY < 3; IndexY++)
			{
				for (IndexX = 0; IndexX < 3; IndexX++)
				{
					oGraphOverview[(IndexY * 3) + IndexX] = new Chart();
					this.tableLayoutPanel1.Controls.Add(oGraphOverview[(IndexY * 3) + IndexX], 0, 0);
				}
			}

			ZommLevelStart = 1;
			ZommLevelEnd = 1;

			//printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
			Settings.RestoreWindowPlacement(this, "Statistics");
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			String sHelp;
			// Statistic Listbox füllen

			TreeNode treenode = new TreeNode();
			TreeNode treenodesub;
			TreeNode treenodeOverview;
			Int32 Number;

			treenodeOverview = new TreeNode(); treenodeOverview.Text = StringTable.TEXT_OVERVIEW; treenodeOverview.Tag = Statistic.Overview;
			trvStatistics.Nodes.Add(treenodeOverview);

			treenode = trvStatistics.Nodes.Add("CDs", "CDs");

			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_1; treenodesub.Tag = Statistic.CDGroupByCategoryCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_2; treenodesub.Tag = Statistic.CDGroupyMediumCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_3; treenodesub.Tag = Statistic.CDGroupyPriceCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_4; treenodesub.Tag = Statistic.CDGroupByNumberOfTracksCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_5; treenodesub.Tag = Statistic.CDGroupByLengthCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_6; treenodesub.Tag = Statistic.CDGroupBySamplerCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_7; treenodesub.Tag = Statistic.CDGroupByRatingCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_8; treenodesub.Tag = Statistic.CDGroupByAttributeCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_9; treenodesub.Tag = Statistic.CDGroupByLabelCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_10; treenodesub.Tag = Statistic.CDGroupByRecordCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_11; treenodesub.Tag = Statistic.CDGroupByArtistArtCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_12; treenodesub.Tag = Statistic.CDGroupByArtistSexCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_13; treenodesub.Tag = Statistic.CDGroupByCountryCount; treenode.Nodes.Add(treenodesub);

			treenode = trvStatistics.Nodes.Add("Tracks", "Tracks");
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_14; treenodesub.Tag = Statistic.TrackGroupByLengthCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_15; treenodesub.Tag = Statistic.TrackGropupByRecordCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_16; treenodesub.Tag = Statistic.TrackGroupByRating; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_17; treenodesub.Tag = Statistic.TrackGroupByBPMCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_20; treenodesub.Tag = Statistic.TrackGroupByAttributeCount; treenode.Nodes.Add(treenodesub);

			treenode = trvStatistics.Nodes.Add("Interpret", "Interpret");
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_18; treenodesub.Tag = Statistic.ArtistTrackMost; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_19; treenodesub.Tag = Statistic.ArtistTrackGroupBySexCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_21; treenodesub.Tag = Statistic.ArtistTrackGroupByCountryCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_22; treenodesub.Tag = Statistic.ArtistTrackGroupByArtistArtCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_23; treenodesub.Tag = Statistic.ArtistCDsMost; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_24; treenodesub.Tag = Statistic.ArtistGroupBySexCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_25; treenodesub.Tag = Statistic.ArtistGroupByCountryCount; treenode.Nodes.Add(treenodesub);
			treenodesub = new TreeNode(); treenodesub.Text = StringTable.TEXT_STATISTICS_GRAF_26; treenodesub.Tag = Statistic.ArtistGroupByArtistArtCount; treenode.Nodes.Add(treenodesub);

			trvStatistics.SelectedNode = treenodeOverview;

			trvStatistics.ExpandAll();
			columnHeaderName.Width = 200;
			columnHeaderValue.Width = 110;

			Number = 0;

			// einfache Statistiken füllen
			ListViewItem ListItem;
			Int64 LongNumber;
			double DoubleNumber;

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS1);
			StatisticTrackLengthSum(out LongNumber);
			Number = Convert.ToInt32(LongNumber / 1000);
			sHelp = Misc.GetTextFromSeconds(Number);
			ListItem.SubItems.Add(sHelp);

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS2);
			StatisticTrackCount(out Number);
			ListItem.SubItems.Add(Number.ToString());

			// longest track
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS3);
			StatisticTrackLongest(out Number);
			sHelp = Hitbase.Miscellaneous.Misc.GetShortTimeString(Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(sHelp);

			// shortest track
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS4);
			StatisticTrackShortest(out Number);
			sHelp = Hitbase.Miscellaneous.Misc.GetShortTimeString(Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(sHelp);

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS5);
			StatisticTrackAverage(out LongNumber);
			sHelp = Hitbase.Miscellaneous.Misc.GetShortTimeString((int)LongNumber);
			ListItem.SubItems.Add(sHelp);

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS6);
			StatisticTrackCountCDAverage(out DoubleNumber);
			ListItem.SubItems.Add(DoubleNumber.ToString());

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS7);
			StatisticCDsCount(out Number);
			ListItem.SubItems.Add(Number.ToString());

			// counter sampler CDs
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS8);
			StatisticCDSamplerCount(out Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(Number.ToString());

			// Cd with greatest number of tracks
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS9);
			StatisticCDTrackMax(out Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(Number.ToString());

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS10);
			StatisticCDLengthAverage(out LongNumber);
			sHelp = Hitbase.Miscellaneous.Misc.GetShortTimeString((int)LongNumber);
			ListItem.SubItems.Add(sHelp);

			// longest cd
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS11);
			StatisticCDLengthLongest(out Number);
			sHelp = Hitbase.Miscellaneous.Misc.GetShortTimeString(Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(sHelp);

			// shortest cd
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS12);
			StatisticCDLengthShortest(out Number);
			sHelp = Hitbase.Miscellaneous.Misc.GetShortTimeString(Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(sHelp);

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS13);
			StatisticArtistCount(out Number);
			ListItem.SubItems.Add(Number.ToString());

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS14);
			StatisticCDSetsCount(out Number);
			ListItem.SubItems.Add(Number.ToString());

			// number of not assigned cds
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS15);
			StatisticCDAssignedNotCount(out Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(Number.ToString());

			// number of assigned cds
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS16);
			StatisticCDAssignedCount(out Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(Number.ToString());

			// loaned cds
			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS17);
			StatisticCDLoanedCount(out Number);
			ListItem.Tag = Number;
			ListItem.SubItems.Add(Number.ToString());

			ListItem = SimpleStatistics.Items.Add(StringTable.TEXT_STATISTICS18);
			StatisticCDTotalValue(out Number);
			sHelp = Hitbase.Miscellaneous.Misc.FormatCurrencyValue(Number);
			ListItem.SubItems.Add(sHelp);

			for (int Value = 0; Value <= 8; Value++)
			{
				oGraphOverview[Value].GetToolTipText += new EventHandler<ToolTipEventArgs>(oGraphControl_GetToolTipText);
				oGraphOverview[Value].AxisScrollBarClicked += new EventHandler<ScrollBarEventArgs>(FormStatistics_AxisScrollBarClicked);
				oGraphOverview[Value].MouseClick += new MouseEventHandler(FormStatistics_MouseClick);
				oGraphOverview[Value].ContextMenuStrip = contextMenuGraph;
			}

			//StaticticsRedraw();
		}

		void FormStatistics_MouseClick(object sender, MouseEventArgs e)
		{
			// Call Hit Test Method
			HitTestResult result = ((Chart)sender).HitTest(e.X, e.Y);

			if (result.PointIndex == -1)
				return;

			// Check if data point is already exploded
			bool exploded = (((Chart)sender).Series[0].Points[result.PointIndex].CustomProperties == "Exploded=true") ? true : false;

			// Remove all exploded attributes
			foreach (DataPoint point in ((Chart)sender).Series[0].Points)
			{
				point.CustomProperties = "";
			}

			// If data point is already exploded get out.
			if (exploded)
				return;

			// If data point is selected
			if (result.ChartElementType == ChartElementType.DataPoint)
			{
				// Set Attribute
				DataPoint point = ((Chart)sender).Series[0].Points[result.PointIndex];
				point.CustomProperties = "Exploded = true";
			}

			// If legend item is selected
			if (result.ChartElementType == ChartElementType.LegendItem)
			{
				// Set Attribute
				DataPoint point = ((Chart)sender).Series[0].Points[result.PointIndex];
				point.CustomProperties = "Exploded = true";
			}
		}

		void FormStatistics_AxisScrollBarClicked(object sender, ScrollBarEventArgs e)
		{
			// Event is handled, no more processing required
			e.IsHandled = true;

			// Reset zoom on X and Y axis
			e.ChartArea.AxisX.ScaleView.ZoomReset();
			e.ChartArea.AxisY.ScaleView.ZoomReset();
		}

		private class ListBoxItem
		{
			public ListBoxItem(Statistic stat)
			{
				Statistic = stat;
			}

			public Statistic Statistic;

			public override string ToString()
			{
				switch (Statistic)
				{
					case Statistic.Overview:
						return StringTable.TEXT_OVERVIEW;

					case Statistic.TrackLengthSum:
						return StringTable.TEXT_STATISTICS1;

					case Statistic.TrackCount:
						return StringTable.TEXT_STATISTICS2;

					case Statistic.TrackLengthLongest:
						return StringTable.TEXT_STATISTICS3;

					case Statistic.TrackLengthShortest:
						return StringTable.TEXT_STATISTICS4;

					case Statistic.TrackLengthAverage:
						return StringTable.TEXT_STATISTICS5;

					case Statistic.TrackCountCDAverage:
						return StringTable.TEXT_STATISTICS6;

					case Statistic.CDCount:
						return StringTable.TEXT_STATISTICS7;

					case Statistic.CDSamplerCount:
						return StringTable.TEXT_STATISTICS8;

					case Statistic.CDTrackMax:
						return StringTable.TEXT_STATISTICS9;

					case Statistic.CDLengthAverage:
						return StringTable.TEXT_STATISTICS10;

					case Statistic.CDLengthLongest:
						return StringTable.TEXT_STATISTICS11;

					case Statistic.CDLengthShortest:
						return StringTable.TEXT_STATISTICS12;

					case Statistic.ArtistCount:
						return StringTable.TEXT_STATISTICS13;

					case Statistic.CDSetsCount:
						return StringTable.TEXT_STATISTICS14;

					case Statistic.CDAssignedNotCount:
						return StringTable.TEXT_STATISTICS15;

					case Statistic.CDAssignedCount:
						return StringTable.TEXT_STATISTICS16;

					case Statistic.CDLoanedCount:
						return StringTable.TEXT_STATISTICS17;

					case Statistic.CDTotalValue:
						return StringTable.TEXT_STATISTICS18;

					case Statistic.CDGroupByCategoryCount:
						return StringTable.TEXT_STATISTICS_GRAF_1;

					case Statistic.CDGroupyMediumCount:
						return StringTable.TEXT_STATISTICS_GRAF_2;

					case Statistic.CDGroupyPriceCount:
						return StringTable.TEXT_STATISTICS_GRAF_3;

					case Statistic.CDGroupByNumberOfTracksCount:
						return StringTable.TEXT_STATISTICS_GRAF_4;

					case Statistic.CDGroupByLengthCount:
						return StringTable.TEXT_STATISTICS_GRAF_5;

					case Statistic.CDGroupBySamplerCount:
						return StringTable.TEXT_STATISTICS_GRAF_6;

					case Statistic.CDGroupByRatingCount:
						return StringTable.TEXT_STATISTICS_GRAF_7;

					case Statistic.CDGroupByAttributeCount:
						return StringTable.TEXT_STATISTICS_GRAF_8;

					case Statistic.CDGroupByLabelCount:
						return StringTable.TEXT_STATISTICS_GRAF_9;

					case Statistic.CDGroupByRecordCount:
						return StringTable.TEXT_STATISTICS_GRAF_10;

					case Statistic.CDGroupByArtistArtCount:
						return StringTable.TEXT_STATISTICS_GRAF_11;

					case Statistic.CDGroupByArtistSexCount:
						return StringTable.TEXT_STATISTICS_GRAF_12;

					case Statistic.CDGroupByCountryCount:
						return StringTable.TEXT_STATISTICS_GRAF_13;

					case Statistic.TrackGroupByLengthCount:
						return StringTable.TEXT_STATISTICS_GRAF_14;

					case Statistic.TrackGropupByRecordCount:
						return StringTable.TEXT_STATISTICS_GRAF_15;

					case Statistic.TrackGroupByRating:
						return StringTable.TEXT_STATISTICS_GRAF_16;

					case Statistic.TrackGroupByBPMCount:
						return StringTable.TEXT_STATISTICS_GRAF_17;

					case Statistic.ArtistTrackMost:
						return StringTable.TEXT_STATISTICS_GRAF_18;

					case Statistic.ArtistTrackGroupBySexCount:
						return StringTable.TEXT_STATISTICS_GRAF_19;

					case Statistic.TrackGroupByAttributeCount:
						return StringTable.TEXT_STATISTICS_GRAF_20;

					case Statistic.ArtistTrackGroupByCountryCount:
						return StringTable.TEXT_STATISTICS_GRAF_21;

					case Statistic.ArtistTrackGroupByArtistArtCount:
						return StringTable.TEXT_STATISTICS_GRAF_22;

					case Statistic.ArtistCDsMost:
						return StringTable.TEXT_STATISTICS_GRAF_23;

					case Statistic.ArtistGroupBySexCount:
						return StringTable.TEXT_STATISTICS_GRAF_24;

					case Statistic.ArtistGroupByCountryCount:
						return StringTable.TEXT_STATISTICS_GRAF_25;

					case Statistic.ArtistGroupByArtistArtCount:
						return StringTable.TEXT_STATISTICS_GRAF_26;

					default:
						return "";
				}
			}
		}

		private void CreateGraph(System.Windows.Forms.DataVisualization.Charting.Chart oChart,
											 System.Windows.Forms.DataVisualization.Charting.ChartArea oChartArea,
											 System.Windows.Forms.DataVisualization.Charting.Legend oLegend,
											 System.Windows.Forms.DataVisualization.Charting.Series oChartSeries,
											 System.Windows.Forms.DataVisualization.Charting.Title oChartTitle)
		{
			List<String> sTextLabels;

			// initialization
			oChartTitle.Text = "";
			sTextLabels = new List<string>();

			// initialize new chart
			oChart.Annotations.Clear();
			oChart.ChartAreas.Clear();
			oChart.Legends.Clear();
			oChart.Series.Clear();
			oChart.Titles.Clear();


			// chart
			oChart.BackColor = System.Drawing.Color.FromArgb(211, 223, 240);
			oChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			oChart.BackSecondaryColor = System.Drawing.Color.White;
			oChart.BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105);
			oChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			oChart.BorderlineWidth = 2;
			oChart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;


			// chartarea
			oChartArea.Area3DStyle.Inclination = 15;
			oChartArea.Area3DStyle.IsClustered = true;
			oChartArea.Area3DStyle.IsRightAngleAxes = false;
			oChartArea.Area3DStyle.Perspective = 10;
			oChartArea.Area3DStyle.Rotation = 10;
			oChartArea.Area3DStyle.WallWidth = 0;
			oChartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			oChartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
			oChartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
			oChartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			oChartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
			oChartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
			oChartArea.BackColor = System.Drawing.Color.Transparent;
			oChartArea.BackSecondaryColor = System.Drawing.Color.White;
			oChartArea.BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
			oChartArea.BorderWidth = 0;
			oChartArea.Name = "Default";
			oChartArea.ShadowColor = System.Drawing.Color.Empty;


			oChartArea.CursorX.IsUserEnabled = true;
			oChartArea.CursorX.IsUserSelectionEnabled = true;
			oChartArea.CursorX.SelectionColor = System.Drawing.Color.Gray;

			oChartArea.CursorY.IsUserEnabled = true;
			oChartArea.CursorY.IsUserSelectionEnabled = true;
			oChartArea.CursorY.SelectionColor = System.Drawing.Color.Gray;

			oChartArea.AxisX.ScaleView.Zoomable = true;
			oChartArea.AxisX.ScrollBar.IsPositionedInside = true;



			oChart.ChartAreas.Add(oChartArea);

			// legend
			oLegend.Alignment = System.Drawing.StringAlignment.Center;
			oLegend.BackColor = System.Drawing.Color.Transparent;
			oLegend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
			oLegend.Enabled = false;
			oLegend.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			oLegend.IsTextAutoFit = false;
			oLegend.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
			oLegend.Name = "Default";
			oChart.Legends.Add(oLegend);

			// series
			oChartSeries.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);
			oChartSeries.ChartArea = "Default";
			oChartSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
			oChartSeries.CustomProperties = "PieLabelStyle=Ellipse";
			oChartSeries.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			oChartSeries.LabelBackColor = System.Drawing.Color.FromArgb(64, 255, 255, 200);
			oChartSeries.Legend = "Default";
			oChartSeries.Name = "Default";
			oChartSeries.ShadowOffset = 5;
			oChartSeries.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			oChartSeries.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			oChart.Series.Add(oChartSeries);

			oChartTitle.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
			oChartTitle.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
			oChartTitle.Name = "Title1";
			oChartTitle.ShadowColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
			oChartTitle.ShadowOffset = 3;
		}

		private void StaticticsRedraw()
		{
			Statistic SelectedStatistic;

			List<String> sTextLabels;

			if (trvStatistics.SelectedNode == null)
			{
				// Nix ausgewählt
				return;
			}
			if (trvStatistics.SelectedNode.Tag == null)
			{
				// Nix ausgewählt
				return;
			}

			SelectedStatistic = (Statistic)trvStatistics.SelectedNode.Tag;

			copyToolStripMenuItem.Enabled = (SelectedStatistic != Statistic.Overview);
			printToolStripMenuItem.Enabled = (SelectedStatistic != Statistic.Overview);
			printPreviewToolStripMenuItem.Enabled = (SelectedStatistic != Statistic.Overview);

			// Statistik Übersichtsseite
			if (SelectedStatistic == Statistic.Overview)
			{

				for (int IndexY = 0; IndexY < 3; IndexY++)
				{
					for (int IndexX = 0; IndexX < 3; IndexX++)
					{
						oGraphOverview[(IndexY * 3) + IndexX].Visible = true;
					}
				}
				tableLayoutPanel1.Visible = true;
				tableLayoutPanel1.SetRowSpan(oGraphOverview[0], 1);
				tableLayoutPanel1.SetColumnSpan(oGraphOverview[0], 1);

				StatisticOverview();
				return;
			}

			// Grafische Statistiken (mit mehreren Elementen)
			for (int IndexY = 0; IndexY < 3; IndexY++)
			{
				for (int IndexX = 0; IndexX < 3; IndexX++)
				{
					if (IndexY == 0 && IndexX == 0)
					{
						oGraphOverview[(IndexY * 3) + IndexX].Visible = true;
					}
					else
					{
						oGraphOverview[(IndexY * 3) + IndexX].Visible = false;
					}
				}
			}
			tableLayoutPanel1.Visible = true;
			tableLayoutPanel1.SetRowSpan(oGraphOverview[0], 3);
			tableLayoutPanel1.SetColumnSpan(oGraphOverview[0], 3);

			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Title oStatisticTitle = new System.Windows.Forms.DataVisualization.Charting.Title();

			// initialization
			oStatisticTitle.Text = "";
			sTextLabels = new List<string>();

			// initialize new chart
			oGraphOverview[0].Annotations.Clear();
			oGraphOverview[0].ChartAreas.Clear();
			oGraphOverview[0].Legends.Clear();
			oGraphOverview[0].Series.Clear();
			oGraphOverview[0].Titles.Clear();

			// chart
			oGraphOverview[0].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
			oGraphOverview[0].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			oGraphOverview[0].BackSecondaryColor = System.Drawing.Color.White;
			oGraphOverview[0].BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
			oGraphOverview[0].BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			oGraphOverview[0].BorderlineWidth = 2;
			oGraphOverview[0].BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;

			// chartarea
			chartArea1.Area3DStyle.Inclination = 15;
			chartArea1.Area3DStyle.IsClustered = true;
			chartArea1.Area3DStyle.IsRightAngleAxes = false;
			chartArea1.Area3DStyle.Perspective = 10;
			chartArea1.Area3DStyle.Rotation = 10;
			chartArea1.Area3DStyle.WallWidth = 0;
			chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.BackColor = System.Drawing.Color.Transparent;
			chartArea1.BackSecondaryColor = System.Drawing.Color.White;
			chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.BorderWidth = 0;
			chartArea1.Name = "Default";
			chartArea1.ShadowColor = System.Drawing.Color.Empty;


			chartArea1.CursorX.IsUserEnabled = true;
			chartArea1.CursorX.IsUserSelectionEnabled = true;
			chartArea1.CursorX.SelectionColor = System.Drawing.Color.Gray;

			chartArea1.CursorY.IsUserEnabled = true;
			chartArea1.CursorY.IsUserSelectionEnabled = true;
			chartArea1.CursorY.SelectionColor = System.Drawing.Color.Gray;

			chartArea1.AxisX.ScaleView.Zoomable = true;
			chartArea1.AxisX.ScrollBar.IsPositionedInside = true;


			oGraphOverview[0].ChartAreas.Add(chartArea1);

			// legend
			legend1.Alignment = System.Drawing.StringAlignment.Center;
			legend1.BackColor = System.Drawing.Color.Transparent;
			legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
			legend1.Enabled = false;
			legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			legend1.IsTextAutoFit = false;
			legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
			legend1.Name = "Default";
			oGraphOverview[0].Legends.Add(legend1);
			oGraphOverview[0].Location = new System.Drawing.Point(16, 61);
			oGraphOverview[0].Name = "chart1";
			oStatisticSeries.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
			oStatisticSeries.ChartArea = "Default";
			oStatisticSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
			oStatisticSeries.CustomProperties = "PieLabelStyle=Ellipse";
			oStatisticSeries.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			oStatisticSeries.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
			oStatisticSeries.Legend = "Default";
			oStatisticSeries.Name = "Default";
			oStatisticSeries.ShadowOffset = 5;
			oStatisticSeries.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			oStatisticSeries.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			oGraphOverview[0].Series.Add(oStatisticSeries);
			oGraphOverview[0].Size = new System.Drawing.Size(412, 296);
			oGraphOverview[0].TabIndex = 1;

			oStatisticTitle.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
			oStatisticTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
			oStatisticTitle.Name = "Title1";
			oStatisticTitle.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			oStatisticTitle.ShadowOffset = 3;

			switch (SelectedStatistic)
			{
				case Statistic.TrackLengthSum:
					oStatisticTitle.Text = "Summe Tracklängen";
					//StatisticTrackLengthSum(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackCount:
					oStatisticTitle.Text = "Anzahl Tracks";
					//StatisticTrackCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackLengthLongest:
					oStatisticTitle.Text = "Längster Track";
					//StatisticTrackLongest(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackLengthShortest:
					oStatisticTitle.Text = "Kürzester Track";
					//StatisticTrackShortest(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackLengthAverage:
					oStatisticTitle.Text = "Tracklänge Durchschnitt";
					//StatisticTrackAverage(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackCountCDAverage:
					oStatisticTitle.Text = "Durchschnittliche Anzahl Tracks pro CD";
					//StatisticTrackCountCDAverage(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDCount:
					oStatisticTitle.Text = "Summe der CDs";
					//StatisticCDsCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDSamplerCount:
					oStatisticTitle.Text = "Summe Sampler CDs";
					//StatisticCDSamplerCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDTrackMax:
					oStatisticTitle.Text = "CD mit größter Anzahl von Tracks";
					//StatisticCDTrackMax(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDLengthAverage:
					oStatisticTitle.Text = "Durchschnittliche Länge der CD";
					//StatisticCDLengthAverage(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDLengthLongest:
					oStatisticTitle.Text = "Längste CD ";
					//StatisticCDLengthLongest(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDLengthShortest:
					oStatisticTitle.Text = "Kürzeste CD";
					//StatisticCDLengthShortest(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistCount:
					oStatisticTitle.Text = "Anzahl Interpreten";
					//StatisticArtistCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDSetsCount:
					oStatisticTitle.Text = "Anzahl der CDSets";
					//StatisticCDSetsCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDAssignedCount:
					oStatisticTitle.Text = "Anzahl der zugewiesenen CDs";
					//StatisticCDAssignedCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDAssignedNotCount:
					oStatisticTitle.Text = "Anzahl der nicht zugewiesenen CDs";
					//StatisticCDAssignedNotCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDLoanedCount:
					oStatisticTitle.Text = "Anzahl der ausgeliehenen CDs";
					//StatisticCDLoanedCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDTotalValue:
					oStatisticTitle.Text = "Gesamtwert der CD Sammlung";
					//StatisticCDTotalValue(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByCategoryCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Kategorie";
					StatisticCDGroupByCategoryCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupyMediumCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Medium";
					StatisticCDGroupyMediumCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupyPriceCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Preis";
					StatisticCDGroupyPriceCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByNumberOfTracksCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Trackanzahl";
					StatisticCDGroupByNumberOfTracksCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByLengthCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Gesamtlänge";
					StatisticCDGroupByLengthCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupBySamplerCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Sampler / Kein Sampler";
					StatisticCDGroupBySamplerCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByRatingCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Bewertung";
					StatisticCDGroupByRatingCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByAttributeCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Kennzeichen";
					StatisticCDGroupByAttributeCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByLabelCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Label";
					StatisticCDGroupByLabelCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByRecordCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Aufnahmejahr";
					StatisticCDGroupByRecordCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByArtistArtCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Interpretenart";
					StatisticCDGroupByArtistArtCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByArtistSexCount:
					oStatisticTitle.Text = "Anzahl der CDs gruppiert nach Geschlecht";
					StatisticCDGroupByArtistSexCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.CDGroupByCountryCount:
					oStatisticTitle.Text = "Anzahl CDs gruppiert nach Land";
					StatisticCDGroupByCountryCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackGroupByLengthCount:
					oStatisticTitle.Text = "Anzahl Tracks gruppiert nach Länge";
					StatisticTrackGroupByLengthCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackGropupByRecordCount:
					oStatisticTitle.Text = "Anzahl Tracks gruppiert nach Aufnahmejahr";
					StatisticTrackGropupByRecordCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackGroupByRating:
					oStatisticTitle.Text = "Anzahl Tracks gruppiert nach Bewertung";
					StatisticTrackGroupByRating(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackGroupByBPMCount:
					oStatisticTitle.Text = "Anzahl Tracks gruppiert nach BPM";
					StatisticTrackGroupByBPMCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistTrackMost:
					oStatisticTitle.Text = "Die meisten Tracks eines Interpreten";
					StatisticArtistTrackMost(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistTrackGroupBySexCount:
					oStatisticTitle.Text = "Gruppiert nach Geschlecht von Interpreten (Tracks)";
					StatisticArtistTrackGroupBySexCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.TrackGroupByAttributeCount:
					oStatisticTitle.Text = "Anzahl Tracks gruppiert nach Kennzeichen von Tracks";
					StatisticTrackGroupByAttributeCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistTrackGroupByCountryCount:
					oStatisticTitle.Text = "Gruppiert nach Ländern (Tracks)";
					StatisticArtistTrackGroupByCountryCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistTrackGroupByArtistArtCount:
					oStatisticTitle.Text = "Gruppiert nach Art des Interpreten (Tracks)";
					StatisticArtistTrackGroupByArtistArtCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistCDsMost:
					oStatisticTitle.Text = "Die meisten CDs eines Interpreten";
					StatisticArtistCDsMost(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistGroupBySexCount:
					oStatisticTitle.Text = "Gruppiert nach Geschlecht eines Interpreten (CDs)";
					StatisticArtistGroupBySexCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistGroupByCountryCount:
					oStatisticTitle.Text = "Gruppiert nach Ländern (CDs)";
					StatisticArtistGroupByCountryCount(oStatisticSeries, chartArea1);
					break;

				case Statistic.ArtistGroupByArtistArtCount:
					oStatisticTitle.Text = "Gruppiert nach Art des Interpreten (CDs)";
					StatisticArtistGroupByArtistArtCount(oStatisticSeries, chartArea1);
					break;

				default:
					break;
			}

			SetStatisticStyle(oStatisticSeries, oStatisticTitle, 0);
		}

		private void SetStatisticStyle(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.Title oStatisticTitle, int CurrentIndex)
		{
			// Set the Titles
			oGraphOverview[CurrentIndex].Titles.Add(oStatisticTitle);

			if (ChartStyle == StatisticChartTypes.Bar)
			{
				// Set Cylinder drawing style
				oGraphOverview[CurrentIndex].Series["Default"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
				// Enable 3D charts
				oGraphOverview[CurrentIndex].ChartAreas["Default"].Area3DStyle.Enable3D = true;
				oGraphOverview[CurrentIndex].Series["Default"]["DrawingStyle"] = "Cylinder";
			}
			else if (ChartStyle == StatisticChartTypes.Line)
			{
				// Enable 3D charts
				oGraphOverview[CurrentIndex].Series["Default"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

				// disable 3D charts
				oGraphOverview[CurrentIndex].ChartAreas["Default"].Area3DStyle.Enable3D = false;
				oGraphOverview[CurrentIndex].Series["Default"]["DrawingStyle"] = "";

				oGraphOverview[CurrentIndex].Series["Default"]["LineWidth"] = "4";
			}
			else if (ChartStyle == StatisticChartTypes.Pie)
			{
				// Enable 3D charts
				oGraphOverview[CurrentIndex].Series["Default"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

				oGraphOverview[CurrentIndex].Series["Default"]["PieLabelStyle"] = "Outside";
				oGraphOverview[CurrentIndex].Series["Default"]["PieDrawingStyle"] = "SoftEdge";
				oStatisticSeries.CustomProperties = "PieLabelStyle=Outside, PieDrawingStyle=SoftEdge";
			}
			else if (ChartStyle == StatisticChartTypes.Donut)
			{
				// set chart type
				oGraphOverview[CurrentIndex].Series["Default"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;

				oGraphOverview[CurrentIndex].Series["Default"]["PieLabelStyle"] = "Outside";

				oGraphOverview[CurrentIndex].Series["Default"]["DoughnutRadius"] = "50";

				// Disable 3D
				oGraphOverview[CurrentIndex].ChartAreas["Default"].Area3DStyle.Enable3D = false;

				// Set drawing style
				oGraphOverview[CurrentIndex].Series["Default"]["PieDrawingStyle"] = "SoftEdge";
			}
		}

		//-------------- end of event handlers ------------------------------

		private void StatisticOverview()
		{
			Int16 CounterX;
			Int16 CounterY;
			int LocationGraphX;
			int LocationGraphY;
			int SizeGraphX;
			int SizeGraphY;
			int CurrentIndex;

			LocationGraphX = 290;
			LocationGraphY = 49;
			SizeGraphX = (this.Size.Width - LocationGraphX) / 3;
			SizeGraphY = (this.Size.Height - LocationGraphY) / 3 - 20;

			for (CounterY = 0; CounterY < 3; CounterY++)
			{
				for (CounterX = 0; CounterX < 3; CounterX++)
				{
					CurrentIndex = (CounterY * 3) + CounterX;

					oChartArea[CurrentIndex] = new ChartArea();
					oChartLegend[CurrentIndex] = new Legend();
					oChartSeries[CurrentIndex] = new Series();
					oChartTitle[CurrentIndex] = new Title();

					oGraphOverview[CurrentIndex].Dock = DockStyle.Fill;

					CreateGraph(oGraphOverview[CurrentIndex], oChartArea[CurrentIndex], oChartLegend[CurrentIndex], oChartSeries[CurrentIndex], oChartTitle[CurrentIndex]);

					oChartTitle[CurrentIndex].Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
					oChartTitle[CurrentIndex].ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
					oChartTitle[CurrentIndex].Name = "Title1";
					oChartTitle[CurrentIndex].ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
					oChartTitle[CurrentIndex].ShadowOffset = 3;

					switch (CurrentIndex)
					{
						case 1:
							oChartTitle[CurrentIndex].Text = "Anzahl CDs gruppiert nach Preis";
							StatisticCDGroupyPriceCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 0:
							oChartTitle[CurrentIndex].Text = "Anzahl CDs gruppiert nach Label";
							StatisticCDGroupByLabelCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 2:
							oChartTitle[CurrentIndex].Text = "Anzahl der CDs gruppiert nach Kategorie";
							StatisticCDGroupByCategoryCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 3:
							oChartTitle[CurrentIndex].Text = "Anzahl Tracks gruppiert nach Länge";
							StatisticTrackGroupByLengthCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 4:
							oChartTitle[CurrentIndex].Text = "Die meisten CDs eines Interpreten";
							StatisticArtistCDsMost(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 5:
							oChartTitle[CurrentIndex].Text = "Anzahl der CDs gruppiert nach Kennzeichen";
							StatisticCDGroupByAttributeCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 6:
							oChartTitle[CurrentIndex].Text = "Die meisten Tracks eines Interpreten";
							StatisticArtistTrackMost(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 7:
							oChartTitle[CurrentIndex].Text = "Anzahl der CDs gruppiert nach Medium";
							StatisticCDGroupyMediumCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;
						case 8:
							oChartTitle[CurrentIndex].Text = "Anzahl der CDs gruppiert nach Bewertung";
							StatisticCDGroupByRatingCount(oChartSeries[CurrentIndex], oChartArea[CurrentIndex]);
							break;

						default:
							break;
					}

					SetStatisticStyle(oChartSeries[CurrentIndex], oChartTitle[CurrentIndex], CurrentIndex);
				}
			}
		}

		private void StatisticTrackLengthSum(out Int64 Value)
		{
			// SUM kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
			DataTable dt = dataBase.ExecuteFreeSql("SELECT Length FROM Track where Length > 0");
			Int64 length = 0;
			foreach (DataRow dr in dt.Rows)
				length += (int)dr["Length"];

			Value = length;
		}

		private void StatisticTrackCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT COUNT(1) FROM Track");
		}

		private void StatisticTrackLongest(out Int32 Value)
		{
			object val = dataBase.ExecuteScalar("SELECT MAX(Length) FROM Track");
			if (val is DBNull)
				Value = 0;
			else
				Value = (int)val;
		}

		private void StatisticTrackShortest(out Int32 Value)
		{
			object val = dataBase.ExecuteScalar("SELECT MIN(Length) FROM Track where Length > 0");

			if (val is DBNull)
				Value = 0;
			else
				Value = (int)val;
		}

		private void StatisticTrackAverage(out Int64 Value)
		{
			// AVG kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
			DataTable dt = dataBase.ExecuteFreeSql("SELECT Length FROM Track where Length > 0");
			Int64 length = 0;
			foreach (DataRow dr in dt.Rows)
				length += (int)dr["Length"];

			if (dt.Rows.Count != 0)
				Value = length / dt.Rows.Count;
			else
				Value = 0;
		}

		private void StatisticTrackCountCDAverage(out double Value)
		{
			// AVG kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
			DataTable dt = dataBase.ExecuteFreeSql("SELECT NumberOfTracks FROM cd");
			Int64 numberOfTracks = 0;
			foreach (DataRow dr in dt.Rows)
				numberOfTracks += (int)dr["NumberOfTracks"];

			if (dt.Rows.Count != 0)
				Value = (double)numberOfTracks / (double)dt.Rows.Count;
			else
				Value = 0;
		}

		private void StatisticCDsCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT count(1) FROM cd");
		}

		private void StatisticCDSamplerCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT count(1) FROM cd where IsSampler <> 0");
		}

		private void StatisticCDTrackMax(out Int32 Value)
		{
			object val = dataBase.ExecuteScalar("SELECT MAX(NumberOfTracks) FROM cd");

			if (val is DBNull)
				Value = 0;
			else
				Value = (int)val;
		}

		private void StatisticCDLengthAverage(out Int64 Value)
		{
			// AVG kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
			DataTable dt = dataBase.ExecuteFreeSql("SELECT Length FROM cd where Length > 0");
			Int64 length = 0;
			foreach (DataRow dr in dt.Rows)
				length += (int)dr["Length"];

			if (dt.Rows.Count != 0)
				Value = length / dt.Rows.Count;
			else
				Value = 0;
		}

		private void StatisticCDLengthLongest(out Int32 Value)
		{
			object val = dataBase.ExecuteScalar("SELECT MAX(Length) FROM cd");

			if (val is DBNull)
				Value = 0;
			else
				Value = (int)val;
		}

		private void StatisticCDLengthShortest(out Int32 Value)
		{
			object val = dataBase.ExecuteScalar("SELECT min(Length) FROM cd where Length > 0");

			if (val is DBNull)
				Value = 0;
			else
				Value = (int)val;
		}

		private void StatisticArtistCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT count(1) FROM PersonGroup WHERE PersonGroupID IN (SELECT ArtistID FROM Track)");
		}

		private void StatisticCDSetsCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT count(1) FROM [Set]");
		}

		private void StatisticCDAssignedCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT COUNT(1) FROM CD WHERE [Identity]<>''");
		}

		private void StatisticCDAssignedNotCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT Count(1) FROM CD WHERE [identity]='' or [identity] is null");
		}

		private void StatisticCDLoanedCount(out Int32 Value)
		{
			Value = (int)dataBase.ExecuteScalar("SELECT count (1) FROM LoanedCD");
		}

		private void StatisticCDTotalValue(out Int32 Value)
		{
			// SUM kann hier nicht benutzt werden, da diese Int32 ist und schnell einen Overflow erzeugt.
			DataTable dt = dataBase.ExecuteFreeSql("SELECT PRICE FROM CD");
			Int32 length = 0;
			foreach (DataRow dr in dt.Rows)
			{
				if (!dr.IsNull("PRICE"))
					length += (int)dr["PRICE"];
			}

			Value = length;
		}

		private void StatisticCDGroupByCategoryCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(Category.CategoryID) AS AnzahlvonIDKategorie, Category.Name " +
											 "FROM CD " +
											"INNER JOIN Category ON Category.CategoryID = CD.CategoryID " +
											"GROUP BY CD.CategoryID, Category.Name;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["AnzahlvonIDKategorie"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["Name"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["Name"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupyMediumCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Sum(1) AS Summe, Medium.Name As ColName " +
											 "FROM Medium INNER JOIN CD ON Medium.MediumID = CD.MediumID " +
											"GROUP BY Medium.Name;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Summe"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupyPriceCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;
			String ValueFrom;
			String ValueTo;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			//cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, ('von ' & ((PRICE/500)*5) & ' bis ' & ((PRICE/500)*5) + 5) As ColName " +
			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, (PRICE/500) As ColName " +
											 "FROM CD " +
											"GROUP BY (PRICE / 500);", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;

				ValueFrom = Misc.FormatCurrencyValue((nValueX - 1) * 500);
				ValueTo = ValueFrom + " bis " + Misc.FormatCurrencyValue(nValueX * 500 - 1);

				oChartLabel.Text = ValueTo + " (" + nValueY.ToString() + ")";

				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = ValueTo;// dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByNumberOfTracksCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, 'Anzahl Tracks <= 5', '1' as SORTORDER " +
											"FROM CD " +
											"WHERE (CD.NumberOfTracks <= 5) " +

											"UNION " +

											"SELECT Count(1) As Anzahl, 'Anzahl Tracks > 5 und <= 10', '2' as SORTORDER " +
											"FROM CD " +
											"WHERE (CD.NumberOfTracks > 5 And CD.NumberOfTracks <= 10) " +

											"UNION " +

											"SELECT Count(1) As Anzahl, 'Anzahl Tracks > 10 und <= 15', '3' as SORTORDER " +
											"FROM CD " +
											"WHERE (CD.NumberOfTracks > 10 And CD.NumberOfTracks <= 15) " +

											"UNION " +

											"SELECT Count(1) As Anzahl, 'Anzahl Tracks > 15 und <= 20', '4' as SORTORDER " +
											"FROM CD " +
											"WHERE (CD.NumberOfTracks > 15 And CD.NumberOfTracks <= 20) " +

											"UNION " +

											"SELECT Count(1) As Anzahl, 'Anzahl Tracks > 20', '5' as SORTORDER " +
											"FROM CD " +
											"WHERE (CD.NumberOfTracks > 20) " +
											"order by SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr[1].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr[1].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByLengthCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, 'Länge <= 5 Min.' As Colname, 1 as SORTORDER " +
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
									  "order by SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];
				if (nValueY <= 0)
					continue;

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupBySamplerCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, 'Sampler' as Beschreibung, 1 as SORTORDER " +
									  "FROM cd " +
									  "WHERE cd.IsSampler=1 " +

									  "UNION " +

									  "SELECT Count(1) As Anzahl, 'Kein Sampler' as Beschreibung, 2 as SORTORDER " +
									  "FROM cd " +
									  "WHERE cd.IsSampler=0 " +

									  "order by SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["Beschreibung"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["Beschreibung"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByRatingCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, Rating " +
											 "FROM cd " +
											"GROUP BY CD.Rating;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["Rating"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["Rating"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByAttributeCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, CD.Codes " +
											 "FROM CD " +
											"GROUP BY CD.Codes " +
										  "HAVING CD.Codes <> '';", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["Codes"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["Codes"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByLabelCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;
			Int32 Counter;
			Int32 ValueDiverse;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, CD.Label As ColName " +
											 "FROM CD " +
											"GROUP BY CD.Label " +
											" " +
										  "HAVING CD.Label <> '' ORDER BY Anzahl desc;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;
			nValueY = 0;
			Counter = 0;
			ValueDiverse = 0;

			while (dr.Read())
			{
				Counter++;
				nValueY = (Int32)dr["Anzahl"];
				if (Counter > 10)
				{
					ValueDiverse += (Int32)nValueY;
					continue;
				}

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			if (ValueDiverse > 0)
			{
				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, ValueDiverse);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = "Sonstige Labels" + "(Anzahl: " + ValueDiverse.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = "Sonstige Labels";
				oGraphicPoint.nValue = ValueDiverse;
				oTable.Add(oGraphicPoint);
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByRecordCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, '>= 1960 und < 1970' As ColName, 1 as SORTORDER " +
									  "FROM CD " +
									  "WHERE CD.YearRecorded >= 1960 AND CD.YearRecorded < 1970 " +

									  "UNION " +

									  "SELECT Count(1) As Anzahl, '>= 1970 und < 1980' As ColName, 1 as SORTORDER " +
									  "FROM CD " +
									  "WHERE CD.YearRecorded >= 1970 AND CD.YearRecorded < 1980 " +

									  "UNION " +

									  "SELECT Count(1) As Anzahl, '>= 1980 und < 1990' As ColName, 1 as SORTORDER " +
									  "FROM CD " +
									  "WHERE CD.YearRecorded >= 1980 AND CD.YearRecorded < 1990 " +

									  "UNION " +

									  "SELECT Count(1) As Anzahl, '>= 1990 und < 2000' As ColName, 2 as SORTORDER " +
									  "FROM CD " +
									  "WHERE CD.YearRecorded >= 1990 AND CD.YearRecorded < 2000 " +

									  "UNION " +

									  "SELECT Count(1) As Anzahl, '>= 2000 und < 2010' As ColName, 3 as SORTORDER " +
									  "FROM CD " +
									  "WHERE CD.YearRecorded >= 2000 AND CD.YearRecorded < 2010 " +

									  "ORDER BY SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				if (nValueY <= 0)
					continue;

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByArtistArtCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, 'Unbekannt' As ColName, 1 as SORTORDER " +
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

									  "ORDER BY SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				if (nValueY <= 0)
					continue;

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByArtistSexCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, 'Unbekannt' As ColName, 1 as SORTORDER " +
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

									  "ORDER BY SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				if (nValueY <= 0)
					continue;

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticCDGroupByCountryCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) As Anzahl, 'Keine Landzuweisung' As ColName, 1 as SORTORDER " +
									  "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
									  "WHERE PersonGroup.Country = '' or PersonGroup.Country is null " +

									  "UNION " +

									  "SELECT Count(1), PersonGroup.Country As ColName, 2 as SORTORDER " +
									  "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
									  "GROUP BY PersonGroup.Country " +
									  "HAVING PersonGroup.Country <> '' " +

									  "ORDER BY SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticTrackGroupByLengthCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(Track.Length) As Anzahl, '> 0 Sek. und <= 30 Sek.' As ColName, 1 as SORTORDER " +
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

								  "ORDER BY SORTORDER", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticTrackGropupByRecordCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, (YearRecorded / 10) AS ColName " +
									"FROM Track " +
								  "WHERE YearRecorded > 0 " +
								  "GROUP BY (YearRecorded / 10);", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];
				if (nValueY <= 0)
					continue;

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;

				string s = string.Format("von {0} bis {1}", (int)dr["ColName"] * 10, (int)dr["ColName"] * 10 + 10) + " (" + nValueY.ToString() + ")";

				oChartLabel.Text = s;
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = s;
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticTrackGroupByRating(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT count(1) As Anzahl, Rating As ColName " +
											 "FROM Track " +
											"GROUP BY Rating;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticTrackGroupByBPMCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, (Bpm / 10) AS ColName " +
											 "FROM Track " +
											"WHERE Bpm > 0 " +
											"GROUP BY (Bpm / 10);", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				string s = string.Format("von {0} bis {1}", (int)dr["ColName"] * 10, (int)dr["ColName"] * 10 + 10);

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistTrackMost(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT TOP (10) Count(1) AS Anzahl, PersonGroup.Name As ColName " +
											 "FROM PersonGroup INNER JOIN Track ON PersonGroup.PersonGroupID = Track.ArtistID " +
											"GROUP BY PersonGroup.Name " +
										  "HAVING (((PersonGroup.Name)<>'' And (PersonGroup.Name) Is Not Null)) " +
											"ORDER BY Anzahl DESC;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistTrackGroupBySexCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, PersonGroup.Sex as ColName " +
											 "FROM PersonGroup, Track " +
											"WHERE (((PersonGroup.PersonGroupID)=[Track].[ArtistID])) " +
											"GROUP BY PersonGroup.Sex;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticTrackGroupByAttributeCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, Code.Letter As ColName " +
											 "FROM Code, Track " +
											"WHERE (((Track.Codes) Like '*'+[Letter]+'*')) " +
											"GROUP BY Code.Letter;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistTrackGroupByCountryCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, PersonGroup.Country As ColName " +
											 "FROM PersonGroup, Track " +
											"WHERE (((PersonGroup.PersonGroupID)=[Track].[ArtistID])) AND PersonGroup.Country <> '' " +
											"GROUP BY PersonGroup.Country", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistTrackGroupByArtistArtCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, PersonGroup.Type As ColName " +
											 "FROM PersonGroup, Track " +
											"WHERE (((PersonGroup.PersonGroupID)=[Track].[ArtistID])) " +
											"GROUP BY PersonGroup.Type;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistCDsMost(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT TOP (10) Count(1) AS Anzahl, PersonGroup.Name As ColName " +
											 "FROM PersonGroup INNER JOIN CD ON PersonGroup.PersonGroupID = CD.ArtistID " +
											"GROUP BY PersonGroup.Name " +
										  "HAVING (((PersonGroup.Name)<>'' And (PersonGroup.Name) Is Not Null)) " +
											"ORDER BY Anzahl DESC;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistGroupBySexCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, PersonGroup.Sex As ColName " +
											 "FROM PersonGroup " +
											"GROUP BY PersonGroup.Sex;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistGroupByCountryCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, PersonGroup.Country As ColName " +
											 "FROM PersonGroup " +
											"WHERE PersonGroup.Country <> '' " +
											"GROUP BY PersonGroup.Country;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void StatisticArtistGroupByArtistArtCount(System.Windows.Forms.DataVisualization.Charting.Series oStatisticSeries, System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1)
		{
			SqlCeCommand cmd;
			SqlCeDataReader dr;
			Int32 nValueX;
			Double nValueY;

			List<GraphicPoint> oTable = new List<GraphicPoint>();

			cmd = new SqlCeCommand("SELECT Count(1) AS Anzahl, PersonGroup.Type As ColName " +
											 "FROM PersonGroup " +
											"GROUP BY PersonGroup.Type;", dataBase.Connection);
			dr = cmd.ExecuteReader();
			nValueX = 1;

			while (dr.Read())
			{
				nValueY = (Int32)dr["Anzahl"];

				System.Windows.Forms.DataVisualization.Charting.DataPoint oHitbaseDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0, nValueY);
				System.Windows.Forms.DataVisualization.Charting.CustomLabel oChartLabel = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();

				oChartLabel.FromPosition = (double)nValueX - 0.5;
				oChartLabel.ToPosition = (double)nValueX + 0.5;
				oChartLabel.Text = dr["ColName"].ToString() + " (" + nValueY.ToString() + ")";
				chartArea1.AxisX.CustomLabels.Add(oChartLabel);

				GraphicPoint oGraphicPoint = new GraphicPoint();
				oGraphicPoint.sName = dr["ColName"].ToString();
				oGraphicPoint.nValue = nValueY;
				oTable.Add(oGraphicPoint);

				// if curve is selected => set line width to 5
				if (ChartStyle == StatisticChartTypes.Line)
				{
					oHitbaseDataPoint.BorderWidth = 5;
				}

				//oStatisticSeries.Points.Add(oHitbaseDataPoint);

				nValueX++;
			}

			FillGraphicsPoints(oStatisticSeries, oTable);

			return;
		}

		private void FillGraphicsPoints(System.Windows.Forms.DataVisualization.Charting.Series oSeries, List<GraphicPoint> oGraphicPoint)
		{
			Int32 nValueX;

			nValueX = 0;
			String[] xValues = new String[oGraphicPoint.Count];
			Double[] yValues = new Double[oGraphicPoint.Count];
			foreach (GraphicPoint gp in oGraphicPoint)
			{
				xValues[(int)nValueX] = (String)gp.sName;
				yValues[(int)nValueX] = Convert.ToDouble(gp.nValue.ToString());
				nValueX++;
			}

			oSeries.Points.DataBindXY(xValues, yValues);

		}

		private void printToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Statistik Übersichtsseite
			//Chart printerChart = new System.Windows.Forms.DataVisualization.Charting.Chart();

			oGraphOverview[0].Printing.PrintDocument = new PrintDocument();
			oGraphOverview[0].Printing.Print(true);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// create a memory stream to save the chart image    
			System.IO.MemoryStream stream = new System.IO.MemoryStream();

			// save the chart image to the stream    
			oGraphOverview[0].SaveImage(stream, System.Drawing.Imaging.ImageFormat.Bmp);

			// create a bitmap using the stream    
			Bitmap bmp = new Bitmap(stream);

			// save the bitmap to the clipboard    
			Clipboard.SetDataObject(bmp);
		}

		private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Print preview chart
			oGraphOverview[0].Printing.PrintPreview();
		}

		private void oGraphControl_GetToolTipText(object sender, ToolTipEventArgs e)
		{

			// Check selevted chart element and set tooltip text
			switch (e.HitTestResult.ChartElementType)
			{
				//case ChartElementType.Axis:
				//    e.Text = e.HitTestResult.Axis.Name;
				//    break;
				//case ChartElementType.ScrollBarLargeDecrement:
				//    e.Text = "A scrollbar large decrement button";
				//    break;
				//case ChartElementType.ScrollBarLargeIncrement:
				//    e.Text = "A scrollbar large increment button";
				//    break;
				//case ChartElementType.ScrollBarSmallDecrement:
				//    e.Text = "A scrollbar small decrement button";
				//    break;
				//case ChartElementType.ScrollBarSmallIncrement:
				//    e.Text = "A scrollbar small increment button";
				//    break;
				//case ChartElementType.ScrollBarThumbTracker:
				//    e.Text = "A scrollbar tracking thumb";
				//    break;
				//case ChartElementType.ScrollBarZoomReset:
				//    e.Text = "The ZoomReset button of a scrollbar";
				//    break;
				case ChartElementType.DataPoint:
//					e.Text = "Anzahl '" + e.HitTestResult.ChartArea.AxisX.CustomLabels[e.HitTestResult.PointIndex].Text + "': " + ((System.Windows.Forms.DataVisualization.Charting.DataPoint)e.HitTestResult.Object).YValues[0];//.PointIndex.ToString();
					e.Text = e.HitTestResult.ChartArea.AxisX.CustomLabels[e.HitTestResult.PointIndex].Text;
					break;
				//case ChartElementType.Gridlines:
				//    e.Text = "Grid Lines";
				//    break;
				//case ChartElementType.LegendArea:
				//    e.Text = "Legend Area";
				//    break;
				//case ChartElementType.LegendItem:
				//    e.Text = "Legend Item";
				//    break;
				//case ChartElementType.PlottingArea:
				//    e.Text = "Plotting Area";
				//    break;
				//case ChartElementType.StripLines:
				//    e.Text = "Strip Lines";
				//    break;
				//case ChartElementType.TickMarks:
				//    e.Text = "Tick Marks";
				//    break;
				//case ChartElementType.Title:
				//    e.Text = "Title";
				//    break;
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void buttonShow_Click(object sender, EventArgs e)
		{
			FormCatalog formCatalog = new FormCatalog(dataBase, null);

			Condition cond = new Condition();
			cond.Add(Field.TrackLength, Operator.Equal, 4849389);

			formCatalog.Condition = cond;

			formCatalog.ShowDialog();
		}

		private void FormStatistics_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.SaveWindowPlacement(this, "Statistics");
		}

		private void trvStatistics_AfterSelect(object sender, TreeViewEventArgs e)
		{
			StaticticsRedraw();
		}

		private void trvStatistics_Click(object sender, EventArgs e)
		{
			//StaticticsRedraw();
		}

		private void StatistikPie_Click(object sender, EventArgs e)
		{
			ChartStyle = StatisticChartTypes.Pie;

			UpdateWindowsState();
			StaticticsRedraw();
		}

		private void StatisticLine_Click(object sender, EventArgs e)
		{
			ChartStyle = StatisticChartTypes.Line;
			UpdateWindowsState();
			StaticticsRedraw();
		}

		private void StatisticBar_Click(object sender, EventArgs e)
		{
			ChartStyle = StatisticChartTypes.Bar;
			UpdateWindowsState();
			StaticticsRedraw();
		}

		private void UpdateWindowsState()
		{
			StatisticLine.Checked = (ChartStyle == StatisticChartTypes.Line);
			StatistikPie.Checked = (ChartStyle == StatisticChartTypes.Pie);
			StatisticBar.Checked = (ChartStyle == StatisticChartTypes.Bar);
			StatisticDonut.Checked = (ChartStyle == StatisticChartTypes.Donut);
		}

		private void StatistikAnzeigen_Click(object sender, EventArgs e)
		{
			ShowSelectedStatisticInCatalog();
		}

		private void ShowSelectedStatisticInCatalog()
		{
			bool track;
			Condition cond = GetConditionFromStatistic(out track);

			if (cond == null)
				return;

			FormCatalog formCatalog = new FormCatalog(dataBase, cond);
			formCatalog.CurrentListType = track ? FormCatalog.ListType.TrackList : FormCatalog.ListType.CDList;
			formCatalog.ShowDialog();
		}



		private Condition GetConditionFromStatistic(out bool track)
		{
			Condition cond = new Condition();
			track = false;

			if (SimpleStatistics.SelectedIndices.Count < 1)
				return null;

			switch (SimpleStatistics.SelectedIndices[0])
			{
				case 2:
					// longest track
					cond.Add(Field.TrackLength, Operator.Equal, SimpleStatistics.Items[SimpleStatistics.SelectedIndices[0]].Tag);
					track = true;
					break;

				case 3:
					// shortest track
					cond.Add(Field.TrackLength, Operator.Equal, SimpleStatistics.Items[SimpleStatistics.SelectedIndices[0]].Tag);
					track = true;
					break;

				case 7:
					// counter sampler CDs
					cond.Add(Field.Sampler, Operator.Equal, 1);
					track = false;
					break;

				case 8:
					// Cd with greatest number of tracks
					cond.Add(Field.TrackNumber, Operator.Equal, SimpleStatistics.Items[SimpleStatistics.SelectedIndices[0]].Tag);
					track = true;
					break;

				case 10:
					// longest cd
					cond.Add(Field.TotalLength, Operator.Equal, SimpleStatistics.Items[SimpleStatistics.SelectedIndices[0]].Tag);
					break;

				case 11:
					// shortest cd
					cond.Add(Field.TotalLength, Operator.Equal, SimpleStatistics.Items[SimpleStatistics.SelectedIndices[0]].Tag);
					break;

				case 14:
					// number of not assigned cds
					cond.Add(Field.Identity, Operator.Equal, "");
					//cond.Add(Field.Identity, Operator.Equal, null);
					break;

				case 15:
					// number of assigned cds
					cond.Add(Field.Identity, Operator.NotEqual, "");
					break;
			}

			return cond;
		}

		private void SimpleStatistics_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SimpleStatistics.SelectedIndices.Count <= 0)
			{
				StatistikAnzeigen.Enabled = false;
				return;
			}

			if (SimpleStatistics.SelectedIndices[0] == 2 ||
				 SimpleStatistics.SelectedIndices[0] == 3 ||
				 SimpleStatistics.SelectedIndices[0] == 7 ||
				 SimpleStatistics.SelectedIndices[0] == 8 ||
				 SimpleStatistics.SelectedIndices[0] == 10 ||
				 SimpleStatistics.SelectedIndices[0] == 11 ||
				 SimpleStatistics.SelectedIndices[0] == 14 ||
				 SimpleStatistics.SelectedIndices[0] == 15)
			{
				StatistikAnzeigen.Enabled = true;
			}
			else
			{
				StatistikAnzeigen.Enabled = false;
			}
		}

		private void SimpleStatistics_DoubleClick(object sender, EventArgs e)
		{
			ShowSelectedStatisticInCatalog();
		}

		private void kopierenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// create a memory stream to save the chart image    
			System.IO.MemoryStream stream = new System.IO.MemoryStream();


			ToolStripDropDownItem menu = (ToolStripDropDownItem)sender;
			ContextMenuStrip strip = (ContextMenuStrip)menu.Owner;
			Chart owner = (Chart)strip.SourceControl;

			// save the chart image to the stream    
			owner.SaveImage(stream, System.Drawing.Imaging.ImageFormat.Bmp);

			// create a bitmap using the stream    
			Bitmap bmp = new Bitmap(stream);

			// save the bitmap to the clipboard    
			Clipboard.SetDataObject(bmp);
		}

		private void druckenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripDropDownItem menu = (ToolStripDropDownItem)sender;
			ContextMenuStrip strip = (ContextMenuStrip)menu.Owner;
			Chart owner = (Chart)strip.SourceControl;

			owner.Printing.PrintDocument = new PrintDocument();
			owner.Printing.Print(true);
			//owner.Printing.PrintDocument = null;
		}

		private void druckvorschauToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripDropDownItem menu = (ToolStripDropDownItem)sender;
			ContextMenuStrip strip = (ContextMenuStrip)menu.Owner;
			Chart owner = (Chart)strip.SourceControl;

			// Print preview chart
			owner.Printing.PrintPreview();
			//owner.Printing.PrintDocument = null;
		}

		private void schnelldruckToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripDropDownItem menu = (ToolStripDropDownItem)sender;
			ContextMenuStrip strip = (ContextMenuStrip)menu.Owner;
			Chart owner = (Chart)strip.SourceControl;

			owner.Printing.PrintDocument = new PrintDocument();
			owner.Printing.Print(false);
			//owner.Printing.PrintDocument = null;
		}

		private void StatisticDonut_Click(object sender, EventArgs e)
		{
			ChartStyle = StatisticChartTypes.Donut;
			UpdateWindowsState();
			StaticticsRedraw();
		}

		private void StatisticZoomIn_Click(object sender, EventArgs e)
		{
			ZommLevelEnd++;

			//// Zoom into the X axis
			//oGraphOverview[0].ChartAreas["Default"].AxisX.ScaleView.Zoom(ZommLevelStart, ZommLevelEnd);

			//// Enable range selection and zooming end user interface
			//oGraphOverview[0].ChartAreas["Default"].CursorX.IsUserEnabled = true;
			//oGraphOverview[0].ChartAreas["Default"].CursorX.IsUserSelectionEnabled = true;
			//oGraphOverview[0].ChartAreas["Default"].AxisX.ScaleView.Zoomable = true;
			//oGraphOverview[0].ChartAreas["Default"].AxisX.ScrollBar.IsPositionedInside = true;

			// Set automatic zooming
			oGraphOverview[0].ChartAreas["Default"].AxisY.ScaleView.Zoomable = true;

			oGraphOverview[0].ChartAreas["Default"].AxisY.ScaleView.Zoom(0, 1);

		}

	}
}
