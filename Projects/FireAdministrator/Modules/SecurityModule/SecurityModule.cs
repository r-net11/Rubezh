using System.Collections.Generic;
using FiresecAPI.Models;
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

            //return null;
			return new List<NavigationItem>()
			{
				new NavigationItem("Права доступа", null, new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>(UsersViewModel, "Пользователи", "/Controls;component/Images/user.png"),
					new NavigationItem<ShowUserGroupsEvent>(RolesViewModel, "Группы", "/Controls;component/Images/users.png"),
				}),
			};
		}
		public override string Name
		{
			get { return "Права доступа"; }
		}
	}
}