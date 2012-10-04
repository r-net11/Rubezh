using System.Collections.Generic;
using System.Linq;


namespace FiresecAPI.Models
{
	public class AM_1_Helper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.AM_1);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x81,
				Name = "Конфигурация модуля",
				Caption = "Конфигурация модуля",
				Default = "0"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "0 Отключено/Замкнуто", 0);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "1 Отключено/Мерцает", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "2 Замкнуто/Отключено", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "3 Замкнуто/Мерцает", 3);
			driver.Properties.Add(property1);

		}
	}
}
