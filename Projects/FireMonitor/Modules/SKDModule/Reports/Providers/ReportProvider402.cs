using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider402 : FilteredSKDReportProvider<ReportFilter402>
	{
		public ReportProvider402()
            : base("Report402", "402. Маршрут сотрудника/посетителя", 402, SKDReportGroup.Events)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотруднки (Посетитель)" },
					{ "c02", "Организация" },
					{ "c03", "Отдел" },
					{ "c04", "Должность (Сопровождающий)" },
				},
                Pages = new List<FilterContainerViewModel>()
				{
					new ZonePageViewModel(),
                    new DoorPageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
            };
		}
	}
}
