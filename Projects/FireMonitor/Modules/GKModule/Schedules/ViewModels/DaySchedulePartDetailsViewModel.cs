using System;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DaySchedulePartDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKDaySchedulePart DaySchedulePart { get; private set; }

		public DaySchedulePartDetailsViewModel(GKDaySchedulePart daySchedulePart = null)
		{
			Title = "Задание интервала";
			DaySchedulePart = daySchedulePart ?? new GKDaySchedulePart();

			StartTime = TimeSpan.FromMilliseconds(DaySchedulePart.StartMilliseconds);
			EndTime = TimeSpan.FromMilliseconds(DaySchedulePart.EndMilliseconds);
		}

		TimeSpan _startTime;
		public TimeSpan StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		TimeSpan _endTime;
		public TimeSpan EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		protected override bool CanSave()
		{
			return EndTime > StartTime;
		}
		protected override bool Save()
		{
			DaySchedulePart.StartMilliseconds = (int)StartTime.TotalMilliseconds;
			DaySchedulePart.EndMilliseconds = (int)EndTime.TotalMilliseconds;
			return true;
		}
	}
}