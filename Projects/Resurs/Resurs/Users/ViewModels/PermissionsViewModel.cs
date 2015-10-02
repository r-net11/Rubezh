using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class PermissionsViewModel : BaseViewModel
	{
		public ObservableCollection<PermissionViewModel> PermissionViewModels { get; set; }
		public PermissionsViewModel(User user , bool flag= true)
		{
			BuildPermissionViewModel(user,flag);
		}

		PermissionViewModel _selectedPermission;
		public PermissionViewModel SelectedPermission
		{
			get { return _selectedPermission; }
			set
			{
				_selectedPermission = value;
				OnPropertyChanged(() => SelectedPermission);
			}
		}

		void BuildPermissionViewModel(User user , bool flag = true)
		{
			PermissionViewModels = new ObservableCollection<PermissionViewModel>();

			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Device));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditDevice));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Apartment));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditApartment));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Tariff));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditTariff));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Report));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditReport));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Plot));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditPlot));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.User,flag));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditUser,flag));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Journal));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditJournal));

			foreach (var permissionViewModel in PermissionViewModels)
			{
				permissionViewModel.IsChecked = user.UserPermissions.Any(x => x.PermissionType == permissionViewModel.PermissionType);
			}

			SelectedPermission = PermissionViewModels.FirstOrDefault();
		}

		public void GetPermissionStrings(User user)
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
