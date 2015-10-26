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
			var driver = new Driver(new Guid("483CD536-D40B-41B0-8E03-4F519B48DEA7"));
			driver.DriverType = DriverType.IncotextNetwork;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.Mercury203Counter);
			driver.DriverParameters.Add(new DriverParameter(new Guid("E428194B-DBDA-4B44-B59A-5AB7438AD972"))
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
				Number = 0,
				EnumDefaultItem = 9600,
				CanWriteInActive = false
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("966542E0-1960-4463-A709-7B3B60ECD43D"))
			{
				Name = ParameterNamesIncotexNetwork.Timeout ,
				Description = "Таймаут ответа",
				ParameterType = ParameterType.Int,
				Number = 1,
				IntDefaultValue = 10,
				IntMinValue = 1,
				CanWriteInActive = false
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("C924C4B9-FB45-4668-A064-44E4CF1300E4"))
			{
				Name = ParameterNamesIncotexNetwork.BroadcastDelay,
				Description = "Задержка широковещательной команды",
				ParameterType = ParameterType.Int,
				Number = 2,
				IntDefaultValue = 10,
				IntMinValue = 1,
				CanWriteInActive = false
			});
			return driver;
		}
	}
}
