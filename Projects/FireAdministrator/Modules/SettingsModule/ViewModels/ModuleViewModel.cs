using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using Infrastructure.Common.Module;

namespace SettingsModule.ViewModels
{
    public class ModuleViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                SettingsViewModel.canAccept = true;
                OnPropertyChanged("IsEnabled");
            }
        }

        public static List<ModuleViewModel> ModuleInitialize()
        {
            var moduleViewModel = new List<ModuleViewModel>();
            var moduleTypes = ModuleType.ModuleInitialize();
            foreach (var moduleType in moduleTypes)
            {
                moduleViewModel.Add(new ModuleViewModel() { Name = moduleType.Name, Description = moduleType.Description, IsEnabled = moduleType.IsEnabled });
            }
            return moduleViewModel;
        }

        public void SetDisableModuleIntoRegister(bool isEnabled)
        {
            try
            {
                RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Modules");
                saveKey.SetValue(Name, isEnabled);
                saveKey.Close();
            }
            catch (Exception) { }
        }

        public static List<ModuleViewModel> LoadDisableModulesFromRegister()
        {
            var moduleViewModel = new List<ModuleViewModel>();
            var moduleTypes =  ModuleType.LoadModulesFromRegister();
            foreach (var  moduleType in moduleTypes)
            {
                moduleViewModel.Add(new ModuleViewModel(){Name = moduleType.Name, Description = moduleType.Description, IsEnabled = moduleType.IsEnabled});
            }
            return moduleViewModel;
        }
    }
}
