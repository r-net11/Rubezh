using DevExpress.XtraReports.UI;

namespace FiresecService.Report.Templates
{
	partial class WorkingTimeReport
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkingTimeReport));
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.EmployeeValueCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.DepartmentValueCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.PositionValueCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.WorkingTimeValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.WorkingTimeValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.RealTotalTimeValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.RealTotalTimeValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.NonAcceptedAdjustmentsValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.AbsenceRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.NonAcceptedAdjustmentsValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.OvertimeRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.DocumentsTimeValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.PresenceDocumentRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.DocumentsTimeValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.AbsenceReasonableRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.DocumentsTimeValueCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.AbsenceDocumentRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.DocumentsTimeValueCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.OvertimeDocumentRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.TotalBalanceValueCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.TotalBalanceRule = new DevExpress.XtraReports.UI.FormattingRule();
			this.EmployeeHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.HeaderTable = new DevExpress.XtraReports.UI.XRTable();
			this.HeaderRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.EmployeeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.DepartmentHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.PositionHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.WorkingTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.RealTotalTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.NonAcceptedAdjustmentsHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.DocumentsTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.TotalBalanceHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
			this.HeaderRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.DepartmentHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.PositionHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.WorkingTimeHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.WorkingTimeHeaderCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.RealTotalTimeHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.RealTotalTimeHeaderCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.NonAcceptedAdjustmentsHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.NonAcceptedAdjustmentsHeaderCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.DocumentsTimeHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.DocumentsTimeHeaderCell3 = new DevExpress.XtraReports.UI.XRTableCell();
			this.DocumentsTimeHeaderCell4 = new DevExpress.XtraReports.UI.XRTableCell();
			this.DocumentsTimeHeaderCell5 = new DevExpress.XtraReports.UI.XRTableCell();
			this.TotalBalanceHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
			this.DepartmentHeaderCell9 = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.HeaderTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			this.xrTable2});
			this.Detail.HeightF = 64F;
			this.Detail.KeepTogether = true;
			// 
			// xrTable2
			// 
			this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable2.Dpi = 254F;
			this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable2.Name = "xrTable2";
			this.xrTable2.OddStyleName = "xrControlStyle1";
			this.xrTable2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			this.xrTableRow2});
			this.xrTable2.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			this.xrTable2.StylePriority.UseBorders = false;
			this.xrTable2.StylePriority.UsePadding = false;
			this.xrTable2.StylePriority.UseTextAlignment = false;
			this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.EmployeeValueCell,
			this.DepartmentValueCell,
			this.PositionValueCell,
			this.WorkingTimeValueCell2,
			this.WorkingTimeValueCell3,
			this.RealTotalTimeValueCell2,
			this.RealTotalTimeValueCell3,
			this.NonAcceptedAdjustmentsValueCell2,
			this.NonAcceptedAdjustmentsValueCell3,
			this.DocumentsTimeValueCell2,
			this.DocumentsTimeValueCell3,
			this.DocumentsTimeValueCell4,
			this.DocumentsTimeValueCell5,
			this.TotalBalanceValueCell});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Weight = 11.5D;
			// 
			// EmployeeValueCell
			// 
			this.EmployeeValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Employee")});
			this.EmployeeValueCell.Dpi = 254F;
			this.EmployeeValueCell.Name = "EmployeeValueCell";
			this.EmployeeValueCell.StylePriority.UseTextAlignment = false;
			this.EmployeeValueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			this.EmployeeValueCell.Weight = 135D;
			// 
			// DepartmentValueCell
			// 
			this.DepartmentValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department")});
			this.DepartmentValueCell.Dpi = 254F;
			this.DepartmentValueCell.Name = "DepartmentValueCell";
			this.DepartmentValueCell.StylePriority.UseTextAlignment = false;
			this.DepartmentValueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			this.DepartmentValueCell.Weight = 135D;
			// 
			// PositionValueCell
			// 
			this.PositionValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Position")});
			this.PositionValueCell.Dpi = 254F;
			this.PositionValueCell.Name = "PositionValueCell";
			this.PositionValueCell.StylePriority.UseTextAlignment = false;
			this.PositionValueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			this.PositionValueCell.Weight = 135D;
			// 
			// WorkingTimeValueCell2
			// 
			this.WorkingTimeValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ScheduleDay")});
			this.WorkingTimeValueCell2.Dpi = 254F;
			this.WorkingTimeValueCell2.Name = "WorkingTimeValueCell2";
			this.WorkingTimeValueCell2.Weight = 100D;
			// 
			// WorkingTimeValueCell3
			// 
			this.WorkingTimeValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ScheduleNight")});
			this.WorkingTimeValueCell3.Dpi = 254F;
			this.WorkingTimeValueCell3.Name = "WorkingTimeValueCell3";
			this.WorkingTimeValueCell3.Weight = 100D;
			// 
			// RealTotalTimeValueCell2
			// 
			this.RealTotalTimeValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.RealPresence")});
			this.RealTotalTimeValueCell2.Dpi = 254F;
			this.RealTotalTimeValueCell2.Name = "RealTotalTimeValueCell2";
			this.RealTotalTimeValueCell2.Weight = 100D;
			// 
			// RealTotalTimeValueCell3
			// 
			this.RealTotalTimeValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.RealNightTime")});
			this.RealTotalTimeValueCell3.Dpi = 254F;
			this.RealTotalTimeValueCell3.Name = "RealTotalTimeValueCell3";
			this.RealTotalTimeValueCell3.Weight = 100D;
			// 
			// NonAcceptedAdjustmentsValueCell2
			// 
			this.NonAcceptedAdjustmentsValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalAbsence")});
			this.NonAcceptedAdjustmentsValueCell2.Dpi = 254F;
			this.NonAcceptedAdjustmentsValueCell2.FormattingRules.Add(this.AbsenceRule);
			this.NonAcceptedAdjustmentsValueCell2.Name = "NonAcceptedAdjustmentsValueCell2";
			this.NonAcceptedAdjustmentsValueCell2.Weight = 150D;
			// 
			// AbsenceRule
			// 
			this.AbsenceRule.Condition = "[TotalAbsence] > 0";
			// 
			// 
			// 
			this.AbsenceRule.Formatting.ForeColor = System.Drawing.Color.Red;
			this.AbsenceRule.Name = "AbsenceRule";
			// 
			// NonAcceptedAdjustmentsValueCell3
			// 
			this.NonAcceptedAdjustmentsValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalNonAcceptedOvertime")});
			this.NonAcceptedAdjustmentsValueCell3.Dpi = 254F;
			this.NonAcceptedAdjustmentsValueCell3.FormattingRules.Add(this.OvertimeRule);
			this.NonAcceptedAdjustmentsValueCell3.Name = "NonAcceptedAdjustmentsValueCell3";
			this.NonAcceptedAdjustmentsValueCell3.Weight = 150D;
			// 
			// OvertimeRule
			// 
			this.OvertimeRule.Condition = "[TotalNonAcceptedOvertime] > 0";
			// 
			// 
			// 
			this.OvertimeRule.Formatting.ForeColor = System.Drawing.Color.DodgerBlue;
			this.OvertimeRule.Name = "OvertimeRule";
			// 
			// DocumentsTimeValueCell2
			// 
			this.DocumentsTimeValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentPresence")});
			this.DocumentsTimeValueCell2.Dpi = 254F;
			this.DocumentsTimeValueCell2.FormattingRules.Add(this.PresenceDocumentRule);
			this.DocumentsTimeValueCell2.Name = "DocumentsTimeValueCell2";
			this.DocumentsTimeValueCell2.Weight = 150D;
			// 
			// PresenceDocumentRule
			// 
			this.PresenceDocumentRule.Condition = "[DocumentPresence] > 0";
			// 
			// 
			// 
			this.PresenceDocumentRule.Formatting.ForeColor = System.Drawing.Color.LimeGreen;
			this.PresenceDocumentRule.Name = "PresenceDocumentRule";
			// 
			// DocumentsTimeValueCell3
			// 
			this.DocumentsTimeValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentAbsenceReasonable")});
			this.DocumentsTimeValueCell3.Dpi = 254F;
			this.DocumentsTimeValueCell3.FormattingRules.Add(this.AbsenceReasonableRule);
			this.DocumentsTimeValueCell3.Name = "DocumentsTimeValueCell3";
			this.DocumentsTimeValueCell3.Weight = 150D;
			// 
			// AbsenceReasonableRule
			// 
			this.AbsenceReasonableRule.Condition = "[DocumentAbsenceReasonable] > 0";
			// 
			// 
			// 
			this.AbsenceReasonableRule.Formatting.ForeColor = System.Drawing.Color.LimeGreen;
			this.AbsenceReasonableRule.Name = "AbsenceReasonableRule";
			// 
			// DocumentsTimeValueCell4
			// 
			this.DocumentsTimeValueCell4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentAbsence")});
			this.DocumentsTimeValueCell4.Dpi = 254F;
			this.DocumentsTimeValueCell4.FormattingRules.Add(this.AbsenceDocumentRule);
			this.DocumentsTimeValueCell4.Name = "DocumentsTimeValueCell4";
			this.DocumentsTimeValueCell4.Weight = 150D;
			// 
			// AbsenceDocumentRule
			// 
			this.AbsenceDocumentRule.Condition = "[DocumentAbsence] > 0";
			// 
			// 
			// 
			this.AbsenceDocumentRule.Formatting.ForeColor = System.Drawing.Color.Red;
			this.AbsenceDocumentRule.Name = "AbsenceDocumentRule";
			// 
			// DocumentsTimeValueCell5
			// 
			this.DocumentsTimeValueCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentOvertime")});
			this.DocumentsTimeValueCell5.Dpi = 254F;
			this.DocumentsTimeValueCell5.FormattingRules.Add(this.OvertimeDocumentRule);
			this.DocumentsTimeValueCell5.Name = "DocumentsTimeValueCell5";
			this.DocumentsTimeValueCell5.Weight = 150D;
			// 
			// OvertimeDocumentRule
			// 
			this.OvertimeDocumentRule.Condition = "[DocumentOvertime] > 0";
			// 
			// 
			// 
			this.OvertimeDocumentRule.Formatting.ForeColor = System.Drawing.Color.DodgerBlue;
			this.OvertimeDocumentRule.Name = "OvertimeDocumentRule";
			// 
			// TotalBalanceValueCell
			// 
			this.TotalBalanceValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalBalance")});
			this.TotalBalanceValueCell.Dpi = 254F;
			this.TotalBalanceValueCell.FormattingRules.Add(this.TotalBalanceRule);
			this.TotalBalanceValueCell.Name = "TotalBalanceValueCell";
			this.TotalBalanceValueCell.Weight = 200D;
			// 
			// TotalBalanceRule
			// 
			this.TotalBalanceRule.Condition = "[TotalBalance] < 0";
			// 
			// 
			// 
			this.TotalBalanceRule.Formatting.ForeColor = System.Drawing.Color.Red;
			this.TotalBalanceRule.Name = "TotalBalanceRule";
			// 
			// EmployeeHeaderCell2
			// 
			this.EmployeeHeaderCell2.Dpi = 254F;
			this.EmployeeHeaderCell2.Name = "EmployeeHeaderCell2";
			this.EmployeeHeaderCell2.Text = "EmployeeHeaderCell2";
			this.EmployeeHeaderCell2.Weight = 0.20930232558139536D;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.BackColor = System.Drawing.Color.LightGray;
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
			this.HeaderTable});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 135F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			this.GroupHeader1.StylePriority.UseBackColor = false;
			// 
			// HeaderTable
			// 
			this.HeaderTable.BackColor = System.Drawing.Color.DarkGray;
			this.HeaderTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
			| DevExpress.XtraPrinting.BorderSide.Right)
			| DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.HeaderTable.Dpi = 254F;
			this.HeaderTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0.06249619F);
			this.HeaderTable.Name = "HeaderTable";
			this.HeaderTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.HeaderTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
			this.HeaderRow1,
			this.HeaderRow2});
			this.HeaderTable.SizeF = new System.Drawing.SizeF(2550F, 134.9375F);
			this.HeaderTable.StylePriority.UseBackColor = false;
			this.HeaderTable.StylePriority.UseBorders = false;
			this.HeaderTable.StylePriority.UsePadding = false;
			this.HeaderTable.StylePriority.UseTextAlignment = false;
			this.HeaderTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			// 
			// HeaderRow1
			// 
			this.HeaderRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.EmployeeHeaderCell,
			this.DepartmentHeaderCell,
			this.PositionHeaderCell,
			this.WorkingTimeHeaderCell,
			this.RealTotalTimeHeaderCell,
			this.NonAcceptedAdjustmentsHeaderCell,
			this.DocumentsTimeHeaderCell,
			this.TotalBalanceHeaderCell});
			this.HeaderRow1.Dpi = 254F;
			this.HeaderRow1.Name = "HeaderRow1";
			this.HeaderRow1.Weight = 11.5D;
			// 
			// EmployeeHeaderCell
			// 
			this.EmployeeHeaderCell.Dpi = 254F;
			this.EmployeeHeaderCell.Name = "EmployeeHeaderCell";
			this.EmployeeHeaderCell.RowSpan = 2;
			this.EmployeeHeaderCell.StylePriority.UsePadding = false;
			this.EmployeeHeaderCell.StylePriority.UseTextAlignment = false;
			this.EmployeeHeaderCell.Text = "employee";
			this.EmployeeHeaderCell.Weight = 0.20930232558139536D;
			// 
			// DepartmentHeaderCell
			// 
			this.DepartmentHeaderCell.Dpi = 254F;
			this.DepartmentHeaderCell.Name = "DepartmentHeaderCell";
			this.DepartmentHeaderCell.RowSpan = 2;
			this.DepartmentHeaderCell.StylePriority.UsePadding = false;
			this.DepartmentHeaderCell.StylePriority.UseTextAlignment = false;
			this.DepartmentHeaderCell.Text = "Subdivision";
			this.DepartmentHeaderCell.Weight = 0.20930232558139536D;
			// 
			// PositionHeaderCell
			// 
			this.PositionHeaderCell.Dpi = 254F;
			this.PositionHeaderCell.Name = "PositionHeaderCell";
			this.PositionHeaderCell.RowSpan = 2;
			this.PositionHeaderCell.StylePriority.UsePadding = false;
			this.PositionHeaderCell.StylePriority.UseTextAlignment = false;
			this.PositionHeaderCell.Text = "Position";
			this.PositionHeaderCell.Weight = 0.20930232558139536D;
			// 
			// WorkingTimeHeaderCell
			// 
			this.WorkingTimeHeaderCell.Dpi = 254F;
			this.WorkingTimeHeaderCell.Name = "WorkingTimeHeaderCell";
			this.WorkingTimeHeaderCell.StylePriority.UsePadding = false;
			this.WorkingTimeHeaderCell.StylePriority.UseTextAlignment = false;
			this.WorkingTimeHeaderCell.Text = "Working time schedule for the period h.";
			this.WorkingTimeHeaderCell.Weight = 0.31007751937984496D;
			// 
			// RealTotalTimeHeaderCell
			// 
			this.RealTotalTimeHeaderCell.Dpi = 254F;
			this.RealTotalTimeHeaderCell.Name = "RealTotalTimeHeaderCell";
			this.RealTotalTimeHeaderCell.StylePriority.UsePadding = false;
			this.RealTotalTimeHeaderCell.StylePriority.UseTextAlignment = false;
			this.RealTotalTimeHeaderCell.Text = "In fact, hours worked, hours.";
			this.RealTotalTimeHeaderCell.Weight = 0.31007751937984496D;
			// 
			// NonAcceptedAdjustmentsHeaderCell
			// 
			this.NonAcceptedAdjustmentsHeaderCell.Dpi = 254F;
			this.NonAcceptedAdjustmentsHeaderCell.Name = "NonAcceptedAdjustmentsHeaderCell";
			this.NonAcceptedAdjustmentsHeaderCell.StylePriority.UsePadding = false;
			this.NonAcceptedAdjustmentsHeaderCell.StylePriority.UseTextAlignment = false;
			this.NonAcceptedAdjustmentsHeaderCell.Text = "Deviations from the schedule is not confirmed by a document h.";
			this.NonAcceptedAdjustmentsHeaderCell.Weight = 0.46511627906976744D;
			// 
			// DocumentsTimeHeaderCell
			// 
			this.DocumentsTimeHeaderCell.Dpi = 254F;
			this.DocumentsTimeHeaderCell.Name = "DocumentsTimeHeaderCell";
			this.DocumentsTimeHeaderCell.StylePriority.UsePadding = false;
			this.DocumentsTimeHeaderCell.StylePriority.UseTextAlignment = false;
			this.DocumentsTimeHeaderCell.Text = "according to the documents, h.";
			this.DocumentsTimeHeaderCell.Weight = 0.93023255813953487D;
			// 
			// TotalBalanceHeaderCell
			// 
			this.TotalBalanceHeaderCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalBalanceHeaderName")});
			this.TotalBalanceHeaderCell.Dpi = 254F;
			this.TotalBalanceHeaderCell.Name = "TotalBalanceHeaderCell";
			this.TotalBalanceHeaderCell.RowSpan = 2;
			this.TotalBalanceHeaderCell.StylePriority.UsePadding = false;
			this.TotalBalanceHeaderCell.StylePriority.UseTextAlignment = false;
			this.TotalBalanceHeaderCell.Weight = 0.31007751937984496D;
			// 
			// HeaderRow2
			// 
			this.HeaderRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
			this.EmployeeHeaderCell2,
			this.DepartmentHeaderCell2,
			this.PositionHeaderCell2,
			this.WorkingTimeHeaderCell2,
			this.WorkingTimeHeaderCell3,
			this.RealTotalTimeHeaderCell2,
			this.RealTotalTimeHeaderCell3,
			this.NonAcceptedAdjustmentsHeaderCell2,
			this.NonAcceptedAdjustmentsHeaderCell3,
			this.DocumentsTimeHeaderCell2,
			this.DocumentsTimeHeaderCell3,
			this.DocumentsTimeHeaderCell4,
			this.DocumentsTimeHeaderCell5,
			this.TotalBalanceHeaderCell2});
			this.HeaderRow2.Dpi = 254F;
			this.HeaderRow2.Name = "HeaderRow2";
			this.HeaderRow2.Weight = 11.5D;
			// 
			// DepartmentHeaderCell2
			// 
			this.DepartmentHeaderCell2.Dpi = 254F;
			this.DepartmentHeaderCell2.Name = "DepartmentHeaderCell2";
			this.DepartmentHeaderCell2.Text = "DepartmentHeaderCell2";
			this.DepartmentHeaderCell2.Weight = 0.20930232558139536D;
			// 
			// PositionHeaderCell2
			// 
			this.PositionHeaderCell2.Dpi = 254F;
			this.PositionHeaderCell2.Name = "PositionHeaderCell2";
			this.PositionHeaderCell2.Text = "PositionHeaderCell2";
			this.PositionHeaderCell2.Weight = 0.20930232558139536D;
			// 
			// WorkingTimeHeaderCell2
			// 
			this.WorkingTimeHeaderCell2.Dpi = 254F;
			this.WorkingTimeHeaderCell2.Name = "WorkingTimeHeaderCell2";
			this.WorkingTimeHeaderCell2.StylePriority.UsePadding = false;
			this.WorkingTimeHeaderCell2.StylePriority.UseTextAlignment = false;
			this.WorkingTimeHeaderCell2.Text = "Total";
			this.WorkingTimeHeaderCell2.Weight = 0.15503875968992248D;
			// 
			// WorkingTimeHeaderCell3
			// 
			this.WorkingTimeHeaderCell3.Dpi = 254F;
			this.WorkingTimeHeaderCell3.Name = "WorkingTimeHeaderCell3";
			this.WorkingTimeHeaderCell3.StylePriority.UsePadding = false;
			this.WorkingTimeHeaderCell3.StylePriority.UseTextAlignment = false;
			this.WorkingTimeHeaderCell3.Text = "Including night";
			this.WorkingTimeHeaderCell3.Weight = 0.15503875968992248D;
			// 
			// RealTotalTimeHeaderCell2
			// 
			this.RealTotalTimeHeaderCell2.Dpi = 254F;
			this.RealTotalTimeHeaderCell2.Name = "RealTotalTimeHeaderCell2";
			this.RealTotalTimeHeaderCell2.StylePriority.UsePadding = false;
			this.RealTotalTimeHeaderCell2.StylePriority.UseTextAlignment = false;
			this.RealTotalTimeHeaderCell2.Text = "Total";
			this.RealTotalTimeHeaderCell2.Weight = 0.15503875968992248D;
			// 
			// RealTotalTimeHeaderCell3
			// 
			this.RealTotalTimeHeaderCell3.Dpi = 254F;
			this.RealTotalTimeHeaderCell3.Name = "RealTotalTimeHeaderCell3";
			this.RealTotalTimeHeaderCell3.StylePriority.UsePadding = false;
			this.RealTotalTimeHeaderCell3.StylePriority.UseTextAlignment = false;
			this.RealTotalTimeHeaderCell3.Text = "Including work at night";
			this.RealTotalTimeHeaderCell3.Weight = 0.15503875968992248D;
			// 
			// NonAcceptedAdjustmentsHeaderCell2
			// 
			this.NonAcceptedAdjustmentsHeaderCell2.Dpi = 254F;
			this.NonAcceptedAdjustmentsHeaderCell2.Name = "NonAcceptedAdjustmentsHeaderCell2";
			this.NonAcceptedAdjustmentsHeaderCell2.StylePriority.UsePadding = false;
			this.NonAcceptedAdjustmentsHeaderCell2.StylePriority.UseTextAlignment = false;
			this.NonAcceptedAdjustmentsHeaderCell2.Text = "Absence (including late arrivals and early departures)";
			this.NonAcceptedAdjustmentsHeaderCell2.Weight = 0.23255813953488372D;
			// 
			// NonAcceptedAdjustmentsHeaderCell3
			// 
			this.NonAcceptedAdjustmentsHeaderCell3.Dpi = 254F;
			this.NonAcceptedAdjustmentsHeaderCell3.Name = "NonAcceptedAdjustmentsHeaderCell3";
			this.NonAcceptedAdjustmentsHeaderCell3.StylePriority.UsePadding = false;
			this.NonAcceptedAdjustmentsHeaderCell3.StylePriority.UseTextAlignment = false;
			this.NonAcceptedAdjustmentsHeaderCell3.Text = "Recycling is the schedule";
			this.NonAcceptedAdjustmentsHeaderCell3.Weight = 0.23255813953488372D;
			// 
			// DocumentsTimeHeaderCell2
			// 
			this.DocumentsTimeHeaderCell2.Dpi = 254F;
			this.DocumentsTimeHeaderCell2.Name = "DocumentsTimeHeaderCell2";
			this.DocumentsTimeHeaderCell2.StylePriority.UsePadding = false;
			this.DocumentsTimeHeaderCell2.StylePriority.UseTextAlignment = false;
			this.DocumentsTimeHeaderCell2.Text = "Presence";
			this.DocumentsTimeHeaderCell2.Weight = 0.23255813953488372D;
			// 
			// DocumentsTimeHeaderCell3
			// 
			this.DocumentsTimeHeaderCell3.Dpi = 254F;
			this.DocumentsTimeHeaderCell3.Name = "DocumentsTimeHeaderCell3";
			this.DocumentsTimeHeaderCell3.StylePriority.UsePadding = false;
			this.DocumentsTimeHeaderCell3.StylePriority.UseTextAlignment = false;
			this.DocumentsTimeHeaderCell3.Text = "Absence for a valid reason";
			this.DocumentsTimeHeaderCell3.Weight = 0.23255813953488372D;
			// 
			// DocumentsTimeHeaderCell4
			// 
			this.DocumentsTimeHeaderCell4.Dpi = 254F;
			this.DocumentsTimeHeaderCell4.Name = "DocumentsTimeHeaderCell4";
			this.DocumentsTimeHeaderCell4.StylePriority.UsePadding = false;
			this.DocumentsTimeHeaderCell4.StylePriority.UseTextAlignment = false;
			this.DocumentsTimeHeaderCell4.Text = "The absence of a valid reason for not";
			this.DocumentsTimeHeaderCell4.Weight = 0.23255813953488372D;
			// 
			// DocumentsTimeHeaderCell5
			// 
			this.DocumentsTimeHeaderCell5.Dpi = 254F;
			this.DocumentsTimeHeaderCell5.Name = "DocumentsTimeHeaderCell5";
			this.DocumentsTimeHeaderCell5.StylePriority.UsePadding = false;
			this.DocumentsTimeHeaderCell5.StylePriority.UseTextAlignment = false;
			this.DocumentsTimeHeaderCell5.Text = "overtime";
			this.DocumentsTimeHeaderCell5.Weight = 0.23255813953488372D;
			// 
			// TotalBalanceHeaderCell2
			// 
			this.TotalBalanceHeaderCell2.Dpi = 254F;
			this.TotalBalanceHeaderCell2.Name = "TotalBalanceHeaderCell2";
			this.TotalBalanceHeaderCell2.Text = "TotalBalanceHeaderCell2";
			this.TotalBalanceHeaderCell2.Weight = 0.31007751937984496D;
			// 
			// DepartmentHeaderCell9
			// 
			this.DepartmentHeaderCell9.Name = "DepartmentHeaderCell9";
			this.DepartmentHeaderCell9.Weight = 0D;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// WorkingTimeReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
			this.Detail,
			this.GroupHeader1});
			this.DataMember = "Data";
			this.DataSourceSchema = resources.GetString("$this.DataSourceSchema");
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
			this.AbsenceRule,
			this.OvertimeRule,
			this.PresenceDocumentRule,
			this.AbsenceReasonableRule,
			this.AbsenceDocumentRule,
			this.OvertimeDocumentRule,
			this.TotalBalanceRule});
			this.Landscape = true;
			this.PageHeight = 2100;
			this.PageWidth = 2970;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
			this.xrControlStyle1});
			this.Version = "15.1";
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.HeaderTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell EmployeeValueCell;
		private DevExpress.XtraReports.UI.XRTableCell DepartmentValueCell;
		private DevExpress.XtraReports.UI.XRTableCell PositionValueCell;
		private DevExpress.XtraReports.UI.XRTableCell EmployeeHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell DepartmentHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell WorkingTimeValueCell3;
		private DevExpress.XtraReports.UI.XRTableCell WorkingTimeValueCell2;
		private DevExpress.XtraReports.UI.XRTableCell RealTotalTimeValueCell2;
		private DevExpress.XtraReports.UI.XRTableCell RealTotalTimeValueCell3;
		private DevExpress.XtraReports.UI.XRTableCell NonAcceptedAdjustmentsValueCell2;
		private DevExpress.XtraReports.UI.XRTable HeaderTable;
		private DevExpress.XtraReports.UI.XRTableRow HeaderRow1;
		private DevExpress.XtraReports.UI.XRTableCell EmployeeHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell DepartmentHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell PositionHeaderCell;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
		private DevExpress.XtraReports.UI.XRTableCell NonAcceptedAdjustmentsValueCell3;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeValueCell2;
		private DevExpress.XtraReports.UI.XRTableCell PositionHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeValueCell3;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeValueCell4;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeValueCell5;
		private DevExpress.XtraReports.UI.XRTableCell TotalBalanceValueCell;
		private DevExpress.XtraReports.UI.XRTableCell WorkingTimeHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell RealTotalTimeHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell NonAcceptedAdjustmentsHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell TotalBalanceHeaderCell;
		private DevExpress.XtraReports.UI.XRTableCell TotalBalanceHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell DepartmentHeaderCell9;
		private DevExpress.XtraReports.UI.XRTableRow HeaderRow2;
		private DevExpress.XtraReports.UI.XRTableCell RealTotalTimeHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell WorkingTimeHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell WorkingTimeHeaderCell3;
		private DevExpress.XtraReports.UI.XRTableCell RealTotalTimeHeaderCell3;
		private DevExpress.XtraReports.UI.XRTableCell NonAcceptedAdjustmentsHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell NonAcceptedAdjustmentsHeaderCell3;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeHeaderCell2;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeHeaderCell4;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeHeaderCell5;
		private FormattingRule OvertimeRule;
		private FormattingRule AbsenceRule;
		private FormattingRule PresenceDocumentRule;
		private FormattingRule AbsenceReasonableRule;
		private FormattingRule AbsenceDocumentRule;
		private FormattingRule OvertimeDocumentRule;
		private FormattingRule TotalBalanceRule;
		private DevExpress.XtraReports.UI.XRTableCell DocumentsTimeHeaderCell3;
	}
}
