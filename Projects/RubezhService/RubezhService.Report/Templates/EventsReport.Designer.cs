namespace RubezhService.Report.Templates
{
	partial class EventsReport
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
			DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventsReport));
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
			this.Detail.HeightF = 63.5F;
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
			this.xrTable1.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			this.xrTable1.StylePriority.UseBorders = false;
			this.xrTable1.StylePriority.UsePadding = false;
			this.xrTable1.StylePriority.UseTextAlignment = false;
			this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3,
            this.xrTableCell4,
            this.xrTableCell5,
            this.xrTableCell6,
            this.xrTableCell7});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCell1
			// 
			this.xrTableCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.SystemDateTime", "{0:dd.MM.yyyy H:mm:ss}")});
			this.xrTableCell1.Dpi = 254F;
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.Weight = 0.085350464333856055D;
			// 
			// xrTableCell2
			// 
			this.xrTableCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DeviceDateTime", "{0:dd.MM.yyyy HH:mm:ss}")});
			this.xrTableCell2.Dpi = 254F;
			this.xrTableCell2.Name = "xrTableCell2";
			this.xrTableCell2.Weight = 0.085350454990875416D;
			// 
			// xrTableCell3
			// 
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Name")});
			this.xrTableCell3.Dpi = 254F;
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.Weight = 0.29083754221373009D;
			// 
			// xrTableCell4
			// 
			this.xrTableCell4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Description")});
			this.xrTableCell4.Dpi = 254F;
			this.xrTableCell4.Name = "xrTableCell4";
			this.xrTableCell4.Weight = 0.15384615384615386D;
			// 
			// xrTableCell5
			// 
			this.xrTableCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Object")});
			this.xrTableCell5.Dpi = 254F;
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.Weight = 0.15384615384615386D;
			// 
			// xrTableCell6
			// 
			this.xrTableCell6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.User")});
			this.xrTableCell6.Dpi = 254F;
			this.xrTableCell6.Name = "xrTableCell6";
			this.xrTableCell6.Weight = 0.15384615384615386D;
			// 
			// xrTableCell7
			// 
			this.xrTableCell7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.System")});
			this.xrTableCell7.Dpi = 254F;
			this.xrTableCell7.Name = "xrTableCell7";
			this.xrTableCell7.Weight = 0.073971700254789177D;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.BackColor = System.Drawing.Color.LightGray;
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 64F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			this.GroupHeader1.StylePriority.UseBackColor = false;
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
			this.xrTable2.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			this.xrTable2.StylePriority.UseBackColor = false;
			this.xrTable2.StylePriority.UseBorders = false;
			this.xrTable2.StylePriority.UseTextAlignment = false;
			this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell8,
            this.xrTableCell9,
            this.xrTableCell10,
            this.xrTableCell11,
            this.xrTableCell12,
            this.xrTableCell13,
            this.xrTableCell14});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTableRow2.StylePriority.UsePadding = false;
			this.xrTableRow2.StylePriority.UseTextAlignment = false;
			this.xrTableRow2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableRow2.Weight = 11.5D;
			// 
			// xrTableCell8
			// 
			this.xrTableCell8.Dpi = 254F;
			this.xrTableCell8.Name = "xrTableCell8";
			this.xrTableCell8.Text = "Дата и время в системе";
			this.xrTableCell8.Weight = 0.085350465107877652D;
			// 
			// xrTableCell9
			// 
			this.xrTableCell9.Dpi = 254F;
			this.xrTableCell9.Name = "xrTableCell9";
			this.xrTableCell9.Text = "Дата и время на устройстве";
			this.xrTableCell9.Weight = 0.085350465107877652D;
			// 
			// xrTableCell10
			// 
			this.xrTableCell10.Dpi = 254F;
			this.xrTableCell10.Name = "xrTableCell10";
			this.xrTableCell10.Text = "Название";
			this.xrTableCell10.Weight = 0.29083753132270629D;
			// 
			// xrTableCell11
			// 
			this.xrTableCell11.Dpi = 254F;
			this.xrTableCell11.Name = "xrTableCell11";
			this.xrTableCell11.Text = "Уточнение";
			this.xrTableCell11.Weight = 0.15384615384615386D;
			// 
			// xrTableCell12
			// 
			this.xrTableCell12.Dpi = 254F;
			this.xrTableCell12.Name = "xrTableCell12";
			this.xrTableCell12.Text = "Объект";
			this.xrTableCell12.Weight = 0.15384615384615386D;
			// 
			// xrTableCell13
			// 
			this.xrTableCell13.Dpi = 254F;
			this.xrTableCell13.Name = "xrTableCell13";
			this.xrTableCell13.Text = "Пользователь";
			this.xrTableCell13.Weight = 0.15384615384615386D;
			// 
			// xrTableCell14
			// 
			this.xrTableCell14.Dpi = 254F;
			this.xrTableCell14.Name = "xrTableCell14";
			this.xrTableCell14.Text = "Подсистема";
			this.xrTableCell14.Weight = 0.073971700254789177D;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// GroupFooter1
			// 
			this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1});
			this.GroupFooter1.Dpi = 254F;
			this.GroupFooter1.GroupUnion = DevExpress.XtraReports.UI.GroupFooterUnion.WithLastDetail;
			this.GroupFooter1.HeightF = 65.00002F;
			this.GroupFooter1.KeepTogether = true;
			this.GroupFooter1.Name = "GroupFooter1";
			// 
			// xrLabel1
			// 
			this.xrLabel1.Dpi = 254F;
			this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(25F, 5.000018F);
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel1.SizeF = new System.Drawing.SizeF(2525F, 60F);
			this.xrLabel1.StylePriority.UseFont = false;
			this.xrLabel1.StylePriority.UsePadding = false;
			this.xrLabel1.StylePriority.UseTextAlignment = false;
			xrSummary1.FormatString = "Всего событий: {0}";
			xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
			xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
			this.xrLabel1.Summary = xrSummary1;
			this.xrLabel1.Text = "Total";
			this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// EventsReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.GroupHeader1,
            this.GroupFooter1});
			this.DataMember = "Data";
			this.DataSourceSchema = resources.GetString("$this.DataSourceSchema");
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.Landscape = true;
			this.PageHeight = 2100;
			this.PageWidth = 2970;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
			this.Version = "15.1";
			this.Controls.SetChildIndex(this.GroupFooter1, 0);
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell6;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell9;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell11;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell12;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell13;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell14;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
		private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private DevExpress.XtraReports.UI.XRSummary xrSummary1;
	}
}
