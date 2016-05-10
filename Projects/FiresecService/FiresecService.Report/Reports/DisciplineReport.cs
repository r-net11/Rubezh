using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class DisciplineReport : BaseReport<List<DisciplineData>>
	{
		public override List<DisciplineData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<DisciplineReportFilter>(f);
			var employeeFilter = dataProvider.GetEmployeeFilter(filter);
			var employees = dataProvider.GetEmployees(employeeFilter, filter.IsDefault);
			var timeTrackResult = dataProvider.DbService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var result = new List<DisciplineData>();
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Item.ScheduleUID == Guid.Empty || !filter.ScheduleSchemas.Contains(employee.Item.ScheduleUID))
						continue;
				}
				var timeTrackEmployeeResult = timeTrackResult.Result.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
				if (timeTrackEmployeeResult != null)
				{
					foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
					{
						dayTimeTrack.Calculate();
						bool hasEarlyLive;
						bool hasLate;
						if (filter.ShowWithoutTolerance)
						{
							hasEarlyLive = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave).TimeSpan > TimeSpan.Zero;
							hasLate = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Late).TimeSpan > TimeSpan.Zero;
						}
						else
						{
							hasEarlyLive = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave).TimeSpan > dayTimeTrack.AllowedEarlyLeave;
							hasLate = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Late).TimeSpan > dayTimeTrack.AllowedLate;
						}
						var hasAbcence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Absence).TimeSpan > TimeSpan.Zero;
						var hasOvertime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime).TimeSpan > TimeSpan.Zero;
						var hasDocument = dayTimeTrack.Totals.Where(x => x.TimeTrackType == TimeTrackType.DocumentPresence ||
							x.TimeTrackType == TimeTrackType.DocumentOvertime ||
							x.TimeTrackType == TimeTrackType.DocumentAbsence).Any(x => x.TimeSpan > TimeSpan.Zero);
						var isShow = filter.ShowEarlуRetirement && hasEarlyLive ||
								filter.ShowDelay && hasLate ||
								filter.ShowAbsence && hasAbcence ||
								filter.ShowOvertime && hasOvertime ||
								filter.ShowConfirmed && hasDocument;
						if (isShow)
						{
							var data = new DisciplineData();
							data.Employee = employee.Name;
							data.Organisation = employee.Organisation;
							data.Department = employee.Department;

							data.Date = dayTimeTrack.Date;

							if (dayTimeTrack.RealTimeTrackParts.Count > 0)
							{
								data.FirstEnter = dayTimeTrack.RealTimeTrackParts.Min(x => x.StartTime);
								data.LastExit = dayTimeTrack.RealTimeTrackParts.Max(x => x.EndTime);
							}

							var absence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Absence);
							if (absence != null)
							{
								data.Absence = absence.TimeSpan;
							}

							var late = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Late);
							if (late != null)
							{
								data.Late = late.TimeSpan;
							}

							var earlyLeave = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave);
							if (earlyLeave != null)
							{
								data.EarlyLeave = earlyLeave.TimeSpan;
							}

							var overtime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime);
							if (overtime != null)
							{
								data.Overtime = overtime.TimeSpan;
							}

							var isHoliday = dayTimeTrack.IsHoliday;

							var document = timeTrackEmployeeResult.Documents.FirstOrDefault(x => (dayTimeTrack.Date >= x.StartDateTime.Date && dayTimeTrack.Date <= x.EndDateTime.Date) && x.TimeTrackDocumentType.ShortName == dayTimeTrack.LetterCode);
							if (document != null)
							{
								data.DocumentDate = document.DocumentDateTime;
								data.DocumentName = document.TimeTrackDocumentType.Name;
								data.DocumentNo = document.DocumentNumber.ToString();
							}

							if (absence.TimeSpan.TotalSeconds > 0 || late.TimeSpan.TotalSeconds > 0 || earlyLeave.TimeSpan.TotalSeconds > 0 || overtime.TimeSpan.TotalSeconds > 0 || document != null)
							{
								result.Add(data);
							}
						}
					}
				}
			}
			return result;
		}
	}
}
