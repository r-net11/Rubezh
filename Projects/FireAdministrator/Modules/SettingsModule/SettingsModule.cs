using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SettingsModule.ViewModels;
using Infrastructure.Common;

namespace SettingsModule
{
	public class SettingsModule : ModuleBase
	{
		SettingsViewModel _settingsViewModel;

		public SettingsModule()
		{
			ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Subscribe(OnShowSettings);
			_settingsViewModel = new SettingsViewModel();
		}

		void OnShowSettings(object obj)
		{
			ServiceFactory.Layout.Show(_settingsViewModel);
		}

		public override void Initialize()
		{
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