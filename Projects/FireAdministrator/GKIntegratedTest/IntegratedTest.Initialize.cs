using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Common;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using RubezhAPI.Journal;
using System.Collections.Generic;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Stopwatch = NUnit.Framework.Compatibility.Stopwatch;

namespace GKIntegratedTest
{
	[TestFixture]
	public partial class ItegratedTest
	{
		GKDevice gkDevice1;
		GKDevice kauDevice11;
		GKDevice kauDevice12;
		List<JournalItem> jourlnalItems;

		[SetUp]
		public void InitializeConnection()
		{
			CheckTime(()=>RunProcess("FiresecService", "FiresecServerPath"), "Запуск сервера");
			for (int i = 1; i <= 10; i++)
			{
				var message = CheckTime(() => ClientManager.Connect(ClientType.Other, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.AdminLogin, GlobalSettingsHelper.GlobalSettings.AdminPassword), "Соединение с сервером (" + i + " попытка)");
				if (message == null)
					break;
				Thread.Sleep(5000);
				if (i == 10)
				{
					MessageBoxService.ShowError("Ошибка соединения с сервером: " + message);
					return;
				}
			}
			InitializeConfiguration();
		}

		public void InitializeConfiguration()
		{
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			InitializeRootDevices();
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			InitializeStates();
			ServiceFactory.Initialize(null, null);
			jourlnalItems = new List<JournalItem>();

			SafeFiresecService.GKCallbackResultEvent -= OnGKCallbackResult;
			SafeFiresecService.GKCallbackResultEvent += OnGKCallbackResult;

			SafeFiresecService.JournalItemsEvent -= OnNewJournalItems;
			SafeFiresecService.JournalItemsEvent += OnNewJournalItems;
		}

		public void SetConfigAndRestartImitator()
		{
			GKManager.UpdateConfiguration();
			SaveConfigToFile(true);
			KillProcess("GKImitator");
			CheckTime(() => RunProcess("GKImitator", "GKImitatorPath"), "Запуск имитатора");
			var connectionStatus = CheckTime<string>(ImitatorManager.Connect, "Подключение к имитатору");
			CheckTime(ClientManager.FiresecService.SetLocalConfig, "Загрузка конфигурации на сервер");
			ClientManager.StartPoll();
		}

