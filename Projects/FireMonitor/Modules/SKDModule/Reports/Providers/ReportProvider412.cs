using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider412 : FilteredSKDReportProvider<ReportFilter412>
	{
		public ReportProvider412()
			: base("Report412", "412. Доступ в зоны сотрудников/посетителей", 412, SKDReportGroup.HR)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Zone", "Зона" },
					{ "Type", "Тип пропуска" },
					{ "Number", "Номер пропуска" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность" },
					{ "Employee", "Сотрудник" },
					{ "Template", "Шаблон доступа" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel(),
					new ZonePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}
