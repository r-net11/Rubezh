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
	public class DepartmentPageViewModel : FilterContainerViewModel
	{
		public DepartmentPageViewModel()
		{
			Title = "Подразделения";
			Filter = new DepartmentsFilterViewModel();
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Unsubscribe(OnOrganisationChanged);
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Subscribe(OnOrganisationChanged);
		}

		IReportFilterDepartment _reportFilter;
		Guid _organisationUID;
		bool _isWithDeleted;
		public DepartmentsFilterViewModel Filter { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			_reportFilter = filter as IReportFilterDepartment;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			_organisationUID = organisations != null ? organisations.FirstOrDefault() : Guid.Empty;
			_isWithDeleted = (filter as IReportFilterArchive).UseArchive;
			InitializeFilter();
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var departmentFilter = filter as IReportFilterDepartment;
			if (departmentFilter != null)
				departmentFilter.Departments = Filter.UIDs;
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
			Filter.Initialize(_reportFilter == null ? null : _reportFilter.Departments, new List<Guid> { _organisationUID }, _isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active);
		}
	}
}
