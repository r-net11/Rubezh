using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using System;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class WorkingTimeReport : BaseReport
	{
		public WorkingTimeReport()
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
			get { return "Справка по отработанному времени"; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<WorkingTimeReportFilter>();

			var employeeFilter = dataProvider.GetEmployeeFilter(filter);
			var employees = dataProvider.GetEmployees(employeeFilter, filter.IsDefault);
			if (filter.IsSearch)
			{
				employeeFilter.FirstName = filter.FirstName;
				employeeFilter.SecondName = filter.SecondName;
				employeeFilter.LastName = filter.LastName;
			}
			var timeTrackResult = dataProvider.DatabaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new WorkingTimeDataSet();
			foreach (var employee in employees)
			{
				var dataRow = dataSet.Data.NewDataRow();

				dataRow.Employee = employee.Name;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;

				var timeTrackEmployeeResult = timeTrackResult.Result.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
				if (timeTrackEmployeeResult != null)
				{
					var totalScheduleDay = new TimeSpan();
					var totalScheduleNight = new TimeSpan();
					var totalPresence = new TimeSpan();
					var totalOvertime = new TimeSpan();
					var totalNight = new TimeSpan();
					var totalDocumentOvertime = new TimeSpan();
					var totalDocumentAbsence = new TimeSpan();
					var balance = new TimeSpan();
					foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
					{
						dayTimeTrack.Calculate();
						//if (dayTimeTrack.NightSettings != null && dayTimeTrack.NightSettings.IsNightSettingsEnabled)
						totalScheduleNight += dayTimeTrack.NightTimeForToday;

						totalScheduleDay = dayTimeTrack.SlideTime != default(TimeSpan)
											? dayTimeTrack.SlideTime
											: dayTimeTrack.PlannedTimeTrackParts.Aggregate(totalScheduleDay, (current, plannedPart) => current + plannedPart.Delta);

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

						var balanceTotal = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Balance);
						if (balanceTotal != null)
						{
							balance += balanceTotal.TimeSpan;
						}
					}
					dataRow.ScheduleDay = totalScheduleDay.TotalHours;
					dataRow.ScheduleNight = totalScheduleNight.TotalHours;
					dataRow.Presence = totalPresence.TotalHours;
					dataRow.Overtime = totalOvertime.TotalHours;
					dataRow.Night = totalNight.TotalHours;
					dataRow.TotalPresence = totalPresence.TotalHours + totalOvertime.TotalHours + totalNight.TotalHours;
					dataRow.DocumentOvertime = totalDocumentOvertime.TotalHours;
					dataRow.DocumentAbsence = totalDocumentAbsence.TotalHours;
					dataRow.Balance = balance.TotalHours;
					dataRow.TotalBalance = balance.TotalHours;
					dataRow.Balance = -(dataRow.ScheduleDay + dataRow.ScheduleNight) + dataRow.TotalPresence;
					dataRow.TotalBalance = dataRow.Balance + dataRow.DocumentOvertime - dataRow.DocumentAbsence;
				}

				dataSet.Data.Rows.Add(dataRow);
			}
			return dataSet;
		}
	}
}