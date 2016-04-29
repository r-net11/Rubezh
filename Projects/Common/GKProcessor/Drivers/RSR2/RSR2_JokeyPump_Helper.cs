using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class RSR2_JokeyPump_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x02,
				DriverType = GKDriverType.RSR2_Bush_Jokey,
				UID = new Guid("0F6B6AEE-4D7A-4e9d-9C16-0072CDC40932"),
				Name = "Прибор пожарный управления Жокей Насосом",
				ShortName = "ППУ ЖН",
				IsControlDevice = true,
				HasLogic = true,
				IgnoreHasLogic = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.ControlCabinet
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Off);
			driver.AvailableStateBits.Add(GKStateBit.TurningOn);
			driver.AvailableStateBits.Add(GKStateBit.TurningOff);
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Задержка на выключение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Питание, В", 80, 0, 100).Multiplier = 10;
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 1, Ом", 340, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 2, Ом", 1000, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 3, Ом", 2350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 4, Ом", 3350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 7, "Порог 5, Ом", 4500, 0, 65535);

			GKDriversHelper.AddIntProprety(driver, 8, "Время выхода на режим, мин", 1, 1, 720);

			var property90 = new GKDriverProperty()
			{
				No = 9,
				Name = "Состояние контакта датчика низкого давления",
				Caption = "Состояние контакта датчика низкого давления",
				Default = 0,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property90, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property90, "Контакт НЗ", 1);
			driver.Properties.Add(property90);

			var property91 = new GKDriverProperty()
			{
				No = 9,
				Name = "Состояние контакта датчика высокого давления",
				Caption = "Состояние контакта датчика высокого давления",
				Default = 0,
				IsLowByte = true,
				Mask = 0x02
			};
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НЗ", 2);
			driver.Properties.Add(property91);

			driver.MeasureParameters.Add(new GKMeasureParameter { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true, IsNotVisible = true });
			driver.MeasureParameters.Add(new GKMeasureParameter { No = 2, Name = "Отсчет задержки на выключение, с", IsDelay = true, IsNotVisible = true });

			return driver;
		}
	}
}