using System;
using XFiresecAPI;

namespace Common.GK
{
	public class AMP_1_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x50,
				DriverType = XDriverType.AMP_1,
				UID = new Guid("d8997f3b-64c4-4037-b176-de15546ce568"),
				Name = "Пожарная адресная метка АМП",
				ShortName = "АМП",
				HasZone = true,
                CanPlaceOnPlan = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

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
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Тип включения выхода при пожаре",
				Caption = "Тип включения выхода при пожаре",
				Default = 2
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
			driver.Properties.Add(property2);

			return driver;
		}
	}
}