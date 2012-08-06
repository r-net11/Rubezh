using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class DatabaseManager
	{
		public List<KauDatabase> KauDatabases { get; private set; }
		public List<GkDatabase> GkDatabases { get; private set; }

		public DatabaseManager()
		{
			CreateDBs();
			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
		}

		void CreateDBs()
		{
			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.GK)
				{
					var gkDatabase = new GkDatabase(device);
					GkDatabases.Add(gkDatabase);

					foreach (var kauDevice in device.Children)
					{
						if (kauDevice.Driver.DriverType == XDriverType.KAU)
						{
							var kauDatabase = new KauDatabase(kauDevice);
							gkDatabase.KauDatabases.Add(kauDatabase);
							KauDatabases.Add(kauDatabase);
						}
					}
				}
			}
		}
	}
}