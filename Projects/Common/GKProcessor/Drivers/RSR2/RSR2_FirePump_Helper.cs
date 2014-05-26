using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class RSR2_FirePump_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x03,
				DriverType = XDriverType.RSR2_Bush_Fire,
				UID = new Guid("6C9192C9-2841-46b6-B653-7834EFA41041"),
				Name = "Блок управления ПН",
				ShortName = "БУШ ПН RSR2",
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

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 2, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Задержка на выключение, с", 2, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Питание, 0.1 В", 80, 0, 100);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 1, Ом", 340, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 2, Ом", 660, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 3, Ом", 2350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 4, Ом", 3350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 7, "Порог 5, Ом", 4500, 0, 65535);

			GKDriversHelper.AddIntProprety(driver, 8, "Время выхода на режим, с", 1, 1, 65535);

			var property90 = new XDriverProperty()
			{
				No = 9,
				Name = "Состояние давления на выходе",
				Caption = "Состояние давления на выходе",
				Default = 0,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property90, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property90, "Контакт НЗ", 1);
			driver.Properties.Add(property90);

			var property91 = new XDriverProperty()
			{
				No = 9,
				Name = "Состояние ДУ ПУСК",
				Caption = "Состояние ДУ ПУСК",
				Default = 0,
				IsLowByte = true,
				Mask = 0x02
			};
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НЗ", 2);
			driver.Properties.Add(property91);

			var property92 = new XDriverProperty()
			{
				No = 9,
				Name = "Состояние ДУ СТОП",
				Caption = "Состояние ДУ СТОП",
				Default = 0,
				IsLowByte = true,
				Mask = 0x04
			};
			GKDriversHelper.AddPropertyParameter(property92, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property92, "Контакт НЗ", 4);
			driver.Properties.Add(property92);

			var property97 = new XDriverProperty()
			{
				No = 9,
				Name = "Наличие ДУ",
				Caption = "Наличие ДУ",
				Default = 0,
				IsLowByte = true,
				Mask = 0x40
			};
			GKDriversHelper.AddPropertyParameter(property97, "Нет", 0);
			GKDriversHelper.AddPropertyParameter(property97, "Есть", 0x40);
			driver.Properties.Add(property97);

			driver.MeasureParameters.Add(new XMeasureParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
			driver.MeasureParameters.Add(new XMeasureParameter() { No = 2, Name = "Отсчет задержки на выключение, с", IsDelay = true });

			return driver;
		}
	}
}