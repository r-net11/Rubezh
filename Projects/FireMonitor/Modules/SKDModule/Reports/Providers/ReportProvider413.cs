using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider413 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider413()
            : base("FilteredTestReport", "413. Отчет по правам доступа", 413, SKDReportGroup.HR)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Название" },
					{ "c02", "Из зоны" },
					{ "c03", "В зону" },
					{ "c04", "Вход" },
					{ "c05", "Выход" },
					{ "c06", "Тип пропуска" },
					{ "c07", "Сотрудник (Посетитель)" },
					{ "c08", "Организация" },
					{ "c09", "Отдел" },
					{ "c10", "Должность (Сопровождающий)" },				
				},
			};
		}
	}
}
