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

		public static bool Save(Organization Organization)
		{
			var result = FiresecManager.FiresecService.SaveOrganizations(new List<Organization> { Organization });
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(Organization Organization)
		{
			var result = FiresecManager.FiresecService.MarkDeletedOrganizations(new List<Organization> { Organization });
			return Common.ShowErrorIfExists(result);
		}
	}
}
