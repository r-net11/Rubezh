using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Infrastructure.Common.Module;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using Infrastructure.Common;

namespace SettingsModule.ViewModels
{
	public class ModuleViewModel : BaseViewModel
	{
		public List<ModuleViewModel> Modules { get; private set; }
		public ModuleViewModel()
		{
			Modules = ModuleInitialize();
		}
		public string Name { get; set; }
		public string Description { get; set; }

		bool _isEnabled = true;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				SetDisableModuleIntoRegister(_isEnabled);
				OnPropertyChanged("IsEnabled");
			}
		}

		public ModuleViewModel(string name, string description, bool isEnabled)
		{
			Name = name;
			Description = description;
			IsEnabled = isEnabled;
		}

		public static List<ModuleViewModel> ModuleInitialize()
		{
			var moduleRegs = ModuleReg.LoadModulesFromRegister();
			if (moduleRegs.Count == 0)
				moduleRegs = ModuleReg.ModuleInitialize();
			return moduleRegs.Select(moduleType => new ModuleViewModel(moduleType.Name, moduleType.Description, moduleType.IsEnabled)).ToList();
		}

		public void SetDisableModuleIntoRegister(bool isEnabled)
		{
			RegistrySettingsHelper.SetBool(Name, isEnabled);
		}

		public static List<ModuleViewModel> LoadDisableModulesFromRegister()
		{
			var moduleTypes = ModuleReg.LoadModulesFromRegister();
			return moduleTypes.Select(moduleType => new ModuleViewModel(moduleType.Name, moduleType.Description, moduleType.IsEnabled)).ToList();
		}
	}
}