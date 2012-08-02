using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK.Obsolete
{
	public class GkDatabase : CommonDatabase
	{
		public GkDatabase(XDevice gkDevice)
		{
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkDevice;

			AddDevice(gkDevice);

			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKIndicator)
				{
					AddDevice(device);
				}
			}

			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKLine)
				{
					AddDevice(device);
				}
			}

			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
		}
	}
}