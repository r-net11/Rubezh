using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhAPI.Models;

namespace SKDModule.ViewModels
{
	public class OrganisationUsersViewModel : OrganisationItemsViewModel<OrganisationUserViewModel>
	{
		public OrganisationUsersViewModel(Organisation organisation) : base(organisation)
		{
			Items = new ObservableCollection<OrganisationUserViewModel>();
			foreach (var user in ClientManager.SecurityConfiguration.Users)
			{
				var userViewModel = new OrganisationUserViewModel(organisation, user);
				Items.Add(userViewModel);
			}
		}

		protected override PermissionType Permission
		{
			get { return PermissionType.Oper_SKD_Organisations_Users; }
		}
	}
}