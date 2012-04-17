using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class GkDB
	{
		short currentChildNo = 1;
		short NextChildNo
		{
			get { return currentChildNo++; }
		}

		public XDevice GkDevice { get; set; }
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }

		public GkDB(XDevice gkDevice)
		{
			GkDevice = gkDevice;

			Devices = new List<XDevice>();
			Zones = new List<XZone>();

			AddDevice(gkDevice);

			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKIndicator)
				{
					device.InternalKAUNo = NextChildNo;
					Devices.Add(device);
				}
			}
		}

		public void AddDevice(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GKIndicator)
				return;

			device.InternalGKUNo = NextChildNo;
			Devices.Add(device);
		}

		public void AddZone(XZone zone)
		{
			zone.InternalGKUNo = NextChildNo;
			Zones.Add(zone);
		}
	}
}