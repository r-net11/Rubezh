using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_RM_4_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_RM_4,
				UID = new Guid("E3001BBB-18DA-437D-AB73-56C701F070D4"),
				Name = "Модуль релейный групповой",
				ShortName = "РМ4",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_RM_1,
				GroupDeviceChildrenCount = 4,
				DriverClassification = GKDriver.DriverClassifications.ActuatingDevice
			};
			return driver;
		}
	}
}