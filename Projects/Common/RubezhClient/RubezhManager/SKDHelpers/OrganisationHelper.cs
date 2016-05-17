using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class OrganisationHelper
	{
		public static IEnumerable<Organisation> Get(OrganisationFilter filter, bool isShowError = true)
		{
			var result = ClientManager.RubezhService.GetOrganisations(filter);
			return Common.ShowErrorIfExists(result, isShowError);
		}

		public static IEnumerable<Organisation> GetByCurrentUser(LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			return Get(new OrganisationFilter() { User = ClientManager.CurrentUser, LogicalDeletationType = logicalDeletationType });
		}

		public static bool Save(OrganisationDetails organisation, bool isNew)
		{
			var result = ClientManager.RubezhService.SaveOrganisation(organisation, isNew);
			return Common.ShowErrorIfExists(result);
		}

		public static Organisation GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new OrganisationFilter();
			filter.UIDs.Add((Guid)uid);
			var operationResult = ClientManager.RubezhService.GetOrganisations(filter);
			var result = Common.ShowErrorIfExists(operationResult);
			if (result != null)
				return result.FirstOrDefault();
			return null;
		}

		public static OrganisationDetails GetDetails(Guid uid)
		{
			var result = ClientManager.RubezhService.GetOrganisationDetails(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static bool AddDoor(Organisation organisation, Guid doorUID)
		{
			var result = ClientManager.RubezhService.AddOrganisationDoor(organisation, doorUID);
			return Common.ShowErrorIfExists(result);
		}

		public static bool RemoveDoor(Organisation organisation, Guid doorUID)
		{
			var result = ClientManager.RubezhService.RemoveOrganisationDoor(organisation, doorUID);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveUsers(Organisation organisation)
		{
			var result = ClientManager.RubezhService.SaveOrganisationUsers(organisation);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(Organisation organisation)
		{
			return MarkDeleted(organisation.UID, organisation.Name);
		}

		public static bool Restore(Organisation organisation)
		{
			return Restore(organisation.UID, organisation.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var result = ClientManager.RubezhService.MarkDeletedOrganisation(uid, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.RubezhService.RestoreOrganisation(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		//public static bool SaveChief(Organisation organisation, Guid chiefUID, string name)
		//{
		//    return SaveChief(organisation.UID, chiefUID, organisation.Name);
		//}

		//public static bool SaveHRChief(Organisation organisation, Guid chiefUID, string name)
		//{
		//    return SaveHRChief(organisation.UID, chiefUID, organisation.Name);
		//}

		public static bool SaveChief(Guid uid, Guid? chiefUID, string name)
		{
			var result = ClientManager.RubezhService.SaveOrganisationChief(uid, chiefUID, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveHRChief(Guid uid, Guid? chiefUID, string name)
		{
			var result = ClientManager.RubezhService.SaveOrganisationHRChief(uid, chiefUID, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool IsAnyItems(Guid uid)
		{
			var operationResult = ClientManager.RubezhService.IsAnyOrganisationItems(uid);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}