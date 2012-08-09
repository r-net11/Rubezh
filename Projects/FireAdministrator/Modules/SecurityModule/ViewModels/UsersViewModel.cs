using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SecurityModule.Views;

namespace SecurityModule.ViewModels
{
    public class UsersViewModel : ViewPartViewModel, IEditingViewModel
    {
        public UsersViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            AddCommand = new RelayCommand(OnAdd);
		}

		public void Initialize()
		{
			Users = new ObservableCollection<UserViewModel>();
			if (FiresecManager.SecurityConfiguration.Users != null)
			{
				foreach (var user in FiresecManager.SecurityConfiguration.Users)
					Users.Add(new UserViewModel(user));
			}
			SelectedUser = Users.FirstOrDefault();
		}

		ObservableCollection<UserViewModel> _users;
		public ObservableCollection<UserViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged("Users");
			}
		}

        UserViewModel _selectedUser;
        public UserViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var userDetailsViewModel = new UserDetailsViewModel();
			if (DialogService.ShowModalWindow(userDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.Users.Add(userDetailsViewModel.User);
                Users.Add(new UserViewModel(userDetailsViewModel.User));

                ServiceFactory.SaveService.SecurityChanged = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var result = MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить пользователя \"{0}\" из списка", SelectedUser.User.Name));
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
                Users.Remove(SelectedUser);

                ServiceFactory.SaveService.SecurityChanged = true;
            }
        }

        bool CanDelete()
        {
            return SelectedUser != null && SelectedUser.User != FiresecManager.CurrentUser;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.User);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
                SelectedUser.User = userDetailsViewModel.User;
                FiresecManager.SecurityConfiguration.Users.Add(SelectedUser.User);

                ServiceFactory.SaveService.SecurityChanged = true;
            }
        }

        bool CanEdit()
        {
            return SelectedUser != null;
        }

        public override void OnShow()
        {
            var usersMenuViewModel = new UsersMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(usersMenuViewModel);

			if (UsersMenuView.Current != null)
				UsersMenuView.Current.AcceptKeyboard = true;
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);

			if (UsersMenuView.Current != null)
				UsersMenuView.Current.AcceptKeyboard = false;
        }
    }
}