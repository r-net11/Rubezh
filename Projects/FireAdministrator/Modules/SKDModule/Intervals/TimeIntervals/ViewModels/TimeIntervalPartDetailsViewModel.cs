using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDTimeIntervalPart TimeIntervalPart { get; private set; }

		public TimeIntervalPartDetailsViewModel(SKDTimeIntervalPart timeIntervalPart = null)
		{
			Title = "Задание интервала";
			TimeIntervalPart = timeIntervalPart ?? new SKDTimeIntervalPart();

			StartTime = TimeIntervalPart.StartTime;
			EndTime = TimeIntervalPart.EndTime;
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
			return StartTime <= EndTime;
		}
		protected override bool Save()
		{
			TimeIntervalPart.StartTime = StartTime;
			TimeIntervalPart.EndTime = EndTime;
			return true;
		}
	}
}