using RubezhAPI.GK;
using System;

namespace RubezhAPI
{
	public static class RK_OPZ_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x26,
				DriverType = GKDriverType.RK_OPZ,
				UID = new Guid("A816B11B-67B9-450D-814E-C5DA393F41C6"),
				Name = "Оповещатель охранно-пожарный звуковой радиоканальный",
				ShortName = "ОПЗ-РК",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true
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

			GKDriversHelper.AddRadioChanelProperties(driver);
			GKDriversHelper.AddIntProprety(driver, 3, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 4, "Время удержания, с", 0, 0, 65535);

			var property1 = new GKDriverProperty()
			{
				No = 5,
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