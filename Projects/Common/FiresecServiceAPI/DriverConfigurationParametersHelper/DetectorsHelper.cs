using System.Collections.Generic;

namespace FiresecAPI.Models
{
	public class DetectorsHelper
	{
		public static void Create(List<Driver> drivers)
		{
			foreach (var driver in drivers)
			{
				switch (driver.DriverType)
				{
					case DriverType.SmokeDetector:
						driver.HasConfigurationProperties = true;
						ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму, 0.01 дб/м", "AU_Smoke", 0, 18, 5, 20);
						ConfigurationDriverHelper.AddIntProprety(driver, 0x9A, "Задержка Пожар-2, c", "AU_Fire2", 0, 0, 0, 255);
						break;

					case DriverType.HeatDetector:
						driver.HasConfigurationProperties = true;
						ConfigurationDriverHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре, C", "AU_Temperature", 0, 65, 54, 85);
						ConfigurationDriverHelper.AddIntProprety(driver, 0x9A, "Задержка Пожар-2, c", "AU_Fire2", 0, 0, 0, 255);
						break;

					case DriverType.CombinedDetector:
						driver.HasConfigurationProperties = true;
						ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму, 0.01 дб/м", "AU_Smoke", 0, 18, 5, 20);
						ConfigurationDriverHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре, C", "AU_Temperature", 0, 65, 54, 85);
						ConfigurationDriverHelper.AddIntProprety(driver, 0x9A, "Задержка Пожар-2, c", "AU_Fire2", 0, 0, 0, 255);
						break;
					case DriverType.RadioSmokeDetector:
						driver.HasConfigurationProperties = true;
						ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму, 0.01 дб/м", "AU_Smoke", 0, 18, 5, 20);
						ConfigurationDriverHelper.AddIntProprety(driver, 0x9A, "Задержка Пожар-2, c", "AU_Fire2", 0, 0, 0, 255);
						break;
				}
			}
		}
	}
}