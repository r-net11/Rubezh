using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class GKRelaysGroup_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver
			{
				DriverType = GKDriverType.GKRelaysGroup,
				UID = new Guid("77980273-4B1D-4ACC-915E-95FFDCD1DD02"),
				Name = "Группа реле",
				ShortName = "Группа реле",
				HasAddress = false,
				IsAutoCreate = true,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				IsReal = false
			};

			return driver;
		}
	}
}