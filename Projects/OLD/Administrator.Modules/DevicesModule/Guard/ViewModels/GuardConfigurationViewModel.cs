using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Guard;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	class GuardConfigurationViewModel : SaveCancelDialogViewModel
	{
		ObservableCollection<UserViewModel> deviceUsersViewModel = new ObservableCollection<UserViewModel>();
		ObservableCollection<UserViewModel> availableUsersViewModel = new ObservableCollection<UserViewModel>();
		ObservableCollection<Zone> userZonesViewModel = new ObservableCollection<Zone>();
		ObservableCollection<Zone> deviceZonesViewModel = new ObservableCollection<Zone>();
		Device SelectedDevice;

		public GuardConfigurationViewModel(Device selectedDevice,
			ObservableCollection<UserViewModel> deviceUsers,
			ObservableCollection<UserViewModel> availableUsers,
			ObservableCollection<Zone> userZones,
			ObservableCollection<Zone> deviceZones)
		{
			Title = "Список охранных пользователей прибора: " + selectedDevice.Driver.ShortName;
			SaveCaption = "Применить";
			deviceUsersViewModel = deviceUsers;
			availableUsersViewModel = availableUsers;
			userZonesViewModel = userZones;
			deviceZonesViewModel = deviceZones;
			SelectedDevice = selectedDevice;

			Users = new ObservableCollection<UserViewModel>();

			//if (FiresecManager.IsFS2Enabled)
			//{
			//	FS2DeviceGetGuardUserListHelper.Run(SelectedDevice);
			//	var guardUsers = FS2DeviceGetGuardUserListHelper.Result;
			//	if (guardUsers != null)
			//	{
			//		for (int i = 0; i < guardUsers.Count; i++)
			//		{
			//			var user = new User();
			//			guardUsers[i].Id = i + 1;
			//			var userViewModel = new UserViewModel(guardUsers[i]);
			//			Users.Add(userViewModel);
			//		}
			//	}
			//}
			//else
			{
				DeviceGetGuardUserListHelper.Run(SelectedDevice);
				var result = DeviceGetGuardUserListHelper.Result;
				if (result != null)
				{
					int CountUsers = byte.Parse(result.ToString().Substring(0, 3));
					for (int i = 0; i < CountUsers; i++)
					{
						var guardUser = new GuardUser();
						guardUser.Id = i + 1;
						guardUser.Name = result.Substring(174 * i + 115, 20);
						guardUser.Password = result.Substring(174 * i + 147, 6);
						var indexOfF = guardUser.Password.IndexOf('F');
						if (indexOfF >= 0)
							guardUser.Password = guardUser.Password.Remove(indexOfF);
						guardUser.CanUnSetZone = (result[174 * i + 107] == '1');
						guardUser.CanSetZone = (result[174 * i + 108] == '1');
						guardUser.KeyTM = result.Substring(174 * i + 135, 12);
						for (int j = 0; j < 64; j++)
						{
							if (result.Substring(174 * i + 153, 64)[j] == '1')
							{
								Zone zone = FiresecManager.Zones.FirstOrDefault(x => FiresecManager.FiresecConfiguration.GetZoneLocalSecNo(x) == j + 1);
								if (zone != null)
									guardUser.ZoneUIDs.Add(zone.UID);
							}
						}
						var userViewModel = new UserViewModel(guardUser);
						Users.Add(userViewModel);
					}
				}
			}

			if (Users.Count > 0)
				SelectedUser = Users.FirstOrDefault();
		}

		public ObservableCollection<GuardUser> DeviceUsers { get; private set; }

		ObservableCollection<UserViewModel> _users;
		public ObservableCollection<UserViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged(() => Users);
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
				OnPropertyChanged(() => SelectedUser);
			}
		}

		ObservableCollection<Zone> _userZones;
		public ObservableCollection<Zone> UserZones
		{
			get { return _userZones; }
			set
			{
				_userZones = value;
				OnPropertyChanged(() => UserZones);
			}
		}
		public void GetUserZones(GuardUser guardUser)
		{
			UserZones = new ObservableCollection<Zone>();
			foreach (var ZoneUID in guardUser.ZoneUIDs)
			{
				var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == ZoneUID);
				if (zone != null)
				{
					UserZones.Add(zone);
				}
			}
		}

		void SaveConfiguration()
		{
			deviceUsersViewModel.Clear();
			availableUsersViewModel.Clear();
			userZonesViewModel.Clear();
			FiresecManager.GuardUsers.Clear();
			foreach (var user in Users)
			{
				FiresecManager.GuardUsers.Add(user.GuardUser);
				var zoneUIDs = new List<Guid>();
				foreach (var ZoneUID in user.GuardUser.ZoneUIDs)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == ZoneUID);
					if (zone != null)
					{
						zoneUIDs.Add(zone.UID);
						userZonesViewModel.Add(zone);
						deviceZonesViewModel.Remove(zone);
					}
				}
				user.GuardUser.ZoneUIDs = zoneUIDs;
				deviceUsersViewModel.Add(user);
			}
		}

		protected override bool Save()
		{
			SaveConfiguration();
			ServiceFactory.SaveService.FSChanged = true;
			return base.Save();
		}
	}
}