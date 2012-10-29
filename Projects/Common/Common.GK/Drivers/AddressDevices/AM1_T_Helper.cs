using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class AM1_T_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xD2,
				DriverType = XDriverType.AM1_T,
				UID = new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8"),
				Name = "Технологическая адресная метка АМ1-Т",
				ShortName = "АМ1-Т",
				HasZone = false,
                CanPlaceOnPlan = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			var property1 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 5
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Технологический шлейф с контролем одного нормально-замкнутого контакта",
				Value = 4
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Технологический шлейф с контролем одного нормально-разомкнутого контакта",
				Value = 5
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			driver.Properties.Add(property1);

			return driver;
		}
	}
}