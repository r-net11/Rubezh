using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Service.Extensions;
using System.ComponentModel.Composition;
using DevExpress.XtraReports.UI;

namespace FiresecService.Report
{
	[Export(typeof(IReportBuildInterceptor))]
	public class ReportBuildInterceptor : IReportBuildInterceptor
	{
		#region IReportBuildInterceptor Members

		public void InvokeAfter(XtraReport report, object customArgs)
		{
		}

		public void InvokeBefore(XtraReport report, object customArgs)
		{
			var filteredReport = report as IFilteredReport;
			if (filteredReport != null)
				filteredReport.ApplyFilter(customArgs);
		}

		#endregion
	}
}
