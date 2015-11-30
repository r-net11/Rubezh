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

namespace GKModule.ViewModels
{
	public class GKUsersViewModel : DialogViewModel
	{
		Guid _deviceUID;

		public GKUsersViewModel(List<GKUser> deviceUsers, Guid deviceUID)
		{
			Title = "Пользователи ГК";
			RewriteUsersCommand = new RelayCommand(OnRewriteUsers);
			
			_deviceUID = deviceUID;
			var deviceDoorUIDs = GKManager.Doors
				.Where(x => x.EnterDevice.GKParent.UID == _deviceUID || x.ExitDevice.GKParent.UID == _deviceUID)
				.Select(x => x.UID).ToList();

			var dbUsers = new List<GKUser>();
			var getDbUsersResult = ClientManager.FiresecService.GetDbDeviceUsers(deviceUID, deviceDoorUIDs);
			if (getDbUsersResult.HasError)
				MessageBoxService.Show(getDbUsersResult.Error);
			else
				dbUsers.AddRange(getDbUsersResult.Result);

			var allUsers = UnionUsers(deviceUsers, dbUsers);
			DeviceUsers = CreateCollection(allUsers, deviceUsers, dbUsers, isDevice: true);
			DbUsers = CreateCollection(allUsers, dbUsers, deviceUsers, isDevice: false);
		}

		public ObservableCollection<GKUserViewModel> DeviceUsers { get; private set; }

		public ObservableCollection<GKUserViewModel> DbUsers { get; private set; }

		public RelayCommand RewriteUsersCommand { get; private set; }
		void OnRewriteUsers()
		{
			var result = ClientManager.FiresecService.GKRewriteUsers(_deviceUID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка при перезаписи пользователей");
			}
		}

		List<GKUser> UnionUsers(List<GKUser> deviceUsers, List<GKUser> dbUsers)
		{
			var result = new List<GKUser>(deviceUsers);
			result.AddRange(dbUsers.Where(dbUser => !result.Any(deviceUser => dbUser.Password == deviceUser.Password)));
			return result;
		}

		ObservableCollection<GKUserViewModel> CreateCollection(List<GKUser> allUsers, List<GKUser> users, List<GKUser> otherUsers, bool isDevice)
		{
			var result = new ObservableCollection<GKUserViewModel>();
			foreach (var user in allUsers)
			{
				var viewModel = new GKUserViewModel(user, isDevice);
				bool isPresentInThis = users.Any(x => x.Password == user.Password);
				bool isPresentInOther = otherUsers.Any(x => x.Password == user.Password);
				if(isPresentInThis && !isPresentInOther)
					viewModel.IsPresent= true;
				if(!isPresentInThis && isPresentInOther)
					viewModel.IsAbsent = true;
				result.Add(viewModel);
			}
			return result;
		}
	}
}