using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class HandDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x55,
				DriverType = XDriverType.HandDetector,
				UID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a"),
				Name = "Ручной извещатель ИПР513-11",
				ShortName = "ИПР",
				HasZone = true,
                IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire2);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			return driver;
		}
	}
}