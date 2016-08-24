using Localization.SKD.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.Services;
using Infrastructure.Common.SKDReports;
using Infrastructure.Events;
using SKDModule.ViewModels;
using System;
using System.Collections.Generic;

namespace SKDModule.Reports.ViewModels
{
	public class DepartmentPageViewModel : FilterContainerViewModel, IOrganisationItemsFilterPage
	{
		public DepartmentPageViewModel()
		{
			Title = CommonResources.Departments;
			Filter = new DepartmentsFilterViewModel();
			ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactoryBase.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
			_OrganisationChangedSubscriber = new OrganisationChangedSubscriber(this);
		}

		IReportFilterDepartment _reportFilter;
		public List<Guid> OrganisationUIDs { get; set; }
		bool _isWithDeleted;
		public DepartmentsFilterViewModel Filter { get; private set; }
		OrganisationChangedSubscriber _OrganisationChangedSubscriber;

		public override void LoadFilter(SKDReportFilter filter)
		{
			_reportFilter = filter as IReportFilterDepartment;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			OrganisationUIDs = organisations ?? new List<Guid>();
			var filterArchive = filter as IReportFilterArchive;
			_isWithDeleted = filterArchive != null && filterArchive.UseArchive;
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

		public void InitializeFilter()
		{
			Filter.Initialize(_reportFilter == null ? null : _reportFilter.Departments, OrganisationUIDs, _isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active);
		}
	}

	public interface IOrganisationItemsFilterPage
	{
		List<Guid> OrganisationUIDs { get; set; }
		void InitializeFilter();
	}

	public class OrganisationChangedSubscriber
	{
		IOrganisationItemsFilterPage _parent;

		public OrganisationChangedSubscriber(IOrganisationItemsFilterPage parent)
		{
			_parent = parent;
			ServiceFactoryBase.Events.GetEvent<SKDReportOrganisationChangedEvent>().Unsubscribe(OnOrganisationChanged);
			ServiceFactoryBase.Events.GetEvent<SKDReportOrganisationChangedEvent>().Subscribe(OnOrganisationChanged);
		}

		void OnOrganisationChanged(List<Guid> organisationUIDs)
		{
			_parent.OrganisationUIDs = organisationUIDs;
			_parent.InitializeFilter();
		}

	}
}
