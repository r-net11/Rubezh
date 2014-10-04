using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_AM_4_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_AM_4,
				UID = new Guid("79EAC50A-D534-4775-A102-BE4872877400"),
				Name = "Метка адресная АМ4-R2",
				ShortName = "АМ4-R2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_AM_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}