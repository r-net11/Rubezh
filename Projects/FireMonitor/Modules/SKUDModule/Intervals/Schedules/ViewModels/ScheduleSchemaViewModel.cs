using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ScheduleSchemaViewModel : BaseObjectViewModel<ScheduleScheme>
	{
		public ScheduleSchemaViewModel(ScheduleScheme scheduleScheme)
			: base(scheduleScheme)
		{
		}
	}
}
