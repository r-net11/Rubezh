using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public static class RMHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_1);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x82,
				Name = "Конфигурация релейного модуля",
				Caption = "Конфигурация релейного модуля",
				Default = "1"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Отключено/Замкнуто", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Отключено/Мерцает", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Замкнуто/Отключено", 3);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Замкнуто/Мерцает", 4);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Мерцает/Отключено", 5);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Мерцает/Замкнуто", 6);
			driver.Properties.Add(property1);

			ConfigurationDriverHelper.AddIntProprety(driver, 0x83, "Задержка на пуск, с", "AU_Delay", 0, 0, 0, 250);
			ConfigurationDriverHelper.AddIntProprety(driver, 0x83, "Время удержания, с", "AU_Time", 0, 0, 0, 250, false, true);

			var property2 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x85,
				Name = "Контроль выхода",
				Caption = "Контроль выхода",
				MinBit = 0,
				MaxBit = 4,
				Default = "1"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property2, "Не контролируется", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property2, "На обрыв", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property2, "На КЗ", 3);
			ConfigurationDriverHelper.AddPropertyParameter(property2, "На КЗ и обрыв", 4);
			driver.Properties.Add(property2);
		}
	}
}