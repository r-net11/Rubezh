using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_MVK2_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_MVK2_Group,
				UID = new Guid("2ABAF6C0-6B9B-4658-8AB6-7FF15914B06F"),
				Name = "Модуль выходов с контролем групповой",
				ShortName = "МВК2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_MVK8,
				GroupDeviceChildrenCount = 2,
				TypeOfBranche = GKDriver.TypesOfBranches.ActuatingDevice
			};
			return driver;
		}
	}
}