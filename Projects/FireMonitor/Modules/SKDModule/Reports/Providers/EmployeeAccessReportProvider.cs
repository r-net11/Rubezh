using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeAccessReportProvider : FilteredSKDReportProvider<EmployeeAccessReportFilter>
	{
		public EmployeeAccessReportProvider()
			: base("Доступ в зоны сотрудников/посетителей", 412, SKDReportGroup.HR, PermissionType.Oper_Reports_Employees_Access)
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
					new PassCardTypePageViewModel() { IsActive = true },
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