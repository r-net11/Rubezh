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
			driver.DriverType = DriverType.System;
			driver.Children.Add(DriverType.BeregunCounter);
			driver.Children.Add(DriverType.BeregunInterface);
			driver.Children.Add(DriverType.MZEP55Counter);
			driver.Children.Add(DriverType.MZEP55Interface);
			return driver;
		}
	}
}