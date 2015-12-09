using Common;
using FireMonitor.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		AutoActivationWatcher _watcher;

		public bool Initialize()
		{
			var result = true;
			LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin(Login, Password))
			{
				Login = ServiceFactory.StartupService.Login;
				Password = ServiceFactory.StartupService.Password;
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
						SafeFiresecService.ReconnectionRequiredEvent += () => { ApplicationService.Invoke(OnReconnectionRequired); };

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

		static List<RubezhAPI.SKD.Organisation> GetOrganisations(Guid clientUID)
		{
			var result = ClientManager.FiresecService.GetOrganisations(new RubezhAPI.SKD.OrganisationFilter());
			return result.HasError ? new List<RubezhAPI.SKD.Organisation>() : result.Result;
		}

		private void OnSynchronizeVariable(Guid clientUID, Variable variable, ContextType targetContextType)
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
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel(this));
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());
			return result;
		}
		protected virtual ShellViewModel CreateShell()
		{
			return new MonitorShellViewModel();
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
		void OnReconnectionRequired()
		{
			try
			{
				ClientManager.FiresecService.SuspendPoll = true;
				var clientCredentials = new ClientCredentials()
				{
					UserName = Login,
					Password = Password,
					ClientType = ClientType.Monitor,
					ClientUID = FiresecServiceFactory.UID
				};

				var operationResult = ClientManager.FiresecService.Connect(FiresecServiceFactory.UID, clientCredentials);
				if (operationResult.HasError)
					Restart(Login, Password);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Bootstrapper.OnReconnectionRequired");
			}
			finally
			{
				ClientManager.FiresecService.SuspendPoll = false;
			}
		}
		public void Restart(string login = null, string password = null)
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
			if (login != null && password != null)
			{
				Login = login;
				Password = password;
			}
			RestartApplication();
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
							Login = arg.Replace("login='", "");
							Login = Login.Replace("'", "");
						}
						if (arg.StartsWith("password='") && arg.EndsWith("'"))
						{
							Password = arg.Replace("password='", "");
							Password = Password.Replace("'", "");
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