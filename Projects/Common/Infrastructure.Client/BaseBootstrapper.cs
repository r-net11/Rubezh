using Common;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Client.Properties;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.About.ViewModels;
using Infrastructure.Common.Configuration;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Infrastructure.Client
{
	public class BaseBootstrapper
	{
		public string Login { get; set; }
		public string Password { get; set; }
		private List<IModule> _modules;
		public BaseBootstrapper()
		{
			Logger.Trace(SystemInfo.GetString());
			RegisterResource();
		}

		protected virtual void RegisterResource()
		{
			ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");
			ServiceFactoryBase.ResourceService.AddResource(typeof(BaseBootstrapper).Assembly, "Login/DataTemplates/Dictionary.xaml");
			ServiceFactoryBase.ResourceService.AddResource(typeof(BaseBootstrapper).Assembly, "Startup/DataTemplates/Dictionary.xaml");
			ServiceFactoryBase.ResourceService.AddResource(typeof(AboutViewModel).Assembly, "About/DataTemplates/Dictionary.xaml");
			ServiceFactoryBase.ResourceService.AddResource(typeof(LoginViewModel).Assembly, "DataTemplates/Dictionary.xaml");
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
					ApplicationService.DoEvents();
					if (!result)
					{
						ApplicationService.ShutDown();
						break;
					}
				}
				catch (StartupCancellationException)
				{
					ApplicationService.ShutDown();
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.PreInitialize");
					MessageBoxService.ShowException(e);
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
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.AterInitialize");
					MessageBoxService.ShowException(e);
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
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.Create");
					MessageBoxService.ShowException(e);
					ApplicationService.ShutDown();
					break;
				}
		}

		protected bool RunShell(ShellViewModel shellViewModel)
		{
			StartupService.Instance.DoStep("Загрузка модулей приложения");
			CreateViewModels();
			shellViewModel.NavigationItems = new ReadOnlyCollection<NavigationItem>(GetNavigationItems());
			if (InitializeModules())
			{
				ApplicationService.User = ClientManager.CurrentUser;
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
					module.RegisterPlanExtension();
				}
				catch (StartupCancellationException)
				{
					throw;
				}
				catch (Exception e)
				{
					Logger.Error(e, "BaseBootstrapper.InitializeModules");
					if (Application.Current != null)
						Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
					MessageBoxService.ShowError(string.Format("Во время инициализации модуля '{0}' произошла ошибка, дальнейшая загрузка невозможна!\nПриложение будет закрыто.\n" + e.Message, module.Name));
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
				ApplicationService.DoEvents();
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
						else
						{
							var message = string.Format("Не найден файл модуля {0}", moduleElement.AssemblyFile);
							Logger.Error(message);
							//MessageBoxService.ShowError(message);
						}
					}
					catch (StartupCancellationException)
					{
						throw;
					}
					catch (Exception e)
					{
						Logger.Error(e, "BaseBootstrapper.ReadConfiguration");
						MessageBoxService.ShowError("Не удалось загрузить модуль " + moduleElement.AssemblyFile);
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
			catch (StartupCancellationException)
			{
				throw;
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

		public void RestartApplication()
		{
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
				Arguments = GetRestartCommandLineArguments()
			};
			Process.Start(processStartInfo);
		}
		protected virtual string GetRestartCommandLineArguments()
		{
			string commandLineArguments = null;
			if (Login != null && Password != null)
				commandLineArguments = "login='" + Login + "' password='" + Password + "'";
			return commandLineArguments;
		}
	}
}