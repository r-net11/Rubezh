using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeTrackViewModel : BaseViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }
		public DocumentsViewModel DocumentsViewModel { get; set; }

		public TimeTrackViewModel(ShortEmployee shortEmployee, List<DayTimeTrack> dayTimeTracks)
		{
			ShortEmployee = shortEmployee;
			if (dayTimeTracks == null)
				dayTimeTracks = new List<DayTimeTrack>();

			DayTracks = new ObservableCollection<DayTrackViewModel>();
			foreach (var dayTimeTrack in dayTimeTracks)
			{
				dayTimeTrack.Calculate();
				var dayTrackViewModel = new DayTrackViewModel(dayTimeTrack);
				DayTracks.Add(dayTrackViewModel);
			}

			Total = new TimeSpan();
			TotalMissed = new TimeSpan();
			TotalInSchedule = new TimeSpan();
			TotalOvertime = new TimeSpan();
			foreach (var dayTimeTrack in dayTimeTracks)
			{
				Total = Total.Add(dayTimeTrack.Total);
				TotalMissed = TotalMissed.Add(dayTimeTrack.TotalMissed);
				TotalInSchedule = TotalInSchedule.Add(dayTimeTrack.TotalInSchedule);
				TotalOvertime = TotalOvertime.Add(dayTimeTrack.TotalOvertime);
			}
		}

		public ObservableCollection<DayTrackViewModel> DayTracks { get; set; }

		public TimeSpan Total { get; private set; }
		public TimeSpan TotalMissed { get; private set; }
		public TimeSpan TotalInSchedule { get; private set; }
		public TimeSpan TotalOvertime { get; private set; }
	}
}