using Common;
using FiresecAPI;
using FiresecAPI.Enums;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.Export;
using FiresecService.Report.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FiresecService.Report.Helpers
{
	public static class ReportingHelpers
	{
		public static List<string> GetReportNames()
		{
			var result = Assembly.Load("StrazhService.Report").GetTypes()
				.Where(x => x.BaseType != null && x.BaseType.Name == "BaseReport")
				.Select(type => ((BaseReport)Activator.CreateInstance(type)).ReportTitle)
				.ToList();

			return result;
		}

		public static OperationResult ExportReport(ReportExportFilter filter)
		{
			var exporter = new ReportExporter(filter);
			exporter.Execute();
			return new OperationResult();
		}
	}
}
