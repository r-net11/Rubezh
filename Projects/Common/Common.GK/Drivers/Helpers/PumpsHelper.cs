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
		}

		static void Update(XDriverType driverType, int maxAddressOnShleif)
		{
			var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == driverType);
			if (driver != null)
			{
				driver.IsRangeEnabled = false;
				driver.MaxAddressOnShleif = maxAddressOnShleif;
			}
		}
	}
}