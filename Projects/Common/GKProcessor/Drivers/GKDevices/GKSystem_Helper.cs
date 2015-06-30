using FiresecAPI.GK;

namespace GKProcessor
{
	public static class GKSystem_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.System,
				UID = GKDriver.System_UID,
				Name = "Локальная сеть",
				ShortName = "Локальная сеть",
				CanEditAddress = false,
				HasAddress = false,
				IsDeviceOnShleif = false,
				IsReal = false
			};
			driver.Children.Add(GKDriverType.GK);

			return driver;
		}
	}
}