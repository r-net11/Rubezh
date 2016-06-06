using System.Collections.Generic;
using Localization.Security.Common;
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
			if (!FiresecManager.CheckPermission(PermissionType.Adm_Security))
				return null;

			return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), "users", new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>(UsersViewModel, CommonResources.Users, "user"),
					new NavigationItem<ShowUserGroupsEvent>(RolesViewModel, CommonResources.RightsTemplate, "users"),
				}),
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Security; }
		}
	}
}