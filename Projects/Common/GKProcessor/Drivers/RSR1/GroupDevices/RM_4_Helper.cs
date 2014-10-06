using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RM_4_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RM_4,
				UID = new Guid("3cb0e7fb-670f-4f32-8123-4b310aee1db8"),
				Name = "РМ-4",
				ShortName = "РМ-4",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RM_1,
				GroupDeviceChildrenCount = 4,
				IsIgnored = true,
			};
			return driver;
		}
	}
}