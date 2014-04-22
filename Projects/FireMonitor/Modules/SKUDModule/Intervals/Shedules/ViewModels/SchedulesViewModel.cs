using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using OrganisationFilter = FiresecAPI.OrganisationFilter;
using SKDModule.Intervals.Common.ViewModels;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class SchedulesViewModel : IntervalViewPartViewModel<OrganisationSchedulesViewModel, ScheduleViewModel, Schedule>
	{
		public SchedulesViewModel()
		{
		}

		protected override OrganisationSchedulesViewModel CreateOrganisationViewModel(FiresecAPI.Organisation organisation)
		{
			return new OrganisationSchedulesViewModel(organisation);
		}
		protected override IEnumerable<Schedule> GetModels()
		{
			return ScheduleHelper.Get(new ScheduleFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });
		}
	}
}