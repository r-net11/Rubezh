using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using RubezhClient;

namespace SKDModule.ViewModels
{
	public class OrganisationUsersViewModel : OrganisationItemsViewModel<OrganisationUserViewModel>
	{
		public OrganisationUsersViewModel(Organisation organisation):base(organisation)
		{
			Items = new ObservableCollection<OrganisationUserViewModel>();
			foreach (var user in ClientManager.SecurityConfiguration.Users)
			{
				var userViewModel = new OrganisationUserViewModel(organisation, user);
				Items.Add(userViewModel);
			}
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_Users; }
		}
	}
}