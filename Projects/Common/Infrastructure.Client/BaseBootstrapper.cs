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
using Infrastructure.Client.Startup;
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
					ApplicationService.DoEvents();
					if (!result)
					{
						ApplicationService.ShutDown();
						break;
					}
				}
				catch (StartupCancellationException)
				{
					throw;
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
					throw;
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
					throw;
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
			StartupService.Instance.DoStep(Resources.Language.BaseBootstrapper.RunModules);
			CreateViewModels();
			shellViewModel.NavigationItems = new ReadOnlyCollection<NavigationItem>(GetNavigationItems());
			if (InitializeModules())
			{
				ApplicationService.User = FiresecManager.CurrentUser;
				StartupService.Instance.DoStep(Resources.Language.BaseBootstrapper.RunShell);
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
					StartupService.Instance.DoStep(string.Format(Resources.Language.BaseBootstrapper.InitializeModules, module.Name));
					module.Initialize();
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
                    MessageBoxService.ShowError(string.Format(Resources.Language.BaseBootstrapper.InitializeModules_Error, module.Name, e.Message));
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
					}
					catch (StartupCancellationException)
					{
						throw;
					}
					catch (Exception e)
					{
						Logger.Error(e, "BaseBootstrapper.ReadConfiguration");
						MessageBoxService.ShowError(string.Format(Resources.Language.BaseBootstrapper.ReadConfiguration_Module_Error,moduleElement.AssemblyFile));
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
				MessageBoxService.ShowError(string.Format(Resources.Language.BaseBootstrapper.GetAssemblyByFileName_Exception, Path.GetFileName(path)));
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