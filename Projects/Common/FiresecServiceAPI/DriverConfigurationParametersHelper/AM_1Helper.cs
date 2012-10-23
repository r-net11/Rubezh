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
				Default = "1"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Один контакт, нормально замкнутый", 0);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Один контакт, нормально разомкнутый", 1);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Два контакта, нормально замкнутые", 2);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Два контакта, нормально разомкнутые", 3);
			driver.Properties.Add(property1);

		}
	}
}
