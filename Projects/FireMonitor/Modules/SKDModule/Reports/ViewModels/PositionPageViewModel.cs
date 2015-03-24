using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class PositionPageViewModel : FilterContainerViewModel
	{
		public PositionPageViewModel()
		{
			Title = "Должность";
			Filter = new PositionsFilterViewModel();
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Unsubscribe(OnOrganisationChanged);
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Subscribe(OnOrganisationChanged);
		}

		IReportFilterPosition _reportFilter;
		bool _isWithDeleted;
		Guid _organisationUID;
		public PositionsFilterViewModel Filter { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			_reportFilter = filter as IReportFilterPosition;
			var filterArchive = filter as IReportFilterArchive;
			_isWithDeleted = filterArchive != null && filterArchive.UseArchive;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			_organisationUID = organisations != null ? organisations.FirstOrDefault() : Guid.Empty;
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
		void OnOrganisationChanged(Guid organisationUID)
		{
			_organisationUID = organisationUID;
			InitializeFilter();
		}

		void InitializeFilter()
		{
			Filter.Initialize(_reportFilter == null ? null : _reportFilter.Positions, new List<Guid> { _organisationUID }, _isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active);
		}
	}
}
