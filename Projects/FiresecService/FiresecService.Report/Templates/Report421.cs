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
	public partial class Report421 : BaseSKDReport
	{
		public Report421()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Дисциплинарный отчет"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter421>();
			var databaseService = new SKDDatabaseService();

			if (filter.Employees == null)
				filter.Employees = new List<Guid>();
			if (filter.Departments == null)
				filter.Departments = new List<Guid>();
			if (filter.Organisations == null)
				filter.Organisations = new List<Guid>();
			var employeeFilter = new EmployeeFilter();
			employeeFilter.OrganisationUIDs = filter.Organisations;
			employeeFilter.DepartmentUIDs = filter.Departments;
			employeeFilter.UIDs = filter.Employees;
			var employeesResult = databaseService.EmployeeTranslator.Get(employeeFilter);

			var timeTrackResult = databaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

			var dataSet = new DataSet421();
			if (employeesResult.Result != null)
			{
				foreach (var employee in employeesResult.Result)
				{
					var timeTrackEmployeeResult = timeTrackResult.Result.TimeTrackEmployeeResults.FirstOrDefault(x => x.ShortEmployee.UID == employee.UID);
					if (timeTrackEmployeeResult != null)
					{
						foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
						{
							var dataRow = dataSet.Data.NewDataRow();

							dataRow.Employee = employee.Name;
							var organisationResult = databaseService.OrganisationTranslator.GetSingle(employee.OrganisationUID);
							if (organisationResult.Result != null)
							{
								dataRow.Organisation = organisationResult.Result.Name;
							}
							if (employee.Department != null)
							{
								dataRow.Department = employee.Department.Name;
							}

							dataRow.Date = dayTimeTrack.Date;

							if (dayTimeTrack.RealTimeTrackParts.Count > 0)
							{
								dataRow.Period = dayTimeTrack.RealTimeTrackParts.Min(x => x.StartTime);
								dataRow.Leave = dayTimeTrack.RealTimeTrackParts.Max(x => x.EndTime);
							}

							var absence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Absence);
							if (absence != null)
							{
								//dataRow.Absence = absence.TimeSpan;
							}

							var late = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Late);
							if (late != null)
							{
								//dataRow.LeaveBefore = late.TimeSpan;
							}

							var earlyLeave = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave);
							if (earlyLeave != null)
							{
								//dataRow.LeaveBefore = earlyLeave.TimeSpan;
							}

							var overtime = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime);
							if (overtime != null)
							{
								//dataRow.Overtime = overtime.TimeSpan;
							}

							var isHoliday = dayTimeTrack.IsHoliday;

							foreach (var document in dayTimeTrack.Documents)
							{
							}

							dataSet.Data.Rows.Add(dataRow);
						}
					}
				}
			}
			return dataSet;
		}
	}
}