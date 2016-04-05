using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public static class RSR2_CodeCardReader_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xE9,
				DriverType = GKDriverType.RSR2_CodeCardReader,
				UID = new Guid("D296DEBC-8993-4798-A2BC-0410FB79E366"),
				Name = "Наборник кодовый со считывателем",
				ShortName = "НСЧ",
				IsControlDevice = true,
				HasGuardZone = true,
				HasZone = false,
				IsPlaceable = true
			};

			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			driver.AvailableStateBits.Add(GKStateBit.TurningOn);
			driver.AvailableStateBits.Add(GKStateBit.TurningOff);
			driver.AvailableStateBits.Add(GKStateBit.Failure);
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Attention);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Attention);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на выключение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 3, "Время удержания на выключение, с", 0, 0, 65535);


			var property1 = new GKDriverProperty()
			{
				No = 4,
				Name = "Режим после удержания включенного состояния",
				Caption = "Режим после удержания включенного состояния",
				Default = 0,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property1, "Выключено", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Включено", 1);
			driver.Properties.Add(property1);

			var property2 = new GKDriverProperty()
			{
				No = 4,
				Name = "Наличие реле",
				Caption = "Наличие реле",
				Default = 2,
				IsLowByte = true,
				Mask = 0x02
			};
			GKDriversHelper.AddPropertyParameter(property2, "Нет", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Есть", 2);
			driver.Properties.Add(property2);

			var property3 = new GKDriverProperty()
			{
				No = 4,
				Name = "Состояние контакта для режима Выключено",
				Caption = "Состояние контакта для режима Выключено",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НР", 0x00);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НЗ", 0x01);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт переключается", 0x02);
			driver.Properties.Add(property3);

			var property4 = new GKDriverProperty()
			{
				No = 4,
				Name = "Состояние контакта для режима Удержания",
				Caption = "Состояние контакта для режима Удержания",
				Default = 4,
				IsHieghByte = true,
				Mask = 0x0C
			};
			GKDriversHelper.AddPropertyParameter(property4, "Контакт НР", 0x00);
			GKDriversHelper.AddPropertyParameter(property4, "Контакт НЗ", 0x04);
			GKDriversHelper.AddPropertyParameter(property4, "Контакт переключается", 0x08);
			driver.Properties.Add(property4);

			var property5 = new GKDriverProperty()
			{
				No = 4,
				Name = "Состояние контакта для режима Включено",
				Caption = "Состояние контакта для режима Включено",
				Default = 0x10,
				IsHieghByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property5, "Контакт НР", 0x00);
			GKDriversHelper.AddPropertyParameter(property5, "Контакт НЗ", 0x10);
			GKDriversHelper.AddPropertyParameter(property5, "Контакт переключается", 0x20);
			driver.Properties.Add(property5);

			GKDriversHelper.AddIntProprety(driver, 5, "Длительность удержания сработки, с", 3, 3, 65535);

			return driver;
		}
	}
}