using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models
{

	public class AM1_O_Helper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = "6"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "0 Шлейф неадресных тепловых извещателей, контакты нормально замкнутые, один оконечный резистор", 6);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "1 Шлейф неадресных тепловых извещателей, контакты нор-мально замкнутые, один око-нечный резистор. Параллельно каждому извещателю допол-нительно ставится резистор", 7);
			driver.Properties.Add(property1);
		}
	}
}
