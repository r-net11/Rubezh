using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class KD_KDK_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF5,
				DriverType = GKDriverType.KD_KDK,
				UID = new Guid("FD24E2EA-B3CF-4F7F-B73A-D60F8749D685"),
				Name = "Концевик(кнопка) КД",
				ShortName = "КДК",
				IsPlaceable = true,
				IsAutoCreate = true,
				MinAddress = 2,
				MaxAddress = 3,
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			var property1 = new GKDriverProperty()
			{
				No = 0,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 1
			};
			var property1Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "Нормально замкнутый",
				Value = 0
			};
			var property1Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "Нормально разомкнутый",
				Value = 1
			};

			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			driver.Properties.Add(property1);

			return driver;
		}
	}
}