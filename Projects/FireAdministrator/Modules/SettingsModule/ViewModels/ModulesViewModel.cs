using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ModulesViewModel : BaseViewModel
	{
		public ModulesViewModel()
		{
			Modules = new List<ModuleViewModel>();

			Modules.Add(new ModuleViewModel("PlansModule.dll", "Планы"));
			Modules.Add(new ModuleViewModel("PlansModule.Kursk.dll", "Планы. Курск"));
			Modules.Add(new ModuleViewModel("SecurityModule.dll", "Пользователи"));
			Modules.Add(new ModuleViewModel("GKModule.dll", "ГК"));
			Modules.Add(new ModuleViewModel("SKDModule.dll", "СКД"));
			Modules.Add(new ModuleViewModel("VideoModule.dll", "Видео"));
			Modules.Add(new ModuleViewModel("AutomationModule.dll", "Автоматизация"));
			Modules.Add(new ModuleViewModel("LayoutModule.dll", "Макеты"));
			Modules.Add(new ModuleViewModel("ReportsModule.dll", "Отчеты"));
			Modules.Add(new ModuleViewModel("FiltersModule.dll", "Фильтры"));
			Modules.Add(new ModuleViewModel("JournalModule.dll", "Журнал событий"));
			Modules.Add(new ModuleViewModel("SoundsModule.dll", "Звуки"));
			Modules.Add(new ModuleViewModel("SettingsModule.dll", "Настройки", false));
			Modules.Add(new ModuleViewModel("DiagnosticsModule.dll", "Диагностика"));

			foreach (var moduleName in GlobalSettingsHelper.GlobalSettings.ModuleItems)
			{
				var moduleViewModel = Modules.FirstOrDefault(x => x.Name == moduleName);
				if (moduleViewModel != null)
				{
					moduleViewModel.IsSelected = true;
				}
			}
		}

		public List<ModuleViewModel> Modules { get; private set; }

		public void Save()
		{
			GlobalSettingsHelper.GlobalSettings.ModuleItems = new List<string>();
			foreach (var moduleViewModel in Modules)
			{
				if (moduleViewModel.IsSelected)
				{
					GlobalSettingsHelper.GlobalSettings.ModuleItems.Add(moduleViewModel.Name);
				}
			}
		}
	}
}