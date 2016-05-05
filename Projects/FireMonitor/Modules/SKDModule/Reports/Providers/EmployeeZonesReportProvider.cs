using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeZonesReportProvider : FilteredSKDReportProvider<EmployeeZonesReportFilter>
	{
		public EmployeeZonesReportProvider()
			: base("Местонахождение сотрудников/посетителей", 417, SKDReportGroup.HR, PermissionType.Oper_Reports_EmployeeZone)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Zone", "Зона" },
					{ "EnterDateTime", "Дата и время входа" },
					{ "Employee", "Сотрудник (Посетитель)" },
					{ "Orgnisation", "Организация" },
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
				MainViewModel = new PlacementMainPageViewModel(),
			};
		}
	}
}