using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;

namespace AlarmModule
{
    public class AlarmModuleLoader
    {
        static AlarmWatcher AlarmWatcher;
        static AlarmVideoWather AlarmVideoWather;

        public AlarmModuleLoader()
        {
            ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupListViewModel());

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void CreateViewModels()
        {
            AlarmWatcher = new AlarmWatcher();
            AlarmVideoWather = new AlarmVideoWather();
        }
    }
}