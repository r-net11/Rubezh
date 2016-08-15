using System;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System.Drawing;

namespace ReportSystem
{
	public partial class MergedReport : XtraReport
	{
		private int _locationY;
		private readonly XtraReport[] mergeReports;

		public MergedReport(XtraReport[] reportsToMerge)
		{
			InitializeComponent();
			mergeReports = reportsToMerge;
		}

		public XRSubreport SetReport(XtraReport report, BandKind band)
		{
			var subreport = new XRSubreport { ReportSource = report };
			Bands[band].Controls.Add(subreport);
			subreport.Location = new Point(0, _locationY);
			_locationY += subreport.Height + 1;

			return subreport;
		}

		private void MergeReport(XtraReport mergeReport)
		{
			var subreport = SetReport(mergeReport, BandKind.Detail);
		}

		private void XtraReportMerged_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			if (mergeReports == null) return;

			foreach (var mr in mergeReports)
				MergeReport(mr);
		}
	}
}
