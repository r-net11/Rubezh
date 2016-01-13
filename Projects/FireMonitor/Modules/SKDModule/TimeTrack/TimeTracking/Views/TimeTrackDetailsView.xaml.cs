﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI;
using FiresecAPI.SKD;
using SKDModule.ViewModels;
using SKDModule.Converters;
using FiresecClient;

namespace SKDModule.Views
{
	public partial class TimeTrackDetailsView : UserControl
	{
		public TimeTrackDetailsView()
		{
			InitializeComponent();
			Loaded += TimeTrackDetailsView_Loaded;
		}

		class TimePart
		{
			public double Delta { get; set; }
			public bool IsInterval { get; set; }
			public string Tooltip { get; set; }
			public Color Color { get; set; }
			public TimeTrackType TimeTrackType { get; set; }
		}

		string TimePartDateToString(DateTime? dateTime)
		{
			if (!dateTime.HasValue) return string.Empty;

			var result = dateTime.Value.TimeOfDay.Hours + ":" + dateTime.Value.TimeOfDay.Minutes + ":" + dateTime.Value.TimeOfDay.Seconds;
			return result;
		}

		string TimePartDateToString(TimeSpan timeSpan)
		{
			var result = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
			return result;
		}

		void TimeTrackDetailsView_Loaded(object sender, RoutedEventArgs e)
		{
			Refresh();
		}

		public void Refresh()
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
						case DocumentType.AbsenceReasonable:
							timeTrackPart.TimeTrackPartType = TimeTrackType.DocumentAbsenceReasonable;
							break;
					}
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.EnterDateTime) + " - " +
					                        TimePartDateToString(timeTrackPart.ExitDateTime) + "\n" +
					                        timeTrackPart.MinTimeTrackDocumentType.Name;
				}

				foreach (var timeTrackPart in dayTimeTrack.RealTimeTrackPartsForCalculates)
				{
					var zoneName = "<Нет в конфигурации>";
					var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
					if (strazhZone != null)
					{
						zoneName = strazhZone.Name;
					}

					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.EnterDateTime) + " - " +
					                        TimePartDateToString(timeTrackPart.ExitDateTime) + "\n" + zoneName;
					timeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				}

				foreach (var timeTrackPart in dayTimeTrack.PlannedTimeTrackParts)
				{
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.EnterDateTime) + " - " +
					                        TimePartDateToString(timeTrackPart.ExitDateTime) + "\n" + timeTrackPart.DayName;
					if (timeTrackPart.StartsInPreviousDay)
						timeTrackPart.Tooltip += "\n" + "Интервал начинается днем рашьше";
					if (timeTrackPart.EndsInNextDay)
						timeTrackPart.Tooltip += "\n" + "Интервал заканчивается днем позже";
					timeTrackPart.TimeTrackPartType = TimeTrackType.Presence;
				}

				foreach (var timeTrackPart in dayTimeTrack.CombinedTimeTrackParts)
				{
					timeTrackPart.Tooltip = TimePartDateToString(timeTrackPart.EnterDateTime) + " - " +
					                        TimePartDateToString(timeTrackPart.ExitDateTime) + "\n" +
					                        timeTrackPart.TimeTrackPartType.ToDescription();
				}

				DrawTimeTrackGrid(dayTimeTrack.DocumentTrackParts, DocumentsGrid);
				DrawTimeTrackGrid(dayTimeTrack.RealTimeTrackPartsForDrawing, RealGrid);
				DrawTimeTrackGrid(dayTimeTrack.PlannedTimeTrackParts, PlannedGrid);
				DrawTimeTrackGrid(dayTimeTrack.CombinedTimeTrackParts.Where(x => !x.NotTakeInCalculations).ToList(), CombinedGrid);
			}

			DrawHoursGrid();
		}

		void DrawTimeTrackGrid(List<TimeTrackPart> timeTrackParts, Grid grid)
		{
			if (timeTrackParts.Count > 0)
			{
				double current = 0;
				var timeParts = new List<TimePart>();

				for (var i = 0; i < timeTrackParts.Count; i++)
				{
					var timeTrackPart = timeTrackParts[i];

					if(!timeTrackPart.ExitDateTime.HasValue) continue;

					var startTimePart = new TimePart {Delta = timeTrackPart.EnterDateTime.TimeOfDay.TotalSeconds - current, IsInterval = false};
					timeParts.Add(startTimePart);

					var endTimePart = new TimePart
					{
						Delta = timeTrackPart.ExitDateTime.Value.TimeOfDay.TotalSeconds - timeTrackPart.EnterDateTime.TimeOfDay.TotalSeconds,
						IsInterval = timeTrackPart.TimeTrackPartType != TimeTrackType.None,
						TimeTrackType = timeTrackPart.TimeTrackPartType,
						Tooltip = timeTrackPart.Tooltip
					};
					timeParts.Add(endTimePart);

					current = timeTrackPart.ExitDateTime.Value.TimeOfDay.TotalSeconds;
				}

				var lastTimePart = new TimePart {Delta = 24*60*60 - current, IsInterval = false};
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
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(widht, GridUnitType.Star) });

					if (timePart.IsInterval)
					{
						var rectangle = new Rectangle {ToolTip = timePart.Tooltip};
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
			TimeLineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
			for (int i = 1; i <= 23; i++)
			{
				TimeLineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				var timeTextBlock = new TextBlock
				{
					Text = i.ToString(),
					Foreground = new SolidColorBrush(Colors.Black),
					HorizontalAlignment = HorizontalAlignment.Center
				};
				Grid.SetRow(timeTextBlock, 0);
				Grid.SetColumn(timeTextBlock, i);
				TimeLineGrid.Children.Add(timeTextBlock);
			}
			TimeLineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
		}
	}
}