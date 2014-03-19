using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

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
			foreach(var device in SKDManager.Devices)
			{
				Watcher.AddObjectStateToSKDStates(skdStates, device);
			}
			return skdStates;
		}

		public static string SKDGetDeviceInfo(SKDDevice device, string userName)
		{
			AddMessage("Запрос информации об устройсве", "", device, userName, true);
			var result = DeviceBytesHelper.GetInfo(device);
			if (result == null)
				result = "Устройство недоступно";
			return result;
		}

		public static bool SKDSyncronyseTime(SKDDevice device, string userName)
		{
			AddMessage("Синхронизациявремени", "", device, userName, true);
			return DeviceBytesHelper.SynchroniseTime(device);
		}

		public static OperationResult<bool> GKWriteConfiguration(SKDDevice device, string userName)
		{
			AddMessage("Запись конфигурации в прибор", "", device, userName, true);
			Stop();
			// Write
			Start();
			return new OperationResult<bool>() { Result = true };
		}

		public static OperationResult<bool> GKUpdateFirmware(SKDDevice device, string fileName, string userName)
		{
			Stop();
			// Update
			Start();
			return new OperationResult<bool> { Result = true };
		}

		public static void AddMessage(string name, string description, SKDDevice device, string userName, bool isAdministrator = false)
		{
			var journalItem = new SKDJournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				Name = name,
				Description = description,
				UserName = userName,
			};

			var skdCallbackResult = new SKDCallbackResult();
			skdCallbackResult.JournalItems.Add(journalItem);
			OnSKDCallbackResult(skdCallbackResult);
		}
	}
}