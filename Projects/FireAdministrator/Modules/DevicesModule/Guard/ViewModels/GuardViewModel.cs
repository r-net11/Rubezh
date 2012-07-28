using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using DevicesModule.Views;

namespace DevicesModule.ViewModels
{
	public class GuardViewModel : ViewPartViewModel
	{
		public GuardViewModel()
		{
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			AddCommand = new RelayCommand(OnAdd);

			AddUserCommand = new RelayCommand(OnAddUser, CanAddUser);
			RemoveUserCommand = new RelayCommand(OnRemoveUser, CanRemoveUser);
			AddZoneCommand = new RelayCommand(OnAddZone, CanAddZone);
			RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemoveZone);
			ShowSynchronizationCommand = new RelayCommand(OnShowSynchronization, CanShowSynchronization);
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
			Devices.Clear();
			SelectedDevice = null;

			foreach (var device in FiresecManager.DeviceConfiguration.Devices)
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
				var deviceZones = new List<int>();
				foreach (var device in SelectedDevice.Children)
				{
					if (device.Driver.DriverType == DriverType.AM1_O)
						if (device.ZoneNo.HasValue)
							deviceZones.Add(device.ZoneNo.Value);
				}

				foreach (var guardUser in FiresecManager.DeviceConfiguration.GuardUsers)
				{
					if ((guardUser.DeviceUID == SelectedDevice.UID) || guardUser.Zones.Any(x => deviceZones.Contains(x)))
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
				var deviceZones = new List<int>();
				foreach (var device in SelectedDevice.Children)
				{
					if (device.Driver.DriverType == DriverType.AM1_O)
						if (device.ZoneNo.HasValue)
							deviceZones.Add(device.ZoneNo.Value);
				}

				foreach (var zoneNo in deviceZones)
				{
					var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
					if ((SelectedDeviceUser != null) && (SelectedDeviceUser.GuardUser.Zones.Contains(zoneNo)))
						UserZones.Add(zone);
					else
						DeviceZones.Add(zone);
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
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return (SelectedAvailableUser != null);
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecManager.DeviceConfiguration.GuardUsers.Remove(SelectedAvailableUser.GuardUser);
			AvailableUsers.Remove(SelectedAvailableUser);
			ServiceFactory.SaveService.DevicesChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var userDetailsViewModel = new UserDetailsViewModel(SelectedAvailableUser.GuardUser);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				SelectedAvailableUser.GuardUser = userDetailsViewModel.GuardUser;
				ServiceFactory.SaveService.DevicesChanged = true;
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
					ServiceFactory.SaveService.DevicesChanged = true;
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
				FiresecManager.DeviceConfiguration.GuardUsers.Add(userDetailsViewModel.GuardUser);
				var userViewModel = new UserViewModel(userDetailsViewModel.GuardUser);
				AvailableUsers.Add(userViewModel);
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		bool CanAddUser()
		{
			return SelectedAvailableUser != null;
		}

		public RelayCommand AddUserCommand { get; private set; }
		void OnAddUser()
		{
			foreach (var guardUser in FiresecManager.DeviceConfiguration.GuardUsers)
			{
				if (guardUser.DeviceUID == SelectedDevice.UID)
					guardUser.DeviceUID = Guid.Empty;
			}
			SelectedAvailableUser.GuardUser.DeviceUID = SelectedDevice.UID;

			DeviceUsers.Add(SelectedAvailableUser);
			AvailableUsers.Remove(SelectedAvailableUser);

			UpdateUsersSelectation();
			ServiceFactory.SaveService.DevicesChanged = true;
		}

		bool CanRemoveUser()
		{
			return SelectedDeviceUser != null;
		}

		public RelayCommand RemoveUserCommand { get; private set; }
		void OnRemoveUser()
		{
			SelectedDeviceUser.GuardUser.DeviceUID = Guid.Empty;
			SelectedDeviceUser.GuardUser.Zones.Clear();

			AvailableUsers.Add(SelectedDeviceUser);
			DeviceUsers.Remove(SelectedDeviceUser);

			UpdateUsersSelectation();
			ServiceFactory.SaveService.DevicesChanged = true;
		}

		bool CanAddZone()
		{
			return ((SelectedDeviceUser != null) && (SelectedDeviceZone != null));
		}

		public RelayCommand AddZoneCommand { get; private set; }
		void OnAddZone()
		{
			SelectedDeviceUser.GuardUser.Zones.Add(SelectedDeviceZone.No);
			UserZones.Add(SelectedDeviceZone);
			DeviceZones.Remove(SelectedDeviceZone);

			UpdateZonesSelectation();
			ServiceFactory.SaveService.DevicesChanged = true;
		}

		bool CanRemoveZone()
		{
			return SelectedUserZone != null;
		}

		public RelayCommand RemoveZoneCommand { get; private set; }
		void OnRemoveZone()
		{
			SelectedDeviceUser.GuardUser.Zones.Add(SelectedUserZone.No);
			DeviceZones.Add(SelectedUserZone);
			UserZones.Remove(SelectedUserZone);

			UpdateZonesSelectation();
			ServiceFactory.SaveService.DevicesChanged = true;
		}

		public override void OnShow()
		{
			FiresecManager.DeviceConfiguration.UpdateGuardConfiguration();
			ServiceFactory.Layout.ShowMenu(new GuardMenuViewModel(this));
			InitializeDevices();

			if (GuardMenuView.Current != null)
				GuardMenuView.Current.AcceptKeyboard = true;
		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);

			if (GuardMenuView.Current != null)
				GuardMenuView.Current.AcceptKeyboard = false;
		}
	}
}