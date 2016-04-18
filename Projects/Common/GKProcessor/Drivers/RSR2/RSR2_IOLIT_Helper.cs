using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RSR2_IOLIT_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x20,
				DriverType = GKDriverType.RSR2_IOLIT,
				UID = new Guid("10DCA010-257E-4D81-B090-72903E0CB029"),
				Name = "ИОЛИТ",
				ShortName = "ИОЛИТ",
				HasZone = true,
				IsPlaceable = true,
				TypeOfBranche = GKDriver.TypesOfBranches.FireDetector
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			var property1 = new GKDriverProperty()
			{
				No = 0,
				Name = "Чуствительность",
				Caption = "Чуствительность",
				Default = 0,
				IsLowByte = true,
				Mask = 0x0F
			};
			GKDriversHelper.AddPropertyParameter(property1, "100%", 0);
			GKDriversHelper.AddPropertyParameter(property1, "75%", 1);
			GKDriversHelper.AddPropertyParameter(property1, "50%", 2);
			GKDriversHelper.AddPropertyParameter(property1, "25%", 3);
			driver.Properties.Add(property1);

			var property11 = new GKDriverProperty()
			{
				No = 0,
				Name = "Режим",
				Caption = "Режим",
				Default = 0,
				IsLowByte = true,
				Mask = 0xF0
			};
			GKDriversHelper.AddPropertyParameter(property11, "Основной Т1 с фиксац. тревоги", 0);
			GKDriversHelper.AddPropertyParameter(property11, "Основной Т1 без фиксац. тревоги", 16);
			GKDriversHelper.AddPropertyParameter(property11, "Основной Т2 с фиксац. тревоги", 32);
			GKDriversHelper.AddPropertyParameter(property11, "Основной Т2 без фиксац. тревоги", 48);
			GKDriversHelper.AddPropertyParameter(property11, "Вспышка с интер. 0.1 с", 64);
			GKDriversHelper.AddPropertyParameter(property11, "Вспышка с интер. 0.5 с", 80);
			driver.Properties.Add(property11);

			return driver;
		}
	}
}