using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class AMP_4_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.AMP_4,
				UID = new Guid("a15d9258-d5b5-4a81-a60a-3c9a308fb528"),
				Name = "Пожарная адресная метка АМП-4",
				ShortName = "АМП-4",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.AMP_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}