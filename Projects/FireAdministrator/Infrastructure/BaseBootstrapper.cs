using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using Infrastructure.Common;
using Infrastructure.Common.Configuration;
using Infrastructure.Common.Navigation;

namespace Infrastructure
{
	public class BaseBootstrapper
	{
		public ReadOnlyCollection<IModule> Modules { get; private set; }

		public BaseBootstrapper()
		{
			Modules = null;
		}

		public List<NavigationItem> InitializeModules()
		{
			ReadConfiguration();
			var navigationItems = new List<NavigationItem>();
			foreach (IModule module in Modules)
			{
				module.Initialize();
				navigationItems.AddRange(module.CreateNavigation());
			}
			return navigationItems;
		}
		protected void ReadConfiguration()
		{
			if (Modules == null)
			{
				Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				ModuleSection section = config.GetSection("modules") as ModuleSection;
				List<IModule> modules = new List<IModule>();
				foreach (ModuleElement module in section.Modules)
				{
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, module.AssemblyFile);
					Assembly asm = GetAssemblyByFileName(path);
					foreach (Type t in asm.GetExportedTypes())
						if (typeof(IModule).IsAssignableFrom(t) && t.GetConstructor(new Type[0]) != null)
							modules.Add((IModule)Activator.CreateInstance(t, new object[0]));
				}
				Modules = new ReadOnlyCollection<IModule>(modules);
			}
		}

		protected void ReInitializeModules()
		{
			foreach (var module in Modules)
			{
				module.Initialize();
			}
		}

		private Assembly GetAssemblyByFileName(string path)
		{
			return GetLoadedAssemblyByFileName(path) ?? Assembly.LoadFile(path);
		}
		private Assembly GetLoadedAssemblyByFileName(string path)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
				if (assembly.Location == path)
					return assembly;
			return null;
		}
	}
}