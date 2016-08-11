using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;

namespace SKDModule.PassCardDesigner.Model
{
	public class MasterReportFactory
	{
		private readonly XtraReport _masterReport;
		private readonly DetailBand _detailBand;
		private XtraReport _inputReport;
		public MasterReportFactory()
		{
			_masterReport = new XtraReport();
			_detailBand = new DetailBand();
			_masterReport.ReportUnit = ReportUnit.TenthsOfAMillimeter;
			_masterReport.Bands.Add(_detailBand);
		}

		public void CreateMasterReport(DataSet datasSet, XtraReport inputReport)
		{
			_inputReport = inputReport;
			_masterReport.DataSource = datasSet;
			_masterReport.ReportPrintOptions.DetailCount = 1;
			_masterReport.Margins = new Margins(50, 50, 50, 50);
			CreateDetailReportBasedOnDetailReportBand();
			_masterReport.ShowPreviewDialog();
		}

		private void CreateDetailReportBasedOnDetailReportBand()
		{
			//Create a Detail Report
			var detailReport = new DetailReportBand();
			_masterReport.Bands.Add(detailReport);

			detailReport.DataSource = _masterReport.DataSource;
			detailReport.DataMember = "Employees";

			//Create bands
			var band = (DetailBand)_inputReport.Bands[BandKind.Detail];
			band.CanGrow = true;
			band.CanShrink = true;
			band.MultiColumn.ColumnSpacing = 50F;
			band.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
			band.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
			band.MultiColumn.ColumnCount = 3;
			detailReport.Bands.Add(band);
		}
	}
}
