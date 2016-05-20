using System;
using RubezhAPI.GK;
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

		public string StartTime
		{
			get { return MillisecondsToString(DaySchedulePart.StartMilliseconds); }
		}

		public string EndTime
		{
			get { return MillisecondsToString(DaySchedulePart.EndMilliseconds); }
		}

		public void Update()
		{
			OnPropertyChanged(() => DaySchedulePart);
			OnPropertyChanged(() => StartTime);
			OnPropertyChanged(() => EndTime);
		}

		string MillisecondsToString(int milliseconds)
		{
			var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
			if (timeSpan.Days > 0)
			{
				return "24:00";
			}
			return timeSpan.Hours.ToString("D2") + ":" + timeSpan.Minutes.ToString("D2");
		}
	}
}