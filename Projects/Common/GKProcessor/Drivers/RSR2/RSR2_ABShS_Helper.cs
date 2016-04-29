using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public static class RSR2_ABShS_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x13,
				DriverType = GKDriverType.RSR2_ABShS,
				UID = new Guid("7C464C3A-DDC5-496A-9747-4CA93B774DCA"),
				Name = "Адресный барьер шлейфа сигнализации",
				ShortName = "АБШС",
				HasZone = true,
				IsPlaceable = true,
				DriverClassification =GKDriver.DriverClassifications.Other
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Failure);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			var property1 = new GKDriverProperty()
			{
				No = 0,
				Name = "Нагрузка",
				Caption = "Нагрузка",
				Default = 0
			};
			var property1Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "Пассивная",
				Value = 0
			};
			var property1Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "Активная",
				Value = 1
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			driver.Properties.Add(property1);
			return driver;
		}
	}
}