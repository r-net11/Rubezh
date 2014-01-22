using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class TimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDTimeInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(SKDTimeInterval timeInterval = null)
		{
			Title = "Задание интервала";
			TimeInterval = timeInterval;
			StartTime = timeInterval.StartTime;
			EndTime = timeInterval.EndTime;
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
			TimeInterval.StartTime = StartTime;
			TimeInterval.EndTime = EndTime;
			return true;
		}
	}
}