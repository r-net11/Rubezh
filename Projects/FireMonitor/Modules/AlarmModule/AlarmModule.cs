using AlarmModule.Imitator;
using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Modularity;

namespace AlarmModule
{
    public class AlarmModule : IModule
    {
        AlarmWatcher _alarmWatcher;

        public void Initialize()
        {
            RegisterResources();

            var alarmGroupListViewModel = new AlarmGroupListViewModel();
            ServiceFactory.Layout.AddAlarmGroups(alarmGroupListViewModel);

            //ShowImitatorView();

            _alarmWatcher = new AlarmWatcher();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
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