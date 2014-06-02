using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace SettingsModule.ViewModels
{
	public class ModulesViewModel : BaseViewModel
	{
		public ModulesViewModel()
		{
			Modules = new List<ModuleViewModel>();

			Modules.Add(new ModuleViewModel("PlansModule.dll"));
			Modules.Add(new ModuleViewModel("PlansModule.Kursk.dll"));
			Modules.Add(new ModuleViewModel("SecurityModule.dll"));
			Modules.Add(new ModuleViewModel("SoundsModule.dll"));
			Modules.Add(new ModuleViewModel("SettingsModule.dll"));
			Modules.Add(new ModuleViewModel("GKModule.dll"));
			Modules.Add(new ModuleViewModel("OPCModule.dll"));
			Modules.Add(new ModuleViewModel("NotificationModule.dll"));
			Modules.Add(new ModuleViewModel("VideoModule.dll"));
			Modules.Add(new ModuleViewModel("DiagnosticsModule.dll"));
			Modules.Add(new ModuleViewModel("ReportsModule.dll"));
			Modules.Add(new ModuleViewModel("SKDModule.dll"));
			Modules.Add(new ModuleViewModel("LayoutModule.dll"));
			Modules.Add(new ModuleViewModel("AutomationModule.dll"));

			Modules.Add(new ModuleViewModel("DevicesModule.dll"));
			Modules.Add(new ModuleViewModel("LibraryModule.dll"));
			Modules.Add(new ModuleViewModel("FiltersModule.dll"));
			Modules.Add(new ModuleViewModel("InstructionsModule.dll"));
			Modules.Add(new ModuleViewModel("AlarmModule.dll"));
			Modules.Add(new ModuleViewModel("JournalModule.dll"));

			if (GlobalSettingsHelper.GlobalSettings.ModuleItems == null)
				GlobalSettingsHelper.GlobalSettings.SetDefaultModules();
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