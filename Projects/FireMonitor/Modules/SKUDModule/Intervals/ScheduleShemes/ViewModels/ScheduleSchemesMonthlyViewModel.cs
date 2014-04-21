using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.Intervals.Common.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using FiresecClient;
using System.Collections.ObjectModel;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public class ScheduleSchemesMonthlyViewModel : ScheduleSchemesViewModel
	{
		public ScheduleSchemesMonthlyViewModel()
			: base()
		{
		}
		public override ScheduleSchemeType Type
		{
			get { return ScheduleSchemeType.Month; }
		}
	}
}
