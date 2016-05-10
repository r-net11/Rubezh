using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class EmployeeHelper
	{
		public static bool Save(Employee employee, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployee(employee, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ShortEmployee item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(ShortEmployee item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedEmployee(uid, name);
			if (operationResult != null && operationResult.HasError &&!operationResult.Error.Contains(Resources.Language.FiresecManager.SKDHHelpers.EmployeeHelper.DBError))
				return true;
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreEmployee(uid, name);
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
			var filter = new EmployeeFilter { LogicalDeletationType = LogicalDeletationType.All, UIDs = new List<Guid> { uid.Value } };
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


		public static bool SetDepartment(ShortEmployee employee, Guid departmentUID)
		{
			return SetDepartment(employee.UID, departmentUID, employee.Name);
		}

		public static bool SetPosition(ShortEmployee employee, Guid positionUID)
		{
			return SetPosition(employee.UID, positionUID, employee.Name);
		}

		public static bool SetDepartment(Guid employeeUID, Guid departmentUID, string name)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployeeDepartment(employeeUID, departmentUID, name);
			return Common.ShowErrorIfExists(operationResult); 
		}

		public static bool SetPosition(Guid employeeUID, Guid positionUID, string name)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployeePosition(employeeUID, positionUID, name);
			return Common.ShowErrorIfExists(operationResult); 
		}

	}
}