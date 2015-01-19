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
					{ "c01", "Зона" },
					{ "c02", "Дата и время входа" },
					{ "c03", "Сотрудник (Посетитель)" },
					{ "c04", "Организация" },
					{ "c05", "Отдел" },
					{ "c06", "Должность (Сопровождающий)" },
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
