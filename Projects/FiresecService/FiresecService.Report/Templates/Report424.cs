using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

namespace FiresecService.Report.Templates
{
	public partial class Report424 : BaseSKDReport
	{
		public Report424()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Справка по отработанному времени"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter424>();
			var databaseService = new SKDDatabaseService();

			if (filter.Employees == null)
				filter.Employees = new List<Guid>();
			if (filter.Departments == null)
				filter.Departments = new List<Guid>();
			if (filter.Positions == null)
				filter.Positions = new List<Guid>();
			if (filter.Organisations == null)
				filter.Organisations = new List<Guid>();
			var employeeFilter = new EmployeeFilter();
			employeeFilter.OrganisationUIDs = filter.Organisations;
			employeeFilter.DepartmentUIDs = filter.Departments;
			employeeFilter.PositionUIDs = filter.Positions;
			employeeFilter.UIDs = filter.Employees;
			var employeesResult = databaseService.EmployeeTranslator.Get(employeeFilter);

			var timeTrackResult = databaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new DataSet424();
			if (employeesResult.Result != null)
			{
				foreach (var employee in employeesResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();

					dataRow.Employee = employee.Name;
					if (employee.Department != null)
					{
						dataRow.Department = employee.Department.Name;
					}
					if (employee.Position != null)
					{
						dataRow.Position = employee.Position.Name;
					}

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
						foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
						{
							foreach (var plannedTimeTrackPart in dayTimeTrack.PlannedTimeTrackParts)
							{
								if (plannedTimeTrackPart.EndTime < new TimeSpan(20, 0, 0))
								{
									totalScheduleDay += plannedTimeTrackPart.EndTime - plannedTimeTrackPart.StartTime;
								}
								if (plannedTimeTrackPart.StartTime > new TimeSpan(20, 0, 0))
								{
									totalScheduleNight += plannedTimeTrackPart.EndTime - plannedTimeTrackPart.StartTime;
								}
								if (plannedTimeTrackPart.StartTime < new TimeSpan(20, 0, 0) && plannedTimeTrackPart.EndTime > new TimeSpan(20, 0, 0))
								{
									totalScheduleDay += new TimeSpan(20, 0, 0) - plannedTimeTrackPart.StartTime;
									totalScheduleNight += plannedTimeTrackPart.EndTime - new TimeSpan(20, 0, 0);
								}
							}

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
						}
						dataRow.ScheduleDay = totalScheduleDay.TotalHours;
						dataRow.ScheduleNight = totalScheduleNight.TotalHours;
						dataRow.Presence = totalPresence.TotalHours;
						dataRow.Overtime = totalOvertime.TotalHours;
						dataRow.Night = totalNight.TotalHours;
						dataRow.TotalPresence = totalPresence.TotalHours + totalOvertime.TotalHours + totalNight.TotalHours;
						dataRow.DocumentOvertime = totalDocumentOvertime.TotalHours;
						dataRow.DocumentAbsence = totalDocumentAbsence.TotalHours;
						dataRow.Balance = -(dataRow.ScheduleDay + dataRow.ScheduleNight) + dataRow.TotalPresence;
						dataRow.TotalBalance = dataRow.Balance + dataRow.DocumentOvertime - dataRow.DocumentAbsence;
					}

					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}
	}
}