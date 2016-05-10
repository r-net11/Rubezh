using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public class GKMirrorDirection_Helper
	{

		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x10A,
				DriverType = GKDriverType.DirectionsMirror,
				UID = new Guid("19AD5199-53DF-40F5-9E35-99726476FD49"),
				Name = "Направление противопожарной защиты",
				ShortName = "НПЗ",
				HasAddress = true,
				IsDeviceOnShleif = false,
				IsPlaceable = false,
				HasMirror = true,
				MinAddress = 1,
				MaxAddress = 2000,
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			return driver;
		}

	}
}
