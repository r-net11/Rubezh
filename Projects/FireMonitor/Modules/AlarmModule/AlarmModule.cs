using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;

namespace AlarmModule
{
    public class AlarmModule
    {
        static AlarmWatcher AlarmWatcher;

        public AlarmModule()
        {
            ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupListViewModel());

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            AlarmWatcher = new AlarmWatcher();
        }
    }
}