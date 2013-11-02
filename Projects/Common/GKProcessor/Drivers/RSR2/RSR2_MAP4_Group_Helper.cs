using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_MAP4_Group_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_MAP4_Group,
				UID = new Guid("FE44E469-55FB-4079-A50D-A0E4C098F0AC"),
				Name = "МАП-4 RSR2",
				ShortName = "МАП-4 RSR2",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RSR2_MAP4,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}