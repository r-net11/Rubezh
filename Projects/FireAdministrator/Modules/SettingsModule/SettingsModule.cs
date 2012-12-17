using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SettingsModule.ViewModels;

namespace SettingsModule
{
	public class SettingsModule : ModuleBase
	{
		SettingsViewModel SettingsViewModel;

		public override void CreateViewModels()
		{
			SettingsViewModel = new SettingsViewModel();
		}

		public override void Initialize()
		{
            SettingsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSettingsEvent>(SettingsViewModel, "Настройки", "/Controls;component/Images/settings.png"),
			};
		}
		public override string Name
		{
			get { return "Настройки"; }
		}
	}
}