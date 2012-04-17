using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class KauDB
	{
		short currentChildNo = 1;
		short NextChildNo
		{
			get { return currentChildNo++; }
		}

		public XDevice KauDevice { get; private set; }
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<KauBinaryObject> BinaryObjects { get; set; }

		public KauDB(XDevice kauDevice)
		{
			KauDevice = kauDevice;

			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			BinaryObjects = new List<KauBinaryObject>();

			AddDevice(kauDevice);

			var indicatorDevice = kauDevice.Children.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAUIndicator);

			indicatorDevice.InternalKAUNo = NextChildNo;
			Devices.Add(indicatorDevice);
		}

		public void AddDevice(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.KAUIndicator)
				return;

			device.InternalKAUNo = NextChildNo;
			Devices.Add(device);
		}

		public void AddZone(XZone zone)
		{
			zone.InternalKAUNo = NextChildNo;
			Zones.Add(zone);
		}
	}
}