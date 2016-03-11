using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_KDKR_Part_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_KDKR_Part,
				UID = new Guid("CDEAC410-1B73-4C8B-BBE6-4D829E168A77"),
				Name = "Линия КД",
				ShortName = "Линия КД",
				IsAutoCreate = true,
				MinAddress = 14,
				MaxAddress = 15,
				HasAddress = false,
				IsReal = false,
				IsPlaceable = false,
			};

			return driver;
		}
	}
}