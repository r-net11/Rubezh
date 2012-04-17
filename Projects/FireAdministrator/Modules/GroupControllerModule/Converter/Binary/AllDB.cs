using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class AllDB
	{
		public List<KauDB> KauDBs { get; set; }
		public List<GkDB> GKDBs { get; set; }

		public void Build()
		{
			CreateDBs();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsDeviceOnShleif)
				{
					var kauDevice = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
					var kauDB = KauDBs.FirstOrDefault(x => x.KauDevice.UID == kauDevice.UID);
					if (kauDevice.UID == kauDB.KauDevice.UID)
						kauDB.AddDevice(device);
				}
			}
		}

		void CreateDBs()
		{
			KauDBs = new List<KauDB>();
			GKDBs = new List<GkDB>();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.KAU)
				{
					var kauDB = new KauDB(device);
					KauDBs.Add(kauDB);
				}

				if (device.Driver.DriverType == XDriverType.GK)
				{
					var gkDBs = new GkDB(device);
					GKDBs.Add(gkDBs);
				}
			}
		}
	}
}