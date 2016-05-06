using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeDoorsReportProvider : FilteredSKDReportProvider<EmployeeDoorsReportFilter>
	{
		public EmployeeDoorsReportProvider()
			: base("Права доступа сотрудников/посетителей", 413, SKDReportGroup.HR, PermissionType.Oper_Reports_Employees_Rights)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel
			{
				Columns = new Dictionary<string, string>
				{
					{ "AccessPoint", "Название" },
					{ "ZoneOut", "Из зоны" },
					{ "ZoneIn", "В зону" },
					{ "Enter", "Вход" },
					{ "Exit", "Выход" },
					{ "Type", "Тип пропуска" },
					{ "Employee", "Сотрудник (Посетитель)" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность (Сопровождающий)" },
				},
				Pages = new List<FilterContainerViewModel>
				{
					new PassCardTypePageViewModel() { IsActive = true },
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