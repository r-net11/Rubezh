using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI;
using FiresecAPI.SKD;
using SKDModule.ViewModels;
using SKDModule.Converters;

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
			public Color Color { get; set; }
			public TimeTrackType TimeTrackType { get; set; }
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

					switch (timeTrackPart.MinTimeTrackDocumentType.DocumentType)
					{
						case DocumentType.Overtime:
							endTimePart.TimeTrackType = TimeTrackType.DocumentOvertime;
							break;

						case DocumentType.Presence:
							endTimePart.TimeTrackType = TimeTrackType.DocumentPresence;
							break;

						case DocumentType.Absence:
							endTimePart.TimeTrackType = TimeTrackType.DocumentAbsence;
							break;
					}
					endTimePart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime) + "\n" + timeTrackPart.MinTimeTrackDocumentType.Name;
					timeParts.Add(endTimePart);

					current = timeTrackPart.EndTime.TotalSeconds;
				}
				var lastTimePart = new TimePart();
				lastTimePart.Delta = 24 * 60 * 60 - current;
				lastTimePart.IsInterval = false;
				timeParts.Add(lastTimePart);

				DrawGrid(timeParts, DocumentsGrid);
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
					endTimePart.TimeTrackType = TimeTrackType.Presence;
					timeParts.Add(endTimePart);

					current = timeTrackPart.EndTime.TotalSeconds;
				}
				var lastTimePart = new TimePart();
				lastTimePart.Delta = 24 * 60 * 60 - current;
				lastTimePart.IsInterval = false;
				timeParts.Add(lastTimePart);

				DrawGrid(timeParts, RealGrid);
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
					endTimePart.TimeTrackType = TimeTrackType.Presence;
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

				DrawGrid(timeParts, PlannedGrid);
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
					endTimePart.IsInterval = trackPart.TimeTrackPartType != TimeTrackType.None;
					endTimePart.Tooltip = TimePartDateToString(trackPart.StartTime) + " - " + TimePartDateToString(trackPart.EndTime) + " " + trackPart.TimeTrackPartType.ToDescription();
					endTimePart.TimeTrackType = trackPart.TimeTrackPartType;
					timeParts.Add(endTimePart);

					current = trackPart.EndTime.TotalSeconds;
				}
				var lastTimePart = new TimePart();
				lastTimePart.Delta = 24 * 60 * 60 - current;
				lastTimePart.IsInterval = false;
				timeParts.Add(lastTimePart);

				DrawGrid(timeParts, CombinedGrid);
			}
		}

		void DrawGrid(List<TimePart> timeParts, Grid grid)
		{
			for (int i = 0; i < timeParts.Count; i++)
			{
				var timePart = timeParts[i];
				var widht = timePart.Delta;
				if (widht >= 0)
				{
					grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(widht, GridUnitType.Star) });

					if (timePart.IsInterval)
					{
						Rectangle rectangle = new Rectangle();
						rectangle.ToolTip = timePart.Tooltip;
						var timeTrackTypeToColorConverter = new TimeTrackTypeToColorConverter();
						rectangle.Fill = (Brush)timeTrackTypeToColorConverter.Convert(timePart.TimeTrackType, null, null, null);
						rectangle.Stroke = new SolidColorBrush(Colors.Black);
						Grid.SetRow(rectangle, 0);
						Grid.SetColumn(rectangle, i);
						grid.Children.Add(rectangle);
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