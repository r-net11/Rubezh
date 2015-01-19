using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;

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

		public override void LoadFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
			Filter.Initialize(employeeFilter == null ? null : employeeFilter.Employees, FiresecAPI.SKD.LogicalDeletationType.Active);
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var employeeFilter = filter as IReportFilterEmployee;
			if (employeeFilter != null)
				employeeFilter.Employees = Filter.UIDs;
		}
	}
}
