using RubezhAPI.GK;
using System;

namespace RubezhAPI
{
	public static class GKSystem_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.System,
				UID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C"),
				Name = "Локальная сеть",
				ShortName = "Локальная сеть",
				HasAddress = false,
				IsDeviceOnShleif = false,
				IsReal = false
			};
			driver.Children.Add(GKDriverType.GK);

			return driver;
		}
	}
}