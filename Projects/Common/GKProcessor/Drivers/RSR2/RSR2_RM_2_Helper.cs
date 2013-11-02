using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_RM_2_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_RM_2,
				UID = new Guid("A2959070-BB9D-45C0-8113-F9662A1CE7BF"),
				Name = "МР-2 RSR2",
				ShortName = "МР-2 RSR2",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RSR2_RM_1,
				GroupDeviceChildrenCount = 2
			};
			return driver;
		}
	}
}