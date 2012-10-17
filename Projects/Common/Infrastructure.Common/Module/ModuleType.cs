using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Infrastructure.Common.Module
{
    public class ModuleType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        public static List<ModuleType> ModuleInitialize()
        {
            var modulesViewModel = new List<ModuleType>
            {
                new ModuleType(){Name = "FiltersModule", Description = "Фильтры журнала событий", IsEnabled = true},
                new ModuleType(){Name = "InstructionsModule", Description = "Инструкции", IsEnabled = true},
                new ModuleType(){Name = "LibraryModule", Description = "Библиотека устройств", IsEnabled = true},
                new ModuleType(){Name = "SoundsModule", Description = "Звуки", IsEnabled = true},
                new ModuleType(){Name = "ReportsModule", Description = "Отчеты", IsEnabled = true}
            };
            return modulesViewModel;
        }

        public static List<ModuleType> LoadModulesFromRegister()
        {
            var modules = ModuleInitialize();
            try
            {
                var registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Modules");
                if (registryKey != null)
                {
                    var Modules = registryKey.GetValueNames();
                    foreach (var module in Modules)
                    {
                        var status = registryKey.GetValue(module);
                        modules.FirstOrDefault(x => x.Name == module).IsEnabled = !status.Equals("False");
                    }
					registryKey.Close();
                }
            }
            catch (Exception) { }
            return modules;
        }
    }
}
