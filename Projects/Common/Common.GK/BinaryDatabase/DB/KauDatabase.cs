using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(XDevice kauDevice)
		{
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			foreach (var device in KauChildrenHelper.GetRealChildren(kauDevice))
			{
				device.KauDatabaseParent = kauDevice;
				AddDevice(device);
			}
		}

		public override void BuildObjects()
		{
			BinaryObjects = new List<BinaryObjectBase>();

			foreach (var device in Devices)
			{
				var deviceBinaryObject = new DeviceBinaryObject(device, DatabaseType);
				BinaryObjects.Add(deviceBinaryObject);
			}
		}
	}
}