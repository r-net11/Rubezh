using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SecurityModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace SecurityModule
{
    public class SecurityModule : ModuleBase
    {
        static UsersViewModel _usersViewModel;
        static RolesViewModel _groupsViewModel;

        public SecurityModule()
        {
            ServiceFactory.Events.GetEvent<ShowUsersEvent>().Unsubscribe(OnShowUsers);
            ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Unsubscribe(OnShowUserGroups);

            ServiceFactory.Events.GetEvent<ShowUsersEvent>().Subscribe(OnShowUsers);
            ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Subscribe(OnShowUserGroups);
        }

        static void CreateViewModels()
        {
        }

        static void OnShowUsers(object obj)
        {
            _usersViewModel = new UsersViewModel();
            ServiceFactory.Layout.Show(_usersViewModel);
        }

        static void OnShowUserGroups(object obj)
        {
            _groupsViewModel = new RolesViewModel();
            ServiceFactory.Layout.Show(_groupsViewModel);
        }

		public override void Initialize()
		{
			CreateViewModels();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("Права доступа", null, new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>("Пользователи", "/Controls;component/Images/user.png"),
					new NavigationItem<ShowUserGroupsEvent>("Роли", "/Controls;component/Images/users.png"),
				}),
			};
		}
	}
}