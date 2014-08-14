using System;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseViewModel
	{
		public DayIntervalPart TimeInterval { get; private set; }

		public TimeIntervalViewModel(DayIntervalPart timeInterval)
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
		public DayIntervalPartTransitionType IntervalTransitionType
		{
			get { return TimeInterval.TransitionType; }
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