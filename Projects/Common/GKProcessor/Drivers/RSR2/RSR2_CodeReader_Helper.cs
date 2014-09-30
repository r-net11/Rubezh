using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_CodeReader_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE7,
				DriverType = XDriverType.RSR2_CodeReader,
				UID = new Guid("FC8AC44B-6B54-470E-92DC-7ED63E5EA62F"),
				Name = "Наборник кодовый R2",
				ShortName = "НК R2",
				IsControlDevice = true,
				HasZone = false,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Attention);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Attention);

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение 1, с", 1, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания 1, с", 1, 1, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на включение 2, с", 1, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 3, "Время удержания 2, с", 1, 1, 65535);


			var property1 = new XDriverProperty()
			{
				No = 4,
				Name = "Режим после удержания включенного состояния",
				Caption = "Режим после удержания включенного состояния",
				Default = 0,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property1, "Выключается", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Остается включенным", 1);
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 4,
				Name = "Наличие реле",
				Caption = "Наличие реле",
				Default = 0,
				IsLowByte = true,
				Mask = 0x02
			};
			GKDriversHelper.AddPropertyParameter(property2, "Нет", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Есть", 2);
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 4,
				Name = "Контакт реле в состоянии ВЫКЛЮЧЕНО",
				Caption = "Контакт реле в состоянии ВЫКЛЮЧЕНО",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НР", 0x00);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НЗ", 0x01);
			GKDriversHelper.AddPropertyParameter(property3, "Мерцание", 0x02);
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 4,
				Name = "Контакт реле в состоянии УДЕРЖАНИЕ",
				Caption = "Контакт реле в состоянии УДЕРЖАНИЕ",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x0C
			};
			GKDriversHelper.AddPropertyParameter(property4, "Контакт НР", 0x00);
			GKDriversHelper.AddPropertyParameter(property4, "Контакт НЗ", 0x04);
			GKDriversHelper.AddPropertyParameter(property4, "Мерцание", 0x08);
			driver.Properties.Add(property4);

			var property5 = new XDriverProperty()
			{
				No = 4,
				Name = "Контакт реле в состоянии ВКЛЮЧЕНО",
				Caption = "Контакт реле в состоянии ВКЛЮЧЕНО",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property5, "Контакт НР", 0x00);
			GKDriversHelper.AddPropertyParameter(property5, "Контакт НЗ", 0x10);
			GKDriversHelper.AddPropertyParameter(property5, "Мерцание", 0x20);
			driver.Properties.Add(property5);

			GKDriversHelper.AddIntProprety(driver, 5, "Длительность удержания сработки, с", 3, 3, 65535);

			return driver;
		}
	}
}