using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    class GuardConfigurationViewModel : DialogViewModel
    {
        void SaveConfiguration()
        {
            deviceUsers.Clear();
            foreach (var user in Users)
            {
                var userViewModel = new UserViewModel(new GuardUser());
                userViewModel.GuardUser.Name = user.GuardUser.Name;
                userViewModel.GuardUser.Password = user.GuardUser.Password;
                userViewModel.GuardUser.CanSetZone = user.GuardUser.CanSetZone;
                userViewModel.GuardUser.CanUnSetZone = user.GuardUser.CanUnSetZone;
                userViewModel.GuardUser.KeyTM = user.GuardUser.KeyTM;
                userViewModel.GuardUser.Zones = user.GuardUser.Zones;
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
        public GuardConfigurationViewModel(Device selectedDevice, ObservableCollection<UserViewModel> DeviceUsers)
        {
            Title = "Синхронизация охранных пользователей";
            deviceUsers = DeviceUsers;
            AcceptCommand = new RelayCommand(OnAccept);
            var result = FiresecManager.DeviceGetGuardUsersList(selectedDevice.UID);
            if (result.Result == null)
                return;
            string res = result.Result.ToString(); //3,104,8,20,12,6,64,64
            Users = new ObservableCollection<UserViewModel>();
            UserZones = new ObservableCollection<Zone>();//(FiresecManager.Zones);
            //string res = GuardViewModel.Userlist;
            if (res == null)
                return;
            int CountUsers = byte.Parse(res.ToString().Substring(0,3));
            for (int i = 0; i < CountUsers; i++)
            {
                User user = new User();
                var userViewModel = new UserViewModel(new GuardUser());
                var guardUser = userViewModel.GuardUser;
                guardUser.Id = i;
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
        }

        public RelayCommand AcceptCommand { get; private set; }
        void OnAccept()
        {
            SaveConfiguration();
        }
    }
}
