using System.Linq;
using XFiresecAPI;

namespace GKModule.Database
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(XDevice kauDevice)
		{
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AddDevice(kauDevice);

			var indicatorDevice = kauDevice.Children.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAUIndicator);
			AddDevice(indicatorDevice);
		}
	}
}