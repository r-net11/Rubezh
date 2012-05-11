using XFiresecAPI;
using System.Collections.Generic;

namespace GKModule.Database
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
				if (device.Driver.DriverType == XDriverType.KAU)
				{
					AddDevice(device);
				}
			}
		}
	}
}