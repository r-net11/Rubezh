using System;
using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class DepartmentHelper
	{
		public static bool Save(Department department)
		{
			var result = FiresecManager.FiresecService.SaveDepartment(department);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var result = FiresecManager.FiresecService.MarkDeletedDepartment(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> Get(DepartmentFilter filter)
		{
			var result = FiresecManager.FiresecService.GetDepartmentList(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortDepartment> GetByOrganisation(Guid? organisationUID)
		{
			if (organisationUID == null)
				return null;
			var result = FiresecManager.FiresecService.GetDepartmentList(new DepartmentFilter { OrganisationUIDs = new List<System.Guid> { organisationUID.Value } });
			return Common.ShowErrorIfExists(result);
		}

		public static Department GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var result = FiresecManager.FiresecService.GetDepartmentDetails(uid.Value);
			return Common.ShowErrorIfExists(result);
		}
	}
}
