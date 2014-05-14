using System.Collections.Generic;
using System.Linq;

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
				Default = "6",
				BitOffset = 4
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Охранный шлейф 300 мс", 6);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "Охранный шлейф 70 мс", 7);

			driver.Properties.Add(property1);
			driver.PresenterKeyPropertyName = "GuardType";
		}
	}
}