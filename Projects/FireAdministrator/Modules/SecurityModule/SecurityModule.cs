using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SecurityModule.ViewModels;

namespace SecurityModule
{
    public class SecurityModule
    {
        static UsersViewModel _usersViewModel;
        static RolesViewModel _groupsViewModel;

        public SecurityModule()
        {
            ServiceFactory.Events.GetEvent<ShowUsersEvent>().Unsubscribe(OnShowUsers);
            ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Unsubscribe(OnShowUserGroups);

            ServiceFactory.Events.GetEvent<ShowUsersEvent>().Subscribe(OnShowUsers);
            ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Subscribe(OnShowUserGroups);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
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
    }
}