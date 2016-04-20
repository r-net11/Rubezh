using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class CardsReportProvider : FilteredSKDReportProvider<CardsReportFilter>
	{
		public CardsReportProvider()
			: base("Сведения о пропусках", 411, SKDReportGroup.HR, PermissionType.Oper_Reports_Cards)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Type", "Статус" },
					{ "Number", "Номер" },
					{ "Organisation", "Организация" },
					{ "Department", "Подразделение" },
					{ "Position", "Должность" },
					{ "Employee", "Сотрудник" },
					{ "Period", "Срок действия" },
				},
				MainViewModel = new PassCardMainPageViewModel(),
				Pages = new List<FilterContainerViewModel>()
				{
					new PassCardTypePageViewModel(),
					new OrganizationPageViewModel(true),
					new DepartmentPageViewModel(),
					new PositionPageViewModel(),
					new EmployeePageViewModel(),
				},
			};
		}
	}
}