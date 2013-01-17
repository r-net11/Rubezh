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

			foreach (var child in KauChildrenHelper.GetRealChildren(kauDevice))
			{
				child.KauDatabaseParent = kauDevice;
				AddDevice(child);
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