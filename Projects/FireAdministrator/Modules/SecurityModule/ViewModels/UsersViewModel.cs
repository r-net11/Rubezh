using System.Collections.ObjectModel;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UsersViewModel : RegionViewModel
    {
        public UsersViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            AddCommand = new RelayCommand(OnAdd);

            Users = new ObservableCollection<UserViewModel>();
            if (FiresecManager.SecurityConfiguration.Users != null)
            {
                foreach (var user in FiresecManager.SecurityConfiguration.Users)
                    Users.Add(new UserViewModel(user));
            }

            if (Users.Count > 0)
                SelectedUser = Users[0];
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

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var result = DialogBox.DialogBox.Show(string.Format("Вы уверенны, что хотите удалить пользователя \"{0}\" из списка", SelectedUser.User.Name),
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
                Users.Remove(SelectedUser);

                SecurityModule.HasChanges = true;
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
            if (ServiceFactory.UserDialogs.ShowModalWindow(userDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
                SelectedUser.User = userDetailsViewModel.User;
                FiresecManager.SecurityConfiguration.Users.Add(SelectedUser.User);

                SecurityModule.HasChanges = true;
            }
        }

        bool CanEdit()
        {
            return SelectedUser != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var userDetailsViewModel = new UserDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(userDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.Users.Add(userDetailsViewModel.User);
                Users.Add(new UserViewModel(userDetailsViewModel.User));

                SecurityModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var usersMenuViewModel = new UsersMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(usersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}