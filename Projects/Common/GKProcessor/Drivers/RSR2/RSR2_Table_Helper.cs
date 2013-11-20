using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_Table_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE3,
				DriverType = XDriverType.RSR2_Table,
				UID = new Guid("A8AC8A76-064B-4ED0-A071-37B4A15958A7"),
				Name = "Адресная табличка",
				ShortName = "ОПОПс",
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

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 5, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 16, 1, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на выключение, с", 5, 1, 65535);

			var property1 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние для модуля Выключено",
				Caption = "Состояние для модуля Выключено",
				Default = 0,
				IsLowByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property1, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Контакт НЗ", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Контакт переключается", 2);
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние для режима Удержания",
				Caption = "Состояние для режима Удержания",
				Default = 0,
				IsLowByte = true,
				Mask = 0x0C
			};
			GKDriversHelper.AddPropertyParameter(property2, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Контакт НЗ", 4);
			GKDriversHelper.AddPropertyParameter(property2, "Контакт переключается", 8);
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние для режима Включено",
				Caption = "Состояние для режима Включено",
				Default = 0,
				IsLowByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property3, "Гашение", 0);
			GKDriversHelper.AddPropertyParameter(property3, "Горение", 16);
			GKDriversHelper.AddPropertyParameter(property3, "Включено", 32);
			driver.Properties.Add(property3);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Отсчет удержания, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Отсчет задержки на выключение, с", IsDelay = true });

			return driver;
		}
	}
}