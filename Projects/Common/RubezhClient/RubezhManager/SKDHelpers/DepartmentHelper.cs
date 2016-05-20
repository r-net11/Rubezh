using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class DepartmentHelper
	{
		public static bool Save(Department department, bool isNew)
		{
			var result = ClientManager.RubezhService.SaveDepartment(department, isNew);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(ShortDepartment item)
		{
			var result = ClientManager.RubezhService.MarkDeletedDepartment(item);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(ShortDepartment item)
		{
			var result = ClientManager.RubezhService.RestoreDepartment(item);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> Get(DepartmentFilter filter, bool isShowError = true)
		{
			var result = ClientManager.RubezhService.GetDepartmentList(filter);
			return Common.ShowErrorIfExists(result, isShowError);
		}

		public static IEnumerable<ShortDepartment> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.RubezhService.GetDepartmentList(new DepartmentFilter
				{
					OrganisationUIDs = new List<Guid> { organisationUID },
				});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> GetByCurrentUser()
		{
			return Get(new DepartmentFilter() { User = ClientManager.CurrentUser });
		}

		public static Department GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var result = ClientManager.RubezhService.GetDepartmentDetails(uid.Value);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveChief(ShortDepartment model, Guid? chiefUID)
		{
			return SaveChief(model.UID, chiefUID, model.Name);
		}

		public static bool SaveChief(Guid uid, Guid? chiefUID, string name)
		{
			var result = ClientManager.RubezhService.SaveDepartmentChief(uid, chiefUID, name);
			return Common.ShowErrorIfExists(result);
		}

		public static ShortDepartment GetSingleShort(Guid uid)
		{
			var filter = new DepartmentFilter();
			filter.UIDs.Add(uid);
			var operationResult = ClientManager.RubezhService.GetDepartmentList(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Guid> GetChildEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.RubezhService.GetChildEmployeeUIDs(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<Guid> GetEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.RubezhService.GetDepartmentEmployeeUIDs(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<Guid> GetParentEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.RubezhService.GetParentEmployeeUIDs(uid);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
