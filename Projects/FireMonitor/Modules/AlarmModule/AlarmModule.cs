using AlarmModule.Imitator;
using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Modularity;

namespace AlarmModule
{
    public class AlarmModule : IModule
    {
        static AlarmWatcher AlarmWatcher;

        public AlarmModule()
        {
            var alarmGroupListViewModel = new AlarmGroupListViewModel();
            ServiceFactory.Layout.AddAlarmGroups(alarmGroupListViewModel);
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
            //ShowImitatorView();

            AlarmWatcher = new AlarmWatcher();
        }

        static void ShowImitatorView()
        {
            var alarmImitatorView = new AlarmImitatorView()
            {
                DataContext = new AlarmImitatorViewModel()
            };
            alarmImitatorView.Show();
        }
    }
}