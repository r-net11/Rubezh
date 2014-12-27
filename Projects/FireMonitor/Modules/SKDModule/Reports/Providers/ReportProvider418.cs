using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider418 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider418()
            : base("FilteredTestReport", "418. Справка о сотруднике (посетителе)", 418, SKDReportGroup.HR)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Сотруднки (Посетитель)" },
					{ "c02", "Табельный номер (Примечание)" },
					{ "c03", "Организация" },
                    { "c04", "Отдел" },
					{ "c05", "Должность (Сопровождающий)" },
				},
			};
		}
	}
}
