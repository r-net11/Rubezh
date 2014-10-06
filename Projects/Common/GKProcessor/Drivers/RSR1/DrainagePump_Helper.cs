using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class DrainagePump_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x70,
				DriverType = GKDriverType.DrainagePump,
				UID = new Guid("FF1245BF-C923-4751-9A75-BDFC18CA0996"),
				Name = "Шкаф управления дренажным насосом",
				ShortName = "Дренажный насос",
				IsControlDevice = true,
				HasLogic = false,
				IsPlaceable = true,
				MaxAddressOnShleif = 15,
				IsIgnored = true,
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);


			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			var property3 = new GKDriverProperty()
			{
				No = 0x8d,
				Name = "Тип контакта датчика ВУ",
				Caption = "Тип контакта датчика ВУ",
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsLowByte = true,
				Mask = 1
			};
			property3.Parameters.Add(new GKDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property3.Parameters.Add(new GKDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 1 });
			driver.Properties.Add(property3);

			var property4 = new GKDriverProperty()
			{
				No = 0x8d,
				Name = "Тип контакта датчика НУ",
				Caption = "Тип контакта датчика НУ",
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsLowByte = true,
				Mask = 2
			};
			property4.Parameters.Add(new GKDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property4.Parameters.Add(new GKDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 2 });
			driver.Properties.Add(property4);

			var property5 = new GKDriverProperty()
			{
				No = 0x8d,
				Name = "Тип контакта датчика АУ",
				Caption = "Тип контакта датчика АУ",
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsLowByte = true,
				Mask = 4
			};
			property5.Parameters.Add(new GKDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property5.Parameters.Add(new GKDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 4 });
			driver.Properties.Add(property5);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x80, Name = "Режим работы" });
			return driver;
		}
	}
}
