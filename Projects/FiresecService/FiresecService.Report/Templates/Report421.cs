using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using SKDDriver;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class Report421 : BaseReport
	{
		public Report421()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Дисциплинарный отчет"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<ReportFilter421>();

			var employeeFilter = dataProvider.GetEmployeeFilter(filter);
			var employees = dataProvider.GetEmployees(employeeFilter);
			var timeTrackResult = dataProvider.DatabaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new DataSet421();
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
                        var dataRow = dataSet.Data.NewDataRow();
                        dataRow.Employee = employee.Name;
                        dataRow.Organisation = employee.Organisation;
                        dataRow.Department = employee.Department;

                        dataRow.Date = dayTimeTrack.Date;

                        if (dayTimeTrack.RealTimeTrackParts.Count > 0)
                        {
                            dataRow.FirstEnter = dayTimeTrack.RealTimeTrackParts.Min(x => x.StartTime);
                            dataRow.LastExit = dayTimeTrack.RealTimeTrackParts.Max(x => x.EndTime);
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

                        foreach (var document in dayTimeTrack.Documents)
                        {
                        }

                        dataSet.Data.Rows.Add(dataRow);
                    }
                }
            }
			return dataSet;
		}
	}
}