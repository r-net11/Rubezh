using System;
using System.Collections.Generic;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class PositionPageViewModel : FilterContainerViewModel, IOrganisationItemsFilterPage
	{
		public PositionPageViewModel()
		{
			Title = "Должности";
			Filter = new PositionsFilterViewModel();

			OrganisationUIDs = new List<Guid>();
		}

		IReportFilterPosition _reportFilter;
		public bool IsWithDeleted { get; set; }
		public List<Guid> OrganisationUIDs { get; set; }
		public PositionsFilterViewModel Filter { get; private set; }
		OrganisationChangedSubscriber _OrganisationChangedSubscriber;

		public override void LoadFilter(SKDReportFilter filter)
		{
			_OrganisationChangedSubscriber = new OrganisationChangedSubscriber(this);
			_reportFilter = filter as IReportFilterPosition;
			var filterArchive = filter as IReportFilterArchive;
			IsWithDeleted = filterArchive != null && filterArchive.UseArchive;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			OrganisationUIDs = organisations != null ? organisations : new List<Guid>();
			InitializeFilter();
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var positionFilter = filter as IReportFilterPosition;
			if (positionFilter != null)
				positionFilter.Positions = Filter.UIDs;
		}

		public void InitializeFilter()
		{
			Filter.Initialize(_reportFilter == null ? null : _reportFilter.Positions, OrganisationUIDs, IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active);
		}

		public override void Unsubscribe()
		{
			_OrganisationChangedSubscriber.Unsubscribe();
			Filter.Unsubscribe();
		}
	}
}