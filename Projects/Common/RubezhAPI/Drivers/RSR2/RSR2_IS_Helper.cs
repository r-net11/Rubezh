using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public static class RSR2_IS_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF8,
				DriverType = GKDriverType.RSR2_IS,
				UID = new Guid("F2D21F78-A237-4CFB-B61A-2D8B8B9163F6"),
				Name = "Индикатор состояния",
				ShortName = "ИС",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.ActuatingDevice
			};

			return driver;
		}
	}
}