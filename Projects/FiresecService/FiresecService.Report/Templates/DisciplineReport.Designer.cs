using Localization.FiresecService.Report.Common;

namespace FiresecService.Report.Templates
{
	partial class DisciplineReport
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
			float[] columnsWidth = { 180F, 120F, 150F, 0F, 0F, 300F, 0F, 200F, 200F, 200F, 200F, 170F, 0F, 180F };

			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisciplineReport));
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell3N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell4N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell6N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell7N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell8N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell9N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell10N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell11N = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			//
			// Detail
			//
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
			this.Detail.Dpi = 254F;
			this.Detail.HeightF = 64F;
			this.Detail.Name = "Detail";
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
            this.xrTableCell7,
            this.xrTableCell8,
            this.xrTableCell9,
            this.xrTableCell10,
            this.xrTableCell11});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			//
			// xrTableCell1
			//
			this.xrTableCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Date", "{0:dd.MM.yyyy}")});
			this.xrTableCell1.Dpi = 254F;
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.StylePriority.UseTextAlignment = false;
			this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell1.WidthF = columnsWidth[0];
			// xrTableCell1
			//
			this.xrTableCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Weekday")});
			this.xrTableCell2.Dpi = 254F;
			this.xrTableCell2.Name = "xrTableCell2";
			this.xrTableCell2.StylePriority.UseTextAlignment = false;
			this.xrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell2.WidthF = columnsWidth[1];
			//
			// xrTableCell2
			//
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.FirstEnter")});
			this.xrTableCell3.Dpi = 254F;
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.StylePriority.UseTextAlignment = false;
			this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell3.WidthF = columnsWidth[2];
			//
			// xrTableCell3
			//
			this.xrTableCell4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.LastExit")});
			this.xrTableCell4.Dpi = 254F;
			this.xrTableCell4.Name = "xrTableCell4";
			this.xrTableCell4.StylePriority.UseTextAlignment = false;
			this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell4.WidthF = columnsWidth[2];
			//
			// xrTableCell4
			//
			this.xrTableCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Employee")});
			this.xrTableCell5.Dpi = 254F;
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.WidthF = columnsWidth[5];
			//
			// xrTableCell5
			//
			this.xrTableCell6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Organisation")});
			this.xrTableCell6.Dpi = 254F;
			this.xrTableCell6.Name = "xrTableCell6";
			this.xrTableCell6.WidthF = columnsWidth[7];
			//
			// xrTableCell6
			//
			this.xrTableCell7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department")});
			this.xrTableCell7.Dpi = 254F;
			this.xrTableCell7.Name = "xrTableCell7";
			this.xrTableCell7.WidthF = columnsWidth[7];
			//
			// xrTableCell23
			//
			this.xrTableCell8.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Late")});
			this.xrTableCell8.Dpi = 254F;
			this.xrTableCell8.Name = "xrTableCell8";
			this.xrTableCell8.StylePriority.UseTextAlignment = false;
			this.xrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell8.WidthF = columnsWidth[0];
			//
			// xrTableCell24
			//
			this.xrTableCell9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.EarlyLeave")});
			this.xrTableCell9.Dpi = 254F;
			this.xrTableCell9.Name = "xrTableCell9";
			this.xrTableCell9.StylePriority.UseTextAlignment = false;
			this.xrTableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell9.WidthF = columnsWidth[0];
			//
			// xrTableCell25
			//
			this.xrTableCell10.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Absence")});
			this.xrTableCell10.Dpi = 254F;
			this.xrTableCell10.Name = "xrTableCell10";
			this.xrTableCell10.StylePriority.UseTextAlignment = false;
			this.xrTableCell10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell10.WidthF = columnsWidth[0];
			//
			// xrTableCell26
			//
			this.xrTableCell11.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Overtime")});
			this.xrTableCell11.Dpi = 254F;
			this.xrTableCell11.Name = "xrTableCell11";
			this.xrTableCell11.StylePriority.UseTextAlignment = false;
			this.xrTableCell11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell11.WidthF = columnsWidth[0];
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
			this.xrTable2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
			this.xrTable2.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			this.xrTable2.StylePriority.UseBackColor = false;
			this.xrTable2.StylePriority.UseBorders = false;
			this.xrTable2.StylePriority.UsePadding = false;
			this.xrTable2.StylePriority.UseTextAlignment = false;
			this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			//
			// xrTableRow2
			//
			this.xrTableRow2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1N,
            this.xrTableCell2N,
            this.xrTableCell3N,
            this.xrTableCell4N,
            this.xrTableCell5N,
            this.xrTableCell6N,
            this.xrTableCell7N,
            this.xrTableCell8N,
            this.xrTableCell9N,
            this.xrTableCell10N,
            this.xrTableCell11N});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Weight = 11.5D;
			//
			// xrTableCell8
			//
			this.xrTableCell1N.Dpi = 254F;
			this.xrTableCell1N.Name = "xrTableCell1N";
			this.xrTableCell1N.Text = CommonResources.Date;
			this.xrTableCell1N.WidthF = columnsWidth[0];
			//
			// xrTableCell12
			//
			this.xrTableCell2N.Dpi = 254F;
			this.xrTableCell2N.Name = "xrTableCell2N";
			this.xrTableCell2N.Text = CommonResources.Day;
			this.xrTableCell2N.WidthF = columnsWidth[1];
			//
			// xrTableCell21
			//
			this.xrTableCell3N.Dpi = 254F;
			this.xrTableCell3N.Name = "xrTableCell3N";
			this.xrTableCell3N.Text = CommonResources.Coming;
			this.xrTableCell3N.WidthF = columnsWidth[2];
			//
			// xrTableCell9
			//
			this.xrTableCell4N.Dpi = 254F;
			this.xrTableCell4N.Name = "xrTableCell4N";
			this.xrTableCell4N.Text = CommonResources.Exit;
			this.xrTableCell4N.WidthF = columnsWidth[2];
			//
			// xrTableCell20
			//
			this.xrTableCell5N.Dpi = 254F;
			this.xrTableCell5N.Name = "xrTableCell5N";
			this.xrTableCell5N.Text = CommonResources.Employee;
			this.xrTableCell5N.WidthF = columnsWidth[5];
			//
			// xrTableCell10
			//
			this.xrTableCell6N.Dpi = 254F;
			this.xrTableCell6N.Name = "xrTableCell6N";
			this.xrTableCell6N.Text = CommonResources.Organization;
			this.xrTableCell6N.WidthF = columnsWidth[7];
			//
			// xrTableCell19
			//
			this.xrTableCell7N.Dpi = 254F;
			this.xrTableCell7N.Name = "xrTableCell7N";
			this.xrTableCell7N.Text = CommonResources.Department;
			this.xrTableCell7N.WidthF = columnsWidth[7];
			//
			// xrTableCell18
			//
			this.xrTableCell8N.Dpi = 254F;
			this.xrTableCell8N.Name = "xrTableCell8N";
			this.xrTableCell8N.Text = CommonResources.Lateness;
			this.xrTableCell8N.WidthF = columnsWidth[0];
			//
			// xrTableCell12
			//
			this.xrTableCell9N.Dpi = 254F;
			this.xrTableCell9N.Name = "xrTableCell9N";
			this.xrTableCell9N.Text = CommonResources.EarlyLeave;
			this.xrTableCell9N.WidthF = columnsWidth[0];
			//
			// xrTableCell17
			//
			this.xrTableCell10N.Dpi = 254F;
			this.xrTableCell10N.Name = "xrTableCell10N";
			this.xrTableCell10N.Text = CommonResources.Absence;
			this.xrTableCell10N.WidthF = columnsWidth[0];
			//
			// xrTableCell13
			//
			this.xrTableCell11N.Dpi = 254F;
			this.xrTableCell11N.Name = "xrTableCell11N";
			this.xrTableCell11N.Text = CommonResources.Timeovers;
			this.xrTableCell11N.WidthF = columnsWidth[0];
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
			this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(25.0F, 5.000018F);
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel1.SizeF = new System.Drawing.SizeF(2525F, 60F);
			this.xrLabel1.StylePriority.UseFont = false;
			this.xrLabel1.StylePriority.UsePadding = false;
			this.xrLabel1.StylePriority.UseTextAlignment = false;
			this.xrSummary1.FormatString = CommonResources.TottallyRecords;
			this.xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
			this.xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
			this.xrLabel1.Summary = this.xrSummary1;
			this.xrLabel1.Text = "Total";
			this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			//
			// DisciplineReport
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
			this.Version = "14.1";
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
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
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell9;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell11;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell4N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell6N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell7N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell8N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell9N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell10N;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell11N;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
		private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private DevExpress.XtraReports.UI.XRSummary xrSummary1;
	}
}
