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
			driver.Children.Add(DriverType.Network1);
			driver.Children.Add(DriverType.Network2);
			return driver;
		}
	}
}