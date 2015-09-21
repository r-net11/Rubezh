using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class DriversConfiguration
	{
		static DriversConfiguration()
		{
			Drivers = new List<Driver>();
			Drivers.Add(SystemCreator.Create());
			Drivers.Add(Network1Creator.Create());
			Drivers.Add(Network1Device1Creator.Create());
			Drivers.Add(Network1Device2Creator.Create());
			Drivers.Add(Network2Creator.Create());
			Drivers.Add(Network2Device1Creator.Create());
			Drivers.Add(Network2Device2Creator.Create());
			Drivers.Add(Network2Device3Creator.Create());
		}

		public static List<Driver> Drivers { get; private set; }
	}
}