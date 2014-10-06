using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RM_3_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RM_3,
				UID = new Guid("15e38fa6-dc41-454b-83e5-d7789064b2e1"),
				Name = "РМ-3",
				ShortName = "РМ-3",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RM_1,
				GroupDeviceChildrenCount = 3,
				IsIgnored = true,
			};
			return driver;
		}
	}
}