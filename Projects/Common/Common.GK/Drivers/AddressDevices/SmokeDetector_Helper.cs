using System.Linq;
using XFiresecAPI;
using System;
using System.Collections.Generic;

namespace Common.GK
{
	public class SmokeDetectorHelper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x61,
				DriverType = XDriverType.SmokeDetector,
				UID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"),
				Name = "Пожарный дымовой извещатель ИП 212-64",
				ShortName = "ИП-64",
				HasZone = true,
			};
			GKDriversHelper.AddAvailableStates(driver, XStateType.Test);
			GKDriversHelper.AddAvailableStates(driver, XStateType.Fire1);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму", 0, 65, 0, 255);
			return driver;
		}
	}
}