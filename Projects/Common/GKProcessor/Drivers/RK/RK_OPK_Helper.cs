using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public static class RK_OPK_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x24,
				DriverType = GKDriverType.RK_OPK,
				UID = new Guid("EA2F40F5-71F5-4BC0-AD60-538A5B311A8C"),
				Name = "Оповещатель охранно-пожарный комбинированный свето-звуковой радиоканальный",
				ShortName = "ОПК-РК",
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

			var property2 = new GKDriverProperty()
			{
				No = 5,
				Name = "Состояние светодиода в режиме удержание и включено",
				Caption = "Состояние светодиода в режиме удержание и включено",
				Default = 2,
				IsLowByte = true,
				Mask = 0x06
			};
			GKDriversHelper.AddPropertyParameter(property2, "Не горит", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Горит", 2);
			GKDriversHelper.AddPropertyParameter(property2, "Мерцание", 4);
			driver.Properties.Add(property2);

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true, IsNotVisible = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Отсчет удержания, с", IsDelay = true, IsNotVisible = true });

			return driver;
		}
	}
}
