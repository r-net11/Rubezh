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
			TimeIntervalPart = timeIntervalPart;

			StartTime = timeIntervalPart.StartTime;
			EndTime = timeIntervalPart.EndTime;
		}

		private DateTime _startTime;
		public DateTime StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		private DateTime _endTime;
		public DateTime EndTime
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
			return StartTime.TimeOfDay < EndTime.TimeOfDay;
		}
		protected override bool Save()
		{
			TimeIntervalPart.StartTime = StartTime;
			TimeIntervalPart.EndTime = EndTime;
			return true;
		}
	}
}