using System.Collections.Generic;

namespace FiresecAPI.Models
{
	public class DetectorsHelper
	{
		public static void Create(List<Driver> drivers)
		{
			foreach (var driver in drivers)
			{
				if (driver.DriverType == DriverType.SmokeDetector)
				{
					driver.HasConfigurationProperties = true;

					ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму", 0, 0, 0, 255);
				}
				if (driver.DriverType == DriverType.HeatDetector)
				{
					driver.HasConfigurationProperties = true;

					ConfigurationDriverHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре", 0, 0, 0, 85);
				}
				if (driver.DriverType == DriverType.CombinedDetector)
				{
					driver.HasConfigurationProperties = true;

					ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму", 0, 0, 0, 255);
					ConfigurationDriverHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре", 0, 0, 0, 85);
					ConfigurationDriverHelper.AddIntProprety(driver, 0x8C, "Порог срабатывания по градиенту температуры", 0, 0, 0, 255);
				}
			}
		}
	}
}