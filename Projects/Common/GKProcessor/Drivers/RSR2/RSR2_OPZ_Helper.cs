using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public static class RSR2_OPZ_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xE6,
				DriverType = GKDriverType.RSR2_OPZ,
				UID = new Guid("24A6FC19-0428-43A9-8B1C-35B748BD202B"),
				Name = "Оповещатель охранно-пожарный звуковой адресный",
				ShortName = "ОПЗ",
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

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 0, 0, 65535);

			var property1 = new GKDriverProperty()
			{
				No = 2,
				Name = "Режим после удержания",
				Caption = "Режим после удержания",
				Default = 1,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property1, "Выключается", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Остаётся включённым", 1);
			driver.Properties.Add(property1);

			return driver;
		}
	}
}