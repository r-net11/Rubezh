using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class AdditionalColumnTypeHelper
	{
		public static bool Save(AdditionalColumnType AdditionalColumnType, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SaveAdditionalColumnType(AdditionalColumnType, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(AdditionalColumnType item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(AdditionalColumnType item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.MarkDeletedAdditionalColumnType(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestoreAdditionalColumnType(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static AdditionalColumnType GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = ClientManager.FiresecService.GetAdditionalColumnTypes(new AdditionalColumnTypeFilter
			{
				UIDs = new List<System.Guid> { uid.Value },
				LogicalDeletationType = LogicalDeletationType.All
			});
			var result = Common.ShowErrorIfExists(operationResult);
			if (result != null && result.Count > 0)
				return result.FirstOrDefault();
			return null;
		}

		public static IEnumerable<AdditionalColumnType> Get(AdditionalColumnTypeFilter filter, bool isShowError = true)
		{
			var operationResult = ClientManager.FiresecService.GetAdditionalColumnTypes(filter);
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}

		public static IEnumerable<AdditionalColumnType> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.FiresecService.GetAdditionalColumnTypes(new AdditionalColumnTypeFilter
				{
					OrganisationUIDs = new List<System.Guid> { organisationUID }
				});
			return Common.ShowErrorIfExists(result);
		}
	}
}