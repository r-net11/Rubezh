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
				Name = "Конфигурация модуля",
				Caption = "Конфигурация модуля",
				Default = "4"
			};
			ConfigurationDriverHelper.AddPropertyParameter(property1, "4 контроль одного нормально-замкнутого контакта", 4);
			ConfigurationDriverHelper.AddPropertyParameter(property1, "5 контроль одного нормально-разомкнутого контакта", 5);
			driver.Properties.Add(property1);

		}
	}
}