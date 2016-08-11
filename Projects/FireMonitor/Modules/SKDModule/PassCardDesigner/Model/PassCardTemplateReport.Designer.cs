using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.XtraReports.UI;

namespace SKDModule.PassCardDesigner.Model
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
			this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
			this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			resources.ApplyResources(this.Detail, "Detail");
			this.Detail.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
			this.Detail.Name = "Detail";
			this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.Detail.StylePriority.UseBackColor = false;
			this.Detail.StylePriority.UseBorderDashStyle = false;
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
			// xrControlStyle1
			// 
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
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
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
			this.Version = "13.2";
			this.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.Stretch;
			this.Watermark.Text = resources.GetString("PassCardTemplateReport.Watermark.Text");
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

			}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private TopMarginBand topMarginBand1;
		private BottomMarginBand bottomMarginBand1;
		private XRControlStyle xrControlStyle1;
	}
}
