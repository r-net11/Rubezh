using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnHelper
	{
		public static IEnumerable<AdditionalColumn> GetByEmployee(Employee employee)
		{
			if (!employee.AdditionalColumnUIDs.IsNotNullOrEmpty())
				return null;
			var filter = new AdditionalColumnFilter();
			filter.EmployeeUIDs.Add(employee.UID);
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumns(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Save(List<AdditionalColumn> items)
		{
			var operationResult = FiresecManager.FiresecService.SaveAdditionalColumns(items);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
