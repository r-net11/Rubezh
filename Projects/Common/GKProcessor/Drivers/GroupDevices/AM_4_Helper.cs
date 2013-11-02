using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class AM_4_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.AM_4,
				UID = new Guid("e495c37a-a414-4b47-af24-fec1f9e43d86"),
				Name = "Пожарная адресная метка АМ-4",
				ShortName = "АМ-4",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.AM_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}