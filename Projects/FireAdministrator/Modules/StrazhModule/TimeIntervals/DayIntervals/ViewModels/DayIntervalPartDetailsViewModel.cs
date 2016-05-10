using System;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DayIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDayIntervalPart DayIntervalPart { get; private set; }

		public DayIntervalPartDetailsViewModel(SKDDayIntervalPart dayIntervalPart = null)
		{
			Title = "Задание интервала";
			DayIntervalPart = dayIntervalPart ?? new SKDDayIntervalPart();

			StartTime = TimeSpan.FromMilliseconds(DayIntervalPart.StartMilliseconds);
			EndTime = TimeSpan.FromMilliseconds(DayIntervalPart.EndMilliseconds);
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
			DayIntervalPart.StartMilliseconds = StartTime.TotalMilliseconds;
			DayIntervalPart.EndMilliseconds = EndTime.TotalMilliseconds;
			return true;
		}
	}
}