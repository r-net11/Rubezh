using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using OrganizationFilter = FiresecAPI.OrganisationFilter;
using System;
using SKDModule.Intervals.Common.ViewModels;

namespace SKDModule.ViewModels
{
	public class NamedIntervalsViewModel : IntervalViewPartViewModel<OrganisationNamedIntervalsViewModel, NamedIntervalViewModel, NamedInterval>
	{
		public NamedIntervalsViewModel()
		{
		}

		public override OrganisationNamedIntervalsViewModel CreateOrganizationViewModel(FiresecAPI.Organisation organization)
		{
			return new OrganisationNamedIntervalsViewModel(organization);
		}
		public override IEnumerable<NamedInterval> GetModels()
		{
			return NamedIntervalHelper.Get(new NamedIntervalFilter() { OrganizationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });
		}
	}
}