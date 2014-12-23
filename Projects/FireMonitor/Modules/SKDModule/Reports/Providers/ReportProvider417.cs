using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
	public class ReportProvider417 : FilteredSKDReportProvider<TestReportFilter>
	{
		public ReportProvider417()
            : base("FilteredTestReport", "417. Отчет о местонахождении персонала", 417, SKDReportGroup.HR)
		{
		}

		public override FilterModel CreateFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Зона" },
					{ "c02", "Дата и время входа" },
					{ "c03", "Сотрудник (Посетитель)" },
					{ "c04", "Организация" },
					{ "c05", "Отдел" },
					{ "c06", "Должность (Сопровождающий)" },
				},
			};
		}
	}
}
