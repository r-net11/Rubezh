using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.Win32;

namespace Infrastructure.Common.Module
{
    public class ModuleReg
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        public ModuleReg(string name, string description, bool isEnabled)
        {
            Name = name;
            Description = description;
            IsEnabled = isEnabled;
        }

        public static List<ModuleReg> ModuleInitialize()
        {
            var modulesViewModel = new List<ModuleReg>
            {
                new ModuleReg("FiltersModule", "Фильтры журнала событий", true),
                new ModuleReg("InstructionsModule", "Инструкции", true),
                new ModuleReg("LibraryModule", "Библиотека устройств", true),
                new ModuleReg("SoundsModule", "Звуки", true),
                new ModuleReg("ReportsModule", "Отчеты", true)
            };
            return modulesViewModel;
        }

        public static List<ModuleReg> LoadModulesFromRegister()
        {
            var modules = ModuleInitialize();
            try
            {
                var registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Modules");
                if (registryKey != null)
                {
                    var moduleNames = registryKey.GetValueNames();
                    if (moduleNames != null)
                    {
                        foreach (var moduleName in moduleNames)
                        {
                            var status = registryKey.GetValue(moduleName);
                            if (status != null)
                            {
                                var module = modules.FirstOrDefault(x => x.Name == moduleName);
                                if (module != null)
                                {
                                    module.IsEnabled = !status.Equals("False");
                                }
                            }
                        }
                    }
                    registryKey.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове LoadModulesFromRegister");
            }
            return modules;
        }
    }
}