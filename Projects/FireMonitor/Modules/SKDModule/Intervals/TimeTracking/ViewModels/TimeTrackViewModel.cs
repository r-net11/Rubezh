using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using System.Collections.Generic;
using System;

namespace SKDModule.ViewModels
{
	public class TimeTrackViewModel : BaseViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }

		public TimeTrackViewModel(ShortEmployee shortEmployee, List<DayTimeTrack> dayTimeTracks)
		{
			ShortEmployee = shortEmployee;
			DayTracks = new ObservableCollection<DayTrackViewModel>();
			foreach (var dayTimeTrack in dayTimeTracks)
			{
				var dayTrackViewModel = new DayTrackViewModel(dayTimeTrack);
				DayTracks.Add(dayTrackViewModel);
			}

			TimeSpan totalTimeSpan = new TimeSpan();
			TimeSpan totalMissTimeSpan = new TimeSpan();
			TimeSpan totalInScheduleTimeSpan = new TimeSpan();
			TimeSpan totalOutScheduleTimeSpan = new TimeSpan();
			foreach (var dayTimeTrack in dayTimeTracks)
			{
				totalTimeSpan = totalTimeSpan.Add(dayTimeTrack.Total);
				totalMissTimeSpan = totalMissTimeSpan.Add(dayTimeTrack.TotalMiss);
				totalInScheduleTimeSpan = totalInScheduleTimeSpan.Add(dayTimeTrack.TotalInSchedule);
				totalOutScheduleTimeSpan = totalOutScheduleTimeSpan.Add(dayTimeTrack.TotalOutSchedule);
			}
			Total = DayTrackViewModel.DateTimeToString(totalTimeSpan);
			TotalMiss = DayTrackViewModel.DateTimeToString(totalMissTimeSpan);
			TotalInSchedule = DayTrackViewModel.DateTimeToString(totalInScheduleTimeSpan);
			TotalOutSchedule = DayTrackViewModel.DateTimeToString(totalOutScheduleTimeSpan);
		}

		public ObservableCollection<DayTrackViewModel> DayTracks { get; set; }

		public string Total { get; private set; }
		public string TotalMiss { get; private set; }
		public string TotalInSchedule { get; private set; }
		public string TotalOutSchedule { get; private set; }
	}
}