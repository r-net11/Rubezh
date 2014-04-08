using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeIntervalPartViewModel : BaseViewModel
	{
		public TimeInterval TimeIntervalPart { get; set; }

		public TimeIntervalPartViewModel(TimeInterval timeIntervalPart)
		{
			TimeIntervalPart = timeIntervalPart;
		}

		public DateTime StartTime
		{
			get { return TimeIntervalPart.StartTime; }
		}

		public DateTime EndTime
		{
			get { return TimeIntervalPart.EndTime; }
		}

		public IntervalTransitionType IntervalTransitionType
		{
			get { return TimeIntervalPart.IntervalTransitionType; }
		}

		public void Update()
		{
			OnPropertyChanged("TimeInterval");
			OnPropertyChanged("StartTime");
			OnPropertyChanged("EndTime");
			OnPropertyChanged("IntervalTransitionType");
		}
	}
}