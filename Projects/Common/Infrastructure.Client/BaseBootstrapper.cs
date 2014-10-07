using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Services;
using Infrastructure.Client.Startup;

namespace Infrastructure.Client
{
	public class BaseBootstrapper
	{
		private List<IModule> _modules;
		public BaseBootstrapper()
		{
			Logger.Trace(SystemInfo.GetString());
			RegisterResource();
		}

		protected virtual void RegisterResource()
		{
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(BaseBootstrapper).Assembly, "Login/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(BaseBootstrapper).Assembly, "Startup/DataTemplates/Dictionary.xaml"));
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
						ApplicationService.ShutDown();
						break;
					}
				}
				catch (StartupCancellationException)
				{
					ApplicationService.ShutDown();
					break;
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.PreInitialize");
					MessageBoxService.ShowExceptionExtended(e);
					ApplicationService.ShutDown();
					break;
				}
		}
		protected void AterInitialize()
		{
			foreach (IModule module in _modules)
				try
				{
					module.AfterInitialize();
					ApplicationService.DoEvents();
				}
				catch (StartupCancellationException)
				{
					ApplicationService.ShutDown();
					break;
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.AterInitialize");
					MessageBoxService.ShowExceptionExtended(e);
					ApplicationService.ShutDown();
					break;
				}
		}
		protected void CreateViewModels()
		{
			foreach (IModule module in _modules)
				try
				{
					module.CreateViewModels();
					ApplicationService.DoEvents();
				}
				catch (StartupCancellationException)
				{
					ApplicationService.ShutDown();
					break;
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.Create");
					MessageBoxService.ShowExceptionExtended(e);
					ApplicationService.ShutDown();
					break;
				}
		}

		protected bool RunShell(ShellViewModel shellViewModel)
		{
			ApplicationService.RegisterShell(shellViewModel);
			StartupService.Instance.DoStep("Загрузка модулей приложения");
			CreateViewModels();
			shellViewModel.NavigationItems = new ReadOnlyCollection<NavigationItem>(GetNavigationItems());
			if (InitializeModules())
			{
				ApplicationService.User = FiresecManager.CurrentUser;
				StartupService.Instance.DoStep("Запуск приложения");
				ApplicationService.Run(shellViewModel);
				return true;
			}
			return false;
		}
		protected bool InitializeModules()
		{
			ReadConfiguration();
			foreach (IModule module in _modules.OrderBy(module => module.Order))
				try
				{
					StartupService.Instance.DoStep(string.Format("Инициализация модуля {0}", module.Name));
					module.Initialize();
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.InitializeModules");
					if (Application.Current != null)
						Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
					MessageBoxService.ShowErrorExtended(string.Format("Во время инициализации модуля '{0}' произошла ошибка, дальнейшая загрузка невозможна!\nПриложение будет закрыто.\n" + e.Message, module.Name));
					StartupService.Instance.Close();
					ApplicationService.ShutDown();
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
					navigationItems.AddRange(items);
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
				ModuleSection moduleSection = config.GetSection("modules") as ModuleSection;
				_modules = new List<IModule>();
				InvestigateAssembly(Assembly.GetEntryAssembly());
				foreach (ModuleElement moduleElement in moduleSection.Modules)
				{
					try
					{
						if (GlobalSettingsHelper.GlobalSettings.ModuleItems == null)
						{
							GlobalSettingsHelper.GlobalSettings.SetDefaultModules();
							GlobalSettingsHelper.Save();
						}
						if (!GlobalSettingsHelper.GlobalSettings.ModuleItems.Contains(moduleElement.AssemblyFile))
							continue;
						string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, moduleElement.AssemblyFile);
						if (File.Exists(path))
						{
							Assembly assembly = GetAssemblyByFileName(path);
							if (assembly != null)
								InvestigateAssembly(assembly);
						}
					}
					catch (Exception e)
					{
						Logger.Error(e, "BaseBootstrapper.ReadConfiguration");
						MessageBoxService.ShowErrorExtended("Не удалось загрузить модуль " + moduleElement.AssemblyFile);
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
				MessageBoxService.ShowErrorExtended(string.Format(Resources.UnableLoadModule, Path.GetFileName(path)));
				return null;
			}
		}
		private Assembly GetLoadedAssemblyByFileName(string path)
		{
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
		private void InvestigateAssembly(Assembly assembly)
		{
			foreach (Type t in assembly.GetExportedTypes())
				if (typeof(IModule).IsAssignableFrom(t) && t.GetConstructor(new Type[0]) != null)
					_modules.Add((IModule)Activator.CreateInstance(t, new object[0]));
		}
	}
}