using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class PositionHelper
	{
		public static bool Save(Position position, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SavePosition(position, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ShortPosition item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(ShortPosition item)
		{
			return Restore(item.UID, item.Name); 
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPosition(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestorePosition(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Position GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetPositionDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetPositionList(new PositionFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortPosition> Get(PositionFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetPositionList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> GetByCurrentUser()
		{
			return Get(new PositionFilter() { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static ShortPosition GetSingleShort(Guid uid)
		{
			var filter = new PositionFilter();
			filter.UIDs.Add(uid);
			var operationResult = FiresecManager.FiresecService.GetPositionList(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}
	}
}