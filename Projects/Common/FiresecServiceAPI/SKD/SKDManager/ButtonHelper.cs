using System;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	public static class ButtonHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("5E4050C3-BDFA-4aeb-B66F-2BAD7F5CDF58"),
				Name = "Кнопка выход",
				ShortName = "Кнопка выход",
				DriverType = SKDDriverType.Button,
				IsPlaceable = true
			};
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			return driver;
		}
	}
}