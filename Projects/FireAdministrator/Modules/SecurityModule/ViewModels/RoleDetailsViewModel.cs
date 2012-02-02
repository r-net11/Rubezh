using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Controls.MessageBox;

namespace SecurityModule.ViewModels
{
    public class RoleDetailsViewModel : SaveCancelDialogContent
    {
        public UserRole Role { get; private set; }

        public RoleDetailsViewModel(UserRole role = null)
        {
            if (role != null)
            {
                Title = string.Format("Свойства роли: {0}", role.Name);
                Role = role;
            }
            else
            {
                Title = "Создание новой роли";

                if (FiresecManager.SecurityConfiguration.UserRoles.IsNotNullOrEmpty())
                    Role = new UserRole() { Id = FiresecManager.SecurityConfiguration.UserRoles.Max(x => x.Id) + 1 };
                else
                    Role = new UserRole() { Id = 1 };
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Permissions = new ObservableCollection<PermissionViewModel>();
            foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
                Permissions.Add(new PermissionViewModel(permissionType));

            Name = Role.Name;
            if (Role.Permissions.IsNotNullOrEmpty())
            {
                foreach (var permissionType in Role.Permissions)
                    Permissions.First(permission => permission.PermissionType == permissionType).IsEnable = true;
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Login");
            }
        }

        public ObservableCollection<PermissionViewModel> Permissions { get; private set; }

        void SaveProperties()
        {
            Role = new UserRole() { Id = Role.Id };
            Role.Name = Name;
            Role.Permissions = new List<PermissionType>(
                Permissions.Where(x => x.IsEnable).Select(x => x.PermissionType)
            );
        }

        protected override void Save(ref bool cancel)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBoxService.Show("Сначала введите название роли");
                cancel = true;
                return;
            }
            else if (Name != Role.Name && FiresecManager.SecurityConfiguration.UserRoles.Any(role => role.Name == Name))
            {
                MessageBoxService.Show("Роль с таким названием уже существует");
                cancel = true;
                return;
            }

            SaveProperties();
        }
    }
}