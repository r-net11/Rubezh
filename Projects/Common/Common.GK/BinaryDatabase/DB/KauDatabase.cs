using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
	public class KauDatabase : CommonDatabase
	{
		public List<XDevice> Devices { get; set; }

		public KauDatabase(XDevice kauDevice)
		{
			Devices = new List<XDevice>();
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			foreach (var device in KauChildrenHelper.GetRealChildren(kauDevice))
			{
				device.KauDatabaseParent = kauDevice;
				AddDevice(device);
			}
		}

		public void AddDevice(XDevice device)
		{
			if (!Devices.Contains(device))
			{
				device.KAUDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}
		}

		public override void BuildObjects()
		{
			Descriptors = new List<BaseDescriptor>();
			foreach (var device in Devices)
			{
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}
		}
	}
}