using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
    class GuardConfigurationViewModel : SaveCancelDialogViewModel
    {
        void SaveConfiguration()
        {
            deviceUsers.Clear();
            availableUsers.Clear();
            userZones.Clear();
            FiresecManager.GuardUsers.Clear();
            foreach (var user in Users)
            {
                FiresecManager.GuardUsers.Add(user.GuardUser);
                List<int> zones = new List<int>();
                foreach (int localNo in user.GuardUser.Zones)
                {
                    Zone zone = FiresecManager.Zones.FirstOrDefault(
                        x => FiresecManager.FiresecConfiguration.GetZoneLocalSecNo(x) == localNo);
                    zones.Add(zone.No);
                    userZones.Add(zone);
                    deviceZones.Remove(zone);
                }
                user.GuardUser.Zones = zones;
                deviceUsers.Add(user);
            }
        }
        public ObservableCollection<GuardUser> DeviceUsers { get; private set; }

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
                GetUserZones(_selectedUser.GuardUser);
                OnPropertyChanged("SelectedUser");
            }
        }

        ObservableCollection<Zone> _userZones;
        public ObservableCollection<Zone> UserZones
        {
            get { return _userZones; }
            set
            {
                _userZones = value;
                OnPropertyChanged("UserZones");
            }
        }
        public void GetUserZones(GuardUser guardUser)
        {
            UserZones = new ObservableCollection<Zone>();
            foreach (int localNo in guardUser.Zones)
            {
                UserZones.Add(FiresecManager.Zones.FirstOrDefault(
                                x => FiresecManager.FiresecConfiguration.GetZoneLocalSecNo(x) == localNo));
            }
        }
        ObservableCollection<UserViewModel> deviceUsers = new ObservableCollection<UserViewModel>();
        ObservableCollection<UserViewModel> availableUsers = new ObservableCollection<UserViewModel>();
        ObservableCollection<Zone> userZones = new ObservableCollection<Zone>();
        ObservableCollection<Zone> deviceZones = new ObservableCollection<Zone>();
        FiresecAPI.OperationResult<string> result;
        private Device SelectedDevice;
        public GuardConfigurationViewModel(Device selectedDevice, ObservableCollection<UserViewModel> DeviceUsers, ObservableCollection<UserViewModel> AvailableUsers, ObservableCollection<Zone> UserZones, ObservableCollection<Zone> DeviceZones)
        {
            Title = "Синхронизация охранных пользователей";
            SaveCaption = "Применить";
            deviceUsers = DeviceUsers;
            availableUsers = AvailableUsers;
            userZones = UserZones;
            deviceZones = DeviceZones;
            SelectedDevice = selectedDevice;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, "Чтение охранной конфигурации");
            if (result.Result == null)
                return;
            string res = result.Result; //3,104,8,20,12,6,64,64
            Users = new ObservableCollection<UserViewModel>();
            int CountUsers = byte.Parse(res.ToString().Substring(0,3));
            for (int i = 0; i < CountUsers; i++)
            {
                User user = new User();
                var userViewModel = new UserViewModel(new GuardUser());
                var guardUser = userViewModel.GuardUser;
                guardUser.Id = i + 1;
                guardUser.Name = res.Substring(174*i + 115, 20);
                guardUser.Password = res.Substring(174*i + 147, 6);
                guardUser.Password = guardUser.Password.Remove(guardUser.Password.IndexOf('F'));
                guardUser.CanUnSetZone = (res[174 * i + 107] == '1');
                guardUser.CanSetZone = (res[174 * i + 108] == '1');
                guardUser.KeyTM = res.Substring(174 * i + 135, 12);
                for (int j = 0; j < 64; j++)
                {
                    if (res.Substring(174*i + 153, 64)[j] == '1')
                    {
                        Zone zone =
                            FiresecManager.Zones.FirstOrDefault(
                                x => FiresecManager.FiresecConfiguration.GetZoneLocalSecNo(x) == j+1);
                        if (zone != null)
                            guardUser.Zones.Add(j+1);
                    }
                }
                Users.Add(userViewModel);
            }
            if (Users.Count > 0)
                SelectedUser = Users.FirstOrDefault();
        }
        void OnPropgress()
        {
            result = FiresecManager.DeviceGetGuardUsersList(SelectedDevice);
        }
        void OnCompleted()
        {
            if (result.HasError)
            {
                MessageBoxService.ShowError(result.Error, "Ошибка при выполнении операции");
                return;
            }
        }
        protected override bool Save()
        {
            SaveConfiguration();
            return base.Save();
        }
    }
}
