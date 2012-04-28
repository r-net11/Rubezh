using System.Collections.Generic;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class BaseBD
	{
		short currentChildNo = 1;
		protected short NextChildNo
		{
			get { return currentChildNo++; }
		}

		public XDevice RootDevice { get; protected set; }
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<KauBinaryObject> BinaryObjects { get; set; }

		public BaseBD()
		{
			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			BinaryObjects = new List<KauBinaryObject>();
		}

		public virtual void AddDevice(XDevice device)
		{
			if (Devices.Contains(device))
				return;

			device.InternalKAUNo = NextChildNo;
			Devices.Add(device);
		}

		public virtual void AddZone(XZone zone)
		{
			if (Zones.Contains(zone))
				return;

			zone.InternalKAUNo = NextChildNo;
			Zones.Add(zone);
		}
	}
}