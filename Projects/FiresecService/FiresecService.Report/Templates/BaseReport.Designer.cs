using Localization.FiresecService.Report.Common;

namespace FiresecService.Report.Templates
{
	partial class BaseReport
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
			this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
			this.lReportName = new DevExpress.XtraReports.UI.XRLabel();
			this.ReportName = new DevExpress.XtraReports.Parameters.Parameter();
			this.lFilterName = new DevExpress.XtraReports.UI.XRLabel();
			this.FilterName = new DevExpress.XtraReports.Parameters.Parameter();
			this.lPeriod = new DevExpress.XtraReports.UI.XRLabel();
			this.Period = new DevExpress.XtraReports.Parameters.Parameter();
			this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
			this.lUserName = new DevExpress.XtraReports.UI.XRLabel();
			this.UserName = new DevExpress.XtraReports.Parameters.Parameter();
			this.lTimestamp = new DevExpress.XtraReports.UI.XRLabel();
			this.Timestamp = new DevExpress.XtraReports.Parameters.Parameter();
			this.lPage = new DevExpress.XtraReports.UI.XRPageInfo();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// TopMargin
			// 
			this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lReportName,
            this.lFilterName,
            this.lPeriod});
			this.TopMargin.Dpi = 254F;
			this.TopMargin.HeightF = 350F;
			this.TopMargin.Name = "TopMargin";
			this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// lReportName
			// 
			this.lReportName.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top;
			this.lReportName.CanShrink = true;
			this.lReportName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.ReportName, "Text", "")});
			this.lReportName.Dpi = 254F;
			this.lReportName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
			this.lReportName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 142.875F);
			this.lReportName.Multiline = true;
			this.lReportName.Name = "lReportName";
			this.lReportName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.lReportName.SizeF = new System.Drawing.SizeF(1700F, 58.42F);
			this.lReportName.StylePriority.UseFont = false;
			this.lReportName.StylePriority.UseTextAlignment = false;
			this.lReportName.Text = "xrLabel1";
			this.lReportName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// ReportName
			// 
			this.ReportName.Name = "ReportName";
			this.ReportName.Visible = false;
			// 
			// lFilterName
			// 
			this.lFilterName.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Bottom;
			this.lFilterName.AutoWidth = true;
			this.lFilterName.CanGrow = false;
			this.lFilterName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.FilterName, "Text", CommonResources.FilterWithColon)});
			this.lFilterName.Dpi = 254F;
			this.lFilterName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 278.8542F);
			this.lFilterName.Name = "lFilterName";
			this.lFilterName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.lFilterName.SizeF = new System.Drawing.SizeF(1700F, 58.42001F);
			this.lFilterName.StylePriority.UseTextAlignment = false;
			this.lFilterName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// FilterName
			// 
			this.FilterName.Name = "FilterName";
			this.FilterName.Visible = false;
			// 
			// lPeriod
			// 
			this.lPeriod.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top;
			this.lPeriod.AutoWidth = true;
			this.lPeriod.CanShrink = true;
			this.lPeriod.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.Period, "Text", CommonResources.DuringPeriod)});
			this.lPeriod.Dpi = 254F;
			this.lPeriod.LocationFloat = new DevExpress.Utils.PointFloat(0F, 220.4341F);
			this.lPeriod.Name = "lPeriod";
			this.lPeriod.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.lPeriod.SizeF = new System.Drawing.SizeF(1700F, 58.42001F);
			this.lPeriod.StylePriority.UseTextAlignment = false;
			this.lPeriod.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// Period
			// 
			this.Period.Name = "Period";
			this.Period.Visible = false;
			// 
			// BottomMargin
			// 
			this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lUserName,
            this.lTimestamp,
            this.lPage});
			this.BottomMargin.Dpi = 254F;
			this.BottomMargin.HeightF = 280F;
			this.BottomMargin.Name = "BottomMargin";
			this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// lUserName
			// 
			this.lUserName.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Bottom;
			this.lUserName.AutoWidth = true;
			this.lUserName.CanGrow = false;
			this.lUserName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.UserName, "Text", CommonResources.UserWithColon)});
			this.lUserName.Dpi = 254F;
			this.lUserName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 90.16997F);
			this.lUserName.Name = "lUserName";
			this.lUserName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.lUserName.SizeF = new System.Drawing.SizeF(1250F, 58.42F);
			this.lUserName.StylePriority.UseTextAlignment = false;
			this.lUserName.Text = "lUserName";
			this.lUserName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// UserName
			// 
			this.UserName.Name = "UserName";
			this.UserName.Visible = false;
			// 
			// lTimestamp
			// 
			this.lTimestamp.AutoWidth = true;
			this.lTimestamp.CanShrink = true;
			this.lTimestamp.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.Timestamp, "Text", CommonResources.ReportDate)});
			this.lTimestamp.Dpi = 254F;
			this.lTimestamp.LocationFloat = new DevExpress.Utils.PointFloat(0F, 31.75F);
			this.lTimestamp.Name = "lTimestamp";
			this.lTimestamp.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.lTimestamp.SizeF = new System.Drawing.SizeF(1250F, 58.42F);
			this.lTimestamp.StylePriority.UseTextAlignment = false;
			this.lTimestamp.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// Timestamp
			// 
			this.Timestamp.Name = "Timestamp";
			this.Timestamp.Type = typeof(System.DateTime);
			this.Timestamp.Visible = false;
			// 
			// lPage
			// 
			this.lPage.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Bottom;
			this.lPage.Dpi = 254F;
			this.lPage.Format = CommonResources.PageOf;
			this.lPage.LocationFloat = new DevExpress.Utils.PointFloat(1300.479F, 90.16997F);
			this.lPage.Name = "lPage";
			this.lPage.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
			this.lPage.SizeF = new System.Drawing.SizeF(399.5208F, 58.42F);
			this.lPage.StylePriority.UseTextAlignment = false;
			this.lPage.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
			// 
			// BaseReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin});
			this.Dpi = 254F;
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.Margins = new System.Drawing.Printing.Margins(200, 200, 350, 280);
			this.PageHeight = 2970;
			this.PageWidth = 2100;
			this.PaperKind = System.Drawing.Printing.PaperKind.A4;
			this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ReportName,
            this.FilterName,
            this.Period,
            this.UserName,
            this.Timestamp});
			this.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter;
			this.Version = "14.1";
			this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.BaseReport_DataSourceDemanded);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
		private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
		private DevExpress.XtraReports.UI.XRLabel lFilterName;
		private DevExpress.XtraReports.Parameters.Parameter FilterName;
		private DevExpress.XtraReports.UI.XRLabel lPeriod;
		private DevExpress.XtraReports.Parameters.Parameter Period;
		private DevExpress.XtraReports.Parameters.Parameter ReportName;
		private DevExpress.XtraReports.UI.XRLabel lUserName;
		private DevExpress.XtraReports.Parameters.Parameter UserName;
		private DevExpress.XtraReports.UI.XRLabel lTimestamp;
		private DevExpress.XtraReports.Parameters.Parameter Timestamp;
		private DevExpress.XtraReports.UI.XRPageInfo lPage;
		private DevExpress.XtraReports.UI.XRLabel lReportName;
	}
}
