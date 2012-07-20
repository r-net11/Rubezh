using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Commom.GK
{
	public class AMP4Helper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AMP_4);

			var property1 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Тип шлейфа",
				Caption = "Тип шлейфа",
				Default = 0
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Шлейф дымовых датчиков с определением двойной сработки",
				Value = 0*16
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Комбинированный шлейф дымовых и тепловых датчиков без определения двойной сработки тепловых датчиков и с определением двойной сработки дымовых",
				Value = 1 * 16
			};
			var property1Parameter3 = new XDriverPropertyParameter()
			{
				Name = "Шлейф тепловых датчиков с определением двойной сработки",
				Value = 2 * 16
			};
			var property1Parameter4 = new XDriverPropertyParameter()
			{
				Name = "Комбинированный шлейф дымовых и тепловых датчиков без определения двойной сработки и без контроля короткого замыкания ШС",
				Value = 3 * 16
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			property1.Parameters.Add(property1Parameter4);
			xDriver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Тип включения выхода при пожаре",
				Caption = "Тип включения выхода при пожаре",
				Default = 0
			};
			var property2Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Не включать",
				Value = 0
			};
			var property2Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Переключается",
				Value = 1
			};
			var property2Parameter3 = new XDriverPropertyParameter()
			{
				Name = "Включен постоянно",
				Value = 2
			};
			property2.Parameters.Add(property2Parameter1);
			property2.Parameters.Add(property2Parameter2);
			property2.Parameters.Add(property2Parameter3);
			xDriver.Properties.Add(property2);
		}
	}
}