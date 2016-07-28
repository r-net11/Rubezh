using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.XtraReports.UI;

namespace ReportSystem.Templates
{
	partial class PassCardTemplateReport
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassCardTemplateReport));
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
			this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			resources.ApplyResources(this.Detail, "Detail");
			this.Detail.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1});
			this.Detail.Name = "Detail";
			this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.Detail.StylePriority.UseBackColor = false;
			this.Detail.StylePriority.UseBorderDashStyle = false;
			// 
			// xrLabel1
			// 
			resources.ApplyResources(this.xrLabel1, "xrLabel1");
			this.xrLabel1.Multiline = true;
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel1.StylePriority.UseForeColor = false;
			// 
			// topMarginBand1
			// 
			resources.ApplyResources(this.topMarginBand1, "topMarginBand1");
			this.topMarginBand1.Name = "topMarginBand1";
			this.topMarginBand1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.TopMargin_BeforePrint);
			// 
			// bottomMarginBand1
			// 
			resources.ApplyResources(this.bottomMarginBand1, "bottomMarginBand1");
			this.bottomMarginBand1.Name = "bottomMarginBand1";
			this.bottomMarginBand1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.BottomMargin_BeforePrint);
			// 
			// PassCardTemplateReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.topMarginBand1,
            this.bottomMarginBand1});
			resources.ApplyResources(this, "$this");
			this.DrawWatermark = true;
			this.ShowPrintMarginsWarning = false;
			this.Version = "13.2";
			this.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.Stretch;
			this.Watermark.Text = resources.GetString("PassCardTemplateReport.Watermark.Text");
			this.DesignerLoaded += new DevExpress.XtraReports.UserDesigner.DesignerLoadedEventHandler(this.PassCardTemplateReport_DesignerLoaded);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private TopMarginBand topMarginBand1;
		private BottomMarginBand bottomMarginBand1;
	}
}
