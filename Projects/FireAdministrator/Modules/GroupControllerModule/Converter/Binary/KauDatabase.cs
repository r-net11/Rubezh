using System.Linq;
using XFiresecAPI;
using System.Collections.Generic;

namespace GKModule.Converter.Binary
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(XDevice kauDevice)
		{
			RootDevice = kauDevice;

			AddDevice(kauDevice);

			var indicatorDevice = kauDevice.Children.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAUIndicator);
			AddDevice(indicatorDevice);
		}

		public void BuildObjects()
		{
			BinaryObjects = new List<BinaryObjectBase>();

			foreach (var device in Devices)
			{
				var deviceBinaryObject = new KauBinaryObject(device);
				BinaryObjects.Add(deviceBinaryObject);
			}
			foreach (var zone in Zones)
			{
				var zoneBinaryObject = new ZoneBinaryObject(zone);
				BinaryObjects.Add(zoneBinaryObject);
			}
		}
	}
}