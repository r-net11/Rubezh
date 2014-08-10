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
			Update();
		}

		public void Update()
		{
			Total = DateTimeToString(DayTimeTrack.Total);
			TotalMissed = DateTimeToString(DayTimeTrack.TotalMissed);
			TotalInSchedule = DateTimeToString(DayTimeTrack.TotalInSchedule);
			TotalOutSchedule = DateTimeToString(DayTimeTrack.TotalOutSchedule);

			IsNormal = false;
			IsHoliday = false;
			IsMissed = false;
			IsDayOff = false;
			IsIll = false;
			IsVacation = false;
			IsTrip = false;

			switch (DayTimeTrack.TimeTrackType)
			{
				case TimeTrackType.Holiday:
					IsHoliday = true;
					break;

				case TimeTrackType.Missed:
					IsMissed = true;
					break;

				case TimeTrackType.DayOff:
					IsDayOff = true;
					break;

				case TimeTrackType.Ill:
					IsIll = true;
					break;

				case TimeTrackType.Vacation:
					IsVacation = true;
					break;

				case TimeTrackType.Trip:
					IsTrip = true;
					break;

				default:
					IsNormal = true;
					break;
			}

			OnPropertyChanged(() => IsNormal);
			OnPropertyChanged(() => IsHoliday);
			OnPropertyChanged(() => IsMissed);
			OnPropertyChanged(() => IsDayOff);
			OnPropertyChanged(() => IsIll);
			OnPropertyChanged(() => IsVacation);
			OnPropertyChanged(() => IsTrip);
			OnPropertyChanged(() => DayTimeTrack);
		}

		public string Total { get; private set; }
		public string TotalMissed { get; private set; }
		public string TotalInSchedule { get; private set; }
		public string TotalOutSchedule { get; private set; }

		public bool IsNormal { get; private set; }
		public bool IsHoliday { get; private set; }
		public bool IsMissed { get; private set; }
		public bool IsDayOff { get; private set; }
		public bool IsIll { get; private set; }
		public bool IsVacation { get; private set; }
		public bool IsTrip { get; private set; }

		public static string DateTimeToString(TimeSpan timeSpan)
		{
			if (timeSpan.TotalHours > 0)
				return timeSpan.TotalHours + "ч " + timeSpan.Minutes + "м";
			else if (timeSpan.Minutes > 0)
				return timeSpan.Minutes + "м";
			return null;
		}
	}
}