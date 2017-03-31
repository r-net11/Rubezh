namespace FiresecService.Report.Templates
{
	partial class DepartmentsReport
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DepartmentsReport));
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTableHeader = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellArchiveHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableContent = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCellLevel = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellArchive = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			this.ArchiveLabel = new DevExpress.XtraReports.UI.CalculatedField();
			((System.ComponentModel.ISupportInitialize)(this.xrTableHeader)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTableContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			this.xrTableContent});
			this.Detail.HeightF = 64F;
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
			this.xrTableCell1.Weight = 0D;
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
			// GroupHeader1
			// 
			this.GroupHeader1.BackColor = System.Drawing.Color.LightGray;
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			this.xrTableHeader});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 64F;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			this.GroupHeader1.StylePriority.UseBackColor = false;
			// 
			// xrTableHeader
			// 
			this.xrTableHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
			| DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTableHeader.Dpi = 254F;
			this.xrTableHeader.KeepTogether = true;
			this.xrTableHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTableHeader.Name = "xrTableHeader";
			this.xrTableHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTableHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			this.xrTableRow3});
			this.xrTableHeader.SizeF = new System.Drawing.SizeF(1686.771F, 63.5F);
			this.xrTableHeader.StylePriority.UseBorders = false;
			this.xrTableHeader.StylePriority.UsePadding = false;
			this.xrTableHeader.StylePriority.UseTextAlignment = false;
			this.xrTableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			// 
			// xrTableRow3
			// 
			this.xrTableRow3.BackColor = System.Drawing.Color.DarkGray;
			this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.xrTableCell16,
			this.xrTableCell11,
			this.xrTableCell12,
			this.xrTableCell13,
			this.xrTableCell14,
			this.xrTableCell15,
			this.xrTableCellArchiveHeader});
			this.xrTableRow3.Dpi = 254F;
			this.xrTableRow3.Name = "xrTableRow3";
			this.xrTableRow3.StylePriority.UseBackColor = false;
			this.xrTableRow3.Weight = 15.13987881291011D;
			// 
			// xrTableCell16
			// 
			this.xrTableCell16.Dpi = 254F;
			this.xrTableCell16.Name = "xrTableCell16";
			this.xrTableCell16.Text = "Level";
			this.xrTableCell16.Weight = 0.41709401709401706D;
			// 
			// xrTableCell11
			// 
			this.xrTableCell11.Dpi = 254F;
			this.xrTableCell11.Name = "xrTableCell11";
			this.xrTableCell11.Text = "Subdivision";
			this.xrTableCell11.Weight = 0.66666666666666652D;
			// 
			// xrTableCell12
			// 
			this.xrTableCell12.Dpi = 254F;
			this.xrTableCell12.Name = "xrTableCell12";
			this.xrTableCell12.Text = "Phone";
			this.xrTableCell12.Weight = 0.41025641025641019D;
			// 
			// xrTableCell13
			// 
			this.xrTableCell13.Dpi = 254F;
			this.xrTableCell13.Name = "xrTableCell13";
			this.xrTableCell13.Text = "head";
			this.xrTableCell13.Weight = 0.66666666666666674D;
			// 
			// xrTableCell14
			// 
			this.xrTableCell14.Dpi = 254F;
			this.xrTableCell14.Name = "xrTableCell14";
			this.xrTableCell14.Text = "A higher division";
			this.xrTableCell14.Weight = 0.66666666666666652D;
			// 
			// xrTableCell15
			// 
			this.xrTableCell15.Dpi = 254F;
			this.xrTableCell15.Name = "xrTableCell15";
			this.xrTableCell15.StylePriority.UseBackColor = false;
			this.xrTableCell15.Text = "Note";
			this.xrTableCell15.Weight = 0.51282051282051277D;
			// 
			// xrTableCellArchiveHeader
			// 
			this.xrTableCellArchiveHeader.Dpi = 254F;
			this.xrTableCellArchiveHeader.Name = "xrTableCellArchiveHeader";
			this.xrTableCellArchiveHeader.Text = "Archive";
			this.xrTableCellArchiveHeader.Weight = 0.20512820512820509D;
			// 
			// xrTableContent
			// 
			this.xrTableContent.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTableContent.Dpi = 254F;
			this.xrTableContent.KeepTogether = true;
			this.xrTableContent.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTableContent.Name = "xrTableContent";
			this.xrTableContent.OddStyleName = "xrControlStyle1";
			this.xrTableContent.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTableContent.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			this.xrTableRow2});
			this.xrTableContent.SizeF = new System.Drawing.SizeF(1686.771F, 63.5F);
			this.xrTableContent.StylePriority.UseBorders = false;
			this.xrTableContent.StylePriority.UsePadding = false;
			this.xrTableContent.StylePriority.UseTextAlignment = false;
			this.xrTableContent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.xrTableCellLevel,
			this.xrTableCell6,
			this.xrTableCell7,
			this.xrTableCell8,
			this.xrTableCell9,
			this.xrTableCell10,
			this.xrTableCellArchive});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Weight = 11.5D;
			// 
			// xrTableCellLevel
			// 
			this.xrTableCellLevel.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Level")});
			this.xrTableCellLevel.Dpi = 254F;
			this.xrTableCellLevel.Name = "xrTableCellLevel";
			this.xrTableCellLevel.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			this.xrTableCellLevel.StylePriority.UsePadding = false;
			this.xrTableCellLevel.StylePriority.UseTextAlignment = false;
			this.xrTableCellLevel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			this.xrTableCellLevel.Weight = 0.41709396492721684D;
			this.xrTableCellLevel.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrTableCellLevel_BeforePrint);
			// 
			// xrTableCell6
			// 
			this.xrTableCell6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department")});
			this.xrTableCell6.Dpi = 254F;
			this.xrTableCell6.Name = "xrTableCell6";
			this.xrTableCell6.Text = "xrTableCell6";
			this.xrTableCell6.Weight = 0.66666658841646631D;
			// 
			// xrTableCell7
			// 
			this.xrTableCell7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Phone")});
			this.xrTableCell7.Dpi = 254F;
			this.xrTableCell7.Name = "xrTableCell7";
			this.xrTableCell7.Text = "xrTableCell7";
			this.xrTableCell7.Weight = 0.41025648850661062D;
			// 
			// xrTableCell8
			// 
			this.xrTableCell8.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Chief")});
			this.xrTableCell8.Dpi = 254F;
			this.xrTableCell8.Name = "xrTableCell8";
			this.xrTableCell8.Text = "xrTableCell8";
			this.xrTableCell8.Weight = 0.66666651016626588D;
			// 
			// xrTableCell9
			// 
			this.xrTableCell9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ParentDepartment")});
			this.xrTableCell9.Dpi = 254F;
			this.xrTableCell9.Name = "xrTableCell9";
			this.xrTableCell9.Text = "xrTableCell9";
			this.xrTableCell9.Weight = 0.66666682316706738D;
			// 
			// xrTableCell10
			// 
			this.xrTableCell10.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Description")});
			this.xrTableCell10.Dpi = 254F;
			this.xrTableCell10.Name = "xrTableCell10";
			this.xrTableCell10.Text = "xrTableCell10";
			this.xrTableCell10.Weight = 0.51282012156951118D;
			// 
			// xrTableCellArchive
			// 
			this.xrTableCellArchive.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ArchiveLabel")});
			this.xrTableCellArchive.Dpi = 254F;
			this.xrTableCellArchive.Name = "xrTableCellArchive";
			this.xrTableCellArchive.StylePriority.UseTextAlignment = false;
			this.xrTableCellArchive.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellArchive.Weight = 0.20512820512820509D;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// ArchiveLabel
			// 
			this.ArchiveLabel.DataMember = "Data";
			this.ArchiveLabel.Expression = "Iif([IsArchive] == True, \'+\' , \'\')";
			this.ArchiveLabel.Name = "ArchiveLabel";
			// 
			// DepartmentsReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
			this.GroupHeader1,
			this.Detail});
			this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
			this.ArchiveLabel});
			this.DataMember = "Data";
			this.DataSourceSchema = resources.GetString("$this.DataSourceSchema");
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
			this.xrControlStyle1});
			this.Version = "15.1";
			this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Report415_BeforePrint);
			this.Controls.SetChildIndex(this.Detail, 0);
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTableHeader)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTableContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTableHeader;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow3;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell11;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell12;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell13;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell14;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell15;
		private DevExpress.XtraReports.UI.XRTable xrTableContent;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell6;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell9;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellArchiveHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellArchive;
		private DevExpress.XtraReports.UI.CalculatedField ArchiveLabel;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell16;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellLevel;
	}
}
