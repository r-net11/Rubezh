using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public List<GKDevice> Devices { get; set; }

		public KauDatabase(GKDevice kauDevice)
		{
			Devices = new List<GKDevice>();
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AllDevices = new List<GKDevice>();
			AddChild(RootDevice);

			foreach (var device in AllDevices)
			{
				device.KauDatabaseParent = RootDevice;
				device.KAUDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}
		}

		List<GKDevice> AllDevices;
		void AddChild(GKDevice device)
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
				deviceDescriptor.Build();
				Descriptors.Add(deviceDescriptor);
			}
			Descriptors.ForEach(x => x.InitializeAllBytes());
		}
	}
}