using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Client;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Navigation;
using Infrastructure.Events;
using SecurityModule.ViewModels;

namespace SecurityModule
{
	public class SecurityModule : ModuleBase
	{
		UsersViewModel UsersViewModel;
		RolesViewModel RolesViewModel;

		public override void CreateViewModels()
		{
			UsersViewModel = new UsersViewModel();
			RolesViewModel = new RolesViewModel();
		}

		public override void Initialize()
		{
			UsersViewModel.Initialize();
			RolesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			if (!ClientManager.CheckPermission(PermissionType.Adm_Security))
				return null;

			return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), "users", new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>(UsersViewModel, "Пользователи", "user"),
					new NavigationItem<ShowUserGroupsEvent>(RolesViewModel, "Шаблоны прав", "users"),
				}),
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Security; }
		}
	}
}