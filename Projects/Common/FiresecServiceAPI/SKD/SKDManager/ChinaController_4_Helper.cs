﻿using System;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ChinaController_4_Helper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("48A0C16E-DEA6-40d2-98B9-A21076BF6F37"),
				Name = "Контроллер на четыре двери",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.ChinaController_4,
				DefaultDoorType = DoorType.TwoWay,
				CanChangeDoorType = true,
				IsPlaceable = true,
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Reader, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Lock, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.LockControl, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Button, 4));

			driver.Properties = CommonChinaControllerHelper.CreateProperties();
			return driver;
		}
	}
}