using System;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DaySchedulePartViewModel : BaseViewModel
	{
		public GKDaySchedulePart DaySchedulePart { get; set; }

		public DaySchedulePartViewModel(GKDaySchedulePart daySchedulePart)
		{
			DaySchedulePart = daySchedulePart;
		}

		public TimeSpan StartTime
		{
			get { return TimeSpan.FromMilliseconds(DaySchedulePart.StartMilliseconds); }
		}

		public TimeSpan EndTime
		{
			get { return TimeSpan.FromMilliseconds(DaySchedulePart.EndMilliseconds); }
		}

		public void Update()
		{
			OnPropertyChanged(() => DaySchedulePart);
			OnPropertyChanged(() => StartTime);
			OnPropertyChanged(() => EndTime);
		}
	}
}