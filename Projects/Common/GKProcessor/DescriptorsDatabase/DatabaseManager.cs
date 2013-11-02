using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public static class DatabaseManager
	{
		public static List<KauDatabase> KauDatabases { get; private set; }
		public static List<GkDatabase> GkDatabases { get; private set; }

		public static void Convert()
		{
			XManager.Invalidate();
			XManager.Prepare();
			CreateDBs();
			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
		}

		static void CreateDBs()
		{
			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.GK)
				{
					var gkDatabase = new GkDatabase(device);
					GkDatabases.Add(gkDatabase);

					foreach (var kauDevice in device.Children)
					{
						if (kauDevice.Driver.IsKauOrRSR2Kau)
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