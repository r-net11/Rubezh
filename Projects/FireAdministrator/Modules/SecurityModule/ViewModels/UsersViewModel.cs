using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UsersViewModel : RegionViewModel
    {
        public UsersViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);
        }

        public void Initialize()
        {
            Users = new ObservableCollection<UserViewModel>();
            if (FiresecManager.SecurityConfiguration.Users != null)
                foreach (var user in FiresecManager.SecurityConfiguration.Users)
                {
                    Users.Add(new UserViewModel(user));
                }
        }

        public ObservableCollection<UserViewModel> Users { get; private set; }

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

        bool CanEditDelete()
        {
            return SelectedUser != null;
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
            Users.Remove(SelectedUser);
            SecurityModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var userDetailsViewModel = new UserDetailsViewModel();
            userDetailsViewModel.Initialize(SelectedUser.User);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(userDetailsViewModel);
            if (result)
            {
                SelectedUser.User = userDetailsViewModel.User;
                SecurityModule.HasChanges = true;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var userDetailsViewModel = new UserDetailsViewModel();
            userDetailsViewModel.Initialize();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(userDetailsViewModel);
            if (result)
            {
                FiresecManager.SecurityConfiguration.Users.Add(userDetailsViewModel.User);
                var userViewModel = new UserViewModel(userDetailsViewModel.User);
                Users.Add(userViewModel);
                SecurityModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var usersMenuViewModel = new UsersMenuViewModel(AddCommand, DeleteCommand, EditCommand);
            ServiceFactory.Layout.ShowMenu(usersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
