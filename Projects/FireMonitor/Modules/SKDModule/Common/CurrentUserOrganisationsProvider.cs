using System.Collections.Generic;
using FiresecClient.SKDHelpers;
using StrazhAPI.SKD;

namespace SKDModule
{
	public sealed class CurrentUserOrganisationsProvider : IOrganisationsProvider
	{
		public IEnumerable<Organisation> Get()
		{
			return OrganisationHelper.GetByCurrentUser();
		}
	}
}