using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class Network1Creator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.Network1;
			driver.Children.Add(DriverType.Network1Device1);
			driver.Children.Add(DriverType.Network1Device2);
			return driver;
		}
	}
}