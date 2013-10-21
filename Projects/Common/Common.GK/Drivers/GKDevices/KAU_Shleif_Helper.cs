using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class KAU_Shleif_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.KAU_Shleif,
				UID = new Guid("2DF54B82-88E5-4168-9D9D-DDE1CFB71294"),
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