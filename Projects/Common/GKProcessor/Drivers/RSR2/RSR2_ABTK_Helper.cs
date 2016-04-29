using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public class RSR2_ABTK_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x14,
				DriverType = GKDriverType.RSR2_ABTK,
				UID = new Guid("45BA7A5E-BF4B-4F63-B80E-95FE422DC70D"),
				Name = "Адресный барьер термокабеля",
				ShortName = "АБТК",
				HasZone = true,
				IsPlaceable = true,
				DriverClassification = GKDriver.DriverClassifications.FireDetector
			};
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Fire1);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Расстояние, м", InternalName = "Distance" });
			return driver;
		}
	}
}