using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public class GKMirrorGuardZone_Helper
	{

		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x10C,
				DriverType = GKDriverType.GuardZonesMirror,
				UID = new Guid("ADB852C8-46CB-405A-A0F8-1212677EFE35"),
				Name = "Зона охранная",
				ShortName = "ЗО",
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
				MinAddress = 1,
				MaxAddress = 2000,
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}

	}
}
