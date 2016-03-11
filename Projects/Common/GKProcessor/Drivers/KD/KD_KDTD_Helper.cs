using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class KD_KDTD_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF7,
				DriverType = GKDriverType.KD_KDTD,
				UID = new Guid("EB21B6CE-6AE8-40C5-92C7-5111D8418163"),
				Name = "ТД КД",
				ShortName = "КДТД",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				IsAutoCreate = true,
				MinAddress = 6,
				MaxAddress = 13,
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на тревогу, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 20, 0, 65535);

			var property1 = new GKDriverProperty()
			{
				No = 2,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 0,
				IsLowByte = true,
				Mask = 0x03
			};

			GKDriversHelper.AddPropertyParameter(property1, "вход - КВ, выход - кнопка", 0);
			GKDriversHelper.AddPropertyParameter(property1, "вход - КВ, выход - кнопка, концевик", 1);
			GKDriversHelper.AddPropertyParameter(property1, "вход - КВ, выход - КВ", 2);
			GKDriversHelper.AddPropertyParameter(property1, "вход - КВ, выход - КВ, концевик", 3);
			driver.Properties.Add(property1);

			var property2 = new GKDriverProperty()
			{
				No = 2,
				Name = "Режим после удержания",
				Caption = "Режим после удержания",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property2, "Выключено", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Включено", 1);
			driver.Properties.Add(property2);

			GKDriversHelper.AddIntProprety(driver, 3, "Замок (номер устройства)", 0, 0, 48);
			GKDriversHelper.AddIntProprety(driver, 4, "Вход (номер устройства)", 0, 0, 48);
			GKDriversHelper.AddIntProprety(driver, 5, "Выход (номер устройства)", 0, 0, 48);
			GKDriversHelper.AddIntProprety(driver, 6, "Концевик (номер устройства)", 0, 0, 48);

			return driver;
		}
	}
}
