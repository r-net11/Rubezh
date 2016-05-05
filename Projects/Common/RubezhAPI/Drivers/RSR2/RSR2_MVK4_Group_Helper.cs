using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_MVK4_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_MVK4_Group,
				UID = new Guid("D3FD9DE2-D2E8-4DAC-B44E-82B202BE3534"),
				Name = "Модуль выходов с контролем групповой",
				ShortName = "МВК4",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_MVK8,
				GroupDeviceChildrenCount = 4,
				DriverClassification = GKDriver.DriverClassifications.ActuatingDevice
			};
			return driver;
		}
	}
}