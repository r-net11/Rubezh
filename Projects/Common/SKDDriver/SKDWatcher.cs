using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using XFiresecAPI;

namespace SKDDriver
{
	public static class SKDWatcher
	{
		public static List<DeviceWatcher> DeviceWatchers { get; private set; }

		public static void Start()
		{
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
				{
					var deviceWatcher = new DeviceWatcher(device);
					DeviceWatchers.Add(deviceWatcher);
					deviceWatcher.StartThread();
				}
			}
		}

		public static void Stop()
		{

		}
	}
}