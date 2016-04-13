using Common;
using FireMonitor.Layout.ViewModels;
using FireMonitor.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		AutoActivationWatcher _watcher;

		public static ShellViewModel ShellViewModel { get; private set; }

		public bool Initialize()
		{
			var result = true;
			LoadingErrorManager.Clear();
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Layout/DataTemplates/Dictionary.xaml");
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
						() => { return ClientManager.SystemConfiguration; },
						SafeFiresecService.ProcessAutomationCallback,
						((SafeFiresecService)ClientManager.FiresecService).ProcedureCallbackResponse,
						OnSynchronizeVariable,
						((SafeFiresecService)ClientManager.FiresecService).AddJournalItem,
						((SafeFiresecService)ClientManager.FiresecService).ControlGKDevice,
						((SafeFiresecService)ClientManager.FiresecService).StartRecord,
						((SafeFiresecService)ClientManager.FiresecService).StopRecord,
						((SafeFiresecService)ClientManager.FiresecService).Ptz,
						((SafeFiresecService)ClientManager.FiresecService).RviAlarm,
						((SafeFiresecService)ClientManager.FiresecService).RviOpenWindow,
						((SafeFiresecService)ClientManager.FiresecService).ControlFireZone,
						((SafeFiresecService)ClientManager.FiresecService).ControlGuardZone,
						((SafeFiresecService)ClientManager.FiresecService).ControlDirection,
						((SafeFiresecService)ClientManager.FiresecService).ControlGKDoor,
						((SafeFiresecService)ClientManager.FiresecService).ControlDelay,
						((SafeFiresecService)ClientManager.FiresecService).ControlPumpStation,
						((SafeFiresecService)ClientManager.FiresecService).ControlMPT,
						((SafeFiresecService)ClientManager.FiresecService).ExportJournal,
						((SafeFiresecService)ClientManager.FiresecService).ExportOrganisation,
						((SafeFiresecService)ClientManager.FiresecService).ExportOrganisationList,
						((SafeFiresecService)ClientManager.FiresecService).ExportConfiguration,
						((SafeFiresecService)ClientManager.FiresecService).ImportOrganisation,
						((SafeFiresecService)ClientManager.FiresecService).ImportOrganisationList,
						GetOrganisations
						);

					ExplicitValue.ResolveObjectName += ObjectReference_ResolveObjectName;
					ExplicitValue.ResolveObjectValue += ExplicitValue_ResolveObjectValue;

					OpcDaHelper.Initialize(ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers, OpcDaHelper.SetTagValue, WriteTagValue);

					GKDriversCreator.Create();
					BeforeInitialize(true);

					ServiceFactory.StartupService.DoStep("Загрузка клиентских настроек");
					ClientSettings.LoadSettings();

					result = Run();
					SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
					SafeFiresecService.RestartEvent += () => { ApplicationService.Invoke(Restart); };

					if (result)
					{
						AterInitialize();
						RunWatcher();
					}

					AutomationProcessor.Start();

					SafeFiresecService.AutomationEvent -= OnAutomationCallback;
					SafeFiresecService.AutomationEvent += OnAutomationCallback;

					ServiceFactory.StartupService.DoStep("Старт полинга сервера");
					ClientManager.StartPoll();

					LoadPlansProperties();

					SafeFiresecService.JournalItemsEvent += OnJournalItems;

					ScheduleRunner.Start();

					AutomationProcessor.RunOnApplicationRun(ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == ClientManager.ClientCredentials.Login), FiresecServiceFactory.UID);
					//MutexHelper.KeepAlive();
					if (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost")
					{
						RegistrySettingsHelper.SetBool("isException", true);
					}
					ServiceFactory.StartupService.Close();
				}
				catch (StartupCancellationException)
				{
					if (Application.Current != null)
						Application.Current.Shutdown();
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

		object ExplicitValue_ResolveObjectValue(Guid objectUID, ObjectType objectType)
		{
			if (objectUID == Guid.Empty)
				return null;
			switch (objectType)
			{
				case ObjectType.Device: return GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Zone: return GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Direction: return GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Delay: return GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.GuardZone: return GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.PumpStation: return GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.MPT: return GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.VideoDevice: return ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.GKDoor: return GKManager.Doors.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Organisation: return RubezhClient.SKDHelpers.OrganisationHelper.GetSingle(objectUID);
			}
			return null;
		}

		string ObjectReference_ResolveObjectName(Guid objectUID, ObjectType objectType)
		{
			if (objectUID == Guid.Empty)
				return "Null";
			switch (objectType)
			{
				case ObjectType.Device:
					var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUID);
					if (device != null)
						return device.PresentationName;
					break;
				case ObjectType.Zone:
					var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUID);
					if (zone != null)
						return zone.PresentationName;
					break;
				case ObjectType.Direction:
					var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUID);
					if (direction != null)
						return direction.PresentationName;
					break;
				case ObjectType.Delay:
					var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objectUID);
					if (delay != null)
						return delay.PresentationName;
					break;
				case ObjectType.GuardZone:
					var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
					if (guardZone != null)
						return guardZone.PresentationName;
					break;
				case ObjectType.PumpStation:
					var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
					if (pumpStation != null)
						return pumpStation.PresentationName;
					break;
				case ObjectType.MPT:
					var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objectUID);
					if (mpt != null)
						return mpt.PresentationName;
					break;
				case ObjectType.VideoDevice:
					var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objectUID);
					if (camera != null)
						return camera.PresentationName;
					break;
				case ObjectType.GKDoor:
					var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == objectUID);
					if (gKDoor != null)
						return gKDoor.PresentationName;
					break;
				case ObjectType.Organisation:
					var organisation = RubezhClient.SKDHelpers.OrganisationHelper.GetSingle(objectUID);
					if (organisation != null)
						return organisation.Name;
					break;
			}
			return "Null";
		}

		private void WriteTagValue(Guid tagUID, object value)
		{
			((SafeFiresecService)ClientManager.FiresecService).WriteOpcDaServerTag(FiresecServiceFactory.UID, tagUID, value);
		}

		void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.GlobalVariable)
			{
				var data = automationCallbackResult.Data as GlobalVariableCallBackData;
				if (data != null)
				{
					var variable = ProcedureExecutionContext.GlobalVariables.FirstOrDefault(x => x.Uid == data.VariableUID);
					variable.Value = data.ExplicitValue.Value;
				}
				return;
			}

			if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.OpcDaTag)
			{
				var data = automationCallbackResult.Data as OpcDaTagCallBackData;
				if (data != null)
					OpcDaHelper.SetTagValue(data.TagUID, data.Value);
				return;
			}

			if (!AutomationHelper.CheckLayoutFilter(automationCallbackResult, GetLayoutUID()))
				return;

			switch (automationCallbackResult.AutomationCallbackType)
			{
				case AutomationCallbackType.ShowDialog:
					var showDialogData = automationCallbackResult.Data as ShowDialogCallbackData;
					if (showDialogData != null)
						LayoutDialogViewModel.Show(showDialogData);
					break;
				case AutomationCallbackType.CloseDialog:
					var closeDialogData = automationCallbackResult.Data as CloseDialogCallbackData;
					if (closeDialogData != null)
						LayoutDialogViewModel.Close(closeDialogData);
					break;
				case AutomationCallbackType.Sound:
					var soundData = (SoundCallbackData)automationCallbackResult.Data;
					var sound =
						ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.FirstOrDefault(
							x => x.Uid == soundData.SoundUID);
					if (sound != null)
						ApplicationService.Invoke(
							() =>
								AlarmPlayerHelper.Play(
									FileHelper.GetSoundFilePath(Path.Combine(ServiceFactoryBase.ContentService.ContentFolder, sound.Uid.ToString())),
									BeeperType.None, false));
					break;
				case AutomationCallbackType.Message:
					var messageData = (MessageCallbackData)automationCallbackResult.Data;
					ApplicationService.Invoke(() =>
					{
						if (messageData.WithConfirmation)
						{
							ApplicationService.BeginInvoke(() =>
								{
									var confirm = MessageBoxService.ShowConfirmation(messageData.Message, "Сообщение");
									ProcedureExecutionContext.CallbackResponse(FiresecServiceFactory.UID, automationCallbackResult.ContextType, automationCallbackResult.CallbackUID, confirm);
								});
						}
						else
							MessageBoxService.ShowExtended(messageData.Message, "Сообщение", messageData.IsModalWindow);
					});
					break;
				case AutomationCallbackType.Property:
					{
						var propertyData = (PropertyCallBackData)automationCallbackResult.Data;
						var ShowObjectDetailsEvent = new CompositePresentationEvent<Guid>();
						switch (propertyData.ObjectType)
						{
							case ObjectType.Device:
								var device = GKManager.Devices.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (device != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>();
								break;

							case ObjectType.Zone:
								var zone = GKManager.Zones.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (zone != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKZoneDetailsEvent>();
								break;

							case ObjectType.Direction:
								var direction = GKManager.Directions.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (direction != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDirectionDetailsEvent>();
								break;

							case ObjectType.Delay:
								var delay = GKManager.Delays.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (delay != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDelayDetailsEvent>();
								break;

							case ObjectType.GuardZone:
								var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (guardZone != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKGuardZoneDetailsEvent>();
								break;

							case ObjectType.VideoDevice:
								var videoDevice = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (videoDevice != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>();
								break;

							case ObjectType.GKDoor:
								var gkDoor = GKManager.Doors.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (gkDoor != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDoorDetailsEvent>();
								break;

							case ObjectType.PumpStation:
								var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (pumpStation != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationDetailsEvent>();
								break;

							case ObjectType.MPT:
								var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == propertyData.ObjectUid);
								if (mpt != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKMPTDetailsEvent>();
								break;
						}
						if (ShowObjectDetailsEvent != null)
							ApplicationService.BeginInvoke(() => ShowObjectDetailsEvent.Publish(propertyData.ObjectUid));
					}
					break;
				case AutomationCallbackType.GetPlanProperty:
					var controlPlanEventArg = new ControlPlanEventArg
					{
						ControlElementType = ControlElementType.Get,
						PlanCallbackData = (PlanCallbackData)automationCallbackResult.Data
					};
					ServiceFactory.Events.GetEvent<ControlPlanEvent>().Publish(controlPlanEventArg);
					ProcedureExecutionContext.CallbackResponse(FiresecServiceFactory.UID, automationCallbackResult.ContextType, automationCallbackResult.CallbackUID, controlPlanEventArg.PlanCallbackData.Value);
					break;
				case AutomationCallbackType.SetPlanProperty:
					controlPlanEventArg = new ControlPlanEventArg
					{
						ControlElementType = ControlElementType.Set,
						PlanCallbackData = (PlanCallbackData)automationCallbackResult.Data
					};
					ServiceFactory.Events.GetEvent<ControlPlanEvent>().Publish(controlPlanEventArg);
					break;
			}

		}

		protected virtual Guid? GetLayoutUID()
		{
			return Guid.Empty;
		}

		void LoadPlansProperties()
		{
			var properties = ClientManager.FiresecService.GetProperties(Guid.Empty);
			if (properties != null)
			{
				if (properties.PlanProperties != null)
					ServiceFactory.Events.GetEvent<ChangePlanPropertiesEvent>().Publish(properties.PlanProperties);
			}
		}

		static List<RubezhAPI.SKD.Organisation> GetOrganisations(Guid clientUID)
		{
			var result = ClientManager.FiresecService.GetOrganisations(new RubezhAPI.SKD.OrganisationFilter());
			return result.HasError ? new List<RubezhAPI.SKD.Organisation>() : result.Result;
		}

		private void OnSynchronizeVariable(Guid clientUID, Variable variable)
		{
			ClientManager.FiresecService.SetVariableValue(variable.Uid, variable.Value);
		}

		private void OnJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			if (isNew)
				foreach (var journalItem in journalItems)
					AutomationProcessor.RunOnJournal(journalItem, ClientManager.CurrentUser, FiresecServiceFactory.UID);
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
			ShellViewModel = CreateShell();
			((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)ShellViewModel.Toolbar);
			if (!RunShell(ShellViewModel))
				result = false;
			//((LayoutService)ServiceFactory.Layout).AddToolbarItem(new GlobalPimActivationViewModel());
			((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
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
		void Restart()
		{
			Restart(Login, Password);
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