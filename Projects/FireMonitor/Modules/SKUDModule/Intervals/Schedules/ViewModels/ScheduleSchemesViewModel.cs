using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;
using SKDModule.Intervals.Common.ViewModels;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ScheduleSchemesViewModel :  IntervalViewPartViewModel<OrganisationScheduleSchemasViewModel, ScheduleSchemaViewModel, ScheduleScheme>
	{
		public ScheduleSchemeType Type { get; private set; }

		public ScheduleSchemesViewModel(ScheduleSchemeType type)
		{
			Type = type;
		}

		public override OrganisationScheduleSchemasViewModel CreateOrganizationViewModel(FiresecAPI.Organisation organization)
		{
			return new OrganisationScheduleSchemasViewModel(organization);
		}
		public override IEnumerable<ScheduleScheme> GetModels()
		{
			// filter with Type
			return Enumerable.Empty<ScheduleScheme>();
		}
	}
}
