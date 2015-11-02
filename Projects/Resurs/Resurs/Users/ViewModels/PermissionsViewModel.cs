using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common;
using ResursDAL;

namespace Resurs.ViewModels
{
	public class PermissionsViewModel : BaseViewModel
	{
		public ObservableCollection<PermissionViewModel> PermissionViewModels { get; set; }
		public PermissionsViewModel(User user)
		{
			CheckAllCommand = new RelayCommand(OnCheckAll);
			UnCheckAllCommand = new RelayCommand(OffCheckAll);

			PermissionViewModels = new ObservableCollection<PermissionViewModel>(Enum.GetValues(typeof(PermissionType)).Cast<PermissionType>().Select(x => new PermissionViewModel(x)));

			foreach (var permissionViewModel in PermissionViewModels)
			{
				permissionViewModel.IsEnabled = !(user.UID == DbCache.CurrentUser.UID && (permissionViewModel.PermissionType == PermissionType.ViewUser || permissionViewModel.PermissionType == PermissionType.EditUser));
				permissionViewModel.IsChecked = user.UserPermissions.Any(x => x.PermissionType == permissionViewModel.PermissionType);
			}
		}

		public RelayCommand CheckAllCommand { get; set; }
		void OnCheckAll()
		{
			PermissionViewModels.ForEach(x => x.IsChecked = true);
		}

		public RelayCommand UnCheckAllCommand { get; set; }
		void OffCheckAll()
		{
			PermissionViewModels.ForEach(x =>
			{
				if (x.IsEnabled)
					x.IsChecked = false;
			});
		}

		public void SavePermissions(User user)
		{
			user.UserPermissions = new List<UserPermission>();
			foreach (var permissionViewModel in PermissionViewModels)
			{
				if (permissionViewModel.IsChecked)
					user.UserPermissions.Add(new UserPermission() { PermissionType = permissionViewModel.PermissionType, User = user });
			}
		}
	}
}