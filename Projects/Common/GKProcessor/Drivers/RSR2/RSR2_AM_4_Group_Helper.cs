using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_AM_4_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_AM_4,
				UID = new Guid("79EAC50A-D534-4775-A102-BE4872877400"),
				Name = "Метка адресная групповая",
				ShortName = "АМ4",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_AM_1,
				GroupDeviceChildrenCount = 4,
				TypeOfBranche = GKDriver.TypesOfBranches.Other
			};
			return driver;
		}
	}
}