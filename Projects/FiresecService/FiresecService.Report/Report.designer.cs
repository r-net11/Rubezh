using System;
using System.Collections.Generic;
using System.Text;

namespace FiresecService.Report
{
    public partial class Report {
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTable xrTable2;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel lbOrderID;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell13;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRTableCell clProductName;
        private DevExpress.XtraReports.UI.XRTableCell clUnitPrice;
        private DevExpress.XtraReports.UI.XRTableCell clQuantity;
        private DevExpress.XtraReports.UI.XRTableCell clDiscount;
        private DevExpress.XtraReports.UI.XRTableCell clSubtotal;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
        private DevExpress.XtraReports.UI.XRTableCell clSupplier;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
		private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
		internal DevExpress.XtraReports.UI.XRPictureBox xrPictureBox4;
        private DevExpress.XtraReports.UI.XRControlStyle EvenStyle;
        private DevExpress.XtraReports.UI.XRControlStyle OddStyle;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report));
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            this.EvenStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.OddStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.clProductName = new DevExpress.XtraReports.UI.XRTableCell();
            this.clSupplier = new DevExpress.XtraReports.UI.XRTableCell();
            this.clUnitPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.clQuantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.clDiscount = new DevExpress.XtraReports.UI.XRTableCell();
            this.clSubtotal = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.lbOrderID = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPictureBox2 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrPictureBox4 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.MaxRowCountParameter = new DevExpress.XtraReports.Parameters.Parameter();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.OrderIdParameter = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsOrderDetails1 = new DevExpress.DataAccess.Sql.SqlDataSource("NWindConnectionString");
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // EvenStyle
            // 
            this.EvenStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
            this.EvenStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(207)))), ((int)(((byte)(189)))));
            this.EvenStyle.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.EvenStyle.BorderWidth = 1F;
            this.EvenStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.EvenStyle.ForeColor = System.Drawing.Color.Black;
            this.EvenStyle.Name = "EvenStyle";
            // 
            // OddStyle
            // 
            this.OddStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(232)))), ((int)(((byte)(220)))));
            this.OddStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(207)))), ((int)(((byte)(189)))));
            this.OddStyle.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.OddStyle.BorderWidth = 1F;
            this.OddStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.OddStyle.ForeColor = System.Drawing.Color.Black;
            this.OddStyle.Name = "OddStyle";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.xrTable2});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.FormattingRules.Add(this.formattingRule1);
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            this.xrPageBreak1.Visible = false;
            // 
            // formattingRule1
            // 
            this.formattingRule1.Condition = "([DataSource.CurrentRowIndex] % [Parameters.MaxRowCountParameter]  == 0) And ([Da" +
    "taSource.CurrentRowIndex] != 0)";
            this.formattingRule1.DataMember = "OrderDetails";
            // 
            // 
            // 
            this.formattingRule1.Formatting.Visible = DevExpress.Utils.DefaultBoolean.True;
            this.formattingRule1.Name = "formattingRule1";
            // 
            // xrTable2
            // 
            this.xrTable2.BorderColor = System.Drawing.Color.Black;
            this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable2.BorderWidth = 1F;
            this.xrTable2.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(650F, 25F);
            this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.clProductName,
            this.clSupplier,
            this.clUnitPrice,
            this.clQuantity,
            this.clDiscount,
            this.clSubtotal});
            this.xrTableRow2.EvenStyleName = "EvenStyle";
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.OddStyleName = "OddStyle";
            this.xrTableRow2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTableRow2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableRow2.Weight = 1D;
            // 
            // clProductName
            // 
            this.clProductName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.ProductName")});
            this.clProductName.Name = "clProductName";
            this.clProductName.Padding = new DevExpress.XtraPrinting.PaddingInfo(7, 5, 3, 3, 100F);
            this.clProductName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.clProductName.Weight = 0.25384615384615383D;
            // 
            // clSupplier
            // 
            this.clSupplier.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.Supplier")});
            this.clSupplier.Name = "clSupplier";
            this.clSupplier.Padding = new DevExpress.XtraPrinting.PaddingInfo(7, 3, 3, 3, 100F);
            this.clSupplier.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.clSupplier.Weight = 0.31538461538461537D;
            // 
            // clUnitPrice
            // 
            this.clUnitPrice.BorderWidth = 1F;
            this.clUnitPrice.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.UnitPrice", "{0:$#,##.00}")});
            this.clUnitPrice.Name = "clUnitPrice";
            this.clUnitPrice.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.clUnitPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.clUnitPrice.Weight = 0.1076923076923077D;
            // 
            // clQuantity
            // 
            this.clQuantity.BorderWidth = 1F;
            this.clQuantity.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Tag", null, "OrderDetails.UnitPrice"),
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.Quantity")});
            this.clQuantity.Name = "clQuantity";
            this.clQuantity.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.clQuantity.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.clQuantity.Weight = 0.1076923076923077D;
            // 
            // clDiscount
            // 
            this.clDiscount.BorderWidth = 1F;
            this.clDiscount.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Tag", null, "OrderDetails.Quantity", "{0:#,##.00%}"),
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.Discount", "{0:0%}")});
            this.clDiscount.Name = "clDiscount";
            this.clDiscount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.clDiscount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.clDiscount.Weight = 0.1076923076923077D;
            // 
            // clSubtotal
            // 
            this.clSubtotal.BorderWidth = 1F;
            this.clSubtotal.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.SubTotal", "{0:$#,##.00}")});
            this.clSubtotal.Name = "clSubtotal";
            this.clSubtotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.clSubtotal.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.clSubtotal.Weight = 0.1076923076923077D;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo1,
            this.lbOrderID,
            this.xrLabel1});
            this.ReportHeader.HeightF = 84F;
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.ReportHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(406F, 67F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(245F, 16F);
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // lbOrderID
            // 
            this.lbOrderID.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.OrderID")});
            this.lbOrderID.Font = new System.Drawing.Font("Tahoma", 18F);
            this.lbOrderID.LocationFloat = new DevExpress.Utils.PointFloat(425F, 17F);
            this.lbOrderID.Name = "lbOrderID";
            this.lbOrderID.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lbOrderID.SizeF = new System.Drawing.SizeF(117F, 33F);
            this.lbOrderID.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.CanShrink = true;
            this.xrLabel1.Font = new System.Drawing.Font("Tahoma", 18F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(50F, 17F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(366F, 33F);
            this.xrLabel1.Text = "Details of the customer order";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTable1
            // 
            this.xrTable1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(214)))), ((int)(((byte)(211)))));
            this.xrTable1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(177)))), ((int)(((byte)(183)))));
            this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable1.BorderWidth = 2F;
            this.xrTable1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(650F, 43F);
            this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell4,
            this.xrTableCell7,
            this.xrTableCell2,
            this.xrTableCell13,
            this.xrTableCell3});
            this.xrTableRow1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTableRow1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.BorderWidth = 1F;
            this.xrTableCell1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox2,
            this.xrLabel2});
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTableCell1.Text = "Product";
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell1.Weight = 0.25384615384615383D;
            // 
            // xrPictureBox2
            // 
            this.xrPictureBox2.BorderWidth = 0F;
            this.xrPictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("xrPictureBox2.Image")));
            this.xrPictureBox2.LocationFloat = new DevExpress.Utils.PointFloat(92F, 4F);
            this.xrPictureBox2.Name = "xrPictureBox2";
            this.xrPictureBox2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrPictureBox2.SizeF = new System.Drawing.SizeF(55.20833F, 37.5F);
            this.xrPictureBox2.Sizing = DevExpress.XtraPrinting.ImageSizeMode.AutoSize;
            // 
            // xrLabel2
            // 
            this.xrLabel2.BorderWidth = 0F;
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(8F, 8F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(66F, 30F);
            this.xrLabel2.Text = "Product";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.BorderWidth = 1F;
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell4.Text = "Supplier";
            this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell4.Weight = 0.31538461538461537D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.BorderWidth = 1F;
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell7.Text = "Unit Price";
            this.xrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell7.Weight = 0.1076923076923077D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.BorderWidth = 1F;
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell2.Text = "Quantity";
            this.xrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell2.Weight = 0.1076923076923077D;
            // 
            // xrTableCell13
            // 
            this.xrTableCell13.BorderWidth = 1F;
            this.xrTableCell13.Name = "xrTableCell13";
            this.xrTableCell13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell13.Text = "Discount";
            this.xrTableCell13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell13.Weight = 0.1076923076923077D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.BorderWidth = 1F;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell3.Text = "Subtotal";
            this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell3.Weight = 0.1076923076923077D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel3});
            this.ReportFooter.HeightF = 33F;
            this.ReportFooter.Name = "ReportFooter";
            this.ReportFooter.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.ReportFooter.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(214)))), ((int)(((byte)(211)))));
            this.xrLabel3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(177)))), ((int)(((byte)(181)))));
            this.xrLabel3.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel3.CanGrow = false;
            this.xrLabel3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.SubTotal")});
            this.xrLabel3.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(408F, 12F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(242F, 21F);
            xrSummary1.FormatString = "{0: Total order cost :   $0.00 }";
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.xrLabel3.Summary = xrSummary1;
            this.xrLabel3.Text = "xrLabel3";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel3.XlsxFormatString = "\"Total order cost: \"$0.00";
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.PageHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.PageHeader.HeightF = 44F;
            this.PageHeader.Name = "PageHeader";
            this.PageHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.PageHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox4});
            this.BottomMargin.HeightF = 75F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPictureBox4
            // 
            this.xrPictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("xrPictureBox4.Image")));
            this.xrPictureBox4.LocationFloat = new DevExpress.Utils.PointFloat(249F, 13F);
            this.xrPictureBox4.Name = "xrPictureBox4";
            this.xrPictureBox4.NavigateUrl = "http://www.devexpress.com/Products/NET/Reporting/";
            this.xrPictureBox4.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrPictureBox4.SizeF = new System.Drawing.SizeF(165.625F, 30.20833F);
            this.xrPictureBox4.Sizing = DevExpress.XtraPrinting.ImageSizeMode.AutoSize;
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5});
            this.GroupFooter1.HeightF = 33F;
            this.GroupFooter1.Name = "GroupFooter1";
            this.GroupFooter1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.GroupFooter1.RepeatEveryPage = true;
            this.GroupFooter1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel5
            // 
            this.xrLabel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(214)))), ((int)(((byte)(211)))));
            this.xrLabel5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(177)))), ((int)(((byte)(181)))));
            this.xrLabel5.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel5.CanGrow = false;
            this.xrLabel5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderDetails.SubTotal")});
            this.xrLabel5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(355F, 12F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(295F, 21F);
            xrSummary2.FormatString = "{0: Total order cost  for this page:   $0.00 }";
            xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Page;
            this.xrLabel5.Summary = xrSummary2;
            this.xrLabel5.Text = "xrLabel5";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel5.XlsxFormatString = "\"Total order cost for this page: \"$0.00";
            // 
            // MaxRowCountParameter
            // 
            this.MaxRowCountParameter.Description = "Max Rows per Page: ";
            this.MaxRowCountParameter.Name = "MaxRowCountParameter";
            this.MaxRowCountParameter.Type = typeof(int);
            this.MaxRowCountParameter.ValueInfo = "10";
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 100F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // OrderIdParameter
            // 
            this.OrderIdParameter.Description = "Order Id:";
            this.OrderIdParameter.Name = "OrderIdParameter";
            this.OrderIdParameter.Type = typeof(int);
            this.OrderIdParameter.ValueInfo = "11077";
            // 
            // dsOrderDetails1
            // 
            customSqlQuery1.Name = "OrderDetails";
            queryParameter1.Name = "OrderId";
            queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter1.ValueInfo = "[Parameters.OrderIdParameter]";
            customSqlQuery1.Parameters.Add(queryParameter1);
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.dsOrderDetails1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.dsOrderDetails1.ResultSchemaSerializable = resources.GetString("dsOrderDetails1.ResultSchemaSerializable");
            // 
            // Report
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.ReportHeader,
            this.ReportFooter,
            this.PageHeader,
            this.BottomMargin,
            this.GroupFooter1,
            this.topMarginBand1});
            this.DataMember = "OrderDetails";
            this.DataSource = this.dsOrderDetails1;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(100, 96, 100, 75);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.OrderIdParameter,
            this.MaxRowCountParameter});
            this.ReportPrintOptions.DetailCountAtDesignTime = 7;
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.EvenStyle,
            this.OddStyle});
            this.Version = "14.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        private DevExpress.XtraReports.Parameters.Parameter MaxRowCountParameter;
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.Parameters.Parameter OrderIdParameter;
        private DevExpress.DataAccess.Sql.SqlDataSource dsOrderDetails1;
    }
}
