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
	public class DepartmentPageViewModel : FilterContainerViewModel, IOrganisationItemsFilterPage
	{
		public DepartmentPageViewModel()
		{
			Title = "Подразделения";
			Filter = new DepartmentsFilterViewModel();
			OrganisationUIDs = new List<Guid>();
		}

		IReportFilterDepartment _reportFilter;
		public List<Guid> OrganisationUIDs { get; set; }
		public bool IsWithDeleted { get; set; }
		public DepartmentsFilterViewModel Filter { get; private set; }
		OrganisationChangedSubscriber _OrganisationChangedSubscriber;

		public override void LoadFilter(SKDReportFilter filter)
		{
			_OrganisationChangedSubscriber = new OrganisationChangedSubscriber(this);
			_reportFilter = filter as IReportFilterDepartment;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			OrganisationUIDs = organisations != null ? organisations : new List<Guid>();
			var filterArchive = filter as IReportFilterArchive;
			IsWithDeleted = filterArchive != null && filterArchive.UseArchive;
			InitializeFilter();
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var departmentFilter = filter as IReportFilterDepartment;
			if (departmentFilter != null)
				departmentFilter.Departments = Filter.UIDs;
		}
		public void InitializeFilter()
		{
			Filter.Initialize(_reportFilter == null ? null : _reportFilter.Departments, OrganisationUIDs, IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active);
		}

		public override void Unsubscribe()
		{
			_OrganisationChangedSubscriber.Unsubscribe();
			Filter.Unsubscribe();
		}
	}
}