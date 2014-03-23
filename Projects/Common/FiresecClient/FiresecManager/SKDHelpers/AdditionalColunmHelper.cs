using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnHelper
	{
		public static AdditionalColumn GetValue(Employee employee, Guid? columnTypeUID)
		{
			if (!employee.AdditionalColumnUIDs.IsNotNullOrEmpty())
				return null;
			var filter = new AdditionalColumnFilter();
			filter.EmployeeUIDs.Add(employee.UID);
			if (columnTypeUID.HasValue != null)
				filter.ColumnTypeUIDs.Add(columnTypeUID.Value);
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumns(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

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
