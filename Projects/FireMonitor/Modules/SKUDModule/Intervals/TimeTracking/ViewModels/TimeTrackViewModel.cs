using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD.EmployeeTimeIntervals;

namespace SKDModule.Intervals.TimeTracking.ViewModels
{
	public class TimeTrackViewModel : BaseObjectViewModel<TimeTrack>
	{
		public TimeTrackViewModel(TimeTrack timeTrack)
			: base(timeTrack)
		{
		}
	}
}
