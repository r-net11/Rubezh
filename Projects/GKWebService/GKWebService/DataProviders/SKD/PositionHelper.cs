using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using RubezhClient;

namespace GKWebService.DataProviders.SKD
{
    public static class PositionHelper
	{
		public static bool Save(Position position, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SavePosition(position, isNew);
			return Common.ThrowErrorIfExists(operationResult);
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
			var operationResult = ClientManager.FiresecService.MarkDeletedPosition(uid, name);
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestorePosition(uid, name);
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static Position GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = ClientManager.FiresecService.GetPositionDetails(uid.Value);
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.FiresecService.GetPositionList(new PositionFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ThrowErrorIfExists(result);
		}

		public static IEnumerable<ShortPosition> Get(PositionFilter filter)
		{
			var operationResult = ClientManager.FiresecService.GetPositionList(filter);
			return Common.ThrowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> GetByCurrentUser()
		{
			return Get(new PositionFilter() { UserUID = ClientManager.CurrentUser.UID });
		}

		public static ShortPosition GetSingleShort(Guid uid)
		{
			var filter = new PositionFilter();
			filter.UIDs.Add(uid);
			var operationResult = ClientManager.FiresecService.GetPositionList(filter);
			return Common.ThrowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static List<Guid> GetEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.FiresecService.GetPositionEmployees(uid);
			return Common.ThrowErrorIfExists(operationResult);
		}
	}
}