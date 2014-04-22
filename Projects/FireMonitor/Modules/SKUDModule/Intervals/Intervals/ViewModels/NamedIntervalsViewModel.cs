using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using OrganisationFilter = FiresecAPI.OrganisationFilter;
using System;
using SKDModule.Intervals.Common.ViewModels;

namespace SKDModule.ViewModels
{
	public class NamedIntervalsViewModel : IntervalViewPartViewModel<OrganisationNamedIntervalsViewModel, NamedIntervalViewModel, NamedInterval>
	{
		public NamedIntervalsViewModel()
		{
		}

		protected override OrganisationNamedIntervalsViewModel CreateOrganisationViewModel(FiresecAPI.Organisation organisation)
		{
			return new OrganisationNamedIntervalsViewModel(organisation);
		}
		protected override IEnumerable<NamedInterval> GetModels()
		{
			return NamedIntervalHelper.Get(new NamedIntervalFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });
		}
	}
}