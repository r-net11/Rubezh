using System;
using System.Collections.Generic;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class EmployeePageViewModel : FilterContainerViewModel, IOrganisationItemsFilterPage
	{
		public EmployeePageViewModel()
		{
			Title = "Сотрудники";
			Filter = new EmployeesFilterViewModel();
		}

		public EmployeesFilterViewModel Filter { get; private set; }
		OrganisationChangedSubscriber _OrganisationChangedSubscriber;
		public List<Guid> OrganisationUIDs { get; set; }
		public bool IsWithDeleted { get; set; }
		bool _allowVisitor;
		public bool AllowVisitor
		{
			get { return _allowVisitor; }
			set
			{
				_allowVisitor = value;
				OnPropertyChanged(() => AllowVisitor);
			}
		}
		bool _isEmployee;
		public bool IsEmployee
		{
			get { return _isEmployee; }
			set
			{
				_isEmployee = value;
				OnPropertyChanged(() => IsEmployee);
				if (AllowVisitor)
					Filter.Initialize(new List<Guid>(), RubezhAPI.SKD.LogicalDeletationType.Active, IsEmployee ? PersonType.Employee : PersonType.Guest);
				Title = value ? "Сотрудники" : "Посетители";
			}
		}

		IReportFilterEmployee _employeeFilter;

		public override void LoadFilter(SKDReportFilter filter)
		{
			_OrganisationChangedSubscriber = new OrganisationChangedSubscriber(this);
			_employeeFilter = filter as IReportFilterEmployee;
			if (_employeeFilter == null)
				return;
			AllowVisitor = _employeeFilter is IReportFilterEmployeeAndVisitor;
			_isEmployee = AllowVisitor ? ((IReportFilterEmployeeAndVisitor)_employeeFilter).IsEmployee : true;
			var organisations = (filter as IReportFilterOrganisation).Organisations;
			OrganisationUIDs = organisations != null ? organisations : new List<Guid>();
			var filterArchive = filter as IReportFilterArchive;
			IsWithDeleted = filterArchive != null && filterArchive.UseArchive;
			OnPropertyChanged(() => IsEmployee);
			InitializeFilter();
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
			if (employeeFilter == null)
				return;
			employeeFilter.Employees = Filter.UIDs;
			employeeFilter.IsSearch = Filter.IsSearch;
			if (Filter.IsSearch)
			{
				employeeFilter.LastName = Filter.LastName;
				employeeFilter.FirstName = Filter.FirstName;
				employeeFilter.SecondName = Filter.SecondName;
			}
			if (AllowVisitor)
				((IReportFilterEmployeeAndVisitor)employeeFilter).IsEmployee = IsEmployee;
		}

		public void InitializeFilter()
		{
			var filter = new EmployeeFilter()
			{
				PersonType = IsEmployee ? PersonType.Employee : PersonType.Guest,
				LogicalDeletationType = LogicalDeletationType.Active,
				OrganisationUIDs = OrganisationUIDs
			};
			if (_employeeFilter.IsSearch)
			{
				filter.LastName = _employeeFilter.LastName;
				filter.FirstName = _employeeFilter.FirstName;
				filter.SecondName = _employeeFilter.SecondName;
			}
			else
				filter.UIDs = Filter.UIDs;
			Filter.Initialize(filter);
		}

		public override void Unsubscribe()
		{
			Filter.Unsubscribe();
			_OrganisationChangedSubscriber.Unsubscribe();
		}
	}
}