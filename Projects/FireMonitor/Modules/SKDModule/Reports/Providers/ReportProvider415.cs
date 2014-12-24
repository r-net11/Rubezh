using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider415 : FilteredSKDReportProvider<ReportFilter415>
	{
		public ReportProvider415()
			: base("Report415", "415. Отчет \"Список отделов организации\"", 415, SKDReportGroup.HR)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Отдел" },
					{ "c02", "Телефон" },
					{ "c03", "Руководитель" },
					{ "c04", "Подразделение" },
					{ "c05", "Примечание" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(false),
					new DepartmentPageViewModel(),
				},
			};
		}
	}
}
