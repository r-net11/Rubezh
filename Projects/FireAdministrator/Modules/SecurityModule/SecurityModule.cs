using System.Collections.Generic;
using Infrastructure;
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
			ServiceFactory.Events.GetEvent<ShowUsersEvent>().Subscribe(OnShowUsers);
			ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Subscribe(OnShowUserGroups);

			_usersViewModel = new UsersViewModel();
			_groupsViewModel = new RolesViewModel();
		}

		void OnShowUsers(object obj)
		{	
			ServiceFactory.Layout.Show(_usersViewModel);
		}

		void OnShowUserGroups(object obj)
		{
			ServiceFactory.Layout.Show(_groupsViewModel);
		}

		public override void Initialize()
		{
			_usersViewModel.Initialize();
			_groupsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("Права доступа", null, new List<NavigationItem>(){
					new NavigationItem<ShowUsersEvent>("Пользователи", "/Controls;component/Images/user.png"),
					new NavigationItem<ShowUserGroupsEvent>("Группы", "/Controls;component/Images/users.png"),
				}),
			};
		}
		public override string Name
		{
			get { return "Права доступа"; }
		}
	}
}