using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_AM_1_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xDB,
				DriverType = XDriverType.RSR2_AM_1,
				UID = new Guid("860F3F6A-9EE9-437B-8220-B66AFDDBD285"),
				Name = "Пожарная адресная метка АМ-1 RSR2",
				ShortName = "АМ-1 RSR2",
				HasZone = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire2);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			var property1 = new XDriverProperty()
			{
				No = 0,
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
			var property1Parameter5 = new XDriverPropertyParameter()
			{
				Name = "Охранный",
				Value = 4
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			property1.Parameters.Add(property1Parameter4);
			property1.Parameters.Add(property1Parameter5);
			driver.Properties.Add(property1);

			return driver;
		}
	}
}