using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_KAU_Shleif_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_KAU_Shleif,
				UID = new Guid("4D5647DF-A278-48F6-9F89-19E4D51B7711"),
				Name = "АЛС",
				ShortName = "АЛС",
				HasAddress = true,
				IsAutoCreate = true,
				MinAddress = 1,
				MaxAddress = 8,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				IsReal = false
			};

			return driver;
		}
	}
}