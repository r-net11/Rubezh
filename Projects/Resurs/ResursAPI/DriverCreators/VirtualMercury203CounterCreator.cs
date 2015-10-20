using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class VirtualMercury203CounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver(new Guid("5AFE758E-5659-4D28-B3E0-BF50F360851E"));
			driver.DriverType = DriverType.VirtualMercury203Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.CanEditTariffType = false;
			driver.DefaultTariffType = TariffType.Electricity;
			var realDriver = DriversConfiguration.GetDriver(DriverType.Mercury203Counter);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			driver.Commands.AddRange(realDriver.Commands);
			return driver;
		}
	}
}
