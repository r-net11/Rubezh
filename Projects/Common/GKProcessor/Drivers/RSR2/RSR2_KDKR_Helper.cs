using System;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_KDKR_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xF3,
				DriverType = GKDriverType.RSR2_KDKR,
				UID = new Guid("CEF793CA-7E3E-446C-82DE-A032445B6CBA"),
				Name = "Контроллер доступа",
				ShortName = "КД-R2",
				IsPlaceable = true,
			};

			driver.AutoCreateChildren.Add(GKDriverType.KD_KDZ);
			driver.AutoCreateChildren.Add(GKDriverType.KD_KDK);
			driver.AutoCreateChildren.Add(GKDriverType.KD_KDKV);
			driver.AutoCreateChildren.Add(GKDriverType.KD_KDTD);
			driver.AutoCreateChildren.Add(GKDriverType.RSR2_KDKR_Part);

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			GKDriversHelper.AddIntProprety(driver, 0, "Число АУ на АЛС1 КД", 6, 1, 6).CanNotEdit = true;
			GKDriversHelper.AddIntProprety(driver, 1, "Число АУ на АЛС3 КД", 0, 0, 32).CanNotEdit = true;
			GKDriversHelper.AddIntProprety(driver, 2, "Число АУ на АЛС4 КД", 0, 0, 32).CanNotEdit = true;
			GKDriversHelper.AddIntProprety(driver, 3, "Число ТД", 8, 1, 8).CanNotEdit = true;

			return driver;
		}
	}
}