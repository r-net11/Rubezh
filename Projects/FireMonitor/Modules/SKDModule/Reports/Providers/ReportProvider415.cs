using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider415 : FilteredSKDReportProvider<ReportFilter415>
	{
		public ReportProvider415()
            : base("Report415", "415. Список подразделений организации", 415, SKDReportGroup.HR)
		{

		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Department", "Подразделение" },
					{ "Phone", "Телефон" },
					{ "Chief", "Руководитель" },
					{ "ParentDepartment", "Подразделение" },
					{ "Description", "Примечание" },
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
