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
			var driver = new Driver();
			driver.DriverType = DriverType.VirtualMercury203Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.CanEditTariffType = false;
			driver.DefaultTariffType = TariffType.Electricity;
			var realDriver = DriversConfiguration.GetDriver(DriverType.Mercury203Counter);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			return driver;
		}
	}
}
