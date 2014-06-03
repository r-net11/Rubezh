using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;

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
			return Get(new OrganisationFilter() { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static bool Save(OrganisationDetails organisation)
		{
			var result = FiresecManager.FiresecService.SaveOrganisation(organisation);
			return Common.ShowErrorIfExists(result);
		}

		public static Organisation GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new OrganisationFilter();
			filter.UIDs.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetOrganisations(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static OrganisationDetails GetDetails(Guid uid)
		{
			var result = FiresecManager.FiresecService.GetOrganisationDetails(uid);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveZones(Organisation Organisation)
		{
			var result = FiresecManager.FiresecService.SaveOrganisationZones(Organisation);
			return Common.ShowErrorIfExists(result);
		}

		public static bool SaveUsers(Organisation Organisation)
		{
			var result = FiresecManager.FiresecService.SaveOrganisationUsers(Organisation);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(Organisation organisation)
		{
			var result = FiresecManager.FiresecService.MarkDeletedOrganisation(organisation.UID);
			return Common.ShowErrorIfExists(result);
		}
	}
}