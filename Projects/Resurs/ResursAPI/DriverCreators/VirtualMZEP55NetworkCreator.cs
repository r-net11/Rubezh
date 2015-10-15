using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class VirtualMZEP55NetworkCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.VirtualMZEP55Network;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.VirtualMZEP55Counter);
			var realDriver = DriversConfiguration.GetDriver(DriverType.MZEP55Network);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			var index = realDriver.DriverParameters.Max(x => x.Number) + 1;
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesVirtualMZEP55Network.PollInterval,
				Description = "Интервал опроса",
				ParameterType = ParameterType.Int,
				Number = index++,
				IntMinValue = 1
			});
			return driver;
		}
	}
}
