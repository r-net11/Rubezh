using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GKModule.ViewModels;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace GKModule.Views
{
	public partial class PlotView
	{
		public PlotView()
		{
			InitializeComponent();
			Loaded += Window1_Loaded;
		}

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			var plotViewModel = DataContext as PlotViewModel;
			if (plotViewModel == null)
				return;
			plotViewModel.PlotViewUpdateAction = Update;
		}

		void Update()
		{
			var plotViewModel = DataContext as PlotViewModel;
			if (plotViewModel == null)
				return;
			var restr = new ViewportAxesRangeRestriction();
			restr.YRange = new DisplayRange(-5, 105);

			plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
			plotter.Children.RemoveAll(typeof(LineGraph));

			plotter.Viewport.Restrictions.Add(restr);

			plotViewModel.CurrentConsumptions = plotViewModel.CurrentConsumptions.OrderBy(x => x.DateTime).ToList();
			var dates = new DateTime[plotViewModel.CurrentConsumptions.Count];
			var curents = new int[plotViewModel.CurrentConsumptions.Count];

			for (int i = 0; i < plotViewModel.CurrentConsumptions.Count; ++i)
			{
				dates[i] = plotViewModel.CurrentConsumptions[i].DateTime;
				curents[i] = plotViewModel.CurrentConsumptions[i].Current;
			}

			var datesDataSource = new EnumerableDataSource<DateTime>(dates);
			datesDataSource.SetXMapping(dateAxis.ConvertToDouble);

			var currentsDataSource = new EnumerableDataSource<int>(curents);
			currentsDataSource.SetYMapping(Convert.ToDouble);

			CompositeDataSource compositeDataSource = new CompositeDataSource(datesDataSource, currentsDataSource);

			plotter.AddLineGraph(compositeDataSource,
					  new Pen(Brushes.Blue, 2),
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