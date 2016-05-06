using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;
using StrazhAPI.Models;

namespace SKDModule.Reports.Providers
{
	public class EventsReportProvider : FilteredSKDReportProvider<EventsReportFilter>
	{
		public EventsReportProvider()
			: base("Отчет по событиям системы контроля доступа", 401, SKDReportGroup.Events, PermissionType.Oper_Reports_Events)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel
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
				Pages = new List<FilterContainerViewModel>
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