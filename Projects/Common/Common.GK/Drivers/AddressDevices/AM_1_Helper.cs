using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class AM_1_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x51,
				DriverType = XDriverType.AM_1,
				UID = new Guid("dba24d99-b7e1-40f3-a7f7-8a47d4433392"),
				Name = "Пожарная адресная метка АМ-1",
				ShortName = "АМ-1",
				HasZone = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire2);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			var property1 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 1
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Один контакт, нормально замкнутый",
				Value = 0
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Один контакт, нормально разомкнутый",
				Value = 1
			};
			var property1Parameter3 = new XDriverPropertyParameter()
			{
				Name = "Два контакта, нормально замкнутые",
				Value = 2
			};
			var property1Parameter4 = new XDriverPropertyParameter()
			{
				Name = "Два контакта, нормально разомкнутые",
				Value = 3
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			property1.Parameters.Add(property1Parameter4);
			driver.Properties.Add(property1);

			return driver;
		}
	}
}