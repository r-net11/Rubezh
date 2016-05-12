using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class PositionHelper
	{
		public static bool Save(Position position, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SavePosition(position, isNew);
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
			var operationResult = ClientManager.FiresecService.MarkDeletedPosition(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestorePosition(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Position GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = ClientManager.FiresecService.GetPositionDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.FiresecService.GetPositionList(new PositionFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<ShortPosition> Get(PositionFilter filter, bool isShowError = true)
		{
			var operationResult = ClientManager.FiresecService.GetPositionList(filter);
			return Common.ShowErrorIfExists(operationResult, isShowError);
		}

		public static IEnumerable<ShortPosition> GetByCurrentUser()
		{
			return Get(new PositionFilter() { User = ClientManager.CurrentUser });
		}

		public static ShortPosition GetSingleShort(Guid uid)
		{
			var filter = new PositionFilter();
			filter.UIDs.Add(uid);
			var operationResult = ClientManager.FiresecService.GetPositionList(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static List<Guid> GetEmployeeUIDs(Guid uid)
		{
			var operationResult = ClientManager.FiresecService.GetPositionEmployees(uid);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}