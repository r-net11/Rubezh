using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class Battery_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xD6,
				DriverType = XDriverType.Battery,
				UID = new Guid("0D24D46E-11E5-41E2-B0E8-19002F2AB295"),
				Name = "Адресный источник питания ИВЭПР-RSR",
				ShortName = "ИВЭПР",
				IsPlaceable = true
			};

			GKDriversHelper.AddDefaultStateBitsClasses(driver);

			var property1 = new XDriverProperty()
			{
				No = 0x84,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 1,
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "1 АКБ",
				Value = 1
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "2 АКБ",
				Value = 2
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			driver.Properties.Add(property1);
			
			return driver;
		}
	}
}