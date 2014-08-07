using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using SKDModule.ViewModels;
using System.Collections.Generic;
using System;

namespace SKDModule.Views
{
	public partial class TimeTrackDetailsView : UserControl
	{
		public TimeTrackDetailsView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(TimeTrackDetailsView_Loaded);
		}

		class TimePart
		{
			public double Delta { get; set; }
			public bool IsInterval { get; set; }
			public string Tooltip { get; set; }
		}

		string TimePartDateToString(DateTime dateTime)
		{
			var result = dateTime.TimeOfDay.Hours.ToString() + ":" + dateTime.TimeOfDay.Minutes.ToString() + ":" + dateTime.TimeOfDay.Seconds.ToString();
			return result;
		}

		void TimeTrackDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			TimeTrackDetailsViewModel timeTrackDetailsViewModel = DataContext as TimeTrackDetailsViewModel;
			if (timeTrackDetailsViewModel != null)
			{
				var dayTimeTrack = timeTrackDetailsViewModel.DayTimeTrack;
				var orderedTimeTrack = dayTimeTrack.TimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();
				if (orderedTimeTrack.Count > 0)
				{
					double current = 0;
					var timeParts = new List<TimePart>();
					for (int i = 0; i < orderedTimeTrack.Count; i++)
					{
						var timeTrackPart = orderedTimeTrack[i];

						var startTimePart = new TimePart();
						startTimePart.Delta = timeTrackPart.StartTime.TimeOfDay.TotalSeconds - current;
						startTimePart.IsInterval = false;
						timeParts.Add(startTimePart);

						var endTimePart = new TimePart();
						endTimePart.Delta = timeTrackPart.EndTime.TimeOfDay.TotalSeconds - timeTrackPart.StartTime.TimeOfDay.TotalSeconds;
						endTimePart.IsInterval = true;
						endTimePart.Tooltip = TimePartDateToString(timeTrackPart.EndTime) + " - " + TimePartDateToString(timeTrackPart.StartTime);
						timeParts.Add(endTimePart);

						current = timeTrackPart.EndTime.TimeOfDay.TotalSeconds;
					}
					var lastTimePart = new TimePart();
					lastTimePart.Delta = 24 * 60 * 60 - current;
					lastTimePart.IsInterval = false;
					timeParts.Add(lastTimePart);

					for (int i = 0; i < timeParts.Count; i++)
					{
						var timePart = timeParts[i];
						var widht = timePart.Delta;
						if (widht >= 0)
						{
							_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

							if (timePart.IsInterval)
							{
								Rectangle rectangle = new Rectangle();
								rectangle.ToolTip = timePart.Tooltip;
								rectangle.Fill = new SolidColorBrush(Colors.Green);
								rectangle.Stroke = new SolidColorBrush(Colors.Black);
								Grid.SetRow(rectangle, 0);
								Grid.SetColumn(rectangle, i);
								_grid.Children.Add(rectangle);
							}
						}
					}
				}
			}

			_timeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
			for (int i = 1; i <= 23; i++)
			{
				_timeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				TextBlock timeTextBlock = new TextBlock();
				timeTextBlock.Text = i.ToString();
				timeTextBlock.Foreground = new SolidColorBrush(Colors.Black);
				timeTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				Grid.SetRow(timeTextBlock, 0);
				Grid.SetColumn(timeTextBlock, i);
				_timeGrid.Children.Add(timeTextBlock);
			}
			_timeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
		}
	}
}