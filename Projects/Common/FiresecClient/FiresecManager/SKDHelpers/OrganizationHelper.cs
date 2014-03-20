using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class OrganizationHelper
	{
		public static IEnumerable<Organization> Get(OrganizationFilter filter)
		{
			var result = FiresecManager.FiresecService.GetOrganizations(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Save(Organization organization)
		{
			var result = FiresecManager.FiresecService.SaveOrganizations(new List<Organization> { organization });
			return Common.ShowErrorIfExists(result);
		}

		public static Organization GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new OrganizationFilter();
			filter.Uids.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetOrganizations(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static bool SaveZones(Organization organization)
		{
			var result = FiresecManager.FiresecService.SaveOrganizationZones(organization);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(Organization organization)
		{
			var result = FiresecManager.FiresecService.MarkDeletedOrganizations(new List<Organization> { organization });
			return Common.ShowErrorIfExists(result);
		}
	}
}