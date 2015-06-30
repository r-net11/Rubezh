﻿using System;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ReaderHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("E54EC896-178E-46F0-844F-0B8BE1770F44"),
				Name = "Считыватель",
				ShortName = "Считыватель",
				DriverType = SKDDriverType.Reader,
				HasZone = true,
				IsPlaceable = true
			};
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			return driver;
		}
	}
}