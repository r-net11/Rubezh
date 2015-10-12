using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeReportProvider : FilteredSKDReportProvider<EmployeeReportFilter>
	{
		public EmployeeReportProvider()
			: base("Справка о сотруднике/посетителе", 418, SKDReportGroup.HR, PermissionType.Oper_Reports_Employee)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", "Сотрудник (Посетитель)" },
					{ "Number", "Табельный номер (Примечание)" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность (Сопровождающий)" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}
