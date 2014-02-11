using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeIntervalPartViewModel : BaseViewModel
	{
		public EmployeeTimeIntervalPart TimeIntervalPart { get; set; }

		public TimeIntervalPartViewModel(EmployeeTimeIntervalPart timeIntervalPart)
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