using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class DepartmentHelper
	{
		public static bool Save(Department department)
		{
			var result = FiresecManager.FiresecService.SaveDepartments(new List<Department> { department });
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(Department department)
		{
			var result = FiresecManager.FiresecService.MarkDeletedDepartments(new List<Department> { department });
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<Department> Get(DepartmentFilter filter)
		{
			var result = FiresecManager.FiresecService.GetDepartments(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static Department GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DepartmentFilter { Uids = new List<Guid> { uid.Value } };
			var result = FiresecManager.FiresecService.GetDepartments(filter);
			return Common.ShowErrorIfExists(result).FirstOrDefault();
		}

		public static void LinkToParent(Department child, Department parent)
		{
			child.ParentDepartmentUid = parent.UID;
			child.OrganizationUid = parent.OrganizationUid;
			parent.ChildDepartmentUids.Add(child.UID);
		}
	}
}
