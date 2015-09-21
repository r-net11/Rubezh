using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class Network2Device3Creator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.Network2Device3;
			return driver;
		}
	}
}