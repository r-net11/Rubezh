using Localization.FiresecService.Report.Common;

namespace FiresecService.Report.Templates
{
	partial class SchedulesReport
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchedulesReport));
			this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCellEmployeeData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellOrganisationData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellDepartmentData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellPositionData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell1ScheduleData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellScheduleTypeData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellBaseScheduleData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellUseHolidayValueData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellFirstEnterLastExitValueData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellAllowedLateData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellAllowedEarlyLeaveData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellAbsenceData = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellOvertimeData = new DevExpress.XtraReports.UI.XRTableCell();
			this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
			this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
			this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
			this.xrTableCellEmployeeHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellOrganisationHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCell1DepartmentHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellPositionHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellScheduleHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellScheduleTypeHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellBaseScheduleHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellUseHolidayValueHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellFirstEnterLastExitValueHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellAllowedLateHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellAllowedEarlyLeaveHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellAbsenceHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrTableCellOvertimeHeader = new DevExpress.XtraReports.UI.XRTableCell();
			this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
			this.FirstEnterLastExitValue = new DevExpress.XtraReports.UI.CalculatedField();
			this.UseHolidayValue = new DevExpress.XtraReports.UI.CalculatedField();
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// Detail
			// 
			this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
			this.Detail.HeightF = 64F;
			// 
			// xrTable1
			// 
			this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable1.Dpi = 254F;
			this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable1.Name = "xrTable1";
			this.xrTable1.OddStyleName = "xrControlStyle1";
			this.xrTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
			this.xrTable1.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			this.xrTable1.StylePriority.UseBorders = false;
			this.xrTable1.StylePriority.UsePadding = false;
			this.xrTable1.StylePriority.UseTextAlignment = false;
			this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow1
			// 
			this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellEmployeeData,
            this.xrTableCellOrganisationData,
            this.xrTableCellDepartmentData,
            this.xrTableCellPositionData,
            this.xrTableCell1ScheduleData,
            this.xrTableCellScheduleTypeData,
            this.xrTableCellBaseScheduleData,
            this.xrTableCellUseHolidayValueData,
            this.xrTableCellFirstEnterLastExitValueData,
            this.xrTableCellAllowedLateData,
            this.xrTableCellAllowedEarlyLeaveData,
            this.xrTableCellAbsenceData,
            this.xrTableCellOvertimeData});
			this.xrTableRow1.Dpi = 254F;
			this.xrTableRow1.Name = "xrTableRow1";
			this.xrTableRow1.Weight = 11.5D;
			// 
			// xrTableCellEmployeeData
			// 
			this.xrTableCellEmployeeData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Employee")});
			this.xrTableCellEmployeeData.Dpi = 254F;
			this.xrTableCellEmployeeData.Name = "xrTableCellEmployeeData";
			this.xrTableCellEmployeeData.Weight = 0.076862745098039212D;
			// 
			// xrTableCellOrganisationData
			// 
			this.xrTableCellOrganisationData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Organisation")});
			this.xrTableCellOrganisationData.Dpi = 254F;
			this.xrTableCellOrganisationData.Name = "xrTableCellOrganisationData";
			this.xrTableCellOrganisationData.Weight = 0.094117647058823528D;
			// 
			// xrTableCellDepartmentData
			// 
			this.xrTableCellDepartmentData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Department")});
			this.xrTableCellDepartmentData.Dpi = 254F;
			this.xrTableCellDepartmentData.Name = "xrTableCellDepartmentData";
			this.xrTableCellDepartmentData.StylePriority.UseTextAlignment = false;
			this.xrTableCellDepartmentData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			this.xrTableCellDepartmentData.Weight = 0.094117647058823528D;
			// 
			// xrTableCellPositionData
			// 
			this.xrTableCellPositionData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Position")});
			this.xrTableCellPositionData.Dpi = 254F;
			this.xrTableCellPositionData.Name = "xrTableCellPositionData";
			this.xrTableCellPositionData.Weight = 0.076862745098039212D;
			// 
			// xrTableCell1ScheduleData
			// 
			this.xrTableCell1ScheduleData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.Schedule")});
			this.xrTableCell1ScheduleData.Dpi = 254F;
			this.xrTableCell1ScheduleData.Name = "xrTableCell1ScheduleData";
			this.xrTableCell1ScheduleData.Weight = 0.076862745098039212D;
			// 
			// xrTableCellScheduleTypeData
			// 
			this.xrTableCellScheduleTypeData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.ScheduleType")});
			this.xrTableCellScheduleTypeData.Dpi = 254F;
			this.xrTableCellScheduleTypeData.Name = "xrTableCellScheduleTypeData";
			this.xrTableCellScheduleTypeData.Weight = 0.082352941176470587D;
			// 
			// xrTableCellBaseScheduleData
			// 
			this.xrTableCellBaseScheduleData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.BaseSchedule")});
			this.xrTableCellBaseScheduleData.Dpi = 254F;
			this.xrTableCellBaseScheduleData.Name = "xrTableCellBaseScheduleData";
			this.xrTableCellBaseScheduleData.Weight = 0.076862745098039212D;
			// 
			// xrTableCellUseHolidayValueData
			// 
			this.xrTableCellUseHolidayValueData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.UseHolidayValue")});
			this.xrTableCellUseHolidayValueData.Dpi = 254F;
			this.xrTableCellUseHolidayValueData.Name = "xrTableCellUseHolidayValueData";
			this.xrTableCellUseHolidayValueData.StylePriority.UseTextAlignment = false;
			this.xrTableCellUseHolidayValueData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellUseHolidayValueData.Weight = 0.058823529411764705D;
			// 
			// xrTableCellFirstEnterLastExitValueData
			// 
			this.xrTableCellFirstEnterLastExitValueData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.FirstEnterLastExitValue")});
			this.xrTableCellFirstEnterLastExitValueData.Dpi = 254F;
			this.xrTableCellFirstEnterLastExitValueData.Name = "xrTableCellFirstEnterLastExitValueData";
			this.xrTableCellFirstEnterLastExitValueData.StylePriority.UseTextAlignment = false;
			this.xrTableCellFirstEnterLastExitValueData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellFirstEnterLastExitValueData.Weight = 0.0784313725490196D;
			// 
			// xrTableCellAllowedLateData
			// 
			this.xrTableCellAllowedLateData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.AllowedLate")});
			this.xrTableCellAllowedLateData.Dpi = 254F;
			this.xrTableCellAllowedLateData.Name = "xrTableCellAllowedLateData";
			this.xrTableCellAllowedLateData.StylePriority.UseTextAlignment = false;
			this.xrTableCellAllowedLateData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellAllowedLateData.Weight = 0.076862745098039212D;
			// 
			// xrTableCellAllowedEarlyLeaveData
			// 
			this.xrTableCellAllowedEarlyLeaveData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.AllowedEarlyLeave")});
			this.xrTableCellAllowedEarlyLeaveData.Dpi = 254F;
			this.xrTableCellAllowedEarlyLeaveData.Name = "xrTableCellAllowedEarlyLeaveData";
			this.xrTableCellAllowedEarlyLeaveData.StylePriority.UseTextAlignment = false;
			this.xrTableCellAllowedEarlyLeaveData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellAllowedEarlyLeaveData.Weight = 0.076862745098039212D;
			// 
			// xrTableCellAbsenceData
			// 
			this.xrTableCellAbsenceData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.AllowedAbsence")});
			this.xrTableCellAbsenceData.Dpi = 254F;
			this.xrTableCellAbsenceData.Name = "xrTableCellAbsenceData";
			this.xrTableCellAbsenceData.StylePriority.UseTextAlignment = false;
			this.xrTableCellAbsenceData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellAbsenceData.Weight = 0.076862745098039212D;
			// 
			// xrTableCellOvertimeData
			// 
			this.xrTableCellOvertimeData.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Data.AllowedOvertime")});
			this.xrTableCellOvertimeData.Dpi = 254F;
			this.xrTableCellOvertimeData.Name = "xrTableCellOvertimeData";
			this.xrTableCellOvertimeData.StylePriority.UseTextAlignment = false;
			this.xrTableCellOvertimeData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableCellOvertimeData.Weight = 0.076862745098039212D;
			// 
			// GroupHeader1
			// 
			this.GroupHeader1.BackColor = System.Drawing.Color.LightGray;
			this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
			this.GroupHeader1.Dpi = 254F;
			this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
			this.GroupHeader1.HeightF = 64F;
			this.GroupHeader1.KeepTogether = true;
			this.GroupHeader1.Name = "GroupHeader1";
			this.GroupHeader1.RepeatEveryPage = true;
			this.GroupHeader1.StylePriority.UseBackColor = false;
			// 
			// xrTable2
			// 
			this.xrTable2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
			this.xrTable2.Dpi = 254F;
			this.xrTable2.KeepTogether = true;
			this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
			this.xrTable2.Name = "xrTable2";
			this.xrTable2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
			this.xrTable2.SizeF = new System.Drawing.SizeF(2550F, 63.5F);
			this.xrTable2.StylePriority.UseBackColor = false;
			this.xrTable2.StylePriority.UseBorders = false;
			this.xrTable2.StylePriority.UsePadding = false;
			this.xrTable2.StylePriority.UseTextAlignment = false;
			this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
			// 
			// xrTableRow2
			// 
			this.xrTableRow2.BackColor = System.Drawing.Color.DarkGray;
			this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellEmployeeHeader,
            this.xrTableCellOrganisationHeader,
            this.xrTableCell1DepartmentHeader,
            this.xrTableCellPositionHeader,
            this.xrTableCellScheduleHeader,
            this.xrTableCellScheduleTypeHeader,
            this.xrTableCellBaseScheduleHeader,
            this.xrTableCellUseHolidayValueHeader,
            this.xrTableCellFirstEnterLastExitValueHeader,
            this.xrTableCellAllowedLateHeader,
            this.xrTableCellAllowedEarlyLeaveHeader,
            this.xrTableCellAbsenceHeader,
            this.xrTableCellOvertimeHeader});
			this.xrTableRow2.Dpi = 254F;
			this.xrTableRow2.Name = "xrTableRow2";
			this.xrTableRow2.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 254F);
			this.xrTableRow2.StylePriority.UsePadding = false;
			this.xrTableRow2.StylePriority.UseTextAlignment = false;
			this.xrTableRow2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
			this.xrTableRow2.Weight = 11.5D;
			// 
			// xrTableCellEmployeeHeader
			// 
			this.xrTableCellEmployeeHeader.Dpi = 254F;
			this.xrTableCellEmployeeHeader.Name = "xrTableCellEmployeeHeader";
			this.xrTableCellEmployeeHeader.Text = "employee";
			this.xrTableCellEmployeeHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellOrganisationHeader
			// 
			this.xrTableCellOrganisationHeader.Dpi = 254F;
			this.xrTableCellOrganisationHeader.Name = "xrTableCellOrganisationHeader";
			this.xrTableCellOrganisationHeader.Text = "Organization";
			this.xrTableCellOrganisationHeader.Weight = 0.094117647058823528D;
			// 
			// xrTableCell1DepartmentHeader
			// 
			this.xrTableCell1DepartmentHeader.Dpi = 254F;
			this.xrTableCell1DepartmentHeader.Name = "xrTableCell1DepartmentHeader";
			this.xrTableCell1DepartmentHeader.Text = "Subdivision";
			this.xrTableCell1DepartmentHeader.Weight = 0.094117647058823528D;
			// 
			// xrTableCellPositionHeader
			// 
			this.xrTableCellPositionHeader.Dpi = 254F;
			this.xrTableCellPositionHeader.Name = "xrTableCellPositionHeader";
			this.xrTableCellPositionHeader.Text = "Position";
			this.xrTableCellPositionHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellScheduleHeader
			// 
			this.xrTableCellScheduleHeader.Dpi = 254F;
			this.xrTableCellScheduleHeader.Name = "xrTableCellScheduleHeader";
			this.xrTableCellScheduleHeader.Text = "Schedule";
			this.xrTableCellScheduleHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellScheduleTypeHeader
			// 
			this.xrTableCellScheduleTypeHeader.Dpi = 254F;
			this.xrTableCellScheduleTypeHeader.Name = "xrTableCellScheduleTypeHeader";
			this.xrTableCellScheduleTypeHeader.Text = "Type of schedule";
			this.xrTableCellScheduleTypeHeader.Weight = 0.082352941176470587D;
			// 
			// xrTableCellBaseScheduleHeader
			// 
			this.xrTableCellBaseScheduleHeader.Dpi = 254F;
			this.xrTableCellBaseScheduleHeader.Name = "xrTableCellBaseScheduleHeader";
			this.xrTableCellBaseScheduleHeader.Text = "Basic schedule";
			this.xrTableCellBaseScheduleHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellUseHolidayValueHeader
			// 
			this.xrTableCellUseHolidayValueHeader.Dpi = 254F;
			this.xrTableCellUseHolidayValueHeader.Name = "xrTableCellUseHolidayValueHeader";
			this.xrTableCellUseHolidayValueHeader.Text = "It does not teach. idle.";
			this.xrTableCellUseHolidayValueHeader.Weight = 0.058823529411764705D;
			// 
			// xrTableCellFirstEnterLastExitValueHeader
			// 
			this.xrTableCellFirstEnterLastExitValueHeader.Dpi = 254F;
			this.xrTableCellFirstEnterLastExitValueHeader.Name = "xrTableCellFirstEnterLastExitValueHeader";
			this.xrTableCellFirstEnterLastExitValueHeader.Text = "Teaches. only the first entry and the last exit";
			this.xrTableCellFirstEnterLastExitValueHeader.Weight = 0.0784313725490196D;
			// 
			// xrTableCellAllowedLateHeader
			// 
			this.xrTableCellAllowedLateHeader.Dpi = 254F;
			this.xrTableCellAllowedLateHeader.Name = "xrTableCellAllowedLateHeader";
			this.xrTableCellAllowedLateHeader.Text = "It does not teach. delay less";
			this.xrTableCellAllowedLateHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellAllowedEarlyLeaveHeader
			// 
			this.xrTableCellAllowedEarlyLeaveHeader.Dpi = 254F;
			this.xrTableCellAllowedEarlyLeaveHeader.Name = "xrTableCellAllowedEarlyLeaveHeader";
			this.xrTableCellAllowedEarlyLeaveHeader.Text = "It does not teach. Early care less";
			this.xrTableCellAllowedEarlyLeaveHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellAbsenceHeader
			// 
			this.xrTableCellAbsenceHeader.Dpi = 254F;
			this.xrTableCellAbsenceHeader.Name = "xrTableCellAbsenceHeader";
			this.xrTableCellAbsenceHeader.Text = "It does not teach. no less than";
			this.xrTableCellAbsenceHeader.Weight = 0.076862745098039212D;
			// 
			// xrTableCellOvertimeHeader
			// 
			this.xrTableCellOvertimeHeader.Dpi = 254F;
			this.xrTableCellOvertimeHeader.Name = "xrTableCellOvertimeHeader";
			this.xrTableCellOvertimeHeader.Text = "It does not teach. processing less";
			this.xrTableCellOvertimeHeader.Weight = 0.076862745098039212D;
			// 
			// xrControlStyle1
			// 
			this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
			this.xrControlStyle1.Name = "xrControlStyle1";
			this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
			// 
			// FirstEnterLastExitValue
			// 
			this.FirstEnterLastExitValue.DataMember = "Data";
			this.FirstEnterLastExitValue.Expression = " Iif([FirstEnterLastExit] == True, \'Yes\', \'No\')";
			this.FirstEnterLastExitValue.Name = "FirstEnterLastExitValue";
			// 
			// UseHolidayValue
			// 
			this.UseHolidayValue.DataMember = "Data";
			this.UseHolidayValue.Expression = " Iif([UseHoliday] != True, \'Yes\', \'No\')";
			this.UseHolidayValue.Name = "UseHolidayValue";
			// 
			// SchedulesReport
			// 
			this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.GroupHeader1});
			this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.FirstEnterLastExitValue,
            this.UseHolidayValue});
			this.DataMember = "Data";
			this.DataSourceSchema = resources.GetString("$this.DataSourceSchema");
			this.ExportOptions.PrintPreview.ActionAfterExport = DevExpress.XtraPrinting.ActionAfterExport.Open;
			this.ExportOptions.PrintPreview.ShowOptionsBeforeExport = false;
			this.Landscape = true;
			this.PageHeight = 2100;
			this.PageWidth = 2970;
			this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
			this.Version = "15.1";
			this.Controls.SetChildIndex(this.GroupHeader1, 0);
			this.Controls.SetChildIndex(this.Detail, 0);
			((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
		private DevExpress.XtraReports.UI.XRTable xrTable1;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellEmployeeData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellOrganisationData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellDepartmentData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellPositionData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellAllowedEarlyLeaveData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellAbsenceData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellOvertimeData;
		private DevExpress.XtraReports.UI.XRTable xrTable2;
		private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellEmployeeHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellOrganisationHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1DepartmentHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellPositionHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellAllowedEarlyLeaveHeader;
		private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCell1ScheduleData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellScheduleTypeData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellBaseScheduleData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellUseHolidayValueData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellFirstEnterLastExitValueData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellAllowedLateData;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellScheduleHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellScheduleTypeHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellBaseScheduleHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellUseHolidayValueHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellFirstEnterLastExitValueHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellAllowedLateHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellAbsenceHeader;
		private DevExpress.XtraReports.UI.XRTableCell xrTableCellOvertimeHeader;
		private DevExpress.XtraReports.UI.CalculatedField FirstEnterLastExitValue;
		private DevExpress.XtraReports.UI.CalculatedField UseHolidayValue;
	}
}
