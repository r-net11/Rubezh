using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class Network2Creator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.Network2;
			driver.Children.Add(DriverType.Network2Device1);
			driver.Children.Add(DriverType.Network2Device2);
			driver.Children.Add(DriverType.Network2Device3);
			return driver;
		}
	}
}