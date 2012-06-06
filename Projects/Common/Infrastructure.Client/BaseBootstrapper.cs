using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using Common;
using FiresecClient;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Client.Properties;
using Infrastructure.Common;
using Infrastructure.Common.About.ViewModels;
using Infrastructure.Common.Configuration;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client
{
	public class BaseBootstrapper
	{
		public ReadOnlyCollection<IModule> Modules { get; private set; }

		public BaseBootstrapper()
		{
			Modules = null;
			RegisterResource();
		}

		protected virtual void RegisterResource()
		{
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(BaseBootstrapper).Assembly, "Login/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(AboutViewModel).Assembly, "About/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(LoginViewModel).Assembly, "DataTemplates/Dictionary.xaml"));
		}

		protected void RunShell(ShellViewModel shellViewModel)
		{
			LoadingService.DoStep("Загрузка модулей приложения");
			shellViewModel.NavigationItems = GetNavigationItems();
			InitializeModules();
			ApplicationService.User = FiresecManager.CurrentUser;
			LoadingService.DoStep("Запуск приложения");
			ApplicationService.Run(shellViewModel);
		}
		protected void InitializeModules()
		{
			ReadConfiguration();
			foreach (IModule module in Modules)
			{
				LoadingService.DoStep(string.Format("Инициализация модуля {0}", module.Name));
				module.Initialize();
			}
		}
		protected List<NavigationItem> GetNavigationItems()
		{
			ReadConfiguration();
			var navigationItems = new List<NavigationItem>();
			foreach (IModule module in Modules)
				navigationItems.AddRange(module.CreateNavigation());
			return navigationItems;
		}

		protected int GetModuleCount()
		{
			if (Modules == null)
			{
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				ModuleSection section = config.GetSection("modules") as ModuleSection;
				return section.Modules.Count;
			}
			else
				return Modules.Count;
		}
		protected void ReadConfiguration()
		{
			if (Modules == null)
			{
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				ModuleSection section = config.GetSection("modules") as ModuleSection;
				List<IModule> modules = new List<IModule>();
				foreach (ModuleElement module in section.Modules)
				{
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, module.AssemblyFile);
					Assembly asm = GetAssemblyByFileName(path);
					if (asm != null)
						foreach (Type t in asm.GetExportedTypes())
							if (typeof(IModule).IsAssignableFrom(t) && t.GetConstructor(new Type[0]) != null)
								modules.Add((IModule)Activator.CreateInstance(t, new object[0]));
				}
				Modules = new ReadOnlyCollection<IModule>(modules);
			}
		}
		private Assembly GetAssemblyByFileName(string path)
		{
			try
			{
				return GetLoadedAssemblyByFileName(path) ?? Assembly.LoadFile(path);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове BaseBootstrapper.GetAssemblyByFileName");
				MessageBoxService.ShowError(string.Format(Resources.UnableLoadModule, Path.GetFileName(path)));
				return null;
			}
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