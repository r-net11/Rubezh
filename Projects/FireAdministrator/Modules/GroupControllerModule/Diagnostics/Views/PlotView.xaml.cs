using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace GKModule.Views
{
	public partial class PlotView : UserControl
	{
		public PlotView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
			restr.YRange = new DisplayRange(-5, 105);
			plotter.Viewport.Restrictions.Add(restr);

			var alsCurrents = new List<AlsCurrent>();
			alsCurrents.Add(new AlsCurrent() { DateTime = DateTime.Now.AddHours(-1), Current = 10 });
			alsCurrents.Add(new AlsCurrent() { DateTime = DateTime.Now.AddHours(-2), Current = 20 });
			alsCurrents.Add(new AlsCurrent() { DateTime = DateTime.Now.AddHours(-3), Current = 30 });

			DateTime[] dates = new DateTime[alsCurrents.Count];
			int[] curents = new int[alsCurrents.Count];

			for (int i = 0; i < alsCurrents.Count; ++i)
			{
				dates[i] = alsCurrents[i].DateTime;
				curents[i] = alsCurrents[i].Current;
			}

			var datesDataSource = new EnumerableDataSource<DateTime>(dates);
			datesDataSource.SetXMapping(x => dateAxis.ConvertToDouble(x));

			var currentsDataSource = new EnumerableDataSource<int>(curents);
			currentsDataSource.SetYMapping(y => Convert.ToDouble(y));

			CompositeDataSource compositeDataSource = new CompositeDataSource(datesDataSource, currentsDataSource);

			plotter.AddLineGraph(compositeDataSource,
					  new Pen(Brushes.Blue, 2),
					  new CirclePointMarker { Size = 10.0, Fill = Brushes.Red },
					  new PenDescription("Статистика токопотребления"));

			plotter.Viewport.FitToView();
		}
	}

	public class AlsCurrent
	{
		public DateTime DateTime { get; set; }
		public int Current { get; set; }
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