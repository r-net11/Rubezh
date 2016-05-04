using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_ABShS_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_ABShS_Group,
				UID = new Guid("71B8E463-4927-4ECD-823E-D59179C9ED29"),
				Name = "Адресный барьер шлейфа сигнализации на 2 входа",
				ShortName = "АБШС2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_ABShS,
				GroupDeviceChildrenCount = 2,
				DriverClassification = GKDriver.DriverClassifications.Other
			};
			return driver;
		}
	}
}