﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace GKModule.Views
{
	public partial class PlotView
	{
		ViewportAxesRangeRestriction restr;
		DispatcherTimer updateCollectionTimer;

		public PlotView()
		{
			InitializeComponent();
			Loaded += Window1_Loaded;
		}

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			RingCurrentConsumptions = new RingCurrentConsumptions();
			var plotViewModel = DataContext as PlotViewModel;
			if (plotViewModel == null)
				return;

			updateCollectionTimer = new DispatcherTimer();
			updateCollectionTimer.Interval = TimeSpan.FromMilliseconds(100);
			updateCollectionTimer.Tick += Update;
			updateCollectionTimer.Start();

			plotViewModel.UpdateOnlineAction = UpdateOnline;
			plotViewModel.UpdateFromDBAction = UpdateFromDB;

			restr = new ViewportAxesRangeRestriction();
			restr.YRange = new DisplayRange(-5, 105);
			plotter.Viewport.Restrictions.Add(restr);

			var ds = new EnumerableDataSource<CurrentConsumption>(RingCurrentConsumptions);
			ds.SetXMapping(x => dateAxis.ConvertToDouble(x.DateTime));
			ds.SetYMapping(y => y.Current);
			plotter.AddLineGraph(ds, new Pen(Brushes.Green, 2), new CirclePointMarker { Size = 5.0, Fill = Brushes.Red }, new PenDescription("Статистика токопотребления"));
		}

		public RingCurrentConsumptions RingCurrentConsumptions { get; set; }
		public void Update(object sender, EventArgs e)
		{
			var plotViewModel = DataContext as PlotViewModel;
			if (plotViewModel == null)
				return;

			var measuresResult = FiresecManager.FiresecService.GetAlsMeasure(plotViewModel.DeviceUid);
			if (measuresResult == null)
				return;
			if (measuresResult.HasError)
			{
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
					MessageBoxService.Show(measuresResult.Error)));
				return;
			}
			RingCurrentConsumptions.Add(measuresResult.Result);
		}

		void UpdateOnline()
		{
			var plotViewModel = DataContext as PlotViewModel;
			if (plotViewModel == null)
				return;

		}


		void UpdateFromDB()
		{
			var plotViewModel = DataContext as PlotViewModel;
			if (plotViewModel == null)
				return;

			plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
			plotter.Children.RemoveAll(typeof(LineGraph));
			plotter.Viewport.Restrictions.Add(restr);
			var orderedCurrentConsumptions = new List<CurrentConsumption>();
			try
			{
				orderedCurrentConsumptions = plotViewModel.CurrentConsumptions.OrderBy(x => x.DateTime).ToList();
			}
			catch
			{
				
			}
			var dates = new DateTime[orderedCurrentConsumptions.Count];
			var curents = new int[orderedCurrentConsumptions.Count];

			for (int i = 0; i < orderedCurrentConsumptions.Count; ++i)
			{
				dates[i] = orderedCurrentConsumptions[i].DateTime;
				curents[i] = orderedCurrentConsumptions[i].Current;
			}

			var datesDataSource = new EnumerableDataSource<DateTime>(dates);
			datesDataSource.SetXMapping(dateAxis.ConvertToDouble);

			var currentsDataSource = new EnumerableDataSource<int>(curents);
			currentsDataSource.SetYMapping(Convert.ToDouble);

			CompositeDataSource compositeDataSource = new CompositeDataSource(datesDataSource, currentsDataSource);

			plotter.AddLineGraph(compositeDataSource,
					  new Pen(Brushes.Green, 2),
					  new CirclePointMarker { Size = 5.0, Fill = Brushes.Red },
					  new PenDescription("Статистика токопотребления"));

			plotter.Viewport.FitToView();
		}
	}

	public class DisplayRange
	{
		public double Start { get; set; }
		public double End { get; set; }

		public DisplayRange(double start, double end)
		{
			Start = start;
			End = end;
		}
	}

	public class ViewportAxesRangeRestriction : IViewportRestriction
	{
		public DisplayRange XRange = null;
		public DisplayRange YRange = null;

		public Rect Apply(Rect oldVisible, Rect newVisible, Viewport2D viewport)
		{
			if (XRange != null)
			{
				newVisible.X = XRange.Start;
				newVisible.Width = XRange.End - XRange.Start;
			}

			if (YRange != null)
			{
				newVisible.Y = YRange.Start;
				newVisible.Height = YRange.End - YRange.Start;
			}

			return newVisible;
		}

		public event EventHandler Changed;
	}
}