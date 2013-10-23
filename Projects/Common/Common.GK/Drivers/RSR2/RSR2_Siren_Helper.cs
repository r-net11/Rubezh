using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_Siren_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE4,
				DriverType = XDriverType.RSR2_Siren,
                UID = new Guid("24A6FC19-0428-43A9-8B1C-35B748BD202B"),
				Name = "Адресный оповещатель",
				ShortName = "ОПОПз",
				IsControlDevice = true,
				HasLogic = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);


			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 5, 0, 65535);
            GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 0, 16, 1, 65535);

            var property1 = new XDriverProperty()
            {
                No = 2,
                Name = "Режим после удержания",
                Caption = "Режим после удержания",
                Default = 0,
                IsLowByte = true,
                Mask = 0x01
            };
            GKDriversHelper.AddPropertyParameter(property1, "Выключается", 0);
            GKDriversHelper.AddPropertyParameter(property1, "Остаётся включённым", 1);
            driver.Properties.Add(property1);

            var property2 = new XDriverProperty()
            {
                No = 2,
                Name = "Состояние светодиода в режиме удержание и включено",
                Caption = "Состояние светодиода в режиме удержание и включено",
                Default = 0,
                IsLowByte = true,
                Mask = 0x0C
            };
            GKDriversHelper.AddPropertyParameter(property2, "Контакт НР", 0);
            GKDriversHelper.AddPropertyParameter(property2, "Контакт НЗ", 4);
            GKDriversHelper.AddPropertyParameter(property2, "Контакт переключается", 8);
            driver.Properties.Add(property2);

            driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
            driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Отсчет удержания, с", IsDelay = true });
			
			return driver;
		}
	}
}
