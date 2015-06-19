using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GkDatabase : CommonDatabase
	{
		List<GKDoor> Doors { get; set; }
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(GKDevice gkControllerDevice)
		{
			Doors = new List<GKDoor>();
			KauDatabases = new List<KauDatabase>();
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkControllerDevice;

			AddDevice(gkControllerDevice);
			foreach (var device in gkControllerDevice.Children)
			{
				if (device.DriverType == GKDriverType.GKIndicator || device.DriverType == GKDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
			Devices.ForEach(x => x.GkDatabaseParent = RootDevice);
		}

		void AddDevice(GKDevice device)
		{
			if (!Devices.Contains(device))
			{
				Devices.Add(device);
			}
		}

		public override void BuildObjects()
		{
			foreach (var door in GKManager.Doors)
			{
				if (door.GkDatabaseParent == RootDevice)
				{
					Doors.Add(door);
				}
			}

			foreach (var device in Devices)
			{
				device.GKDescriptorNo = NextDescriptorNo;
			}

			foreach (var door in Doors)
			{
				door.GKDescriptorNo = NextDescriptorNo;
			}
		}
	}
}