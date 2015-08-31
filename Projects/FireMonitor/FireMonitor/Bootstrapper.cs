﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.Windows.Threading;
using Infrastructure.Client.Startup;
using System.Threading;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		private string _login;
		private string _password;
		private AutoActivationWatcher _watcher;

		public bool Initialize()
		{
			var result = true;
			LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin(_login, _password))
			{
				_login = ServiceFactory.StartupService.Login;
				_password = ServiceFactory.StartupService.Password;
				try
				{
					CreateModules();

                    ServiceFactory.StartupService.DoStep("Загрузка лицензии");
                    FiresecManager.GetLicense();

					ServiceFactory.StartupService.ShowLoading("Чтение конфигурации", 15);
					ServiceFactory.StartupService.AddCount(GetModuleCount());

					ServiceFactory.StartupService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					ServiceFactory.StartupService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration("Monitor/Configuration");

					GKDriversCreator.Create();
					BeforeInitialize(true);

					ServiceFactory.StartupService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll();

					ServiceFactory.StartupService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
					{
						ServiceFactory.StartupService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();

						result = Run();
						SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };

						if (result)
						{
							AterInitialize();
							RunWatcher();
						}
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						ShutDown();
						return false;
					}
															
                    SafeFiresecService.ReconnectionErrorEvent += x => { ApplicationService.Invoke(OnReconnectionError, x); };
					SafeFiresecService.LicenseChangedEvent += () => { ApplicationService.Invoke(OnLicenseChanged); };

					//MutexHelper.KeepAlive();
					if (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost")
					{
						RegistrySettingsHelper.SetBool("isException", true);
					}
					ServiceFactory.StartupService.Close();
				}
				catch (StartupCancellationException)
				{
					throw;
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

		static void ShutDown()
		{
			FiresecManager.Disconnect();
			if (Application.Current != null)
				Application.Current.Shutdown();
		}

        protected virtual bool Run()
		{
			var result = true;
			var shell = CreateShell();
			((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
			if (!RunShell(shell))
				result = false;
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());
			return result;
		}
		protected virtual ShellViewModel CreateShell()
		{
			return new MonitorShellViewModel();
		}

        void OnReconnectionError(string error)
        {
            if (!MessageBoxService.ShowConfirmation(String.Format("Связь с сервером восстановлена после сбоя, однако подключение не удалось по причине:\n\"{0}\"\nПовторить попытку подключения?", error))
                && Application.Current != null)
            {
                Application.Current.Shutdown();
            }
        }

		void OnLicenseChanged()
		{
			MessageBoxService.ShowWarning("Сервер изменил параметры лицензии. Программа будет перезагружена.");
			Restart();
		}

		void OnConfigurationChanged()
		{
			var restartView = new RestartApplicationViewModel();
			var isRestart = DialogService.ShowModalWindow(restartView);
			if (isRestart)
				Restart();
			else
			{
				var timer = new DispatcherTimer();
				timer.Tick += (s, e) =>
				{
					timer.Stop();
					Restart();
				};
				timer.Interval = TimeSpan.FromSeconds(restartView.Total);
				timer.Start();
			}
		}
		void Restart()
		{
			using (new WaitWrapper())
			{
				ApplicationService.ApplicationWindow.IsEnabled = false;
				ServiceFactory.ContentService.Invalidate();
				FiresecManager.FiresecService.StopPoll();
				LoadingErrorManager.Clear();
				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();
				ApplicationService.ShutDown();
			}
			RestartApplication();
		}

		public void RestartApplication()
		{
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
				Arguments = GetRestartCommandLineArguments()
			};
			System.Diagnostics.Process.Start(processStartInfo);
		}
		protected virtual string GetRestartCommandLineArguments()
		{
			string commandLineArguments = null;
			if (_login != null && _password != null)
				commandLineArguments = "login='" + _login + "' password='" + _password + "'";
			return commandLineArguments;
		}
		public virtual void InitializeCommandLineArguments(string[] args)
		{
			if (args != null)
			{
				if (args.Count() >= 2)
				{
					foreach (var arg in args)
					{
						if (arg.StartsWith("login='") && arg.EndsWith("'"))
						{
							_login = arg.Replace("login='", "");
							_login = _login.Replace("'", "");
						}
						if (arg.StartsWith("password='") && arg.EndsWith("'"))
						{
							_password = arg.Replace("password='", "");
							_password = _password.Replace("'", "");
						}
					}
				}
			}
		}
				
		private void RunWatcher()
		{
			_watcher = new AutoActivationWatcher();
			_watcher.Run();
		}
	}
}