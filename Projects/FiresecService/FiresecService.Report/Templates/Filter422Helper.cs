using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using FiresecAPI.SKD;

namespace FiresecService.Report.Templates
{
	public class Filter422Helper
	{
		public List<Employee> GetData(ReportFilter422 filter)
		{
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

			var result = new List<Employee>();
			foreach (var employee in employees)
			{
				if (filter.Schedules != null && filter.Schedules.Count > 0)
				{
					if (employee.Schedule != null)
					{
						//if (!filter.Schedules.Contains(employee.Schedule.UID))
						//    continue;
					}
				}
				result.Add(employee);
			}

			return result;
		}
	}
}