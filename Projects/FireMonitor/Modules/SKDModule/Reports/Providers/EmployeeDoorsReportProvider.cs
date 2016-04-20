using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EmployeeDoorsReportProvider : FilteredSKDReportProvider<EmployeeDoorsReportFilter>
	{
		public EmployeeDoorsReportProvider()
			: base("Права доступа сотрудников/посетителей", 413, SKDReportGroup.HR, PermissionType.Oper_Reports_Employees_Rights)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "NoDoor", "Название" },
					{ "NoExitZone", "Из зоны" },
					{ "NoEnterZone", "В зону" },
					{ "Enter", "Вход" },
					{ "Exit", "Выход" },
					{ "Type", "Тип пропуска" },
					{ "Employee", "Сотрудник (Посетитель)" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность (Сопровождающий)" },				
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel() { IsActive = false },
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