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
			driver.AddParameter("Скорость интерфейса", ParameterType.Enum, parameterEnumItems: new List<ParameterEnumItem>
				{
					new ParameterEnumItem{ Name = "2400", Value = 0 },
					new ParameterEnumItem{ Name = "4800", Value = 1 },
					new ParameterEnumItem{ Name = "9600", Value = 2 },
					new ParameterEnumItem{ Name = "19200", Value = 3 },
				});
			driver.AddParameter("Задержка при ожидании ответа, мс", ParameterType.Int);
			driver.Children.Add(DriverType.MZEP55Counter);
			return driver;
		}
	}
}
