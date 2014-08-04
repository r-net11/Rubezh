using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DayTrackViewModel : BaseViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }

		public DayTrackViewModel(DayTimeTrack dayTimeTrack)
		{
			DayTimeTrack = dayTimeTrack;
			Total = DateTimeToString(DayTimeTrack.Total);
			TotalMiss = DateTimeToString(DayTimeTrack.TotalMiss);
			TotalInSchedule = DateTimeToString(DayTimeTrack.TotalInSchedule);
			TotalOutSchedule = DateTimeToString(DayTimeTrack.TotalOutSchedule);
		}

		public string Total { get; private set; }
		public string TotalMiss { get; private set; }
		public string TotalInSchedule { get; private set; }
		public string TotalOutSchedule { get; private set; }

		public static string DateTimeToString(TimeSpan timeSpan)
		{
			if (timeSpan.Hours > 0)
				return timeSpan.Hours + "ч " + timeSpan.Minutes + "м";
			else
				return timeSpan.Minutes + "м";
		}
	}
}