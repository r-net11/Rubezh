using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.Intervals.Common.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public class ScheduleSchemesSlideViewModel : ScheduleSchemesViewModel
	{
		public ScheduleSchemesSlideViewModel()
			: base()
		{
		}
		public override ScheduleSchemeType Type
		{
			get { return ScheduleSchemeType.SlideDay; }
		}
	}
}