using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class Network1Device2Creator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.Network1Device2;
			return driver;
		}
	}
}