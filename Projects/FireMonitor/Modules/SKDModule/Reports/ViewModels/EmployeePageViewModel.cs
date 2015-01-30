using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.Reports.ViewModels
{
	public class EmployeePageViewModel : FilterContainerViewModel
	{
		public EmployeePageViewModel()
		{
			Title = "Сотрудник";
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
                    Filter.Initialize(new List<Guid>(), FiresecAPI.SKD.LogicalDeletationType.Active, IsEmployee ? PersonType.Employee : PersonType.Guest);
            }
        }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
            AllowVisitor = employeeFilter != null && employeeFilter is IReportFilterEmployeeAndVisitor;
            _isEmployee = AllowVisitor ? ((IReportFilterEmployeeAndVisitor)employeeFilter).IsEmployee : true;
            OnPropertyChanged(() => IsEmployee);
            Filter.Initialize(employeeFilter == null ? null : employeeFilter.Employees, FiresecAPI.SKD.LogicalDeletationType.Active, IsEmployee ? PersonType.Employee : PersonType.Guest);
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
			if (employeeFilter != null)
				employeeFilter.Employees = Filter.UIDs;
            if (AllowVisitor)
                ((IReportFilterEmployeeAndVisitor)employeeFilter).IsEmployee = IsEmployee;
		}
	}
}