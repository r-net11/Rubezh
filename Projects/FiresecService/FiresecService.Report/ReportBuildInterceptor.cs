using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.UI;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.ComponentModel.Composition;

namespace FiresecService.Report
{
	[Export(typeof(IReportBuildInterceptor))]
	public class ReportBuildInterceptor : IReportBuildInterceptor
	{
		#region IReportBuildInterceptor Members

		public void InvokeAfter(XtraReport report, object customArgs)
		{
			report.Disposed += (s, e) =>
			{
				var rep = (XtraReport)report;
				var dataSource = report.DataSource as IDisposable;
				if (dataSource != null)
					dataSource.Dispose();
				report.DataSource = null;
			};
		}

		public void InvokeBefore(XtraReport report, object customArgs)
		{
			var filteredReport = report as IFilteredReport;
			if (filteredReport != null)
				filteredReport.ApplyFilter(customArgs as SKDReportFilter);
		}

		#endregion IReportBuildInterceptor Members
	}
}