using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using DevExpress.Xpo.Helpers;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class DisciplineReport : BaseReport
	{
		public DisciplineReport()
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
			get { return "Дисциплинарный отчет"; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DisciplineReportFilter>();
			var employeeFilter = dataProvider.GetEmployeeFilter(filter);
			var employees = dataProvider.GetEmployees(employeeFilter, filter.IsDefault);
			var timeTrackResult = dataProvider.DatabaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo).Result;

			var dataSet = new DisciplineDataSet();
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Any())
				{
					if (employee.Item.Schedule == null || !filter.ScheduleSchemas.Contains(employee.Item.Schedule.UID))
						continue;
				}
				var timeTrackEmployeeResult = timeTrackResult.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
				if (timeTrackEmployeeResult != null)
				{
					foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
					{
						dayTimeTrack.Calculate();

						var dataRow = dataSet.Data.NewDataRow();
						dataRow.Employee = employee.Name;
						dataRow.Organisation = employee.Organisation;
						dataRow.Department = employee.Department;
						dataRow.Date = dayTimeTrack.Date;
						dataRow.Weekday = dayTimeTrack.Date.ToString("ddd");

						if (dayTimeTrack.RealTimeTrackParts.Any(x => x.IsForURVZone))
						{
							dataRow.FirstEnter = GetFirstEnterString(dayTimeTrack);
							dataRow.LastExit = GetLastExitString(dayTimeTrack);
						}

						var absence = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.Absence);
						var late = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.Late);
						var earlyLeave = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.EarlyLeave);
						var overtime = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.Overtime);

						var isShowAbsence = absence.TotalSeconds > default(int) && filter.ShowAbsence;
						var isShowLate = late.TotalSeconds > default(int) && filter.ShowLate;
						var isShowEarlуLeave = earlyLeave.TotalSeconds > default(int) && filter.ShowEarlуLeave;
						var isShowOvertime = overtime.TotalSeconds > default(int) && filter.ShowOvertime && filter.ShowShiftedViolation;

						if (isShowAbsence || isShowLate || isShowEarlуLeave || isShowOvertime || filter.ShowAllViolation)
						{
							dataRow.Absence = absence == TimeSpan.Zero ? string.Empty : absence.ToString(@"hh\:mm\:ss");
							dataRow.Late = late == TimeSpan.Zero ? string.Empty : late.ToString(@"hh\:mm\:ss");
							dataRow.EarlyLeave = earlyLeave == TimeSpan.Zero ? string.Empty : earlyLeave.ToString(@"hh\:mm\:ss");
							dataRow.Overtime = overtime == TimeSpan.Zero ? string.Empty : overtime.ToString(@"hh\:mm\:ss");
							dataSet.Data.Rows.Add(dataRow);
						}
					}
				}
			}

			return dataSet;
		}

		private string GetFirstEnterString(DayTimeTrack dayTimeTrack)
		{
			var resultDateTime = dayTimeTrack.RealTimeTrackParts
				.Where(x => x.IsForURVZone)
				.Select(x => x.EnterDateTime)
				.DefaultIfEmpty()
				.Min();

			if (resultDateTime == default(DateTime)) return string.Empty;

			return resultDateTime.Date < dayTimeTrack.Date
				? "Пред. день"
				: resultDateTime.TimeOfDay.ToString(@"hh\:mm\:ss");
		}

		private string GetLastExitString(DayTimeTrack dayTimeTrack)
		{
			var resultDateTime = dayTimeTrack.RealTimeTrackParts
				.Where(x => x.IsForURVZone)
				.Select(x => x.ExitDateTime)
				.DefaultIfEmpty()
				.Max();

			if (resultDateTime == default(DateTime?)) return string.Empty;

			return resultDateTime.Value.Date > dayTimeTrack.Date
				? "След. день"
				: resultDateTime.Value.TimeOfDay.ToString(@"hh\:mm\:ss");
		}

		private TimeSpan GetTimespanForTimeTrackType(IEnumerable<TimeTrackTotal> totals, TimeTrackType type)
		{
			var result = totals.FirstOrDefault(x => x.TimeTrackType == type);
			return result != null ? result.TimeSpan : TimeSpan.Zero;
		}
	}
}