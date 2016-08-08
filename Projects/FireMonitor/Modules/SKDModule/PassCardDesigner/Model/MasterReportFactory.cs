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
		private DataSet _dataSet;

		public MasterReportFactory()
		{
			_masterReport = new XtraReport();
			_detailBand = new DetailBand();
			_detailBand.MultiColumn.ColumnSpacing = 10F;
			_masterReport.ReportUnit = ReportUnit.TenthsOfAMillimeter;
			_detailBand.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
			_detailBand.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
			_detailBand.MultiColumn.ColumnCount = 2;
			_masterReport.Bands.Add(_detailBand);

		}
		public void CreateMasterReport(DataSet datasSet, XtraReport inputReport)
		{
			_inputReport = inputReport;
			_dataSet = datasSet;
			_masterReport.DataSource = datasSet;
			_masterReport.ReportPrintOptions.DetailCount = 1;
		//	_masterReport.DataMember = datasSet.Tables[0].TableName;
			_masterReport.Margins = new Margins(50, 50, 50, 50);
		//	CreateDetailReportBasedOnSubreport(_masterReport);
			CreateDetailReportBasedOnDetailReportBand(_masterReport);
			_masterReport.ShowPreviewDialog();
		}

		/// <summary>
		/// Set filled dataset as report datasource
		/// </summary>
		/// <param name="dataSet"></param>
		public XtraReport SetDataSet(DataSet dataSet)
		{
			_masterReport.DataSource = dataSet;
			_masterReport.DataMember = dataSet.Tables[0].TableName;

			return _masterReport;
		}

		/// <summary>
		/// Set report and band height (based on report units)
		/// </summary>
		/// <param name="height">Input height in report units</param>
		public XtraReport SetHeight(int height)
		{
			_masterReport.Height = height;
			_detailBand.Height = height;

			return _masterReport;
		}

		/// <summary>
		/// Set report and band width (based on report units)
		/// </summary>
		/// <param name="width">Input width in report units</param>
		public XtraReport SetWith(int width)
		{
			_masterReport.Width = width;
			_detailBand.Width = width;

			return _masterReport;
		}

		/// <summary>
		/// Add next sub-report to master-report
		/// </summary>
		/// <returns>Master report with input sub-report</returns>
		public XtraReport AddNextSubreport(XtraReport report)
		{
			XRSubreport subreport = new XRSubreport();
			_detailBand.Controls.Add(subreport);

			return _masterReport;
		}

		private void CreateDetailReportBasedOnDetailReportBand(XtraReport report)
		{
			//Create a Detail Report
			DetailReportBand detailReport = new DetailReportBand();
			report.Bands.Add(detailReport);

			//Generate the DataMember based on data relations
			//NwindDataSet ds = report.DataSource as NwindDataSet;
	//		string detailDataMember = string.Format("{0}.{1}", ds.Tables[report.DataMember].TableName,
		//		ds.Relations[0].RelationName);

			//detailReport.DataSource = ds;
			//detailReport.DataMember = detailDataMember;
			detailReport.DataSource = report.DataSource;
			detailReport.DataMember = "Employees";

			//Create bands
		//	CreateReportHeader(detailReport, "Orders", Color.Gold, 16);
			var band = (DetailBand)_inputReport.Bands[BandKind.Detail];
			band.CanGrow = true;
			band.CanShrink = true;
			band.MultiColumn.ColumnSpacing = 10F;
			//detailReport.ReportUnit = ReportUnit.TenthsOfAMillimeter;
			band.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
			band.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
			band.MultiColumn.ColumnCount = 1;
			detailReport.Bands.Add(band);
		}

		private void CreateDetailReportBasedOnSubreport(XtraReport report)
		{
			//Create a Subreport
			XRSubreport subreport = new XRSubreport();

			XRSubreport subreport2 = new XRSubreport();
			XRSubreport subreport3 = new XRSubreport();
			DetailBand detailBand = report.Bands[BandKind.Detail] as DetailBand;
			detailBand.MultiColumn.ColumnSpacing = 10F;
			detailBand.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
			detailBand.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
			detailBand.Controls.Add(subreport);
		//	detailBand.Controls.Add(subreport2);
			//detailBand.Controls.Add(subreport3);

			subreport.LocationF = new PointF(0, 50);//detailBand.HeightF);
		//	subreport.WidthF = 200;// report.PageWidth - report.Margins.Right - report.Margins.Left;

		//	subreport2.LocationF = new PointF(650, 50);//detailBand.HeightF); uncomment
		//	subreport2.WidthF = 200;//report.PageWidth - report.Margins.Right - report.Margins.Left;

			//subreport3.LocationF = new PointF(650 + 650, 50); uncomment
			//subreport3.WidthF = 200;
			//Create a detail report
			XtraReport detailReport = new XtraReport {DataSource = _dataSet, DataMember = "Employees"};
			//	XtraReport detailReport2 = new XtraReport { DataSource = _dataSet, DataMember = "Employees" };uncomment
			//	XtraReport detailReport3 = new XtraReport { DataSource = _dataSet, DataMember = "Employees" }; uncomment

			subreport.ReportSource = detailReport;
			//	subreport2.ReportSource = detailReport2; uncomment
			//	subreport3.ReportSource = detailReport3; uncomment
			//Create bands
			detailReport.Bands.Add(_inputReport.Bands[BandKind.Detail]);
			//	detailReport2.Bands.Add(_inputReport.Bands[BandKind.Detail]);uncomment
			//	detailReport3.Bands.Add(_inputReport.Bands[BandKind.Detail]);uncomment

			//Add a parameter for filtering
			//var name = _dataSet.Tables[0].Rows[0]["LastName"].ToString();
			//var name2 = _dataSet.Tables[0].Rows[1]["LastName"].ToString();
			//var name3 = _dataSet.Tables[0].Rows[2]["LastName"].ToString();
			//Parameter param = new Parameter { Name = "LastName", Type = typeof(string), Visible = false, Value = name };
			//Parameter param2 = new Parameter { Name = "LastName", Type = typeof(string), Visible = false, Value = name2 };
			//Parameter param3 = new Parameter { Name = "LastName", Type = typeof(string), Visible = false, Value = name3 };

			//detailReport.Parameters.Add(param);

			//detailReport.FilterString = "[LastName] == [Parameters.LastName]";

			//detailReport2.Parameters.Add(param2);
			//detailReport2.FilterString = "[LastName] == [Parameters.LastName]";
			//detailReport3.Parameters.Add(param3);
			//detailReport3.FilterString = "[LastName] == [Parameters.LastName]";

			//Handle the subreport.BeforePrint event for filtering details dynamycally
			//subreport.BeforePrint += subreport_BeforePrint;
			//subreport2.BeforePrint += subreport_BeforePrint;
			//subreport3.BeforePrint += subreport_BeforePrint;

		//	_masterReport.CreateDocument();
		//_masterReport.Pages.Add(new PSPage(new PageData()));

			//foreach (var el in detailReport.Pages)
			//{
			//	PSPage page = (PSPage) el;
			//	page.Bricks
			//}
			//detailReport.Pages
		}

		void subreport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			XRSubreport subreport = sender as XRSubreport;
			XtraReport mainReport = subreport.Report as XtraReport;

			//String currentCustID = mainReport.GetCurrentColumnValue("FirstName").ToString();
			//subreport.ReportSource.Parameters["FirstName"].Value = currentCustID;
			//Obtain the current CustomerID value for filtering the detail report

			String currentCustID = mainReport.GetCurrentColumnValue("LastName").ToString();

			subreport.ReportSource.Parameters["LastName"].Value = currentCustID;
		}

	}
}
