using System.Data;
namespace Resurs.Reports.Templates
{
	partial class ChangeFlowReport
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
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
			this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
			this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
			this.AbonentName = new DevExpress.XtraReports.Parameters.Parameter();
			this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
			this.Address = new DevExpress.XtraReports.Parameters.Parameter();
			this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
			this.DeviceName = new DevExpress.XtraReports.Parameters.Parameter();
			this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
			this.EndTime = new DevExpress.XtraReports.Parameters.Parameter();
			this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
			this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
			this.StartTime = new DevExpress.XtraReports.Parameters.Parameter();
			this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
			this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
			this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
			this.counterDataSet1 = new Resurs.Reports.DataSources.CounterDataSet();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.counterDataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
			this.Detail.HeightF = 31.25F;
			this.Detail.Name = "Detail";
			this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// xrTable1
			// 
			this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable1.Name = "xrTable1";
			this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
			this.xrTable1.SizeF = new System.Drawing.SizeF(650F, 25F);
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3});
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCell1
			// 
			this.xrTableCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DateTime")});
			this.xrTableCell1.Name = "xrTableCell1";
			this.xrTableCell1.Weight = 0.32692311716603706D;
			// 
			// xrTableCell2
			// 
			this.xrTableCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Tariff")});
			this.xrTableCell2.Name = "xrTableCell2";
			this.xrTableCell2.Weight = 0.20054935664921017D;
			// 
			// xrTableCell3
			// 
			this.xrTableCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.CounterValue")});
			this.xrTableCell3.Name = "xrTableCell3";
			this.xrTableCell3.Weight = 0.32967038332760989D;
			// 
			// TopMargin
			// 
			this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine1,
            this.xrLabel15,
            this.xrLabel14,
            this.xrLabel13,
            this.xrLabel12,
            this.xrLabel11,
            this.xrLabel10,
            this.xrLabel9,
            this.xrLabel8,
            this.xrLabel1,
            this.xrLabel7});
			this.TopMargin.HeightF = 257.2501F;
			this.TopMargin.Name = "TopMargin";
			this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.TopMargin.StylePriority.UseTextAlignment = false;
			this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// xrLine1
			// 
			this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.041565F, 255F);
			this.xrLine1.Name = "xrLine1";
			this.xrLine1.SizeF = new System.Drawing.SizeF(648.9583F, 2F);
			// 
			// xrLabel15
			// 
			this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
			this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(399.9999F, 232F);
			this.xrLabel15.Name = "xrLabel15";
			this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel15.SizeF = new System.Drawing.SizeF(154.1667F, 23F);
			this.xrLabel15.StylePriority.UseFont = false;
			this.xrLabel15.Text = "Значение расхода";
			// 
			// xrLabel14
			// 
			this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
			this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(247.9166F, 232F);
			this.xrLabel14.Name = "xrLabel14";
			this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel14.SizeF = new System.Drawing.SizeF(100F, 23F);
			this.xrLabel14.StylePriority.UseFont = false;
			this.xrLabel14.Text = "Тариф";
			// 
			// xrLabel13
			// 
			this.xrLabel13.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
			this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(1.041667F, 232F);
			this.xrLabel13.Name = "xrLabel13";
			this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel13.SizeF = new System.Drawing.SizeF(42.70833F, 23F);
			this.xrLabel13.StylePriority.UseFont = false;
			this.xrLabel13.Text = "Дата";
			// 
			// xrLabel12
			// 
			this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
			this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(247.9166F, 200.75F);
			this.xrLabel12.Name = "xrLabel12";
			this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel12.SizeF = new System.Drawing.SizeF(152.0833F, 17.70834F);
			this.xrLabel12.StylePriority.UseFont = false;
			this.xrLabel12.StylePriority.UseTextAlignment = false;
			this.xrLabel12.Text = "История расхода";
			this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
			// 
			// xrLabel11
			// 
			this.xrLabel11.CanGrow = false;
			this.xrLabel11.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.AbonentName, "Text", "Имя пользователя счетчика: {0}")});
			this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(10F, 163.3749F);
			this.xrLabel11.Name = "xrLabel11";
			this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel11.SizeF = new System.Drawing.SizeF(629.9999F, 23F);
			this.xrLabel11.Text = "[]";
			// 
			// AbonentName
			// 
			this.AbonentName.Description = "AbonentName";
			this.AbonentName.Name = "AbonentName";
			// 
			// xrLabel10
			// 
			this.xrLabel10.CanGrow = false;
			this.xrLabel10.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.Address, "Text", "Адрес счетчика: {0}")});
			this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(10F, 140.3749F);
			this.xrLabel10.Name = "xrLabel10";
			this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel10.SizeF = new System.Drawing.SizeF(629.9998F, 23F);
			this.xrLabel10.Text = "xrLabel10";
			// 
			// Address
			// 
			this.Address.Description = "Address";
			this.Address.Name = "Address";
			this.Address.Type = typeof(int);
			this.Address.ValueInfo = "0";
			// 
			// xrLabel9
			// 
			this.xrLabel9.CanGrow = false;
			this.xrLabel9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.DeviceName, "Text", "Название счетчика: {0}")});
			this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(10F, 71.37502F);
			this.xrLabel9.Name = "xrLabel9";
			this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel9.SizeF = new System.Drawing.SizeF(629.9999F, 23F);
			// 
			// DeviceName
			// 
			this.DeviceName.Description = "DeviceName";
			this.DeviceName.Name = "DeviceName";
			// 
			// xrLabel8
			// 
			this.xrLabel8.CanGrow = false;
			this.xrLabel8.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.EndTime, "Text", "Время окончания: {0}")});
			this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(10F, 117.375F);
			this.xrLabel8.Name = "xrLabel8";
			this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel8.SizeF = new System.Drawing.SizeF(629.9998F, 23F);
			this.xrLabel8.Text = "xrLabel8";
			// 
			// EndTime
			// 
			this.EndTime.Description = "EndTime";
			this.EndTime.Name = "EndTime";
			this.EndTime.Type = typeof(System.DateTime);
			this.EndTime.ValueInfo = "2015-10-05";
			// 
			// xrLabel1
			// 
			this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold);
			this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(192.7082F, 37.08334F);
			this.xrLabel1.Name = "xrLabel1";
			this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel1.SizeF = new System.Drawing.SizeF(282.2083F, 23.87501F);
			this.xrLabel1.StylePriority.UseFont = false;
			this.xrLabel1.StylePriority.UseTextAlignment = false;
			this.xrLabel1.Text = "Изменение расхода счетчика";
			this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
			// 
			// xrLabel7
			// 
			this.xrLabel7.CanGrow = false;
			this.xrLabel7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.StartTime, "Text", "Время начала: {0}")});
			this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(10F, 94.37504F);
			this.xrLabel7.Name = "xrLabel7";
			this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrLabel7.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
			this.xrLabel7.SizeF = new System.Drawing.SizeF(629.9999F, 22.99999F);
			// 
			// StartTime
			// 
			this.StartTime.Description = "StartTime";
			this.StartTime.Name = "StartTime";
			this.StartTime.Type = typeof(System.DateTime);
			this.StartTime.ValueInfo = "2015-10-05";
			// 
			// BottomMargin
			// 
			this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo1,
            this.xrPageInfo2});
			this.BottomMargin.HeightF = 98.95834F;
			this.BottomMargin.Name = "BottomMargin";
			this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
			this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
			// 
			// xrPageInfo1
			// 
			this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(550F, 27.41667F);
			this.xrPageInfo1.Name = "xrPageInfo1";
			this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrPageInfo1.SizeF = new System.Drawing.SizeF(100F, 23F);
			this.xrPageInfo1.StylePriority.UseTextAlignment = false;
			this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
			// 
			// xrPageInfo2
			// 
			this.xrPageInfo2.Format = "{0:d MMMM yyyy \'г.\'}";
			this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 27.41667F);
			this.xrPageInfo2.Name = "xrPageInfo2";
			this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
			this.xrPageInfo2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
			this.xrPageInfo2.SizeF = new System.Drawing.SizeF(120.8333F, 23F);
			// 
			// counterDataSet1
			// 
			this.counterDataSet1.DataSetName = "CounterDataSet";
			this.counterDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// ChangeFlowReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
			this.DataMember = "Data";
			this.DataSource = this.counterDataSet1;
			this.Margins = new System.Drawing.Printing.Margins(100, 100, 257, 99);
			this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.StartTime,
            this.EndTime,
            this.DeviceName,
            this.Address,
            this.AbonentName});
			this.RequestParameters = false;
			this.Version = "15.1";
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.counterDataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.DetailBand Detail;
		private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
		private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
		private DevExpress.XtraReports.UI.XRLabel xrLabel1;
		private DevExpress.XtraReports.UI.XRLabel xrLabel7;
		private DevExpress.XtraReports.Parameters.Parameter StartTime;
		private DevExpress.XtraReports.UI.XRLabel xrLabel8;
		private DevExpress.XtraReports.Parameters.Parameter EndTime;
		private DevExpress.XtraReports.Parameters.Parameter DeviceName;
		private DevExpress.XtraReports.UI.XRLabel xrLabel9;
		private DevExpress.XtraReports.UI.XRLabel xrLabel10;
		private DevExpress.XtraReports.Parameters.Parameter Address;
		private DevExpress.XtraReports.UI.XRLabel xrLabel11;
		private DevExpress.XtraReports.Parameters.Parameter AbonentName;
		private DevExpress.XtraReports.UI.XRLine xrLine1;
		private DevExpress.XtraReports.UI.XRLabel xrLabel15;
		private DevExpress.XtraReports.UI.XRLabel xrLabel14;
		private DevExpress.XtraReports.UI.XRLabel xrLabel13;
		private DevExpress.XtraReports.UI.XRLabel xrLabel12;
		private Resurs.Reports.DataSources.CounterDataSet counterDataSet1;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
		private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
		private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo2;
	}
}
