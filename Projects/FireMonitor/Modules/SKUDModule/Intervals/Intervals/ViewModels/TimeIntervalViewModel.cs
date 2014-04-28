using System;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseViewModel
	{
		public TimeInterval TimeInterval { get; private set; }

		public TimeIntervalViewModel(TimeInterval timeInterval)
		{
			TimeInterval = timeInterval;
		}

		public TimeSpan BeginTime
		{
			get { return TimeInterval.BeginTime; }
		}
		public TimeSpan EndTime
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
			OnPropertyChanged(() => BeginTime);
			OnPropertyChanged(() => EndTime);
			OnPropertyChanged(() => IntervalTransitionType);
		}
	}
}