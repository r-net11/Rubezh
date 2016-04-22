using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RubezhAPI;
using RubezhAPI.SKD;
using SKDModule.ViewModels;
using SKDModule.Converters;
using RubezhClient;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

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
			string result;
			if (timeSpan.Days > 0)
				result = "24:00:00";
			else
				result = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
			return result;
		}

		void TimeTrackDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			TimeTrackDetailsViewModel timeTrackDetailsViewModel = DataContext as TimeTrackDetailsViewModel;
			if (timeTrackDetailsViewModel != null)
			{
				var dayTimeTrack = timeTrackDetailsViewModel.DayTimeTrack;

				foreach (var timeTrackPart in dayTimeTrack.DocumentTrackParts)
				{
					switch (timeTrackPart.MinTimeTrackDocumentType.DocumentType)
					{
						case DocumentType.Overtime:
							timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentOvertime;
							break;

						case DocumentType.Presence:
							timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentPresence;
							break;

						case DocumentType.Absence:
							timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentAbsence;
							break;
					}
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime) + "\n" + timeTrackPart.MinTimeTrackDocumentType.Name;
				}

				foreach (var timeTrackPart in dayTimeTrack.RealTimeTrackParts)
				{
					var zoneName = "<Нет в конфигурации>";
					var gkZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
					if (gkZone != null)
					{
						zoneName = gkZone.Name;
					}
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime) + "\n" + zoneName;
					timeTrackPart.Tooltip += GetTooltip(timeTrackPart);
					timeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				}

				foreach (var timeTrackPart in dayTimeTrack.PlannedTimeTrackParts)
				{
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime) + "\n" + timeTrackPart.DayName;
					timeTrackPart.Tooltip += GetTooltip(timeTrackPart);
					timeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				}

				foreach (var timeTrackPart in dayTimeTrack.CombinedTimeTrackParts)
				{
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.StartTime) + " - " + TimePartDateToString(timeTrackPart.EndTime) + "\n" + timeTrackPart.TimeTrackPartType.ToDescription();
				}
				DrawTimeTrackGrid(dayTimeTrack.DocumentTrackParts, DocumentsGrid);
				DrawTimeTrackGrid(dayTimeTrack.RealTimeTrackParts, RealGrid);
				DrawTimeTrackGrid(dayTimeTrack.PlannedTimeTrackParts, PlannedGrid);
				DrawTimeTrackGrid(dayTimeTrack.CombinedTimeTrackParts, CombinedGrid);
			}

			DrawHoursGrid();
		}

		void DrawTimeTrackGrid(List<TimeTrackPart> timeTrackParts, Grid grid)
		{
			if (timeTrackParts.Count > 0)
			{
				double current = 0;
				var timeParts = new List<TimePart>();
				for (int i = 0; i < timeTrackParts.Count; i++)
				{
					var timeTrackPart = timeTrackParts[i];

					var startTimePart = new TimePart();
					startTimePart.Delta = timeTrackPart.StartTime.TotalSeconds - current;
					startTimePart.IsInterval = false;
					timeParts.Add(startTimePart);

					var endTimePart = new TimePart();
					endTimePart.Delta = timeTrackPart.EndTime.TotalSeconds - timeTrackPart.StartTime.TotalSeconds;
					endTimePart.IsInterval = timeTrackPart.TimeTrackPartType != TimeTrackType.None;
					endTimePart.TimeTrackType = timeTrackPart.TimeTrackPartType;
					endTimePart.Tooltip = timeTrackPart.Tooltip;
					timeParts.Add(endTimePart);

					current = timeTrackPart.EndTime.TotalSeconds;
				}
				var lastTimePart = new TimePart();
				lastTimePart.Delta = 24 * 60 * 60 - current;
				lastTimePart.IsInterval = false;
				timeParts.Add(lastTimePart);

				DrawGrid(timeParts, grid);
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
		string GetTooltip(TimeTrackPart timeTrackPart)
		{
			var result = "";
			if (timeTrackPart.StartsInPreviousDay && timeTrackPart.EndsInNextDay)
				result = "\n" + "Интервал начинается днем раньше и заканчивается днем позже";
			else if (timeTrackPart.StartsInPreviousDay)
				result = "\n" + "Интервал начинается днем раньше";
		    else if (timeTrackPart.EndsInNextDay)
				result = "\n" + "Интервал заканчивается днем позже";
			return result;
		}
	}
}