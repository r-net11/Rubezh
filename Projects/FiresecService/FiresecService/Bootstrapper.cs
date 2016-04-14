﻿using Common;
using FiresecService.Models;
using FiresecService.Processor;
using FiresecService.Report;
using FiresecService.Service;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace FiresecService
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");
				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Name = "Main window";
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				FiresecService.Service.FiresecService.ServerState = ServerState.Starting;

				UILogger.Log("Проверка лицензии");
				if (!FiresecLicenseProcessor.TryLoadLicense())
					UILogger.Log("Ошибка лицензии", true);

				UILogger.Log("Проверка соединения с БД");
				using (var dbService = new DbService())
				{
					if (dbService.CheckConnection().HasError)
						UILogger.Log("Ошибка соединения с БД", true);
				}

				UILogger.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				UILogger.Log("Открытие хоста");
				FiresecServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);

				ProcedureExecutionContext.Initialize(
					ContextType.Server,
					() => { return ConfigurationCashHelper.SystemConfiguration; },
					Service.FiresecService.NotifyAutomation,
					null,
					null,
					ProcedureHelper.AddJournalItem,
					ProcedureHelper.ControlGKDevice,
					ProcedureHelper.StartRecord,
					ProcedureHelper.StopRecord,
					ProcedureHelper.Ptz,
					ProcedureHelper.RviAlarm,
					ProcedureHelper.RviOpenWindow,
					ProcedureHelper.ControlFireZone,
					ProcedureHelper.ControlGuardZone,
					ProcedureHelper.ControlDirection,
					ProcedureHelper.ControlGKDoor,
					ProcedureHelper.ControlDelay,
					ProcedureHelper.ControlPumpStation,
					ProcedureHelper.ControlMPT,
					ProcedureHelper.ExportJournal,
					ProcedureHelper.ExportOrganisation,
					ProcedureHelper.ExportOrganisationList,
					ProcedureHelper.ExportConfiguration,
					ProcedureHelper.ImportOrganisation,
					ProcedureHelper.ImportOrganisationList,
					GetOrganisations
					);

				ExplicitValue.ResolveObjectName += ObjectReference_ResolveObjectName;
				ExplicitValue.ResolveObjectValue += ExplicitValue_ResolveObjectValue;

				OpcDaHelper.Initialize(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers, ReadTagValue, WriteTagValue);

				GKProcessor.Create();
				UILogger.Log("Запуск ГК");
				GKProcessor.Start();

				UILogger.Log("Запуск сервиса отчетов");
				if (ReportServiceManager.Run())
				{
					UILogger.Log("Сервис отчетов запущен: " + ConnectionSettingsManager.ReportServerAddress);
					MainViewModel.SetReportAddress(ConnectionSettingsManager.ReportServerAddress);
				}
				else
				{
					UILogger.Log("Ошибка при запуске сервиса отчетов", true);
					MainViewModel.SetReportAddress("<Ошибка>");
				}

				AutomationProcessor.Start();
				RviProcessor.Start();
				ScheduleRunner.Start();
				ServerTaskRunner.Start();
				AutomationProcessor.RunOnApplicationRun();
				ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
				UILogger.Log("Готово");
				OpcDaServersProcessor.Start();
				UILogger.Log("Запуск OPC DA");
				FiresecService.Service.FiresecService.ServerState = ServerState.Ready;
				FiresecService.Service.FiresecService.AfterConnect += FiresecService_AfterConnect;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
				Close();
			}
		}

		static object ExplicitValue_ResolveObjectValue(Guid objectUID, ObjectType objectType)
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

		static string ObjectReference_ResolveObjectName(Guid objectUID, ObjectType objectType)
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

		static void ReadTagValue(Guid tagUID, object value)
		{
			OpcDaHelper.SetTagValue(tagUID, value);
			FiresecService.Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
			{
				CallbackUID = Guid.NewGuid(),
				ContextType = ContextType.Server,
				AutomationCallbackType = AutomationCallbackType.OpcDaTag,
				Data = new OpcDaTagCallBackData
				{
					TagUID = tagUID,
					Value = value
				}
			}, null);
		}

		static void WriteTagValue(Guid tagUID, object value)
		{
			string error;
			OpcDaServersProcessor.WriteTag(tagUID, value, out error);
		}

		static void FiresecService_AfterConnect(Guid clientUID)
		{
			foreach (var tag in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.SelectMany(x => x.Tags))
				FiresecService.Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.OpcDaTag,
					Data = new OpcDaTagCallBackData
					{
						TagUID = tag.Uid,
						Value = OpcDaHelper.GetTagValue(tag.Uid)
					}
				}, clientUID);

			foreach (var variable in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables)
				FiresecService.Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.GlobalVariable,
					Data = new GlobalVariableCallBackData
					{
						VariableUID = variable.Uid,
						ExplicitValue = (ExplicitValue)variable
					}
				}, clientUID);
		}

		static List<RubezhAPI.SKD.Organisation> GetOrganisations(Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.GetOrganisations(clientUID, new RubezhAPI.SKD.OrganisationFilter());
			return result.HasError ? new List<RubezhAPI.SKD.Organisation>() : result.Result;
		}

		private static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false, false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");

				BalloonHelper.ShowFromServer("Ошибка во время загрузки");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			ServerLoadHelper.SetStatus(FSServerState.Closed);
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}
			System.Environment.Exit(1);

#if DEBUG
			return;
#else
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
		}
	}
}