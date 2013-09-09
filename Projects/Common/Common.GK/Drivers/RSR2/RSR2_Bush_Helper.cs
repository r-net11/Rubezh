using System;
using XFiresecAPI;

namespace Common.GK
{
	public class RSR2_Pump_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xE0,
				DriverType = XDriverType.RSR2_Bush,
				UID = new Guid("1743FA7E-EF69-45B7-90CD-D9BF2B44644C"),
				Name = "Блок управления шкафом RSR2",
				ShortName = "БУШ RSR2",
				IsControlDevice = true,
				HasLogic = true,
				IgnoreHasLogic = true,
				IsPlaceable = true
			};
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 2, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Задержка на выключение, с", 0, 2, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Питание, 0.1 В", 0, 80, 0, 100);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 1", 0, 340, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 2", 0, 660, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 3", 0, 2350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 4", 0, 3350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 7, "Порог 5", 0, 4500, 0, 65535);

			var property8 = new XDriverProperty()
			{
				No = 8,
				Name = "Тип шкафа",
				Caption = "Тип шкафа",
				Default = 1
			};
			var property8Parameter1 = new XDriverPropertyParameter()
			{
				Name = "ДН",
				Value = 1
			};
			property8.Parameters.Add(property8Parameter1);
			driver.Properties.Add(property8);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Отсчет задержки на выключение, с", IsDelay = true });

			return driver;
		}
	}
}