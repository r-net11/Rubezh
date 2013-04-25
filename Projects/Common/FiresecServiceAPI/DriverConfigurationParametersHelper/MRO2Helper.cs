using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MRO2Helper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO_2);
			if (driver != null)
			{
				driver.HasConfigurationProperties = true;
				ConfigurationDriverHelper.AddIntProprety(driver, 0x82, "Количество повторов", "Количество повторов", 0, 3, 0, 255);
				ConfigurationDriverHelper.AddIntProprety(driver, 0x83, "Сообщений в памяти", "Сообщений в памяти", 0, 1, 0, 255).IsReadOnly = true;
				ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Сопротивление выходной линии, Ом", "Сопротивление выходной линии, Ом", 0, 0, 0, 255).IsReadOnly = true;
				ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x87, "Режим работы", 0, "Ведущий", "Ведомый", 0, 0, 0, false, false, "1");
				ConfigurationDriverHelper.AddIntProprety(driver, 0x88, "Задержка включения, с", "Задержка включения, с", 0, 0, 0, 255);
			}
		}
	}
}