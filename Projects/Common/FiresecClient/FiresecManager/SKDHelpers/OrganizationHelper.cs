using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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