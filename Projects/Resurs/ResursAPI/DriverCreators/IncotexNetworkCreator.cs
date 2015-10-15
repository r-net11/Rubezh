using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class IncotexNetworkCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.IncotextNetwork;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.Mercury203Counter);
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesIncotexNetwork.BautRate,
				Description = "Скорость обмена",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem { Name = "9600", Value = 9600 },
					new ParameterEnumItem { Name = "19200", Value = 19200 },
					new ParameterEnumItem { Name = "57600", Value = 57600 },
					new ParameterEnumItem { Name = "115200", Value = 115200 }
				},
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesIncotexNetwork.Timeout ,
				Description = "Таймаут ответа",
				ParameterType = ParameterType.Int,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesIncotexNetwork.BroadcastDelay,
				Description = "Задержка широковещательной команды",
				ParameterType = ParameterType.Int,
				Number = 2
			});
			return driver;
		}
	}
}
