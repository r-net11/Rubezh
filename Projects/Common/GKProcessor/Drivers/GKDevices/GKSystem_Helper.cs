using FiresecAPI.GK;

namespace GKProcessor
{
	public static class GKSystem_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.System,
				UID = XDriver.System_UID,
				Name = "Локальная сеть",
				ShortName = "Локальная сеть",
				CanEditAddress = false,
				HasAddress = false,
				IsDeviceOnShleif = false,
				IsReal = false
			};
			driver.Children.Add(XDriverType.GK);

			return driver;
		}
	}
}