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

		public static bool MarkDeleted(Organisation organisation)
		{
			return MarkDeleted(organisation);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var result = FiresecManager.FiresecService.MarkDeletedOrganisation(uid);
			return Common.ShowErrorIfExists(result);
		}
	}
}