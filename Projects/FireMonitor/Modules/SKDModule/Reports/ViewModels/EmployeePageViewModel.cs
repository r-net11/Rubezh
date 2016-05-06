using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class EmployeePageViewModel : FilterContainerViewModel
	{
		public EmployeePageViewModel()
		{
			Title = "Сотрудники";
			Filter = new EmployeesFilterViewModel();
		}

		public EmployeesFilterViewModel Filter { get; private set; }

		private bool _allowVisitor;

		public bool AllowVisitor
		{
			get { return _allowVisitor; }
			set
			{
				_allowVisitor = value;
				OnPropertyChanged(() => AllowVisitor);
			}
		}

		private bool _isEmployee;

		public bool IsEmployee
		{
			get { return _isEmployee; }
			set
			{
				_isEmployee = value;
				OnPropertyChanged(() => IsEmployee);

				if (AllowVisitor)
					Filter.Initialize(new List<Guid>(), LogicalDeletationType.Active,
						IsEmployee ? PersonType.Employee : PersonType.Guest);
				Title = value ? "Сотрудники" : "Посетители";
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
			if (employeeFilter == null)
				return;

			AllowVisitor = employeeFilter is IReportFilterEmployeeAndVisitor;
			_isEmployee = !AllowVisitor || ((IReportFilterEmployeeAndVisitor)employeeFilter).IsEmployee;
			OnPropertyChanged(() => IsEmployee);
			Filter.Initialize(CreateEmployeeFilter(employeeFilter));
		}

		public EmployeeFilter CreateEmployeeFilter(IReportFilterEmployee employeeFilter)
		{
			return new EmployeeFilter
			{
				PersonType = IsEmployee ? PersonType.Employee : PersonType.Guest,
				LogicalDeletationType = LogicalDeletationType.Active,
				UIDs = employeeFilter.IsSearch ? SearchEmployees() : employeeFilter.Employees,
			};
		}

		public override void UpdateFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
			if (employeeFilter == null)
				return;

			employeeFilter.IsSearch = Filter.IsSearch;
			employeeFilter.Employees = Filter.IsSearch ? SearchEmployees() : Filter.UIDs;

			if (AllowVisitor)
				((IReportFilterEmployeeAndVisitor)employeeFilter).IsEmployee = IsEmployee;
		}

		public List<Guid> SearchEmployees()
		{
			return Filter.Organisations == null
				? null
				: (
				from x in Filter.Organisations
				from child in x.Children
				where child.Model != null
				where !string.IsNullOrEmpty(Filter.FirstName) && child.Model.FirstName.Contains(Filter.FirstName)
					|| !string.IsNullOrEmpty(Filter.SecondName) && child.Model.SecondName.Contains(Filter.SecondName)
					|| !string.IsNullOrEmpty(Filter.LastName) && child.Model.LastName.Contains(Filter.LastName)
						select child.UID
				).ToList();
		}
	}
}