using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RM_3_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RM_3,
				UID = new Guid("15e38fa6-dc41-454b-83e5-d7789064b2e1"),
				Name = "РМ-3",
				ShortName = "РМ-3",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RM_1,
				GroupDeviceChildrenCount = 3
			};
			return driver;
		}
	}
}