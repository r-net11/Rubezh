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
		UsersViewModel _usersViewModel;
		RolesViewModel _groupsViewModel;

		public SecurityModule()
		{
			_usersViewModel = new UsersViewModel();
			_groupsViewModel = new RolesViewModel();
		}

		public override void Initialize()
		{
			_usersViewModel.Initialize();
			_groupsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
            if (!FiresecManager.CheckPermission(PermissionType.Adm_Security))
                return null;

            //return null;
			return new List<NavigationItem>()
			{
				new NavigationItem("Права доступа", null, new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>(_usersViewModel, "Пользователи", "/Controls;component/Images/user.png"),
					new NavigationItem<ShowUserGroupsEvent>(_groupsViewModel, "Группы", "/Controls;component/Images/users.png"),
				}),
			};
		}
		public override string Name
		{
			get { return "Права доступа"; }
		}
	}
}