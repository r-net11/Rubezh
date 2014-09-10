using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDayIntervalPart DayIntervalPart { get; private set; }

		public DayIntervalPartDetailsViewModel(SKDDayIntervalPart dayIntervalPart = null)
		{
			Title = "Задание интервала";
			DayIntervalPart = dayIntervalPart ?? new SKDDayIntervalPart();

			StartTime = DayIntervalPart.StartTime;
			EndTime = DayIntervalPart.EndTime;
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
			DayIntervalPart.StartTime = StartTime;
			DayIntervalPart.EndTime = EndTime;
			return true;
		}
	}
}