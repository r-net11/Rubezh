using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class MZEP55InterfaceCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.MZEP55Interface;
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Скорость интерфейса",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem{ Name = "2400", Value = 0 },
					new ParameterEnumItem{ Name = "4800", Value = 1 },
					new ParameterEnumItem{ Name = "9600", Value = 2 },
					new ParameterEnumItem{ Name = "19200", Value = 3 },
				},
				EnumDefaultItem = 2,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter
			{
				Name = "Таймаут ответа, мс",
				ParameterType = ParameterType.Int,
				IntMinValue = 0,
				IntDefaultValue = 1,
				Number = 1
			});
			driver.Children.Add(DriverType.MZEP55Counter);
			return driver;
		}
	}
}
