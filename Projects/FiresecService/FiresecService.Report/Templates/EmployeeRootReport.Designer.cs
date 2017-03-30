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
			this.PositionOrEscortLabel = new DevExpress.XtraReports.UI.XRLabel();
			this.OrganisationLabel = new DevExpress.XtraReports.UI.XRLabel();
			this.EmployeeNameLabel = new DevExpress.XtraReports.UI.XRLabel();
			this.DepartmentLabel = new DevExpress.XtraReports.UI.XRLabel();
			this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
			this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
			this.DataTable = new DevExpress.XtraReports.UI.XRTable();
			this.DataRow = new DevExpress.XtraReports.UI.XRTableRow();
			this.DateTimeCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.ZoneCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.HeaderTable = new DevExpress.XtraReports.UI.XRTable();
			this.HeaderRow = new DevExpress.XtraReports.UI.XRTableRow();
			this.DateTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.ZoneHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(this.DataTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.HeaderTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.PositionOrEscortLabel,
            this.OrganisationLabel,
            this.EmployeeNameLabel,
            this.DepartmentLabel});
			this.Detail.HeightF = 152F;
			this.Detail.KeepTogether = true;
			this.Detail.KeepTogetherWithDetailReports = true;
			// 
			// PositionOrEscortLabel
			// 
			this.PositionOrEscortLabel.Dpi = 254F;
			this.PositionOrEscortLabel.LocationFloat = new DevExpress.Utils.PointFloat(860.5844F, 88.90002F);
			this.PositionOrEscortLabel.Name = "PositionOrEscortLabel";
			this.PositionOrEscortLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.PositionOrEscortLabel.SizeF = new System.Drawing.SizeF(826.187F, 50.8F);
			this.PositionOrEscortLabel.StylePriority.UseTextAlignment = false;
			this.PositionOrEscortLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// OrganisationLabel
			// 
			this.OrganisationLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Organisation", "Company: {0}")});
			this.OrganisationLabel.Dpi = 254F;
			this.OrganisationLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 88.90002F);
			this.OrganisationLabel.Name = "OrganisationLabel";
			this.OrganisationLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.OrganisationLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			this.OrganisationLabel.StylePriority.UseTextAlignment = false;
			this.OrganisationLabel.Text = "OrganisationLabel";
			this.OrganisationLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// EmployeeNameLabel
			// 
			this.EmployeeNameLabel.Dpi = 254F;
			this.EmployeeNameLabel.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 25.40002F);
			this.EmployeeNameLabel.Name = "EmployeeNameLabel";
			this.EmployeeNameLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.EmployeeNameLabel.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			this.EmployeeNameLabel.StylePriority.UseTextAlignment = false;
			this.EmployeeNameLabel.Text = "EmployeeNameLabel";
			this.EmployeeNameLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// DepartmentLabel
			// 
			this.DepartmentLabel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Department", "Division: {0}")});
			this.DepartmentLabel.Dpi = 254F;
			this.DepartmentLabel.LocationFloat = new DevExpress.Utils.PointFloat(855.2921F, 25.00001F);
			this.DepartmentLabel.Name = "DepartmentLabel";
			this.DepartmentLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.DepartmentLabel.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			this.DepartmentLabel.StylePriority.UseTextAlignment = false;
			this.DepartmentLabel.Text = "DepartmentLabel";
			this.DepartmentLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// DetailReport
			// 
			this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1,
            this.GroupHeader1});
			this.DetailReport.DataMember = "Employee.Employee_Data";
			this.DetailReport.Dpi = 254F;
			this.DetailReport.Level = 0;
			this.DetailReport.Name = "DetailReport";
			this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
			this.DetailReport.ReportPrintOptions.DetailCountOnEmptyDataSource = 0;
			// 
			// Detail1
			// 
			this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.DataTable});
			this.Detail1.Dpi = 254F;
			this.Detail1.HeightF = 64F;
			this.Detail1.Name = "Detail1";
			this.Detail1.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("DateTime", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
			// 
			// DataTable
			// 
			this.DataTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.DataTable.Dpi = 254F;
			this.DataTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.DataTable.Name = "DataTable";
			this.DataTable.OddStyleName = "xrControlStyle1";
			this.DataTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.DataTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.DataRow});
			this.DataTable.SizeF = new System.Drawing.SizeF(1686.771F, 63.5F);
			this.DataTable.StylePriority.UseBorders = false;
			this.DataTable.StylePriority.UsePadding = false;
			this.DataTable.StylePriority.UseTextAlignment = false;
			this.DataTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// DataRow
			// 
			this.DataRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.DateTimeCell,
            this.ZoneCell});
			this.DataRow.Dpi = 254F;
			this.DataRow.Name = "DataRow";
			this.DataRow.Weight = 11.5D;
			// 
			// DateTimeCell
			// 
			this.DateTimeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.DateTime", "{0:dd.MM.yyyy H:mm:ss}")});
			this.DateTimeCell.Dpi = 254F;
			this.DateTimeCell.Name = "DateTimeCell";
			this.DateTimeCell.StylePriority.UseTextAlignment = false;
			this.DateTimeCell.Weight = 0.36325779406620395D;
			// 
			// ZoneCell
			// 
			this.ZoneCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.Zone")});
			this.ZoneCell.Dpi = 254F;
			this.ZoneCell.Name = "ZoneCell";
			this.ZoneCell.Weight = 0.64376981204387074D;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.HeaderTable});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 64F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			// 
			// HeaderTable
			// 
			this.HeaderTable.BackColor = System.Drawing.Color.DarkGray;
			this.HeaderTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.HeaderTable.Dpi = 254F;
			this.HeaderTable.KeepTogether = true;
			this.HeaderTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.HeaderTable.Name = "HeaderTable";
			this.HeaderTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.HeaderRow});
			this.HeaderTable.SizeF = new System.Drawing.SizeF(1686.771F, 63.5F);
			this.HeaderTable.StylePriority.UseBackColor = false;
			this.HeaderTable.StylePriority.UseBorders = false;
			this.HeaderTable.StylePriority.UseTextAlignment = false;
			this.HeaderTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// HeaderRow
			// 
			this.HeaderRow.BackColor = System.Drawing.Color.DarkGray;
			this.HeaderRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.DateTimeHeaderCell,
            this.ZoneHeaderCell});
			this.HeaderRow.Dpi = 254F;
			this.HeaderRow.Name = "HeaderRow";
			this.HeaderRow.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.HeaderRow.StylePriority.UsePadding = false;
			this.HeaderRow.StylePriority.UseTextAlignment = false;
			this.HeaderRow.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.HeaderRow.Weight = 11.5D;
			// 
			// DateTimeHeaderCell
			// 
			this.DateTimeHeaderCell.CanGrow = false;
			this.DateTimeHeaderCell.Dpi = 254F;
			this.DateTimeHeaderCell.Name = "DateTimeHeaderCell";
			this.DateTimeHeaderCell.Text = "Date / Time of passage";
			this.DateTimeHeaderCell.Weight = 0.3607227998621324D;
			this.DateTimeHeaderCell.WordWrap = false;
			// 
			// ZoneHeaderCell
			// 
			this.ZoneHeaderCell.CanGrow = false;
			this.ZoneHeaderCell.Dpi = 254F;
			this.ZoneHeaderCell.Name = "ZoneHeaderCell";
			this.ZoneHeaderCell.Text = "Zone";
			this.ZoneHeaderCell.Weight = 0.63927720013786771D;
			this.ZoneHeaderCell.WordWrap = false;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// EmployeeRootReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.DetailReport});
			this.DataMember = "Employee";
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
			this.Version = "15.1";
			this.Controls.SetChildIndex(this.DetailReport, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.DataTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.HeaderTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		DevExpress.XtraReports.UI.XRLabel DescriptionLabel;
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
		//DevExpress.XtraReports.UI.XRTableCell AccessPointHeaderCell;
		DevExpress.XtraReports.UI.XRTableCell ZoneHeaderCell;
		//DevExpress.XtraReports.UI.XRTableCell PassHeaderCell;
		DevExpress.XtraReports.UI.XRTable DataTable;
		DevExpress.XtraReports.UI.XRTableRow DataRow;
		DevExpress.XtraReports.UI.XRTableCell DateTimeCell;
		//DevExpress.XtraReports.UI.XRTableCell AccessPointCell;
		DevExpress.XtraReports.UI.XRTableCell ZoneCell;
		//DevExpress.XtraReports.UI.XRTableCell PassCell;
		DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;

		#endregion
	}
}
