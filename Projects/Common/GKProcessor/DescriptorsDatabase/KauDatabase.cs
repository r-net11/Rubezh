using System.Collections.Generic;
using XFiresecAPI;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public List<XDevice> Devices { get; set; }

		public KauDatabase(XDevice kauDevice)
		{
			Devices = new List<XDevice>();
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AllDevices = new List<XDevice>();
			AddChild(RootDevice);

			foreach (var device in AllDevices)
			{
				device.KauDatabaseParent = RootDevice;
				device.KAUDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}
		}

		List<XDevice> AllDevices;
		void AddChild(XDevice device)
		{
			if (device.IsNotUsed)
				return;

			if (device.IsRealDevice)
				AllDevices.Add(device);

			foreach (var child in device.Children)
			{
				AddChild(child);
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
			Descriptors.ForEach(x => x.InitializeAllBytes());
		}
	}
}