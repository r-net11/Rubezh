namespace FiresecService.Report.Templates
{
	partial class Report415
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
			DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report415));
			this.Detail = new DevExpress.XtraReports.UI.DetailBand();
			this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
			this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
			this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource("SKDDriver.Properties.Settings.SKDConnectionString");
			this.organisation = new DevExpress.XtraReports.Parameters.Parameter();
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
			this.Detail.HeightF = 45.83333F;
			this.Detail.Name = "Detail";
			this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// TopMargin
			// 
			this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1});
			this.TopMargin.HeightF = 100F;
			this.TopMargin.Name = "TopMargin";
			this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// xrLabel1
			// 
			this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 32.25001F);
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel1.SizeF = new System.Drawing.SizeF(630F, 23F);
			this.xrLabel1.Text = "Список отделов организации";
			// 
			// BottomMargin
			// 
			this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2});
			this.BottomMargin.HeightF = 100F;
			this.BottomMargin.Name = "BottomMargin";
			this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// xrLabel2
			// 
			this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 38.49999F);
			this.xrLabel2.Name = "xrLabel2";
			this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel2.SizeF = new System.Drawing.SizeF(184.375F, 23F);
			this.xrLabel2.Text = "[Parameters.organisation]";
			// 
			// sqlDataSource1
			// 
			customSqlQuery1.Name = "Department";
			customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
			this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
			this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
			// 
			// organisation
			// 
			this.organisation.Name = "organisation";
			this.organisation.Visible = false;
			// 
			// xrTable1
			// 
			this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
			this.xrTable1.Name = "xrTable1";
			this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
			this.xrTable1.SizeF = new System.Drawing.SizeF(650F, 25F);
			this.xrTable1.StylePriority.UseBorders = false;
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3,
            this.xrTableCell4,
            this.xrTableCell5});
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCell1
			// 
			this.xrTableCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Department.Description")});
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.Text = "xrTableCell1";
			this.xrTableCell1.Weight = 0.66666666666666663D;
			// 
			// xrTableCell2
			// 
			this.xrTableCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Department.LastName")});
			this.xrTableCell2.Name = "xrTableCell2";
			this.xrTableCell2.Text = "xrTableCell2";
			this.xrTableCell2.Weight = 0.66666666666666663D;
			// 
			// xrTableCell3
			// 
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Department.Name")});
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.Text = "xrTableCell3";
			this.xrTableCell3.Weight = 0.66666666666666663D;
			// 
			// xrTableCell4
			// 
			this.xrTableCell4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Department.ParentName")});
			this.xrTableCell4.Name = "xrTableCell4";
			this.xrTableCell4.Text = "xrTableCell4";
			this.xrTableCell4.Weight = 0.66666666666666663D;
			// 
			// xrTableCell5
			// 
			this.xrTableCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Department.Phone")});
			this.xrTableCell5.Name = "xrTableCell5";
			this.xrTableCell5.Text = "xrTableCell5";
			this.xrTableCell5.Weight = 0.66666666666666663D;
			// 
			// Report415
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
			this.DataMember = "Department";
			this.DataSource = this.sqlDataSource1;
			this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.organisation});
			this.Version = "14.1";
			this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.Report415_DataSourceDemanded);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
		private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
		private DevExpress.XtraReports.UI.XRLabel xrLabel2;
		private DevExpress.XtraReports.Parameters.Parameter organisation;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
	}
}
