using System;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using ReportSystem.Api.Interfaces;
using System.Drawing;

namespace ReportSystem.UI.Reports
{
	public partial class MergedReport : XtraReport
	{
		private int _locationY;
		private readonly XtraReport[] _mergeReports;

		public MergedReport(XtraReport[] reportsToMerge, IPaperKindSetting paperKindSetting)
		{
			InitializeComponent();
			_mergeReports = reportsToMerge;
			DefaultPrinterSettingsUsing.UseLandscape = false;
			Landscape = true;
			PaperKind = PaperKind.Custom;
			PageWidth = paperKindSetting.Width * 10;
			PageHeight = paperKindSetting.Height * 10;
			//DefaultPrinterSettingsUsing.UseLandscape = false;
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

		private void XtraReportMerged_BeforePrint(object sender, PrintEventArgs e)
		{
			if (_mergeReports == null) return;

			foreach (var mr in _mergeReports)
				MergeReport(mr);
		}
	}
}
