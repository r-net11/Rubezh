using System.Collections.ObjectModel;
using StrazhAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class OrganisationUsersViewModel : OrganisationItemsViewModel<OrganisationUserViewModel>
	{
		public OrganisationUsersViewModel(Organisation organisation):base(organisation)
		{
			Items = new ObservableCollection<OrganisationUserViewModel>();
			foreach (var user in FiresecManager.SecurityConfiguration.Users)
			{
				var userViewModel = new OrganisationUserViewModel(organisation, user);
				Items.Add(userViewModel);
			}
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_Organisations_Users; }
		}
	}
}