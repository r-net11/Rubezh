using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class RSR2_Valve_DU_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x0A,
				DriverType = GKDriverType.RSR2_Valve_DU,
				UID = new Guid("79D5B336-8892-4d56-929C-1D95F0FF0992"),
				Name = "Блок управления задвижкой КВ-ДУ (БАЭС)",
				ShortName = "БУЗ КВ-ДУ (БАЭС)",
				IsControlDevice = true,
				HasLogic = true,
				IgnoreHasLogic = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			driver.AvailableStateBits.Add(GKStateBit.TurningOn);
			driver.AvailableStateBits.Add(GKStateBit.TurningOff);
			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.Stop_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на открытие, с", 0, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Удержание открытия, мин", 0, 0, 720);
			GKDriversHelper.AddIntProprety(driver, 2, "Питание, В", 80, 0, 100).Multiplier = 10;
			GKDriversHelper.AddIntProprety(driver, 3, "Порог 1, Ом", 340, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 4, "Порог 2, Ом", 1000, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 5, "Порог 3, Ом", 2350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 6, "Порог 4, Ом", 3350, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 7, "Порог 5, Ом", 4500, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 8, "Время хода, с", 180, 1, 65535);

			var property90 = new GKDriverProperty()
			{
				No = 9,
				Name = "КВОткр",
				Caption = "КВОткр",
				Default = 0,
				IsLowByte = true,
				Mask = 1
			};
			GKDriversHelper.AddPropertyParameter(property90, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property90, "Контакт НЗ", 1);
			driver.Properties.Add(property90);

			var property91 = new GKDriverProperty()
			{
				No = 9,
				Name = "КВЗакр",
				Caption = "КВЗакр",
				Default = 0,
				IsLowByte = true,
				Mask = 2
			};
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property91, "Контакт НЗ", 2);
			driver.Properties.Add(property91);

			var property92 = new GKDriverProperty()
			{
				No = 9,
				Name = "ДНУ",
				Caption = "ДНУ",
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
				Name = "ДВУ",
				Caption = "ДВУ",
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
				Name = "ДУОткр",
				Caption = "ДУОткр",
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
				Name = "ДУЗакр",
				Caption = "ДУЗакр",
				Default = 0,
				IsHieghByte = true,
				Mask = 32
			};
			GKDriversHelper.AddPropertyParameter(property95, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property95, "Контакт НЗ", 32);
			driver.Properties.Add(property95);

			var property96 = new GKDriverProperty()
			{
				No = 9,
				Name = "ДУСтоп",
				Caption = "ДУСтоп",
				Default = 0,
				IsLowByte = true,
				Mask = 64
			};
			GKDriversHelper.AddPropertyParameter(property96, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property96, "Контакт НЗ", 64);
			driver.Properties.Add(property96);

			var property98 = new GKDriverProperty()
			{
				No = 9,
				Name = "ДУ",
				Caption = "ДУ",
				Default = 0,
				IsHieghByte = true,
				Mask = 1
			};
			GKDriversHelper.AddPropertyParameter(property98, "Нет", 0);
			GKDriversHelper.AddPropertyParameter(property98, "Есть", 1);
			driver.Properties.Add(property98);

			return driver;
		}
	}
}