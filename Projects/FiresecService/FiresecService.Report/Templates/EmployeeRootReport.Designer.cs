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
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
			this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel1});
			this.Detail.Dpi = 254F;
			this.Detail.HeightF = 152F;
			this.Detail.KeepTogether = true;
			this.Detail.KeepTogetherWithDetailReports = true;
			this.Detail.Name = "Detail";
			// 
			// xrLabel4
			// 
			this.xrLabel4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Position", "Должность: {0}")});
			this.xrLabel4.Dpi = 254F;
			this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(868.5206F, 88.90002F);
			this.xrLabel4.Name = "xrLabel4";
			this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel4.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			this.xrLabel4.StylePriority.UseTextAlignment = false;
			this.xrLabel4.Text = "xrLabel4";
			this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrLabel3
			// 
			this.xrLabel3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Organisation", "Организация: {0}")});
			this.xrLabel3.Dpi = 254F;
			this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 88.90002F);
			this.xrLabel3.Name = "xrLabel3";
			this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel3.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			this.xrLabel3.StylePriority.UseTextAlignment = false;
			this.xrLabel3.Text = "xrLabel3";
			this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrLabel2
			// 
			this.xrLabel2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Name", "Сотрудник: {0}")});
			this.xrLabel2.Dpi = 254F;
			this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(25.40002F, 25.40002F);
			this.xrLabel2.Name = "xrLabel2";
			this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel2.SizeF = new System.Drawing.SizeF(824.5997F, 50.8F);
			this.xrLabel2.StylePriority.UseTextAlignment = false;
			this.xrLabel2.Text = "xrLabel2";
			this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrLabel1
			// 
			this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Department", "Подразделение: {0}")});
			this.xrLabel1.Dpi = 254F;
			this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(868.5206F, 25.40002F);
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel1.SizeF = new System.Drawing.SizeF(831.4792F, 50.8F);
			this.xrLabel1.StylePriority.UseTextAlignment = false;
			this.xrLabel1.Text = "xrLabel1";
			this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
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
            this.xrTable1});
			this.Detail1.Dpi = 254F;
			this.Detail1.HeightF = 64F;
			this.Detail1.Name = "Detail1";
			this.Detail1.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("DateTime", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
			// 
			// xrTable1
			// 
			this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable1.Dpi = 254F;
			this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable1.Name = "xrTable1";
			this.xrTable1.OddStyleName = "xrControlStyle1";
			this.xrTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
			this.xrTable1.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			this.xrTable1.StylePriority.UseBorders = false;
			this.xrTable1.StylePriority.UsePadding = false;
			this.xrTable1.StylePriority.UseTextAlignment = false;
			this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell3,
            this.xrTableCell5});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCell3
			// 
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.DateTime", "{0:dd.MM.yyyy H:mm:ss}")});
			this.xrTableCell3.Dpi = 254F;
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.StylePriority.UseTextAlignment = false;
			this.xrTableCell3.Weight = 0.20474937987826522D;
			// 
			// xrTableCell5
			// 
			this.xrTableCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Employee.Employee_Data.Zone")});
			this.xrTableCell5.Dpi = 254F;
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.Weight = 0.87217369704481174D;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 64F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			// 
			// xrTable2
			// 
			this.xrTable2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable2.Dpi = 254F;
			this.xrTable2.KeepTogether = true;
			this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable2.Name = "xrTable2";
			this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
			this.xrTable2.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			this.xrTable2.StylePriority.UseBackColor = false;
			this.xrTable2.StylePriority.UseBorders = false;
			this.xrTable2.StylePriority.UseTextAlignment = false;
			this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell10,
            this.xrTableCell12});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTableRow2.StylePriority.UsePadding = false;
			this.xrTableRow2.StylePriority.UseTextAlignment = false;
			this.xrTableRow2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableRow2.Weight = 11.5D;
			// 
			// xrTableCell10
			// 
			this.xrTableCell10.Dpi = 254F;
			this.xrTableCell10.Name = "xrTableCell10";
			this.xrTableCell10.Text = "Дата/время прохода";
			this.xrTableCell10.Weight = 0.20474938582669394D;
			// 
			// xrTableCell12
			// 
			this.xrTableCell12.Dpi = 254F;
			this.xrTableCell12.Name = "xrTableCell12";
			this.xrTableCell12.Text = "Зона";
			this.xrTableCell12.Weight = 0.87217369109638287D;
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
			this.DataSourceSchema = resources.GetString("$this.DataSourceSchema");
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
			this.Version = "14.1";
			this.Controls.SetChildIndex(this.DetailReport, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.XRLabel xrLabel4;
		private DevExpress.XtraReports.UI.XRLabel xrLabel3;
		private DevExpress.XtraReports.UI.XRLabel xrLabel2;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
		private DevExpress.XtraReports.UI.DetailBand Detail1;
		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell12;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
	}
}
