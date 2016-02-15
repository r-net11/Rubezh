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
				IsPlaceable = true
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
                Mask = 0x03
            };
            GKDriversHelper.AddPropertyParameter(property1, "Полная", 0);
            driver.Properties.Add(property1);

			return driver;
		}
	}
}