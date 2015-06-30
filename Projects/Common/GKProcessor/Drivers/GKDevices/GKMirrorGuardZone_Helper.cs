using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class GKMirrorGuardZone_Helper
	{

		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x998,
				DriverType = GKDriverType.RSR2_GKMirrorGuardZone,
				UID = new Guid("ADB852C8-46CB-405A-A0F8-1212677EFE35"),
				Name = "Охранные зоны",
				ShortName = "Охранные зоны",
				CanEditAddress = true,
				HasAddress = false,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}

	}
}
