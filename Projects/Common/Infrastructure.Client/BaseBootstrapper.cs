using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Common;
using FiresecClient;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Client.Properties;
using Infrastructure.Common;
using Infrastructure.Common.About.ViewModels;
using Infrastructure.Common.Configuration;
using Infrastructure.Common.Module;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.BalloonTrayTip.Views;

namespace Infrastructure.Client
{
	public class BaseBootstrapper
	{
		private List<IModule> _modules;
		private List<ModuleReg> modulesFromReg = new List<ModuleReg>();
		public BaseBootstrapper()
		{
			Logger.Trace(SystemInfo.GetString());
			modulesFromReg = ModuleReg.LoadModulesFromRegister();
			_modules = null;
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

		protected void CreateModules()
		{
			ReadConfiguration();
		}

		protected void BeforeInitialize(bool firstTime)
		{
			foreach (IModule module in _modules)
				try
				{
					var result = module.BeforeInitialize(firstTime);
					if (!result)
					{
						Application.Current.Shutdown();
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.PreInitialize");
					MessageBoxService.ShowError(e.Message);
					Application.Current.Shutdown();
				}
		}

		protected void AterInitialize()
		{
			foreach (IModule module in _modules)
				try
				{
					module.AfterInitialize();
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.AterInitialize");
					MessageBoxService.ShowError(e.Message);
					Application.Current.Shutdown();
				}
		}

		protected void CreateViewModels()
		{
			foreach (IModule module in _modules)
				try
				{
					module.CreateViewModels();
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.Create");
					MessageBoxService.ShowError(e.Message);
					Application.Current.Shutdown();
				}
		}

		protected void RunShell(ShellViewModel shellViewModel)
		{
			LoadingService.DoStep("Загрузка модулей приложения");
			CreateViewModels();
			shellViewModel.NavigationItems = GetNavigationItems();
			if (InitializeModules())
			{
				ApplicationService.User = FiresecManager.CurrentUser;
				LoadingService.DoStep("Запуск приложения");
				ApplicationService.Run(shellViewModel);
			}
		}
		protected bool InitializeModules()
		{
			ReadConfiguration();
			foreach (IModule module in _modules.OrderBy(module => module.Order))
				try
				{
					LoadingService.DoStep(string.Format("Инициализация модуля {0}", module.Name));
					module.Initialize();
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.InitializeModules");
					Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
					LoadingService.Close();
					MessageBoxService.ShowError(string.Format("Во время инициализации модуля '{0}' произошла ошибка, дальнейшая загрузка невозможна!\nПриложение будет закрыто.\n" + e.Message, module.Name));
					Application.Current.Shutdown();
					return false;
				}
			return true;
		}
		protected List<NavigationItem> GetNavigationItems()
		{
			ReadConfiguration();
			var navigationItems = new List<NavigationItem>();
			foreach (IModule module in _modules)
			{
				var items = module.CreateNavigation();
				if (items != null && items.Count() > 0)
				{
					navigationItems.AddRange(items);
				}
			}
			return navigationItems;
		}

		protected int GetModuleCount()
		{
			if (_modules == null)
			{
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				ModuleSection section = config.GetSection("modules") as ModuleSection;
				return section.Modules.Count;
			}
			else
				return _modules.Count;
		}
		protected void ReadConfiguration()
		{
			if (_modules == null)
			{
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				ModuleSection section = config.GetSection("modules") as ModuleSection;
				_modules = new List<IModule>();
				foreach (ModuleElement moduleElement in section.Modules)
				{
					var moduledescr = moduleElement.AssemblyFile.Substring(0, moduleElement.AssemblyFile.ToString().LastIndexOf('.'));
					if (modulesFromReg.FirstOrDefault(x => (moduledescr == x.Name) && (x.IsEnabled == false)) == null)
					{
					}
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, moduleElement.AssemblyFile);
					if (File.Exists(path))
					{
						Assembly assembly = GetAssemblyByFileName(path);
						if (assembly != null)
							foreach (Type t in assembly.GetExportedTypes())
								if (typeof(IModule).IsAssignableFrom(t) && t.GetConstructor(new Type[0]) != null)
									_modules.Add((IModule)Activator.CreateInstance(t, new object[0]));
					}
				}
			}
			ApplicationService.RegisterModules(_modules);
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
			//Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var assemblies = from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
							 where !(assembly is System.Reflection.Emit.AssemblyBuilder) &&
							 assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder" &&
							 !assembly.GlobalAssemblyCache &&
							 assembly.CodeBase != Assembly.GetExecutingAssembly().CodeBase
							 select assembly;
			foreach (Assembly assembly in assemblies)
				if (assembly.Location == path)
					return assembly;
			return null;
		}
	}
}