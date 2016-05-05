using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class OrganisationHelper
	{
		public static IEnumerable<Organisation> Get(OrganisationFilter filter)
		{
			var result = FiresecManager.FiresecService.GetOrganisations(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<Organisation> GetByCurrentUser()
		{
			return Get(new OrganisationFilter { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static bool Save(OrganisationDetails organisation, bool isNew)
		{
			var result = FiresecManager.FiresecService.SaveOrganisation(organisation, isNew);
			return Common.ShowErrorIfExists(result);
		}

		public static Organisation GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new OrganisationFilter();
			filter.UIDs.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetOrganisations(filter);
			var result = Common.ShowErrorIfExists(operationResult);
			if (result != null)
				return result.FirstOrDefault();
			return null;
		}

		public static OrganisationDetails GetDetails(Guid uid)
		{
			var result = FiresecManager.FiresecService.GetOrganisationDetails(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveDoors(Organisation organisation)
		{
			var result = FiresecManager.FiresecService.SaveOrganisationDoors(organisation);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveUsers(Organisation organisation)
		{
			var result = FiresecManager.FiresecService.SaveOrganisationUsers(organisation);
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
			var result = FiresecManager.FiresecService.MarkDeletedOrganisation(uid, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreOrganisation(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool SaveChief(Organisation organisation, Guid chiefUID, string name)
		{
			return SaveChief(organisation.UID, chiefUID, organisation.Name);
		}

		public static bool SaveHRChief(Organisation organisation, Guid chiefUID, string name)
		{
			return SaveHRChief(organisation.UID, chiefUID, organisation.Name);
		}

		public static bool SaveChief(Guid uid, Guid chiefUID, string name)
		{
			var result = FiresecManager.FiresecService.SaveOrganisationChief(uid, chiefUID, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveHRChief(Guid uid, Guid chiefUID, string name)
		{
			var result = FiresecManager.FiresecService.SaveOrganisationHRChief(uid, chiefUID, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool IsAnyItems(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.IsAnyOrganisationItems(uid);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}