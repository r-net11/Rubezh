using RubezhAPI.GK;
using System;
namespace GKProcessor
{
	public class GKMirrorPerformersDevice

	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x10D,
				DriverType = GKDriverType.ControlDevicesMirror,
				UID = new Guid("E4850E38-1800-4024-B764-B8CF6D9AAD49"),
				Name = "Устройство исполнительное",
				ShortName = "УИС",
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