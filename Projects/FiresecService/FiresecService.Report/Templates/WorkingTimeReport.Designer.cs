using DevExpress.XtraReports.UI;
using Localization.FiresecService.Report.Common;

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
            float[] columnsWidth = { 135F, 200F, 300F, 600F };

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkingTimeReport));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.EmployeeValueCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.DepartmentValueCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.PositionValueCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.EmployeeHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.WorkingTimeValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.WorkingTimeValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.RealTotalTimeValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.RealTotalTimeValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.NonAcceptedAdjustmentsValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.NonAcceptedAdjustmentsValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.DocumentsTimeValueCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.PositionHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            OvertimeRule = new FormattingRule();
            AbsenceRule = new FormattingRule();
            PresenceDocumentRule = new FormattingRule();
            AbsenceReasonableRule = new FormattingRule();
            AbsenceDocumentRule = new FormattingRule();
            OvertimeDocumentRule = new FormattingRule();
            TotalBalanceRule = new FormattingRule();
            this.DocumentsTimeValueCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.DocumentsTimeValueCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.DocumentsTimeValueCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TotalBalanceValueCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.HeaderTable = new DevExpress.XtraReports.UI.XRTable();
            this.HeaderRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.EmployeeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.DepartmentHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.DepartmentHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.PositionHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.WorkingTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.RealTotalTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.NonAcceptedAdjustmentsHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.DocumentsTimeHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.TotalBalanceHeaderCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.TotalBalanceHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.DepartmentHeaderCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.HeaderRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.RealTotalTimeHeaderCell2 = new DevExpress.XtraReports.UI.XRTableCell();
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
            this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeaderTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            //
            // Detail
            //
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
            this.Detail.Dpi = 254F;
            this.Detail.HeightF = 64F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
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
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.xrTableRow2 });
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
            this.EmployeeValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Employee") });
            this.EmployeeValueCell.Dpi = 254F;
            this.EmployeeValueCell.Name = "EmployeeValueCell";
            this.EmployeeValueCell.StylePriority.UseTextAlignment = false;
            this.EmployeeValueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.EmployeeValueCell.Weight = columnsWidth[0];
            //
            // DepartmentValueCell
            //
            this.DepartmentValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department") });
            this.DepartmentValueCell.Dpi = 254F;
            this.DepartmentValueCell.Name = "DepartmentValueCell";
            this.DepartmentValueCell.StylePriority.UseTextAlignment = false;
            this.DepartmentValueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.DepartmentValueCell.Weight = columnsWidth[0];
            //
            // PositionValueCell
            //
            this.PositionValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Position") });
            this.PositionValueCell.Dpi = 254F;
            this.PositionValueCell.Name = "PositionValueCell";
            this.PositionValueCell.StylePriority.UseTextAlignment = false;
            this.PositionValueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.PositionValueCell.Weight = columnsWidth[0];
            //
            // WorkingTimeValueCell2
            //
            this.WorkingTimeValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ScheduleDay") });
            this.WorkingTimeValueCell2.Dpi = 254F;
            this.WorkingTimeValueCell2.Name = "WorkingTimeValueCell2";
            this.WorkingTimeValueCell2.Weight = columnsWidth[1] / 2;
            //
            // WorkingTimeValueCell3
            //
            this.WorkingTimeValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ScheduleNight") });
            this.WorkingTimeValueCell3.Dpi = 254F;
            this.WorkingTimeValueCell3.Name = "WorkingTimeValueCell3";
            this.WorkingTimeValueCell3.Weight = columnsWidth[1] / 2;
            //
            // RealTotalTimeValueCell2
            //
            this.RealTotalTimeValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.RealPresence") });
            this.RealTotalTimeValueCell2.Dpi = 254F;
            this.RealTotalTimeValueCell2.Name = "RealTotalTimeValueCell2";
            this.RealTotalTimeValueCell2.Weight = columnsWidth[1] / 2;
            //
            // RealTotalTimeValueCell3
            //
            this.RealTotalTimeValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.RealNightTime") });
            this.RealTotalTimeValueCell3.Dpi = 254F;
            this.RealTotalTimeValueCell3.Name = "RealTotalTimeValueCell3";
            this.RealTotalTimeValueCell3.Weight = columnsWidth[1] / 2;
            //
            // NonAcceptedAdjustmentsValueCell2
            //
            this.NonAcceptedAdjustmentsValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalAbsence") });
            this.NonAcceptedAdjustmentsValueCell2.Dpi = 254F;
            NonAcceptedAdjustmentsValueCell2.FormattingRules.Add(AbsenceRule);
            this.NonAcceptedAdjustmentsValueCell2.Name = "NonAcceptedAdjustmentsValueCell2";
            this.NonAcceptedAdjustmentsValueCell2.Weight = columnsWidth[2] / 2;
            //
            // AbsenceRule
            //
            this.AbsenceRule.Condition = "[TotalAbsence] > 0";
            this.AbsenceRule.Formatting.ForeColor = System.Drawing.Color.Red;
            this.AbsenceRule.Name = "TotalAbsence";
            //
            // NonAcceptedAdjustmentsValueCell3
            //
            this.NonAcceptedAdjustmentsValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalNonAcceptedOvertime") });
            this.NonAcceptedAdjustmentsValueCell3.Dpi = 254F;
            this.NonAcceptedAdjustmentsValueCell3.Name = "NonAcceptedAdjustmentsValueCell3";
            this.NonAcceptedAdjustmentsValueCell3.Weight = columnsWidth[2] / 2;
            NonAcceptedAdjustmentsValueCell3.FormattingRules.Add(OvertimeRule);
            //
            // OvertimeRule
            //
            this.OvertimeRule.Condition = "[TotalNonAcceptedOvertime] > 0";
            this.OvertimeRule.Formatting.ForeColor = System.Drawing.Color.DodgerBlue;
            this.OvertimeRule.Name = "TotalNonAcceptedOvertime";
            //
            // DocumentsTimeValueCell2
            //
            this.DocumentsTimeValueCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentPresence") });
            this.DocumentsTimeValueCell2.Dpi = 254F;
            this.DocumentsTimeValueCell2.Name = "DocumentsTimeValueCell2";
            this.DocumentsTimeValueCell2.Weight = columnsWidth[3] / 4;
            DocumentsTimeValueCell2.FormattingRules.Add(PresenceDocumentRule);
            //
            // PresenceDocumentRule
            //
            this.PresenceDocumentRule.Condition = "[DocumentPresence] > 0";
            this.PresenceDocumentRule.Formatting.ForeColor = System.Drawing.Color.LimeGreen;
            this.PresenceDocumentRule.Name = "DocumentPresence";
            //
            // DocumentsTimeValueCell3
            //
            this.DocumentsTimeValueCell3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentAbsenceReasonable") });
            this.DocumentsTimeValueCell3.Dpi = 254F;
            this.DocumentsTimeValueCell3.Name = "DocumentsTimeValueCell3";
            this.DocumentsTimeValueCell3.Weight = columnsWidth[3] / 4;
            DocumentsTimeValueCell3.FormattingRules.Add(AbsenceReasonableRule);
            //
            // AbsenceReasonableRule
            //
            this.AbsenceReasonableRule.Condition = "[DocumentAbsenceReasonable] > 0";
            this.AbsenceReasonableRule.Formatting.ForeColor = System.Drawing.Color.LimeGreen;
            this.AbsenceReasonableRule.Name = "DocumentAbsenceReasonable";
            //
            // DocumentsTimeValueCell4
            //
            this.DocumentsTimeValueCell4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentAbsence") });
            this.DocumentsTimeValueCell4.Dpi = 254F;
            this.DocumentsTimeValueCell4.Name = "DocumentsTimeValueCell4";
            this.DocumentsTimeValueCell4.Weight = columnsWidth[3] / 4;
            DocumentsTimeValueCell4.FormattingRules.Add(AbsenceDocumentRule);
            //
            // AbsenceDocumentRule
            //
            this.AbsenceDocumentRule.Condition = "[DocumentAbsence] > 0";
            this.AbsenceDocumentRule.Formatting.ForeColor = System.Drawing.Color.Red;
            this.AbsenceDocumentRule.Name = "DocumentAbsence";
            //
            // DocumentsTimeValueCell5
            //
            this.DocumentsTimeValueCell5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.DocumentOvertime") });
            this.DocumentsTimeValueCell5.Dpi = 254F;
            this.DocumentsTimeValueCell5.Name = "DocumentsTimeValueCell5";
            this.DocumentsTimeValueCell5.Weight = columnsWidth[3] / 4;
            DocumentsTimeValueCell5.FormattingRules.Add(OvertimeDocumentRule);
            //
            // OvertimeDocumentRule
            //
            this.OvertimeDocumentRule.Condition = "[DocumentOvertime] > 0";
            this.OvertimeDocumentRule.Formatting.ForeColor = System.Drawing.Color.DodgerBlue;
            this.OvertimeDocumentRule.Name = "DocumentOvertime";
            //
            // TotalBalanceValueCell
            //
            this.TotalBalanceValueCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalBalance") });
            this.TotalBalanceValueCell.Dpi = 254F;
            this.TotalBalanceValueCell.Name = "TotalBalanceValueCell";
            this.TotalBalanceValueCell.Weight = columnsWidth[1];
            TotalBalanceValueCell.FormattingRules.Add(TotalBalanceRule);
            //
            // TotalBalanceRule
            //
            this.TotalBalanceRule.Condition = "[TotalBalance] < 0";
            this.TotalBalanceRule.Formatting.ForeColor = System.Drawing.Color.Red;
            this.TotalBalanceRule.Name = "TotalBalance";
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
            this.HeaderRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[]
            {
                this.EmployeeHeaderCell,
                this.DepartmentHeaderCell,
                this.PositionHeaderCell,
                this.WorkingTimeHeaderCell,
                this.RealTotalTimeHeaderCell,
                this.NonAcceptedAdjustmentsHeaderCell,
                this.DocumentsTimeHeaderCell,
                this.TotalBalanceHeaderCell
            });
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
            this.EmployeeHeaderCell.Text = CommonResources.Employee;
            EmployeeHeaderCell.WidthF = columnsWidth[0];
            //
            // DepartmentHeaderCell
            //
            this.DepartmentHeaderCell.Dpi = 254F;
            this.DepartmentHeaderCell.Name = "DepartmentHeaderCell";
            this.DepartmentHeaderCell.RowSpan = 2;
            this.DepartmentHeaderCell.StylePriority.UsePadding = false;
            this.DepartmentHeaderCell.StylePriority.UseTextAlignment = false;
            this.DepartmentHeaderCell.Text = CommonResources.Department;
            DepartmentHeaderCell.WidthF = columnsWidth[0];
            //
            // PositionHeaderCell
            //
            this.PositionHeaderCell.Dpi = 254F;
            this.PositionHeaderCell.Name = "PositionHeaderCell";
            this.PositionHeaderCell.RowSpan = 2;
            this.PositionHeaderCell.StylePriority.UsePadding = false;
            this.PositionHeaderCell.StylePriority.UseTextAlignment = false;
            this.PositionHeaderCell.Text = CommonResources.Position;
            PositionHeaderCell.WidthF = columnsWidth[0];
            //
            // WorkingTimeHeaderCell
            //
            this.WorkingTimeHeaderCell.Dpi = 254F;
            this.WorkingTimeHeaderCell.Name = "WorkingTimeHeaderCell";
            this.WorkingTimeHeaderCell.StylePriority.UsePadding = false;
            this.WorkingTimeHeaderCell.StylePriority.UseTextAlignment = false;
            this.WorkingTimeHeaderCell.Text = CommonResources.WorkTimeSchedule;
            this.WorkingTimeHeaderCell.WidthF = columnsWidth[1];
            //
            // RealTotalTimeHeaderCell
            //
            this.RealTotalTimeHeaderCell.Dpi = 254F;
            this.RealTotalTimeHeaderCell.Name = "RealTotalTimeHeaderCell";
            this.RealTotalTimeHeaderCell.StylePriority.UsePadding = false;
            this.RealTotalTimeHeaderCell.StylePriority.UseTextAlignment = false;
            this.RealTotalTimeHeaderCell.Text = CommonResources.FactWorkTime;
            this.RealTotalTimeHeaderCell.WidthF = columnsWidth[1];
            //
            // NonAcceptedAdjustmentsHeaderCell
            //
            this.NonAcceptedAdjustmentsHeaderCell.Dpi = 254F;
            this.NonAcceptedAdjustmentsHeaderCell.Name = "NonAcceptedAdjustmentsHeaderCell";
            this.NonAcceptedAdjustmentsHeaderCell.StylePriority.UsePadding = false;
            this.NonAcceptedAdjustmentsHeaderCell.StylePriority.UseTextAlignment = false;
            this.NonAcceptedAdjustmentsHeaderCell.Text = CommonResources.ScheduleDeviationNotConfirmed;
            this.NonAcceptedAdjustmentsHeaderCell.WidthF = columnsWidth[2];
            //
            // DocumentsTimeHeaderCell
            //
            this.DocumentsTimeHeaderCell.Dpi = 254F;
            this.DocumentsTimeHeaderCell.Name = "DocumentsTimeHeaderCell";
            this.DocumentsTimeHeaderCell.StylePriority.UsePadding = false;
            this.DocumentsTimeHeaderCell.StylePriority.UseTextAlignment = false;
            this.DocumentsTimeHeaderCell.Text = CommonResources.DocumentTime;
            this.DocumentsTimeHeaderCell.WidthF = columnsWidth[3];
            //
            // TotalBalanceHeaderCell
            //
            this.TotalBalanceHeaderCell.Dpi = 254F;
            this.TotalBalanceHeaderCell.Name = "TotalBalanceHeaderCell";
            this.TotalBalanceHeaderCell.StylePriority.UsePadding = false;
            this.TotalBalanceHeaderCell.StylePriority.UseTextAlignment = false;
            this.TotalBalanceHeaderCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] { new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.TotalBalanceHeaderName") });
            TotalBalanceHeaderCell.RowSpan = 2;
            this.TotalBalanceHeaderCell.WidthF = columnsWidth[1];
            //
            // HeaderRow2
            //
            this.HeaderRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[]
            {
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
                this.TotalBalanceHeaderCell2
            });
            //
            // EmployeeHeaderCell2
            //
            this.EmployeeHeaderCell2.Dpi = 254F;
            this.EmployeeHeaderCell2.Name = "EmployeeHeaderCell2";
            this.EmployeeHeaderCell2.Text = "EmployeeHeaderCell2";
            this.EmployeeHeaderCell2.WidthF = columnsWidth[0];
            //
            // DepartmentHeaderCell2
            //
            this.DepartmentHeaderCell2.Dpi = 254F;
            this.DepartmentHeaderCell2.Name = "DepartmentHeaderCell2";
            this.DepartmentHeaderCell2.Text = "DepartmentHeaderCell2";
            this.DepartmentHeaderCell2.WidthF = columnsWidth[0];
            //
            // PositionHeaderCell2
            //
            this.PositionHeaderCell2.Dpi = 254F;
            this.PositionHeaderCell2.Name = "PositionHeaderCell2";
            this.PositionHeaderCell2.Text = "PositionHeaderCell2";
            this.PositionHeaderCell2.WidthF = columnsWidth[0];
            //
            // WorkingTimeHeaderCell2
            //
            this.WorkingTimeHeaderCell2.Dpi = 254F;
            this.WorkingTimeHeaderCell2.Name = "WorkingTimeHeaderCell2";
            this.WorkingTimeHeaderCell2.StylePriority.UsePadding = false;
            this.WorkingTimeHeaderCell2.StylePriority.UseTextAlignment = false;
            this.WorkingTimeHeaderCell2.Text = CommonResources.Tottally;
            this.WorkingTimeHeaderCell2.WidthF = columnsWidth[1] / 2;
            //
            // WorkingTimeHeaderCell3
            //
            this.WorkingTimeHeaderCell3.Dpi = 254F;
            this.WorkingTimeHeaderCell3.Name = "WorkingTimeHeaderCell3";
            this.WorkingTimeHeaderCell3.StylePriority.UsePadding = false;
            this.WorkingTimeHeaderCell3.StylePriority.UseTextAlignment = false;
            this.WorkingTimeHeaderCell3.Text = CommonResources.IncludeNightTime;
            this.WorkingTimeHeaderCell3.WidthF = columnsWidth[1] / 2;
            //
            // RealTotalTimeHeaderCell2
            //
            this.RealTotalTimeHeaderCell2.Dpi = 254F;
            this.RealTotalTimeHeaderCell2.Name = "RealTotalTimeHeaderCell2";
            this.RealTotalTimeHeaderCell2.StylePriority.UsePadding = false;
            this.RealTotalTimeHeaderCell2.StylePriority.UseTextAlignment = false;
            this.RealTotalTimeHeaderCell2.Text = CommonResources.Tottally;
            this.RealTotalTimeHeaderCell2.WidthF = columnsWidth[1] / 2;
            //
            // RealTotalTimeHeaderCell3
            //
            this.RealTotalTimeHeaderCell3.Dpi = 254F;
            this.RealTotalTimeHeaderCell3.Name = "RealTotalTimeHeaderCell3";
            this.RealTotalTimeHeaderCell3.StylePriority.UsePadding = false;
            this.RealTotalTimeHeaderCell3.StylePriority.UseTextAlignment = false;
            this.RealTotalTimeHeaderCell3.Text = CommonResources.IncludeNightWork;
            this.RealTotalTimeHeaderCell3.WidthF = columnsWidth[1] / 2;
            //
            // NonAcceptedAdjustmentsHeaderCell2
            //
            this.NonAcceptedAdjustmentsHeaderCell2.Dpi = 254F;
            this.NonAcceptedAdjustmentsHeaderCell2.Name = "NonAcceptedAdjustmentsHeaderCell2";
            this.NonAcceptedAdjustmentsHeaderCell2.StylePriority.UsePadding = false;
            this.NonAcceptedAdjustmentsHeaderCell2.StylePriority.UseTextAlignment = false;
            this.NonAcceptedAdjustmentsHeaderCell2.Text = CommonResources.AbsenceInclEarlyLeaveLate;
            this.NonAcceptedAdjustmentsHeaderCell2.WidthF = columnsWidth[2] / 2;
            //
            // NonAcceptedAdjustmentsHeaderCell3
            //
            this.NonAcceptedAdjustmentsHeaderCell3.Dpi = 254F;
            this.NonAcceptedAdjustmentsHeaderCell3.Name = "NonAcceptedAdjustmentsHeaderCell3";
            this.NonAcceptedAdjustmentsHeaderCell3.StylePriority.UsePadding = false;
            this.NonAcceptedAdjustmentsHeaderCell3.StylePriority.UseTextAlignment = false;
            this.NonAcceptedAdjustmentsHeaderCell3.Text = CommonResources.TimeoverOutSchedule;
            this.NonAcceptedAdjustmentsHeaderCell3.WidthF = columnsWidth[2] / 2;
            //
            // DocumentsTimeHeaderCell2
            //
            this.DocumentsTimeHeaderCell2.Dpi = 254F;
            this.DocumentsTimeHeaderCell2.Name = "DocumentsTimeHeaderCell2";
            this.DocumentsTimeHeaderCell2.StylePriority.UsePadding = false;
            this.DocumentsTimeHeaderCell2.StylePriority.UseTextAlignment = false;
            this.DocumentsTimeHeaderCell2.Text = CommonResources.Presence;
            this.DocumentsTimeHeaderCell2.WidthF = columnsWidth[3] / 4;
            //
            // DocumentsTimeHeaderCell3
            //
            this.DocumentsTimeHeaderCell3.Dpi = 254F;
            this.DocumentsTimeHeaderCell3.Name = "DocumentsTimeHeaderCell3";
            this.DocumentsTimeHeaderCell3.StylePriority.UsePadding = false;
            this.DocumentsTimeHeaderCell3.StylePriority.UseTextAlignment = false;
            this.DocumentsTimeHeaderCell3.Text = CommonResources.ReasonAbsence;
            this.DocumentsTimeHeaderCell3.WidthF = columnsWidth[3] / 4;
            //
            // DocumentsTimeHeaderCell4
            //
            this.DocumentsTimeHeaderCell4.Dpi = 254F;
            this.DocumentsTimeHeaderCell4.Name = "DocumentsTimeHeaderCell4";
            this.DocumentsTimeHeaderCell4.StylePriority.UsePadding = false;
            this.DocumentsTimeHeaderCell4.StylePriority.UseTextAlignment = false;
            this.DocumentsTimeHeaderCell4.Text = CommonResources.NotReasonAbsence;
            this.DocumentsTimeHeaderCell4.WidthF = columnsWidth[3] / 4;
            //
            // DocumentsTimeHeaderCell5
            //
            this.DocumentsTimeHeaderCell5.Dpi = 254F;
            this.DocumentsTimeHeaderCell5.Name = "DocumentsTimeHeaderCell5";
            this.DocumentsTimeHeaderCell5.StylePriority.UsePadding = false;
            this.DocumentsTimeHeaderCell5.StylePriority.UseTextAlignment = false;
            this.DocumentsTimeHeaderCell5.Text = CommonResources.Overtime;
            this.DocumentsTimeHeaderCell5.WidthF = columnsWidth[3] / 4;
            //
            // TotalBalanceHeaderCell2
            //
            this.TotalBalanceHeaderCell2.Dpi = 254F;
            this.TotalBalanceHeaderCell2.Name = "TotalBalanceHeaderCell2";
            this.TotalBalanceHeaderCell2.Text = "TotalBalanceHeaderCell2";
            this.TotalBalanceHeaderCell2.WidthF = columnsWidth[1];


            this.HeaderRow2.Dpi = 254F;
            this.HeaderRow2.Name = "HeaderRow2";
            this.HeaderRow2.Weight = 11.5D;
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
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[]
            {
                AbsenceRule,
                OvertimeRule,
                PresenceDocumentRule,
                AbsenceReasonableRule,
                AbsenceDocumentRule,
                AbsenceDocumentRule,
                OvertimeDocumentRule,
                TotalBalanceRule
            });
            this.Landscape = true;
            this.PageHeight = 2100;
            this.PageWidth = 2970;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
            this.Version = "14.1";
            this.Controls.SetChildIndex(this.GroupHeader1, 0);
            this.Controls.SetChildIndex(this.Detail, 0);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeaderTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
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
