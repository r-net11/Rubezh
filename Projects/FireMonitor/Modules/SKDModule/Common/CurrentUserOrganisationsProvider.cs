
using System.Collections.Generic;
using FiresecClient.SKDHelpers;
using StrazhAPI.SKD;

namespace SKDModule
{
	public class CurrentUserOrganisationsProvider : IOrganisationsProvider
	{
		#region <IOrganisationsProvider>

		public IEnumerable<Organisation> Get()
		{
			return OrganisationHelper.GetByCurrentUser();
		}

		#endregion </IOrganisationsProvider>
	}
}