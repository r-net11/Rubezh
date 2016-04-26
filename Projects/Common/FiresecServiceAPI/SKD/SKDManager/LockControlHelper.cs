using FiresecAPI.GK;
using System;

namespace FiresecAPI.SKD
{
	public static class LockControlHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("4DFF9DDD-BCB3-4089-AD98-AE925ECFFB44"),
                Name = Resources.Language.SKD.SKDManager.LockControlHelper.Name,
                ShortName = Resources.Language.SKD.SKDManager.LockControlHelper.ShortName,
				DriverType = SKDDriverType.LockControl,
				IsPlaceable = true
			};
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			return driver;
		}
	}
}