using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Infrastructure;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SecurityModule.ViewModels;

namespace SecurityModule
{
	public class SecurityModule : ModuleBase
	{
		private UsersViewModel _usersViewModel;
		private RolesViewModel _rolesViewModel;

		public override void CreateViewModels()
		{
			_usersViewModel = new UsersViewModel();
			_rolesViewModel = new RolesViewModel();
		}

		public override void Initialize()
		{
			UpdateAdminPredefinedPermissions();
			_usersViewModel.Initialize();
			_rolesViewModel.Initialize();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			if (!FiresecManager.CheckPermission(PermissionType.Adm_Security))
				return null;

			return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), "users", new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>(_usersViewModel, "Пользователи", "user"),
					new NavigationItem<ShowUserGroupsEvent>(_rolesViewModel, "Шаблоны прав", "users"),
				}),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Security; }
		}

		private void UpdateAdminPredefinedPermissions()
		{
			var adm = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(u => u.Login == "adm");
			if (adm == null)
				return;

			var permissions = Enum.GetNames(typeof (PermissionType)).Except(new List<string>
			{
				PermissionType.All.ToString(),
				PermissionType.Adm_All.ToString(),
				PermissionType.Oper_All.ToString()
			});
			permissions.ForEach(p =>
			{
				if (adm.PermissionStrings.All(ps => ps != p))
				{
					adm.PermissionStrings.Add(p);
					ServiceFactory.SaveService.SecurityChanged = true;
				}
			});
		}
	}
}