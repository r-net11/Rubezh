using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using FiresecAPI.SKD;

namespace FiresecService.Report.Templates
{
	public class Filter402Helper
	{
		public List<SKDDriver.DataAccess.PassJournal> GetData(ReportFilter402 filter)
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

			var zoneUIDs = new List<Guid>();
			if (filter.Zones != null)
			{
				zoneUIDs.AddRange(filter.Zones);
			}
			if (filter.Doors != null)
			{
				foreach (var doorUID in filter.Doors)
				{
					var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
					if (door != null)
					{
						if (door.InDevice != null && door.InDevice.ZoneUID != Guid.Empty)
						{
							zoneUIDs.Add(door.InDevice.ZoneUID);
						}
						if (door.OutDevice != null && door.OutDevice.ZoneUID != Guid.Empty)
						{
							zoneUIDs.Add(door.OutDevice.ZoneUID);
						}
					}
				}
			}

			var result = new List<SKDDriver.DataAccess.PassJournal>();
			foreach (var employee in employees)
			{
				var passJournal2 = databaseService.PassJournalTranslator.GetEmployeeRoot(employee.UID, zoneUIDs, filter.DateTimeFrom, filter.DateTimeTo);
				if (passJournal2 != null)
				{
					result.AddRange(passJournal2);
				}
			}
			return result;
		}
	}
}