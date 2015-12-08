namespace FiresecService.Report.Templates
{
	partial class DevicesReport
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
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			this.devicesDataSet1 = new FiresecService.Report.DataSources.DevicesDataSet();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.devicesDataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
			this.Detail.HeightF = 63.5F;
			this.Detail.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Number", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.HeightF = 66.14584F;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			// 
			// xrTable2
			// 
			this.xrTable2.AnchorVertical = ((DevExpress.XtraReports.UI.VerticalAnchorStyles)((DevExpress.XtraReports.UI.VerticalAnchorStyles.Top | DevExpress.XtraReports.UI.VerticalAnchorStyles.Bottom)));
			this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable2.Dpi = 254F;
			this.xrTable2.KeepTogether = true;
			this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable2.Name = "xrTable2";
			this.xrTable2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
			this.xrTable2.SizeF = new System.Drawing.SizeF(1700F, 66.14584F);
			this.xrTable2.StylePriority.UseBorders = false;
			this.xrTable2.StylePriority.UsePadding = false;
			this.xrTable2.StylePriority.UseTextAlignment = false;
			this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell8,
            this.xrTableCell5});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.StylePriority.UseBackColor = false;
			this.xrTableRow2.Weight = 11.5D;
			// 
			// xrTableCell8
			// 
			this.xrTableCell8.CanGrow = false;
			this.xrTableCell8.Dpi = 254F;
			this.xrTableCell8.Name = "xrTableCell8";
			this.xrTableCell8.Text = "Устройство";
			this.xrTableCell8.Weight = 0.155439300248699D;
			// 
			// xrTableCell5
			// 
			this.xrTableCell5.CanGrow = false;
			this.xrTableCell5.Dpi = 254F;
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.Text = "Количество";
			this.xrTableCell5.Weight = 0.155439193043903D;
			// 
			// xrTable1
			// 
			this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable1.Dpi = 254F;
			this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable1.Name = "xrTable1";
			this.xrTable1.OddStyleName = "xrControlStyle1";
			this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
			this.xrTable1.SizeF = new System.Drawing.SizeF(1701F, 63.5F);
			this.xrTable1.StylePriority.UseBorders = false;
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell7});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 0.567901234567901D;
			// 
			// xrTableCell1
			// 
			this.xrTableCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Nomination")});
			this.xrTableCell1.Dpi = 254F;
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.Weight = 0.458891658203233D;
			// 
			// xrTableCell7
			// 
			this.xrTableCell7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Number")});
			this.xrTableCell7.Dpi = 254F;
			this.xrTableCell7.Name = "xrTableCell7";
			this.xrTableCell7.Weight = 0.459431400950788D;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// devicesDataSet1
			// 
			this.devicesDataSet1.DataSetName = "DevicesDataSet";
			this.devicesDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// DevicesReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.GroupHeader1});
			this.DataMember = "Data";
			this.DataSource = this.devicesDataSet1;
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.Margins = new System.Drawing.Printing.Margins(201, 198, 350, 280);
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
			this.Version = "15.1";
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.devicesDataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion
		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
		private DataSources.DevicesDataSet devicesDataSet1;
	}
}