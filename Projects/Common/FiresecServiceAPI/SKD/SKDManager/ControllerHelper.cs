using System;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ControllerHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("93AFFD73-ACA8-421e-BA5B-1E5D3E5113B4"),
				Name = "Контроллер СКД",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.Controller,
				IsPlaceable = true
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.Properties = CommonChinaControllerHelper.CreateProperties();
			return driver;
		}
	}
}