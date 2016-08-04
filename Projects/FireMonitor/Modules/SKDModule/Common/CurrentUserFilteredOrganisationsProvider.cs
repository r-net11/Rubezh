
using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient.SKDHelpers;
using StrazhAPI.SKD;

namespace SKDModule
{
	public class CurrentUserFilteredOrganisationsProvider : IOrganisationsProvider
	{
		#region <Поля и свойства>

		private readonly IEnumerable<Guid> _allowedOrganisationIDs;

		#endregion </Поля и свойства>

		#region <Конструктор>

		public CurrentUserFilteredOrganisationsProvider(IEnumerable<Guid> allowedOrganisationIDs)
		{
			_allowedOrganisationIDs = allowedOrganisationIDs;
		}
		
		#endregion </Конструктор>

		#region <IOrganisationsProvider>

		public IEnumerable<Organisation> Get()
		{
			return OrganisationHelper.GetByCurrentUser().Where(org => _allowedOrganisationIDs.Any(orgID => orgID == org.UID));
		}

		#endregion </IOrganisationsProvider>
	}
}