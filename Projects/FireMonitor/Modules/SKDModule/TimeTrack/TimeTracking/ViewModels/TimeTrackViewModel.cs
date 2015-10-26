using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackViewModel : BaseViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }
		public string ScheduleName { get; private set; }
		public DocumentsViewModel DocumentsViewModel { get; private set; }

		public TimeTrackViewModel(TimeTrackFilter timeTrackFilter, TimeTrackEmployeeResult timeTrackEmployeeResult)
		{
			DocumentsViewModel = new DocumentsViewModel(timeTrackEmployeeResult, timeTrackFilter.StartDate, timeTrackFilter.EndDate);

			ShortEmployee = timeTrackEmployeeResult.ShortEmployee;
			ScheduleName = timeTrackEmployeeResult.ScheduleName;
			if (timeTrackEmployeeResult.DayTimeTracks == null)
			    timeTrackEmployeeResult.DayTimeTracks = new List<DayTimeTrack>();

			DayTracks = new ObservableCollection<DayTrackViewModel>();
			foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
			{
				dayTimeTrack.Calculate();
				var dayTrackViewModel = new DayTrackViewModel(dayTimeTrack, timeTrackFilter, timeTrackEmployeeResult.ShortEmployee);
				DayTracks.Add(dayTrackViewModel);
			}

			Totals = new List<TimeTrackTotal>();
			foreach (var timeTrackType in timeTrackFilter.TotalTimeTrackTypeFilters)
			{
				Totals.Add(new TimeTrackTotal(timeTrackType));
			}

			foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
			{
				foreach (var timeTrackTotal in dayTimeTrack.Totals)
				{
					var total = Totals.FirstOrDefault(x => x.TimeTrackType == timeTrackTotal.TimeTrackType);
					if (total != null)
					{
						total.TimeSpan += timeTrackTotal.TimeSpan;
					}
				}
			}
			OnPropertyChanged(() => Totals);
		}

		public ObservableCollection<DayTrackViewModel> DayTracks { get; set; }
		public List<TimeTrackTotal> Totals { get; private set; }
	}
}