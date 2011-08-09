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

            AlarmGroupListViewModel alarmGroupListViewModel = new AlarmGroupListViewModel();
            ServiceFactory.Layout.AddAlarmGroups(alarmGroupListViewModel);

            //ShowImitatorView();

            _alarmWatcher = new AlarmWatcher();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void ShowImitatorView()
        {
            AlarmImitatorView alarmImitatorView = new AlarmImitatorView();
            AlarmImitatorViewModel alarmImitatorViewModel = new AlarmImitatorViewModel();
            alarmImitatorView.DataContext = alarmImitatorViewModel;
            alarmImitatorView.Show();
        }
    }
}