﻿using System;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ChinaController_2_4Helper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("2A2F8A93-E4A0-4799-9DE2-6FB58E035FE1"),
				Name = "Контроллер на две двери и четыре считывателя",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.ChinaController_2_4,
				DefaultDoorType = DoorType.TwoWay,
				CanChangeDoorType = true,
				IsPlaceable = true,
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Reader, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Lock, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.LockControl, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Button, 4));

			driver.Properties = CommonChinaControllerHelper.CreateProperties();
			return driver;
		}
	}
}