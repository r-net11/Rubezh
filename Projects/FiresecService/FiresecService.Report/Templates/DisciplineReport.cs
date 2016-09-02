using Common;
using Localization.FiresecService.Report.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using System;
using System.Collections.Generic;
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
			get { return CommonResources.DisciplinaryReport; }
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
					var crossNightTimeTrackParts = new List<TimeTrackPart>();
					foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
					{

						dayTimeTrack.CrossNightTimeTrackParts = crossNightTimeTrackParts;
						dayTimeTrack.Calculate();
						crossNightTimeTrackParts = dayTimeTrack.CrossNightTimeTrackParts;


						if(filter.ShowAllViolation && dayTimeTrack.PlannedTimeTrackParts.IsEmpty()) continue;

						var dataRow = dataSet.Data.NewDataRow();
						dataRow.Employee = employee.Name;
						dataRow.Organisation = employee.Organisation;
						dataRow.Department = employee.Department;
						dataRow.Date = dayTimeTrack.Date;
						dataRow.Weekday = dayTimeTrack.Date.ToString("ddd");

						if (dayTimeTrack.RealTimeTrackParts.Any(x => x.IsForURVZone && !x.NotTakeInCalculations))
						{
							dataRow.FirstEnter = GetFirstEnterString(dayTimeTrack);
							dataRow.LastExit = GetLastExitString(dayTimeTrack);
						}

						var absence = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.Absence);
						var late = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.Late);
						var earlyLeave = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.EarlyLeave);
						var overtime = GetTimespanForTimeTrackType(dayTimeTrack.Totals, TimeTrackType.Overtime);

						if (filter.ShowAllViolation)
						{
							dataRow.Absence = GetFormattedTime(absence);
							dataRow.Late = GetFormattedTime(late);
							dataRow.EarlyLeave = GetFormattedTime(earlyLeave);
							dataRow.Overtime = GetFormattedTime(overtime);

							dataSet.Data.Rows.Add(dataRow);
						}
						else if (filter.ShowShiftedViolation)
						{
							if (filter.ShowAbsence)
								dataRow.Absence = GetFormattedTime(absence);
							if (filter.ShowLate)
								dataRow.Late = GetFormattedTime(late);
							if (filter.ShowEarlуLeave)
								dataRow.EarlyLeave = GetFormattedTime(earlyLeave);
							if (filter.ShowOvertime)
								dataRow.Overtime = GetFormattedTime(overtime);

							if((filter.ShowAbsence && !string.IsNullOrEmpty(dataRow.Absence))
								|| (filter.ShowLate && !string.IsNullOrEmpty(dataRow.Late))
								|| (filter.ShowEarlуLeave && !string.IsNullOrEmpty(dataRow.EarlyLeave))
								|| (filter.ShowOvertime && !string.IsNullOrEmpty(dataRow.Overtime)))
								dataSet.Data.Rows.Add(dataRow);
						}
					}
				}
			}

			return dataSet;
		}

		private string GetFormattedTime(TimeSpan time)
		{
			return time == TimeSpan.Zero ? string.Empty : string.Format("{0:00}:{1:00}", (int) time.TotalHours, time.Minutes);
		}

		private static string GetFirstEnterString(DayTimeTrack dayTimeTrack)
		{
			var resultDateTime = dayTimeTrack.RealTimeTrackParts
				.Where(x => x.IsForURVZone)
				.Select(x => x.EnterDateTime)
				.DefaultIfEmpty()
				.Min();

			if(resultDateTime == default(DateTime)) return string.Empty;

			return resultDateTime.Date < dayTimeTrack.Date
				? CommonResources.PreviouslyDay
				: resultDateTime.TimeOfDay.ToString(@"hh\:mm\:ss");
		}

		private static string GetLastExitString(DayTimeTrack dayTimeTrack)
		{
			var resultDateTime = dayTimeTrack.RealTimeTrackParts
				.Where(x => x.IsForURVZone)
				.Select(x => x.ExitDateTime)
				.DefaultIfEmpty()
				.Max();

			if (resultDateTime == default(DateTime?)) return string.Empty;

			return resultDateTime.Value.Date > dayTimeTrack.Date
				? CommonResources.NextDay
				: resultDateTime.Value.TimeOfDay.ToString(@"hh\:mm\:ss");
		}

		private static TimeSpan GetTimespanForTimeTrackType(IEnumerable<TimeTrackTotal> totals, TimeTrackType type)
		{
			var result = totals.FirstOrDefault(x => x.TimeTrackType == type);
			return result != null ? result.TimeSpan : TimeSpan.Zero;
		}
	}
}