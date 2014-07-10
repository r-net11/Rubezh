using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.Journal;

namespace SKDDriver
{
	public static class SKDProcessorManager
	{
		public static void OnSKDCallbackResult(SKDCallbackResult skdCallbackResult)
		{
			if (skdCallbackResult.JournalItems.Count +
				skdCallbackResult.SKDStates.DeviceStates.Count > 0)
			{
				if (SKDCallbackResultEvent != null)
					SKDCallbackResultEvent(skdCallbackResult);
			}
		}
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;

		#region Main
		public static bool MustMonitor = false;

		public static void Start()
		{
			WatcherManager.Start();
		}

		public static void Stop()
		{
			WatcherManager.Stop();
		}

		static void SuspendMonitoring(SKDDevice device)
		{
			if (WatcherManager.Watchers != null && device != null)
			{
				var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.Device.UID == device.UID);
				if (watcher != null)
					watcher.Suspend();
			}
		}

		static void ResumeMonitoring(SKDDevice device)
		{
			if (WatcherManager.Watchers != null && device != null)
			{
				var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.Device.UID == device.UID);
				if (watcher != null)
					watcher.Resume();
			}
		}

		#endregion

		public static SKDStates SKDGetStates()
		{
			var skdStates = new SKDStates();
			foreach (var device in SKDManager.Devices)
			{
				skdStates.DeviceStates.Add(device.State);
				//Watcher.AddDeviceStateToSKDStates(skdStates, device);
			}
			foreach (var zone in SKDManager.Zones)
			{
				skdStates.ZoneStates.Add(zone.State);
				//Watcher.AddZoneStateToSKDStates(skdStates, zone);
			}
			return skdStates;
		}

		public static string SKDGetDeviceInfo(SKDDevice device, string userName)
		{
			AddMessage(JournalEventNameType.Запрос_информации_об_устройстве, device, userName, true);
			var result = AdministratorHelper.GetInfo(device);
			if (result == null)
				result = "Устройство недоступно";
			return result;
		}

		public static bool SKDSyncronyseTime(SKDDevice device, string userName)
		{
			AddMessage(JournalEventNameType.Синхронизация_времени, device, userName, true);
			return AdministratorHelper.SynchroniseTime(device);
		}

		public static OperationResult<bool> SKDWriteConfiguration(SKDDevice device, string userName)
		{
			AddMessage(JournalEventNameType.Запись_конфигурации_в_прибор, device, userName, true);
			OperationResult<bool> result;
			Stop();
			result = AdministratorHelper.WriteConfig(device);
			Start();
			return result;
		}

		public static OperationResult<bool> SKDUpdateFirmware(SKDDevice device, string fileName, string userName)
		{
			AddMessage(JournalEventNameType.Обновление_ПО_прибора, device, userName, true);
			OperationResult<bool> result;
			Stop();
			result = AdministratorHelper.UpdateFirmware(device);
			Start();
			return result;
		}

		public static void AddMessage(JournalEventNameType journalEventNameType, SKDDevice device, string userName, bool isAdministrator = false)
		{
			AddMessage(journalEventNameType, FiresecAPI.GK.EventDescription.NULL, device, userName, isAdministrator);
		}

		public static void AddMessage(JournalEventNameType journalEventNameType, FiresecAPI.GK.EventDescription description, SKDDevice device, string userName, bool isAdministrator = false)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				Description = description,
				ObjectName = device.Name,
				ObjectUID = device.UID,
				JournalObjectType = JournalObjectType.GKDevice,
				UserName = userName,
			};

			var skdCallbackResult = new SKDCallbackResult();
			skdCallbackResult.JournalItems.Add(journalItem);
			OnSKDCallbackResult(skdCallbackResult);
		}

		public static void SendControlCommand(SKDDevice device, byte code)
		{
			var bytes = new List<byte>();
			bytes.Add(8);
			bytes.Add(code);
			WatcherManager.Send(new Action<SendResult>(sendResult =>
			{
				if (sendResult.HasError)
				{
					//GKProcessorManager.AddGKMessage(EventNameEnum.Ошибка_при_выполнении_команды, description, xBase, null);
				}
			}),
			device, bytes);
		}
	}
}