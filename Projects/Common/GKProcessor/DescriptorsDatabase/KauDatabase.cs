using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using System.Linq;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(GKDevice kauDevice)
		{
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AllDevices = new List<GKDevice>();
			AddChild(RootDevice);

			foreach (var device in AllDevices)
			{
				device.KauDatabaseParent = RootDevice;
				Devices.Add(device);
			}
		}

		List<GKDevice> AllDevices;
		void AddChild(GKDevice device)
		{
			if (device.IsNotUsed)
				return;

			AllDevices.Add(device);

			foreach (var child in device.Children)
			{
				AddChild(child);
			}
		}

		public override void BuildObjects()
		{
			foreach (var device in Devices)
			{
				device.KAUDescriptorNo = NextDescriptorNo;
			}
		}
	}
}