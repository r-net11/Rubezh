using System.Collections.Generic;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Ribbon;
using Infrastructure.Events;
using SettingsModule.ViewModels;

namespace SettingsModule
{
	public class SettingsModule : ModuleBase
	{
		SettingsViewModel SettingsViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<EditValidationEvent>().Subscribe(OnEditValidation);
			
			SettingsViewModel = new SettingsViewModel();
			ServiceFactory.RibbonService.AddRibbonItems(new RibbonMenuItemViewModel(ModuleType.ToDescription(), SettingsViewModel, "/Controls;component/Images/BSettings.png", "Настройка приложения") { Order = int.MaxValue - 1 });
		}

		public override void Initialize()
		{
			SettingsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return null;
		}
		protected override ModuleType ModuleType
		{
			get { return ModuleType.Settings; }
		}

		void OnEditValidation(object obj)
		{
			SettingsViewModel.ShowErrorsFilterCommand.Execute();
		}
	}
}