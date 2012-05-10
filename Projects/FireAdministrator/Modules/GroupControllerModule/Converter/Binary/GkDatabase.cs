using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class GkDatabase : CommonDatabase
	{
		public GkDatabase(XDevice gkDevice)
		{
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