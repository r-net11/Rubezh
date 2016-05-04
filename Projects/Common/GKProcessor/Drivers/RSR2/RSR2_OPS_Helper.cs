using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public static class RSR2_OPS_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xE3,
				DriverType = GKDriverType.RSR2_OPS,
				UID = new Guid("A8AC8A76-064B-4ED0-A071-37B4A15958A7"),
				Name = "Оповещатель пожарный световой адресный",
				ShortName = "ОПС",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.Announcers
			};

			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			driver.AvailableStateBits.Add(GKStateBit.TurningOn);
			driver.AvailableStateBits.Add(GKStateBit.TurningOff);
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на выключение, с", 0, 0, 65535);

			var property1 = new GKDriverProperty()
			{
				No = 3,
				Name = "Состояние для режима Выключено",
				Caption = "Состояние для режима Выключено",
				Default = 0,
				IsLowByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property1, "Не горит", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Горит", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Мерцание", 2);
			driver.Properties.Add(property1);

			var property2 = new GKDriverProperty()
			{
				No = 3,
				Name = "Состояние для режима Удержания",
				Caption = "Состояние для режима Удержания",
				Default = 4,
				IsLowByte = true,
				Mask = 0x0C
			};
			GKDriversHelper.AddPropertyParameter(property2, "Не горит", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Горит", 4);
			GKDriversHelper.AddPropertyParameter(property2, "Мерцание", 8);
			driver.Properties.Add(property2);

			var property3 = new GKDriverProperty()
			{
				No = 3,
				Name = "Состояние для режима Включено",
				Caption = "Состояние для режима Включено",
				Default = 16,
				IsLowByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property3, "Не горит", 0);
			GKDriversHelper.AddPropertyParameter(property3, "Горит", 16);
			GKDriversHelper.AddPropertyParameter(property3, "Мерцание", 32);
			driver.Properties.Add(property3);

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true, IsNotVisible = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Отсчет удержания, с", IsDelay = true, IsNotVisible = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 3, Name = "Отсчет задержки на выключение, с", IsDelay = true, IsNotVisible = true });

			return driver;
		}
	}
}