using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_Buz_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 11,
				DriverType = GKDriverType.RSR2_Buz,
				UID = new Guid("08E8C44C-16B7-469f-AEEA-C48D32240682"),
				Name = "Блок управления задвижкой R2",
				ShortName = "БУЗ R2",
				IsControlDevice = true,
				HasZone = false,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Удержание открытия, мин", 0, 0, 720);
			GKDriversHelper.AddIntProprety(driver, 2, "Питание, 0.1 В", 80, 0, 100);
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 1, Ом", 340, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 2, Ом", 660, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 3, Ом", 2350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 4, Ом", 3350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 7, "Порог 5, Ом", 4500, 0, 65535);

			GKDriversHelper.AddIntProprety(driver, 8, "Время хода, с", 60, 1, 65535);

			var property90 = new GKDriverProperty()
			{
				No = 9,
				Name = "КВ Откр",
				Caption = "КВ Откр",
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
				Name = "КВ Закр",
				Caption = "КВ Закр",
				Default = 0,
				IsLowByte = true,
				Mask = 0x02
			};
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НЗ", 2);
			driver.Properties.Add(property91);

			var property92 = new GKDriverProperty()
			{
				No = 9,
				Name = "МВ Откр",
				Caption = "МВ Откр",
				Default = 0,
				IsLowByte = true,
				Mask = 0x04
			};
			GKDriversHelper.AddPropertyParameter(property92, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property92, "Контакт НЗ", 4);
			driver.Properties.Add(property92);

			var property93 = new GKDriverProperty()
			{
				No = 9,
				Name = "МВ Закр",
				Caption = "МВ Закр",
				Default = 0,
				IsLowByte = true,
				Mask = 0x08
			};
			GKDriversHelper.AddPropertyParameter(property93, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property93, "Контакт НЗ", 8);
			driver.Properties.Add(property93);

			var property94 = new GKDriverProperty()
			{
				No = 9,
				Name = "ДУ Откр",
				Caption = "ДУ Откр",
				Default = 0,
				IsLowByte = true,
				Mask = 16
			};
			GKDriversHelper.AddPropertyParameter(property94, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property94, "Контакт НЗ", 16);
			driver.Properties.Add(property94);

			var property95 = new GKDriverProperty()
			{
				No = 9,
				Name = "ДУ Закр",
				Caption = "ДУ Закр",
				Default = 0,
				IsLowByte = true,
				Mask = 32
			};
			GKDriversHelper.AddPropertyParameter(property95, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property95, "Контакт НЗ", 32);
			driver.Properties.Add(property95);

			var property96 = new GKDriverProperty()
			{
				No = 9,
				Name = "ДУ Стоп",
				Caption = "ДУ Стоп",
				Default = 0,
				IsLowByte = true,
				Mask = 64
			};
			GKDriversHelper.AddPropertyParameter(property96, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property96, "Контакт НЗ", 64);
			driver.Properties.Add(property96);

			var property97 = new GKDriverProperty()
			{
				No = 9,
				Name = "ОГВ",
				Caption = "ОГВ",
				Default = 0,
				IsLowByte = true,
				Mask = 128
			};
			GKDriversHelper.AddPropertyParameter(property97, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property97, "Контакт НЗ", 128);
			driver.Properties.Add(property97);

			var property98 = new GKDriverProperty()
			{
				No = 9,
				Name = "Наличие ДУ",
				Caption = "Наличие ДУ",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property98, "Нет", 0);
			GKDriversHelper.AddPropertyParameter(property98, "Есть", 0x01);
			driver.Properties.Add(property98);

			var property99 = new GKDriverProperty()
			{
				No = 9,
				Name = "Режим после удержания",
				Caption = "Режим после удержания",
				Default = 0,
				IsHieghByte = true,
				Mask = 0x02
			};
			GKDriversHelper.AddPropertyParameter(property99, "Закрыто", 0);
			GKDriversHelper.AddPropertyParameter(property99, "Открыто", 0x02);
			driver.Properties.Add(property98);

			return driver;
		}
	}
}