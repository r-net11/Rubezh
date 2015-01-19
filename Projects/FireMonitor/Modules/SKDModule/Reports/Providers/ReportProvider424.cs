using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider424 : FilteredSKDReportProvider<ReportFilter424>
	{
		public ReportProvider424()
			: base("Report424", "424. Справка по отработанному времени", 424, SKDReportGroup.TimeTracking)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотрудник" },
					{ "c02", "Отработанное время" },
					{ "c03", "Организация" },
					{ "c04", "Отдел" },
					{ "c05", "Должность" },
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
