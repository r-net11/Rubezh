using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using XFiresecAPI;
using Common;
using Infrastructure.Common.Windows;

namespace SKDDriver
{
	public static class WatcherManager
	{
		public static List<Watcher> Watchers { get; private set; }

		public static void Start()
		{
			foreach (var device in SKDManager.Devices)
			{
				device.State = new SKDDeviceState();
			}

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