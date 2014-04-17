using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnHelper
	{
		public static string GetValue(ShortEmployee employee, Guid? columnTypeUID)
		{
			return employee.TextColumns.FirstOrDefault(x => x.ColumnTypeUID == columnTypeUID).Text;
		}

		public static bool Save(List<AdditionalColumn> items)
		{
			var operationResult = FiresecManager.FiresecService.SaveAdditionalColumns(items);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
