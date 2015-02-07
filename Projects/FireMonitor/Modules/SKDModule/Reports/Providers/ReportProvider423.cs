using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider423 : FilteredSKDReportProvider<ReportFilter423>
	{
		public ReportProvider423()
			: base("Report423", "Отчет по оправдательным документам", 423, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", "Сотрудник" },
					{ "StartDateTime", "Дата начала" },
					{ "EndDateTime", "Дата окончания" },
					//{ "StartDateTime2", "Время начала" },
					//{ "EndDateTime2", "Время окончания" },
					{ "DocumentType", "Тип" },
					{ "DocumentName", "Документ" },
					{ "DocumentShortName", "Буквенный код" },
					{ "DocumentCode", "Числовой код" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(false),
					new DepartmentPageViewModel(),
					new EmployeePageViewModel(),
					new DocumentFilterPageViewModel(),
				},
			};
		}
	}
}
