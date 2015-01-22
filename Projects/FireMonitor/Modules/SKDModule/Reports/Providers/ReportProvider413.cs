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
					{ "c01", "Название" },
					{ "c02", "Из зоны" },
					{ "c03", "В зону" },
					{ "c04", "Вход" },
					{ "c05", "Выход" },
					{ "c06", "Тип пропуска" },
					{ "c07", "Сотрудник (Посетитель)" },
					{ "c08", "Организация" },
					{ "c09", "Отдел" },
					{ "c10", "Должность (Сопровождающий)" },				
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
