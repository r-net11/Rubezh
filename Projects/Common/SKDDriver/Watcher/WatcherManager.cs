using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;

namespace SKDDriver
{
	public static class WatcherManager
	{
		public static List<Watcher> Watchers { get; private set; }

		public static void Start()
		{
			SKDManager.CreateStates();

			Watchers = new List<Watcher>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
				{
					var deviceWatcher = new Watcher(device);
					Watchers.Add(deviceWatcher);
					deviceWatcher.StartThread();
				}
			}

			VideoWatcher.Start();

			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
			ApplicationService.Restarting += new Action(ApplicationService_Restarting);
		}

		public static void Stop()
		{
			try
			{
				if (Watchers != null)
					foreach (var watcher in Watchers)
						watcher.StopThread();

				VideoWatcher.Stop();
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalWatcherManager.Stop");
			}
		}

		public static void Send(Action<SendResult> onCompleted, SKDDevice controllerDevice, List<byte> data)
		{
			if (controllerDevice != null)
			{
				var watcher = Watchers.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
				if (watcher != null)
				{
					watcher.AddTask(() =>
					{
						var sendResult = SKDDeviceProcessor.SendBytes(controllerDevice, data);
						onCompleted(sendResult);
					});
				}
				else
				{
					Logger.Error("JournalWatcherManager.Send watcher = null " + controllerDevice.UID.ToString());
				}
			}
			else
			{
				//GKProcessorManager.AddGKMessage(EventNameEnum.Ошибка_при_выполнении_команды_над_устройством, EventDescription.Не_найдено_родительское_устройство_ГК, null, null);
			}
		}

		static void ApplicationService_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!e.Cancel)
			{
				Stop();
			}
		}
		static void ApplicationService_Restarting()
		{
			Stop();
		}
	}
}