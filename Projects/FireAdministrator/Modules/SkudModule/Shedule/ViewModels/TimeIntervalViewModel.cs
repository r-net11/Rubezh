using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SkudModule.ViewModels
{
	public class TimeIntervalViewModel : BaseViewModel
	{
		public SKDTimeInterval TimeInterval { get; set; }

		public TimeIntervalViewModel(SKDTimeInterval timeInterval)
		{
			TimeInterval = timeInterval;
		}

		public DateTime StartTime
		{
			get { return TimeInterval.StartTime; }
		}

		DateTime _endTime;
		public DateTime EndTime
		{
			get { return TimeInterval.EndTime; }
		}

		public void Update()
		{
			OnPropertyChanged("TimeInterval");
			OnPropertyChanged("StartTime");
			OnPropertyChanged("EndTime");
		}
	}
}