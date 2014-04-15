using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseViewModel
	{
		public TimeInterval TimeInterval { get; private set; }

		public TimeIntervalViewModel(TimeInterval timeInterval)
		{
			TimeInterval = timeInterval;
		}

		public DateTime StartTime
		{
			get { return TimeInterval.StartTime; }
		}
		public DateTime EndTime
		{
			get { return TimeInterval.EndTime; }
		}
		public IntervalTransitionType IntervalTransitionType
		{
			get { return TimeInterval.IntervalTransitionType; }
		}

		public void Update()
		{
			OnPropertyChanged(() => TimeInterval);
			OnPropertyChanged(() => StartTime);
			OnPropertyChanged(() => EndTime);
			OnPropertyChanged(() => IntervalTransitionType);
		}
	}
}