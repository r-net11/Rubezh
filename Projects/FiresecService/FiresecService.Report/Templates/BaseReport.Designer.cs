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
			this.TopMargin.HeightF = 108.3333F;
			this.TopMargin.Name = "TopMargin";
			this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// lReportName
			// 
			this.lReportName.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top;
			this.lReportName.CanShrink = true;
			this.lReportName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.ReportName, "Text", "")});
			this.lReportName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
			this.lReportName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 12.5F);
			this.lReportName.Multiline = true;
			this.lReportName.Name = "lReportName";
			this.lReportName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.lReportName.SizeF = new System.Drawing.SizeF(649.9998F, 23F);
			this.lReportName.StylePriority.UseFont = false;
			this.lReportName.Text = "xrLabel1";
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
            new DevExpress.XtraReports.UI.XRBinding(this.FilterName, "Text", "Фильтр: {0}")});
			this.lFilterName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 75F);
			this.lFilterName.Name = "lFilterName";
			this.lFilterName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.lFilterName.SizeF = new System.Drawing.SizeF(649.9999F, 23F);
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
            new DevExpress.XtraReports.UI.XRBinding(this.Period, "Text", "За период {0}")});
			this.lPeriod.LocationFloat = new DevExpress.Utils.PointFloat(0F, 50F);
			this.lPeriod.Name = "lPeriod";
			this.lPeriod.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.lPeriod.SizeF = new System.Drawing.SizeF(649.9999F, 23F);
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
			this.BottomMargin.HeightF = 62.25001F;
			this.BottomMargin.Name = "BottomMargin";
			this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// lUserName
			// 
			this.lUserName.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Bottom;
			this.lUserName.AutoWidth = true;
			this.lUserName.CanGrow = false;
			this.lUserName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.UserName, "Text", "Пользователь: {0}")});
			this.lUserName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 32.99999F);
			this.lUserName.Name = "lUserName";
			this.lUserName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.lUserName.SizeF = new System.Drawing.SizeF(423.5417F, 23F);
			this.lUserName.Text = "lUserName";
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
            new DevExpress.XtraReports.UI.XRBinding(this.Timestamp, "Text", "Дата и время формирования отчета: {0:dd.MM.yyyy HH:mm:ss}")});
			this.lTimestamp.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
			this.lTimestamp.Name = "lTimestamp";
			this.lTimestamp.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.lTimestamp.SizeF = new System.Drawing.SizeF(423.5417F, 23F);
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
			this.lPage.Format = "Стр {0} из {1}";
			this.lPage.LocationFloat = new DevExpress.Utils.PointFloat(504.1667F, 32.99999F);
			this.lPage.Name = "lPage";
			this.lPage.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.lPage.SizeF = new System.Drawing.SizeF(145.8333F, 23F);
			this.lPage.StylePriority.UseTextAlignment = false;
			this.lPage.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
			// 
			// BaseReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin});
			this.Margins = new System.Drawing.Printing.Margins(100, 100, 108, 62);
			this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ReportName,
            this.FilterName,
            this.Period,
            this.UserName,
            this.Timestamp});
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
