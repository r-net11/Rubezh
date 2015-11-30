using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DepartmentsReportProvider : FilteredSKDReportProvider<DepartmentsReportFilter>
	{
		public DepartmentsReportProvider()
			: base("Список подразделений организации", 415, SKDReportGroup.HR, PermissionType.Oper_Reports_Departments)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string>(),
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(false),
					new DepartmentPageViewModel(),
				},
			};
		}
	}
}