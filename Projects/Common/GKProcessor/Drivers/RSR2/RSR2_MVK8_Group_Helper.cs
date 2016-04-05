using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_MVK8_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_MVK8_Group,
				UID = new Guid("5EE43F54-5F4C-4412-8797-3D7F9A92D2E3"),
				Name = "Модуль выходов с контролем МВК8",
				ShortName = "МВК8",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_MVK8,
				GroupDeviceChildrenCount = 8
			};
			return driver;
		}
	}
}