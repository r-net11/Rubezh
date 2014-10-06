using System;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class LockHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("578537D0-36A9-4916-BC8F-80CDEDC6AE15"),
				Name = "Замок",
				ShortName = "Замок",
				DriverType = SKDDriverType.Lock,
				IsPlaceable = true
			};
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			return driver;
		}
	}
}