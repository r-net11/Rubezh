using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.Intervals.Common.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class OrganisationScheduleSchemasViewModel : OrganisationIntervalViewModel<ScheduleSchemaViewModel, ScheduleScheme>
	{
		public OrganisationScheduleSchemasViewModel(FiresecAPI.Organisation organization)
			: base(organization)
		{
		}

		protected override ScheduleSchemaViewModel CreateViewModel(ScheduleScheme model)
		{
			return new ScheduleSchemaViewModel(model);
		}
	}
}
