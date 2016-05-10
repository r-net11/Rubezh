using System;
using System.Linq;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RoleDetailsViewModel : SaveCancelDialogViewModel
	{
		public UserRole Role { get; private set; }
		public Guid UID { get; private set; }
		public PermissionsViewModel PermissionsViewModel { get; private set; }

		public RoleDetailsViewModel(UserRole role = null)
		{
			if (role != null)
			{
				Title = string.Format("Свойства шаблона прав: {0}", role.Name);
				Role = role;
			}
			else
			{
				Title = "Создание нового шаблона прав";
				Role = new UserRole();
			}

			PermissionsViewModel = new PermissionsViewModel(Role.PermissionStrings);
			CopyProperties();
		}

		void CopyProperties()
		{
			UID = Role.UID;
			Name = Role.Name;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		void SaveProperties()
		{
			Role = new UserRole();
			Role.UID = UID;
			Role.Name = Name;
			Role.PermissionStrings = PermissionsViewModel.GetPermissionStrings();
		}

		protected override bool Save()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBoxService.Show("Сначала введите название шаблона прав");
				return false;
			}
			else if (Name != Role.Name && FiresecManager.SecurityConfiguration.UserRoles.Any(role => role.Name == Name))
			{
				MessageBoxService.Show("Шаблон прав с таким названием уже существует");
				return false;
			}

			SaveProperties();
			return base.Save();
		}
	}
}