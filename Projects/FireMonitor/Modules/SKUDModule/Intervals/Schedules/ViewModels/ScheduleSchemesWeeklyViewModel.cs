using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.Intervals.Common.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ScheduleSchemesWeeklyViewModel : ScheduleSchemesViewModel
	{
		public ScheduleSchemesWeeklyViewModel()
			: base(ScheduleSchemeType.Week)
		{
		}
	}
}
