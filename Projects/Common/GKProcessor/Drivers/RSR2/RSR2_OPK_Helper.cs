using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_OPK_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE4,
				DriverType = XDriverType.RSR2_OPK,
				UID = new Guid("F5BCB799-26F9-4225-9866-ACDE37C78C03"),
				Name = "Оповещатель комбинированный",
				ShortName = "ОПК-RSR2",
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


			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 5, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 16, 1, 65535);

			var property1 = new XDriverProperty()
			{
				No = 2,
				Name = "Режим после удержания",
				Caption = "Режим после удержания",
				Default = 0,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property1, "Выключается", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Остаётся включённым", 1);
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 2,
				Name = "Состояние светодиода в режиме удержание и включено",
				Caption = "Состояние светодиода в режиме удержание и включено",
				Default = 0,
				IsLowByte = true,
				Mask = 0x06
			};
			GKDriversHelper.AddPropertyParameter(property2, "Гашение", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Горение", 2);
			GKDriversHelper.AddPropertyParameter(property2, "Мерцание", 4);
			driver.Properties.Add(property2);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 2, Name = "Отсчет удержания, с", IsDelay = true });

			return driver;
		}
	}

	
}