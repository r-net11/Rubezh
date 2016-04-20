using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class EventsReportProvider : FilteredSKDReportProvider<EventsReportFilter>
	{
		public EventsReportProvider()
			: base("Отчет по событиям", 401, SKDReportGroup.Events, PermissionType.Oper_Reports_Events)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "SystemDateTime", "Дата и время в системе" },
					{ "DeviceDateTime", "Дата и время на устройстве" },
					{ "Name", "Название" },
					{ "Object", "Объект" },
					{ "User", "Пользователь" },
					{ "System", "Подсистема" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new EventTypePageViewModel(),
					new SKDObjectPageViewModel(),
					new EmployeePageViewModel(),
					new UserPageViewModel(),
				},
			};
		}
	}
}