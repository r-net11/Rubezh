using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class EmployeeHelper
	{
		public static bool Save(Employee Employee)
		{
			var operationResult = FiresecManager.FiresecService.SaveEmployees(new List<Employee> { Employee });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Employee Employee)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedEmployees(new List<Employee> { Employee });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Employee GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new EmployeeFilter();
			filter.Uids.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetEmployees(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Employee> Get(EmployeeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetEmployees(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
