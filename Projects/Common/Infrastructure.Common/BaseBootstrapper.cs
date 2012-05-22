using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using Common;
using Infrastructure.Common.Configuration;
using Infrastructure.Common.MessageBox;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Properties;
using Infrastructure.Common.Login.ViewModels;

namespace Infrastructure.Common
{
	public class BaseBootstrapper
	{
		public ReadOnlyCollection<IModule> Modules { get; private set; }

		public BaseBootstrapper()
		{
			Modules = null;
		}

		protected void InitializeModules()
		{
			ReadConfiguration();
			foreach (IModule module in Modules)
				module.Initialize();
		}
		protected List<NavigationItem> GetNavigationItems()
		{
			ReadConfiguration();
			var navigationItems = new List<NavigationItem>();
			foreach (IModule module in Modules)
				navigationItems.AddRange(module.CreateNavigation());
			return navigationItems;
		}

		protected bool PerformLogin(IUserDialogService userDialogs, string title)
		{
			var loginViewModel = new LoginViewModel()
			{
				Title = title
			};
			while (!loginViewModel.IsConnected && !loginViewModel.IsCanceled)
			{
				userDialogs.ShowModalWindow(loginViewModel);
				if (!string.IsNullOrEmpty(loginViewModel.Message))
					MessageBoxService.Show(loginViewModel.Message);
			}
			return loginViewModel.IsConnected;
		}

		private void ReadConfiguration()
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
			catch (Exception ex)
			{
				Logger.Error(ex);
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
