using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecAPI.SKD;
using ReactiveUI;
using SKDModule.ViewModels;

namespace SKDModule.Model
{
	public class TimeTrack : ReactiveObject
	{
		#region Properties

		public ShortEmployee ShortEmployee { get; private set; }
		public string ScheduleName { get; private set; }
		public DocumentsViewModel DocumentsViewModel { get; private set; }
		/// <summary>
		/// Коллекция отображаемых дней в УРВ
		/// </summary>
		public ObservableCollection<DayTrack> DayTracks { get; set; }

		public ObservableCollection<TimeTrackTotal> Totals { get; set; }
		#endregion

		#region Constructors

		public TimeTrack(TimeTrackFilter timeTrackFilter, TimeTrackEmployeeResult timeTrackEmployeeResult)
		{
			DocumentsViewModel = new DocumentsViewModel(timeTrackEmployeeResult, timeTrackFilter.StartDate, timeTrackFilter.EndDate);

			ShortEmployee = timeTrackEmployeeResult.ShortEmployee;
			ScheduleName = timeTrackEmployeeResult.ScheduleName;

			if (timeTrackEmployeeResult.DayTimeTracks == null)
			    timeTrackEmployeeResult.DayTimeTracks = new List<DayTimeTrack>();

			//Выбор всех типов интервалов, которые указаны в фильтре типов
			Totals = new ObservableCollection<TimeTrackTotal>(timeTrackFilter.TotalTimeTrackTypeFilters.Select(x => new TimeTrackTotal(x)));
			DayTracks = new ObservableCollection<DayTrack>();
			var crossNightNTimeTrackParts = new List<TimeTrackPart>();
			
			foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
			{
				if (string.IsNullOrEmpty(dayTimeTrack.Error))
				{
					dayTimeTrack.CrossNightTimeTrackParts = crossNightNTimeTrackParts;
					dayTimeTrack.Calculate();
					crossNightNTimeTrackParts = dayTimeTrack.CrossNightTimeTrackParts;
				}
				else
				{
					dayTimeTrack.TimeTrackType = TimeTrackType.None;
					dayTimeTrack.Tooltip = TimeTrackType.None.ToDescription();
				}

				DayTracks.Add(new DayTrack(dayTimeTrack, timeTrackFilter, timeTrackEmployeeResult.ShortEmployee));
			}

			//Вычисление и запись результата отображения фильтрованных интервалов в таблице
			foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
			{
				if(dayTimeTrack == null || dayTimeTrack.Totals == null) continue;

				foreach (var timeTrackTotal in dayTimeTrack.Totals)
				{
					var total = Totals.FirstOrDefault(x => x.TimeTrackType == timeTrackTotal.TimeTrackType);
					if (total != null)
					{
						total.TimeSpan += timeTrackTotal.TimeSpan;
					}
				}
			}
		}

		#endregion
	}
}