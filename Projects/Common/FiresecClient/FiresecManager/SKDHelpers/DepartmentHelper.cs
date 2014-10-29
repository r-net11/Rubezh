using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class DepartmentHelper
	{
		public static bool Save(Department department, bool isNew)
		{
			var result = FiresecManager.FiresecService.SaveDepartment(department, isNew);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(ShortDepartment item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(ShortDepartment item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var result = FiresecManager.FiresecService.MarkDeletedDepartment(uid, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(Guid uid, string name)
		{
			var result = FiresecManager.FiresecService.RestoreDepartment(uid, name);
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
					UserUID = FiresecManager.CurrentUser.UID
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
	}
}
