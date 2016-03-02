using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DepartmentsReportProvider : FilteredSKDReportProvider<DepartmentsReportFilter>
	{
		public DepartmentsReportProvider()
			: base("Список подразделений организации", 415, SKDReportGroup.HR, PermissionType.Oper_Reports_Departments)
		{

		}

		public override FilterModel GetFilterModel()
		{
			var organisationPage = new OrganizationPageViewModel(false);
			organisationPage.CheckFirstOrganisation(Filter);

			return  new FilterModel
			{
				Columns = new Dictionary<string, string>(),
				Pages = new List<FilterContainerViewModel>
				{
					organisationPage,
					new DepartmentPageViewModel(),
				},
			};
		}
	}
}
