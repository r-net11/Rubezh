using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class UsersViewModel : RegionViewModel, IEditingViewModel
    {
        public UsersViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);

            Users = new ObservableCollection<UserViewModel>();
            foreach (var guardUser in FiresecManager.DeviceConfiguration.GuardUsers)
            {
                var guardUserViewModel = new UserViewModel(guardUser);
                Users.Add(guardUserViewModel);
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

        bool CanEditDelete()
        {
            return (SelectedUser != null);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.DeviceConfiguration.GuardUsers.Remove(SelectedUser.GuardUser);
            Users.Remove(SelectedUser);

            DevicesModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.GuardUser);
            if (ServiceFactory.UserDialogs.ShowModalWindow(userDetailsViewModel))
            {
                SelectedUser.GuardUser = userDetailsViewModel.GuardUser;
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var userDetailsViewModel = new UserDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(userDetailsViewModel))
            {
                FiresecManager.DeviceConfiguration.GuardUsers.Add(userDetailsViewModel.GuardUser);
                Users.Add(new UserViewModel(userDetailsViewModel.GuardUser));
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new UsersMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}