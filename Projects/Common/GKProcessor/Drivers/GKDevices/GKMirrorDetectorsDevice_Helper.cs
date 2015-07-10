using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class GKMirrorDetectorsDevice_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x998,
				DriverType = GKDriverType.RSR2_GKMirrorDetectorsDevice,
				UID = new Guid("134F0703-05CE-4025-BA12-1710362B1FFB"),
				Name = "Извещательные устройства",
				ShortName = "Извещательные устройства",
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