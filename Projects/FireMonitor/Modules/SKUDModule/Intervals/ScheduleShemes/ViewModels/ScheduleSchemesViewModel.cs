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

		protected override OrganisationScheduleSchemasViewModel CreateOrganisationViewModel(FiresecAPI.Organisation organisation)
		{
			return new OrganisationScheduleSchemasViewModel(Type, organisation);
		}
		protected override IEnumerable<ScheduleScheme> GetModels()
		{
			return ScheduleSchemaHelper.Get(new ScheduleSchemeFilter()
			{
				OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs,
				Type = Type,
			});
		}

		public override void OnShow()
		{
			base.OnShow();
			// заменить на подписание на Event редактирования NamedInterval - выполнять в отдельном потоке
			foreach (var organisation in Organisations)
				organisation.ReloadNamedIntervals();
		}
	}
}
