using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	class RSR2_OPKZ_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xEC,
				DriverType = GKDriverType.RSR2_OPKZ,
				UID = new Guid("8a277079-341b-4933-beb7-4e34b58199d7"),
				Name = "Оповещатель звуковой адресный комбинированный",
				ShortName = "ОПКЗ",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
			};

			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Off);
			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Failure);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);

			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Время удержания, с", 0, 0, 65535);

			var property1 = new GKDriverProperty()
			{
				No = 1,
				Name = "Режим после удержания",
				Caption = "Режим после удержания",
				Default = 1,
				IsLowByte = true,
				Mask = 0x01
			};
			GKDriversHelper.AddPropertyParameter(property1, "Выключается", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Остаётся включённым", 1);
			driver.Properties.Add(property1);

			return driver;
		}

	}
}
