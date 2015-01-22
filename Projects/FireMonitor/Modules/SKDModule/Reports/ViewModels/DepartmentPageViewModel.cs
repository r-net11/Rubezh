using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;

namespace SKDModule.Reports.ViewModels
{
	public class DepartmentPageViewModel : FilterContainerViewModel
	{
		public DepartmentPageViewModel()
		{
			Title = "Подразделения";
			Filter = new DepartmentsFilterViewModel();
		}

		public DepartmentsFilterViewModel Filter { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var departmentFilter = filter as IReportFilterDepartment;
			Filter.Initialize(departmentFilter == null ? null : departmentFilter.Departments, FiresecAPI.SKD.LogicalDeletationType.Active);
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var departmentFilter = filter as IReportFilterDepartment;
			if (departmentFilter != null)
				departmentFilter.Departments = Filter.UIDs;
		}
	}
}
