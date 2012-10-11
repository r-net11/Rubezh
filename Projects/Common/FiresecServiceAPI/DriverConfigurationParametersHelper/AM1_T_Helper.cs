using System.Collections.Generic;
using System.Linq;


namespace FiresecAPI.Models
{
	public class AM1_T_Helper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_T);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = "4"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "4 Один контакт, нормально замкнутый", 4);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "5 Один контакт, нормально разомкнутый", 5);
			driver.Properties.Add(property1);

		}
	}
}