﻿using System;
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
			var timeTrackResult = dataProvider.DatabaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new DisciplineDataSet();
			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Item.Schedule == null || !filter.ScheduleSchemas.Contains(employee.Item.Schedule.UID))
						continue;
				}
				var timeTrackEmployeeResult = timeTrackResult.Result.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
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

						if (dayTimeTrack.RealTimeTrackParts.Count > 0)
						{
							dataRow.FirstEnter = dayTimeTrack.RealTimeTrackParts.Min(x => x.EnterDateTime.TimeOfDay);
							dataRow.LastExit = dayTimeTrack.RealTimeTrackParts.Where(x => x.ExitDateTime.HasValue).Select(x => x.ExitDateTime).DefaultIfEmpty().Max().GetValueOrDefault().TimeOfDay;
						}

						var absence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Absence);
						if (absence != null)
						{
							dataRow.Absence = absence.TimeSpan;
						}

						var late = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Late);
						if (late != null)
						{
							dataRow.Late = late.TimeSpan;
						}

						var earlyLeave = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave);
						if (earlyLeave != null)
						{
							dataRow.EarlyLeave = earlyLeave.TimeSpan;
						}

						var overtime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime);
						if (overtime != null)
						{
							dataRow.Overtime = overtime.TimeSpan;
						}

						var isHoliday = dayTimeTrack.IsHoliday;

						var document = timeTrackEmployeeResult.Documents.FirstOrDefault(x => (dayTimeTrack.Date >= x.StartDateTime.Date && dayTimeTrack.Date <= x.EndDateTime.Date) && x.TimeTrackDocumentType.ShortName == dayTimeTrack.LetterCode);
						if (document != null)
						{
							dataRow.DocumentDate = document.DocumentDateTime;
							dataRow.DocumentName = document.TimeTrackDocumentType.Name;
							dataRow.DocumentNo = document.DocumentNumber.ToString();
						}

						if (absence.TimeSpan.TotalSeconds > 0 || late.TimeSpan.TotalSeconds > 0 || earlyLeave.TimeSpan.TotalSeconds > 0 || overtime.TimeSpan.TotalSeconds > 0 || document != null)
						{
							dataSet.Data.Rows.Add(dataRow);
						}
					}
				}
			}
			return dataSet;
		}
	}
}