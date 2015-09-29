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
		public PermissionsViewModel(User user)
		{
			PermissionViewModels = new ObservableCollection<PermissionViewModel>();

			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Device, user.IsViewDevice));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditDevice,  user.IsEditDevice));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.Apartment, user.IsViewApartment));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditApartment, user.IsEditApartment));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.User, user.IsViewUser));
			PermissionViewModels.Add(new PermissionViewModel(PermissionType.EditUser, user.IsEditUser));

			OnPropertyChanged(() => PermissionViewModels);
			SelectedPermission = PermissionViewModels.FirstOrDefault();

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

		ObservableCollection<PermissionViewModel> _permissionViewModels;
		public ObservableCollection<PermissionViewModel> PermissionViewModels
		{
			get { return _permissionViewModels; }
			private set
			{
				_permissionViewModels = value;
				OnPropertyChanged(() => PermissionViewModels);
			}
		}

		public void GetPermissionStrings(User user)
		{
			foreach (var permission in PermissionViewModels)
			{
				switch(permission.PermissionType)
				{
					case PermissionType.Device:
						user.IsViewDevice = permission.IsChecked;
						break;
					case PermissionType.EditDevice:
						user.IsEditDevice = permission.IsChecked;
						break;
					case PermissionType.Apartment:
						user.IsViewApartment = permission.IsChecked;
						break;
					case PermissionType.EditApartment:
						user.IsEditApartment = permission.IsChecked;
						break;
					case PermissionType.User:
						user.IsViewUser = permission.IsChecked;
						break;
					case PermissionType.EditUser:
						user.IsEditUser = permission.IsChecked;
						break;
				}
			}
		}
	}
}
