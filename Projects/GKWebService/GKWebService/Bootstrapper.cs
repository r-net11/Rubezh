using GKProcessor;
using GKWebService.DataProviders;
using GKWebService.DataProviders.MPTHubs;
using GKWebService.Models;
using Infrastructure.Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GKWebService.DataProviders.DeviceParametersHub;
using GKWebService.DataProviders.Devices;
using GKWebService.DataProviders.FireZones;
using GKWebService.DataProviders.PumpStations;
using GKWebService.DataProviders.DoorsHub;

namespace GKWebService
{
	public static class Bootstrapper
	{
		private static object syncBootstrapper = new object();

		public static void Run()
		{

			SubscribeOnServiceStateEvents();

			for (int i = 1; i <= 10; i++)
			{
				var message = RubezhClient.ClientManager.Connect(ClientType.WebService, ConnectionSettingsManager.ServerAddress,
					GlobalSettingsHelper.GlobalSettings.WebLogin, GlobalSettingsHelper.GlobalSettings.WebPassword);
				if (message == null)
				{
                    break;
				}
				Thread.Sleep(5000);
				if (i == 10)
				{
					//UILogger.Log("Ошибка соединения с сервером: " + message);
					return;
				}
			}

			InitServer();
		}

		private static void InitServer()
		{
			InitializeGK();
			RubezhClient.ClientManager.StartPoll();
		}

		private static void SubscribeOnServiceStateEvents()
		{
			SafeFiresecService.ConfigurationChangedEvent += SafeFiresecServiceOnConfigurationChangedEvent;
			SafeFiresecService.OnConnectionAppeared += SafeFiresecServiceOnConnectionAppeared;
		}

		private static void SafeFiresecServiceOnConnectionAppeared()
		{
			//InitServer();
			//PlansUpdater.Instance.
		}

		private static void SafeFiresecServiceOnConfigurationChangedEvent()
		{
			InitServer();
			if (ConfigHub.Instance != null) 
				ConfigHub.Instance.ConfigHubUpdate();
		}

		static void InitializeGK()
		{
			RubezhClient.ClientManager.GetConfiguration("GKWEB/Configuration");
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			InitializeStates();

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			SafeFiresecService.JournalItemsEvent -= new Action<List<JournalItem>, bool>(OnNewJournalItem);
			SafeFiresecService.JournalItemsEvent += new Action<List<JournalItem>, bool>(OnNewJournalItem);

			SafeFiresecService.GKPropertyChangedEvent += GKPropertyChangedEvent;

			ShowAllObjects();
		}

		private static void GKPropertyChangedEvent(GKPropertyChangedCallback obj)
		{

		}

		static void InitializeStates()
		{
			var gkStates = RubezhClient.ClientManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
			CopyDeviceMeasureParametersStates(gkStates);
		}

		static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			CopyGKStates(gkCallbackResult.GKStates);
			CopyDeviceMeasureParametersStates(gkCallbackResult.GKStates);
			if (AlarmsUpdaterHub.Instance != null)
			{
				AlarmsUpdaterHub.Instance.BroadcastAlarms();
			}
		}

		static void CopyGKStates(GKStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
					if (DevicesHub.Instance != null)
					{
						DevicesHub.Instance.DevicesUpdate(device);
					}
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = GKManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					remoteZoneState.CopyTo(zone.State);
					if (FireZonesHub.Instance != null)
					{
						FireZonesHub.Instance.BroadcastFireZone(zone);
					}
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = GKManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					remoteDirectionState.CopyTo(direction.State);
					if (DirectionsUpdaterHub.Instance != null)
					{
						DirectionsUpdaterHub.Instance.BroadcastDirection(direction);
					}
				}
			}
			foreach (var remotePumpStationState in gkStates.PumpStationStates)
			{
				var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
				if (pumpStation != null)
				{
					remotePumpStationState.CopyTo(pumpStation.State);
					if (PumpStationsHub.Instance != null)
						PumpStationsHub.Instance.PumpStationstUpdate(pumpStation);
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = GKManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay == null)
					delay = GKManager.Delays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
					if (DelaysUpdaterHub.Instance != null)
						DelaysUpdaterHub.Instance.DelayUpdate(new Delay(delay));
				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
				}
			}
			foreach (var mptState in gkStates.MPTStates)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == mptState.UID);
				if (mpt != null)
				{
					mptState.CopyTo(mpt.State);
					if (MptUpdaterHub.Instance != null)
					{
						MptUpdaterHub.Instance.MptUpdate(mpt);
					}
				}
			}
			foreach (var guardZoneState in gkStates.GuardZoneStates)
			{
				var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == guardZoneState.UID);
				if (guardZone != null)
				{
					guardZoneState.CopyTo(guardZone.State);
					if (GuardZonesHub.Instance != null)
						GuardZonesHub.Instance.GuardZoneUpdate(guardZone);
				}
			}
			foreach (var doorState in gkStates.DoorStates)
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorState.UID);
				if (door != null)
				{
					doorState.CopyTo(door.State);
					if (DoorsHub.Instance != null)
						DoorsHub.Instance.DoorUpdate(door);
				}
			}
		}

		static void CopyDeviceMeasureParametersStates(GKStates gkStates)
		{
			foreach (var deviceMeasureParameter in gkStates.DeviceMeasureParameters)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceMeasureParameter.DeviceUID);
				if (device != null)
				{
					var diff = device.State.XMeasureParameterValues.Where(p => deviceMeasureParameter.MeasureParameterValues.Any(
						l => p.Name == l.Name && p.StringValue != l.StringValue));

					if (device.State.XMeasureParameterValues.Count != deviceMeasureParameter.MeasureParameterValues.Count || diff.Count() > 0)
					{
						device.State.XMeasureParameterValues = deviceMeasureParameter.MeasureParameterValues;

						if (DeviceParametersUpdaterHub.Instance != null)
							DeviceParametersUpdaterHub.Instance.DeviceParameterUpdate(deviceMeasureParameter);
					}
				}
			}
		}

		static void OnNewJournalItem(List<JournalItem> journalItems, bool boool)
		{
			if (JournalUpdaterHub.Instance != null)
				JournalUpdaterHub.Instance.BroadcastNewJournalItems(journalItems);
		}

		static void ShowAllObjects()
		{
			//foreach(var device in GKManager.Devices)
			//{
			//    Trace.WriteLine(device.PresentationName);
			//}
			//foreach (var zone in GKManager.Zones)
			//{
			//    Trace.WriteLine(zone.PresentationName);
			//}
			//foreach (var direction in GKManager.Directions)
			//{
			//    Trace.WriteLine(direction.PresentationName);
			//}
			//foreach (var delay in GKManager.Delays)
			//{
			//    Trace.WriteLine(delay.PresentationName);
			//}
			//foreach (var mpt in GKManager.MPTs)
			//{
			//    Trace.WriteLine(mpt.PresentationName);
			//}
			//foreach (var pumpStation in GKManager.PumpStations)
			//{
			//    Trace.WriteLine(pumpStation.PresentationName);
			//}
			//foreach (var guardZone in GKManager.GuardZones)
			//{
			//    Trace.WriteLine(guardZone.PresentationName);
			//}
			//foreach (var door in GKManager.Doors)
			//{
			//    Trace.WriteLine(door.PresentationName);
			//}
		}
	}
}