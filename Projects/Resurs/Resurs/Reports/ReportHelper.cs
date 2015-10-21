using DevExpress.XtraReports.UI;
using Resurs.Reports.Templates;

namespace Resurs.Reports
{
	public static class ReportHelper
	{
		public static XtraReport GetDefaultReport(ReportType reportType)
		{
			XtraReport defaultReport;
			switch (reportType)
			{
				case ReportType.ChangeFlow:
					defaultReport = new ChangeFlowReport(); break;
				case ReportType.Debtors:
					defaultReport = new DebtorsReport(); break;
				case ReportType.Receipts:
					defaultReport = new ReceiptsReport(); break;
				case ReportType.ChangeValue:
					defaultReport = new ChangeValueReport(); break;
				default:
					defaultReport = new XtraReport(); break;
			}
			return defaultReport;
		}
	}
}