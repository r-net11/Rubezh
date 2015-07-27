using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public  class GKMirrorFireZone
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x998,
				DriverType = GKDriverType.RSR2_GKMirrorFireZone,
				UID = new Guid("C1C19C8D-6462-4D5C-A9EC-56E2222AC245"),
				Name = "Пожарные зоны",
				ShortName = "Пожарные зоны",
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
				MinAddress = 1,
				MaxAddress = 2000
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}

	}
}
