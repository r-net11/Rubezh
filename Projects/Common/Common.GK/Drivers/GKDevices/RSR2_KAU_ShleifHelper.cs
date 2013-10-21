using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_KAU_Shleif_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_KAU_Shleif,
				UID = new Guid("4D5647DF-A278-48F6-9F89-19E4D51B7711"),
				Name = "Шлейф",
				ShortName = "Шлейф",
				CanEditAddress = false,
				HasAddress = true,
				IsAutoCreate = true,
				MinAddress = 1,
				MaxAddress = 8,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
			};

			return driver;
		}
	}
}