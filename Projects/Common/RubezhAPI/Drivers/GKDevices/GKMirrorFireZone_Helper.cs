using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public  class GKMirrorFireZone
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x10B,
				DriverType = GKDriverType.FireZonesMirror,
				UID = new Guid("C1C19C8D-6462-4D5C-A9EC-56E2222AC245"),
				Name = "Зона пожарная",
				ShortName = "ЗП",
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
