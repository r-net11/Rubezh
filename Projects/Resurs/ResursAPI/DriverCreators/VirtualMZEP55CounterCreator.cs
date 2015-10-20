using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class VirtualMZEP55CounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver(new Guid("5449277A-C174-4BC2-913F-E6B9BDB5DF88"));
			driver.DriverType = DriverType.VirtualMZEP55Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.CanEditTariffType = false;
			driver.DefaultTariffType = TariffType.Heat;
			var realDriver = DriversConfiguration.GetDriver(DriverType.MZEP55Counter);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			return driver;
		}
	}
}
