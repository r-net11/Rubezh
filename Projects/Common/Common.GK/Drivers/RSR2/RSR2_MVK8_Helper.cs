using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_MVK8_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE2,
				DriverType = XDriverType.RSR2_MVK8,
				UID = new Guid("3E55ACEF-D0D6-443A-A247-E9D5D116429A"),
				Name = "МВК RSR2",
				ShortName = "МВК RSR2",
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

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 10, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 0, 1, 1, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на выключение, с", 0, 1, 1, 65535);

			var property1 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние контакта для режима Выключено",
				Caption = "Состояние контакта для режима Выключено",
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
				Name = "Состояние контакта для режима Удержания",
				Caption = "Состояние контакта для режима Удержания",
				Default = 4,
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
				Name = "Состояние контакта для режима Включено",
				Caption = "Состояние контакта для режима Включено",
				Default = 16,
				IsLowByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НЗ", 16);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт переключается", 32);
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 4,
				Name = "Контроль",
				Caption = "Контроль",
				Default = 3,
				IsLowByte = true,
			};
			GKDriversHelper.AddPropertyParameter(property4, "Без контроля", 0);
			GKDriversHelper.AddPropertyParameter(property4, "Обрыв", 1);
			GKDriversHelper.AddPropertyParameter(property4, "КЗ", 2);
			GKDriversHelper.AddPropertyParameter(property4, "Обрыв и КЗ", 3);
			driver.Properties.Add(property4);

			GKDriversHelper.AddIntProprety(driver, 5, "Норма питания, 0.1В", 0, 80, 1, 1000);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Отсчет удержания, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Отсчет задержки на выключение, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 4, Name = "Питание, 0.1В" });
			driver.AUParameters.Add(new XAUParameter() { No = 5, Name = "Ед АЦП выхода" });

			return driver;
		}
	}
}