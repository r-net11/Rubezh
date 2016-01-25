namespace FiresecService.Report.Templates
{
	partial class EmployeeRootReport
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmployeeRootReport));
			PositionOrEscortLabel = new DevExpress.XtraReports.UI.XRLabel();
			OrganisationLabel = new DevExpress.XtraReports.UI.XRLabel();
			EmployeeNameLabel = new DevExpress.XtraReports.UI.XRLabel();
			DepartmentLabel = new DevExpress.XtraReports.UI.XRLabel();
			DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
			Detail1 = new DevExpress.XtraReports.UI.DetailBand();
			DataTable = new DevExpress.XtraReports.UI.XRTable();
			DataRow = new DevExpress.XtraReports.UI.XRTableRow();
			DateTimeCell = new DevExpress.XtraReports.UI.XRTableCell();
			ZoneCell = new DevExpress.XtraReports.UI.XRTableCell();
			GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			HeaderTable = new DevExpress.XtraReports.UI.XRTable();
			HeaderRow = new DevExpress.XtraReports.UI.XRTableRow();
			DateTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			ZoneHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(DataTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(HeaderTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			PositionOrEscortLabel,
			OrganisationLabel,
			EmployeeNameLabel,
			DepartmentLabel});
			Detail.Dpi = 254F;
			Detail.HeightF = 152F;
			Detail.KeepTogether = true;
			Detail.KeepTogetherWithDetailReports = true;
			Detail.Name = "Detail";
			// 
			// EmployeeNameLabel
			// 
			EmployeeNameLabel.Dpi = 254F;
			EmployeeNameLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 25.40002F);
			EmployeeNameLabel.Name = "EmployeeNameLabel";
			EmployeeNameLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			EmployeeNameLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			EmployeeNameLabel.StylePriority.UseTextAlignment = false;
			EmployeeNameLabel.Text = "EmployeeNameLabel";
			EmployeeNameLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			//PositionOrEscortLabel
			//
			PositionOrEscortLabel.Dpi = 254F;
			PositionOrEscortLabel.LocationFloat = new DevExpress.Utils.PointFloat(868.5206F, 88.90002F);
			PositionOrEscortLabel.Name = "PositionOrEscortLabel";
			PositionOrEscortLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			PositionOrEscortLabel.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			PositionOrEscortLabel.StylePriority.UseTextAlignment = false;
			PositionOrEscortLabel.Text = "PositionOrEscortLabel";
			PositionOrEscortLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// OrganisationLabel
			// 
			OrganisationLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Organisation", "Организация: {0}")});
			OrganisationLabel.Dpi = 254F;
			OrganisationLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 88.90002F);
			OrganisationLabel.Name = "OrganisationLabel";
			OrganisationLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			OrganisationLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			OrganisationLabel.StylePriority.UseTextAlignment = false;
			OrganisationLabel.Text = "OrganisationLabel";
			OrganisationLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// DepartmentLabel
			// 
			DepartmentLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Department", "Подразделение: {0}")});
			DepartmentLabel.Dpi = 254F;
			DepartmentLabel.LocationFloat = new DevExpress.Utils.PointFloat(868.5206F, 25.40002F);
			DepartmentLabel.Name = "DepartmentLabel";
			DepartmentLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			DepartmentLabel.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			DepartmentLabel.StylePriority.UseTextAlignment = false;
			DepartmentLabel.Text = "DepartmentLabel";
			DepartmentLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// DetailReport
			// 
			DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
			Detail1,
			GroupHeader1});
			DetailReport.DataMember = "Employee.Employee_Data";
			DetailReport.Dpi = 254F;
			DetailReport.Level = 0;
			DetailReport.Name = "DetailReport";
			DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
			DetailReport.ReportPrintOptions.DetailCountOnEmptyDataSource = 0;
			// 
			// Detail1
			// 
			Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			DataTable});
			Detail1.Dpi = 254F;
			Detail1.HeightF = 64F;
			Detail1.Name = "Detail1";
			Detail1.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
			new DevExpress.XtraReports.UI.GroupField("DateTime", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
			// 
			// DataTable
			// 
			DataTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			DataTable.Dpi = 254F;
			DataTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			DataTable.Name = "DataTable";
			DataTable.OddStyleName = "xrControlStyle1";
			DataTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			DataTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			DataRow});
			DataTable.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			DataTable.StylePriority.UseBorders = false;
			DataTable.StylePriority.UsePadding = false;
			DataTable.StylePriority.UseTextAlignment = false;
			DataTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// DataRow
			// 
			//DataRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			//DateTimeCell,
			//AccessPointCell,
			//ZoneCell,
			//PassCell});
			DataRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			DateTimeCell,
			ZoneCell});
			DataRow.Dpi = 254F;
			DataRow.Name = "DataRow";
			DataRow.Weight = 11.5D;
			// 
			// DateTimeCell
			// 
			DateTimeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.DateTime", "{0:dd.MM.yyyy H:mm:ss}")});
			DateTimeCell.Dpi = 254F;
			DateTimeCell.Name = "DateTimeCell";
			DateTimeCell.StylePriority.UseTextAlignment = false;
			DateTimeCell.WidthF = 600F;
			// 
			// ZoneCell
			// 
			ZoneCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.Zone")});
			ZoneCell.Dpi = 254F;
			ZoneCell.Name = "ZoneCell";
			ZoneCell.WidthF = 1100F;
			// 
			// GroupHeader1
			// 
			GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			HeaderTable});
			GroupHeader1.Dpi = 254F;
			GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			GroupHeader1.HeightF = 64F;
			GroupHeader1.KeepTogether = true;
			GroupHeader1.Name = "GroupHeader1";
			GroupHeader1.RepeatEveryPage = true;
			// 
			// HeaderTable
			// 
			HeaderTable.BackColor = System.Drawing.Color.DarkGray;
			HeaderTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
			| DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			HeaderTable.Dpi = 254F;
			HeaderTable.KeepTogether = true;
			HeaderTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			HeaderTable.Name = "HeaderTable";
			HeaderTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			HeaderRow});
			HeaderTable.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			HeaderTable.StylePriority.UseBackColor = false;
			HeaderTable.StylePriority.UseBorders = false;
			HeaderTable.StylePriority.UseTextAlignment = false;
			HeaderTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// HeaderRow
			// 
			HeaderRow.BackColor = System.Drawing.Color.DarkGray;
			HeaderRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			DateTimeHeaderCell,
			ZoneHeaderCell});
			HeaderRow.Dpi = 254F;
			HeaderRow.Name = "HeaderRow";
			HeaderRow.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			HeaderRow.StylePriority.UsePadding = false;
			HeaderRow.StylePriority.UseTextAlignment = false;
			HeaderRow.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			HeaderRow.Weight = 11.5D;
			// 
			// DateTimeHeaderCell
			// 
			DateTimeHeaderCell.Dpi = 254F;
			DateTimeHeaderCell.Name = "DateTimeHeaderCell";
			DateTimeHeaderCell.Text = "Дата/время прохода";
			//DateTimeHeaderCell.Weight = 0.29D;
			DateTimeHeaderCell.WidthF = 600F;
			// 
			// ZoneHeaderCell
			// 
			ZoneHeaderCell.Dpi = 254F;
			ZoneHeaderCell.Name = "ZoneHeaderCell";
			ZoneHeaderCell.Text = "Зона";
			ZoneHeaderCell.WidthF = 1100F;
			// 
			// xrControlStyle1
			// 
			xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			xrControlStyle1.Name = "xrControlStyle1";
			xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// EmployeeRootReport
			// 
			Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
			DetailReport});
			DataMember = "Employee";
			DataSourceSchema = resources.GetString("$DataSourceSchema");
			ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
			xrControlStyle1});
			Version = "14.1";
			Controls.SetChildIndex(DetailReport, 0);
			Controls.SetChildIndex(Detail, 0);
			((System.ComponentModel.ISupportInitialize)(DataTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(HeaderTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion
		DevExpress.XtraReports.UI.XRLabel PositionOrEscortLabel;
		DevExpress.XtraReports.UI.XRLabel OrganisationLabel;
		DevExpress.XtraReports.UI.XRLabel EmployeeNameLabel;
		DevExpress.XtraReports.UI.XRLabel DepartmentLabel;
		DevExpress.XtraReports.UI.DetailReportBand DetailReport;
		DevExpress.XtraReports.UI.DetailBand Detail1;
		DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		DevExpress.XtraReports.UI.XRTable HeaderTable;
		DevExpress.XtraReports.UI.XRTableRow HeaderRow;
		DevExpress.XtraReports.UI.XRTableCell DateTimeHeaderCell;
		DevExpress.XtraReports.UI.XRTableCell ZoneHeaderCell;
		DevExpress.XtraReports.UI.XRTable DataTable;
		DevExpress.XtraReports.UI.XRTableRow DataRow;
		DevExpress.XtraReports.UI.XRTableCell DateTimeCell;
		DevExpress.XtraReports.UI.XRTableCell ZoneCell;
		DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
	}
}