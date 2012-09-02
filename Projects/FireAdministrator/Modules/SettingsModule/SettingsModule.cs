using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SettingsModule.ViewModels;
using Infrastructure.Client;

namespace SettingsModule
{
	public class SettingsModule : ModuleBase
	{
		SettingsViewModel _settingsViewModel;

		public SettingsModule()
		{
			_settingsViewModel = new SettingsViewModel();
		}

		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSettingsEvent>(_settingsViewModel, "Настройки", "/Controls;component/Images/settings.png"),
			};
		}
		public override string Name
		{
			get { return "Настройки"; }
		}
	}
}