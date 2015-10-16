using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class VirtualIncotextNetworkCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.VirtualIncotextNetwork;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.VirtualMercury203Counter);
			var realDriver = DriversConfiguration.GetDriver(DriverType.IncotextNetwork);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			var index = realDriver.DriverParameters.Max(x => x.Number) + 1;
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesIncotexNetworkVirtual.PollInterval,
				Description = "Интервал опроса",
				ParameterType = ParameterType.Int,
				Number = index++,
				IntMinValue = 1
			});
			return driver;
		}
	}
}