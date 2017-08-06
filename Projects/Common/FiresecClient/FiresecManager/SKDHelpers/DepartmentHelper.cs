using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class DepartmentHelper
	{
		public static bool Save(Department department, bool isNew)
		{
			var result = FiresecManager.FiresecService.SaveDepartment(department, isNew);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Save(Department department, bool isNew, bool ignoreErrors)
		{
			var result = FiresecManager.FiresecService.SaveDepartment(department, isNew);
			if (!ignoreErrors)
			{
				return Common.ShowErrorIfExists(result);
			}
			return true;
		}

		public static bool MarkDeleted(ShortDepartment item)
		{
			var result = FiresecManager.FiresecService.MarkDeletedDepartment(item);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(ShortDepartment item)
		{
			var result = FiresecManager.FiresecService.RestoreDepartment(item);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> Get(DepartmentFilter filter)
		{
			var result = FiresecManager.FiresecService.GetDepartmentList(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetDepartmentList(new DepartmentFilter 
				{ 
					OrganisationUIDs = new List<Guid> { organisationUID },
				});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> GetByCurrentUser()
		{
			return Get(new DepartmentFilter() { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static Department GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var result = FiresecManager.FiresecService.GetDepartmentDetails(uid.Value);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveChief(ShortDepartment model, Guid chiefUID)
		{
			return SaveChief(model.UID, chiefUID, model.Name);
		}
		
		public static bool SaveChief(Guid uid, Guid chiefUID, string name)
		{
			var result = FiresecManager.FiresecService.SaveDepartmentChief(uid, chiefUID, name);
			return Common.ShowErrorIfExists(result);
		}
		
		public static ShortDepartment GetSingleShort(Guid uid)
		{
			var filter = new DepartmentFilter();
			filter.UIDs.Add(uid);
			var operationResult = FiresecManager.FiresecService.GetDepartmentList(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Guid> GetChildEmployeeUIDs(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.GetChildEmployeeUIDs(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<Guid> GetParentEmployeeUIDs(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.GetParentEmployeeUIDs(uid);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
