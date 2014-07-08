using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using JournalItem = FiresecAPI.SKD.JournalItem;
using FiresecAPI.Events;

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
			AddMessage(GlobalEventNameEnum.Запрос_информации_об_устройстве, device, userName, true);
			var result = AdministratorHelper.GetInfo(device);
			if (result == null)
				result = "Устройство недоступно";
			return result;
		}

		public static bool SKDSyncronyseTime(SKDDevice device, string userName)
		{
			AddMessage(GlobalEventNameEnum.Синхронизация_времени, device, userName, true);
			return AdministratorHelper.SynchroniseTime(device);
		}

		public static OperationResult<bool> SKDWriteConfiguration(SKDDevice device, string userName)
		{
			AddMessage(GlobalEventNameEnum.Запись_конфигурации_в_прибор, device, userName, true);
			OperationResult<bool> result;
			Stop();
			result = AdministratorHelper.WriteConfig(device);
			Start();
			return result;
		}

		public static OperationResult<bool> SKDUpdateFirmware(SKDDevice device, string fileName, string userName)
		{
			AddMessage(GlobalEventNameEnum.Обновление_ПО_прибора, device, userName, true);
			OperationResult<bool> result;
			Stop();
			result = AdministratorHelper.UpdateFirmware(device);
			Start();
			return result;
		}

		public static void AddMessage(GlobalEventNameEnum globalEventNameEnum, SKDDevice device, string userName, bool isAdministrator = false)
		{
			AddMessage(globalEventNameEnum, EventDescription.NULL, device, userName, isAdministrator);
		}

		public static void AddMessage(GlobalEventNameEnum globalEventNameEnum, EventDescription description, SKDDevice device, string userName, bool isAdministrator = false)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				Name = globalEventNameEnum,
				Description = description,
				ObjectName = device.Name,
				ObjectUID = device.UID,
				ObjectType = ObjectType.Устройство_СКД,
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