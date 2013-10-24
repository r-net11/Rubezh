using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SettingsModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;

namespace SettingsModule
{
	public class SettingsModule : ModuleBase
	{
		SettingsViewModel SettingsViewModel;

		public override void CreateViewModels()
		{
            ServiceFactory.Events.GetEvent<EditValidationEvent>().Subscribe(OnEditValidation);
            
            SettingsViewModel = new SettingsViewModel();
			ServiceFactory.RibbonService.AddRibbonItems(new RibbonMenuItemViewModel("Настройки", SettingsViewModel, "/Controls;component/Images/BSettings.png", "Настройка приложения") { Order = int.MaxValue - 1 });
		}

		public override void Initialize()
		{
			SettingsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return null;
		}
		public override string Name
		{
			get { return "Настройки"; }
		}

        void OnEditValidation(object obj)
        {
			SettingsViewModel.ShowErrorsFilterCommand.Execute();
        }
	}
}