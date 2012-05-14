using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SettingsModule.ViewModels;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;

namespace SettingsModule
{
    public class SettingsModule : ModuleBase
    {
        static SettingsViewModel _settingsViewModel;

        public SettingsModule()
        {
            ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Unsubscribe(OnShowSettings);
            ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Subscribe(OnShowSettings);
        }

        void CreateViewModels()
        {
            _settingsViewModel = new SettingsViewModel();
        }

        static void OnShowSettings(object obj)
        {
            ServiceFactory.Layout.Show(_settingsViewModel);
        }

		public override void Initialize()
		{
			CreateViewModels();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSettingsEvent>("Настройки", "/Controls;component/Images/settings.png"),
			};
		}
	}
}