		void InitializeRootDevices()	
		{
			var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			Assert.IsNotNull(systemDriver, "В GKManager.Drivers не найден драйвер System");
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { Driver = systemDriver, DriverUID = systemDriver.UID };
			gkDevice1 = GKManager.AddDevice(systemDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			var ipProperty = gkDevice1.Properties.FirstOrDefault(x => x.Name == ("IPAddress"));
			if (ipProperty != null)
			{
				ipProperty.StringValue = "127.0.0.1";
			}
			kauDevice11 = GKManager.AddDevice(gkDevice1, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice12 = GKManager.AddDevice(gkDevice1, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);
		}

		static string SaveConfigToFile(bool isLocal)
		{
			try
			{
				var tempFolderName = AppDataFolderHelper.GetServerAppDataPath("Config");
				if (!Directory.Exists(tempFolderName))
					Directory.CreateDirectory(tempFolderName);
				TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
				AddConfiguration(tempFolderName, "GKDeviceConfiguration.xml", GKManager.DeviceConfiguration);
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigManager.SaveAllConfigToFile");
			}
			return null;
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion = 1, int majorVersion = 1)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			var filePath = Path.Combine(folderName, name);
			if (File.Exists(filePath))
				File.Delete(filePath);
			ZipSerializeHelper.Serialize(configuration, filePath);
			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		void WaitWhileState(GKBase gkBase, XStateClass gkState, int milliseconds)
		{
			int timeOut = 0;
			while (gkBase.State.StateClass != gkState && timeOut < milliseconds)
			{
				Thread.Sleep(50);
				timeOut += 50;
			}
		}

		public void RunProcess(string processName, string processPathName) // sync (max 10 sec)
		{
			try
			{
				if (!CheckProcessIsRunning(processName))
				{
					var processPath = RegistrySettingsHelper.GetString(processPathName);
					if (!String.IsNullOrEmpty(processPath))
					{
						Process.Start(processPath);
					}
				}
				for (int i = 0; i < 10; i ++)
				{
					Thread.Sleep(1000);
					if (CheckProcessIsRunning(processName))
						return;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool CheckProcessIsRunning(string processName)
		{
			try
			{
				var processes = Process.GetProcessesByName(processName);
				var processes2 = Process.GetProcessesByName(processName + ".vshost");
				return processes.Any() || processes2.Any(x => x.Threads.Count > 20);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void KillProcess(string processName)
		{
			try
			{
				if (CheckProcessIsRunning(processName))
				{
					var processes = Process.GetProcessesByName(processName);
					var processes2 = Process.GetProcessesByName(processName + ".vshost");
					processes.ForEach(x => x.Kill());
					processes2.ForEach(x => x.Kill());
				}
				for (int i = 0; i < 10; i++)
				{
					Thread.Sleep(1000);
					if (!CheckProcessIsRunning(processName))
						return;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		T CheckTime<T>(Func<T> a, string traceMessage)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var t = a();
			stopwatch.Stop();
			Trace.WriteLine(traceMessage + "=" + stopwatch.Elapsed);
			return t;
		}

		void CheckTime(Action a, string traceMessage)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			a();
			stopwatch.Stop();
			Trace.WriteLine(traceMessage + "=" + stopwatch.Elapsed);
		}

		void InitializeStates()
		{
			var gkStates = ClientManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
		}

		void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			Dispatcher.CurrentDispatcher.Invoke(() => CopyGKStates(gkCallbackResult.GKStates));
		}

		public void OnNewJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			if (isNew)
			{
				ApplicationService.Invoke(() => jourlnalItems.AddRange(journalItems));
			}
		}

		void CopyGKStates(GKStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
					device.State.OnStateChanged();
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = GKManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					remoteZoneState.CopyTo(zone.State);
					zone.State.OnStateChanged();
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = GKManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					remoteDirectionState.CopyTo(direction.State);
					direction.State.OnStateChanged();
				}
			}
			foreach (var remotePumpStationState in gkStates.PumpStationStates)
			{
				var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
				if (pumpStation != null)
				{
					remotePumpStationState.CopyTo(pumpStation.State);
					pumpStation.State.OnStateChanged();
				}
			}
			foreach (var remoteMPTState in gkStates.MPTStates)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == remoteMPTState.UID);
				if (mpt != null)
				{
					remoteMPTState.CopyTo(mpt.State);
					mpt.State.OnStateChanged();
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = GKManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
					delay.State.OnStateChanged();
				}
				else
				{
					delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.UID == delayState.UID);
					if (delay == null)
						delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
					if (delay != null)
					{
						delayState.CopyTo(delay.State);
						delay.State.OnStateChanged();
					}
				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim == null)
					pim = GKManager.GlobalPims.FirstOrDefault(x => x.DeviceUid == remotePimState.ReferenceUid);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
					pim.State.OnStateChanged();
				}
			}
			foreach (var remoteGuardZoneState in gkStates.GuardZoneStates)
			{
				var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == remoteGuardZoneState.UID);
				if (guardZone != null)
				{
					remoteGuardZoneState.CopyTo(guardZone.State);
					guardZone.State.OnStateChanged();
				}
			}
			foreach (var remoteDoorState in gkStates.DoorStates)
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == remoteDoorState.UID);
				if (door != null)
				{
					remoteDoorState.CopyTo(door.State);
					door.State.OnStateChanged();
				}
			}
			foreach (var remoteSKDZoneState in gkStates.SKDZoneStates)
			{
				var skdZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == remoteSKDZoneState.UID);
				if (skdZone != null)
				{
					remoteSKDZoneState.CopyTo(skdZone.State);
					skdZone.State.OnStateChanged();
				}
			}
			foreach (var deviceMeasureParameter in gkStates.DeviceMeasureParameters)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceMeasureParameter.DeviceUID);
				if (device != null)
				{
					device.State.XMeasureParameterValues = deviceMeasureParameter.MeasureParameterValues;
					device.State.OnMeasureParametersChanged();
				}
			}
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddDevice(device.Children[1], GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}
	}
}
