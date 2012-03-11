using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SettingsModule.ViewModels;

namespace SettingsModule
{
    public class SettingsModule
    {
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

        static void OnShowSettings(object obj)
        {
            ServiceFactory.Layout.Show(_settingsViewModel);
        }
    }
}