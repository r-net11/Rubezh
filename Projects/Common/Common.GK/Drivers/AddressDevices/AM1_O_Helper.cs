using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class AM1_O_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x34,
				DriverType = XDriverType.AM1_O,
				UID = new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2"),
				Name = "Охранная адресная метка АМ1-О",
				ShortName = "АМ1-О",
				HasZone = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			var property1 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 6
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Охранный шлейф",
				Value = 6
			};
			property1.Parameters.Add(property1Parameter1);
			driver.Properties.Add(property1);

			return driver;
		}
	}
}