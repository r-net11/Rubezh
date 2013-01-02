using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SecurityModule.ViewModels
{
    public class RolesViewModel : MenuViewPartViewModel, IEditingViewModel
    {
        public RolesViewModel()
        {
			Menu = new RolesMenuViewModel(this);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);
            RegisterShortcuts();
		}

		public void Initialize()
		{
            Roles = new ObservableCollection<RoleViewModel>();
            foreach (var role in FiresecManager.SecurityConfiguration.UserRoles)
                Roles.Add(new RoleViewModel(role));
			SelectedRole = Roles.FirstOrDefault();
        }

		ObservableCollection<RoleViewModel> _roles;
		public ObservableCollection<RoleViewModel> Roles
		{
			get { return _roles; }
			set
			{
				_roles = value;
				OnPropertyChanged("Roles");
			}
		}

        RoleViewModel _selectedRole;
        public RoleViewModel SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged("SelectedRole");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var roleDetailsViewModel = new RoleDetailsViewModel();
            if (DialogService.ShowModalWindow(roleDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.UserRoles.Add(roleDetailsViewModel.Role);
                var roleViewModel = new RoleViewModel(roleDetailsViewModel.Role);
                Roles.Add(roleViewModel);
                SelectedRole = roleViewModel;
                ServiceFactory.SaveService.SecurityChanged = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var result = MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить роль \"{0}\" из списка? Тогда будут удалены и все пользователи с этой ролью", SelectedRole.Role.Name));
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
				FiresecManager.SecurityConfiguration.Users = FiresecManager.SecurityConfiguration.Users.Where(x => x.RoleUID != SelectedRole.Role.UID).ToList();
                Roles.Remove(SelectedRole);

                ServiceFactory.SaveService.SecurityChanged = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var roleDetailsViewModel = new RoleDetailsViewModel(SelectedRole.Role);
			if (DialogService.ShowModalWindow(roleDetailsViewModel))
            {
				RemovePermissionsFromUsersWithRole(SelectedRole.Role.UID, SelectedRole.Role.PermissionStrings, roleDetailsViewModel.Role.PermissionStrings);
				AddPermissionsToUsersWithRole(SelectedRole.Role.UID, SelectedRole.Role.PermissionStrings, roleDetailsViewModel.Role.PermissionStrings);

                FiresecManager.SecurityConfiguration.UserRoles.Remove(SelectedRole.Role);
                SelectedRole.Role = roleDetailsViewModel.Role;
                FiresecManager.SecurityConfiguration.UserRoles.Add(SelectedRole.Role);

                ServiceFactory.SaveService.SecurityChanged = true;
            }
        }

		void RemovePermissionsFromUsersWithRole(Guid roleUID, List<string> oldPermissions, List<string> newPermissions)
        {
            foreach (var permissionType in oldPermissions.Where(x => newPermissions.Contains(x) == false))
            {
				foreach (var user in FiresecManager.SecurityConfiguration.Users.Where(x => x.RoleUID == roleUID))
					user.PermissionStrings.Remove(permissionType);
            }
        }

		void AddPermissionsToUsersWithRole(Guid roleUID, List<string> oldPermissions, List<string> newPermissions)
        {
            foreach (var permissionType in newPermissions.Where(x => oldPermissions.Contains(x) == false))
            {
				foreach (var user in FiresecManager.SecurityConfiguration.Users.Where(x => x.RoleUID == roleUID))
					user.PermissionStrings.Add(permissionType);
            }
        }

        bool CanEditDelete()
        {
            return SelectedRole != null;
        }

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }
    }
}