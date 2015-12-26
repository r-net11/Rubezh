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
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.employeeRootDataSet1 = new FiresecService.Report.DataSources.EmployeeRootDataSet();
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.employeeRootDataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			this.xrTable2});
			this.Detail.HeightF = 63.5F;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.BackColor = System.Drawing.Color.LightGray;
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			this.xrTable1});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 64F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			this.GroupHeader1.StylePriority.UseBackColor = false;
			// 
			// xrTable1
			// 
			this.xrTable1.BackColor = System.Drawing.Color.DarkGray;
			this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
			| DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable1.Dpi = 254F;
			this.xrTable1.KeepTogether = true;
			this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0.4999695F);
			this.xrTable1.Name = "xrTable1";
			this.xrTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			this.xrTableRow1});
			this.xrTable1.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			this.xrTable1.StylePriority.UseBorders = false;
			this.xrTable1.StylePriority.UsePadding = false;
			this.xrTable1.StylePriority.UseTextAlignment = false;
			this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.xrTableCell1,
			this.xrTableCell2});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCell1
			// 
			this.xrTableCell1.Dpi = 254F;
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.Text = "Дата/время прохода";
			this.xrTableCell1.Weight = 0.15384616648438571D;
			// 
			// xrTableCell2
			// 
			this.xrTableCell2.Dpi = 254F;
			this.xrTableCell2.Name = "xrTableCell2";
			this.xrTableCell2.Text = "Зона";
			this.xrTableCell2.Weight = 0.15384614120792228D;
			// 
			// employeeRootDataSet1
			// 
			this.employeeRootDataSet1.DataSetName = "EmployeeRootDataSet";
			this.employeeRootDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// xrTable2
			// 
			this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable2.Dpi = 254F;
			this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable2.Name = "xrTable2";
			this.xrTable2.OddStyleName = "xrControlStyle1";
			this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			this.xrTableRow2});
			this.xrTable2.SizeF = new System.Drawing.SizeF(1700F, 63.5F);
			this.xrTable2.StylePriority.UseBorders = false;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.xrTableCell3,
			this.xrTableCell5});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Weight = 0.567901234567901D;
			// 
			// xrTableCell3
			// 
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DateTime")});
			this.xrTableCell3.Dpi = 254F;
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.StylePriority.UseTextAlignment = false;
			this.xrTableCell3.Text = "xrTableCell3";
			this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
			this.xrTableCell3.Weight = 0.270788912579957D;
			// 
			// xrTableCell5
			// 
			this.xrTableCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Zone")});
			this.xrTableCell5.Dpi = 254F;
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.StylePriority.UseTextAlignment = false;
			this.xrTableCell5.Text = "xrTableCell5";
			this.xrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
			this.xrTableCell5.Weight = 0.270788912579957D;
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
			this.GroupHeader1});
			this.DataMember = "Data";
			this.DataSource = this.employeeRootDataSet1;
			this.DataSourceSchema = null;
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
			this.xrControlStyle1});
			this.Version = "15.1";
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.employeeRootDataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
		private DataSources.EmployeeRootDataSet employeeRootDataSet1;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
	}
}
