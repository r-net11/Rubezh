using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using Common.GK;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Microsoft.Win32;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		public bool Initialize()
		{
			var result = true;
			LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

			if (ServiceFactory.LoginService.ExecuteConnect(App.Login, App.Password))
			{
				ServiceFactory.Events.GetEvent<UserChangedEvent>().Publish(false);
				App.Login = ServiceFactory.LoginService.Login;
				App.Password = ServiceFactory.LoginService.Password;
				try
				{
					CreateModules();

					LoadingService.Show("Чтение конфигурации", 15);
					LoadingService.AddCount(GetModuleCount());

					if (!App.IsMulticlient)
					{
						LoadingService.DoStep("Синхронизация файлов");
						FiresecManager.UpdateFiles();
					}

					LoadingService.DoStep("Загрузка конфигурации с сервера");
					if (App.IsMulticlient)
						FiresecManager.GetConfiguration("Multiclient/Configuration/" + App.MulticlientId);
					else
						FiresecManager.GetConfiguration("Monitor/Configuration");

					GKDriversCreator.Create();
					BeforeInitialize(true);

					LoadingService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll(false);

					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
					{
						LoadingService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();
						Notifier.Initialize();
						var shell = new MonitorShellViewModel();
						((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
						if (!RunShell(shell))
							result = false;
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());
						SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					LoadingService.Close();

					AterInitialize();

					//MutexHelper.KeepAlive();
					ProgressWatcher.Run();
					if (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost")
					{
						RegistrySettingsHelper.SetBool("isException", true);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.InitializeFs");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return false;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return false;
			}
			return result;
		}

		bool IsRestarting = false;

		private void OnConfigurationChanged()
		{
			try
			{
				ServiceFactory.ContentService.Invalidate();
				if (IsRestarting)
					return;
				FiresecManager.FiresecService.SuspendPoll = true;
				if (FiresecManager.FSAgent != null)
					FiresecManager.FSAgent.SuspendPoll = true;
				LoadingErrorManager.Clear();
				IsRestarting = true;
				ProgressWatcher.Close();
				ApplicationService.Restart();

				LoadingService.Show("Перезагрузка конфигурации", 10);
				LoadingService.AddCount(10);

				LoadingService.DoStep("Загрузка конфигурации с сервера");
				if (App.IsMulticlient)
					FiresecManager.GetConfiguration("Multiclient/Configuration/" + App.MulticlientId);
				else
					FiresecManager.GetConfiguration("Monitor/Configuration");

				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();

				BeforeInitialize(false);
				InitializeModules();
				if (ApplicationService.Modules.Any(x => x.Name == "Устройства и Зоны"))
					ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
				if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Guid.Empty);

				AterInitialize();

				LoadingService.Close();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Bootstrapper.OnConfigurationChanged");
			}
			finally
			{
				IsRestarting = false;
				FiresecManager.FiresecService.SuspendPoll = false;
				if (FiresecManager.FSAgent != null)
					FiresecManager.FSAgent.SuspendPoll = false;
			}
		}

		private void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			Application.Current.Shutdown();
		}
	}
}