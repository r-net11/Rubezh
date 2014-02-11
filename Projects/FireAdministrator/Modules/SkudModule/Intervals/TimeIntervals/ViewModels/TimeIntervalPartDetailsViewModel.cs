using System;
using FiresecAPI;
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

		DateTime _startTime;
		public DateTime StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged("StartTime");
			}
		}

		DateTime _endTime;
		public DateTime EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged("EndTime");
			}
		}

		protected override bool Save()
		{
			TimeIntervalPart.StartTime = StartTime;
			TimeIntervalPart.EndTime = EndTime;
			return true;
		}
	}
}