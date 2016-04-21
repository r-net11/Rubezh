using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class EmployeeHelper
	{
		public static bool Save(Employee employee, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SaveEmployee(employee, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ShortEmployee item)
		{
			return MarkDeleted(item.UID, item.Name, item.Type == PersonType.Employee);
		}

		public static bool Restore(ShortEmployee item)
		{
			return Restore(item.UID, item.Name, item.Type == PersonType.Employee);
		}

		public static bool MarkDeleted(Guid uid, string name, bool isEmployee)
		{
			var operationResult = ClientManager.FiresecService.MarkDeletedEmployee(uid, name, isEmployee);
			if (operationResult != null && operationResult.HasError && !operationResult.Error.Contains("Ошибка БД"))
				return true;
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name, bool isEmployee)
		{
			var operationResult = ClientManager.FiresecService.RestoreEmployee(uid, name, isEmployee);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortEmployee> Get(EmployeeFilter filter, bool isShowError = true)
		{
			var operationResult = ClientManager.FiresecService.GetEmployeeList(filter);
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}

		public static ShortEmployee GetSingleShort(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new EmployeeFilter { LogicalDeletationType = LogicalDeletationType.All, UIDs = new List<Guid> { uid.Value }, IsAllPersonTypes = true };
			var operationResult = ClientManager.FiresecService.GetEmployeeList(filter);
			var result = Common.ShowErrorIfExists(operationResult);
			return result != null ? result.FirstOrDefault() : null;
		}

		public static Employee GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = ClientManager.FiresecService.GetEmployeeDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static TimeTrackResult GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var operationResult = ClientManager.FiresecService.GetTimeTracks(filter, startDate, endDate);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortEmployee> GetShortByOrganisation(Guid organisationUID)
		{
			var operationResult = ClientManager.FiresecService.GetEmployeeList(new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisationUID } });
			return Common.ShowErrorIfExists(operationResult);
		}


		public static bool SetDepartment(ShortEmployee employee, Guid? departmentUID)
		{
			return SetDepartment(employee.UID, departmentUID, employee.Name);
		}

		public static bool SetPosition(ShortEmployee employee, Guid? positionUID)
		{
			return SetPosition(employee.UID, positionUID, employee.Name);
		}

		public static bool SetDepartment(Guid employeeUID, Guid? departmentUID, string name)
		{
			var operationResult = ClientManager.FiresecService.SaveEmployeeDepartment(employeeUID, departmentUID, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SetPosition(Guid employeeUID, Guid? positionUID, string name)
		{
			var operationResult = ClientManager.FiresecService.SaveEmployeePosition(employeeUID, positionUID, name);
			return Common.ShowErrorIfExists(operationResult);
		}

	}
}