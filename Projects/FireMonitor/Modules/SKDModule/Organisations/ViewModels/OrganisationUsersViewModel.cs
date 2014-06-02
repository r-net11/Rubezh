using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class OrganisationUsersViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationUsersViewModel(Organisation organisation)
		{
			Organisation = organisation;
			Users = new ObservableCollection<OrganisationUserViewModel>();
			foreach (var user in FiresecManager.SecurityConfiguration.Users)
			{
				var userViewModel = new OrganisationUserViewModel(organisation, user);
				Users.Add(userViewModel);
			}
			SelectedUser = Users.FirstOrDefault();
		}

		ObservableCollection<OrganisationUserViewModel> _users;
		public ObservableCollection<OrganisationUserViewModel> Users
		{
			get { return _users; }
			private set
			{
				_users = value;
				OnPropertyChanged("Users");
			}
		}

		OrganisationUserViewModel _selectedUser;
		public OrganisationUserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged("SelectedUser");
			}
		}
	}
}