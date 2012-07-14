using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public static class DriverConfigurationParametersHelper
	{
		public static void CreateKnownProperties(List<Driver> drivers)
		{
			foreach (var driverType in new List<DriverType>() { DriverType.RM_1, DriverType.RM_2, DriverType.RM_3, DriverType.RM_4, DriverType.RM_5 })
			{
				var driver = drivers.FirstOrDefault(x => x.DriverType == driverType);
				RMHelper.Create(driver);
			}
			MROHelper.Create(drivers);
			AMP4Helper.Create(drivers);
			MDUHelper.Create(drivers);
			BUZHelper.Create(drivers);
			foreach (var driverType in new List<DriverType>() { DriverType.Pump, DriverType.JokeyPump, DriverType.Compressor, DriverType.DrenazhPump, DriverType.CompensationPump })
			{
				var driver = drivers.FirstOrDefault(x => x.DriverType == driverType);
				BUNHelper.Create(driver);
			}
			MPTHelper.Create(drivers);
			DetectorsHelper.Create(drivers);
		}
	}
}