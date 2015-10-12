using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class SystemCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DeviceType = DeviceType.System;
			driver.DriverType = DriverType.System;
			driver.Children.Add(DriverType.BeregunNetwork);
			driver.Children.Add(DriverType.MZEP55Network);
			driver.Children.Add(DriverType.IncotextNetwork);
			return driver;
		}
	}
}