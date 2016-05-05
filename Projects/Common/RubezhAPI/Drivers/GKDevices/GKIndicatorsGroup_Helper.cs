using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class GKIndicatorsGroup_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver
			{
				DriverType = GKDriverType.GKIndicatorsGroup,
				UID = new Guid("848642DD-BE2F-443B-9739-FF6C7C06DC4C"),
				Name = "Группа индикаторов",
				ShortName = "Группа индикаторов",
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