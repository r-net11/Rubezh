using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;

namespace DevicesModule.ViewModels
{
    public class GuardViewModel : MenuViewPartViewModel
    {
        public GuardViewModel()
        {
            Menu = new GuardMenuViewModel(this);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);

            ReadGuardUserCommand = new RelayCommand(OnReadGuardUser);
            WriteGuardUserCommand = new RelayCommand(OnWriteGuardUser, CanWriteGuardUser);
            AddUserCommand = new RelayCommand(OnAddUser, CanAddUser);
            RemoveUserCommand = new RelayCommand(OnRemoveUser, CanRemoveUser);
            AddZoneCommand = new RelayCommand(OnAddZone, CanAddZone);
            RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemoveZone);
            ShowSynchronizationCommand = new RelayCommand(OnShowSynchronization, CanShowSynchronization);
        }

        public class User
        {
            public bool[] Attr = new bool[8];
            public char[] Name = new char[20];
            public char[] KeyTM = new char[12];
            public char[] Pass = new char[6];
            public bool[] Zones = new bool[64];
            public bool[] ReservedZones = new bool[64];

            public User()
            {
                Name = "".ToCharArray();
                KeyTM = "".ToCharArray();
                Pass = "".ToCharArray();
            }
        }

        bool[] UsersMask = new bool[104];
        List<User> Users;

        public RelayCommand ReadGuardUserCommand { get; private set; }
        void OnReadGuardUser()
        {
            GuardConfigurationViewModel guardConfigurationViewModel = new GuardConfigurationViewModel(SelectedDevice, DeviceUsers, AvailableUsers, UserZones, DeviceZones);
            DialogService.ShowModalWindow(guardConfigurationViewModel);
            UpdateZonesSelectation();
            UpdateUsersSelectation();
        }
        string AddCharsToLen(string str, int Len, char ch)
        {
            int i;
            var result = str;
            if (Len > str.Length)
                for (i = str.Length + 1; i <= Len; i++)
                    result = result + ch;
            return result;
        }

        User user = new User();

        string CodeDateToTranslate()
        {
			Byte CountUsers = 0;
			List<User> Users = new List<User>(80);
			foreach (var deviceUsers in DeviceUsers)
			{
				var guardUser = deviceUsers.GuardUser;
				User user1 = new User();
				user1.Attr[0] = guardUser.CanUnSetZone;
				user1.Attr[1] = guardUser.CanSetZone;
				user1.Name = guardUser.Name.ToCharArray();
				user1.Pass = guardUser.Password!=null?guardUser.Password.ToCharArray():"".ToCharArray();

				foreach (var userZone in guardUser.ZoneUIDs)
				{
					var localNo = FiresecManager.FiresecConfiguration.GetZoneLocalSecNo(FiresecManager.Zones.FirstOrDefault(x=>x.UID == userZone)) - 1;
					user1.Zones[localNo] = true;
				}

				UsersMask[Users.Count] = true;
				Users.Add(user1);
				CountUsers++;
			}

            //TEST MODE
            #region 
			/*
            List<User> Users = new List<User>(80);
            Byte CountUsers = 0;
            User user1 = new User();
            user1.Attr[0] = true;
            user1.Attr[1] = true;
            user1.Name = "man".ToCharArray();
            user1.Pass = "123".ToCharArray();
            user1.Zones[0] = true;
            user1.Zones[2] = true;
            UsersMask[Users.Count] = true;
            Users.Add(user1);
            CountUsers++;

            User user2 = new User();
            user2.Attr[0] = false;
            user2.Attr[1] = true;
            user2.Name = "woman".ToCharArray();
            user2.Pass = "456".ToCharArray();
            user2.Zones[1] = true;
            UsersMask[Users.Count] = true;
            Users.Add(user2);
            CountUsers++;
			*/
            #endregion

            string s;
            CountUsers.ToString();
            var DeviceGuardData = string.Format("{0,3}", CountUsers.ToString()).Replace(' ', '0'); //3
            foreach (bool b in UsersMask)
                DeviceGuardData = b == true ? DeviceGuardData + "1" : DeviceGuardData + "0"; //104
            for (int i = 0; i < Users.Capacity; i++) //x80
            {
                if (i >= CountUsers)
                    Users.Add(user);
                foreach (bool b in Users[i].Attr)
                    DeviceGuardData = b == true ? DeviceGuardData + "1" : DeviceGuardData + "0"; //8
                s = new string(Users[i].Name);
                s = AddCharsToLen(s, 20, ' '); //20
                DeviceGuardData = DeviceGuardData + s;
                s = new string(Users[i].KeyTM);
                s = AddCharsToLen(s, 12, '0'); //12
                DeviceGuardData = DeviceGuardData + s;
                s = new string(Users[i].Pass);
                s = AddCharsToLen(s, 6, 'F'); //6
                DeviceGuardData = DeviceGuardData + s;
                foreach (bool b in Users[i].Zones)
                    DeviceGuardData = b == true ? DeviceGuardData + "1" : DeviceGuardData + "0"; //64
                foreach (bool b in Users[i].ReservedZones)
                    DeviceGuardData = b == true ? DeviceGuardData + "1" : DeviceGuardData + "0"; //64
            }
            return DeviceGuardData;
        }

        public RelayCommand WriteGuardUserCommand { get; private set; }
        void OnWriteGuardUser()
        {
            if (SelectedDevice != null)
            {
				if (DeviceUsers.Count == 0)
				{
                    var result = MessageBoxService.ShowConfirmation("Вы действительно хотите стереть всех охранных пользователей из устройства?", "Не выбрано ни одного пользователя");
					if (result == MessageBoxResult.No)
						return;
				}
                ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, "Запись охранной конфигурации");
            }
        }
        OperationResult<bool> _operationResult;
        void OnPropgress()
        {
            var userlist = CodeDateToTranslate();
            _operationResult = FiresecManager.DeviceSetGuardUsersList(SelectedDevice, userlist);
        }
        void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно", "Firesec");
        }

        public static string Userlist { get; set; }
        bool CanWriteGuardUser()
        {
			foreach (var deviceUser in DeviceUsers)
				if (deviceUser.GuardUser.ZoneUIDs.Count == 0)
					return false;
			return true;
        }

        public void Initialize()
        {
            Devices = new ObservableCollection<Device>();
            DeviceUsers = new ObservableCollection<UserViewModel>();
            AvailableUsers = new ObservableCollection<UserViewModel>();
            UserZones = new ObservableCollection<Zone>();
            DeviceZones = new ObservableCollection<Zone>();

            InitializeDevices();
        }

        void InitializeDevices()
        {
            FiresecManager.FiresecConfiguration.UpateGuardZoneSecPanelUID();
            Devices.Clear();
            SelectedDevice = null;

            foreach (var device in FiresecManager.Devices)
            {
                if ((device.Driver.DriverType == DriverType.USB_Rubezh_2OP) || (device.Driver.DriverType == DriverType.Rubezh_2OP))
                    Devices.Add(device);
            }
            SelectedDevice = Devices.FirstOrDefault();
        }

        void InitializeUsers()
        {
            DeviceUsers.Clear();
            AvailableUsers.Clear();
            SelectedDeviceUser = null;
            SelectedAvailableUser = null;

            if (SelectedDevice != null)
            {
                var deviceZones = new List<Guid>();
                foreach (var device in SelectedDevice.Children)
                {
                    if (device.Driver.DriverType == DriverType.AM1_O)
                        if (device.ZoneUID != Guid.Empty)
                            deviceZones.Add(device.ZoneUID);
                }

                foreach (var guardUser in FiresecManager.GuardUsers)
                {
					if ((guardUser.DeviceUID != Guid.Empty && guardUser.DeviceUID == SelectedDevice.UID) || guardUser.ZoneUIDs.Any(x => deviceZones.Contains(x)))
                        DeviceUsers.Add(new UserViewModel(guardUser));
                    else
                        AvailableUsers.Add(new UserViewModel(guardUser));
                }
            }

            UpdateUsersSelectation();
        }

        void InitializeZones()
        {
            UserZones.Clear();
            DeviceZones.Clear();
            SelectedUserZone = null;
            SelectedDeviceZone = null;

            if (SelectedDevice != null)
            {
                foreach (var zone in FiresecManager.Zones)
                {
                    if (zone.SecPanelUID == SelectedDevice.UID)
                    {
                        if ((SelectedDeviceUser != null) && (SelectedDeviceUser.GuardUser.ZoneUIDs.Contains(zone.UID)))
                            UserZones.Add(zone);
                        else
                            DeviceZones.Add(zone);
                    }
                }

                UpdateZonesSelectation();
            }
        }

        void UpdateUsersSelectation()
        {
            SelectedDeviceUser = DeviceUsers.FirstOrDefault();
            SelectedAvailableUser = AvailableUsers.FirstOrDefault();
        }

        void UpdateZonesSelectation()
        {
            SelectedUserZone = UserZones.FirstOrDefault();
            SelectedDeviceZone = DeviceZones.FirstOrDefault();
        }

        ObservableCollection<Device> _devices;
        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                InitializeUsers();
                OnPropertyChanged("SelectedDevice");
            }
        }

        ObservableCollection<UserViewModel> _deviceUsers;
        public ObservableCollection<UserViewModel> DeviceUsers
        {
            get { return _deviceUsers; }
            set
            {
                _deviceUsers = value;
                OnPropertyChanged("DeviceUsers");
            }
        }

        UserViewModel _selectedDeviceUser;
        public UserViewModel SelectedDeviceUser
        {
            get { return _selectedDeviceUser; }
            set
            {
                _selectedDeviceUser = value;
                InitializeZones();
                OnPropertyChanged("SelectedDeviceUser");
            }
        }

        ObservableCollection<UserViewModel> _availableUsers;
        public ObservableCollection<UserViewModel> AvailableUsers
        {
            get { return _availableUsers; }
            set
            {
                _availableUsers = value;
                OnPropertyChanged("AvailableUsers");
            }
        }

        UserViewModel _selectedAvailableUser;
        public UserViewModel SelectedAvailableUser
        {
            get { return _selectedAvailableUser; }
            set
            {
                _selectedAvailableUser = value;
                OnPropertyChanged("SelectedAvailableUser");
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

        Zone _selectedUserZone;
        public Zone SelectedUserZone
        {
            get { return _selectedUserZone; }
            set
            {
                _selectedUserZone = value;
                OnPropertyChanged("SelectedUserZone");
            }
        }

        ObservableCollection<Zone> _deviceZones;
        public ObservableCollection<Zone> DeviceZones
        {
            get { return _deviceZones; }
            set
            {
                _deviceZones = value;
                OnPropertyChanged("DeviceZones");
            }
        }

        Zone _selectedDeviceZone;
        public Zone SelectedDeviceZone
        {
            get { return _selectedDeviceZone; }
            set
            {
                _selectedDeviceZone = value;
                OnPropertyChanged("SelectedDeviceZone");
            }
        }

        public bool CanShowSynchronization()
        {
            return SelectedDevice != null;
        }

        public RelayCommand ShowSynchronizationCommand { get; private set; }
        void OnShowSynchronization()
        {
            var guardSynchronizationViewModel = new GuardSynchronizationViewModel(SelectedDevice);
            if (DialogService.ShowModalWindow(guardSynchronizationViewModel))
            {
                ServiceFactory.SaveService.FSChanged = true;
            }
        }

        bool CanEditDelete()
        {
            return (SelectedAvailableUser != null);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.GuardUsers.Remove(SelectedAvailableUser.GuardUser);
            AvailableUsers.Remove(SelectedAvailableUser);
            ServiceFactory.SaveService.FSChanged = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var userDetailsViewModel = new UserDetailsViewModel(SelectedAvailableUser.GuardUser);
            if (DialogService.ShowModalWindow(userDetailsViewModel))
            {
                SelectedAvailableUser.GuardUser = userDetailsViewModel.GuardUser;
                ServiceFactory.SaveService.FSChanged = true;
            }
        }

        public void EditDeviceUser()
        {
            if (SelectedDeviceUser != null)
            {
                var userDetailsViewModel = new UserDetailsViewModel(SelectedDeviceUser.GuardUser);
                if (DialogService.ShowModalWindow(userDetailsViewModel))
                {
                    SelectedDeviceUser.GuardUser = userDetailsViewModel.GuardUser;
                    ServiceFactory.SaveService.FSChanged = true;
                }
            }
        }

        public void EditUser()
        {
            if (SelectedAvailableUser != null)
            {
                OnEdit();
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var userDetailsViewModel = new UserDetailsViewModel();
            if (DialogService.ShowModalWindow(userDetailsViewModel))
            {
                FiresecManager.GuardUsers.Add(userDetailsViewModel.GuardUser);
                var userViewModel = new UserViewModel(userDetailsViewModel.GuardUser);
                AvailableUsers.Add(userViewModel);
                ServiceFactory.SaveService.FSChanged = true;
            }
        }

        bool CanAddUser()
        {
            return SelectedAvailableUser != null;
        }

        public RelayCommand AddUserCommand { get; private set; }
        void OnAddUser()
        {
            foreach (var guardUser in FiresecManager.GuardUsers)
            {
                if (guardUser.DeviceUID == SelectedDevice.UID)
                    guardUser.DeviceUID = Guid.Empty;
            }
            SelectedAvailableUser.GuardUser.DeviceUID = SelectedDevice.UID;

            DeviceUsers.Add(SelectedAvailableUser);
            AvailableUsers.Remove(SelectedAvailableUser);

            UpdateUsersSelectation();
            ServiceFactory.SaveService.FSChanged = true;
        }

        bool CanRemoveUser()
        {
            return SelectedDeviceUser != null;
        }

        public RelayCommand RemoveUserCommand { get; private set; }
        void OnRemoveUser()
        {
            SelectedDeviceUser.GuardUser.DeviceUID = Guid.Empty;
            SelectedDeviceUser.GuardUser.ZoneUIDs.Clear();

            AvailableUsers.Add(SelectedDeviceUser);
            DeviceUsers.Remove(SelectedDeviceUser);

            UpdateUsersSelectation();
            ServiceFactory.SaveService.FSChanged = true;
        }

        bool CanAddZone()
        {
            return ((SelectedDeviceUser != null) && (SelectedDeviceZone != null));
        }

        public RelayCommand AddZoneCommand { get; private set; }
        void OnAddZone()
        {
            SelectedDeviceUser.GuardUser.ZoneUIDs.Add(SelectedDeviceZone.UID);
            UserZones.Add(SelectedDeviceZone);
            DeviceZones.Remove(SelectedDeviceZone);

            UpdateZonesSelectation();
            ServiceFactory.SaveService.FSChanged = true;
        }

        bool CanRemoveZone()
        {
            return SelectedUserZone != null;
        }

        public RelayCommand RemoveZoneCommand { get; private set; }
        void OnRemoveZone()
        {
            SelectedDeviceUser.GuardUser.ZoneUIDs.Remove(SelectedUserZone.UID);
            DeviceZones.Add(SelectedUserZone);
            UserZones.Remove(SelectedUserZone);

            UpdateZonesSelectation();
            ServiceFactory.SaveService.FSChanged = true;
        }

        public override void OnShow()
        {
            FiresecManager.FiresecConfiguration.DeviceConfiguration.UpdateGuardConfiguration();
            base.OnShow();
            InitializeDevices();
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }
}