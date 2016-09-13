using FiresecClient;
using FiresecClient.SKDHelpers;
using StrazhAPI.SKD;
using System.Collections.Generic;

namespace SKDModule.Common
{
	public sealed class CurrentUserDeletedOrganisationsProvider : IOrganisationsProvider
	{
		public IEnumerable<Organisation> Get()
		{
			return OrganisationHelper.Get(new OrganisationFilter
			{
				UserUID = FiresecManager.CurrentUser.UID,
				LogicalDeletationType = LogicalDeletationType.All
			});
		}
	}
}
