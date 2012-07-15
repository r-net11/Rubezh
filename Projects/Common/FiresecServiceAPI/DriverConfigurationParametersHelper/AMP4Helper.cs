using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class AMP4Helper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.AMP_4);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x81,
				Name = "Тип шлейфа",
				Caption = "Тип шлейфа",
				Default = "0"
			};
			var property1Parameter1 = new DriverPropertyParameter()
			{
				Name = "Шлейф дымовых датчиков с определением двойной сработки",
				Value = (0*16).ToString()
			};
			var property1Parameter2 = new DriverPropertyParameter()
			{
				Name = "Комбинированный шлейф дымовых и тепловых датчиков без определения двойной сработки тепловых датчиков и с определением двойной сработки дымовых",
				Value = (1 * 16).ToString()
			};
			var property1Parameter3 = new DriverPropertyParameter()
			{
				Name = "Шлейф тепловых датчиков с определением двойной сработки",
				Value = (2 * 16).ToString()
			};
			var property1Parameter4 = new DriverPropertyParameter()
			{
				Name = "Комбинированный шлейф дымовых и тепловых датчиков без определения двойной сработки и без контроля короткого замыкания ШС",
				Value = (3 * 16).ToString()
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			property1.Parameters.Add(property1Parameter4);
			driver.Properties.Add(property1);

			var property2 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x81,
				Name = "Тип включения выхода при пожаре",
				Caption = "Тип включения выхода при пожаре",
				Default = "0"
			};
			var property2Parameter1 = new DriverPropertyParameter()
			{
				Name = "Не включать",
				Value = "0"
			};
			var property2Parameter2 = new DriverPropertyParameter()
			{
				Name = "Переключается",
				Value = "1"
			};
			var property2Parameter3 = new DriverPropertyParameter()
			{
				Name = "Включен постоянно",
				Value = "2"
			};
			property2.Parameters.Add(property2Parameter1);
			property2.Parameters.Add(property2Parameter2);
			property2.Parameters.Add(property2Parameter3);
			driver.Properties.Add(property2);
		}
	}
}