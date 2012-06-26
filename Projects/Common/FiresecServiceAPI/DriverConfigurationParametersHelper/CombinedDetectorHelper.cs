using System.Linq;
using XFiresecAPI;
using System.Collections.Generic;

namespace FiresecAPI.Models
{
	public class CombinedDetectorHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.CombinedDetector);

			ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму", 0, 0, 0, 255);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре", 0, 0, 0, 85);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x8C, "Порог срабатывания по градиенту температуры", 0, 0, 0, 255);
		}
	}
}