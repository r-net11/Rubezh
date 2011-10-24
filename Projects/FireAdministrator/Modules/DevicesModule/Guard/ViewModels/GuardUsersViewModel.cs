using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardUsersViewModel : RegionViewModel
    {
        public GuardUsersViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);
        }

        public void Initialize()
        {
            GuardUsers = new ObservableCollection<GuardUserViewModel>();
            foreach (var guardUser in FiresecManager.DeviceConfiguration.GuardUsers)
            {
                var guardUserViewModel = new GuardUserViewModel(guardUser);
                GuardUsers.Add(guardUserViewModel);
            }

            if (GuardUsers.Count > 0)
                SelectedGuardUser = GuardUsers[0];
        }

        public ObservableCollection<GuardUserViewModel> GuardUsers { get; private set; }

        GuardUserViewModel _selectedGuardUser;
        public GuardUserViewModel SelectedGuardUser
        {
            get { return _selectedGuardUser; }
            set
            {
                _selectedGuardUser = value;
                OnPropertyChanged("SelectedGuardUser");
            }
        }

        bool CanEditDelete()
        {
            return (SelectedGuardUser != null);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.DeviceConfiguration.GuardUsers.Remove(SelectedGuardUser.GuardUser);
            GuardUsers.Remove(SelectedGuardUser);
                DevicesModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var guardUserDetailsViewModel = new GuardUserDetailsViewModel();
            guardUserDetailsViewModel.Initialize(SelectedGuardUser.GuardUser);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(guardUserDetailsViewModel);
            if (result)
            {
                SelectedGuardUser.GuardUser = guardUserDetailsViewModel.GuardUser;
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var guardUserDetailsViewModel = new GuardUserDetailsViewModel();
            guardUserDetailsViewModel.Initialize();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(guardUserDetailsViewModel);
            if (result)
            {
                FiresecManager.DeviceConfiguration.GuardUsers.Add(guardUserDetailsViewModel.GuardUser);
                var guardUserViewModel = new GuardUserViewModel(guardUserDetailsViewModel.GuardUser);
                GuardUsers.Add(guardUserViewModel);
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var guardUsersMenuViewModel = new GuardUsersMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(guardUsersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
