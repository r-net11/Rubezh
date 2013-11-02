using System;
using XFiresecAPI;

namespace GKProcessor
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
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(XStateBit.Stop_InManual);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Уставка времени хода задвижки, с", 0, 1, 180, 65535);
			GKDriversHelper.AddIntProprety(driver, 0x8e, "Время отложенного запуска, с", 0, 0, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0x8f, "Время удержания запуска, мин", 0, 0, 0, 360);

			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Концевой выключатель «Открыто»", 0, "НР", "НЗ", 1);
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Муфтовый выключатель Открыто/ДУ Открыть", 1, "НР", "НЗ", 2);
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Концевой выключатель «Закрыто»", 2, "НР", "НЗ", 4);
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Муфтовый выключатель Закрыто/ДУ Закрыть", 3, "НР", "НЗ", 8);
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Кнопка Открыть УЗЗ", 4, "НР", "НЗ", 16);
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Кнопка Закрыть УЗЗ", 5, "НР", "НЗ", 32);
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Кнопка Стоп УЗЗ", 6, "НР", "НЗ", 64);

			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Муфтовые выключатели", 9, "нет", "есть", 2).IsHieghByte = true;
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Датчик уровня", 10, "нет", "есть", 4).IsHieghByte = true;
			GKDriversHelper.AddPlainEnumProprety2(driver, 0x8d, "Функция УЗЗ", 11, "отключена", "включена", 8).IsHieghByte=true;

			//var additionalSwitcherProperty = new XDriverProperty()
			//{
			//    No = 0x8d,
			//    Name = "Дополнительные выключатели",
			//    Caption = "Дополнительные выключатели",
			//    Default = 0,
			//    Offset = 0,
			//    Mask = 3,
			//    IsHieghByte=true
			//};
			//var parameter1 = new XDriverPropertyParameter()
			//{
			//    Name = "Нет",
			//    Value = 0
			//};
			//var parameter2 = new XDriverPropertyParameter()
			//{
			//    Name = "муфтовые выключатели",
			//    Value = 1
			//};
			//var parameter3 = new XDriverPropertyParameter()
			//{
			//    Name = "датчик уровня",
			//    Value = 2
			//};
			//additionalSwitcherProperty.Parameters.Add(parameter1);
			//additionalSwitcherProperty.Parameters.Add(parameter2);
			//additionalSwitcherProperty.Parameters.Add(parameter3);
			//driver.Properties.Add(additionalSwitcherProperty);

			driver.AUParameters.Add(new XAUParameter() { No = 0x80, Name = "Режим работы" });

			return driver;
		}
	}
}