using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class PumpsHelper
	{
		public static void Create()
		{
			Update(XDriverType.Pump, 8);
			Update(XDriverType.JokeyPump, 8);
			Update(XDriverType.Compressor, 8);
			Update(XDriverType.DrenazhPump, 8);
			Update(XDriverType.CompensationPump, 8);
		}

		static void Update(XDriverType driverType, int maxAddressOnShleif)
		{
			var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == driverType);
			driver.IsRangeEnabled = false;
			driver.MaxAddressOnShleif = maxAddressOnShleif;
		}
	}
}