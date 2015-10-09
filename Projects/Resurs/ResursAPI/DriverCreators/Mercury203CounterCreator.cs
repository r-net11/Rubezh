using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class Mercury203CounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.Mercury203Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesMercury203.GADDR,
				Description = "Групповой адрес",
				ParameterType = ParameterType.Int,
				IsReadOnly = true,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesMercury203.DateTime,
				Description = "Дата и время",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesMercury203.PowerLimit,
				Description = "Лимит мощности",
				ParameterType = ParameterType.Double,
				IsReadOnly = true,
				Number = 2
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = ParameterNamesMercury203.PowerLimitPerMonth,
				Description = "Месячный лимит мощности",
				ParameterType = ParameterType.Double,
				IsReadOnly = true,
				Number = 3
			});
			return driver;
		}
	}
}
