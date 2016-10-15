using FiresecService.Report.DataSources;
using Localization.FiresecService.Report.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using StrazhDAL.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeZonesReport : BaseReport
	{
		public EmployeeZonesReport()
		{
			InitializeComponent();
		}

		private void CreateDetails()
		{
			var filter = GetFilter<EmployeeZonesReportFilter>();

			var resources = new System.ComponentModel.ComponentResourceManager(typeof(EmployeeZonesReport));
			var detail = new DevExpress.XtraReports.UI.DetailBand();
			var xrTableContent = new DevExpress.XtraReports.UI.XRTable();
			var xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			var xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
			DevExpress.XtraReports.UI.XRTableCell xrTableCellExit = null;
			var xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
			var groupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			var xrTableHeader = new DevExpress.XtraReports.UI.XRTable();
			var xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			var xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			DevExpress.XtraReports.UI.XRTableCell xrTableCellExitHeader = null;
			var xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
			var xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(xrTableContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(xrTableHeader)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			//
			// Detail
			//
			detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			xrTableContent});
			detail.Dpi = 254F;
			detail.HeightF = 64F;
			detail.Name = "Detail";
			//
			// xrTableContent
			//
			xrTableContent.Borders = (DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
									 | DevExpress.XtraPrinting.BorderSide.Bottom;
			xrTableContent.Dpi = 254F;
			xrTableContent.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			xrTableContent.Name = "xrTableContent";
			xrTableContent.OddStyleName = "xrControlStyle1";
			xrTableContent.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			xrTableContent.Rows.AddRange(new[] { xrTableRow2 });
			xrTableContent.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			xrTableContent.StylePriority.UseBorders = false;
			xrTableContent.StylePriority.UsePadding = false;
			xrTableContent.StylePriority.UseTextAlignment = false;
			xrTableContent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// xrTableRow2
			//
			if (filter.UseCurrentDate)
			{
				xrTableRow2.Cells.AddRange(new[] {
					xrTableCell9,
					xrTableCell10,
					xrTableCell12,
					xrTableCell13,
					xrTableCell14,
					xrTableCell15,
					xrTableCell16});
			}
			else
			{
				xrTableCellExit = new DevExpress.XtraReports.UI.XRTableCell();
				xrTableRow2.Cells.AddRange(new[] {
					xrTableCell9,
					xrTableCell10,
					xrTableCellExit,
					xrTableCell12,
					xrTableCell13,
					xrTableCell14,
					xrTableCell15,
					xrTableCell16});
			}
			xrTableRow2.Dpi = 254F;
			xrTableRow2.Name = "xrTableRow2";
			xrTableRow2.Weight = 11.5D;
			//
			// xrTableCell9
			//
			xrTableCell9.DataBindings.AddRange(new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Zone") });
			xrTableCell9.Dpi = 254F;
			xrTableCell9.Name = "xrTableCell9";
			xrTableCell9.Weight = 0.15384615384615386D;
			//
			// xrTableCell10
			//
			xrTableCell10.DataBindings.AddRange(new[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.EnterDateTime", "{0:dd.MM.yyyy HH:mm:ss}")});
			xrTableCell10.Dpi = 254F;
			xrTableCell10.Name = "xrTableCell10";
			xrTableCell10.Weight = 0.10056871617686532D;
			//
			// xrTableCellExit
			//
			if (!filter.UseCurrentDate)
			{
				if (xrTableCellExit != null)
				{
					xrTableCellExit.DataBindings.AddRange(new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ExitDateTime", "{0:dd.MM.yyyy HH:mm:ss}") });
					xrTableCellExit.Dpi = 254F;
					xrTableCellExit.Name = "xrTableCellExit";
					xrTableCellExit.Weight = 0.10056870156204054D;
				}
			}
			//
			// xrTableCell12
			//
			xrTableCell12.DataBindings.AddRange(new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Period") });
			xrTableCell12.Dpi = 254F;
			xrTableCell12.Name = "xrTableCell12";
			xrTableCell12.Weight = 0.105D;
			//
			// xrTableCell13
			//
			xrTableCell13.DataBindings.AddRange(new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Employee") });
			xrTableCell13.Dpi = 254F;
			xrTableCell13.Name = "xrTableCell13";
			xrTableCell13.Weight = 0.31367849608366905D;
			//
			// xrTableCell14
			//
			xrTableCell14.DataBindings.AddRange(new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Orgnisation") });
			xrTableCell14.Dpi = 254F;
			xrTableCell14.Name = "xrTableCell14";
			xrTableCell14.Weight = 0.15384615384615386D;
			//
			// xrTableCell15
			//
			xrTableCell15.DataBindings.AddRange(new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department") });
			xrTableCell15.Dpi = 254F;
			xrTableCell15.Name = "xrTableCell15";
			xrTableCell15.Weight = 0.15384615384615386D;
			//
			// xrTableCell16
			//
			xrTableCell16.DataBindings.AddRange(
				filter.IsEmployee
				? new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Position") }
				: new[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Escort") });
			xrTableCell16.Dpi = 254F;
			xrTableCell16.Name = "xrTableCell16";
			xrTableCell16.Weight = 0.15384615384615386D;
			//
			// GroupHeader1
			//
			groupHeader1.BackColor = System.Drawing.Color.LightGray;
			groupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			xrTableHeader});
			groupHeader1.Dpi = 254F;
			groupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			groupHeader1.HeightF = 64F;
			groupHeader1.KeepTogether = true;
			groupHeader1.Name = "GroupHeader1";
			groupHeader1.RepeatEveryPage = true;
			groupHeader1.StylePriority.UseBackColor = false;
			//
			// xrTableHeader
			//
			xrTableHeader.BackColor = System.Drawing.Color.DarkGray;
			xrTableHeader.Borders = ((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
									 | DevExpress.XtraPrinting.BorderSide.Right)
									| DevExpress.XtraPrinting.BorderSide.Bottom;
			xrTableHeader.Dpi = 254F;
			xrTableHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			xrTableHeader.Name = "xrTableHeader";
			xrTableHeader.Rows.AddRange(new[] { xrTableRow1 });
			xrTableHeader.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			xrTableHeader.StylePriority.UseBackColor = false;
			xrTableHeader.StylePriority.UseBorders = false;
			xrTableHeader.StylePriority.UseTextAlignment = false;
			xrTableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// xrTableRow1
			//
			if (filter.UseCurrentDate)
			{
				xrTableRow1.Cells.AddRange(new[] {
					xrTableCell1,
					xrTableCell2,
					xrTableCell4,
					xrTableCell5,
					xrTableCell6,
					xrTableCell7,
					xrTableCell8});
			}
			else
			{
				xrTableCellExitHeader = new DevExpress.XtraReports.UI.XRTableCell();
				xrTableRow1.Cells.AddRange(new[] {
					xrTableCell1,
					xrTableCell2,
					xrTableCellExitHeader,
					xrTableCell4,
					xrTableCell5,
					xrTableCell6,
					xrTableCell7,
					xrTableCell8});
			}
			xrTableRow1.Dpi = 254F;
			xrTableRow1.Name = "xrTableRow1";
			xrTableRow1.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			xrTableRow1.StylePriority.UsePadding = false;
			xrTableRow1.StylePriority.UseTextAlignment = false;
			xrTableRow1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			xrTableRow1.Weight = 11.5D;
			//
			// xrTableCell1
			//
			xrTableCell1.Dpi = 254F;
			xrTableCell1.Name = "xrTableCell1";
			xrTableCell1.Text = CommonResources.Zone;
			xrTableCell1.Weight = 0.15384615384615386D;
			//
			// xrTableCell2
			//
			xrTableCell2.Dpi = 254F;
			xrTableCell2.Name = "xrTableCell2";
			xrTableCell2.Text = CommonResources.EnterDate;
			xrTableCell2.Weight = 0.10056870156204052D;
			//
			// xrTableCellExitHeader
			//
			if (!filter.UseCurrentDate)
			{
				if (xrTableCellExitHeader != null)
				{
					xrTableCellExitHeader.Dpi = 254F;
					xrTableCellExitHeader.Name = "xrTableCellExitHeader";
					xrTableCellExitHeader.Text = CommonResources.ExitDate;
					xrTableCellExitHeader.Weight = 0.10056870156204054D;
				}
			}
			//
			// xrTableCell4
			//
			xrTableCell4.Dpi = 254F;
			xrTableCell4.Name = "xrTableCell4";
			xrTableCell4.Text = CommonResources.Duration;
			xrTableCell4.Weight = 0.105D;
			//
			// xrTableCell5
			//
			xrTableCell5.Dpi = 254F;
			xrTableCell5.Name = "xrTableCell5";
			xrTableCell5.Text = filter.IsEmployee ? CommonResources.Employee : CommonResources.Visitor;
			xrTableCell5.Weight = 0.31367851069849384D;
			//
			// xrTableCell6
			//
			xrTableCell6.Dpi = 254F;
			xrTableCell6.Name = "xrTableCell6";
			xrTableCell6.Text = CommonResources.Organization;
			xrTableCell6.Weight = 0.15384615384615386D;
			//
			// xrTableCell7
			//
			xrTableCell7.Dpi = 254F;
			xrTableCell7.Name = "xrTableCell7";
			xrTableCell7.Text = CommonResources.Department;
			xrTableCell7.Weight = 0.15384615384615386D;
			//
			// xrTableCell8
			//
			xrTableCell8.Dpi = 254F;
			xrTableCell8.Name = "xrTableCell8";
			xrTableCell8.Text = filter.IsEmployee ? CommonResources.Position : CommonResources.Maintainer;
			xrTableCell8.Weight = 0.15384615384615386D;
			//
			// xrControlStyle1
			//
			xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			xrControlStyle1.Name = "xrControlStyle1";
			xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			//
			// EmployeeZonesReport
			//
			Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
			detail,
			groupHeader1});
			DataMember = "Data";
			DataSourceSchema = resources.GetString("$DataSourceSchema");
			ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			Landscape = true;
			PageHeight = 2100;
			PageWidth = 2970;
			StyleSheet.AddRange(new[] { xrControlStyle1 });
			Version = "14.1";
			Controls.SetChildIndex(groupHeader1, 0);
			Controls.SetChildIndex(detail, 0);
			((System.ComponentModel.ISupportInitialize)(xrTableContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(xrTableHeader)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return CommonResources.Location + (GetFilter<EmployeeZonesReportFilter>().IsEmployee ? CommonResources.EmployeeNo : CommonResources.VisitorNo); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
			CreateDetails();
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeZonesReportFilter>();
			if (filter.UseCurrentDate)
				filter.ReportDateTime = DateTime.Now;

			var dataSet = new EmployeeZonesDataSet();

			if (dataProvider.DatabaseService.PassJournalTranslator == null)
				return dataSet;

			var employees = dataProvider.GetEmployees(filter);
			var zoneMap = SKDManager.Zones.ToDictionary(zone => zone.UID, zone => zone.Name);

			var enterJournal = dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesLastEnterPassJournal(employees.Select(item => item.UID), filter.Zones, filter.ReportDateTime);

			foreach (var record in enterJournal)
				AddRecord(dataProvider, dataSet, record, filter, zoneMap);

			return dataSet;
		}

		private void AddRecord(DataProvider dataProvider, EmployeeZonesDataSet ds, PassJournal record, EmployeeZonesReportFilter filter, Dictionary<Guid, string> zoneMap)
		{
			var dataRow = ds.Data.NewDataRow();
			var employee = dataProvider.GetEmployee(record.EmployeeUID);

			if (employee == null)
				return;

			dataRow.Employee = employee.Name;
			dataRow.Orgnisation = employee.Organisation;
			dataRow.Department = employee.Department;
			dataRow.Position = employee.Position;
			dataRow.Zone = zoneMap.ContainsKey(record.ZoneUID) ? zoneMap[record.ZoneUID] : CommonResources.ZoneNotFound;
			dataRow.EnterDateTime = record.EnterTime;

			TimeSpan periodTimeSpanValue;
			if (record.ExitTime.HasValue)
			{
				dataRow.ExitDateTime = record.ExitTime.Value;
				periodTimeSpanValue = dataRow.ExitDateTime - dataRow.EnterDateTime;
			}
			else
				periodTimeSpanValue = DateTime.Now - record.EnterTime;

			dataRow.Period = string.Format(CommonResources.HMS, (int)periodTimeSpanValue.TotalHours, periodTimeSpanValue.Minutes, periodTimeSpanValue.Seconds);

			if (!filter.IsEmployee)
			{
				var escortUID = employee.Item.EscortUID;
				if (escortUID.HasValue)
				{
					var escort = dataProvider.GetEmployee(escortUID.Value);
					dataRow.Escort = escort.Name;
				}
			}
			ds.Data.Rows.Add(dataRow);
		}
	}
}