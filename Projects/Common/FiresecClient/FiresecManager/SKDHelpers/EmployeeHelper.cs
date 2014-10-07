using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class EmployeeHelper
	{
		public static bool Save(Employee employee)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployee(employee);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedEmployee(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.RestoreEmployee(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortEmployee> Get(EmployeeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetEmployeeList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static ShortEmployee GetSingleShort(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new EmployeeFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetEmployeeList(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static Employee GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetEmployeeDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static TimeTrackResult GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var operationResult = FiresecManager.FiresecService.GetTimeTracks(filter, startDate, endDate);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortEmployee> GetShortByOrganisation(Guid organisationUID)
		{
			var operationResult = FiresecManager.FiresecService.GetEmployeeList(new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisationUID } });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SetDepartment(Guid employeeUID, Guid departmentUID)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployeeDepartment(employeeUID, departmentUID);
			return Common.ShowErrorIfExists(operationResult); 
		}

		public static bool SetPosition(Guid employeeUID, Guid positionUID)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployeePosition(employeeUID, positionUID);
			return Common.ShowErrorIfExists(operationResult); 
		}

	}
}