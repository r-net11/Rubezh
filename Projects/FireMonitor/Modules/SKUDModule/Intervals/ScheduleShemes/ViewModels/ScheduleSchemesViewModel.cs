using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using SKDModule.Intervals.Common.ViewModels;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public abstract class ScheduleSchemesViewModel : IntervalViewPartViewModel<OrganisationScheduleSchemasViewModel, ScheduleSchemeViewModel, ScheduleScheme>
	{
		public abstract ScheduleSchemeType Type { get; }

		public ScheduleSchemesViewModel()
		{
		}

		protected override OrganisationScheduleSchemasViewModel CreateOrganizationViewModel(FiresecAPI.Organisation organization)
		{
			return new OrganisationScheduleSchemasViewModel(Type, organization);
		}
		protected override IEnumerable<ScheduleScheme> GetModels()
		{
			return ScheduleSchemaHelper.Get(new ScheduleSchemeFilter()
			{
				OrganizationUIDs = FiresecManager.CurrentUser.OrganisationUIDs,
				Type = Type,
			});
		}

		public override void OnShow()
		{
			base.OnShow();
			// заменить на подписание на Event редактирования NamedInterval - выполнять в отдельном потоке
			foreach (var organization in Organisations)
				organization.ReloadNamedIntervals();
		}
	}
}
