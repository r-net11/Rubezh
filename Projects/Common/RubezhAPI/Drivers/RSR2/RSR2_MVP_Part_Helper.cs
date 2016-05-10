using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_MVP_Part_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_MVP_Part,
				UID = new Guid("118A822D-25F0-429a-A1E8-3EFEB40900E0"),
				Name = "Линия МВП",
				ShortName = "Линия МВП",
				IsAutoCreate = true,
				MinAddress = 1,
				MaxAddress = 2,
				HasAddress = false,
				IsReal = false,
				IsPlaceable = false,
			};

			return driver;
		}
	}
}