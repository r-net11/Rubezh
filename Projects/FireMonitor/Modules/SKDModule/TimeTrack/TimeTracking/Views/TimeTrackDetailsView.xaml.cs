using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI;
using FiresecAPI.SKD;
using SKDModule.ViewModels;

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
			public TimeTrackPartType DayTrackDualIntervalPartType { get; set; }
		}

		string TimePartDateToString(DateTime dateTime)
		{
			var result = dateTime.TimeOfDay.Hours.ToString() + ":" + dateTime.TimeOfDay.Minutes.ToString() + ":" + dateTime.TimeOfDay.Seconds.ToString();
			return result;
		}

		string TimePartDateToString(TimeSpan timeSpan)
		{
			var result = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
			return result;
		}

		void TimeTrackDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			TimeTrackDetailsViewModel timeTrackDetailsViewModel = DataContext as TimeTrackDetailsViewModel;
			if (timeTrackDetailsViewModel != null)
			{
				var dayTimeTrack = timeTrackDetailsViewModel.DayTimeTrack;
				DrawDocumentTimeTrackGrid(dayTimeTrack);
				DrawRealTimeTrackGrid(dayTimeTrack);
				DrawPlannedTimeTrackGrid(dayTimeTrack);
				DrawCombinedTimeTrackGrid(dayTimeTrack);
			}

			DrawHoursGrid();
		}

		void DrawDocumentTimeTrackGrid(DayTimeTrack dayTimeTrack)
		{
			if (dayTimeTrack.DocumentTrackParts.Count > 0)
			{
				double current = 0;
				var timeParts = new List<TimePart>();
				for (int i = 0; i < dayTimeTrack.DocumentTrackParts.Count; i++)
				{
					var timeTrackPart = dayTimeTrack.DocumentTrackParts[i];

					var startTimePart = new TimePart();
					startTimePart.Delta = timeTrackPart.StartTime.TotalSeconds - current;
					startTimePart.IsInterval = false;
					timeParts.Add(startTimePart);

					var endTimePart = new TimePart();
					endTimePart.Delta = timeTrackPart.EndTime.TotalSeconds - timeTrackPart.StartTime.TotalSeconds;
					endTimePart.IsInterval = true;
					endTimePart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime);
					timeParts.Add(endTimePart);

					current = timeTrackPart.EndTime.TotalSeconds;
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
						DocumentsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

						if (timePart.IsInterval)
						{
							Rectangle rectangle = new Rectangle();
							rectangle.ToolTip = timePart.Tooltip;
							rectangle.Fill = new SolidColorBrush(Colors.Green);
							rectangle.Stroke = new SolidColorBrush(Colors.Black);
							Grid.SetRow(rectangle, 0);
							Grid.SetColumn(rectangle, i);
							DocumentsGrid.Children.Add(rectangle);
						}
					}
				}
			}
		}

		void DrawRealTimeTrackGrid(DayTimeTrack dayTimeTrack)
		{
			if (dayTimeTrack.RealTimeTrackParts.Count > 0)
			{
				double current = 0;
				var timeParts = new List<TimePart>();
				for (int i = 0; i < dayTimeTrack.RealTimeTrackParts.Count; i++)
				{
					var timeTrackPart = dayTimeTrack.RealTimeTrackParts[i];

					var startTimePart = new TimePart();
					startTimePart.Delta = timeTrackPart.StartTime.TotalSeconds - current;
					startTimePart.IsInterval = false;
					timeParts.Add(startTimePart);

					var endTimePart = new TimePart();
					endTimePart.Delta = timeTrackPart.EndTime.TotalSeconds - timeTrackPart.StartTime.TotalSeconds;
					endTimePart.IsInterval = true;
					endTimePart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime);
					timeParts.Add(endTimePart);

					current = timeTrackPart.EndTime.TotalSeconds;
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
						RealGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

						if (timePart.IsInterval)
						{
							Rectangle rectangle = new Rectangle();
							rectangle.ToolTip = timePart.Tooltip;
							rectangle.Fill = new SolidColorBrush(Colors.Green);
							rectangle.Stroke = new SolidColorBrush(Colors.Black);
							Grid.SetRow(rectangle, 0);
							Grid.SetColumn(rectangle, i);
							RealGrid.Children.Add(rectangle);
						}
					}
				}
			}
		}

		void DrawPlannedTimeTrackGrid(DayTimeTrack dayTimeTrack)
		{
			if (dayTimeTrack.PlannedTimeTrackParts.Count > 0)
			{
				double current = 0;
				var timeParts = new List<TimePart>();
				for (int i = 0; i < dayTimeTrack.PlannedTimeTrackParts.Count; i++)
				{
					var timeTrackPart = dayTimeTrack.PlannedTimeTrackParts[i];

					var startTimePart = new TimePart();
					startTimePart.Delta = timeTrackPart.StartTime.TotalSeconds - current;
					startTimePart.IsInterval = false;
					timeParts.Add(startTimePart);

					var endTimePart = new TimePart();
					endTimePart.Delta = timeTrackPart.EndTime.TotalSeconds - timeTrackPart.StartTime.TotalSeconds;
					endTimePart.IsInterval = true;
					endTimePart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime);
					timeParts.Add(endTimePart);

					current = timeTrackPart.EndTime.TotalSeconds;
				}
				var lastTimePart = new TimePart();
				lastTimePart.Delta = 24 * 60 * 60 - 1 - current;
				if (lastTimePart.Delta > 0)
				{
					lastTimePart.IsInterval = false;
					timeParts.Add(lastTimePart);
				}

				for (int i = 0; i < timeParts.Count; i++)
				{
					var timePart = timeParts[i];
					var widht = timePart.Delta;
					if (widht >= 0)
					{
						PlannedGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

						if (timePart.IsInterval)
						{
							Rectangle rectangle = new Rectangle();
							rectangle.ToolTip = timePart.Tooltip;
							rectangle.Fill = new SolidColorBrush(Colors.Green);
							rectangle.Stroke = new SolidColorBrush(Colors.Black);
							Grid.SetRow(rectangle, 0);
							Grid.SetColumn(rectangle, i);
							PlannedGrid.Children.Add(rectangle);
						}
					}
				}
			}
		}

		void DrawCombinedTimeTrackGrid(DayTimeTrack dayTimeTrack)
		{
			if (dayTimeTrack.CombinedTimeTrackParts.Count > 0)
			{
				double current = 0;
				var timeParts = new List<TimePart>();
				for (int i = 0; i < dayTimeTrack.CombinedTimeTrackParts.Count; i++)
				{
					var trackPart = dayTimeTrack.CombinedTimeTrackParts[i];

					var startTimePart = new TimePart();
					startTimePart.Delta = trackPart.StartTime.TotalSeconds - current;
					startTimePart.IsInterval = false;
					timeParts.Add(startTimePart);

					var endTimePart = new TimePart();
					endTimePart.Delta = trackPart.EndTime.TotalSeconds - trackPart.StartTime.TotalSeconds;
					endTimePart.IsInterval = trackPart.TimeTrackPartType != TimeTrackPartType.None;
					endTimePart.Tooltip = TimePartDateToString(trackPart.StartTime) + " - " + TimePartDateToString(trackPart.EndTime) + " " + trackPart.TimeTrackPartType.ToDescription();
					endTimePart.DayTrackDualIntervalPartType = trackPart.TimeTrackPartType;
					timeParts.Add(endTimePart);

					current = trackPart.EndTime.TotalSeconds;
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
						CombinedGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

						if (timePart.IsInterval)
						{
							Rectangle rectangle = new Rectangle();
							rectangle.ToolTip = timePart.Tooltip;
							switch (timePart.DayTrackDualIntervalPartType)
							{
								case TimeTrackPartType.PlanedOnly:
									rectangle.Fill = new SolidColorBrush(Colors.Red);
									break;

								case TimeTrackPartType.MissedButInsidePlan:
									rectangle.Fill = new SolidColorBrush(Colors.Pink);
									break;

								case TimeTrackPartType.AsPlanned:
									rectangle.Fill = new SolidColorBrush(Colors.Green);
									break;

								case TimeTrackPartType.RealOnly:
									rectangle.Fill = new SolidColorBrush(Colors.Yellow);
									break;

								case TimeTrackPartType.InBrerak:
									rectangle.Fill = new SolidColorBrush(Colors.Violet);
									break;

								case TimeTrackPartType.Late:
									rectangle.Fill = new SolidColorBrush(Colors.SkyBlue);
									break;

								case TimeTrackPartType.EarlyLeave:
									rectangle.Fill = new SolidColorBrush(Colors.LightPink);
									break;

								case TimeTrackPartType.Document:
									rectangle.Fill = new SolidColorBrush(Colors.Brown);
									break;
							}
							rectangle.Stroke = new SolidColorBrush(Colors.Black);
							Grid.SetRow(rectangle, 0);
							Grid.SetColumn(rectangle, i);
							CombinedGrid.Children.Add(rectangle);
						}
					}
				}
			}
		}

		void DrawHoursGrid()
		{
			TimeLineGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
			for (int i = 1; i <= 23; i++)
			{
				TimeLineGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				TextBlock timeTextBlock = new TextBlock();
				timeTextBlock.Text = i.ToString();
				timeTextBlock.Foreground = new SolidColorBrush(Colors.Black);
				timeTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				Grid.SetRow(timeTextBlock, 0);
				Grid.SetColumn(timeTextBlock, i);
				TimeLineGrid.Children.Add(timeTextBlock);
			}
			TimeLineGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
		}
	}
}