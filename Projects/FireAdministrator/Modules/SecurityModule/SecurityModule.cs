using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using SecurityModule.ViewModels;

namespace SecurityModule
{
    public class SecurityModule : IModule
    {
        static UsersViewModel UsersViewModel;
        static GroupsViewModel GroupsViewModel;

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
            UsersViewModel = new UsersViewModel();
            GroupsViewModel = new GroupsViewModel();
        }

        static void OnShowUsers(string obj)
        {
            ServiceFactory.Layout.Show(UsersViewModel);
        }

        static void OnShowUserGroups(string obj)
        {
            ServiceFactory.Layout.Show(GroupsViewModel);
        }

        public static bool HasChanges { get; set; }
    }
}