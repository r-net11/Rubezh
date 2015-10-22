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
			var driver = new Driver(new Guid("6EB76465-1C8E-4B39-90EC-641B9EFF0717"));
			driver.DriverType = DriverType.VirtualIncotextNetwork;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.VirtualMercury203Counter);
			var realDriver = DriversConfiguration.GetDriver(DriverType.IncotextNetwork);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			var index = realDriver.DriverParameters.Max(x => x.Number) + 1;
			driver.DriverParameters.Add(new DriverParameter(new Guid("32EB321F-F39C-4144-A65C-44BEB850E7C7"))
			{
				Name = ParameterNamesIncotexNetworkVirtual.PollInterval,
				Description = "Интервал опроса",
				ParameterType = ParameterType.Int,
				Number = index++,
				IntMinValue = 1000,
				IntDefaultValue = 1000,
				CanWriteInActive = false
			});
			return driver;
		}
	}
}