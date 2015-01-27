using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider401 : FilteredSKDReportProvider<ReportFilter401>
	{
		public ReportProvider401()
            : base("Report401", "401. Отчет по событиям системы контроля доступа", 401, SKDReportGroup.Events)
		{
		}

		public override FilterModel GetFilterModel()
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
				},
            };
		}
	}
}
