using System;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	public static class LockHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("578537D0-36A9-4916-BC8F-80CDEDC6AE15"),
                Name = Resources.Language.SKD.SKDManager.LockHelper.Name,
                ShortName = Resources.Language.SKD.SKDManager.LockHelper.ShortName,
				DriverType = SKDDriverType.Lock,
				IsPlaceable = true
			};
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateClasses.Add(XStateClass.Off);
			driver.AvailableStateClasses.Add(XStateClass.Attention);

			return driver;
		}
	}
}