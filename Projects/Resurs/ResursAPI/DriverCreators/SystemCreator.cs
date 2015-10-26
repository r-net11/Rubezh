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
			var driver = new Driver(new Guid("76EFAF4D-9D6F-4DCD-8A0A-B4A9EE983ED7"));
			driver.DeviceType = DeviceType.System;
			driver.DriverType = DriverType.System;
			driver.Children.Add(DriverType.BeregunNetwork);
			driver.Children.Add(DriverType.MZEP55Network);
			driver.Children.Add(DriverType.IncotextNetwork);
			driver.Children.Add(DriverType.VirtualIncotextNetwork);
			driver.Children.Add(DriverType.VirtualMZEP55Network);
			return driver;
		}
	}
}