using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnTypeHelper
	{
		public static bool Save(AdditionalColumnType AdditionalColumnType)
		{
			var operationResult = FiresecManager.FiresecService.SaveAdditionalColumnType(AdditionalColumnType);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedAdditionalColumnType(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static AdditionalColumnType GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumnTypeDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortAdditionalColumnType> Get(AdditionalColumnTypeFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetAdditionalColumnTypeList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortAdditionalColumnType> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetAdditionalColumnTypeList(new AdditionalColumnTypeFilter
				{
					OrganisationUIDs = new List<System.Guid> { organisationUID }
				});
			return Common.ShowErrorIfExists(result);
		}
	}
}