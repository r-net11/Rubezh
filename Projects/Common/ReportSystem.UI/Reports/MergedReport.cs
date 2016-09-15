using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using ReportSystem.Api.Interfaces;
using System.Drawing;
using System.Drawing.Printing;

namespace ReportSystem.UI.Reports
{
	/// <summary>
	/// Класс отчёта, который хранит вложенные отчёты-шаблоны пропусков.
	/// </summary>
	public partial class MergedReport : XtraReport
	{
		private int _locationY;
		private readonly IEnumerable<XtraReport> _mergeReports;

		public MergedReport(IEnumerable<XtraReport> reportsToMerge, IPaperKindSetting paperKindSetting)
		{
			InitializeComponent();
			_mergeReports = reportsToMerge;
			DefaultPrinterSettingsUsing.UseLandscape = false;
			Landscape = true;
			PaperKind = PaperKind.Custom;
			PageWidth = paperKindSetting.Width * 10; //TODO: find workaround
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
			SetReport(mergeReport, BandKind.Detail);
		}

		private void XtraReportMerged_BeforePrint(object sender, PrintEventArgs e)
		{
			if (_mergeReports == null) return;

			foreach (var mr in _mergeReports)
				MergeReport(mr);
		}
	}
}
