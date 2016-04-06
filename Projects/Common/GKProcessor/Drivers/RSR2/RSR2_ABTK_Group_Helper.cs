using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_ABTK_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_ABTK_Group,
				UID = new Guid("D59115A5-B3A6-4D76-A744-0B719F81B15D"),
				Name = "Адресный барьер термокабеля на 2 входа",
				ShortName = "АБТК2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_ABTK,
				GroupDeviceChildrenCount = 2
			};
			return driver;
		}
	}
}
