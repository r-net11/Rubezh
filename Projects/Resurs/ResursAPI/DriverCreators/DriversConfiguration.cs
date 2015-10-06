﻿using System;
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
			Drivers.Add(BeregunInterfaceCreator.Create());
			Drivers.Add(BeregunCounterCreator.Create());
			Drivers.Add(MZEP55CounterCreator.Create());
			Drivers.Add(MZEP55InterfaceCreator.Create());
		}

		public static List<Driver> Drivers { get; private set; }

		public static Driver GetDriver(DriverType driverType)
		{
			var driver = Drivers.FirstOrDefault(x => x.DriverType == driverType);
			if (driver == null)
				throw new NullReferenceException();
			return driver;
		}
	}
}