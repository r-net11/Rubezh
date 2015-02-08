using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeRootReportProvider : FilteredSKDReportProvider<EmployeeRootReportFilter>
	{
		public EmployeeRootReportProvider()
			: base("Маршрут сотрудника/посетителя", 402, SKDReportGroup.Events, PermissionType.Oper_Reports_EmployeeRoot)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Name", "Сотруднки (Посетитель)" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность (Сопровождающий)" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new ZonePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}
