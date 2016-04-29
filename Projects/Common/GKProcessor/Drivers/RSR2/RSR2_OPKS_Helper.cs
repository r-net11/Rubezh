using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public static class RSR2_OPKS_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xEB,
				DriverType = GKDriverType.RSR2_OPKS,
				UID = new Guid("8e339f21-e32e-4548-adc3-3ab6c0d2768a"),
				Name = "Оповещатель пожарный световой адресный комбинированный",
				ShortName = "ОПКС",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.Announcers
			};

			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOffNow_InManual);

			var property1 = new GKDriverProperty()
			{
				No = 0,
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
				No = 0,
				Name = "Состояние для режима Включено 1",
				Caption = "Состояние для режима Включено 1",
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
				No = 0,
				Name = "Состояние для режима Включено 2",
				Caption = "Состояние для режима Включено 2",
				Default = 16,
				IsLowByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property3, "Не горит", 0);
			GKDriversHelper.AddPropertyParameter(property3, "Горит", 16);
			GKDriversHelper.AddPropertyParameter(property3, "Мерцание", 32);
			driver.Properties.Add(property3);

			return driver;
		}
	}
}