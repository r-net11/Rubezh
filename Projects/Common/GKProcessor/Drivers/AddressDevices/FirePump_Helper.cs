using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class FirePump_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x70,
				DriverType = XDriverType.FirePump,
				UID = new Guid("8bff7596-aef4-4bee-9d67-1ae3dc63ca94"),
				Name = "Шкаф управления пожарным насосом",
				ShortName = "Пожарный насос",
				IsControlDevice = true,
				HasLogic = false,
				IsPlaceable = true,
				MaxAddressOnShleif = 15
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);


			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Время ожидания ВнР, c", 3, 3, 30);

			var property3 = new XDriverProperty()
			{
				No = 0x8d,
				Name = "Тип контакта датчика ВнР",
				Caption = "Тип контакта датчика ВнР",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsLowByte = true,
				Mask = 1
			};
			property3.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property3.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 1 });
			driver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x8d,
				Name = "Тип контакта кнопки ПУСК",
				Caption = "Тип контакта кнопки ПУСК",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsLowByte = true,
				Mask = 2
			};
			property4.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property4.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 2 });
			driver.Properties.Add(property4);

			var property5 = new XDriverProperty()
			{
				No = 0x8d,
				Name = "Тип контакта кнопки СТОП",
				Caption = "Тип контакта кнопки СТОП",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsLowByte = true,
				Mask = 4
			};
			property5.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property5.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 4 });
			driver.Properties.Add(property5);

			var property6 = new XDriverProperty()
			{
				No = 0x8d,
				Name = "Дистанционное управление",
				Caption = "Дистанционное управление",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsHieghByte = true,
				Mask = 8
			};
			property6.Parameters.Add(new XDriverPropertyParameter() { Name = "Нет", Value = 0 });
			property6.Parameters.Add(new XDriverPropertyParameter() { Name = "Есть", Value = 8 });
			driver.Properties.Add(property6);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x80, Name = "Режим работы" });
			return driver;
		}
	}
}
