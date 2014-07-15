using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_CodeReader_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xF0,
				DriverType = XDriverType.RSR2_CodeReader,
				UID = new Guid("FC8AC44B-6B54-470E-92DC-7ED63E5EA62F"),
				Name = "Наборник кода RSR2",
				ShortName = "НК RSR2",
				HasZone = false,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			return driver;
		}
	}
}