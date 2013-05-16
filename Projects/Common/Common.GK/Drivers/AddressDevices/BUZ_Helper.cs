using System;
using XFiresecAPI;

namespace Common.GK
{
	public class BUZ_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x71,
				DriverType = XDriverType.Valve,
				UID = new Guid("4935848f-0084-4151-a0c8-3a900e3cb5c5"),
				Name = "Шкаф управления задвижкой",
				ShortName = "ШУЗ",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);

			driver.AvailableCommands.Add(XStateType.TurnOn_InManual);
			driver.AvailableCommands.Add(XStateType.TurnOnNow_InManual);
			driver.AvailableCommands.Add(XStateType.TurnOff_InManual);
			driver.AvailableCommands.Add(XStateType.Stop_InManual);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Уставка времени хода задвижки, с", 0, 1, 1, 65535);
			GKDriversHelper.AddIntProprety(driver, 0x8e, "Время отложенного запуска, с", 0, 0, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0x8f, "Время удержания запуска, мин", 0, 0, 0, 360);

			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Концевой выключатель «Открыто»", 0, "НР", "НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Муфтовый выключатель Открыто/ДУ Открыть", 1, "НР", "НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Концевой выключатель «Закрыто»", 2, "НР", "НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Муфтовый выключатель Закрыто/ДУ Закрыть", 3, "НР", "НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Кнопка Открыть УЗЗ", 4, "НР", "НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Кнопка Закрыть УЗЗ", 5, "НР", "НЗ");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Кнопка Стоп УЗЗ", 6, "НР", "НЗ");

			//GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Муфтовые выключатели", 9, "есть", "нет");
			//GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Датчик уровня", 10, "нет", "есть");
			GKDriversHelper.AddPlainEnumProprety(driver, 0x8d, "Функция УЗЗ", 11, "отключена", "включена");

			var additionalSwitcherProperty = new XDriverProperty()
			{
				No = 0x8d,
				Name = "Дополнительные выключатели",
				Caption = "Дополнительные выключатели",
				Default = 0,
				Offset = 9,
				Mask = 3
			};
			var parameter1 = new XDriverPropertyParameter()
			{
				Name = "Нет",
				Value = 0
			};
			var parameter2 = new XDriverPropertyParameter()
			{
				Name = "муфтовые выключатели",
				Value = 1
			};
			var parameter3 = new XDriverPropertyParameter()
			{
				Name = "датчик уровня",
				Value = 2
			};
			additionalSwitcherProperty.Parameters.Add(parameter1);
			additionalSwitcherProperty.Parameters.Add(parameter2);
			additionalSwitcherProperty.Parameters.Add(parameter3);
			driver.Properties.Add(additionalSwitcherProperty);

			return driver;
		}
	}
}