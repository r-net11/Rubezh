using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SettingsModule.ViewModels;

namespace SettingsModule
{
    public class SettingsModule
    {
        public static bool HasChanges { get; set; }
        static SettingsViewModel _settingsViewModel;

        public SettingsModule()
        {
            ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Unsubscribe(OnShowSettings);
            ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Subscribe(OnShowSettings);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            _settingsViewModel = new SettingsViewModel();
        }

        static void OnShowSettings(string obj)
        {
            ServiceFactory.Layout.Show(_settingsViewModel);
        }
    }
}