using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RM_2_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RM_2,
				UID = new Guid("ea5f5372-c76c-4e92-b879-0afa0ee979c7"),
				Name = "РМ-2",
				ShortName = "РМ-2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RM_1,
				GroupDeviceChildrenCount = 2,
				IsIgnored = true,
			};
			return driver;
		}
	}
}