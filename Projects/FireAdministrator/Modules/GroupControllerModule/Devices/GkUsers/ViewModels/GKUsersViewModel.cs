using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using System.Collections.ObjectModel;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using RubezhAPI;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class GKUsersViewModel : DialogViewModel
	{
		Guid _deviceUID;

		public GKUsersViewModel(List<GKUser> deviceUsers, Guid deviceUID)
		{
			Title = "Пользователи ГК";
			RewriteUsersCommand = new RelayCommand(OnRewriteUsers);
			PreviousConflictCommand = new RelayCommand(OnPreviousConflict, CanPreviousConflict);
			NextConflictCommand = new RelayCommand(OnNextConflict, CanNextConflict);
			_deviceUID = deviceUID;
			var deviceDoorUIDs = GKManager.Doors
				.Where(x => (x.EnterDevice.GKParent != null && x.EnterDevice.GKParent.UID == _deviceUID) || 
					(x.ExitDevice.GKParent != null && x.ExitDevice.GKParent.UID == _deviceUID))
				.Select(x => x.UID).ToList();

			var dbUsers = new List<GKUser>();
			var getDbUsersResult = ClientManager.FiresecService.GetDbDeviceUsers(deviceUID, deviceDoorUIDs);
			if (getDbUsersResult.HasError)
				MessageBoxService.Show(getDbUsersResult.Error);
			else
				dbUsers.AddRange(getDbUsersResult.Result);

			deviceUsers = deviceUsers.Where(x => x.IsActive).ToList();
			var allUsers = UnionUsers(deviceUsers, dbUsers);
			InitializeCollections(allUsers, deviceUsers, dbUsers);
			CurrentIndex = 0;
		}

		GKUserViewModel _selectedDeviceUser;
		public GKUserViewModel SelectedDeviceUser
		{
			get { return _selectedDeviceUser; }
			set
			{
				_selectedDeviceUser = value;
				OnPropertyChanged(() => SelectedDeviceUser);
			}
		}
		
		public ObservableCollection<GKUserViewModel> DeviceUsers { get; private set; }

		GKUserViewModel _selectedDbUser;
		public GKUserViewModel SelectedDbUser
		{
			get { return _selectedDbUser; }
			set
			{
				_selectedDbUser = value;
				OnPropertyChanged(() => SelectedDbUser);
			}
		}
		
		public ObservableCollection<GKUserViewModel> DbUsers { get; private set; }

		public RelayCommand RewriteUsersCommand { get; private set; }
		void OnRewriteUsers()
		{
			var result = ClientManager.FiresecService.GKRewriteUsers(_deviceUID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка при перезаписи пользователей");
			}
			else
				Close();
		}

		int CurrentIndex
		{
			get 
			{ 
				return DeviceUsers.IndexOf(SelectedDeviceUser); 
			}
			set
			{
				if (DeviceUsers.Count > value)
				{
					SelectedDeviceUser = DeviceUsers[value];
					SelectedDbUser = DbUsers[value];
				}
			}
		}

		List<int> MismatchedIndexes { get { return DeviceUsers.Where(x => x.IsMissmatch).Select(x => DeviceUsers.IndexOf(x)).ToList(); } }

		public RelayCommand PreviousConflictCommand { get; private set; }
		void OnPreviousConflict()
		{
			CurrentIndex = MismatchedIndexes.LastOrDefault(x => x < CurrentIndex);
		}
		bool CanPreviousConflict()
		{
			return MismatchedIndexes.Any(x => x < CurrentIndex);
		}
		
		public RelayCommand NextConflictCommand { get; private set; }
		void OnNextConflict()
		{
			CurrentIndex = MismatchedIndexes.FirstOrDefault(x => x > CurrentIndex);
		}
		bool CanNextConflict()
		{
			return MismatchedIndexes.Any(x => x > CurrentIndex);
		}

		List<GKUser> UnionUsers(List<GKUser> deviceUsers, List<GKUser> dbUsers)
		{
			var result = new List<GKUser>();
			foreach (var deviceUser in deviceUsers)
			{
				var user = deviceUser.Clone();
				var dbUser = dbUsers.FirstOrDefault(x => x.Password == deviceUser.Password);
				if (dbUser != null)
					user.Descriptors.AddRange(dbUser.Descriptors);
				result.Add(user);
			}
			var dbUsersToAdd = dbUsers
				.Where(dbUser => 
					!result.Any(x => x.Password == dbUser.Password))
				.Select(x => x.Clone());
			result.AddRange(dbUsersToAdd);
			return result;
		}

		void InitializeCollections(List<GKUser> allUsers, List<GKUser> deviceUsers, List<GKUser> dbUsers)
		{
			DeviceUsers = new ObservableCollection<GKUserViewModel>();
			DbUsers = new ObservableCollection<GKUserViewModel>();
			foreach (var user in allUsers)
			{
				var deviceUser = deviceUsers.FirstOrDefault(x => x.Password == user.Password);
				var dbUser = dbUsers.FirstOrDefault(x => x.Password == user.Password);
				var deviceViewModel = new GKUserViewModel(deviceUser ?? dbUser, isDevice: true);
				var dbViewModel = new GKUserViewModel(dbUser ?? deviceUser, isDevice: false);
				
				if (deviceUser != null && dbUser == null)
				{
					deviceViewModel.IsPresent = true;
					dbViewModel.IsAbsent = true;
				}
				else if (deviceUser == null && dbUser != null)
				{
					deviceViewModel.IsAbsent = true;
					dbViewModel.IsPresent = true;
				}
				else if (user.Descriptors.Count > 0
					&& (user.Descriptors
							.Any(x => 
								!deviceUser.Descriptors.Any(y => y.DescriptorNo == x.DescriptorNo && y.ScheduleNo == x.ScheduleNo))
						|| user.Descriptors
							.Any(x => 
								!dbUser.Descriptors.Any(y => y.DescriptorNo == x.DescriptorNo && y.ScheduleNo == x.ScheduleNo))))
				{
					deviceViewModel.IsDescriptorMissmatch = true;
					dbViewModel.IsDescriptorMissmatch = true;
				}
				else if (deviceUser.Fio != dbUser.Fio ||
					deviceUser.IsActive != dbUser.IsActive ||
					deviceUser.UserType != dbUser.UserType ||
					deviceUser.GkLevel != dbUser.GkLevel ||
					deviceUser.GkLevelSchedule != dbUser.GkLevelSchedule)
				{
					deviceViewModel.IsNonStructureMissmatch = true;
					dbViewModel.IsNonStructureMissmatch = true;
				}
				DeviceUsers.Add(deviceViewModel);
				DbUsers.Add(dbViewModel);
			}
		}
	}
}