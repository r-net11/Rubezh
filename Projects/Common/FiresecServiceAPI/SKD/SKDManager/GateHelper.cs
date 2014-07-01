using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class GateHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("464B4AC5-B192-4890-9605-C41C6E3C883B"),
				Name = "Шлакбаум",
				ShortName = "Шлакбаум",
				DriverType = SKDDriverType.Gate,
				HasZone = true,
			};
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			return driver;
		}
	}
}