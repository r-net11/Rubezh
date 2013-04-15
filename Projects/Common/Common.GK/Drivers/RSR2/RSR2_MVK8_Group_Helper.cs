using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_MVK8_Group_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_MVK8_Group,
				UID = new Guid("5EE43F54-5F4C-4412-8797-3D7F9A92D2E3"),
				Name = "МВК-8 RSR2",
				ShortName = "МВК-8 RSR2",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RSR2_MVK8,
				GroupDeviceChildrenCount = 8
			};
			return driver;
		}
	}
}