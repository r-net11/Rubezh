using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class WorkingTimeReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<WorkingTimeReportFilter>(f);

			var employeeFilter = dataProvider.GetEmployeeFilter(filter);
			var employees = dataProvider.GetEmployees(employeeFilter, filter.IsDefault);
			if (filter.IsSearch)
			{
				employeeFilter.FirstName = filter.FirstName;
				employeeFilter.SecondName = filter.SecondName;
				employeeFilter.LastName = filter.LastName;
			}
			var timeTrackResult = dataProvider.DbService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new WorkingTimeDataSet();
			foreach (var employee in employees)
			{
				var dataRow = dataSet.Data.NewDataRow();

				dataRow.Employee = employee.Name;
				dataRow.Organisation = employee.Organisation;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;

				var timeTrackEmployeeResult = timeTrackResult.Result.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
				if (timeTrackEmployeeResult != null)
				{
					var totalSchedule = new TimeSpan();
					var totalScheduleNight = new TimeSpan();
					var totalPresence = new TimeSpan();
					var totalOvertime = new TimeSpan();
					var totalNight = new TimeSpan();
					var totalDocumentOvertime = new TimeSpan();
					var totalDocumentAbsence = new TimeSpan();
					var totalBalance = new TimeSpan();
					foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
					{
						dayTimeTrack.Calculate();
						var nightSettings = dayTimeTrack.NightSettings;
						var plannedTimeTrackParts = dayTimeTrack.PlannedTimeTrackParts;

						if (nightSettings != null && nightSettings.NightEndTime != nightSettings.NightStartTime)
						{
							if (nightSettings.NightEndTime > nightSettings.NightStartTime)
							{
								totalScheduleNight += CalculateEveningTime(nightSettings.NightStartTime, nightSettings.NightEndTime, plannedTimeTrackParts);
							}
							else
							{
								totalScheduleNight += CalculateEveningTime(nightSettings.NightStartTime, new TimeSpan(23, 59, 59), plannedTimeTrackParts) + CalculateEveningTime(new TimeSpan(0, 0, 0), nightSettings.NightEndTime, plannedTimeTrackParts);
							}
						}
						plannedTimeTrackParts.ForEach(x => totalSchedule += x.Delta);
						var presence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Presence);
						if (presence != null)
						{
							totalPresence += presence.TimeSpan;
						}

						var overtime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime);
						if (overtime != null)
						{
							totalOvertime += overtime.TimeSpan;
						}

						var night = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Night);
						if (night != null)
						{
							totalNight += night.TimeSpan;
						}

						var documentOvertime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentOvertime);
						if (documentOvertime != null)
						{
							totalDocumentOvertime += documentOvertime.TimeSpan;
						}

						var documentAbsence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentAbsence);
						if (documentAbsence != null)
						{
							totalDocumentAbsence += documentAbsence.TimeSpan;
						}
						var balance = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Balance);
						if (balance != null)
						{
							totalBalance += balance.TimeSpan;
						}
					}
					dataRow.ScheduleDay = (totalSchedule - totalScheduleNight).TotalHours;
					dataRow.ScheduleNight = totalScheduleNight.TotalHours;
					dataRow.Presence = totalPresence.TotalHours;
					dataRow.Overtime = totalOvertime.TotalHours;
					dataRow.Night = totalNight.TotalHours;
					dataRow.TotalPresence = totalPresence.TotalHours + totalNight.TotalHours;
					dataRow.DocumentOvertime = totalDocumentOvertime.TotalHours;
					dataRow.DocumentAbsence = totalDocumentAbsence.TotalHours;
					dataRow.Balance = totalBalance.TotalHours - dataRow.DocumentOvertime + dataRow.DocumentAbsence;
					dataRow.TotalBalance = totalBalance.TotalHours;
				}

				dataSet.Data.Rows.Add(dataRow);
			}
			return dataSet;
		}

		TimeSpan CalculateEveningTime(TimeSpan start, TimeSpan end, List<TimeTrackPart> timeTrackParts)
		{
			var result = new TimeSpan();
			if (end > TimeSpan.Zero)
			{
				foreach (var trackPart in timeTrackParts)
				{
					if (trackPart.EndTime == new TimeSpan(23, 59, 0))
						trackPart.EndTime += new TimeSpan(0, 1, 0);
					if (trackPart.StartTime <= start && trackPart.EndTime >= end)
					{
						result += end - start;
					}
					else
					{
						if ((trackPart.StartTime >= start && trackPart.StartTime <= end) ||
							(trackPart.EndTime >= start && trackPart.EndTime <= end))
						{
							var minStartTime = trackPart.StartTime < start ? start : trackPart.StartTime;
							var minEndTime = trackPart.EndTime > end ? end : trackPart.EndTime;
							result += minEndTime - minStartTime;
						}
					}
				}
			}
			return result;
		}
	}
}
