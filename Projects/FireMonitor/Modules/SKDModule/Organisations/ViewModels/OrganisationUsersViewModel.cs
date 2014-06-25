using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

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
			//foreach (var user in Users)
			//{
			//	if (organisation.UserUIDs.Any(x => x == user.User.UID))
			//		user.IsChecked = true;
			//}
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