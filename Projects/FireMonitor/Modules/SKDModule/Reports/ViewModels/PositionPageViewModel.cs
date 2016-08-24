using System;
using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.Services;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class PositionPageViewModel : FilterContainerViewModel, IOrganisationItemsFilterPage
	{
		public PositionPageViewModel()
		{
			Title = CommonResources.Positions;
			Filter = new PositionsFilterViewModel();
			ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
			_OrganisationChangedSubscriber = new OrganisationChangedSubscriber(this);
		}

		IReportFilterPosition _reportFilter;
		bool _isWithDeleted;
		public List<Guid> OrganisationUIDs { get; set; }
		public PositionsFilterViewModel Filter { get; private set; }
		OrganisationChangedSubscriber _OrganisationChangedSubscriber;

		public override void LoadFilter(SKDReportFilter filter)
		{
			_reportFilter = filter as IReportFilterPosition;
			var filterArchive = filter as IReportFilterArchive;
			_isWithDeleted = filterArchive != null && filterArchive.UseArchive;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			OrganisationUIDs = organisations ?? new List<Guid>();
			InitializeFilter();
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var positionFilter = filter as IReportFilterPosition;
			if (positionFilter != null)
				positionFilter.Positions = Filter.UIDs;
		}

		void OnUseArchive(bool isWithDeleted)
		{
			_isWithDeleted = isWithDeleted;
			InitializeFilter();
		}

		public void InitializeFilter()
		{
			Filter.Initialize(_reportFilter == null ? null : _reportFilter.Positions, OrganisationUIDs, _isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active);
		}
	}
}
