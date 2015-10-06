using DevExpress.XtraReports.UI;
using Resurs.Reports.Templates;
using Resurs.ViewModels;

namespace Resurs.Reports
{
	public static class ReportHelper
	{
		public static XtraReport GetDefaultReport(ReportType reportType)
		{
			XtraReport defaultReport = null;
			switch (reportType)
			{
				case ReportType.ChangeFlow:
					defaultReport = new ChangeFlowReport(ReportsViewModel.Filter); break;
				case ReportType.Debtors:
					defaultReport = new DebtorsReport(); break;
			}
			return defaultReport;
		}
	}
}