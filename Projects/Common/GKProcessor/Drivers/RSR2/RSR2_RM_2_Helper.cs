using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_RM_2_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_RM_2,
				UID = new Guid("A2959070-BB9D-45C0-8113-F9662A1CE7BF"),
				Name = "Модуль релейный РМ2-R2",
				ShortName = "РМ2-R2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_RM_1,
				GroupDeviceChildrenCount = 2
			};
			return driver;
		}
	}
}