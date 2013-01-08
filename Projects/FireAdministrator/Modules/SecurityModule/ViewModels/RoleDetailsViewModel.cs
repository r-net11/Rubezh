using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RoleDetailsViewModel : SaveCancelDialogViewModel
	{
		public UserRole Role { get; private set; }
		public Guid UID { get; private set; }

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
				Role = new UserRole();
			}

			CopyProperties();
		}

		void CopyProperties()
		{
			Permissions = new ObservableCollection<PermissionViewModel>();
			foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
				Permissions.Add(new PermissionViewModel(permissionType));

			UID = Role.UID;
			Name = Role.Name;
				foreach (var permissionString in Role.PermissionStrings)
					Permissions.First(permission => permission.Name == permissionString).IsEnable = true;
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
			Role = new UserRole();
			Role.UID = UID;
			Role.Name = Name;
			Role.PermissionStrings = new List<string>(
				Permissions.Where(x => x.IsEnable).Select(x => x.Name)
			);
		}

		protected override bool Save()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBoxService.Show("Сначала введите название роли");
				return false;
			}
			else if (Name != Role.Name && FiresecManager.SecurityConfiguration.UserRoles.Any(role => role.Name == Name))
			{
				MessageBoxService.Show("Роль с таким названием уже существует");
				return false;
			}

			SaveProperties();
			return base.Save();
		}
	}
}