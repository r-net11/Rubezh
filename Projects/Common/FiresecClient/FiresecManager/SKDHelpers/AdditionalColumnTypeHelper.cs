using System;
using System.Collections.Generic;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class AdditionalColumnTypeHelper
	{
		public static bool Save(AdditionalColumnType AdditionalColumnType, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SaveAdditionalColumnType(AdditionalColumnType, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ShortAdditionalColumnType item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(ShortAdditionalColumnType item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedAdditionalColumnType(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreAdditionalColumnType(uid, name);
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

		public static IEnumerable<ShortAdditionalColumnType> GetByCurrentUser()
		{
			return Get(new AdditionalColumnTypeFilter { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static IEnumerable<ShortAdditionalColumnType> GetShortByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetAdditionalColumnTypeList(new AdditionalColumnTypeFilter
				{
					OrganisationUIDs = new List<Guid> { organisationUID }
				});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<AdditionalColumnType> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetAdditionalColumnTypes(new AdditionalColumnTypeFilter
			{
				OrganisationUIDs = new List<Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}
	}
}