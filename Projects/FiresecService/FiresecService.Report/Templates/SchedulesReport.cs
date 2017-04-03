using FiresecService.Report.DataSources;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.SKD.ReportFilters;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class SchedulesReport : BaseReport
	{
		public SchedulesReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return CommonResources.WorkScheduleReport; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<SchedulesReportFilter>();
			var dataSet = new SchedulesDataSet();

			var employees = dataProvider.GetEmployees(filter).Where(x => x.Item.Schedule != null);
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Item.Schedule == null || !filter.ScheduleSchemas.Contains(employee.Item.Schedule.UID))
						continue;
				}

				var dataRow = dataSet.Data.NewDataRow();
				dataRow.Employee = employee.Name;
				dataRow.Organisation = employee.Organisation;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;

				Debug.Assert(employee.Item.Schedule != null, "employee.Item.Schedule != null");
				dataRow.Schedule = employee.Item.Schedule.Name;

				var scheduleResult = dataProvider.DatabaseService.ScheduleTranslator.GetSingle(employee.Item.Schedule.UID).Result;
				if (scheduleResult != null)
				{
					dataRow.UseHoliday = !scheduleResult.IsIgnoreHoliday;
					dataRow.FirstEnterLastExit = scheduleResult.IsOnlyFirstEnter;
					dataRow.AllowedLate = scheduleResult.AllowedLate == default(int) ? string.Empty : scheduleResult.AllowedLate.ToString();
					dataRow.AllowedEarlyLeave = scheduleResult.AllowedEarlyLeave == default(int) ? string.Empty : scheduleResult.AllowedEarlyLeave.ToString();
					dataRow.AllowedAbsence = scheduleResult.AllowedAbsentLowThan == default(int) ? string.Empty : scheduleResult.AllowedAbsentLowThan.ToString();
					dataRow.AllowedOvertime = scheduleResult.NotAllowOvertimeLowerThan == default(int) ? string.Empty : scheduleResult.NotAllowOvertimeLowerThan.ToString();

					var scheduleSchemeResult = dataProvider.DatabaseService.ScheduleSchemeTranslator.GetSingle(scheduleResult.ScheduleSchemeUID);
					if (scheduleSchemeResult.Result != null)
					{
						dataRow.BaseSchedule = scheduleSchemeResult.Result.Name;
						dataRow.ScheduleType = scheduleSchemeResult.Result.Type.ToDescription();
					}
				}

				dataSet.Data.Rows.Add(dataRow);
			}

			return dataSet;
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.xrTableCellEmployeeHeader.Text = CommonResources.Employee;
			this.xrTableCellOrganisationHeader.Text = CommonResources.Organization;
			this.xrTableCell1DepartmentHeader.Text = CommonResources.Department;
			this.xrTableCellPositionHeader.Text = CommonResources.Position;
			this.xrTableCellScheduleHeader.Text = CommonResources.WorkSchedule;
			this.xrTableCellScheduleTypeHeader.Text = CommonResources.ScheduleType;
			this.xrTableCellBaseScheduleHeader.Text = CommonResources.BasicSchedule;
			this.xrTableCellUseHolidayValueHeader.Text = CommonResources.IgnoreHolidays;
			this.xrTableCellFirstEnterLastExitValueHeader.Text = CommonResources.FirstEntrLastExt;
			this.xrTableCellAllowedLateHeader.Text = CommonResources.IgnoreAllowedLate;
			this.xrTableCellAllowedEarlyLeaveHeader.Text = CommonResources.IgnoreEarlyLeave;
			this.xrTableCellAbsenceHeader.Text = CommonResources.IgnoreAbsence;
			this.xrTableCellOvertimeHeader.Text = CommonResources.IgnoreTimeover;
			this.FirstEnterLastExitValue.Expression = string.Format(" Iif([FirstEnterLastExit] == True, {0}, {1})", CommonResources.Yes, CommonResources.No);
			this.UseHolidayValue.Expression = string.Format(" Iif([UseHoliday] != True, {0}, {1})", CommonResources.Yes, CommonResources.No);
		}
	}
}