using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using SettingsModule.ViewModels;

namespace SettingsModule
{
    public class SettingsModule : IModule
    {
        static SettingsViewModel _settingsViewModel;
        public static bool HasChanges { get; set; }

        public SettingsModule()
        {
            ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Subscribe(OnShowSettings);
            HasChanges = false;
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

        void CreateViewModels()
        {
            _settingsViewModel = new SettingsViewModel();
            _settingsViewModel.Initialize();
        }

        static void OnShowSettings(string obj)
        {
            ServiceFactory.Layout.Show(_settingsViewModel);
        }
    }
}
