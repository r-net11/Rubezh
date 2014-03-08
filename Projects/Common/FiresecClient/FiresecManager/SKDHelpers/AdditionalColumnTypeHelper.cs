using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnTypeHelper
	{
		public static bool Save(AdditionalColumnType AdditionalColumnType)
		{
			var operationResult = FiresecManager.FiresecService.SaveAdditionalColumnTypes(new List<AdditionalColumnType> { AdditionalColumnType });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(AdditionalColumnType AdditionalColumnType)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedAdditionalColumnTypes(new List<AdditionalColumnType> { AdditionalColumnType });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static AdditionalColumnType GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new AdditionalColumnTypeFilter();
			filter.Uids.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumnTypes(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<AdditionalColumnType> Get(AdditionalColumnTypeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumnTypes(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
