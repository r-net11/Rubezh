using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class KauDB : BaseBD
	{
		public KauDB(XDevice kauDevice)
		{
			RootDevice = kauDevice;

			AddDevice(kauDevice);

			var indicatorDevice = kauDevice.Children.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAUIndicator);
			AddDevice(indicatorDevice);
		}

		//public void AddDevice(XDevice device)
		//{
		//    if (Devices.Contains(device))
		//        return;

		//    device.InternalKAUNo = NextChildNo;
		//    Devices.Add(device);
		//}

		//public void AddZone(XZone zone)
		//{
		//    zone.InternalKAUNo = NextChildNo;
		//    Zones.Add(zone);
		//}
	}
}