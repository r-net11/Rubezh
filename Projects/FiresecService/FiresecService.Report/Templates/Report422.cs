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
	public partial class Report422 : BaseSKDReport
	{
		public Report422()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Отчет по графикам работы"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter422>();
			var dataSet = new DataSet422();

			var databaseService = new SKDDatabaseService();

			var employees = new List<Employee>();
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
			if (employeesResult.Result != null)
			{
				employees = employeesResult.Result.ToList();
			}

			foreach (var employee in employees)
			{
				if (filter.ScheduleSchemas != null && filter.ScheduleSchemas.Count > 0)
				{
					if (employee.Schedule != null)
					{
						if (!filter.ScheduleSchemas.Contains(employee.Schedule.UID))
							continue;
					}
				}

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
				if (employee.Position != null)
				{
					dataRow.Position = employee.Position.Name;
				}

				if (employee.Schedule != null)
				{
					dataRow.Schedule = employee.Schedule.Name;
					var scheduleResult = databaseService.ScheduleTranslator.GetSingle(employee.Schedule.UID);
					if (scheduleResult.Result != null)
					{
						dataRow.UseHoliday = !scheduleResult.Result.IsIgnoreHoliday;
						dataRow.FirstEnterLastExit = scheduleResult.Result.IsOnlyFirstEnter;
						dataRow.Delay = scheduleResult.Result.AllowedLate;
						dataRow.LeaveBefore = scheduleResult.Result.AllowedLate;

						var scheduleSchemeResult = databaseService.ScheduleSchemeTranslator.GetSingle(scheduleResult.Result.ScheduleSchemeUID);
						if (scheduleSchemeResult.Result != null)
						{
							dataRow.BaseSchedule = scheduleSchemeResult.Result.Name;
							dataRow.ScheduleType = scheduleSchemeResult.Result.Type.ToDescription();
						}
					}
				}

				dataSet.Data.Rows.Add(dataRow);
			}

			return dataSet;
		}
	}
}