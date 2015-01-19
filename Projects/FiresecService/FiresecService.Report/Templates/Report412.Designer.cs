namespace FiresecService.Report.Templates
{
	partial class Report412
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report412));
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.HeightF = 25F;
			this.Detail.Name = "Detail";
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.BackColor = System.Drawing.Color.LightGray;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 25F;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			this.GroupHeader1.StylePriority.UseBackColor = false;
			// 
			// Report412
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.GroupHeader1});
			this.DataSourceSchema = resources.GetString("$this.DataSourceSchema");
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.Version = "14.1";
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
	}
}
