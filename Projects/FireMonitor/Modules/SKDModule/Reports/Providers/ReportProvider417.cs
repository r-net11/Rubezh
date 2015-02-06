using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider417 : FilteredSKDReportProvider<ReportFilter417>
	{
		public ReportProvider417()
			: base("Report417", "417. Местонахождение сотрудников/посетителей", 417, SKDReportGroup.HR)
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
