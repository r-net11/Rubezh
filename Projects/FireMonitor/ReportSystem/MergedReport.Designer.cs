namespace ReportSystem
{
	partial class MergedReport
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
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
			this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
			this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
			this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Dpi = 254F;
			this.Detail.Expanded = false;
			this.Detail.HeightF = 0F;
			this.Detail.Name = "Detail";
			this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			this.Detail.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBand;
			this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// ReportHeader
			// 
			this.ReportHeader.Dpi = 254F;
			this.ReportHeader.Expanded = false;
			this.ReportHeader.HeightF = 0F;
			this.ReportHeader.Name = "ReportHeader";
			// 
			// PageHeader
			// 
			this.PageHeader.Dpi = 254F;
			this.PageHeader.Expanded = false;
			this.PageHeader.HeightF = 0F;
			this.PageHeader.Name = "PageHeader";
			// 
			// topMarginBand1
			// 
			this.topMarginBand1.Dpi = 254F;
			this.topMarginBand1.HeightF = 254F;
			this.topMarginBand1.Name = "topMarginBand1";
			// 
			// bottomMarginBand1
			// 
			this.bottomMarginBand1.Dpi = 254F;
			this.bottomMarginBand1.HeightF = 254F;
			this.bottomMarginBand1.Name = "bottomMarginBand1";
			// 
			// XtraReport2
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.ReportHeader,
            this.PageHeader,
            this.topMarginBand1,
            this.bottomMarginBand1});
			this.Dpi = 254F;
			this.Margins = new System.Drawing.Printing.Margins(254, 254, 254, 254);
			this.PageHeight = 2970;
			this.PageWidth = 2100;
			this.PaperKind = System.Drawing.Printing.PaperKind.A4;
			this.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter;
			this.Version = "13.2";
			this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XtraReportMerged_BeforePrint);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
		private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
		private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
		private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
	}
}
