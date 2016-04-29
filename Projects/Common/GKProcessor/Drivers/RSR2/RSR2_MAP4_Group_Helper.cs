using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_MAP4_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_MAP4_Group,
				UID = new Guid("FE44E469-55FB-4079-A50D-A0E4C098F0AC"),
				Name = "Метка адресная пожарная групповая",
				ShortName = "АМП4",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_MAP4,
				GroupDeviceChildrenCount = 4,
				DriverClassification = GKDriver.DriverClassifications.Other
			};
			return driver;
		}
	}
}