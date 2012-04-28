using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class GkDB : BaseBD
	{
		public GkDB(XDevice gkDevice)
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

		//public void AddDevice(XDevice device)
		//{
		//    if (Devices.Contains(device))
		//        return;

		//    device.InternalGKUNo = NextChildNo;
		//    Devices.Add(device);
		//}

		//public void AddZone(XZone zone)
		//{
		//    zone.InternalGKUNo = NextChildNo;
		//    Zones.Add(zone);
		//}
	}
}