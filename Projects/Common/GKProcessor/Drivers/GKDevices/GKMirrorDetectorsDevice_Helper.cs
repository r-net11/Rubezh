using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class GKMirrorDetectorsDevice_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x10E,
				DriverType = GKDriverType.DetectorDevicesMirror,
				UID = new Guid("134F0703-05CE-4025-BA12-1710362B1FFB"),
				Name = "Устройство извещательное или извещатель",
				ShortName = "УИЗВ",
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
			driver.AvailableStateClasses.Add(XStateClass.Ignore);

			return driver;
		}
	}
}