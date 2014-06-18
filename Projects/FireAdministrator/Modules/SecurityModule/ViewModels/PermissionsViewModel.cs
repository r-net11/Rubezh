using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class PermissionsViewModel : BaseViewModel
	{
		public PermissionsViewModel(List<string> permissionStrings)
		{
			BuildPermissionTree();
			FillAllPermissions();

			RootPermission.IsExpanded = true;
			SelectedPermission = RootPermission;
			foreach (var permission in AllPermissions)
			{
				permission.ExpandToThis();
				permission._isChecked = permissionStrings.Contains(permission.PermissionType.ToString());
			}

			OnPropertyChanged("RootPermissions");
		}

		public List<PermissionViewModel> AllPermissions;

		public void FillAllPermissions()
		{
			AllPermissions = new List<PermissionViewModel>();
			AddChildPlainPermissions(RootPermission);
		}

		void AddChildPlainPermissions(PermissionViewModel parentViewModel)
		{
			AllPermissions.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainPermissions(childViewModel);
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

		PermissionViewModel _rootPermission;
		public PermissionViewModel RootPermission
		{
			get { return _rootPermission; }
			private set
			{
				_rootPermission = value;
				OnPropertyChanged("RootPermission");
			}
		}

		public PermissionViewModel[] RootPermissions
		{
			get { return new PermissionViewModel[] { RootPermission }; }
		}

		void BuildPermissionTree()
		{
			RootPermission = new PermissionViewModel(PermissionType.All,
				new List<PermissionViewModel>()
			    {
			        new PermissionViewModel(PermissionType.Adm_All,
			            new List<PermissionViewModel>()
			            {
			                new PermissionViewModel(PermissionType.Adm_ViewConfig),
			                new PermissionViewModel(PermissionType.Adm_SetNewConfig),
							new PermissionViewModel(PermissionType.Adm_WriteDeviceConfig),
							new PermissionViewModel(PermissionType.Adm_ChangeDevicesSoft),
							new PermissionViewModel(PermissionType.Adm_Security),
							new PermissionViewModel(PermissionType.Adm_SKUD),
			            }),
			        new PermissionViewModel(PermissionType.Oper_All,
			            new List<PermissionViewModel>()
			            {
			                new PermissionViewModel(PermissionType.Oper_Login),
			                new PermissionViewModel(PermissionType.Oper_Logout),
							new PermissionViewModel(PermissionType.Oper_LogoutWithoutPassword),
							new PermissionViewModel(PermissionType.Oper_NoAlarmConfirm),
							new PermissionViewModel(PermissionType.Oper_AddToIgnoreList),
							new PermissionViewModel(PermissionType.Oper_RemoveFromIgnoreList),
							new PermissionViewModel(PermissionType.Oper_SecurityZone),
							new PermissionViewModel(PermissionType.Oper_ControlDevices),
							new PermissionViewModel(PermissionType.Oper_ChangeView),
							new PermissionViewModel(PermissionType.Oper_MayNotConfirmCommands),
							new PermissionViewModel(PermissionType.Oper_SKD,
								new List<PermissionViewModel>()
								{
									new PermissionViewModel(PermissionType.Oper_SKD_Employees),
									new PermissionViewModel(PermissionType.Oper_SKD_Guests),
									new PermissionViewModel(PermissionType.Oper_SKD_HR),
									new PermissionViewModel(PermissionType.Oper_SKD_Organisations),
								}),
			            }),
			    });
		}

		public List<string> GetPermissionStrings()
		{
			var result = new List<string>();
			foreach (var permission in AllPermissions)
			{
				if (permission.IsChecked)
					result.Add(permission.PermissionType.ToString());
			}
			return result;
		}
	}
}