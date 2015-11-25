using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Common;
using FireMonitor.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Threading;
using Infrastructure.Client.Startup;
using RubezhAPI.Journal;
using Infrastructure.Automation;
using RubezhAPI.Automation;
using System.Collections.Generic;

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
					ClientManager.GetLicense();

					ServiceFactory.StartupService.ShowLoading("Чтение конфигурации", 15);
					ServiceFactory.StartupService.AddCount(GetModuleCount());

					ServiceFactory.StartupService.DoStep("Синхронизация файлов");
					ClientManager.UpdateFiles();

					ServiceFactory.StartupService.DoStep("Загрузка конфигурации с сервера");
					ClientManager.GetConfiguration("Monitor/Configuration");
					ProcedureExecutionContext.Initialize(
						ContextType.Client,
						ClientManager.SystemConfiguration,
						ClientManager.SecurityConfiguration,
						SafeFiresecService.ProcessAutomationCallback,
						ClientManager.FiresecService.ProcedureCallbackResponse,
						OnSynchronizeVariable,
						ClientManager.FiresecService.AddJournalItem,
						ClientManager.FiresecService.ControlGKDevice,
						ClientManager.FiresecService.StartRecord,
						ClientManager.FiresecService.StopRecord,
						ClientManager.FiresecService.Ptz,
						ClientManager.FiresecService.RviAlarm,
						ClientManager.FiresecService.ControlFireZone,
						ClientManager.FiresecService.ControlGuardZone,
						ClientManager.FiresecService.ControlDirection,
						ClientManager.FiresecService.ControlGKDoor,
						ClientManager.FiresecService.ControlDelay,
						ClientManager.FiresecService.ControlPumpStation,
						ClientManager.FiresecService.ControlMPT,
						ClientManager.FiresecService.ExportJournal,
						ClientManager.FiresecService.ExportOrganisation,
						ClientManager.FiresecService.ExportOrganisationList,
						ClientManager.FiresecService.ExportConfiguration,
						ClientManager.FiresecService.ImportOrganisation,
						ClientManager.FiresecService.ImportOrganisationList,
						GetOrganisations
						);

					GKDriversCreator.Create();
					BeforeInitialize(true);

					ServiceFactory.StartupService.DoStep("Старт полинга сервера");
					ClientManager.StartPoll();

					ServiceFactory.StartupService.DoStep("Проверка прав пользователя");
					if (ClientManager.CheckPermission(PermissionType.Oper_Login))
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
					SafeFiresecService.JournalItemsEvent += OnJournalItems;

					ScheduleRunner.Start();

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

		static List<RubezhAPI.SKD.Organisation> GetOrganisations()
		{
			var result = ClientManager.FiresecService.GetOrganisations(new RubezhAPI.SKD.OrganisationFilter());
			return result.HasError ? new List<RubezhAPI.SKD.Organisation>() : result.Result;
		}

		private void OnSynchronizeVariable(Variable variable, ContextType targetContextType)
		{
			if (targetContextType == ContextType.Client)
			{
				var remoteVariable = ClientManager.FiresecService.GetVariable(variable.Uid);
				if (remoteVariable != null)
				{
					variable.ExplicitValue = remoteVariable.ExplicitValue;
					variable.ExplicitValues = remoteVariable.ExplicitValues;
				}
			}
			else
			{
				ClientManager.FiresecService.SetVariableValue(variable.Uid, ProcedureExecutionContext.GetValue(variable));
			}
		}

		private void OnJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			if (isNew)
				foreach (var journalItem in journalItems)
					AutomationProcessor.RunOnJournal(journalItem);
		}

		static void ShutDown()
		{
			ClientManager.Disconnect();
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
				ClientManager.FiresecService.StopPoll();
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