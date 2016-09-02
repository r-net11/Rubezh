using Localization.FiresecService.Report.Common;

namespace FiresecService.Report.Templates
{
	partial class DocumentsReport
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentsReport));
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell18 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell19 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell17 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell20 = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
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
            this.xrTableCell3,
            this.xrTableCell4,
            this.xrTableCell5,
            this.xrTableCell6,
            this.xrTableCell18,
            this.xrTableCell16,
            this.xrTableCell19,
            this.xrTableCell17,
            this.xrTableCell20});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCell3
			// 
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Employee")});
			this.xrTableCell3.Dpi = 254F;
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.Weight = 0.21538461899616307D;
			// 
			// xrTableCell4
			// 
			this.xrTableCell4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.StartDateTime", "{0:dd.MM.yyyy}")});
			this.xrTableCell4.Dpi = 254F;
			this.xrTableCell4.Name = "xrTableCell4";
			this.xrTableCell4.StylePriority.UseTextAlignment = false;
			this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell4.Weight = 0.11597633045805984D;
			// 
			// xrTableCell5
			// 
			this.xrTableCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.EndDateTime", "{0:dd.MM.yyyy}")});
			this.xrTableCell5.Dpi = 254F;
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.StylePriority.UseTextAlignment = false;
			this.xrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell5.Weight = 0.1159762735761834D;
			// 
			// xrTableCell6
			// 
			this.xrTableCell6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.StartDateTime", "{0:%H\\:mm}")});
			this.xrTableCell6.Dpi = 254F;
			this.xrTableCell6.Name = "xrTableCell6";
			this.xrTableCell6.StylePriority.UseTextAlignment = false;
			this.xrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell6.Weight = 0.082840248423920571D;
			// 
			// xrTableCell18
			// 
			this.xrTableCell18.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.EndDateTime", "{0:%H\\:mm}")});
			this.xrTableCell18.Dpi = 254F;
			this.xrTableCell18.Name = "xrTableCell18";
			this.xrTableCell18.StylePriority.UseTextAlignment = false;
			this.xrTableCell18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell18.Weight = 0.0828402348806167D;
			// 
			// xrTableCell16
			// 
			this.xrTableCell16.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentType")});
			this.xrTableCell16.Dpi = 254F;
			this.xrTableCell16.Name = "xrTableCell16";
			this.xrTableCell16.Weight = 0.16568057901055155D;
			// 
			// xrTableCell19
			// 
			this.xrTableCell19.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentName")});
			this.xrTableCell19.Dpi = 254F;
			this.xrTableCell19.Name = "xrTableCell19";
			this.xrTableCell19.Weight = 0.68191702879711769D;
			// 
			// xrTableCell17
			// 
			this.xrTableCell17.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentShortName")});
			this.xrTableCell17.Dpi = 254F;
			this.xrTableCell17.Name = "xrTableCell17";
			this.xrTableCell17.StylePriority.UseTextAlignment = false;
			this.xrTableCell17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell17.Weight = 0.083718283662829851D;
			// 
			// xrTableCell20
			// 
			this.xrTableCell20.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentCode")});
			this.xrTableCell20.Dpi = 254F;
			this.xrTableCell20.Name = "xrTableCell20";
			this.xrTableCell20.StylePriority.UseTextAlignment = false;
			this.xrTableCell20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCell20.Weight = 0.08372077998623273D;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Department", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 84F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Level = 1;
			this.GroupHeader1.Name = "GroupHeader1";
			// 
			// xrLabel1
			// 
			this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department")});
			this.xrLabel1.Dpi = 254F;
			this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
			this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 12.69997F);
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.xrLabel1.SizeF = new System.Drawing.SizeF(1700F, 58.42F);
			this.xrLabel1.StylePriority.UseFont = false;
			this.xrLabel1.StylePriority.UseTextAlignment = false;
			this.xrLabel1.Text = "xrLabel1";
			this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// GroupHeader2
			// 
			this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
			this.GroupHeader2.Dpi = 254F;
			this.GroupHeader2.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader2.HeightF = 64F;
			this.GroupHeader2.KeepTogether = true;
			this.GroupHeader2.Name = "GroupHeader2";
			this.GroupHeader2.RepeatEveryPage = true;
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
            this.xrTableCell10,
            this.xrTableCell11,
            this.xrTableCell12,
            this.xrTableCell13,
            this.xrTableCell8,
            this.xrTableCell1,
            this.xrTableCell9,
            this.xrTableCell2,
            this.xrTableCell15});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Weight = 11.5D;
			// 
			// xrTableCell10
			// 
			this.xrTableCell10.Dpi = 254F;
			this.xrTableCell10.Name = "xrTableCell10";
			this.xrTableCell10.Text = CommonResources.Employee;
			this.xrTableCell10.Weight = 0.21538461899616312D;
			// 
			// xrTableCell11
			// 
			this.xrTableCell11.Dpi = 254F;
			this.xrTableCell11.Name = "xrTableCell11";
			this.xrTableCell11.Text = CommonResources.StartDate;
			this.xrTableCell11.Weight = 0.1159763304580598D;
			// 
			// xrTableCell12
			// 
			this.xrTableCell12.Dpi = 254F;
			this.xrTableCell12.Name = "xrTableCell12";
			this.xrTableCell12.Text = CommonResources.EndDate;
			this.xrTableCell12.Weight = 0.11597633045805983D;
			// 
			// xrTableCell13
			// 
			this.xrTableCell13.Dpi = 254F;
			this.xrTableCell13.Name = "xrTableCell13";
			this.xrTableCell13.Text = CommonResources.StartTime;
			this.xrTableCell13.Weight = 0.082840235783503566D;
			// 
			// xrTableCell8
			// 
			this.xrTableCell8.Dpi = 254F;
			this.xrTableCell8.Name = "xrTableCell8";
			this.xrTableCell8.Text = CommonResources.EndTime;
			this.xrTableCell8.Weight = 0.082840238040720965D;
			// 
			// xrTableCell1
			// 
			this.xrTableCell1.Dpi = 254F;
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.Text = CommonResources.Type;
			this.xrTableCell1.Weight = 0.1656804747271115D;
			// 
			// xrTableCell9
			// 
			this.xrTableCell9.Dpi = 254F;
			this.xrTableCell9.Name = "xrTableCell9";
			this.xrTableCell9.Text = CommonResources.Document;
			this.xrTableCell9.Weight = 0.68191724591488478D;
			// 
			// xrTableCell2
			// 
			this.xrTableCell2.Dpi = 254F;
			this.xrTableCell2.Name = "xrTableCell2";
			this.xrTableCell2.Text = CommonResources.LitteralCode;
			this.xrTableCell2.Weight = 0.083718108555867282D;
			// 
			// xrTableCell15
			// 
			this.xrTableCell15.Dpi = 254F;
			this.xrTableCell15.Name = "xrTableCell15";
			this.xrTableCell15.Text = CommonResources.NumCode;
			this.xrTableCell15.Weight = 0.083720949516546156D;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// DocumentsReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.GroupHeader1,
            this.GroupHeader2});
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
			this.Controls.SetChildIndex(this.GroupHeader2, 0);
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell11;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell12;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell13;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell9;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell15;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell6;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell18;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell16;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell19;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell17;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell20;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
	}
}
