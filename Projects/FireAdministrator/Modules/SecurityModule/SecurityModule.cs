using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using SecurityModule.ViewModels;

namespace SecurityModule
{
    public class SecurityModule : IModule
    {
        public SecurityModule()
        {
            ServiceFactory.Events.GetEvent<ShowUsersEvent>().Subscribe(OnShowUsers);
            ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Subscribe(OnShowUserGroups);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            usersViewModel = new UsersViewModel();
            usersViewModel.Initialize();

            groupsViewModel = new GroupsViewModel();
            groupsViewModel.Initialize();
        }

        static UsersViewModel usersViewModel;
        static GroupsViewModel groupsViewModel;

        static void OnShowUsers(string obj)
        {
            ServiceFactory.Layout.Show(usersViewModel);
        }

        static void OnShowUserGroups(string obj)
        {
            ServiceFactory.Layout.Show(groupsViewModel);
        }

        public static bool HasChanges { get; set; }
    }
}