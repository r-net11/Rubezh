using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using SKDModule.ViewModels;
using System.Collections.Generic;
using System;
using FiresecAPI;
using FiresecAPI.SKD;

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
			public DayTrackDualIntervalPartType DayTrackDualIntervalPartType { get; set; }
		}

		string TimePartDateToString(DateTime dateTime)
		{
			var result = dateTime.TimeOfDay.Hours.ToString() + ":" + dateTime.TimeOfDay.Minutes.ToString() + ":" + dateTime.TimeOfDay.Seconds.ToString();
			return result;
		}

		string TimePartDateToString(TimeSpan timeSpan)
		{
			var result = timeSpan.Hours.ToString() + ":" + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString();
			return result;
		}

		void TimeTrackDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			TimeTrackDetailsViewModel timeTrackDetailsViewModel = DataContext as TimeTrackDetailsViewModel;
			if (timeTrackDetailsViewModel != null)
			{
				var dayTimeTrack = timeTrackDetailsViewModel.DayTimeTrack;

				DrawTrackGrid(dayTimeTrack);
				DrawSheduleGrid(dayTimeTrack);
				DrawDualSheduleGrid(dayTimeTrack);
			}

			DrawHoursGrid();
		}

		void DrawTrackGrid(DayTimeTrack dayTimeTrack)
		{			
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

		void DrawSheduleGrid(DayTimeTrack dayTimeTrack)
		{
			var orderedIntervals = dayTimeTrack.Intervals.OrderBy(x => x.BeginDate.Value.Ticks).ToList();

			double current = 0;
			var timeParts = new List<TimePart>();
			for (int i = 0; i < orderedIntervals.Count; i++)
			{
				var interval = orderedIntervals[i];

				var startTimePart = new TimePart();
				startTimePart.Delta = interval.BeginDate.Value.TimeOfDay.TotalSeconds - current;
				startTimePart.IsInterval = false;
				timeParts.Add(startTimePart);

				var endTimePart = new TimePart();
				endTimePart.Delta = interval.EndDate.Value.TimeOfDay.TotalSeconds - interval.BeginDate.Value.TimeOfDay.TotalSeconds;
				endTimePart.IsInterval = true;
				endTimePart.Tooltip = TimePartDateToString(interval.EndDate.Value) + " - " + TimePartDateToString(interval.BeginDate.Value);
				timeParts.Add(endTimePart);

				current = interval.EndDate.Value.TimeOfDay.TotalSeconds;
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
					_sheduleGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

					if (timePart.IsInterval)
					{
						Rectangle rectangle = new Rectangle();
						rectangle.ToolTip = timePart.Tooltip;
						rectangle.Fill = new SolidColorBrush(Colors.Green);
						rectangle.Stroke = new SolidColorBrush(Colors.Black);
						Grid.SetRow(rectangle, 0);
						Grid.SetColumn(rectangle, i);
						_sheduleGrid.Children.Add(rectangle);
					}
				}
			}
		}

		void DrawDualSheduleGrid(DayTimeTrack dayTimeTrack)
		{
			double current = 0;
			var timeParts = new List<TimePart>();
			for (int i = 0; i < dayTimeTrack.DayTrackDualIntervalParts.Count; i++)
			{
				var dualTrackPart = dayTimeTrack.DayTrackDualIntervalParts[i];

				var startTimePart = new TimePart();
				startTimePart.Delta = dualTrackPart.StartTime.TotalSeconds - current;
				startTimePart.IsInterval = false;
				timeParts.Add(startTimePart);

				var endTimePart = new TimePart();
				endTimePart.Delta = dualTrackPart.EndTime.TotalSeconds - dualTrackPart.StartTime.TotalSeconds;
				endTimePart.IsInterval = dualTrackPart.DayTrackDualIntervalPartType != DayTrackDualIntervalPartType.None;
				endTimePart.Tooltip = TimePartDateToString(dualTrackPart.EndTime) + " - " + TimePartDateToString(dualTrackPart.StartTime) + " " + dualTrackPart.DayTrackDualIntervalPartType.ToDescription();
				endTimePart.DayTrackDualIntervalPartType = dualTrackPart.DayTrackDualIntervalPartType;
				timeParts.Add(endTimePart);

				current = dualTrackPart.EndTime.TotalSeconds;
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
					_dualGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

					if (timePart.IsInterval)
					{
						Rectangle rectangle = new Rectangle();
						rectangle.ToolTip = timePart.Tooltip;
						switch(timePart.DayTrackDualIntervalPartType)
						{
							case DayTrackDualIntervalPartType.Planed:
								rectangle.Fill = new SolidColorBrush(Colors.Red);
								break;

							case DayTrackDualIntervalPartType.Both:
								rectangle.Fill = new SolidColorBrush(Colors.Green);
								break;

							case DayTrackDualIntervalPartType.Real:
								rectangle.Fill = new SolidColorBrush(Colors.Yellow);
								break;
						}
						rectangle.Stroke = new SolidColorBrush(Colors.Black);
						Grid.SetRow(rectangle, 0);
						Grid.SetColumn(rectangle, i);
						_dualGrid.Children.Add(rectangle);
					}
				}
			}
		}

		void DrawHoursGrid()
		{
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