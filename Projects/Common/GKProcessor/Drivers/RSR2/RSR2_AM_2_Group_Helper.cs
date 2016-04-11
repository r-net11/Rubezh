using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_AM_2_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver
			{
				DriverType = GKDriverType.RSR2_AM_2,
				UID = new Guid("F8496E34-8F48-436F-954C-79B6BA918BBB"),
				Name = "Метка адресная групповая",
				ShortName = "АМ2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_AM_1,
				GroupDeviceChildrenCount = 2
			};
			return driver;
		}
	}
}