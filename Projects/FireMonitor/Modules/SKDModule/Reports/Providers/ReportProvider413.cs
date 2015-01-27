using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider413 : FilteredSKDReportProvider<ReportFilter413>
	{
		public ReportProvider413()
            : base("Report413", "413. Права доступа сотрудников/посетителей", 413, SKDReportGroup.HR)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "AccessPoint", "Название" },
					{ "ZoneOut", "Из зоны" },
					{ "ZoneIn", "В зону" },
					{ "Enter", "Вход" },
					{ "Exit", "Выход" },
					{ "Type", "Тип пропуска" },
					{ "Employee", "Сотрудник (Посетитель)" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность (Сопровождающий)" },				
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel(),
					new ZonePageViewModel(),
					new SchedulePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}
